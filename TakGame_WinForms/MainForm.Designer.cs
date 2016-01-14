namespace TakGame_WinForms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panelControls = new System.Windows.Forms.Panel();
            this.tablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.player2Controls = new System.Windows.Forms.Panel();
            this.btnCap1 = new System.Windows.Forms.Button();
            this.btnStand1 = new System.Windows.Forms.Button();
            this.btnFlat1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.player1Controls = new System.Windows.Forms.Panel();
            this.btnCap0 = new System.Windows.Forms.Button();
            this.btnStand0 = new System.Windows.Forms.Button();
            this.btnFlat0 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnRotate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.lblAiLevel = new System.Windows.Forms.ToolStripLabel();
            this.listAiLevel = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lblStatus = new System.Windows.Forms.ToolStripLabel();
            this.btnCancel = new System.Windows.Forms.ToolStripButton();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.miFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.miMove = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveEnter = new System.Windows.Forms.ToolStripMenuItem();
            this.historyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.miMovePrev = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveNext = new System.Windows.Forms.ToolStripMenuItem();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.panelControls.SuspendLayout();
            this.tablePanel.SuspendLayout();
            this.player2Controls.SuspendLayout();
            this.player1Controls.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControls
            // 
            this.panelControls.BackColor = System.Drawing.Color.Linen;
            this.panelControls.Controls.Add(this.tablePanel);
            this.panelControls.Controls.Add(this.toolStrip);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControls.Location = new System.Drawing.Point(0, 24);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(577, 131);
            this.panelControls.TabIndex = 0;
            // 
            // tablePanel
            // 
            this.tablePanel.ColumnCount = 2;
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePanel.Controls.Add(this.player2Controls, 1, 0);
            this.tablePanel.Controls.Add(this.player1Controls, 0, 0);
            this.tablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel.Location = new System.Drawing.Point(0, 25);
            this.tablePanel.Margin = new System.Windows.Forms.Padding(0);
            this.tablePanel.Name = "tablePanel";
            this.tablePanel.RowCount = 1;
            this.tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 106F));
            this.tablePanel.Size = new System.Drawing.Size(577, 106);
            this.tablePanel.TabIndex = 1;
            // 
            // player2Controls
            // 
            this.player2Controls.Controls.Add(this.btnCap1);
            this.player2Controls.Controls.Add(this.btnStand1);
            this.player2Controls.Controls.Add(this.btnFlat1);
            this.player2Controls.Controls.Add(this.label2);
            this.player2Controls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.player2Controls.Location = new System.Drawing.Point(291, 3);
            this.player2Controls.Name = "player2Controls";
            this.player2Controls.Size = new System.Drawing.Size(283, 100);
            this.player2Controls.TabIndex = 1;
            // 
            // btnCap1
            // 
            this.btnCap1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCap1.BackColor = System.Drawing.Color.Transparent;
            this.btnCap1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCap1.Location = new System.Drawing.Point(28, 17);
            this.btnCap1.Name = "btnCap1";
            this.btnCap1.Padding = new System.Windows.Forms.Padding(2);
            this.btnCap1.Size = new System.Drawing.Size(80, 80);
            this.btnCap1.TabIndex = 4;
            this.btnCap1.Text = "Flat x 20";
            this.btnCap1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCap1.UseVisualStyleBackColor = false;
            // 
            // btnStand1
            // 
            this.btnStand1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStand1.BackColor = System.Drawing.Color.Transparent;
            this.btnStand1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnStand1.Location = new System.Drawing.Point(114, 16);
            this.btnStand1.Name = "btnStand1";
            this.btnStand1.Padding = new System.Windows.Forms.Padding(2);
            this.btnStand1.Size = new System.Drawing.Size(80, 80);
            this.btnStand1.TabIndex = 3;
            this.btnStand1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStand1.UseVisualStyleBackColor = false;
            // 
            // btnFlat1
            // 
            this.btnFlat1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFlat1.BackColor = System.Drawing.Color.Transparent;
            this.btnFlat1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnFlat1.Location = new System.Drawing.Point(200, 16);
            this.btnFlat1.Name = "btnFlat1";
            this.btnFlat1.Padding = new System.Windows.Forms.Padding(2);
            this.btnFlat1.Size = new System.Drawing.Size(80, 80);
            this.btnFlat1.TabIndex = 2;
            this.btnFlat1.Text = "Flat x 20";
            this.btnFlat1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnFlat1.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(283, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Player 2\'s reserve";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // player1Controls
            // 
            this.player1Controls.Controls.Add(this.btnCap0);
            this.player1Controls.Controls.Add(this.btnStand0);
            this.player1Controls.Controls.Add(this.btnFlat0);
            this.player1Controls.Controls.Add(this.label1);
            this.player1Controls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.player1Controls.Location = new System.Drawing.Point(3, 3);
            this.player1Controls.Name = "player1Controls";
            this.player1Controls.Size = new System.Drawing.Size(282, 100);
            this.player1Controls.TabIndex = 0;
            // 
            // btnCap0
            // 
            this.btnCap0.BackColor = System.Drawing.Color.Transparent;
            this.btnCap0.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCap0.Location = new System.Drawing.Point(175, 16);
            this.btnCap0.Name = "btnCap0";
            this.btnCap0.Padding = new System.Windows.Forms.Padding(2);
            this.btnCap0.Size = new System.Drawing.Size(80, 80);
            this.btnCap0.TabIndex = 3;
            this.btnCap0.Text = "Flat x 20";
            this.btnCap0.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCap0.UseVisualStyleBackColor = false;
            // 
            // btnStand0
            // 
            this.btnStand0.BackColor = System.Drawing.Color.Transparent;
            this.btnStand0.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnStand0.Location = new System.Drawing.Point(89, 16);
            this.btnStand0.Name = "btnStand0";
            this.btnStand0.Padding = new System.Windows.Forms.Padding(2);
            this.btnStand0.Size = new System.Drawing.Size(80, 80);
            this.btnStand0.TabIndex = 2;
            this.btnStand0.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStand0.UseVisualStyleBackColor = false;
            // 
            // btnFlat0
            // 
            this.btnFlat0.BackColor = System.Drawing.Color.Transparent;
            this.btnFlat0.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnFlat0.Location = new System.Drawing.Point(3, 16);
            this.btnFlat0.Name = "btnFlat0";
            this.btnFlat0.Padding = new System.Windows.Forms.Padding(2);
            this.btnFlat0.Size = new System.Drawing.Size(80, 80);
            this.btnFlat0.TabIndex = 1;
            this.btnFlat0.Text = "Flat x 20";
            this.btnFlat0.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnFlat0.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(282, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Player 1\'s reserve";
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnUndo,
            this.btnRotate,
            this.toolStripSeparator2,
            this.lblAiLevel,
            this.listAiLevel,
            this.toolStripSeparator1,
            this.lblStatus,
            this.btnCancel});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(577, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnNew
            // 
            this.btnNew.Image = global::TakGame_WinForms.Properties.Resources._new;
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(51, 22);
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Image = global::TakGame_WinForms.Properties.Resources.edit_undo;
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(56, 22);
            this.btnUndo.Text = "Undo";
            this.btnUndo.ToolTipText = "Undo last move";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRotate
            // 
            this.btnRotate.Image = global::TakGame_WinForms.Properties.Resources.rotate;
            this.btnRotate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.Size = new System.Drawing.Size(61, 22);
            this.btnRotate.Text = "Rotate";
            this.btnRotate.ToolTipText = "Rotate board view by 90 degrees";
            this.btnRotate.Click += new System.EventHandler(this.btnRotate_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // lblAiLevel
            // 
            this.lblAiLevel.Name = "lblAiLevel";
            this.lblAiLevel.Size = new System.Drawing.Size(51, 22);
            this.lblAiLevel.Text = "AI Level:";
            // 
            // listAiLevel
            // 
            this.listAiLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listAiLevel.Items.AddRange(new object[] {
            "Off",
            "Level 3",
            "Level 4"});
            this.listAiLevel.Name = "listAiLevel";
            this.listAiLevel.Size = new System.Drawing.Size(75, 25);
            this.listAiLevel.SelectedIndexChanged += new System.EventHandler(this.listAiLevel_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 22);
            this.lblStatus.Text = "Ready";
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::TakGame_WinForms.Properties.Resources.stop;
            this.btnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(63, 22);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miMove});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(577, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "menuStrip1";
            // 
            // miFile
            // 
            this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileOpen,
            this.miFileSave,
            this.miFileSaveAs});
            this.miFile.Name = "miFile";
            this.miFile.Size = new System.Drawing.Size(37, 20);
            this.miFile.Text = "&File";
            // 
            // miFileOpen
            // 
            this.miFileOpen.Name = "miFileOpen";
            this.miFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.miFileOpen.Size = new System.Drawing.Size(155, 22);
            this.miFileOpen.Text = "&Open...";
            this.miFileOpen.Click += new System.EventHandler(this.miFileOpen_Click);
            // 
            // miFileSave
            // 
            this.miFileSave.Name = "miFileSave";
            this.miFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miFileSave.Size = new System.Drawing.Size(155, 22);
            this.miFileSave.Text = "&Save";
            this.miFileSave.Click += new System.EventHandler(this.miFileSave_Click);
            // 
            // miFileSaveAs
            // 
            this.miFileSaveAs.Name = "miFileSaveAs";
            this.miFileSaveAs.Size = new System.Drawing.Size(155, 22);
            this.miFileSaveAs.Text = "Save &as...";
            this.miFileSaveAs.Click += new System.EventHandler(this.miFileSaveAs_Click);
            // 
            // miMove
            // 
            this.miMove.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMoveEnter,
            this.historyToolStripMenuItem,
            this.toolStripMenuItem1,
            this.miMovePrev,
            this.miMoveNext});
            this.miMove.Name = "miMove";
            this.miMove.Size = new System.Drawing.Size(49, 20);
            this.miMove.Text = "&Move";
            // 
            // miMoveEnter
            // 
            this.miMoveEnter.Name = "miMoveEnter";
            this.miMoveEnter.ShortcutKeyDisplayString = "e";
            this.miMoveEnter.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.miMoveEnter.Size = new System.Drawing.Size(197, 22);
            this.miMoveEnter.Text = "&Enter notation...";
            this.miMoveEnter.Click += new System.EventHandler(this.miMoveEnter_Click);
            // 
            // historyToolStripMenuItem
            // 
            this.historyToolStripMenuItem.Name = "historyToolStripMenuItem";
            this.historyToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.historyToolStripMenuItem.Text = "&History";
            this.historyToolStripMenuItem.Click += new System.EventHandler(this.historyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(194, 6);
            // 
            // miMovePrev
            // 
            this.miMovePrev.Name = "miMovePrev";
            this.miMovePrev.ShortcutKeyDisplayString = "left";
            this.miMovePrev.Size = new System.Drawing.Size(197, 22);
            this.miMovePrev.Text = "Navigate back";
            this.miMovePrev.Click += new System.EventHandler(this.miMovePrev_Click);
            // 
            // miMoveNext
            // 
            this.miMoveNext.Name = "miMoveNext";
            this.miMoveNext.ShortcutKeyDisplayString = "right";
            this.miMoveNext.Size = new System.Drawing.Size(197, 22);
            this.miMoveNext.Text = "Navigate forward";
            this.miMoveNext.Click += new System.EventHandler(this.miMoveNext_Click);
            // 
            // dlgSave
            // 
            this.dlgSave.DefaultExt = "ptn";
            this.dlgSave.Filter = "Portable Tak Notation Files (*.ptn)|*.ptn";
            this.dlgSave.Title = "Save Tak notation as";
            // 
            // dlgOpen
            // 
            this.dlgOpen.Filter = "Portable Tak Notation Files (*.ptn)|*.ptn";
            this.dlgOpen.Title = "Open Tak notation file";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 511);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.mainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "Tak";
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.tablePanel.ResumeLayout(false);
            this.player2Controls.ResumeLayout(false);
            this.player1Controls.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.TableLayoutPanel tablePanel;
        private System.Windows.Forms.Panel player2Controls;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel player1Controls;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.Button btnFlat0;
        private System.Windows.Forms.Button btnCap1;
        private System.Windows.Forms.Button btnStand1;
        private System.Windows.Forms.Button btnFlat1;
        private System.Windows.Forms.Button btnCap0;
        private System.Windows.Forms.Button btnStand0;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel lblStatus;
        private System.Windows.Forms.ToolStripButton btnCancel;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel lblAiLevel;
        private System.Windows.Forms.ToolStripComboBox listAiLevel;
        private System.Windows.Forms.ToolStripButton btnRotate;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem miFile;
        private System.Windows.Forms.ToolStripMenuItem miFileOpen;
        private System.Windows.Forms.ToolStripMenuItem miFileSave;
        private System.Windows.Forms.ToolStripMenuItem miFileSaveAs;
        private System.Windows.Forms.SaveFileDialog dlgSave;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.ToolStripMenuItem miMove;
        private System.Windows.Forms.ToolStripMenuItem miMoveEnter;
        private System.Windows.Forms.ToolStripMenuItem historyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem miMovePrev;
        private System.Windows.Forms.ToolStripMenuItem miMoveNext;
    }
}

