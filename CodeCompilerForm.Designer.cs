namespace SM64DSe
{
    partial class CodeCompilerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeCompilerForm));
            this.lblFolder = new System.Windows.Forms.Label();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.s = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.lblOffset = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.btnClean = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnInternal = new System.Windows.Forms.RadioButton();
            this.btnExternal = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtOverlayId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOverlay = new System.Windows.Forms.RadioButton();
            this.btnInjection = new System.Windows.Forms.RadioButton();
            this.btnGeneric = new System.Windows.Forms.RadioButton();
            this.btnDynamicLibrary = new System.Windows.Forms.RadioButton();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.btnSelectExternal = new System.Windows.Forms.Button();
            this.btnSelectInternal = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(8, 16);
            this.lblFolder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(48, 17);
            this.lblFolder.TabIndex = 0;
            this.lblFolder.Text = "Folder";
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(64, 12);
            this.txtFolder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(419, 22);
            this.txtFolder.TabIndex = 2;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(8, 48);
            this.lblOutput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(85, 17);
            this.lblOutput.TabIndex = 3;
            this.lblOutput.Text = "External File";
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(97, 44);
            this.txtOutput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(385, 22);
            this.txtOutput.TabIndex = 4;
            // 
            // s
            // 
            this.s.Location = new System.Drawing.Point(468, 192);
            this.s.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.s.Name = "s";
            this.s.Size = new System.Drawing.Size(85, 28);
            this.s.TabIndex = 5;
            this.s.Text = "Compile!";
            this.s.UseVisualStyleBackColor = true;
            this.s.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // lblOffset
            // 
            this.lblOffset.AutoSize = true;
            this.lblOffset.Location = new System.Drawing.Point(168, 30);
            this.lblOffset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.Size = new System.Drawing.Size(46, 17);
            this.lblOffset.TabIndex = 6;
            this.lblOffset.Text = "Offset";
            // 
            // txtOffset
            // 
            this.txtOffset.Enabled = false;
            this.txtOffset.Location = new System.Drawing.Point(223, 26);
            this.txtOffset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(120, 22);
            this.txtOffset.TabIndex = 7;
            // 
            // btnClean
            // 
            this.btnClean.Location = new System.Drawing.Point(372, 192);
            this.btnClean.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClean.Name = "btnClean";
            this.btnClean.Size = new System.Drawing.Size(88, 28);
            this.btnClean.TabIndex = 9;
            this.btnClean.Text = "Clean Patch";
            this.btnClean.UseVisualStyleBackColor = true;
            this.btnClean.Click += new System.EventHandler(this.btnClean_Click);
            // 
            // txtInput
            // 
            this.txtInput.Enabled = false;
            this.txtInput.Location = new System.Drawing.Point(97, 76);
            this.txtInput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(385, 22);
            this.txtInput.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 80);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "Internal File";
            // 
            // btnInternal
            // 
            this.btnInternal.AutoSize = true;
            this.btnInternal.Location = new System.Drawing.Point(8, 23);
            this.btnInternal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnInternal.Name = "btnInternal";
            this.btnInternal.Size = new System.Drawing.Size(76, 21);
            this.btnInternal.TabIndex = 13;
            this.btnInternal.Text = "Internal";
            this.btnInternal.UseVisualStyleBackColor = true;
            this.btnInternal.CheckedChanged += new System.EventHandler(this.btnInternal_CheckedChanged);
            // 
            // btnExternal
            // 
            this.btnExternal.AutoSize = true;
            this.btnExternal.Checked = true;
            this.btnExternal.Location = new System.Drawing.Point(96, 23);
            this.btnExternal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExternal.Name = "btnExternal";
            this.btnExternal.Size = new System.Drawing.Size(80, 21);
            this.btnExternal.TabIndex = 14;
            this.btnExternal.TabStop = true;
            this.btnExternal.Text = "External";
            this.btnExternal.UseVisualStyleBackColor = true;
            this.btnExternal.CheckedChanged += new System.EventHandler(this.btnExternal_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnInternal);
            this.groupBox1.Controls.Add(this.btnExternal);
            this.groupBox1.Location = new System.Drawing.Point(372, 108);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(181, 63);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Type";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtOverlayId);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnOverlay);
            this.groupBox2.Controls.Add(this.btnInjection);
            this.groupBox2.Controls.Add(this.btnGeneric);
            this.groupBox2.Controls.Add(this.btnDynamicLibrary);
            this.groupBox2.Controls.Add(this.txtOffset);
            this.groupBox2.Controls.Add(this.lblOffset);
            this.groupBox2.Location = new System.Drawing.Point(12, 108);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(352, 112);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Patch Settings";
            // 
            // txtOverlayId
            // 
            this.txtOverlayId.Enabled = false;
            this.txtOverlayId.Location = new System.Drawing.Point(223, 53);
            this.txtOverlayId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtOverlayId.Name = "txtOverlayId";
            this.txtOverlayId.Size = new System.Drawing.Size(120, 22);
            this.txtOverlayId.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(191, 57);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 17);
            this.label2.TabIndex = 14;
            this.label2.Text = "Id";
            // 
            // btnOverlay
            // 
            this.btnOverlay.AutoSize = true;
            this.btnOverlay.Location = new System.Drawing.Point(8, 54);
            this.btnOverlay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOverlay.Name = "btnOverlay";
            this.btnOverlay.Size = new System.Drawing.Size(78, 21);
            this.btnOverlay.TabIndex = 13;
            this.btnOverlay.Text = "Overlay";
            this.btnOverlay.UseVisualStyleBackColor = true;
            this.btnOverlay.CheckedChanged += new System.EventHandler(this.btnOverlay_CheckedChanged);
            // 
            // btnInjection
            // 
            this.btnInjection.AutoSize = true;
            this.btnInjection.Location = new System.Drawing.Point(257, 82);
            this.btnInjection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnInjection.Name = "btnInjection";
            this.btnInjection.Size = new System.Drawing.Size(81, 21);
            this.btnInjection.TabIndex = 12;
            this.btnInjection.Text = "Injection";
            this.btnInjection.UseVisualStyleBackColor = true;
            this.btnInjection.Visible = false;
            this.btnInjection.CheckedChanged += new System.EventHandler(this.btnInjection_CheckedChanged);
            // 
            // btnGeneric
            // 
            this.btnGeneric.AutoSize = true;
            this.btnGeneric.Location = new System.Drawing.Point(8, 27);
            this.btnGeneric.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGeneric.Name = "btnGeneric";
            this.btnGeneric.Size = new System.Drawing.Size(79, 21);
            this.btnGeneric.TabIndex = 11;
            this.btnGeneric.Text = "Generic";
            this.btnGeneric.UseVisualStyleBackColor = true;
            this.btnGeneric.CheckedChanged += new System.EventHandler(this.btnGeneric_CheckedChanged);
            // 
            // btnDynamicLibrary
            // 
            this.btnDynamicLibrary.AutoSize = true;
            this.btnDynamicLibrary.Checked = true;
            this.btnDynamicLibrary.Location = new System.Drawing.Point(8, 82);
            this.btnDynamicLibrary.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDynamicLibrary.Name = "btnDynamicLibrary";
            this.btnDynamicLibrary.Size = new System.Drawing.Size(131, 21);
            this.btnDynamicLibrary.TabIndex = 0;
            this.btnDynamicLibrary.TabStop = true;
            this.btnDynamicLibrary.Text = "Dynamic Library";
            this.btnDynamicLibrary.UseVisualStyleBackColor = true;
            this.btnDynamicLibrary.CheckedChanged += new System.EventHandler(this.btnDynamicLibrary_CheckedChanged);
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(492, 10);
            this.btnSelectFolder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(61, 28);
            this.btnSelectFolder.TabIndex = 17;
            this.btnSelectFolder.Text = "...";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // btnSelectExternal
            // 
            this.btnSelectExternal.Location = new System.Drawing.Point(492, 41);
            this.btnSelectExternal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSelectExternal.Name = "btnSelectExternal";
            this.btnSelectExternal.Size = new System.Drawing.Size(61, 28);
            this.btnSelectExternal.TabIndex = 18;
            this.btnSelectExternal.Text = "...";
            this.btnSelectExternal.UseVisualStyleBackColor = true;
            this.btnSelectExternal.Click += new System.EventHandler(this.btnSelectExternal_Click);
            // 
            // btnSelectInternal
            // 
            this.btnSelectInternal.Enabled = false;
            this.btnSelectInternal.Location = new System.Drawing.Point(492, 73);
            this.btnSelectInternal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSelectInternal.Name = "btnSelectInternal";
            this.btnSelectInternal.Size = new System.Drawing.Size(61, 28);
            this.btnSelectInternal.TabIndex = 19;
            this.btnSelectInternal.Text = "...";
            this.btnSelectInternal.UseVisualStyleBackColor = true;
            this.btnSelectInternal.Click += new System.EventHandler(this.btnSelectInternal_Click);
            // 
            // CodeCompilerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 229);
            this.Controls.Add(this.btnSelectInternal);
            this.Controls.Add(this.btnSelectExternal);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClean);
            this.Controls.Add(this.s);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.lblFolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "CodeCompilerForm";
            this.Text = "Code Compiler";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button s;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Button btnClean;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton btnInternal;
        private System.Windows.Forms.RadioButton btnExternal;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton btnInjection;
        private System.Windows.Forms.RadioButton btnGeneric;
        private System.Windows.Forms.RadioButton btnDynamicLibrary;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Button btnSelectExternal;
        private System.Windows.Forms.Button btnSelectInternal;
        private System.Windows.Forms.RadioButton btnOverlay;
        private System.Windows.Forms.TextBox txtOverlayId;
        private System.Windows.Forms.Label label2;
    }
}