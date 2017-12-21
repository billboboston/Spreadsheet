///Author: William Boston
///SID: 11421575
///
/// HW 10
/// Last Modified:
///     3/30/2017

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpts321
{
    public class SpreadSheet
    {
        // 2D Array of Cells from the Abstract Base Class "CellBase"
        public Cell[,] CellGrid;
        public UndoRedo EditHistory;

        public Dictionary<string, HashSet<Cell>> RefrenceBy;



        public int RowCount { get; }
        public int ColumnCount { get; }

        public event PropertyChangedEventHandler CellPropertyChanged;
        public event PropertyChangedEventHandler UndoPropertyChanged;


        // Constructor
        public SpreadSheet(int inRow, int inColumn)
        {
            EditHistory = new UndoRedo();
            EditHistory.PropertyChanged += Text_PropertyChanged;

            CellGrid = new Cell[inRow, inColumn];
            RefrenceBy = new Dictionary<string, HashSet<Cell>>();

            RowCount = inRow;
            ColumnCount = inColumn;

            for (int i = 0; i < inRow; i++) // initalizing 2D Array
            {
                for (int h = 0; h < inColumn; h++) // nested loop
                {
                    CellGrid[i, h] = new Cell(i, h);
                    CellGrid[i, h].PropertyChanged += Text_PropertyChanged;
                }
            }

        }

        /// <summary>
        /// Event Handler for the case that CellData returns a TextChanged Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Text_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Cell NewCell;
            CommandObj tempObj = null;
            if (sender is CommandObj)
            {
                tempObj = (CommandObj)sender;
                NewCell = new Cell(tempObj.Row, tempObj.Col);
                NewCell.Text = tempObj.Text;
                NewCell.BGColor = tempObj.cColor;
            }

            else
            {
                NewCell = (Cell)sender;
            }

            if (NewCell.Text == null) // null check
            {
                NewCell.Value = null;
            }
            else if (!NewCell.Text.StartsWith("=")) // the text dosn't need to be evaluated
            {
                NewCell.Value = NewCell.Text;
                //if (CheckCircularRefrence(NewCell)) NewCell.Text = "#Circular Refrence!";
                updateCellRefrences(NewCell);
            }
            else                // call evaluation function
            {
                CellEvaluation(NewCell);
            }

            if(tempObj != null)
            {
                UndoPropertyChanged(NewCell,e);
                return;
            }
            CellPropertyChanged(NewCell, e);

        }
        
        /// <summary>
        /// Removes 'Cell_to_Remove' from all its instnaces in the RefrencedBy Dictionary
        /// </summary>
        /// <param name="Cell_to_Remove"></param>
        public void ClearRefrence(Cell Cell_to_Remove)
        {
            List<string> VarList = Cell_to_Remove.Expression.getVarNames();

            if(VarList.Count > 0)
            {
                List<Cell> CellList = new List<Cell>();

                foreach (string s in VarList) //converting string list to Cell list
                {
                    if (CheckVar(s))
                    {
                        CellList.Add(GetCell(s));
                    }
                }

                foreach (Cell c in CellList) // loop through Cell List
                {
                    RefrenceBy[c.Label].Remove(Cell_to_Remove);   //lookup and remove from ref dict
                }
            }
           
        }

        /// <summary>
        /// Adds 'Cell_to_Add' to all its refrenced variables via the RefrencedBy Dictionary
        /// </summary>
        /// <param name="Cell_to_Add"></param>
        public void addReference(Cell Cell_to_Add)
        {
            List<string> VarList = Cell_to_Add.Expression.getVarNames(); //getting variables that current cell refrences
            List<Cell> CellList = new List<Cell>();

            foreach (string s in VarList)       // converting to list of Cells
            {
                var temp = GetCell(s);
                if (temp == null) return;
                CellList.Add(temp);
            }
            
            foreach (Cell c in CellList)        // loop through Cell list
            {
                if (!RefrenceBy.ContainsKey(c.Label)) // creating a new key and hashset for the refrenced cell
                {
                    HashSet<Cell> ValList = new HashSet<Cell>(); // creating dictionary Value list
                    ValList.Add(Cell_to_Add);
                    RefrenceBy.Add(c.Label, ValList);
                }
                else                        // adding cell to the refrenceby list
                {
                    RefrenceBy[c.Label].Add(Cell_to_Add);
                }
            }
        }

        /// <summary>
        /// Updates Cell variales to its refrenced cell(s) Value
        /// </summary>
        /// <param name="Cell_to_Update"></param>
        public bool updateVars(Cell Cell_to_Update)
        {
            List<string> VarList = Cell_to_Update.Expression.getVarNames();

            foreach (string s in VarList) // loop through list
            {
                if(!CheckVar(s))
                {
                    //ClearRefrence(Cell_to_Update);
                    Cell_to_Update.Value = "Error!";
                    Cell_to_Update.Text = "#Bad Refrence!";
                  //  Cell_to_Update.Expression = null;
                    return false;
                }
                Cell TempCell = GetCell(s); // get cell from string value
                int VarValue = 0;

                if (Int32.TryParse(TempCell.Value, out VarValue)) // parse the String to int
                {
                    Cell_to_Update.Expression.SetVar(s, VarValue); // setting Dict varibles
                }
            }
            return true;
        }

        public bool CheckVar(string CellLabel)
        {
            int ColIndex = ((int)CellLabel[0] % 65);    // col index conversion (char to int)
            CellLabel = CellLabel.Remove(0, 1);     // Removeing Column lable
            int RowIndex = (int.Parse(CellLabel) - 1);  // Row index parse 
            if ((ColIndex <= 26 && ColIndex >= 0) && (RowIndex <= 50 && RowIndex >= 0)) // checking index parameters
            {
                return true;
            }
            return false;
        }

        public int CheckCircularRefrence(Cell cell)
        {
            if (RefrenceBy.ContainsKey(cell.Label))
            {
                foreach (Cell c in RefrenceBy[cell.Label])
                {
                    if( cell.Label == c.Label)
                    {
                        return 2;
                    }
                    if (RecursiveCheck(c, cell)) return 1;
                }
            }
            return 0;
        }

        public bool RecursiveCheck(Cell cell,Cell start)
        {
            if (cell.Label == start.Label) return true;
            if(RefrenceBy.ContainsKey(cell.Label))
            {
                foreach (Cell c in RefrenceBy[cell.Label])
                {
                    if (RecursiveCheck(c, start)) return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// recursively updates all cells that are refrencing CurCell
        /// </summary>
        public void updateCellRefrences(Cell CurCell)
        {
            if (RefrenceBy.ContainsKey(CurCell.Label))    // checking if cur cell is refrenced by anything
            {
                foreach (Cell c in RefrenceBy[CurCell.Label])
                {
                    updateVars(c);                  // updating variables to new cell value (not text)
                    c.update();
                    updateCellRefrences(c);         // recursive call to Refrencing Cells

                    CellPropertyChanged(c, new PropertyChangedEventArgs("TextChange")); // flagging this cell as changed
                }
            }
        }

        /// <summary>
        /// Takes in a string for a cell name. 
        /// Retuns: Cell for sucessful lookup; null for not found
        /// </summary>
        /// <param name="CellLabel"></param>
        /// <returns></returns>
        public Cell GetCell(string CellLabel)
        {
            int RowIndex = -1, ColIndex = -1;    // init indexs
                
            ColIndex = ((int)CellLabel[0] % 65);    // col index conversion (char to int)
            CellLabel = CellLabel.Remove(0, 1);     // Removeing Column lable
            RowIndex = (int.Parse(CellLabel) - 1);  // Row index parse 
            
            if((ColIndex <= 26 && ColIndex >= 0) && (RowIndex <= 50 && RowIndex >= 0)) // checking index parameters
            {
                return CellGrid[RowIndex, ColIndex];    // returning directly from Cell 2D Array
            }
            
            return null;            // just in case no data was found
        }

        /// <summary>
        /// takes in Cell and parses the Text to Expressions or Data
        /// </summary>
        /// <param name="newCell"></param>
        private void CellEvaluation(Cell NewCell)
        {
            string CellInfo = NewCell.Text.ToString();   
            if (NewCell.Text == "=") { return; }   
            CellInfo = CellInfo.Remove(0, 1);               // removing '=' to isolate expression
            
            ClearRefrence(NewCell);                         // clearing cells refrence history (cleanup)

            NewCell.Expression = new ExpTree(CellInfo);             // creating new Expression Tree

            if(updateVars(NewCell))                                   // updating NewCell's Expression Variables
            {
                NewCell.Value = NewCell.Expression.Eval().ToString();   // Evaluating NewCell's Expression
                addReference(NewCell);
            }
            

            int checkVal = CheckCircularRefrence(NewCell);
            if (checkVal == 1)
            {
                ClearRefrence(NewCell);
                NewCell.Value = "Error!";
                NewCell.Text = "#Circular Refrence!";
            }
            else if (checkVal == 2)
            {
                ClearRefrence(NewCell);
                NewCell.Value = "Error!";
                NewCell.Text = "#Self Refrence!";
            }
            updateCellRefrences(NewCell);                   // Updating All cells that refrence this Cell
            
        }

        public void AddUndo(ref Cell inCell, string EditType,bool inLink)
        {
            EditHistory.AddUndo(inCell, EditType, inLink);
        }

    }
}
