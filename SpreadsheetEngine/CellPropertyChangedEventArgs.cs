namespace SpreadsheetEngine;

    public class CellPropertyChangedEventArgs : EventArgs
    {
        public AbstractCell Cell { get; }
        public string PropertyName { get; }

        public CellPropertyChangedEventArgs(AbstractCell cell, string propertyName)
        {
            Cell = cell ?? throw new ArgumentNullException(nameof(cell));
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }
    }
