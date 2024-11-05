// <copyright file="Spreadsheet.cs" company="Osaze Ogieriakhi 11784953">
// Copyright (c) Osaze Ogieriakhi 11784953. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml;

namespace SpreadsheetEngine;

/// <summary>
/// The spreadsheet class that manages the cells in the logic layer.
/// </summary>
public class Spreadsheet
{
    /// <summary>
    /// An array of cells for the spreadsheet.
    /// </summary>
    private readonly Cell[,] cells;

    private Stack<ICommand> redo;
    private Stack<ICommand> undo;

    /// <summary>
    /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
    /// </summary>
    /// <param name="rowCount"></param>
    /// <param name="columnCount"></param>
    public Spreadsheet(int rowCount, int columnCount)
    {

        this.redo = new Stack<ICommand>();
        this.undo = new Stack<ICommand>();
        this.RowCount = rowCount;
        this.ColumnCount = columnCount;

        // Initialize the 2D array of cells
        this.cells = new Cell[rowCount, columnCount];

        // Create cells and subscribe to PropertyChanged event
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < columnCount; col++)
            {
                this.cells[row, col] = new Cell(row, col);
                this.cells[row, col].PropertyChanged += this.OnCellPropertyChanged;
            }
        }
    }

    /// <summary>
    /// This is the event handler that notifies the UI that the spreadsheet has changed.
    /// </summary>
    public event PropertyChangedEventHandler? SpreadsheetPropertyChanged;

    // public event PropertyChangedEventHandler? BaseCellPropertyChanged;

    /// <summary>
    /// Gets the size Parameters for the Spreadsheet to be Columns.
    /// </summary>
    public int ColumnCount { get; }

    /// <summary>
    /// Gets the size Parameters for the Spreadsheet to be Rows.
    /// </summary>
    public int RowCount { get; }

    /// <summary>
    /// Method designed to access a cell in a spreadhseet using
    /// it coordinates (row and column index).
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="columnIndex"></param>
    /// <returns>It returns the cell as a Cell.</returns>
    public AbstractCell? GetCell(int rowIndex, int columnIndex)
    {
        if (rowIndex >= 0 && rowIndex < this.RowCount && columnIndex >= 0 && columnIndex < this.ColumnCount)
        {
            return this.cells[rowIndex, columnIndex];
        }

        return null;
    }
    
    /// <summary>
    /// Undo on spreadsheet.
    /// </summary>
    public void Undo()
    {
        if (this.undo.Count == 0)
        {
            return;
        }

        var temp = this.undo.Pop();
        temp.Unexecute();
        this.redo.Push(temp);
    }

    /// <summary>
    /// Redo on spreadsheet.
    /// </summary>
    public void Redo()
    {
        // Should be impossible for count to be 0, since I disable the button is the stack is empty.
        if (this.redo.Count == 0)
        {
            return;
        }

        var temp = this.redo.Pop();
        temp.Execute();
        this.undo.Push(temp);
    }
    
    /// <summary>
    /// Color change for a list of cells. Empty lists should be
    /// handled before this method is called so that a command
    /// that changes no cells is not called.
    /// </summary>
    /// /// <param name="cellsChanged">List of cells to be changed.</param>
    /// /// <param name="next">Color to change to.</param>
    public void RequestColorChange(List<AbstractCell> cellsChanged, uint next)
    {
        List<uint> prev = new List<uint>();
        foreach (Cell cell in cellsChanged)
        {
            prev.Add(cell.BGColor);
        }

        ICommand temp = new ColorChange(cellsChanged, prev, next);
        temp.Execute();
        this.undo.Push(temp);
        this.redo.Clear();
    }
    
    public void RequestTextChange(AbstractCell cellChanged, string next)
    {
        string prev = cellChanged.Text;
        ICommand temp = new TextChange(cellChanged, prev, next);
        temp.Execute();
        this.undo.Push(temp);
        this.redo.Clear();
    }
    
    
    /// <summary>
    /// Gets message from command at top of redo stack.
    /// </summary>
    /// <returns>Message from command at top of stack, if nothing in stack returns just redo.</returns>>
    public string GetRedoMessage()
    {
        if (this.redo.TryPeek(out ICommand? output))
        {
            return "Redo " + output.Message();
        }

        return "Redo";
    }

    /// <summary>
    /// Gets message from command at top of undo stack.
    /// </summary>
    /// <returns>Message from command at top of stack, if nothing in stack returns just Undo.</returns>>
    public string GetUndoMessage()
    {
        if (this.undo.TryPeek(out ICommand? output))
        {
            return "Undo " + output.Message();
        }

        return "Undo";
    }

    /// <summary>
    /// Check if undo stack is empty.
    /// </summary>
    /// <returns>True if stack is empty, otherwise false..</returns>>
    public bool EmptyUndo()
    {
        if (this.undo.Count == 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if redo stack is empty.
    /// </summary>
    /// <returns>True if stack is empty, otherwise false..</returns>>
    public bool EmptyRedo()
    {
        if (this.redo.Count == 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Method to save the spreadsheet from the filepath passed through.
    /// </summary>
    /// <param name="outputStream"></param>
    public void SaveSpreadsheet(Stream outputStream)
    {
        XmlWriter writer = XmlWriter.Create(outputStream);
        writer.WriteStartDocument();
        writer.WriteStartElement("Spreadsheet");

        for (int col = 0; col < this.ColumnCount; col++)
        {
            for (int row = 0; row < this.RowCount; ++row)
            {
                AbstractCell? presentCell = this.GetCell(row, col);

                if (presentCell?.Text != null || presentCell?.Value != null)
                {
                    writer.WriteStartElement("Cell");

                   // writer.WriteAttributeString("name", presentCell.Name);
                    writer.WriteAttributeString("row", presentCell.RowIndex.ToString());
                    writer.WriteAttributeString("column", presentCell.ColumnIndex.ToString());
                    writer.WriteAttributeString("text", presentCell.Text);
                    writer.WriteAttributeString("value", presentCell.Value);

                    writer.WriteEndElement();
                }
            }
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Close();
    }

    /// <summary>
    /// To load the spreadsheet.
    /// </summary>
    /// <param name="stream"></param>
    public void LoadSpreadsheet(Stream stream)
    {
        this.ClearSpreadsheet();
        XmlReader reader = XmlReader.Create(stream);

        // Read the root element
        reader.ReadStartElement("Spreadsheet");

        while (reader.IsStartElement("Cell"))
        {
            // Extract cell attributes
            // string cellName = reader.GetAttribute("name");
#pragma warning disable CS8604 // Possible null reference argument.
            int row = int.Parse(reader.GetAttribute("row"));
            int col = int.Parse(reader.GetAttribute("column"));
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string? text = reader.GetAttribute("text");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string? value = reader.GetAttribute("value");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8604 // Possible null reference argument.
            // Load cell if all necessary data is available
            if (row != -1 && col != -1)
            {
                Cell cell = new Cell(row, col);
                this.LoadCell(cell, row, col, value, text);
            }

            // Move to the next cell
            reader.ReadStartElement("Cell");
        }

        // Read the end of the root element
        reader.ReadEndElement();

        // Close the reader
        reader.Close();
    }

    /// <summary>
    /// Function to get a cell name by passing in its location.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns>A string representing the name.</returns>
    public string GetCellName(int row, int column)
    {
        // Convert the column number to a letter
        char columnLetter = (char)('A' + column);

        // Convert the row number to its string representation and add 1 (since row numbers are 0-indexed)
        int rowNumber = row + 1;

        // Concatenate the column letter and row number to form the cell name
        return $"{columnLetter}{rowNumber}";
    }
    
    /*
    /// <summary>
    /// Method designed to subscribe to the change of a cell.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnCellPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // if(e.PropertyName == "BGColor")  then change the color of the ui
        if (sender is AbstractCell cell)
        {
            if (cell.Text != null && cell.Text[0] == '=')
            {
                string expression = cell.Text.Substring(1);

                ExpressionTree tree = new ExpressionTree(expression);
                this.GetVariables(expression, tree);
                string cellName = this.GetCellName(cell.RowIndex, cell.ColumnIndex);

                int check = this.CheckReferences(cell, cellName, tree);

                switch (check)
                {
                    case 0: cell.Value = tree.Evaluate().ToString(CultureInfo.CurrentCulture);
                        break;
                    case 1: cell.Value = "!(selfReference)";
                        break;
                    case 2: cell.Value = "!(circularReference)";
                        break;
                    case 3:
                        cell.Value = "!(badReference)";
                        break;
                }
            }
            else
            {
                cell.Value = cell.Text;
            }

            // UpdateDependentCells(cell);
            this.SpreadsheetPropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs($"Cell_{cell.RowIndex}_{cell.ColumnIndex}"));
        }
    }
    */

    /// <summary>
    /// Gets the column count.
    /// </summary>
    /// <param name="sender"> Integer to use for indexing the Cell row.</param>
    /// <param name="e"> Integer to use for indexing the Cell .</param>
    protected virtual void OnCellPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is Cell)
        {
            this.EvaluateCellValue((Cell)sender);
        }

        if (sender is Cell cell)
        {
            foreach (var item in cell.ReferencedBy)
            {
                this.EvaluateCellValue(item);
            }
        }
    }

    private void EvaluateCellValue(AbstractCell a)
    {
        Cell changeCell = (Cell)a;
        if (changeCell.Text.Length == 0)
        {
            changeCell.Value = changeCell.Text;
            return;
        }

        if (changeCell.Text[0] == '=')
        {
            ExpressionTree tree;
            try
            {
                tree = new ExpressionTree(changeCell.Text[1..]);
            }
            catch (Exception)
            {
                // Error in expression tree creation, must be operand missing since there is no other error that can occur here.
                changeCell.Value = "Operator Error";
                return;
            }

            GetVariables(changeCell.Text[1..], tree);
            // Remove no longer referenced.
            foreach (var item in changeCell.RefrencedTo.ToList())
            {
                
                if (!tree.variables.ContainsKey((char)(item.ColumnIndex + 'A') + (item.RowIndex + 1).ToString()))
                {
                    // Remove the reference from the referencedTo list of changeCell
                    changeCell.RefrencedTo.Remove(item);

                    // Remove changeCell from the referencedBy list of the item in the tree
                    if (item is Cell concreteItem)
                    {
                        concreteItem.ReferencedBy.Remove(changeCell);
                    }
                }
            }

            foreach (var item in tree.GetVariables())
            {
                try
                {
                    AbstractCell? ab = this.GetCell(int.Parse(item.Substring(1)) - 1, item[0] - 'A') ?? null;
                    if (ab == null)
                    {
                        throw new Exception();
                    }

                    string test = ab.Value;
                    if (ab is Cell ex)
                    {
                        if (!ex.ReferencedBy.Contains(changeCell))
                        {
                            ex.ReferencedBy.Add(changeCell);
                            changeCell.RefrencedTo.Add(ex);
                        }
                    }

                    if (double.TryParse(test, out var value))
                    {
                        tree.SetVariable(item, value);
                    }
                }
                catch (Exception)
                {
                    // Error in finding the cell, cell reference must be wrong or some string is input that is not a cell.
                    changeCell.Value = "Cell Reference Error";
                    return;
                }
            }

            try
            {
                changeCell.Value = tree.Evaluate().ToString(CultureInfo.InvariantCulture);
            }
            catch (InvalidOperationException)
            {
                changeCell.Value = "##";
            }
        }
        else
        {
            changeCell.Value = changeCell.Text;
        }
    }

    /// <summary>
    /// Function designed to find a cell referenced by it names,
    /// returning the named cell as a "Cell" type.
    /// </summary>
    /// <param name="cellName"></param>
    /// <returns>The desired cell.</returns>
    private AbstractCell? GetCellByName(string cellName)
    {
        if (Regex.IsMatch(cellName, @"^[A-Z]\d+$"))
        {
            // Extract the column letter and row number
            char columnLetter = cellName[0];
            int rowNumber = int.Parse(cellName.Substring(1));

            // Convert the column letter to a 0-based index
            int columnIndex = columnLetter - 'A';

            // Check if the row and column indices are within bounds
            if (rowNumber > 0 && columnIndex >= 0 && columnIndex < this.ColumnCount && rowNumber <= this.RowCount)
            {
                // Return the cell at the specified indices
                return this.GetCell(rowNumber - 1, columnIndex);
            }
        }

        return null; // Return null if the cell is not found
    }

    /// <summary>
    /// Clears the entire spreadsheet.
    /// </summary>
    private void ClearSpreadsheet()
    {
        foreach (var cell in this.cells)
        {
            cell.Text = null;
        }
    }

    /// <summary>
    /// To check and see if a Cell Text has a circular reference.
    /// </summary>
    /// <param name="cellName"></param>
    /// <param name="cellText"></param>
    /// <param name="visitedCells"></param>
    /// <returns>A bool whether or not there is a circular reference.</returns>
    private bool DetectCircularReference(string cellName, string? cellText, List<string> visitedCells)
    {
        // Check if the current cell name is already in the list of visited cells
        if (visitedCells.Contains(cellName))
        {
            return true; // Circular reference detected
        }

        // Add the current cell name to the list of visited cells
        visitedCells.Add(cellName);

        // Extract cell references from the cell text
        List<string> cellReferences = this.ExtractCellReferences(cellText);

        foreach (var reference in cellReferences)
        {
            AbstractCell? celll = this.GetCellByName(reference);
            string? cellTexxt = celll?.Text;

            // Check if the reference itself has circular references
            if (this.DetectCircularReference(reference, cellTexxt, visitedCells))
            {
                return true; // Circular reference detected
            }
        }

        // If no circular references were found for the current cell, remove it from the list
        visitedCells.Remove(cellName);

        // No circular references found
        return false;
    }

    /// <summary>
    /// Method to remove all cells referenced in a Cell's texts.
    /// </summary>
    /// <param name="cellText"></param>
    /// <returns>a List of cells that are being referenced by a cell.</returns>
    private List<string> ExtractCellReferences(string? cellText)
    {
        List<string> result = new List<string>();
        int i = 0;
        while (i < cellText.Length - 1)
        {
            if (char.IsLetter(cellText[i]))
            {
                string cellName = cellText[i].ToString() + cellText[i + 1].ToString();
                result.Add(cellName);
                i += 2; // Increment by 2 since we processed two characters
            }
            else
            {
                i++;
            }
        }

        return result;
    }

    /// <summary>
    /// Checks to see a cell referenced is incorrect.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>a bool.</returns>
    private bool ContainsInvalidCellReference(string? expression)
    {
        // Regular expression pattern to match cell references (e.g., A1, B2, Z123)
        string pattern = @"\b[A-Z]+\d+\b";

        // Match cell references in the expression
        MatchCollection matches = Regex.Matches(expression, pattern);

        // Check each matched cell reference
        foreach (Match match in matches)
        {
            string cellReference = match.Value;

            // Extract row and column indices from the cell reference
            string columnLetters = Regex.Match(cellReference, "[A-Z]+").Value;
            int rowIndex = int.Parse(Regex.Match(cellReference, @"\d+").Value);
            columnLetters = columnLetters.ToUpper();

            // Check if column letters are within the valid range (A to Z)
            if (columnLetters.Any(c => c < 'A' || c > 'Z'))
            {
                return true; // Invalid column letters
            }

            // Convert column letters to a numerical index
            int columnIndex = 0;
            for (int i = 0; i < columnLetters.Length; i++)
            {
                columnIndex = (columnIndex * 26) + (columnLetters[i] - 'A' + 1);
            }

            // Check if the row and column indices are within the valid range of the spreadsheet
            if (rowIndex <= 0 || columnIndex <= 0 ||
                rowIndex > this.RowCount || columnIndex > this.ColumnCount)
            {
                return true; // Out of range row or column index
            }
        }

        return false; // No invalid cell references found
    }

    /// <summary>
    /// To check whether formulas inputted are in the right format and their references.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="cellName"></param>
    /// <param name="tree"></param>
    /// <returns>a number determining if it has any violations.</returns>
    private int CheckReferences(AbstractCell cell, string cellName, ExpressionTree tree)
    {
        int result;

        List<string> dependentCells = new List<string>();
        if (tree.variables.ContainsKey(cellName))
        {
            result = 1; // it is a self reference "!(selfReference)";
            return result;
        }

        if (this.ContainsInvalidCellReference(cell.Text))
        {
            result = 3;
            return result;
        }
        else if (this.DetectCircularReference(cellName, cell.Text, dependentCells))
        {
            result = 2; // a circular reference was detected.
        }
        else
        {
            result = 0;
        }

        return result;
    }

    /// <summary>
    /// To directly load the cell into the spreadsheet.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="value"></param>
    /// <param name="text"></param>
    private void LoadCell(Cell cell, int row, int col, string? value, string? text)
    {
        // cell.BgColor = bgcolor;
        AbstractCell? celler = cell;
        celler.Text = null;
        this.cells[row, col].Text = text;

        // cell.Text = text;
        this.cells[row, col].Value = value;
    }

    /// <summary>
    /// Method for getting the variables present in the text of a cell.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="tree"></param>
    private void GetVariables(string text, ExpressionTree tree)
    {
        int i = 0;
        while (i < text.Length - 1)
        {
            if (char.IsLetter(text[i]))
            {
                string cellName = text[i].ToString() + text[i + 1].ToString();

                AbstractCell? cell = this.GetCellByName(cellName);
                if (cell != null)
                {
                    tree.SetVariable(cellName, Convert.ToDouble(cell.Value));
                }

                i += 2; // Increment by 2 since we processed two characters
            }
            else
            {
                i++;
            }
        }
    }

    /// <summary>
    /// Concrete cell class for which the spreadsheet class works with.
    /// </summary>
    private class Cell : AbstractCell
    {
        /// <summary>
        /// List of Cells that reference this cell.
        /// </summary>
        internal readonly List<AbstractCell> ReferencedBy = new List<AbstractCell>();

        /// <summary>
        /// CList of Cells that this cell references.
        /// </summary>
        internal List<AbstractCell> RefrencedTo = new List<AbstractCell>();
        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        public Cell(int rowIndex, int colIndex)
            : base(rowIndex, colIndex)
        {
        }

        /// <inheritdoc/>
        public override string? Value
        {
            get => this.valuue;
            protected internal set
            {
                if (this.valuue == value)
                {
                    return;
                }
                
                this.valuue = value;
                this.OnPropertyChanged(nameof(this.Value));
            }
        }
    }
}