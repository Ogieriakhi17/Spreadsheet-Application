using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SpreadsheetEngine;

    public abstract class AbstractCell : INotifyPropertyChanged
    {
        protected string? text;
        protected string? valuue;
        protected uint bGColor;
        
        private readonly int rowIndex, columnIndex;

        public int RowIndex
        {
            get { return rowIndex; }
        }

        public int ColumnIndex 
        {
            get { return columnIndex; }
        }

        public string? Text
        {
            get => text;
            set => this.SetField(ref text, value);
        }

        public abstract string? Value
        {

            get; //=> valuue;
           protected internal set; // => this.SetField(ref valuue, value);
        }

        /*NAME: ABSTRACT CELL
        *DESCRIPTION: Constructor for the Abstract Cell class
        *
        *PAREMETERS: NONE
        *POSTCONDITONS:NONE
        */
        public AbstractCell(int rowIndexx, int columnIndexx)
        {
            rowIndex = rowIndexx;
            columnIndex = columnIndexx;
            BGColor = 0xFFFFFFFF;
        }
        
        public uint BGColor
        {
            get => bGColor;
            set => this.SetField(ref bGColor, value);
        }

        /*NAME: PROPERTY CHANGED 
        *DESCRIPTION:
        *
        *PAREMETERS:
        *POSTCONDITONS:
        */
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        /*NAME: EVALUATE FORMULA 
         *DESCRIPTION:
         *
         *PAREMETERS:
         *POSTCONDITONS:
         */
        private string EvaluateFormula()
        {

            return "evaluatedFormula";
        }

        /*NAME: SET FIELD
         *DESCRIPTION: Function to set the fields of the cell class and broadcast them
         *
         *PAREMETERS: The field and its property name 
         *POSTCONDITONS: NONE
         */
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

    }

    