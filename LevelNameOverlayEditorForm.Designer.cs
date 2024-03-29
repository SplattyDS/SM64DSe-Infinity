﻿namespace SM64DSe
{
    partial class LevelNameOverlayEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelNameOverlayEditorForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnImportXML = new System.Windows.Forms.ToolStripButton();
            this.lstLevels = new System.Windows.Forms.ListBox();
            this.txtLevelName = new System.Windows.Forms.TextBox();
            this.txtShortName = new System.Windows.Forms.TextBox();
            this.nudOverlayID = new System.Windows.Forms.NumericUpDown();
            this.lblLevelName = new System.Windows.Forms.Label();
            this.lblShortName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblXMLData = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOverlayID)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.btnClose,
            this.toolStripSeparator2,
            this.btnImportXML});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1148, 27);
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
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
            // lstLevels
            // 
            this.lstLevels.Font = new System.Drawing.Font("Consolas", 8F);
            this.lstLevels.FormattingEnabled = true;
            this.lstLevels.ItemHeight = 15;
            this.lstLevels.Location = new System.Drawing.Point(12, 30);
            this.lstLevels.Name = "lstLevels";
            this.lstLevels.Size = new System.Drawing.Size(701, 454);
            this.lstLevels.TabIndex = 11;
            this.lstLevels.SelectedIndexChanged += new System.EventHandler(this.lstLevels_SelectedIndexChanged);
            // 
            // txtLevelName
            // 
            this.txtLevelName.Location = new System.Drawing.Point(810, 50);
            this.txtLevelName.Name = "txtLevelName";
            this.txtLevelName.Size = new System.Drawing.Size(326, 22);
            this.txtLevelName.TabIndex = 12;
            this.txtLevelName.TextChanged += new System.EventHandler(this.txtLevelName_TextChanged);
            // 
            // txtShortName
            // 
            this.txtShortName.Location = new System.Drawing.Point(810, 78);
            this.txtShortName.Name = "txtShortName";
            this.txtShortName.Size = new System.Drawing.Size(326, 22);
            this.txtShortName.TabIndex = 13;
            this.txtShortName.TextChanged += new System.EventHandler(this.txtShortName_TextChanged);
            // 
            // nudOverlayID
            // 
            this.nudOverlayID.Location = new System.Drawing.Point(810, 123);
            this.nudOverlayID.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.nudOverlayID.Name = "nudOverlayID";
            this.nudOverlayID.Size = new System.Drawing.Size(326, 22);
            this.nudOverlayID.TabIndex = 14;
            this.nudOverlayID.ValueChanged += new System.EventHandler(this.nudOverlayID_ValueChanged);
            // 
            // lblLevelName
            // 
            this.lblLevelName.AutoSize = true;
            this.lblLevelName.Location = new System.Drawing.Point(719, 53);
            this.lblLevelName.Name = "lblLevelName";
            this.lblLevelName.Size = new System.Drawing.Size(85, 17);
            this.lblLevelName.TabIndex = 15;
            this.lblLevelName.Text = "Level name:";
            // 
            // lblShortName
            // 
            this.lblShortName.AutoSize = true;
            this.lblShortName.Location = new System.Drawing.Point(719, 81);
            this.lblShortName.Name = "lblShortName";
            this.lblShortName.Size = new System.Drawing.Size(85, 17);
            this.lblShortName.TabIndex = 16;
            this.lblShortName.Text = "Short name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(726, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "Overlay ID:";
            // 
            // lblXMLData
            // 
            this.lblXMLData.AutoSize = true;
            this.lblXMLData.Location = new System.Drawing.Point(961, 30);
            this.lblXMLData.Name = "lblXMLData";
            this.lblXMLData.Size = new System.Drawing.Size(175, 17);
            this.lblXMLData.TabIndex = 18;
            this.lblXMLData.Text = "SM64DSe Level XML Data";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(948, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(188, 17);
            this.label2.TabIndex = 19;
            this.label2.Text = "SM64DS Level Overlay Data";
            // 
            // LevelNameOverlayEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1148, 507);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblXMLData);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblShortName);
            this.Controls.Add(this.lblLevelName);
            this.Controls.Add(this.nudOverlayID);
            this.Controls.Add(this.txtShortName);
            this.Controls.Add(this.txtLevelName);
            this.Controls.Add(this.lstLevels);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LevelNameOverlayEditorForm";
            this.Text = "Level XML Editor";
            this.Load += new System.EventHandler(this.LevelNameOverlayEditorForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOverlayID)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnImportXML;
        private System.Windows.Forms.ToolStripButton btnClose;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ListBox lstLevels;
        private System.Windows.Forms.TextBox txtLevelName;
        private System.Windows.Forms.TextBox txtShortName;
        private System.Windows.Forms.NumericUpDown nudOverlayID;
        private System.Windows.Forms.Label lblLevelName;
        private System.Windows.Forms.Label lblShortName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblXMLData;
        private System.Windows.Forms.Label label2;
    }
}