///Author: William Boston
///SID: 11421575
///
/// HW 10
///  Last modified:
///     3/10/2017
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace Cpts321
{   
    /// <summary>
    /// Base class for the undo and redo stacks
    /// </summary>
    public class CommandObj
    {
        private string CData;
        private string m_CellText;
        private int m_row;
        private int m_col;
        private Color m_CellColor;
        private bool m_Link;
        
        public string Text { get { return m_CellText; } }
        public Color cColor { get { return m_CellColor; } }
        public int Row { get { return m_row; } }
        public int Col { get { return m_col; } }
        public string Command { get { return CData; } }
       
        // Constuctor
        public CommandObj(Cell inCell, string inCommand,bool inLink)
        {
            Link = inLink;
            CData = inCommand;
            m_CellText = inCell.Text;
            m_CellColor = inCell.BGColor;
            m_row = inCell.RowIndex;
            m_col = inCell.ColumnIndex;
        }

        /// <summary>
        /// i created this variable to determine if undo/redo items were linked like a selction of cells
        /// </summary>
        public bool Link
        {
            get
            {
                return m_Link;
            }
            set
            {
                m_Link = value;
            }
        }
    }


    /// <summary>
    ///  Contains the Undo & Redo Stacks
    /// </summary>
    public class UndoRedo 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Stack<CommandObj> UndoStk;
        private Stack<CommandObj> RedoStk;

        public UndoRedo()
        {
            PropertyChanged = null;
            UndoStk = new Stack<CommandObj>();
            RedoStk = new Stack<CommandObj>();
        }
        
        public int UndoSize { get { return UndoStk.Count(); } }
        public int RedoSize { get { return RedoStk.Count(); } }

        public void AddUndo(Cell inCell,string inString,bool inLink)
        {            
            UndoStk.Push(new CommandObj(inCell, inString,inLink));
            RedoStk.Clear();
        }
        

        /// <summary>
        /// Actual Undo and Redo Funcion
        /// </summary>
        /// <param name="inSheet"></param>
        public void Undo(SpreadSheet inSheet)
        {
            CommandObj tempObj = UndoStk.Pop();

            if (UndoStk.Count > 0 && UndoStk.Peek().Link == true)
                tempObj.Link = true;
            RedoStk.Push(new CommandObj(inSheet.CellGrid[tempObj.Row, tempObj.Col], tempObj.Command, tempObj.Link));

            switch (tempObj.Command)
            {
                case "Cell Text Changed":
                    inSheet.CellGrid[tempObj.Row, tempObj.Col].Text = tempObj.Text;
                    break;

                case "Cell Background Color Change":
                    inSheet.CellGrid[tempObj.Row, tempObj.Col].BGColor = tempObj.cColor;
                    break;

                default:
                    break;
            }
            while (UndoStk.Count > 0 && UndoStk.Peek().Link == true) // checking for linked items
            {
                tempObj = UndoStk.Pop();

                RedoStk.Push(new CommandObj(inSheet.CellGrid[tempObj.Row, tempObj.Col], tempObj.Command, tempObj.Link));

                switch (tempObj.Command)
                {
                    case "Cell Text Changed":
                        inSheet.CellGrid[tempObj.Row, tempObj.Col].Text = tempObj.Text;
                        break;

                    case "Cell Background Color Change":
                        inSheet.CellGrid[tempObj.Row, tempObj.Col].BGColor = tempObj.cColor;
                        break;

                    default:
                        break;
                }
                
            }
            RedoStk.Peek().Link = false;

            inSheet.ClearRefrence(inSheet.CellGrid[tempObj.Row, tempObj.Col]); // this is the one line that prevented me from submiting for the last 2 hours

        }
        public void Redo(SpreadSheet inSheet)
        {
            CommandObj tempObj = RedoStk.Pop();
            if (tempObj.Command == "Cell Background Color Change")
                tempObj.Link = true;

            UndoStk.Push(new CommandObj(inSheet.CellGrid[tempObj.Row, tempObj.Col], tempObj.Command, tempObj.Link));

            switch (tempObj.Command)
            {
                case "Cell Text Changed":
                    inSheet.CellGrid[tempObj.Row, tempObj.Col].Text = tempObj.Text;
                    break;

                case "Cell Background Color Change":
                    inSheet.CellGrid[tempObj.Row, tempObj.Col].BGColor = tempObj.cColor;
                    break;

                default:
                    break;
            }

           
            while (RedoStk.Count > 0 && RedoStk.Peek().Link == true) // Checking for linked items
            {
                tempObj = RedoStk.Pop();

                UndoStk.Push(new CommandObj(inSheet.CellGrid[tempObj.Row, tempObj.Col], tempObj.Command, tempObj.Link));

                switch (tempObj.Command)
                {
                    case "Cell Text Changed":
                        inSheet.CellGrid[tempObj.Row, tempObj.Col].Text = tempObj.Text;
                        inSheet.ClearRefrence(inSheet.CellGrid[tempObj.Row, tempObj.Col]);
                        break;

                    case "Cell Background Color Change":
                        inSheet.CellGrid[tempObj.Row, tempObj.Col].BGColor = tempObj.cColor;
                        break;

                    default:
                        break;
                }
            }
            UndoStk.Peek().Link = false;
            inSheet.ClearRefrence(inSheet.CellGrid[tempObj.Row, tempObj.Col]); // this is the one line that prevented me from submiting for the last 2 hours
        }

        /// <summary>
        /// get the command for the user to see
        /// </summary>
        public string UndoCommand { get { return UndoStk.Peek().Command; } }
        public string RedoCommand { get { return RedoStk.Peek().Command; } }
        
    }
}
