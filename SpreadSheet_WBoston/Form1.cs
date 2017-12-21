///Author: William Boston
///SID: 11421575
///
/// HW 7
/// 

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Cpts321;

using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpreadSheet_WBoston
{
    public partial class Form1 : Form
    {
        private SpreadSheet SpreadSheetData;
        private ColorDialog ColorWindow;
        

        public Form1()
        {
            
            ColorWindow = new ColorDialog();
            SpreadSheetData = new SpreadSheet(50, 26);
            SpreadSheetData.CellPropertyChanged += Cell_PropertyChange; // Subscribing to the SpreadSheet Exception
            //SpreadSheetData.UndoPropertyChanged += Undo_RedoChange;
            
            InitializeComponent();
        }

        /// <summary>
        /// Initalizing the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadSpreadSheet(object sender, EventArgs e)
        {
            foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")  // creates 26 alphabetical rows
            {
                dataGridView1.Columns.Add(c.ToString(), c.ToString());
            }
            for (int i = 1; i <= 50; ++i) // crating 50 rows
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i - 1].HeaderCell.Value = i.ToString();
            }

            //<><> resizeing cells for UI design <><>
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
            //dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }

        
        private void updateUndoRedoButtons()
        {
            if (SpreadSheetData.EditHistory.UndoSize > 0)
            {
                UndoMenuItem.Text = "Undo " + SpreadSheetData.EditHistory.UndoCommand;
                UndoMenuItem.Enabled = true;
            }
            else
            {
                UndoMenuItem.Text = "Undo";
                UndoMenuItem.Enabled = false;
            }
            if (SpreadSheetData.EditHistory.RedoSize > 0)
            {
                RedoMenuItem.Text = "Redo " + SpreadSheetData.EditHistory.RedoCommand;
                RedoMenuItem.Enabled = true;
            }
            else
            {
                RedoMenuItem.Text = "Redo";
                RedoMenuItem.Enabled = false;
            }
        }

        // event handler for the text Change event
        private void Cell_PropertyChange(object sender, PropertyChangedEventArgs e) // What to do once you recieve Exception
        {
            Cell TempCell = (Cell)sender;
            if (e.PropertyName == "TextChange")
            {
                dataGridView1.Rows[TempCell.RowIndex].Cells[TempCell.ColumnIndex].Value =
                    SpreadSheetData.CellGrid[TempCell.RowIndex, TempCell.ColumnIndex].Value; // Setting UI value to New Value
            }
            else if (e.PropertyName == "ColorChange")
            {
                dataGridView1.Rows[TempCell.RowIndex].Cells[TempCell.ColumnIndex].Style.BackColor =
                SpreadSheetData.CellGrid[TempCell.RowIndex, TempCell.ColumnIndex].BGColor;
            }



        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = SpreadSheetData.CellGrid[e.RowIndex, e.ColumnIndex].Text;
        }
        // first event
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //textBox1.Text = SpreadSheetData.CellGrid[e.RowIndex, e.ColumnIndex].Text;
            // change UI cell data from m_value to text;
            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = SpreadSheetData.CellGrid[e.RowIndex, e.ColumnIndex].Text;
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = SpreadSheetData.CellGrid[e.RowIndex, e.ColumnIndex].Value;
        }
        // final event in changing cell value
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex >= 0 && e.RowIndex <= 49) && (e.ColumnIndex >= 0 && e.ColumnIndex <= 26)) // parameter check
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null) // null check in case no text was input
                {
                    string inString = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(); // converting cell.Value to string

                    SpreadSheetData.AddUndo(ref SpreadSheetData.CellGrid[e.RowIndex, e.ColumnIndex], "Cell Text Changed", false);
                    SpreadSheetData.CellGrid[e.RowIndex, e.ColumnIndex].Text = inString;                // SETTING spreadsheetData class cell value
                    updateUndoRedoButtons();

                }
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = SpreadSheetData.CellGrid[e.RowIndex, e.ColumnIndex].Value;
            }
            
          }

        
        private void chooseBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool result = true;

            if (DialogResult.Cancel != ColorWindow.ShowDialog()) // checks if user canceled selection
            {
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    //dataGridView1.SelectedCells[i].Style.BackColor = ColorWindow.Color;

                    if (i == dataGridView1.SelectedCells.Count - 1)
                        result = false;  // making last item link value false
                    else
                        result = true; // setting link value to true

                    SpreadSheetData.AddUndo(
                           ref SpreadSheetData.CellGrid[dataGridView1.SelectedCells[i].RowIndex, dataGridView1.SelectedCells[i].ColumnIndex],
                           "Cell Background Color Change",
                           result);
                    SpreadSheetData.CellGrid[dataGridView1.SelectedCells[i].RowIndex, dataGridView1.SelectedCells[i].ColumnIndex].BGColor = ColorWindow.Color;
                    
                }
                updateUndoRedoButtons();
            }
        }
          

        private void demo1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* <><><>- Demo for HW 7 -<><><> */
            List<string> StrList = new List<string>();
            StrList.Add("1"); StrList.Add("=A2+2"); StrList.Add("=A2*B2+3");

            int RowIndex = 0, ColIndex = 0;
            foreach (string s in StrList)
            {
                SpreadSheetData.CellGrid[RowIndex, ColIndex].Text = ("'" + s) + "'";
                ColIndex++;
            }

            RowIndex = 1;
            ColIndex = 0;
            foreach (string s in StrList)
            {
                SpreadSheetData.CellGrid[RowIndex, ColIndex].Text = s;
                ColIndex++;
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadSheetData.EditHistory.Undo(SpreadSheetData);
            updateUndoRedoButtons();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadSheetData.EditHistory.Redo(SpreadSheetData);
            updateUndoRedoButtons();
        }

       

        
    }
}
