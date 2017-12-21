///Author: William Boston
///SID: 11421575
///
/// HW 10
/// Last Modified:
///     3/10/2017
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpts321
{
    /// <summary>
    /// BaseClass For Cells
    /// Abstract Class with mem
    /// </summary>
    public abstract class BaseCell
    {
        protected int m_RowIndex; 
        protected int m_ColumnIndex;

        protected Color m_Color;

        protected string m_text;
        protected string m_value;

        //public event PropertyChangedEventHandler PropertyChanged;

        public Color BGColor
        {
            get { return m_Color; }
            set {
               if (m_Color == value)
                {
                    return;
                }
                m_Color = value;
                NotifyColorChanged("ColorChange");
            }
        }

        public string Text
        {
            get {
                return this.m_text;
            }

            set {
                if (this.m_text == value) { // if text hasn't been changed
                    return;
                }

                m_text = value;             // Text has changed
                NotifyTextChanged("TextChange");
            }
        }
        public string Value
        {
            get {
                return m_value;
            }
            set {
                m_value = value;
            }
        }

        protected abstract void NotifyTextChanged([CallerMemberName] String propertyName = "");
        protected abstract void NotifyColorChanged([CallerMemberName] String propertyName = "");
    }
    
    // <>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>-<>
    

    /// <summary>
    /// Inherits from  BaseCell, and acts as a container to instantiate the abstract Cell class
    /// </summary>
    public class Cell : BaseCell
    {
        public int RowIndex { get { return m_RowIndex; } /*set { m_RowIndex = value; }*/ }
        public int ColumnIndex { get { return m_ColumnIndex; } /*set { m_ColumnIndex = value; }*/ }

        public ExpTree Expression;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// builds cell with base expression of "1+2+3"
        /// </summary>
        /// <param name="inRow"></param>
        /// <param name="inColumn"></param>
        public Cell(int inRow, int inColumn)
        {
            m_text = "";
            m_value = "";
            PropertyChanged = null;
            m_RowIndex = inRow;
            m_ColumnIndex = inColumn;
            Expression = new ExpTree("1+2+3");            
        }
        
        /// <summary>
        /// returns a string value for the Key value in the Variable Dicitionary
        /// </summary>
        public string Label { get { return "[" + RowIndex + "," + ColumnIndex + "]"; } }

        public Cell(Cell inCell)
        {
            this.m_ColumnIndex = inCell.ColumnIndex;
            this.m_RowIndex = inCell.RowIndex;
            this.Expression = inCell.Expression;
            this.m_text = inCell.Text;
            this.m_value = inCell.Value;
            this.m_Color = inCell.BGColor;
            this.PropertyChanged = null;
        }
        /// <summary>
        /// re-Evaluates and stores to m_Value
        /// </summary>
        public void update()
        {
            Value = Expression.Eval().ToString();
        }
        // funcion that flaggs as a text change event

        protected override void NotifyTextChanged([CallerMemberName] String PropertyName = "TextChange")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected override void NotifyColorChanged([CallerMemberName] String PropertyName = "ColorChange")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
