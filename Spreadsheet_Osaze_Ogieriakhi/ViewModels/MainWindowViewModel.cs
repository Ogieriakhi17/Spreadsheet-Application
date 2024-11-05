// <copyright file="MainWindowViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Reactive.Linq;
using System.Threading.Tasks;

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.

namespace Spreadsheet_Osaze_Ogieriakhi.ViewModels;

using System.IO;
using System.Reactive;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;
using SpreadsheetEngine;

/// <inheritdoc />
public class MainWindowViewModel : ViewModelBase
{
    private readonly List<CellViewModel> selectedCells = new();
    private Spreadsheet demo;
    private List<RowViewModel> spreadsheetData;

    private bool _isUndoReady;
    private bool _isRedoReady;
    private string _uMessage;
    private string _rMessage;
    /// <summary>
    /// Gets Interaction for asking a file to save.
    /// </summary>
    public Interaction<Unit, uint?> AskForAColor { get; }

    /// <summary>
    /// Gets Interaction for asking a file to save.
    /// </summary>
    public Interaction<Unit, string?> AskForFileToLoad { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// MainWindowViewModel constructor.
    /// </summary>
    public MainWindowViewModel()
    {
        this.AskForFileToLoad = new Interaction<Unit, string?>();
        this.AskForAColor = new Interaction<Unit, uint?>();
        this.IsUndoReady = false;
        this.IsRedoReady = false;
        this._uMessage = "Undo";
        this._rMessage = "Redo";
        //this.demo.SpreadsheetPropertyChanged += this.OnSpreadsheetPropertyChanged;
    }
    
    public bool IsUndoReady
    {
        get
        {
            return this._isUndoReady;
        }

        set
        {
            if (this._isUndoReady != value)
            {
                this.RaiseAndSetIfChanged(ref this._isUndoReady, value); // Notify property changed
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether redo can be preformed.
    /// </summary>
    public bool IsRedoReady
    {
        get
        {
            return this._isRedoReady;
        }

        set
        {
            if (this._isRedoReady != value)
            {
                this.RaiseAndSetIfChanged(ref this._isRedoReady, value);
            }
        }
    }

    /// <summary>
    /// Gets or sets message to display in undo menu header.
    /// </summary>
    public string UndoMessage
    {
        get => this._uMessage;

        set
        {
            if (this._uMessage != value)
            {
                this.RaiseAndSetIfChanged(ref this._uMessage, value); // Notify property changed
            }
        }
    }

    /// <summary>
    /// Gets or sets message to display in redo menu header.
    /// </summary>
    public string RedoMessage
    {
        get => this._rMessage;

        set
        {
            if (this._rMessage != value)
            {
                this.RaiseAndSetIfChanged(ref this._rMessage, value); // Notify property changed
            }
        }
    }

    /// <summary>
    /// Initializes spreadsheet objects with objects already created.
    /// </summary>
    /// <param name="sheetRows"> Used for RowViewModel.</param>
    /// <param name="sheet"> Used for spreadsheet object.</param>
    public void InitializeSpreadsheet(List<RowViewModel> sheetRows, Spreadsheet sheet)
    {
        this.spreadsheetData = sheetRows;
        this.demo = sheet;
    }

    /// <summary>
    /// This is a method to select a cell in the spreadsheet.
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="columnIndex"></param>
    public void SelectCell(int rowIndex, int columnIndex)
    {

        var clickedCell = this.GetCellModel(rowIndex, columnIndex);
       // CellViewModel clickedCell = new CellViewModel(demo.GetCell(rowIndex, columnIndex));
        var shouldEditCell = clickedCell.IsSelected;
        this.ResetSelection();

        // add the pressed cell back to the list
        this.selectedCells?.Add(clickedCell);
        clickedCell.IsSelected = true;
        if (shouldEditCell)
        {
            clickedCell.CanEdit = true;
        }
    }

    /// <summary>
    /// Method designed to support the cell selection functionality.
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="columnIndex"></param>
    public void ToggleCellSelection(int rowIndex, int columnIndex)
    {
        var clickedCell = this.GetCellModel(rowIndex, columnIndex);
       // var clickedCell = new CellViewModel(demo.GetCell(rowIndex, columnIndex));
        if (clickedCell.IsSelected == false)
        {
            this.selectedCells.Add(clickedCell);
            clickedCell.IsSelected = true;
        }
        else
        {
            this.selectedCells.Remove(clickedCell);
            clickedCell.IsSelected = false;
        }
    }

    /// <summary>
    /// Method designed to assist in the undo and redo functionality.
    /// </summary>
    public void ResetSelection()
    {
        // clear current selection
        foreach (var cell in this.selectedCells)
        {
            cell.IsSelected = false;
            cell.CanEdit = false;
        }

        this.selectedCells.Clear();
    }



    /// <summary>
    /// Method defined to load the a spreadsheet from a file when clicked.
    /// </summary>
    public void LoadFromFile()
    {
        string fileName = "spreadsheetData.xml";
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        using (Stream stream = File.OpenRead(filePath))
        {
            this.demo.LoadSpreadsheet(stream); // Load XML contents into the spreadSheet
        }
    }

    /// <summary>
    /// To save the present spreadsheet being worked on.
    /// </summary>
    public void SaveToFile()
    {
        string fileName = "SpreadsheetData.xml";

        // Combine the file name with the current directory to get the full path
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        // Create a new XML file
        using (Stream stream = File.Create(filePath))
        {
            // Save the spreadsheet data to the XML file
            this.demo.SaveSpreadsheet(stream);
        }
    }

    /// <summary>
    /// Method to test the functionality of the referencing and updating of the spreadsheet.
    /// </summary>
    public void RunDemo()
    {
        /*
        this.demo.GetCell(0, 0).Text = "=A1"; // A1
        this.demo.GetCell(0, 1).Text = "=B2*3"; // B1
        this.demo.GetCell(1, 1).Text = "=A2*3"; // B2
        this.demo.GetCell(1, 0).Text = "=A1*5"; // A2
        */
    }

    public CellViewModel GetCellModel(int row, int col)
    {
        return this.spreadsheetData[row][col];
    }

    
    
    /// <summary>
    /// Gets cell at row and col.
    /// </summary>
    /// <param name="row"> Row of cell added.</param>
    /// <param name="col"> column of cell added.</param>
    /// <param name="val"> string that text should be set to.</param>
    public void SetCellText(int row, int col, string val)
    {
       this.demo.RequestTextChange(this.GetCellModel(row, col).Cell, val);
       this.IsRedoReady = false;
       this.IsUndoReady = true;
       this.UpdateMessages();
    }
    
    /// <summary>
    /// Gets cell text at row and col.
    /// </summary>
    /// <returns>Cell at index.</returns>
    /// <param name="row"> Row of cell added.</param>
    /// <param name="col"> column of cell added.</param>
    public string GetCellText(int row, int col)
    {
        return this.spreadsheetData[row][col].Text;
    }
    
    /// <summary>
    /// Task for asking for a color.
    /// </summary>
    /// <returns> Nothing.</returns>
    public async Task ColorPicker()
    {
        if (this.selectedCells.Count == 0)
        {
            return;
        }

        // Wait for the user to select the file to load from.
        var color = await this.AskForAColor.Handle(default);
        if (color != null)
        {
            List<AbstractCell> cells = this.selectedCells.Select(vm => vm.Cell).ToList();
            this.demo.RequestColorChange(cells, color.Value); // change to colo
            this.IsRedoReady = false;
            this.IsUndoReady = true;
            this.UpdateMessages();
        }
    }
    /// <summary>
    /// Undoes the last command given to the spreadsheet.
    /// </summary>
    public void UndoCommand()
    {
        this.demo.Undo();
        this.IsUndoReady = !this.demo.EmptyUndo();
        this.IsRedoReady = true;
        this.UpdateMessages();
    }

    /// <summary>
    /// Redoes the last command undone to the spreadsheet.
    /// </summary>
    public void RedoCommand()
    {
        this.demo.Redo();
        this.IsRedoReady = !this.demo.EmptyRedo();
        this.IsUndoReady = true;
        this.UpdateMessages();
    }

    /// <summary>
    /// Gets or sets message to display in undo menu header.
    /// </summary>
    private void UpdateMessages()
    {
        this.UndoMessage = this.demo.GetUndoMessage();
        this.RedoMessage = this.demo.GetRedoMessage();
    }
}

