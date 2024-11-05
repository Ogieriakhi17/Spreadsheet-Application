// <copyright file="RowViewModelToBrushConverter.cs" company="Osaze Ogieriakhi">
// Copyright (c) Osaze Ogieriakhi. All rights reserved.
// </copyright>

namespace Spreadsheet_GettingStarted.ValueConverters;

using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Spreadsheet_Osaze_Ogieriakhi;

/// <summary>
/// Class for converting the brush to Row View Model.
/// </summary>
public class RowViewModelToIBrushConverter : IValueConverter
{
    /// <summary>
    /// Instance of the brush converter.
    /// </summary>
    public static readonly RowViewModelToIBrushConverter Instance = new();
    private RowViewModel? currentRow;
    private int cellCounter;

    /// <summary>
    /// Convert method.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns>the brush for changing cell color</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // if the converter used for the wrong type throw an exception
        if (value is not RowViewModel row)
        {
            return new BindingNotification(
                new InvalidCastException(),
                BindingErrorType.Error);
        }

        // NOTE: Rows are rendered from column 0 to n and in order
        if (this.currentRow != row)
        {
            this.currentRow = row;
            this.cellCounter = 0;
        }

        var brush = this.currentRow.Cells[this.cellCounter].IsSelected
            ? new SolidColorBrush(0xff3393df)
            : new SolidColorBrush(this.currentRow.Cells[this.cellCounter].BackgroundColor);
        this.cellCounter++;
        if (this.cellCounter >= this.currentRow.Cells.Count)
        {
            this.currentRow = null;
        }

        return brush;
    }

    /// <summary>
    /// Convert back method.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns>void.</returns>
    /// <exception cref="NotImplementedException">To show if it has been implemented.</exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}