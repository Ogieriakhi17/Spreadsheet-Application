namespace SpreadsheetEngine;

public class ColorChange : ICommand
{
    private static readonly string MessageText = "color change";
    private readonly List<AbstractCell> _affectedCells;
    private readonly List<uint> _prev;
    private readonly uint _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorChange"/> class.
    /// </summary>
    /// <param name="cells">Cells that are changed.</param>
    /// <param name="prev">List of colors for all previous cells.</param>
    /// <param name="next">Color they are all changing to.</param>
    public ColorChange(List<AbstractCell> cells, List<uint> prev, uint next)
    {
        this._prev = prev;
        this._next = next;
        this._affectedCells = new List<AbstractCell>(cells);
    }

    /// <inheritdoc/>
    public void Execute()
    {
        foreach (AbstractCell item in this._affectedCells)
        {
            item.BGColor = this._next;
        }
    }

    /// <inheritdoc/>
    public void Unexecute()
    {
        for (int i = 0; i < this._affectedCells.Count; i++)
        {
            this._affectedCells[i].BGColor = this._prev[i];
        }
    }

    /// <inheritdoc/>
    public string Message()
    {
        return MessageText;
    }
}