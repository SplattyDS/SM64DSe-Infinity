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
            this.label7 = new System.Windows.Forms.Label();
            this.txtRenderer = new System.Windows.Forms.TextBox();
            this.btnImportMultiple = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudObjectID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudActorID)).BeginInit();
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
            this.btnImportMultiple,
            this.toolStripSeparator2,
            this.btnSort,
            this.btnDisplay});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1073, 31);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(44, 28);
            this.btnSave.Text = "Save";
            this.btnSave.ToolTipText = "Save the level names and overlay IDs";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(49, 28);
            this.btnClose.Text = "Close";
            this.btnClose.ToolTipText = "Close the window";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // btnImportXML
            // 
            this.btnImportXML.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImportXML.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportXML.Name = "btnImportXML";
            this.btnImportXML.Size = new System.Drawing.Size(91, 28);
            this.btnImportXML.Text = "Import XML";
            this.btnImportXML.ToolTipText = "Import level names from an XML file";
            this.btnImportXML.Click += new System.EventHandler(this.btnImportXML_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // btnSort
            // 
            this.btnSort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSort.Name = "btnSort";
            this.btnSort.Size = new System.Drawing.Size(130, 28);
            this.btnSort.Text = "Sort by: Object ID";
            this.btnSort.ToolTipText = "Import level names from an XML file";
            this.btnSort.Click += new System.EventHandler(this.btnSort_Click);
            // 
            // btnDisplay
            // 
            this.btnDisplay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDisplay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDisplay.Name = "btnDisplay";
            this.btnDisplay.Size = new System.Drawing.Size(157, 28);
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
            this.lstObjects.Size = new System.Drawing.Size(568, 454);
            this.lstObjects.TabIndex = 11;
            this.lstObjects.SelectedIndexChanged += new System.EventHandler(this.lstObjects_SelectedIndexChanged);
            // 
            // txtObjectName
            // 
            this.txtObjectName.Location = new System.Drawing.Point(723, 114);
            this.txtObjectName.Name = "txtObjectName";
            this.txtObjectName.Size = new System.Drawing.Size(326, 22);
            this.txtObjectName.TabIndex = 12;
            this.txtObjectName.TextChanged += new System.EventHandler(this.txtObjectName_TextChanged);
            // 
            // txtInternalName
            // 
            this.txtInternalName.Location = new System.Drawing.Point(723, 142);
            this.txtInternalName.Name = "txtInternalName";
            this.txtInternalName.Size = new System.Drawing.Size(326, 22);
            this.txtInternalName.TabIndex = 13;
            this.txtInternalName.TextChanged += new System.EventHandler(this.txtInternalName_TextChanged);
            // 
            // nudObjectID
            // 
            this.nudObjectID.Location = new System.Drawing.Point(723, 59);
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
            this.lblObjectName.Location = new System.Drawing.Point(625, 117);
            this.lblObjectName.Name = "lblObjectName";
            this.lblObjectName.Size = new System.Drawing.Size(92, 17);
            this.lblObjectName.TabIndex = 15;
            this.lblObjectName.Text = "Object name:";
            // 
            // lblInternalName
            // 
            this.lblInternalName.AutoSize = true;
            this.lblInternalName.Location = new System.Drawing.Point(619, 145);
            this.lblInternalName.Name = "lblInternalName";
            this.lblInternalName.Size = new System.Drawing.Size(98, 17);
            this.lblInternalName.TabIndex = 16;
            this.lblInternalName.Text = "Internal name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(647, 61);
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
            this.nudActorID.Location = new System.Drawing.Point(723, 86);
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
            this.label2.Location = new System.Drawing.Point(655, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 17);
            this.label2.TabIndex = 20;
            this.label2.Text = "Actor ID:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(586, 173);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 17);
            this.label4.TabIndex = 24;
            this.label4.Text = "Bank requirements:";
            // 
            // txtBankReq
            // 
            this.txtBankReq.Location = new System.Drawing.Point(723, 170);
            this.txtBankReq.Name = "txtBankReq";
            this.txtBankReq.Size = new System.Drawing.Size(326, 22);
            this.txtBankReq.TabIndex = 23;
            this.txtBankReq.TextChanged += new System.EventHandler(this.txtBankReq_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(600, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 17);
            this.label5.TabIndex = 26;
            this.label5.Text = "DL requirements:";
            // 
            // txtDlReq
            // 
            this.txtDlReq.Location = new System.Drawing.Point(723, 198);
            this.txtDlReq.Name = "txtDlReq";
            this.txtDlReq.Size = new System.Drawing.Size(326, 22);
            this.txtDlReq.TabIndex = 25;
            this.txtDlReq.TextChanged += new System.EventHandler(this.txtDlReq_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(787, 252);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 17);
            this.label3.TabIndex = 27;
            this.label3.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(586, 273);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(463, 241);
            this.txtDescription.TabIndex = 28;
            this.txtDescription.Text = "";
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(117, 32);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(301, 22);
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
            this.btnAdd.Location = new System.Drawing.Point(424, 32);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 31;
            this.btnAdd.Text = "Add new";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(505, 32);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 32;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(645, 229);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 17);
            this.label7.TabIndex = 34;
            this.label7.Text = "Renderer:";
            // 
            // txtRenderer
            // 
            this.txtRenderer.Location = new System.Drawing.Point(723, 226);
            this.txtRenderer.Name = "txtRenderer";
            this.txtRenderer.Size = new System.Drawing.Size(326, 22);
            this.txtRenderer.TabIndex = 33;
            this.txtRenderer.TextChanged += new System.EventHandler(this.txtRenderer_TextChanged);
            // 
            // btnImportMultiple
            // 
            this.btnImportMultiple.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImportMultiple.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportMultiple.Name = "btnImportMultiple";
            this.btnImportMultiple.Size = new System.Drawing.Size(171, 28);
            this.btnImportMultiple.Text = "Import Multiple Objects";
            this.btnImportMultiple.ToolTipText = "Import level names from an XML file";
            this.btnImportMultiple.Click += new System.EventHandler(this.btnImportMultiple_Click);
            // 
            // ObjectDatabaseEdtiorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 526);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtRenderer);
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
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtRenderer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSort;
        private System.Windows.Forms.ToolStripButton btnDisplay;
        private System.Windows.Forms.ToolStripButton btnImportMultiple;
    }
}