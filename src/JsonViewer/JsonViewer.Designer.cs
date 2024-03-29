using System.Windows.Forms;

namespace EPocalipse.Json.Viewer
{
    partial class JsonViewer
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JsonViewer));
            this.mnuTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFind = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopyValue = new System.Windows.Forms.ToolStripMenuItem();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.mnuVisualizerPnl = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuShowOnRight = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShowOnBottom = new System.Windows.Forms.ToolStripMenuItem();
            this.pageTextView = new System.Windows.Forms.TabPage();
            this.txtJson = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnPaste = new System.Windows.Forms.ToolStripButton();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFormat = new System.Windows.Forms.ToolStripButton();
            this.btnStrip = new System.Windows.Forms.ToolStripSplitButton();
            this.btnStripToCurly = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStripToSqr = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.removenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSpecialCharsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblError = new System.Windows.Forms.LinkLabel();
            this.pageTreeView = new System.Windows.Forms.TabPage();
            this.spcViewer = new System.Windows.Forms.SplitContainer();
            this.tvJson = new System.Windows.Forms.TreeView();
            this.pnlFind = new System.Windows.Forms.Panel();
            this.btnCloseFind = new System.Windows.Forms.Button();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.lblFind = new System.Windows.Forms.Label();
            this.pnlVisualizer = new System.Windows.Forms.Panel();
            this.cbVisualizers = new System.Windows.Forms.ComboBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageIn = new System.Windows.Forms.TabPage();
            this.listViewIn = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPageOut = new System.Windows.Forms.TabPage();
            this.listViewOut = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuTree.SuspendLayout();
            this.mnuVisualizerPnl.SuspendLayout();
            this.pageTextView.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.pageTreeView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcViewer)).BeginInit();
            this.spcViewer.Panel1.SuspendLayout();
            this.spcViewer.Panel2.SuspendLayout();
            this.spcViewer.SuspendLayout();
            this.pnlFind.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageIn.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tabPageOut.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuTree
            // 
            this.mnuTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator2,
            this.mnuFind,
            this.mnuExpandAll,
            this.toolStripMenuItem1,
            this.mnuCopy,
            this.mnuCopyValue});
            this.mnuTree.Name = "mnuTree";
            this.mnuTree.Size = new System.Drawing.Size(143, 148);
            this.mnuTree.Opening += new System.ComponentModel.CancelEventHandler(this.mnuTree_Opening);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(139, 6);
            // 
            // mnuFind
            // 
            this.mnuFind.Name = "mnuFind";
            this.mnuFind.Size = new System.Drawing.Size(142, 22);
            this.mnuFind.Text = "&Find";
            this.mnuFind.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // mnuExpandAll
            // 
            this.mnuExpandAll.Name = "mnuExpandAll";
            this.mnuExpandAll.Size = new System.Drawing.Size(142, 22);
            this.mnuExpandAll.Text = "Expand &All";
            this.mnuExpandAll.Click += new System.EventHandler(this.expandallToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(139, 6);
            // 
            // mnuCopy
            // 
            this.mnuCopy.Name = "mnuCopy";
            this.mnuCopy.Size = new System.Drawing.Size(142, 22);
            this.mnuCopy.Text = "&Copy";
            this.mnuCopy.Click += new System.EventHandler(this.mnuCopy_Click);
            // 
            // mnuCopyValue
            // 
            this.mnuCopyValue.Name = "mnuCopyValue";
            this.mnuCopyValue.Size = new System.Drawing.Size(142, 22);
            this.mnuCopyValue.Text = "Copy &Value";
            this.mnuCopyValue.Click += new System.EventHandler(this.mnuCopyValue_Click);
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.White;
            this.imgList.Images.SetKeyName(0, "obj.bmp");
            this.imgList.Images.SetKeyName(1, "array.bmp");
            this.imgList.Images.SetKeyName(2, "prop.bmp");
            // 
            // mnuVisualizerPnl
            // 
            this.mnuVisualizerPnl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShowOnRight,
            this.mnuShowOnBottom});
            this.mnuVisualizerPnl.Name = "mnuVisualizerPnl";
            this.mnuVisualizerPnl.Size = new System.Drawing.Size(120, 48);
            // 
            // mnuShowOnRight
            // 
            this.mnuShowOnRight.Checked = true;
            this.mnuShowOnRight.CheckOnClick = true;
            this.mnuShowOnRight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuShowOnRight.Name = "mnuShowOnRight";
            this.mnuShowOnRight.Size = new System.Drawing.Size(119, 22);
            this.mnuShowOnRight.Text = "&Right";
            this.mnuShowOnRight.Click += new System.EventHandler(this.rightToolStripMenuItem_Click);
            // 
            // mnuShowOnBottom
            // 
            this.mnuShowOnBottom.CheckOnClick = true;
            this.mnuShowOnBottom.Name = "mnuShowOnBottom";
            this.mnuShowOnBottom.Size = new System.Drawing.Size(119, 22);
            this.mnuShowOnBottom.Text = "&Bottom";
            this.mnuShowOnBottom.Click += new System.EventHandler(this.rightToolStripMenuItem_Click);
            // 
            // pageTextView
            // 
            this.pageTextView.Controls.Add(this.txtJson);
            this.pageTextView.Controls.Add(this.toolStrip1);
            this.pageTextView.Controls.Add(this.lblError);
            this.pageTextView.Location = new System.Drawing.Point(4, 22);
            this.pageTextView.Name = "pageTextView";
            this.pageTextView.Padding = new System.Windows.Forms.Padding(3);
            this.pageTextView.Size = new System.Drawing.Size(784, 532);
            this.pageTextView.TabIndex = 1;
            this.pageTextView.Text = "Text";
            this.pageTextView.UseVisualStyleBackColor = true;
            // 
            // txtJson
            // 
            this.txtJson.AcceptsReturn = true;
            this.txtJson.AcceptsTab = true;
            this.txtJson.BackColor = System.Drawing.SystemColors.Info;
            this.txtJson.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtJson.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtJson.HideSelection = false;
            this.txtJson.Location = new System.Drawing.Point(3, 28);
            this.txtJson.MaxLength = 0;
            this.txtJson.Multiline = true;
            this.txtJson.Name = "txtJson";
            this.txtJson.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtJson.Size = new System.Drawing.Size(778, 479);
            this.txtJson.TabIndex = 4;
            this.txtJson.TextChanged += new System.EventHandler(this.txtJson_TextChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPaste,
            this.btnCopy,
            this.toolStripSeparator1,
            this.btnFormat,
            this.btnStrip,
            this.toolStripSplitButton1});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(778, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnPaste
            // 
            this.btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPaste.Image = ((System.Drawing.Image)(resources.GetObject("btnPaste.Image")));
            this.btnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(43, 22);
            this.btnPaste.Text = "&Paste";
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnCopy.Image")));
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(42, 22);
            this.btnCopy.Text = "&Copy";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnFormat
            // 
            this.btnFormat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFormat.Image = ((System.Drawing.Image)(resources.GetObject("btnFormat.Image")));
            this.btnFormat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.Size = new System.Drawing.Size(53, 22);
            this.btnFormat.Text = "&Format";
            this.btnFormat.Click += new System.EventHandler(this.btnFormat_Click);
            // 
            // btnStrip
            // 
            this.btnStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStripToCurly,
            this.btnStripToSqr});
            this.btnStrip.Image = ((System.Drawing.Image)(resources.GetObject("btnStrip.Image")));
            this.btnStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStrip.Name = "btnStrip";
            this.btnStrip.Size = new System.Drawing.Size(79, 22);
            this.btnStrip.Text = "Strip to {}";
            this.btnStrip.ButtonClick += new System.EventHandler(this.btnStripToCurly_Click);
            // 
            // btnStripToCurly
            // 
            this.btnStripToCurly.Name = "btnStripToCurly";
            this.btnStripToCurly.Size = new System.Drawing.Size(131, 22);
            this.btnStripToCurly.Text = "Strip to {}";
            this.btnStripToCurly.Click += new System.EventHandler(this.btnStripToCurly_Click);
            // 
            // btnStripToSqr
            // 
            this.btnStripToSqr.Name = "btnStripToSqr";
            this.btnStripToSqr.Size = new System.Drawing.Size(131, 22);
            this.btnStripToSqr.Text = "Strip to []";
            this.btnStripToSqr.Click += new System.EventHandler(this.btnStripToSqr_Click);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removenToolStripMenuItem,
            this.removeSpecialCharsToolStripMenuItem});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(152, 22);
            this.toolStripSplitButton1.Text = "Remove new lines (\\n)";
            this.toolStripSplitButton1.ButtonClick += new System.EventHandler(this.removeNewLineMenuItem_Click);
            // 
            // removenToolStripMenuItem
            // 
            this.removenToolStripMenuItem.Name = "removenToolStripMenuItem";
            this.removenToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.removenToolStripMenuItem.Text = "Remove new lines (\\n)";
            this.removenToolStripMenuItem.Click += new System.EventHandler(this.removeNewLineMenuItem_Click);
            // 
            // removeSpecialCharsToolStripMenuItem
            // 
            this.removeSpecialCharsToolStripMenuItem.Name = "removeSpecialCharsToolStripMenuItem";
            this.removeSpecialCharsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.removeSpecialCharsToolStripMenuItem.Text = "Remove special chars (\\)";
            this.removeSpecialCharsToolStripMenuItem.Click += new System.EventHandler(this.removeSpecialCharsToolStripMenuItem_Click);
            // 
            // lblError
            // 
            this.lblError.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.lblError.LinkColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(3, 507);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(778, 22);
            this.lblError.TabIndex = 5;
            this.lblError.TabStop = true;
            this.lblError.Text = "aa";
            this.lblError.VisitedLinkColor = System.Drawing.Color.Red;
            this.lblError.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblError_LinkClicked);
            // 
            // pageTreeView
            // 
            this.pageTreeView.Controls.Add(this.spcViewer);
            this.pageTreeView.Location = new System.Drawing.Point(4, 22);
            this.pageTreeView.Name = "pageTreeView";
            this.pageTreeView.Padding = new System.Windows.Forms.Padding(3);
            this.pageTreeView.Size = new System.Drawing.Size(784, 532);
            this.pageTreeView.TabIndex = 0;
            this.pageTreeView.Text = "Viewer";
            this.pageTreeView.UseVisualStyleBackColor = true;
            // 
            // spcViewer
            // 
            this.spcViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcViewer.Location = new System.Drawing.Point(3, 3);
            this.spcViewer.Name = "spcViewer";
            // 
            // spcViewer.Panel1
            // 
            this.spcViewer.Panel1.Controls.Add(this.tvJson);
            this.spcViewer.Panel1.Controls.Add(this.pnlFind);
            // 
            // spcViewer.Panel2
            // 
            this.spcViewer.Panel2.Controls.Add(this.pnlVisualizer);
            this.spcViewer.Panel2.Controls.Add(this.cbVisualizers);
            this.spcViewer.Size = new System.Drawing.Size(778, 526);
            this.spcViewer.SplitterDistance = 560;
            this.spcViewer.TabIndex = 5;
            // 
            // tvJson
            // 
            this.tvJson.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvJson.ContextMenuStrip = this.mnuTree;
            this.tvJson.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvJson.HideSelection = false;
            this.tvJson.ImageIndex = 0;
            this.tvJson.ImageList = this.imgList;
            this.tvJson.Location = new System.Drawing.Point(0, 0);
            this.tvJson.Name = "tvJson";
            this.tvJson.SelectedImageIndex = 0;
            this.tvJson.Size = new System.Drawing.Size(560, 496);
            this.tvJson.TabIndex = 3;
            this.tvJson.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvJson_BeforeExpand);
            this.tvJson.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvJson_AfterSelect);
            this.tvJson.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JsonViewer_KeyDown);
            this.tvJson.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvJson_MouseDown);
            // 
            // pnlFind
            // 
            this.pnlFind.Controls.Add(this.btnCloseFind);
            this.pnlFind.Controls.Add(this.txtFind);
            this.pnlFind.Controls.Add(this.lblFind);
            this.pnlFind.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFind.Location = new System.Drawing.Point(0, 496);
            this.pnlFind.Name = "pnlFind";
            this.pnlFind.Size = new System.Drawing.Size(560, 30);
            this.pnlFind.TabIndex = 6;
            this.pnlFind.Visible = false;
            // 
            // btnCloseFind
            // 
            this.btnCloseFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseFind.Location = new System.Drawing.Point(420, 6);
            this.btnCloseFind.Name = "btnCloseFind";
            this.btnCloseFind.Size = new System.Drawing.Size(16, 15);
            this.btnCloseFind.TabIndex = 2;
            this.btnCloseFind.Click += new System.EventHandler(this.btnCloseFind_Click);
            // 
            // txtFind
            // 
            this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFind.Location = new System.Drawing.Point(32, 6);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(374, 21);
            this.txtFind.TabIndex = 1;
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFind_KeyDown);
            // 
            // lblFind
            // 
            this.lblFind.AutoSize = true;
            this.lblFind.Location = new System.Drawing.Point(3, 8);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(29, 12);
            this.lblFind.TabIndex = 0;
            this.lblFind.Text = "&Find";
            // 
            // pnlVisualizer
            // 
            this.pnlVisualizer.ContextMenuStrip = this.mnuVisualizerPnl;
            this.pnlVisualizer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVisualizer.Location = new System.Drawing.Point(0, 29);
            this.pnlVisualizer.Name = "pnlVisualizer";
            this.pnlVisualizer.Size = new System.Drawing.Size(214, 497);
            this.pnlVisualizer.TabIndex = 6;
            // 
            // cbVisualizers
            // 
            this.cbVisualizers.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbVisualizers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbVisualizers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVisualizers.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbVisualizers.FormattingEnabled = true;
            this.cbVisualizers.ItemHeight = 23;
            this.cbVisualizers.Location = new System.Drawing.Point(0, 0);
            this.cbVisualizers.Name = "cbVisualizers";
            this.cbVisualizers.Size = new System.Drawing.Size(214, 29);
            this.cbVisualizers.Sorted = true;
            this.cbVisualizers.TabIndex = 7;
            this.cbVisualizers.SelectedIndexChanged += new System.EventHandler(this.cbVisualizers_SelectedIndexChanged);
            this.cbVisualizers.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cbVisualizers_Format);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.pageTreeView);
            this.tabControl.Controls.Add(this.pageTextView);
            this.tabControl.Controls.Add(this.tabPageIn);
            this.tabControl.Controls.Add(this.tabPageOut);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(792, 558);
            this.tabControl.TabIndex = 6;
            // 
            // tabPageIn
            // 
            this.tabPageIn.Controls.Add(this.listViewIn);
            this.tabPageIn.Location = new System.Drawing.Point(4, 22);
            this.tabPageIn.Name = "tabPageIn";
            this.tabPageIn.Size = new System.Drawing.Size(784, 532);
            this.tabPageIn.TabIndex = 3;
            this.tabPageIn.Text = "In";
            this.tabPageIn.UseVisualStyleBackColor = true;
            // 
            // listViewIn
            // 
            this.listViewIn.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewIn.ContextMenuStrip = this.contextMenuStrip1;
            this.listViewIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewIn.FullRowSelect = true;
            this.listViewIn.GridLines = true;
            this.listViewIn.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewIn.Location = new System.Drawing.Point(0, 0);
            this.listViewIn.Name = "listViewIn";
            this.listViewIn.Size = new System.Drawing.Size(784, 532);
            this.listViewIn.TabIndex = 0;
            this.listViewIn.UseCompatibleStateImageBehavior = false;
            this.listViewIn.View = System.Windows.Forms.View.Details;
            this.listViewIn.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewIn_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time";
            this.columnHeader1.Width = 151;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Header";
            this.columnHeader2.Width = 224;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Body";
            this.columnHeader3.Width = 387;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CopyToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 26);
            // 
            // CopyToolStripMenuItem
            // 
            this.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem";
            this.CopyToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.CopyToolStripMenuItem.Text = "����";
            this.CopyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // tabPageOut
            // 
            this.tabPageOut.Controls.Add(this.listViewOut);
            this.tabPageOut.Location = new System.Drawing.Point(4, 22);
            this.tabPageOut.Name = "tabPageOut";
            this.tabPageOut.Size = new System.Drawing.Size(784, 532);
            this.tabPageOut.TabIndex = 4;
            this.tabPageOut.Text = "Out";
            this.tabPageOut.UseVisualStyleBackColor = true;
            // 
            // listViewOut
            // 
            this.listViewOut.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listViewOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewOut.FullRowSelect = true;
            this.listViewOut.GridLines = true;
            this.listViewOut.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewOut.Location = new System.Drawing.Point(0, 0);
            this.listViewOut.Name = "listViewOut";
            this.listViewOut.Size = new System.Drawing.Size(784, 532);
            this.listViewOut.TabIndex = 0;
            this.listViewOut.UseCompatibleStateImageBehavior = false;
            this.listViewOut.View = System.Windows.Forms.View.Details;
            this.listViewOut.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewIn_MouseDoubleClick);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Time";
            this.columnHeader4.Width = 105;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Headers";
            this.columnHeader5.Width = 199;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "QueryString";
            this.columnHeader6.Width = 460;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // JsonViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Name = "JsonViewer";
            this.Size = new System.Drawing.Size(792, 558);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JsonViewer_KeyDown);
            this.mnuTree.ResumeLayout(false);
            this.mnuVisualizerPnl.ResumeLayout(false);
            this.pageTextView.ResumeLayout(false);
            this.pageTextView.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pageTreeView.ResumeLayout(false);
            this.spcViewer.Panel1.ResumeLayout(false);
            this.spcViewer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcViewer)).EndInit();
            this.spcViewer.ResumeLayout(false);
            this.pnlFind.ResumeLayout(false);
            this.pnlFind.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageIn.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabPageOut.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.ContextMenuStrip mnuTree;
        private System.Windows.Forms.ToolStripMenuItem mnuFind;
        private System.Windows.Forms.ToolStripMenuItem mnuExpandAll;
        private System.Windows.Forms.ContextMenuStrip mnuVisualizerPnl;
        private System.Windows.Forms.ToolStripMenuItem mnuShowOnRight;
        private System.Windows.Forms.ToolStripMenuItem mnuShowOnBottom;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuCopy;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyValue;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.TabPage pageTextView;
        private System.Windows.Forms.TextBox txtJson;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnPaste;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnFormat;
        private System.Windows.Forms.ToolStripSplitButton btnStrip;
        private System.Windows.Forms.ToolStripMenuItem btnStripToCurly;
        private System.Windows.Forms.ToolStripMenuItem btnStripToSqr;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem removenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeSpecialCharsToolStripMenuItem;
        private System.Windows.Forms.LinkLabel lblError;
        private System.Windows.Forms.TabPage pageTreeView;
        private System.Windows.Forms.SplitContainer spcViewer;
        private System.Windows.Forms.TreeView tvJson;
        private System.Windows.Forms.Panel pnlFind;
        private System.Windows.Forms.Button btnCloseFind;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.Panel pnlVisualizer;
        private System.Windows.Forms.ComboBox cbVisualizers;
        public TabControl tabControl;
        private TabPage tabPageIn;
        private ListView listViewIn;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private TabPage tabPageOut;
        public ListView listViewOut;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem CopyToolStripMenuItem;
        private NotifyIcon notifyIcon1;
    }
}
