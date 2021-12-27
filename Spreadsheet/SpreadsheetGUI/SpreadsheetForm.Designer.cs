
namespace SpreadsheetGUI
{
    partial class Spreadsheet1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.EnterContents = new System.Windows.Forms.Button();
            this.ContentsForCell = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSpreadsheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.selectingCellsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editingCellsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formulaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.featureInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CellName = new System.Windows.Forms.Label();
            this.SavedState = new System.Windows.Forms.Label();
            this.CellValue = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.CurrentSpreadSheetPanel = new SS.SpreadsheetPanel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // EnterContents
            // 
            this.EnterContents.Location = new System.Drawing.Point(190, 30);
            this.EnterContents.Name = "EnterContents";
            this.EnterContents.Size = new System.Drawing.Size(75, 23);
            this.EnterContents.TabIndex = 1;
            this.EnterContents.Text = "Enter";
            this.EnterContents.UseVisualStyleBackColor = true;
            this.EnterContents.Click += new System.EventHandler(this.EnterContents_Click);
            // 
            // ContentsForCell
            // 
            this.ContentsForCell.Location = new System.Drawing.Point(0, 30);
            this.ContentsForCell.Name = "ContentsForCell";
            this.ContentsForCell.Size = new System.Drawing.Size(160, 22);
            this.ContentsForCell.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(800, 28);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.newSpreadsheetToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // newSpreadsheetToolStripMenuItem
            // 
            this.newSpreadsheetToolStripMenuItem.Name = "newSpreadsheetToolStripMenuItem";
            this.newSpreadsheetToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.newSpreadsheetToolStripMenuItem.Text = "New Spreadsheet";
            this.newSpreadsheetToolStripMenuItem.Click += new System.EventHandler(this.newSpreadsheetToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectingCellsToolStripMenuItem,
            this.editingCellsToolStripMenuItem,
            this.formulaToolStripMenuItem,
            this.featureInfoToolStripMenuItem});
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem1.Text = "Help";
            // 
            // selectingCellsToolStripMenuItem
            // 
            this.selectingCellsToolStripMenuItem.Name = "selectingCellsToolStripMenuItem";
            this.selectingCellsToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.selectingCellsToolStripMenuItem.Text = "Selecting Cells";
            this.selectingCellsToolStripMenuItem.Click += new System.EventHandler(this.selectingCellsToolStripMenuItem_Click);
            // 
            // editingCellsToolStripMenuItem
            // 
            this.editingCellsToolStripMenuItem.Name = "editingCellsToolStripMenuItem";
            this.editingCellsToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.editingCellsToolStripMenuItem.Text = "Editing Cells";
            this.editingCellsToolStripMenuItem.Click += new System.EventHandler(this.editingCellsToolStripMenuItem_Click);
            // 
            // formulaToolStripMenuItem
            // 
            this.formulaToolStripMenuItem.Name = "formulaToolStripMenuItem";
            this.formulaToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.formulaToolStripMenuItem.Text = "Formula";
            this.formulaToolStripMenuItem.Click += new System.EventHandler(this.formulaToolStripMenuItem_Click);
            // 
            // featureInfoToolStripMenuItem
            // 
            this.featureInfoToolStripMenuItem.Name = "featureInfoToolStripMenuItem";
            this.featureInfoToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.featureInfoToolStripMenuItem.Text = "Feature Info";
            this.featureInfoToolStripMenuItem.Click += new System.EventHandler(this.featureInfoToolStripMenuItem_Click);
            // 
            // CellName
            // 
            this.CellName.AutoSize = true;
            this.CellName.Location = new System.Drawing.Point(50, 55);
            this.CellName.Name = "CellName";
            this.CellName.Size = new System.Drawing.Size(25, 17);
            this.CellName.TabIndex = 4;
            this.CellName.Text = "A1";
            // 
            // SavedState
            // 
            this.SavedState.AutoSize = true;
            this.SavedState.Location = new System.Drawing.Point(334, 5);
            this.SavedState.Name = "SavedState";
            this.SavedState.Size = new System.Drawing.Size(56, 17);
            this.SavedState.TabIndex = 5;
            this.SavedState.Text = "Untitled";
            // 
            // CellValue
            // 
            this.CellValue.AutoSize = true;
            this.CellValue.Location = new System.Drawing.Point(105, 55);
            this.CellValue.Name = "CellValue";
            this.CellValue.Size = new System.Drawing.Size(18, 17);
            this.CellValue.TabIndex = 6;
            this.CellValue.Text = "\"\"";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "sprd";
            this.saveFileDialog1.FileName = "Untitled.sprd";
            this.saveFileDialog1.Filter = "Spreadsheet files (*.sprd)|*.sprd|All files (*.*)|*.*";
            this.saveFileDialog1.InitialDirectory = "c:\\";
            this.saveFileDialog1.Title = "Save to File";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Spreadsheet files (*.sprd)|*.sprd|All files (*.*)|*.*";
            this.openFileDialog1.InitialDirectory = "c:\\";
            this.openFileDialog1.Title = "Open FIle";
            // 
            // CurrentSpreadSheetPanel
            // 
            this.CurrentSpreadSheetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentSpreadSheetPanel.Cursor = System.Windows.Forms.Cursors.Cross;
            this.CurrentSpreadSheetPanel.Location = new System.Drawing.Point(0, 80);
            this.CurrentSpreadSheetPanel.Name = "CurrentSpreadSheetPanel";
            this.CurrentSpreadSheetPanel.Size = new System.Drawing.Size(800, 372);
            this.CurrentSpreadSheetPanel.TabIndex = 0;
            // 
            // Spreadsheet1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.CellValue);
            this.Controls.Add(this.SavedState);
            this.Controls.Add(this.CellName);
            this.Controls.Add(this.ContentsForCell);
            this.Controls.Add(this.EnterContents);
            this.Controls.Add(this.CurrentSpreadSheetPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Spreadsheet1";
            this.Text = "Spreadsheet";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel CurrentSpreadSheetPanel;
        private System.Windows.Forms.Button EnterContents;
        private System.Windows.Forms.TextBox ContentsForCell;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newSpreadsheetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Label CellName;
        private System.Windows.Forms.Label SavedState;
        private System.Windows.Forms.Label CellValue;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem selectingCellsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editingCellsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formulaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem featureInfoToolStripMenuItem;
    }
}

