// <copyright file="MainWindow.axaml.cs" company="Osaze Ogieriakhi 11784953">
// Copyright (c) Osaze Ogieriakhi 11784953. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Platform.Storage;
using Spreadsheet_Osaze_Ogieriakhi.ViewModels;

namespace Spreadsheet_Osaze_Ogieriakhi.Views;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Spreadsheet_Osaze_Ogieriakhi.ViewModels;
using SpreadsheetEngine;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private readonly Spreadsheet spreadsheet;

    //private readonly List<CellViewModel> selectedCells = new();
    private readonly List<RowViewModel> rowsView;
    private bool isInitialized;

    /// <summary>
    /// Gets the 2D array that represents the logical spreadsheet.
    /// </summary>
    public CellViewModel[][] Rows { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    ///
    // private Spreadsheet SpreadsheetDataGrid = new Spreadsheet(50, 26);
    public MainWindow()
    {
       // this.WhenActivated(d =>
           // d(this.ViewModel!.AskForAColor.RegisterHandler(this.PickColor)));
       
       
       this.spreadsheet = new Spreadsheet(50, 'Z' - 'A' + 1);
       var rowCount = this.spreadsheet.RowCount;
       var columnCount = this.spreadsheet.ColumnCount;
       this.Rows = Enumerable.Range(0, rowCount)
           .Select(row => Enumerable.Range(0, columnCount)
               .Select(column => new CellViewModel(this.spreadsheet.GetCell(row, column))).ToArray())
           .ToArray();
       this.rowsView = new List<RowViewModel>();
       foreach (var col in this.Rows)
       {
           this.rowsView.Add(new RowViewModel(col.ToList()));
       }
       this.InitializeComponent();

       DataContextChanged += (sender, args) =>
        {
            if (this.DataContext is MainWindowViewModel viewModel)
            {
                this.InitializeDataGrid(this.Spreadsheet_Osaze, viewModel);
                viewModel.InitializeSpreadsheet(this.rowsView, this.spreadsheet);
            }
        };
       
        this.spreadsheet.SpreadsheetPropertyChanged += this.OnSpreadsheetPropertyChanged;
    }

        /// <summary>
    /// Method to initialize the spreadsheet for the UI layer.
    /// </summary>
    /// <param name="dataGrid"></param>
    public void InitializeDataGrid(DataGrid dataGrid, MainWindowViewModel viewModel)
    {
        if (this.isInitialized)
        {
            return;
        }

        // initialize A to Z columns headers since these are indexed this is not a behavior supported by default
        var columnCount = 'Z' - 'A' + 1;
        foreach (var columnIndex in Enumerable.Range(0, columnCount))
        {
            // for each column we will define the header text and
            // the binding to use
            var columnHeader = (char)('A' + columnIndex);
            var columnTemplate = new DataGridTemplateColumn
            {
                Header = columnHeader,
                CellStyleClasses = { "SpreadsheetCellClass" },
                CellTemplate = new
                    FuncDataTemplate<RowViewModel>((value, namescope) =>
                        new TextBlock
                        {
                            [!TextBlock.TextProperty] =
                                new Binding($"[{columnIndex}].Value"),
                            TextAlignment = TextAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,
                            Padding = Thickness.Parse("5,0,5,0"),
                        }),
                CellEditingTemplate = new
                    FuncDataTemplate<RowViewModel>((value, namescope) =>
                        new TextBox()),
            };
            dataGrid.Columns.Add(columnTemplate);
        }

        dataGrid.ItemsSource = this.rowsView;
        dataGrid.LoadingRow += (sender, args) => { args.Row.Header = (args.Row.GetIndex() + 1).ToString(); };
        this.isInitialized = true;
        
        dataGrid.PreparingCellForEdit += (sender, args) =>
        {
            if (args.EditingElement is not TextBox textInput)
            {
                return;

            }
            var rowIndex = args.Row.GetIndex();
            var columnIndex = args.Column.DisplayIndex;
            textInput.Text = viewModel.GetCellText(
                rowIndex,
                columnIndex);
        };

        dataGrid.CellEditEnding += (sender, args) =>
        {
            if (args.EditingElement is not TextBox textInput) return;
            var rowIndex = args.Row.GetIndex();
            var columnIndex = args.Column.DisplayIndex;
            viewModel.SetCellText(rowIndex, columnIndex,
                textInput.Text);
        };
        // we use the following event to write our own selection logic
        dataGrid.CellPointerPressed += (sender, args) =>
        {
            // get the pressed cell
            var rowIndex = args.Row.GetIndex();
            var columnIndex = args.Column.DisplayIndex;
            // are we selected multiple cells
            var multipleSelection =
                args.PointerPressedEventArgs.KeyModifiers != KeyModifiers.None;
            if (multipleSelection == false)
            {
                viewModel.SelectCell(rowIndex, columnIndex);
            }
            else
            {
                viewModel.ToggleCellSelection(rowIndex, columnIndex);
            }
        };

        dataGrid.BeginningEdit += (sender, args) =>
        {
            // get the pressed cell
            var rowIndex = args.Row.GetIndex();
            var columnIndex = args.Column.DisplayIndex;
            var cell = viewModel.GetCellModel(rowIndex, columnIndex);
            if (cell.CanEdit == false)
            {
                args.Cancel = true;
            }
            else
            {
                viewModel.ResetSelection();
            }
        };
        
    }
    private async Task SaveButtonFile(InteractionContext<Unit, string?> interaction)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Spreadsheet",
        });
        if (file == null)
        {
            return;
        }

        string filePath = file.Path.AbsolutePath;
        interaction.SetOutput(filePath);
    }
    
    [Obsolete("Obsolete")]
    private async Task PickColor(InteractionContext<Unit, uint?> interaction)
    {
        var colorPicker = new ColorPicker();
        var stackPanel = new StackPanel();
        stackPanel.Children.Add(colorPicker);
        Window dialog = new Window
        {
            Content = stackPanel,
            Title = "Choose background color",
            Width = 300,
            Height = 200,
        };
        await dialog.ShowDialog(this);
        interaction.SetOutput(colorPicker.Color.ToUint32());
    }
    
    /// <summary>
    /// For updating the spreadsheet value.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSpreadsheetPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != null)
        {
            if (e.PropertyName.StartsWith("Cell_"))
            {
                // Extract cell indices from property name
                var indices = e.PropertyName.Split('_').Skip(1).Select(int.Parse).ToArray();
                int rowIndex = indices[0];
                int colIndex = indices[1];

                // Notify UI that the corresponding cell value has changed
                //RaisePropertyChanged(this.Rows[rowIndex][colIndex].Value,this.Rows[rowIndex][colIndex].Value, this.Rows[rowIndex][colIndex].Value);
            }
        }
    }
}
