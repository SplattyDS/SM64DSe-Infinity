namespace SM64DSe
{
    partial class ObjectDatabaseEdtiorForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectDatabaseEdtiorForm));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btnSave = new System.Windows.Forms.ToolStripButton();
			this.btnClose = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btnImportXML = new System.Windows.Forms.ToolStripButton();
			this.btnApplyXML = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.btnSort = new System.Windows.Forms.ToolStripButton();
			this.btnDisplay = new System.Windows.Forms.ToolStripButton();
			this.lstObjects = new System.Windows.Forms.ListBox();
			this.txtObjectName = new System.Windows.Forms.TextBox();
			this.txtInternalName = new System.Windows.Forms.TextBox();
			this.nudObjectID = new System.Windows.Forms.NumericUpDown();
			this.lblObjectName = new System.Windows.Forms.Label();
			this.lblInternalName = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lblXMLData = new System.Windows.Forms.Label();
			this.nudActorID = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtBankReq = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtDlReq = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtDescription = new System.Windows.Forms.RichTextBox();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.cmbRenderer = new System.Windows.Forms.ComboBox();
			this.btnBorder = new System.Windows.Forms.Button();
			this.btnFill = new System.Windows.Forms.Button();
			this.lblBorder = new System.Windows.Forms.Label();
			this.lblFill = new System.Windows.Forms.Label();
			this.lblRenderer1 = new System.Windows.Forms.Label();
			this.txtRenderer1 = new System.Windows.Forms.TextBox();
			this.btnRenderer1 = new System.Windows.Forms.Button();
			this.btnRenderer2 = new System.Windows.Forms.Button();
			this.txtRenderer2 = new System.Windows.Forms.TextBox();
			this.lblRenderer2 = new System.Windows.Forms.Label();
			this.nudOffset1X = new System.Windows.Forms.NumericUpDown();
			this.lblOffset1 = new System.Windows.Forms.Label();
			this.nudOffset1Y = new System.Windows.Forms.NumericUpDown();
			this.nudOffset1Z = new System.Windows.Forms.NumericUpDown();
			this.nudOffset2Z = new System.Windows.Forms.NumericUpDown();
			this.nudOffset2Y = new System.Windows.Forms.NumericUpDown();
			this.lblOffset2 = new System.Windows.Forms.Label();
			this.nudOffset2X = new System.Windows.Forms.NumericUpDown();
			this.lblScale = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.nudScale = new System.Windows.Forms.NumericUpDown();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudObjectID)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudActorID)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset1X)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset1Y)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset1Z)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset2Z)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset2Y)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset2X)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudScale)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.btnClose,
            this.toolStripSeparator1,
            this.btnImportXML,
            this.btnApplyXML,
            this.toolStripSeparator2,
            this.btnSort,
            this.btnDisplay});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1262, 27);
			this.toolStrip1.TabIndex = 10;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// btnSave
			// 
			this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(44, 24);
			this.btnSave.Text = "Save";
			this.btnSave.ToolTipText = "Save the level names and overlay IDs";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnClose
			// 
			this.btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(49, 24);
			this.btnClose.Text = "Close";
			this.btnClose.ToolTipText = "Close the window";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
			// 
			// btnImportXML
			// 
			this.btnImportXML.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnImportXML.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnImportXML.Name = "btnImportXML";
			this.btnImportXML.Size = new System.Drawing.Size(91, 24);
			this.btnImportXML.Text = "Import XML";
			this.btnImportXML.ToolTipText = "Import level names from an XML file";
			this.btnImportXML.Click += new System.EventHandler(this.btnImportXML_Click);
			// 
			// btnApplyXML
			// 
			this.btnApplyXML.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnApplyXML.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnApplyXML.Name = "btnApplyXML";
			this.btnApplyXML.Size = new System.Drawing.Size(85, 24);
			this.btnApplyXML.Text = "Apply XML";
			this.btnApplyXML.ToolTipText = "Import level names from an XML file";
			this.btnApplyXML.Click += new System.EventHandler(this.btnbtnApplyXML_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
			// 
			// btnSort
			// 
			this.btnSort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnSort.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSort.Name = "btnSort";
			this.btnSort.Size = new System.Drawing.Size(130, 24);
			this.btnSort.Text = "Sort by: Object ID";
			this.btnSort.ToolTipText = "Import level names from an XML file";
			this.btnSort.Click += new System.EventHandler(this.btnSort_Click);
			// 
			// btnDisplay
			// 
			this.btnDisplay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnDisplay.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnDisplay.Name = "btnDisplay";
			this.btnDisplay.Size = new System.Drawing.Size(157, 24);
			this.btnDisplay.Text = "Display: Object Name";
			this.btnDisplay.ToolTipText = "Import level names from an XML file";
			this.btnDisplay.Click += new System.EventHandler(this.btnDisplay_Click);
			// 
			// lstObjects
			// 
			this.lstObjects.Font = new System.Drawing.Font("Consolas", 8F);
			this.lstObjects.FormattingEnabled = true;
			this.lstObjects.ItemHeight = 15;
			this.lstObjects.Location = new System.Drawing.Point(12, 60);
			this.lstObjects.Name = "lstObjects";
			this.lstObjects.Size = new System.Drawing.Size(487, 454);
			this.lstObjects.TabIndex = 11;
			this.lstObjects.SelectedIndexChanged += new System.EventHandler(this.lstObjects_SelectedIndexChanged);
			// 
			// txtObjectName
			// 
			this.txtObjectName.Location = new System.Drawing.Point(642, 114);
			this.txtObjectName.Name = "txtObjectName";
			this.txtObjectName.Size = new System.Drawing.Size(326, 22);
			this.txtObjectName.TabIndex = 12;
			this.txtObjectName.TextChanged += new System.EventHandler(this.txtObjectName_TextChanged);
			// 
			// txtInternalName
			// 
			this.txtInternalName.Location = new System.Drawing.Point(642, 142);
			this.txtInternalName.Name = "txtInternalName";
			this.txtInternalName.Size = new System.Drawing.Size(326, 22);
			this.txtInternalName.TabIndex = 13;
			this.txtInternalName.TextChanged += new System.EventHandler(this.txtInternalName_TextChanged);
			// 
			// nudObjectID
			// 
			this.nudObjectID.Location = new System.Drawing.Point(642, 59);
			this.nudObjectID.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
			this.nudObjectID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.nudObjectID.Name = "nudObjectID";
			this.nudObjectID.Size = new System.Drawing.Size(326, 22);
			this.nudObjectID.TabIndex = 14;
			this.nudObjectID.ValueChanged += new System.EventHandler(this.nudObjectID_ValueChanged);
			// 
			// lblObjectName
			// 
			this.lblObjectName.AutoSize = true;
			this.lblObjectName.Location = new System.Drawing.Point(544, 117);
			this.lblObjectName.Name = "lblObjectName";
			this.lblObjectName.Size = new System.Drawing.Size(92, 17);
			this.lblObjectName.TabIndex = 15;
			this.lblObjectName.Text = "Object name:";
			// 
			// lblInternalName
			// 
			this.lblInternalName.AutoSize = true;
			this.lblInternalName.Location = new System.Drawing.Point(538, 145);
			this.lblInternalName.Name = "lblInternalName";
			this.lblInternalName.Size = new System.Drawing.Size(98, 17);
			this.lblInternalName.TabIndex = 16;
			this.lblInternalName.Text = "Internal name:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(566, 61);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 17);
			this.label1.TabIndex = 17;
			this.label1.Text = "Object ID:";
			// 
			// lblXMLData
			// 
			this.lblXMLData.AutoSize = true;
			this.lblXMLData.Location = new System.Drawing.Point(781, 30);
			this.lblXMLData.Name = "lblXMLData";
			this.lblXMLData.Size = new System.Drawing.Size(247, 17);
			this.lblXMLData.TabIndex = 18;
			this.lblXMLData.Text = "SM64DSe Object Database XML Data";
			// 
			// nudActorID
			// 
			this.nudActorID.Location = new System.Drawing.Point(642, 86);
			this.nudActorID.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
			this.nudActorID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.nudActorID.Name = "nudActorID";
			this.nudActorID.Size = new System.Drawing.Size(326, 22);
			this.nudActorID.TabIndex = 19;
			this.nudActorID.ValueChanged += new System.EventHandler(this.nudActorID_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(574, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 17);
			this.label2.TabIndex = 20;
			this.label2.Text = "Actor ID:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(505, 173);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(131, 17);
			this.label4.TabIndex = 24;
			this.label4.Text = "Bank requirements:";
			// 
			// txtBankReq
			// 
			this.txtBankReq.Location = new System.Drawing.Point(642, 170);
			this.txtBankReq.Name = "txtBankReq";
			this.txtBankReq.Size = new System.Drawing.Size(326, 22);
			this.txtBankReq.TabIndex = 23;
			this.txtBankReq.TextChanged += new System.EventHandler(this.txtBankReq_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(519, 201);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(117, 17);
			this.label5.TabIndex = 26;
			this.label5.Text = "DL requirements:";
			// 
			// txtDlReq
			// 
			this.txtDlReq.Location = new System.Drawing.Point(642, 198);
			this.txtDlReq.Name = "txtDlReq";
			this.txtDlReq.Size = new System.Drawing.Size(326, 22);
			this.txtDlReq.TabIndex = 25;
			this.txtDlReq.TextChanged += new System.EventHandler(this.txtDlReq_TextChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(706, 223);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(83, 17);
			this.label3.TabIndex = 27;
			this.label3.Text = "Description:";
			// 
			// txtDescription
			// 
			this.txtDescription.Location = new System.Drawing.Point(505, 243);
			this.txtDescription.Name = "txtDescription";
			this.txtDescription.Size = new System.Drawing.Size(463, 271);
			this.txtDescription.TabIndex = 28;
			this.txtDescription.Text = "";
			this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
			// 
			// txtSearch
			// 
			this.txtSearch.Location = new System.Drawing.Point(117, 32);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(220, 22);
			this.txtSearch.TabIndex = 29;
			this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 35);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(99, 17);
			this.label6.TabIndex = 30;
			this.label6.Text = "Search object:";
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(343, 32);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(75, 23);
			this.btnAdd.TabIndex = 31;
			this.btnAdd.Text = "Add new";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Location = new System.Drawing.Point(424, 32);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(75, 23);
			this.btnRemove.TabIndex = 32;
			this.btnRemove.Text = "Remove";
			this.btnRemove.UseVisualStyleBackColor = true;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// cmbRenderer
			// 
			this.cmbRenderer.FormattingEnabled = true;
			this.cmbRenderer.Location = new System.Drawing.Point(1054, 85);
			this.cmbRenderer.Name = "cmbRenderer";
			this.cmbRenderer.Size = new System.Drawing.Size(121, 24);
			this.cmbRenderer.TabIndex = 35;
			this.cmbRenderer.SelectedIndexChanged += new System.EventHandler(this.cmbRenderer_SelectedIndexChanged);
			// 
			// btnBorder
			// 
			this.btnBorder.Location = new System.Drawing.Point(1095, 116);
			this.btnBorder.Margin = new System.Windows.Forms.Padding(4);
			this.btnBorder.Name = "btnBorder";
			this.btnBorder.Size = new System.Drawing.Size(80, 28);
			this.btnBorder.TabIndex = 36;
			this.btnBorder.Text = "#XXXXXX";
			this.btnBorder.UseVisualStyleBackColor = true;
			this.btnBorder.Visible = false;
			this.btnBorder.Click += new System.EventHandler(this.btnBorder_Click);
			// 
			// btnFill
			// 
			this.btnFill.Location = new System.Drawing.Point(1095, 152);
			this.btnFill.Margin = new System.Windows.Forms.Padding(4);
			this.btnFill.Name = "btnFill";
			this.btnFill.Size = new System.Drawing.Size(80, 28);
			this.btnFill.TabIndex = 37;
			this.btnFill.Text = "#XXXXXX";
			this.btnFill.UseVisualStyleBackColor = true;
			this.btnFill.Visible = false;
			this.btnFill.Click += new System.EventHandler(this.btnFill_Click);
			// 
			// lblBorder
			// 
			this.lblBorder.AutoSize = true;
			this.lblBorder.Location = new System.Drawing.Point(1033, 122);
			this.lblBorder.Name = "lblBorder";
			this.lblBorder.Size = new System.Drawing.Size(55, 17);
			this.lblBorder.TabIndex = 38;
			this.lblBorder.Text = "Border:";
			this.lblBorder.Visible = false;
			// 
			// lblFill
			// 
			this.lblFill.AutoSize = true;
			this.lblFill.Location = new System.Drawing.Point(1059, 158);
			this.lblFill.Name = "lblFill";
			this.lblFill.Size = new System.Drawing.Size(29, 17);
			this.lblFill.TabIndex = 39;
			this.lblFill.Text = "Fill:";
			this.lblFill.Visible = false;
			// 
			// lblRenderer1
			// 
			this.lblRenderer1.Location = new System.Drawing.Point(1051, 173);
			this.lblRenderer1.Name = "lblRenderer1";
			this.lblRenderer1.Size = new System.Drawing.Size(124, 23);
			this.lblRenderer1.TabIndex = 40;
			this.lblRenderer1.Text = "lblRenderer1";
			this.lblRenderer1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// txtRenderer1
			// 
			this.txtRenderer1.Enabled = false;
			this.txtRenderer1.Location = new System.Drawing.Point(984, 198);
			this.txtRenderer1.Name = "txtRenderer1";
			this.txtRenderer1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.txtRenderer1.Size = new System.Drawing.Size(221, 22);
			this.txtRenderer1.TabIndex = 41;
			// 
			// btnRenderer1
			// 
			this.btnRenderer1.Location = new System.Drawing.Point(1211, 197);
			this.btnRenderer1.Name = "btnRenderer1";
			this.btnRenderer1.Size = new System.Drawing.Size(34, 23);
			this.btnRenderer1.TabIndex = 42;
			this.btnRenderer1.Text = "...";
			this.btnRenderer1.UseVisualStyleBackColor = true;
			this.btnRenderer1.Click += new System.EventHandler(this.btnRenderer1_Click);
			// 
			// btnRenderer2
			// 
			this.btnRenderer2.Location = new System.Drawing.Point(1211, 253);
			this.btnRenderer2.Name = "btnRenderer2";
			this.btnRenderer2.Size = new System.Drawing.Size(34, 23);
			this.btnRenderer2.TabIndex = 45;
			this.btnRenderer2.Text = "...";
			this.btnRenderer2.UseVisualStyleBackColor = true;
			this.btnRenderer2.Click += new System.EventHandler(this.btnRenderer2_Click);
			// 
			// txtRenderer2
			// 
			this.txtRenderer2.Enabled = false;
			this.txtRenderer2.Location = new System.Drawing.Point(984, 254);
			this.txtRenderer2.Name = "txtRenderer2";
			this.txtRenderer2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.txtRenderer2.Size = new System.Drawing.Size(221, 22);
			this.txtRenderer2.TabIndex = 44;
			// 
			// lblRenderer2
			// 
			this.lblRenderer2.Location = new System.Drawing.Point(1051, 229);
			this.lblRenderer2.Name = "lblRenderer2";
			this.lblRenderer2.Size = new System.Drawing.Size(124, 23);
			this.lblRenderer2.TabIndex = 43;
			this.lblRenderer2.Text = "lblRenderer2";
			this.lblRenderer2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// nudOffset1X
			// 
			this.nudOffset1X.Location = new System.Drawing.Point(984, 311);
			this.nudOffset1X.Maximum = new decimal(new int[] {
            32000000,
            0,
            0,
            0});
			this.nudOffset1X.Minimum = new decimal(new int[] {
            32000000,
            0,
            0,
            -2147483648});
			this.nudOffset1X.Name = "nudOffset1X";
			this.nudOffset1X.Size = new System.Drawing.Size(75, 22);
			this.nudOffset1X.TabIndex = 46;
			// 
			// lblOffset1
			// 
			this.lblOffset1.Location = new System.Drawing.Point(1051, 285);
			this.lblOffset1.Name = "lblOffset1";
			this.lblOffset1.Size = new System.Drawing.Size(124, 23);
			this.lblOffset1.TabIndex = 47;
			this.lblOffset1.Text = "Model Offset 1:";
			this.lblOffset1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// nudOffset1Y
			// 
			this.nudOffset1Y.Location = new System.Drawing.Point(1077, 311);
			this.nudOffset1Y.Maximum = new decimal(new int[] {
            32000000,
            0,
            0,
            0});
			this.nudOffset1Y.Minimum = new decimal(new int[] {
            32000000,
            0,
            0,
            -2147483648});
			this.nudOffset1Y.Name = "nudOffset1Y";
			this.nudOffset1Y.Size = new System.Drawing.Size(75, 22);
			this.nudOffset1Y.TabIndex = 48;
			// 
			// nudOffset1Z
			// 
			this.nudOffset1Z.Location = new System.Drawing.Point(1170, 311);
			this.nudOffset1Z.Maximum = new decimal(new int[] {
            32000000,
            0,
            0,
            0});
			this.nudOffset1Z.Minimum = new decimal(new int[] {
            32000000,
            0,
            0,
            -2147483648});
			this.nudOffset1Z.Name = "nudOffset1Z";
			this.nudOffset1Z.Size = new System.Drawing.Size(75, 22);
			this.nudOffset1Z.TabIndex = 49;
			// 
			// nudOffset2Z
			// 
			this.nudOffset2Z.Location = new System.Drawing.Point(1170, 368);
			this.nudOffset2Z.Maximum = new decimal(new int[] {
            32000000,
            0,
            0,
            0});
			this.nudOffset2Z.Minimum = new decimal(new int[] {
            32000000,
            0,
            0,
            -2147483648});
			this.nudOffset2Z.Name = "nudOffset2Z";
			this.nudOffset2Z.Size = new System.Drawing.Size(75, 22);
			this.nudOffset2Z.TabIndex = 53;
			// 
			// nudOffset2Y
			// 
			this.nudOffset2Y.Location = new System.Drawing.Point(1077, 368);
			this.nudOffset2Y.Maximum = new decimal(new int[] {
            32000000,
            0,
            0,
            0});
			this.nudOffset2Y.Minimum = new decimal(new int[] {
            32000000,
            0,
            0,
            -2147483648});
			this.nudOffset2Y.Name = "nudOffset2Y";
			this.nudOffset2Y.Size = new System.Drawing.Size(75, 22);
			this.nudOffset2Y.TabIndex = 52;
			// 
			// lblOffset2
			// 
			this.lblOffset2.Location = new System.Drawing.Point(1051, 342);
			this.lblOffset2.Name = "lblOffset2";
			this.lblOffset2.Size = new System.Drawing.Size(124, 23);
			this.lblOffset2.TabIndex = 51;
			this.lblOffset2.Text = "Model Offset 2:";
			this.lblOffset2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// nudOffset2X
			// 
			this.nudOffset2X.Location = new System.Drawing.Point(984, 368);
			this.nudOffset2X.Maximum = new decimal(new int[] {
            32000000,
            0,
            0,
            0});
			this.nudOffset2X.Minimum = new decimal(new int[] {
            32000000,
            0,
            0,
            -2147483648});
			this.nudOffset2X.Name = "nudOffset2X";
			this.nudOffset2X.Size = new System.Drawing.Size(75, 22);
			this.nudOffset2X.TabIndex = 50;
			// 
			// lblScale
			// 
			this.lblScale.Location = new System.Drawing.Point(1051, 117);
			this.lblScale.Name = "lblScale";
			this.lblScale.Size = new System.Drawing.Size(124, 23);
			this.lblScale.TabIndex = 54;
			this.lblScale.Text = "Scale:";
			this.lblScale.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(1051, 61);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(124, 23);
			this.label7.TabIndex = 55;
			this.label7.Text = "Renderer:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// nudScale
			// 
			this.nudScale.Location = new System.Drawing.Point(1077, 143);
			this.nudScale.Maximum = new decimal(new int[] {
            32000000,
            0,
            0,
            0});
			this.nudScale.Minimum = new decimal(new int[] {
            32000000,
            0,
            0,
            -2147483648});
			this.nudScale.Name = "nudScale";
			this.nudScale.Size = new System.Drawing.Size(75, 22);
			this.nudScale.TabIndex = 56;
			// 
			// ObjectDatabaseEdtiorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1262, 526);
			this.Controls.Add(this.nudScale);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.lblScale);
			this.Controls.Add(this.nudOffset2Z);
			this.Controls.Add(this.nudOffset2Y);
			this.Controls.Add(this.lblOffset2);
			this.Controls.Add(this.nudOffset2X);
			this.Controls.Add(this.nudOffset1Z);
			this.Controls.Add(this.nudOffset1Y);
			this.Controls.Add(this.lblOffset1);
			this.Controls.Add(this.nudOffset1X);
			this.Controls.Add(this.btnRenderer2);
			this.Controls.Add(this.txtRenderer2);
			this.Controls.Add(this.lblRenderer2);
			this.Controls.Add(this.btnRenderer1);
			this.Controls.Add(this.txtRenderer1);
			this.Controls.Add(this.lblRenderer1);
			this.Controls.Add(this.lblFill);
			this.Controls.Add(this.lblBorder);
			this.Controls.Add(this.btnFill);
			this.Controls.Add(this.btnBorder);
			this.Controls.Add(this.cmbRenderer);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.txtSearch);
			this.Controls.Add(this.txtDescription);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtDlReq);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtBankReq);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.nudActorID);
			this.Controls.Add(this.lblXMLData);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblInternalName);
			this.Controls.Add(this.lblObjectName);
			this.Controls.Add(this.nudObjectID);
			this.Controls.Add(this.txtInternalName);
			this.Controls.Add(this.txtObjectName);
			this.Controls.Add(this.lstObjects);
			this.Controls.Add(this.toolStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "ObjectDatabaseEdtiorForm";
			this.Text = "Object DB Editor";
			this.Load += new System.EventHandler(this.ObjectDatabaseEdtiorForm_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudObjectID)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudActorID)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset1X)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset1Y)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset1Z)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset2Z)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset2Y)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudOffset2X)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudScale)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnImportXML;
        private System.Windows.Forms.ToolStripButton btnClose;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ListBox lstObjects;
        private System.Windows.Forms.TextBox txtObjectName;
        private System.Windows.Forms.TextBox txtInternalName;
        private System.Windows.Forms.NumericUpDown nudObjectID;
        private System.Windows.Forms.Label lblObjectName;
        private System.Windows.Forms.Label lblInternalName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblXMLData;
        private System.Windows.Forms.NumericUpDown nudActorID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBankReq;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDlReq;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox txtDescription;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSort;
        private System.Windows.Forms.ToolStripButton btnDisplay;
        private System.Windows.Forms.ToolStripButton btnApplyXML;
		private System.Windows.Forms.ComboBox cmbRenderer;
		private System.Windows.Forms.Button btnBorder;
		private System.Windows.Forms.Button btnFill;
		private System.Windows.Forms.Label lblBorder;
		private System.Windows.Forms.Label lblFill;
		private System.Windows.Forms.Label lblRenderer1;
		private System.Windows.Forms.TextBox txtRenderer1;
		private System.Windows.Forms.Button btnRenderer1;
		private System.Windows.Forms.Button btnRenderer2;
		private System.Windows.Forms.TextBox txtRenderer2;
		private System.Windows.Forms.Label lblRenderer2;
		private System.Windows.Forms.NumericUpDown nudOffset1X;
		private System.Windows.Forms.Label lblOffset1;
		private System.Windows.Forms.NumericUpDown nudOffset1Y;
		private System.Windows.Forms.NumericUpDown nudOffset1Z;
		private System.Windows.Forms.NumericUpDown nudOffset2Z;
		private System.Windows.Forms.NumericUpDown nudOffset2Y;
		private System.Windows.Forms.Label lblOffset2;
		private System.Windows.Forms.NumericUpDown nudOffset2X;
		private System.Windows.Forms.Label lblScale;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown nudScale;
	}
}