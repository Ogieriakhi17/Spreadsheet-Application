namespace SpreadsheetEngine;

public class TextChange : ICommand
{
    private static readonly string MessageText = "text change";
    private readonly string _changed;
    private readonly string _prev;
    private readonly AbstractCell _affectedCell;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextChange"/> class.
    /// </summary>
    /// <param name="cell">Cell that is affected.</param>
    /// <param name="prevVal">Previous text value.</param>
    /// <param name="changeToVal">Value that text is changing to.</param>
    public TextChange(AbstractCell cell, string prevVal, string changeToVal)
    {
        this._affectedCell = cell;
        this._prev = prevVal;
        this._changed = changeToVal;
    }

    /// <inheritdoc/>
    public void Execute()
    {
        this._affectedCell.Text = this._changed;
    }

    /// <inheritdoc/>
    public void Unexecute()
    {
        this._affectedCell.Text = this._prev;
    }

    /// <inheritdoc/>
    public string Message()
    {
        return MessageText;
    }
}