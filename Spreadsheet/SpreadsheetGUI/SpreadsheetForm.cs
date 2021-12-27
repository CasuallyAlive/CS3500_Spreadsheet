// Author: Jordy A. Larrea Rodriguez V 3.4 (03/18/2021)

using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// The type of delegate used to register for model changed events
    /// </summary>
    /// <param name="sender"></param>
    public delegate void ModelChangedHandler(Spreadsheet model);
    public partial class Spreadsheet1 : Form
    {
        private Spreadsheet model;
        private string filename;
        public object SpreadSheetApplicationContext { get; private set; }

        public Spreadsheet1()
        {
            InitializeComponent();
            CurrentSpreadSheetPanel.SelectionChanged += OnSelectionChanged;
            CurrentSpreadSheetPanel.TextBoxContentsSet += OnSettingTextBox;
            this.FormClosing += WindowClosing;
            model = new Spreadsheet(IsValid, s => s.ToUpper(),"ps6");
            filename = "Untitled.sprd";
            MessageBox.Show("Hi! Thank you for using my spreadsheet!\nThe spreadsheet is fairly intuitive to use, but you can always reference the \"Help\" menu located in the upper left hand corner.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// Event is triggered whenever the form is closed. If the window is closed without having saved this spreadsheet, then the user is prompted on whether they would like to 
        /// save there changes to the spreadsheet; otherwise, the form closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosing(object sender, FormClosingEventArgs e)
        {
            //Asks the user if they want to save changes
            if (model.Changed)
            {
                DialogResult SaveChangesResult =
            MessageBox.Show("Want to save your changes to this spreadsheet before closing?", "Un-Saved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (SaveChangesResult == DialogResult.Yes)
                    SaveSpreadsheet();
                else if (SaveChangesResult == DialogResult.Cancel)
                    e.Cancel = true;
            }
            return;
        }
        /// <summary>
        /// Validator function that is passed to the spreadsheet for validation of variable names
        /// </summary>
        /// <param name="name"></param>
        /// <returns> 
        /// False if name is null or is only one character long or name is greater than 3 characters long or is not a letter followed by one or two numbers.
        /// True if name is a letter followed by one or two numbers, e.g. name = "A12". 
        /// </returns>
        private bool IsValid(string name)
        {
            if (Object.ReferenceEquals(name, null) || (name.Length <= 1 || name.Length > 3))
                return false;
            Regex varMatcher = new Regex(@"^[a-zA-Z][0-9]{1,2}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return varMatcher.IsMatch(name);
        }
    
        /// <summary>
        /// Listener for the SelectionChanged event, calls for the SpreadsheetPanel to place a textbox at the disclosed coordinates inhabited by the selected cell.
        /// </summary>
        /// <param name="ssp"></param>
        private void OnSelectionChanged(SpreadsheetPanel ssp)
        {
            ssp.GetSelection(out int col, out int row);
            GetName(col, row, out string name);
            string contents = GetContentsOfCell(name);
            CellName.Text = name;
            ContentsForCell.Text = contents;
            CellValue.Text = "\"" + GetValueOfCell(name) + "\"";
            ssp.PlaceTextBoxAtCell(col, row, contents);
        }
        /// <summary>
        /// Listener for the TextBoxContentsSet event, calls for the SpreadsheetPanel to place the ultimate value of the textbox into the cell whenever the 'enter' key is clicked
        /// while focused on a cell. Sets up the cell in this spreadsheet with associated value, contents, and dependencies.
        /// </summary>
        /// <param name="ssp"></param>
        private void OnSettingTextBox(SpreadsheetPanel ssp,out bool IsError)
        {
            ssp.GetSelection(out int col, out int row);
            GetName(col, row, out string name);
            ssp.GetContentsOfTextBox(out string TextBoxContents);
            IsError = false;
            try
            {
                ReCalculateCells(model.SetContentsOfCell(name, TextBoxContents));
            }
            catch (Exception e)
            {
                IsError = true;
                if (e is CircularException)
                {
                    DialogResult resultOfErrorMessage = 
                        MessageBox.Show("Circular-Dependency detected!\nUnable to set the cell with the specified value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (e is FormulaFormatException)
                {
                    DialogResult resultOfErrorMessage =
                        MessageBox.Show("Invalid Formula detected!\nUnable to set the cell with the specified Formula.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            CellValue.Text = GetValueOfCell(name);
            SavedState.Text = filename + " - Un-Saved"; 
        }
        /// <summary>
        /// Gets the name of the cell by converting integer values to Characters (ASCII) and starting the indexing at 1. 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="name"></param>
        private void GetName(int col, int row, out string name)
        {
            name = "" + (char)(col + 65) + (row + 1);
        }
        /// <summary>
        /// Override key processes. 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Only switch focus between the button and the textbox in this spreadsheet form. 
            if (keyData == Keys.Tab)
            {
                if (ContentsForCell.Focused)
                    EnterContents.Focus();
                else if (EnterContents.Focused)
                    ContentsForCell.Focus();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        /// <summary>
        /// Gets the contents contained at cell with title name. Appends '=' to the end of the string if the contents are of type formula.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>
        /// A string representation of a cell's contents.
        /// </returns>
        private string GetContentsOfCell(string name)
        {
            object contents = model.GetCellContents(name);
            if (contents is Formula)
                return "=" + contents.ToString();
            return contents.ToString();
        }
        /// <summary>
        /// Gets the value contained at cell with title name. If the value is FormulaError, then gets a string that implies that the result of a formula is not a value.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>
        /// A string representation of a cell's value.
        /// </returns>
        private string GetValueOfCell(string name)
        {
            object contents = model.GetCellValue(name);
            if (contents is FormulaError)
                return "!Value";
            if (Double.TryParse(contents.ToString(), out double result))
                return Math.Round(result, 9).ToString();
            return contents.ToString();
        }
        /// <summary>
        /// Closes this spreadsheet whenever the strip menu item titled 'close' is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// loads an existing spreadsheet on this form whenever the strip menu item titled 'open' is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (model.Changed)
            {
                DialogResult result =
                   MessageBox.Show("Would you like to save the current spreadsheet?", "Save to File", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                    SaveSpreadsheet();
            }
            DialogResult resultsOpenFile = openFileDialog1.ShowDialog();
            if (resultsOpenFile == DialogResult.Cancel)
                return;
            try { 
                try
                {
                    model = new Spreadsheet(openFileDialog1.FileName, IsValid, s => s.ToUpper(), "ps6");
                }
                catch (SpreadsheetReadWriteException)
                {
                    MessageBox.Show("Could not open file!\nPlease make sure that your file-name and directory are correct.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                CurrentSpreadSheetPanel.Clear();
                ReCalculateCells(model.GetNamesOfAllNonemptyCells().ToList());
                model.Save(openFileDialog1.FileName);
                filename = Path.GetFileName(openFileDialog1.FileName);
                SavedState.Text = filename;
            }
            catch (Exception)
            {
                MessageBox.Show("Could not open file!\nFile was invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        /// <summary>
        /// Opens a new window with a fresh spreadsheet that is independent of this spreadsheet whenever the strip menu item titled 'menu' is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newSpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.getAppContext().RunForm(new Spreadsheet1());
        }
        /// <summary>
        /// Prompts the user to save the spreadsheet to a file with a 'sprd' extension whenever the strip menu item titled 'save' is clicked. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = 
                MessageBox.Show("Would you like to save the current spreadsheet?", "Save to File", MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
            if(result == DialogResult.OK)
                SaveSpreadsheet();
            return;
        }
        /// <summary>
        /// Writes the spreadsheet to a file with a 'sprd' extension. 
        /// </summary>
        private void SaveSpreadsheet()
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
                return;
            try
            {
                model.Save(saveFileDialog1.FileName);
            }
            catch (SpreadsheetReadWriteException)
            {
                MessageBox.Show("Invalid Save directory or filename", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.filename = Path.GetFileName(saveFileDialog1.FileName);
            SavedState.Text = filename + " - Saved";
            return;
        }
        /// <summary>
        /// Sets the value of the currently selected cell with the contents of the ContentsForCell textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterContents_Click(object sender, EventArgs e)
        {
            CurrentSpreadSheetPanel.GetSelection(out int col, out int row);
            GetName(col, row, out string name);
            try
            {
                ReCalculateCells(model.SetContentsOfCell(name, ContentsForCell.Text));
            }
            catch (Exception error)
            {
                if (error is CircularException)
                {
                    DialogResult resultOfErrorMessage = MessageBox.Show("Circular-Dependency detected!\nUnable to set the cell with the specified value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (error is FormulaFormatException)
                {
                    DialogResult resultOfErrorMessage =
                   MessageBox.Show("Invalid Formula detected!\nUnable to set the cell with the specified Formula.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            CurrentSpreadSheetPanel.SetTextBoxContents(ContentsForCell.Text);
            SavedState.Text = filename + " - Un-Saved";
        }
        /// <summary>
        /// Upon changing a cell, the values of the cells dependent on said cell are updated.
        /// </summary>
        /// <param name="CellsToRecalculate"></param>
        private void ReCalculateCells(IList<string> CellsToRecalculate)
        {
            foreach(string name in CellsToRecalculate)
            {
                GetCellCoordinates(out int col, out int row, name);
                CurrentSpreadSheetPanel.SetValue(col, row, GetValueOfCell(name));
            }
        }
        /// <summary>
        /// Converts string cell names to grid coordinates. 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="name"></param>
        private void GetCellCoordinates(out int col, out int row, string name)
        {
            col = (int)name[0] - 65;
            row = Int32.Parse(name.Substring(1, name.Length - 1)) - 1;
        }
        /// <summary>
        /// Information for Selecting cells.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectingCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
             MessageBox.Show("A cell can be selected directly by one of two ways: by clicking on the cell or by pressing on the arrow keys when focused on another cell.\n (left key - select left,right key - select right, up key - select above, down key - select below)\nIn this spreadsheet cells are automatically " +
                 "selected whenever either \"enter\" or \"tab\" keys are pressed while focused on a cell with the former causing the cell below the current cell to be selected while the latter causes the cell to the immediate " +
                 "right of the current cell to be selected. *NOTE* The \"left\" and \"right\" keys will not change selection if the contents of the cell are being edited.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// Information for editing cells.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editingCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A cell can be edited by one of two ways: directly on a text field on the cell or via a textbox located in the upper left corner. Any selection on a cell will automatically place the previous contents of the " +
                "cell on the respective textboxes. Simply provide input and press any of the selection-specific keys (for info on selection consult \"Help -> Selecting Cells\") to set the value of the cell if the first method is used; " +
                "otherwise, press the button labeled \"Enter\" located to the right of the textbox.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// Information for creating formulas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formulaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Formulas can be generated by starting an input string with the \"=\" character.\n*Note* Formulas with improper syntax or Cycles will result in an error and the Formula will be destroyed.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// Information for extra feature.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void featureInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The special feature I came up with is essentially an interactable and dynamic textbox that appears whenever you, the user, select a cell. The cell exists as a property in the embedded class, DrawingPanel, in SpreadsheetPanel. I used events and " +
                "normal methods to allow communication between the panel and the main spreadsheet form which took much longer than I expected, due to the textbox having to be updated constantly whenever required by the spreadsheet form or the redraw mechanism of the panel.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}
