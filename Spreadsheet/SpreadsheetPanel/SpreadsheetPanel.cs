﻿// Written by Joe Zachary for CS 3500, September 2011.
// Added dynamic textbox - Jordy A. Larrea Rodriguez 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace SS
{
    /// <summary>
    /// The type of delegate used to register for SelectionChanged events.
    /// </summary>
    /// <param name="sender"></param>

    public delegate void SelectionChangedHandler(SpreadsheetPanel sender);
    /// <summary>
    /// The type of delegate used to register for TextBox contents set events. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="errorOccurred">true if an error is encountered, false otherwise</param>
    public delegate void SetTextBoxFieldHandler(SpreadsheetPanel sender, out bool errorOccurred);

    /// <summary>
    /// A panel that displays a spreadsheet with 26 columns (labeled A-Z) and 99 rows
    /// (labeled 1-99).  Each cell on the grid can display a non-editable string.  One 
    /// of the cells is always selected (and highlighted).  When the selection changes, a 
    /// SelectionChanged event is fired.  Clients can register to be notified of
    /// such events.
    /// 
    /// None of the cells are editable.  They are for display purposes only.
    /// </summary>

    public partial class SpreadsheetPanel : UserControl
    {
        /// <summary>
        /// The event used to send notifications of a selection change
        /// </summary>
        public event SelectionChangedHandler SelectionChanged;
        /// <summary>
        /// The event used to send notifications of the contents in a textbox being sent
        /// </summary>
        public event SetTextBoxFieldHandler TextBoxContentsSet;

        // The SpreadsheetPanel is composed of a DrawingPanel (where the grid is drawn),
        // a horizontal scroll bar, and a vertical scroll bar.
        private DrawingPanel drawingPanel;
        private HScrollBar hScroll;
        private VScrollBar vScroll;

        // These constants control the layout of the spreadsheet grid.  The height and
        // width measurements are in pixels.
        private const int DATA_COL_WIDTH = 80;
        private const int DATA_ROW_HEIGHT = 20;
        private const int LABEL_COL_WIDTH = 30;
        private const int LABEL_ROW_HEIGHT = 30;
        private const int PADDING = 2;
        private const int SCROLLBAR_WIDTH = 20;
        private const int COL_COUNT = 26;
        private const int ROW_COUNT = 99;


        /// <summary>
        /// Creates an empty SpreadsheetPanel
        /// </summary>

        public SpreadsheetPanel()
        {

            InitializeComponent();
            // The DrawingPanel is quite large, since it has 26 columns and 99 rows.  The
            // SpreadsheetPanel itself will usually be smaller, which is why scroll bars
            // are necessary.
            drawingPanel = new DrawingPanel(this);
            drawingPanel.Location = new Point(0, 0);
            drawingPanel.AutoScroll = false;
            // A custom vertical scroll bar.  It is designed to scroll in multiples of rows.
            vScroll = new VScrollBar();
            vScroll.SmallChange = 1;
            vScroll.Maximum = ROW_COUNT;

            // A custom horizontal scroll bar.  It is designed to scroll in multiples of columns.
            hScroll = new HScrollBar();
            hScroll.SmallChange = 1;
            hScroll.Maximum = COL_COUNT;

            // A custom button, that gives the enter key functionality when interacting with a textbox.
            // Add the drawing panel and the scroll bars to the SpreadsheetPanel.
            Controls.Add(drawingPanel);
            Controls.Add(vScroll);
            Controls.Add(hScroll);
            // Arrange for the drawing panel to be notified when it needs to scroll itself.
            hScroll.Scroll += drawingPanel.HandleHScroll;
            vScroll.Scroll += drawingPanel.HandleVScroll;
            // notifies the button listener whenever a key is entered. 
        }

        
        /// <summary>
        /// Clears the display.
        /// </summary>

        public void Clear()
        {
            drawingPanel.Clear();
        }

        private void errorAlert()
        {
            
        }
        /// <summary>
        /// If the zero-based column and row are in range, sets the value of that
        /// cell and returns true.  Otherwise, returns false.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(int col, int row, string value)
        {
            return drawingPanel.SetValue(col, row, value);
        }
        /// <summary>
        /// Makes a textbox at the location of the grid that was clicked by the user. If the cell was non-empty, then replace the contents of the textbox 
        /// with the selected cell's contents. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void PlaceTextBoxAtCell(int col, int row, string contents)
        {
            drawingPanel.CreateTextBox(col, row);
            drawingPanel.SetTextBoxContents(contents);
        }
        /// <summary>
        /// Gets the contents or string in the textbox at the currently selected cell.
        /// </summary>
        /// <returns>
        /// The contents of the textbox.
        /// </returns>
        public void GetContentsOfTextBox(out string contents)
        {
             contents = drawingPanel.CurrentCell.Text;
        }
        /// <summary>
        /// If the zero-based column and row are in range, assigns the value
        /// of that cell to the out parameter and returns true.  Otherwise,
        /// returns false.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        public bool GetValue(int col, int row, out string value)
        {
            return drawingPanel.GetValue(col, row, out value);
        }
        /// <summary>
        /// Sets the text field of the textbox with contents which can represent either a string, a double, or a formula.
        /// </summary>
        /// <param name="contents"></param>
        public void SetTextBoxContents(string contents)
        {
            drawingPanel.SetTextBoxContents(contents);
        }

        /// <summary>
        /// If the zero-based column and row are in range, uses them to set
        /// the current selection and returns true.  Otherwise, returns false.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>

        public bool SetSelection(int col, int row)
        {
            return drawingPanel.SetSelection(col, row);
        }


        /// <summary>
        /// Assigns the column and row of the current selection to the
        /// out parameters.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>

        public void GetSelection(out int col, out int row)
        {
            drawingPanel.GetSelection(out col, out row);
        }


        /// <summary>
        /// When the SpreadsheetPanel is resized, we set the size and locations of the three
        /// components that make it up.
        /// </summary>
        /// <param name="eventargs"></param>

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            if (FindForm() == null || FindForm().WindowState != FormWindowState.Minimized)
            {
                drawingPanel.Size = new Size(Width - SCROLLBAR_WIDTH, Height - SCROLLBAR_WIDTH);
                vScroll.Location = new Point(Width - SCROLLBAR_WIDTH, 0);
                vScroll.Size = new Size(SCROLLBAR_WIDTH, Height - SCROLLBAR_WIDTH);
                vScroll.LargeChange = (Height - SCROLLBAR_WIDTH) / DATA_ROW_HEIGHT;
                hScroll.Location = new Point(0, Height - SCROLLBAR_WIDTH);
                hScroll.Size = new Size(Width - SCROLLBAR_WIDTH, SCROLLBAR_WIDTH);
                hScroll.LargeChange = (Width - SCROLLBAR_WIDTH) / DATA_COL_WIDTH;
            }
        }



        /// <summary>
        /// Used internally to keep track of cell addresses
        /// </summary>

        private class Address
        {

            public int Col { get; set; }
            public int Row { get; set; }

            public Address(int c, int r)
            {
                Col = c;
                Row = r;
            }

            public override int GetHashCode()
            {
                return Col.GetHashCode() ^ Row.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if ((obj == null) || !(obj is Address))
                {
                    return false;
                }
                Address a = (Address)obj;
                return Col == a.Col && Row == a.Row;
            }

        }


        /// <summary>
        /// The panel where the spreadsheet grid is drawn.  It keeps track of the
        /// current selection as well as what is supposed to be drawn in each cell.
        /// </summary>

        private class DrawingPanel : Panel
        {
            // Columns and rows are numbered beginning with 0.  This is the coordinate
            // of the selected cell.
            private int _selectedCol;
            private int _selectedRow;

            // Coordinate of cell in upper-left corner of display
            private int _firstColumn = 0;
            private int _firstRow = 0;

            // The strings contained by the spreadsheet
            private Dictionary<Address, String> _values;

            // The containing panel
            private SpreadsheetPanel _ssp;

            // Textbox field that appears upon selecting a cell
            public TextBox CurrentCell { get; set; }

            /// <summary>
            /// Makes a textbox at the location of the grid that was clicked by the user. 
            /// </summary>
            /// <param name="row"></param>
            /// <param name="col"></param>
            public void CreateTextBox(int col, int row)
            {
                // Upon selecting another cell, remove the previous textbox from controls
                this.Controls.Remove(CurrentCell);
                // Position determined by the offset of the labels plus the padding plus the scaled horizontal or vertical components (determined by _firstColumn and _firstRow).
                CurrentCell = new TextBox();
                CurrentCell.Location = new System.Drawing.Point(LABEL_COL_WIDTH + 1+(col - _firstColumn) * DATA_COL_WIDTH, LABEL_ROW_HEIGHT + 1 + (row -_firstRow) * DATA_ROW_HEIGHT); 
                CurrentCell.Size = new System.Drawing.Size(DATA_COL_WIDTH, DATA_ROW_HEIGHT);
                this.Controls.Add(CurrentCell);
                // Setting the event handler for the enter key functionality which allows the user to set the value of the cell.
                CurrentCell.KeyDown += OnSettingTextBox;
                // The Textbox's focus is prioritized
                CurrentCell.BringToFront();
                CurrentCell.Focus();
            }
            public DrawingPanel(SpreadsheetPanel ss)
            {
                DoubleBuffered = true;
                _values = new Dictionary<Address, String>();
                _ssp = ss;
                CurrentCell = new TextBox();
            }

            private void OnSettingTextBox(object sender, KeyEventArgs e)
            {
                e.Handled = true;
                if (!Controls.Contains(CurrentCell))
                    return;
                if (e.KeyCode == Keys.Enter)
                    ShiftCellSouthOrEast(ref _selectedCol, 25,ref _selectedRow, 98, 1);
                else if(e.KeyCode == Keys.Right)
                    ShiftCellSouthOrEast(ref _selectedRow, 98, ref _selectedCol, 25, 1);
                else if (e.KeyCode == Keys.Left)
                    ShiftCellNorthOrWest(ref _selectedRow, 98, ref _selectedCol, 25, -1);
                else if(e.KeyCode == Keys.Up)
                    ShiftCellNorthOrWest(ref _selectedCol, 25, ref _selectedRow, 98, -1);
                else if (e.KeyCode == Keys.Down)
                    ShiftCellSouthOrEast(ref _selectedCol, 25, ref _selectedRow, 98, 1);
            }
            /// <summary>
            /// On pressing either right-arrow/tab or down-arrow/enter when focused on the selected cell's textbox,
            /// Shift selection of the cell east or south respectively.
            /// </summary>
            /// <param name="ShiftComponent"></param>
            /// <param name="UpperLimit1"></param>
            /// <param name="StationaryComponent"></param>
            /// <param name="UpperLimit2"></param>
            /// <param name="shiftVal"></param>
            private void ShiftCellSouthOrEast(ref int ShiftComponent, int UpperLimit1, ref int StationaryComponent, int UpperLimit2, int shiftVal)
            { 
                Controls.Remove(CurrentCell);
                CurrentCell.KeyDown -= OnSettingTextBox;
                _ssp.TextBoxContentsSet(_ssp, out bool error);
                if (error)
                    return;
                if (ShiftComponent == UpperLimit1 && StationaryComponent == UpperLimit2)
                    ShiftComponent = 0;
                else if (ShiftComponent < UpperLimit1 && StationaryComponent == UpperLimit2)
                    ShiftComponent += shiftVal;
                StationaryComponent = (StationaryComponent < UpperLimit2) ? StationaryComponent + shiftVal : 0;
                if (_ssp.SelectionChanged != null)
                    _ssp.SelectionChanged(_ssp);
            }
            /// <summary>
            /// On pressing either left-arrow or up-arrow when focused on the selected cell's textbox,
            /// Shift selection of the cell west or north respectively.
            /// </summary>
            /// <param name="ShiftComponent"></param>
            /// <param name="UpperLimit1"></param>
            /// <param name="StationaryComponent"></param>
            /// <param name="UpperLimit2"></param>
            /// <param name="shiftVal"></param>
            private void ShiftCellNorthOrWest(ref int ShiftComponent, int UpperLimit1, ref int StationaryComponent, int UpperLimit2, int shiftVal)
            {
                Controls.Remove(CurrentCell);
                CurrentCell.KeyDown -= OnSettingTextBox;
                _ssp.TextBoxContentsSet(_ssp, out bool error);
                if (error)
                    return;
                if (ShiftComponent == 0 && StationaryComponent == 0)
                    ShiftComponent = UpperLimit1;
                else if (ShiftComponent > 0 && StationaryComponent == 0)
                    ShiftComponent += shiftVal;
                StationaryComponent = (StationaryComponent > 0) ? StationaryComponent + shiftVal : UpperLimit2;
                if (_ssp.SelectionChanged != null)
                    _ssp.SelectionChanged(_ssp);
            }
            /// <summary>
            /// Overrides default key processes.
            /// </summary>
            /// <param name="msg"></param>
            /// <param name="keyData"></param>
            /// <returns></returns>
            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                // Shift cell selection to the right, essentially a horizontal "enter". 
                if (keyData == Keys.Tab && CurrentCell.Focused)
                {
                    ShiftCellSouthOrEast(ref _selectedRow, 98, ref _selectedCol, 25, 1);
                    return true;
                }
                // Undo Default functionality of left and right keys
                if ((keyData == Keys.Up || keyData == Keys.Down))
                    return false;
                // Only allow horizontal selection shifts if the cell is empty. Otherwise, retain keys for moving cursor selection.
                if (CurrentCell.Text != "" && (keyData == Keys.Left || keyData == Keys.Right))
                {
                    if (keyData == Keys.Left && CurrentCell.SelectionStart > 0)
                        CurrentCell.SelectionStart -= 1;
                    else if (keyData == Keys.Right && CurrentCell.SelectionStart < CurrentCell.Text.Length)
                        CurrentCell.SelectionStart += 1;
                    return true;
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }
      
            private bool InvalidAddress(int col, int row)
            {
                return col < 0 || row < 0 || col >= COL_COUNT || row >= ROW_COUNT;
            }


            public void Clear()
            {
                _values.Clear();
                Invalidate();
            }


            public bool SetValue(int col, int row, string c)
            {
                if (InvalidAddress(col, row))
                {
                    return false;
                }

                Address a = new Address(col, row);
                if (c == null || c == "")
                {
                    _values.Remove(a);
                }
                else
                {
                    _values[a] = c;
                }
                Invalidate();
                return true;
            }


            public bool GetValue(int col, int row, out string c)
            {
                if (InvalidAddress(col, row))
                {
                    c = null;
                    return false;
                }
                if (!_values.TryGetValue(new Address(col, row), out c))
                {
                    c = "";
                }
                return true;
            }

            public bool SetSelection(int col, int row)
            {
                if (InvalidAddress(col, row))
                {
                    return false;
                }
                _selectedCol = col;
                _selectedRow = row;
                Invalidate();
                return true;
            }

            public void GetSelection(out int col, out int row)
            {
                col = _selectedCol;
                row = _selectedRow;
            }


            public void HandleHScroll(Object sender, ScrollEventArgs args)
            {
                var prevColumn = _firstColumn;  
                _firstColumn = args.NewValue;
                // Re-Draw the textbox
                ShiftTextBoxHorizontally(prevColumn);
                Invalidate();
            }

            public void HandleVScroll(Object sender, ScrollEventArgs args)
            {
                var prevRow =_firstRow;
                _firstRow = args.NewValue;
                // Re-Draw the textbox
                ShiftTextBoxVertically(prevRow);
                Invalidate();
            }
            /// <summary>
            /// Shifts the Textbox horizontally relative to the transformation of the grid by the horizontal scrollbar
            /// </summary>
            /// <param name="prevColumn"></param>
            private void ShiftTextBoxHorizontally(int prevColumn)
            {
                if (prevColumn == _firstColumn)
                    return;
                // The Vertical shift of the textbox is determined by the difference of the previous row reference and the new row reference plus the previous vertical component of the textbox.
                CurrentCell.Location = new System.Drawing.Point(CurrentCell.Location.X + DATA_COL_WIDTH * (prevColumn - _firstColumn), CurrentCell.Location.Y);
                // Turn off the visibility of the textbox if the textbox is outside of the view range of the user in the X component.
                if (CurrentCell.Location.X < 31)
                    CurrentCell.Visible = false;
                // Turn on the visibility of the textbox if the textbox is once again in the view range of the user (check both x and y coordinates)
                else if (!CurrentCell.Visible && (CurrentCell.Location.X >= 31 && CurrentCell.Location.Y >= 31))
                    CurrentCell.Visible = true;
            }
            /// <summary>
            /// Shifts the Textbox Vertically relative to the transformation of the grid by the vertical scrollbar
            /// </summary>
            /// <param name="prevColumn"></param>
            private void ShiftTextBoxVertically(int prevRow)
            {
                if (prevRow == _firstRow)
                    return;
                // The Vertical shift of the textbox is determined by the difference of the previous row reference and the new row reference plus the previous vertical component of the textbox.
                CurrentCell.Location = new System.Drawing.Point(CurrentCell.Location.X, CurrentCell.Location.Y + DATA_ROW_HEIGHT *(prevRow - _firstRow));
                // Turn off the visibility of the textbox if the textbox is outside of the view range of the user in the Y component.
                if (CurrentCell.Location.Y < 31) 
                    CurrentCell.Visible = false;
                // Turn on the visibility of the textbox if the textbox is once again in the view range of the user (check both x and y coordinates)
                else if (!CurrentCell.Visible && (CurrentCell.Location.X >= 31 && CurrentCell.Location.Y >= 31))
                    CurrentCell.Visible = true;                    
            }
            protected override void OnPaint(PaintEventArgs e)
            {

                // Clip based on what needs to be refreshed.
                Region clip = new Region(e.ClipRectangle);
                e.Graphics.Clip = clip;
                // Color the background of the data area white
                e.Graphics.FillRectangle(
                    new SolidBrush(Color.White),
                    LABEL_COL_WIDTH,
                    LABEL_ROW_HEIGHT,
                    (COL_COUNT - _firstColumn) * DATA_COL_WIDTH,
                    (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT);

                // Pen, brush, and fonts to use
                Brush brush = new SolidBrush(Color.Black);
                Pen pen = new Pen(brush);
                Font regularFont = Font;
                Font boldFont = new Font(regularFont, FontStyle.Bold);

                // Draw the column lines
                int bottom = LABEL_ROW_HEIGHT + (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT;
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, bottom));
                for (int x = 0; x <= (COL_COUNT - _firstColumn); x++)
                {
                    e.Graphics.DrawLine(
                        pen,
                        new Point(LABEL_COL_WIDTH + x * DATA_COL_WIDTH, 0),
                        new Point(LABEL_COL_WIDTH + x * DATA_COL_WIDTH, bottom));
                }

                // Draw the column labels
                for (int x = 0; x < COL_COUNT - _firstColumn; x++)
                {
                    Font f = (_selectedCol - _firstColumn == x) ? boldFont : Font;
                    DrawColumnLabel(e.Graphics, x, f);
                }

                // Draw the row lines
                int right = LABEL_COL_WIDTH + (COL_COUNT - _firstColumn) * DATA_COL_WIDTH;
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(right, 0));
                for (int y = 0; y <= ROW_COUNT - _firstRow; y++)
                {
                    e.Graphics.DrawLine(
                        pen,
                        new Point(0, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT),
                        new Point(right, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT));
                }

                // Draw the row labels
                for (int y = 0; y < (ROW_COUNT - _firstRow); y++)
                {
                    Font f = (_selectedRow - _firstRow == y) ? boldFont : Font;
                    DrawRowLabel(e.Graphics, y, f);
                }

                // Highlight the selection, if it is visible
                if ((_selectedCol - _firstColumn >= 0) && (_selectedRow - _firstRow >= 0))
                {
                    e.Graphics.DrawRectangle(
                        pen,
                        new Rectangle(LABEL_COL_WIDTH + (_selectedCol - _firstColumn) * DATA_COL_WIDTH + 1,
                                      LABEL_ROW_HEIGHT + (_selectedRow - _firstRow) * DATA_ROW_HEIGHT + 1,
                                      DATA_COL_WIDTH - 2,
                                      DATA_ROW_HEIGHT - 2));
                }

                // Draw the text
                foreach (KeyValuePair<Address, String> address in _values)
                {
                    String text = address.Value;
                    int x = address.Key.Col - _firstColumn;
                    int y = address.Key.Row - _firstRow;
                    float height = e.Graphics.MeasureString(text, regularFont).Height;
                    float width = e.Graphics.MeasureString(text, regularFont).Width;
                    if (x >= 0 && y >= 0)
                    {
                        Region cellClip = new Region(new Rectangle(LABEL_COL_WIDTH + x * DATA_COL_WIDTH + PADDING,
                                                                   LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT,
                                                                   DATA_COL_WIDTH - 2 * PADDING,
                                                                   DATA_ROW_HEIGHT));
                        cellClip.Intersect(clip);
                        e.Graphics.Clip = cellClip;
                        e.Graphics.DrawString(
                            text,
                            regularFont,
                            brush,
                            LABEL_COL_WIDTH + x * DATA_COL_WIDTH + PADDING,
                            LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT - height) / 2);
                    }
                }
            }


            /// <summary>
            /// Draws a column label.  The columns are indexed beginning with zero.
            /// </summary>
            /// <param name="g"></param>
            /// <param name="x"></param>
            /// <param name="f"></param>
            private void DrawColumnLabel(Graphics g, int x, Font f)
            {
                String label = ((char)('A' + x + _firstColumn)).ToString();
                float height = g.MeasureString(label, f).Height;
                float width = g.MeasureString(label, f).Width;
                g.DrawString(
                      label,
                      f,
                      new SolidBrush(Color.Black),
                      LABEL_COL_WIDTH + x * DATA_COL_WIDTH + (DATA_COL_WIDTH - width) / 2,
                      (LABEL_ROW_HEIGHT - height) / 2);
            }


            /// <summary>
            /// Draws a row label.  The rows are indexed beginning with zero.
            /// </summary>
            /// <param name="g"></param>
            /// <param name="y"></param>
            /// <param name="f"></param>
            private void DrawRowLabel(Graphics g, int y, Font f)
            {
                String label = (y + 1 + _firstRow).ToString();
                float height = g.MeasureString(label, f).Height;
                float width = g.MeasureString(label, f).Width;
                g.DrawString(
                    label,
                    f,
                    new SolidBrush(Color.Black),
                    LABEL_COL_WIDTH - width - PADDING,
                    LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT - height) / 2);
            }


            /// <summary>
            /// Determines which cell, if any, was clicked.  Generates a SelectionChanged event.  All of
            /// the indexes are zero based.
            /// </summary>
            /// <param name="e"></param>

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnClick(e);
                int x = (e.X - LABEL_COL_WIDTH) / DATA_COL_WIDTH;
                int y = (e.Y - LABEL_ROW_HEIGHT) / DATA_ROW_HEIGHT;
                if (e.X > LABEL_COL_WIDTH && e.Y > LABEL_ROW_HEIGHT && (x + _firstColumn < COL_COUNT) && (y + _firstRow < ROW_COUNT))
                {
                    _selectedCol = x + _firstColumn;
                    _selectedRow = y + _firstRow;
                    if (_ssp.SelectionChanged != null)
                    {
                        _ssp.SelectionChanged(_ssp);
                    }
                }
                Invalidate();
            }
            /// <summary>
            /// Sets the text field of the textbox with contents which can represent either a string, a double, or a formula.
            /// </summary>
            /// <param name="contents"></param>
            public void SetTextBoxContents(string contents)
            {
                CurrentCell.Text = contents;
                CurrentCell.SelectAll();
            }
        }

    }
}