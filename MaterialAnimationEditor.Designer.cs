namespace SM64DSe
{
    partial class MaterialAnimationEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaterialAnimationEditor));
            this.ssMain = new System.Windows.Forms.StatusStrip();
            this.lblMainStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpgModel = new System.Windows.Forms.TabPage();
            this.splModel = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.chkHasAnim = new System.Windows.Forms.CheckBox();
            this.btnRemoveFrame = new System.Windows.Forms.Button();
            this.btnAddFrame = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtAlpha = new System.Windows.Forms.TextBox();
            this.chkAlpha = new System.Windows.Forms.CheckBox();
            this.btnEmi = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtEmiRed = new System.Windows.Forms.TextBox();
            this.chkEmiBlue = new System.Windows.Forms.CheckBox();
            this.txtEmiBlue = new System.Windows.Forms.TextBox();
            this.chkEmiRed = new System.Windows.Forms.CheckBox();
            this.txtEmiGreen = new System.Windows.Forms.TextBox();
            this.chkEmiGreen = new System.Windows.Forms.CheckBox();
            this.btnSpec = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSpecRed = new System.Windows.Forms.TextBox();
            this.chkSpecBlue = new System.Windows.Forms.CheckBox();
            this.txtSpecBlue = new System.Windows.Forms.TextBox();
            this.chkSpecRed = new System.Windows.Forms.CheckBox();
            this.txtSpecGreen = new System.Windows.Forms.TextBox();
            this.chkSpecGreen = new System.Windows.Forms.CheckBox();
            this.btnAmb = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAmbRed = new System.Windows.Forms.TextBox();
            this.chkAmbBlue = new System.Windows.Forms.CheckBox();
            this.txtAmbBlue = new System.Windows.Forms.TextBox();
            this.chkAmbRed = new System.Windows.Forms.CheckBox();
            this.txtAmbGreen = new System.Windows.Forms.TextBox();
            this.chkAmbGreen = new System.Windows.Forms.CheckBox();
            this.btnDif = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDifRed = new System.Windows.Forms.TextBox();
            this.chkDifBlue = new System.Windows.Forms.CheckBox();
            this.txtDifBlue = new System.Windows.Forms.TextBox();
            this.chkDifRed = new System.Windows.Forms.CheckBox();
            this.txtDifGreen = new System.Windows.Forms.TextBox();
            this.chkDifGreen = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstMaterialProperties = new System.Windows.Forms.ListBox();
            this.txtNumFrames = new System.Windows.Forms.TextBox();
            this.lblFrameNum = new System.Windows.Forms.Label();
            this.txtCurrentFrameNum = new System.Windows.Forms.TextBox();
            this.lblFrame = new System.Windows.Forms.Label();
            this.btnLastFrame = new System.Windows.Forms.Button();
            this.btnNextFrame = new System.Windows.Forms.Button();
            this.btnPreviousFrame = new System.Windows.Forms.Button();
            this.btnFirstFrame = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.txtModelPreviewScale = new System.Windows.Forms.TextBox();
            this.tsModelPreview = new System.Windows.Forms.ToolStrip();
            this.lblModelPreviewScale = new System.Windows.Forms.ToolStripLabel();
            this.mstrMain = new System.Windows.Forms.MenuStrip();
            this.mnitLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitImport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnitExport = new System.Windows.Forms.ToolStripMenuItem();
            this.glModelView = new SM64DSe.FormControls.ModelGLControlWithMarioSizeReference();
            this.ssMain.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpgModel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splModel)).BeginInit();
            this.splModel.Panel1.SuspendLayout();
            this.splModel.Panel2.SuspendLayout();
            this.splModel.SuspendLayout();
            this.tsModelPreview.SuspendLayout();
            this.mstrMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // ssMain
            // 
            this.ssMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblMainStatus});
            this.ssMain.Location = new System.Drawing.Point(0, 710);
            this.ssMain.Name = "ssMain";
            this.ssMain.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.ssMain.Size = new System.Drawing.Size(1184, 26);
            this.ssMain.TabIndex = 1;
            this.ssMain.Text = "statusStrip1";
            // 
            // lblMainStatus
            // 
            this.lblMainStatus.Name = "lblMainStatus";
            this.lblMainStatus.Size = new System.Drawing.Size(50, 20);
            this.lblMainStatus.Text = "Ready";
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpgModel);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 28);
            this.tcMain.Margin = new System.Windows.Forms.Padding(4);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(1184, 682);
            this.tcMain.TabIndex = 2;
            // 
            // tpgModel
            // 
            this.tpgModel.Controls.Add(this.splModel);
            this.tpgModel.Location = new System.Drawing.Point(4, 25);
            this.tpgModel.Margin = new System.Windows.Forms.Padding(4);
            this.tpgModel.Name = "tpgModel";
            this.tpgModel.Padding = new System.Windows.Forms.Padding(4);
            this.tpgModel.Size = new System.Drawing.Size(1176, 653);
            this.tpgModel.TabIndex = 0;
            this.tpgModel.Text = "Model (BMD)";
            this.tpgModel.UseVisualStyleBackColor = true;
            // 
            // splModel
            // 
            this.splModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splModel.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splModel.IsSplitterFixed = true;
            this.splModel.Location = new System.Drawing.Point(4, 4);
            this.splModel.Margin = new System.Windows.Forms.Padding(4);
            this.splModel.Name = "splModel";
            // 
            // splModel.Panel1
            // 
            this.splModel.Panel1.Controls.Add(this.label2);
            this.splModel.Panel1.Controls.Add(this.chkHasAnim);
            this.splModel.Panel1.Controls.Add(this.btnRemoveFrame);
            this.splModel.Panel1.Controls.Add(this.btnAddFrame);
            this.splModel.Panel1.Controls.Add(this.label7);
            this.splModel.Panel1.Controls.Add(this.txtAlpha);
            this.splModel.Panel1.Controls.Add(this.chkAlpha);
            this.splModel.Panel1.Controls.Add(this.btnEmi);
            this.splModel.Panel1.Controls.Add(this.label6);
            this.splModel.Panel1.Controls.Add(this.txtEmiRed);
            this.splModel.Panel1.Controls.Add(this.chkEmiBlue);
            this.splModel.Panel1.Controls.Add(this.txtEmiBlue);
            this.splModel.Panel1.Controls.Add(this.chkEmiRed);
            this.splModel.Panel1.Controls.Add(this.txtEmiGreen);
            this.splModel.Panel1.Controls.Add(this.chkEmiGreen);
            this.splModel.Panel1.Controls.Add(this.btnSpec);
            this.splModel.Panel1.Controls.Add(this.label5);
            this.splModel.Panel1.Controls.Add(this.txtSpecRed);
            this.splModel.Panel1.Controls.Add(this.chkSpecBlue);
            this.splModel.Panel1.Controls.Add(this.txtSpecBlue);
            this.splModel.Panel1.Controls.Add(this.chkSpecRed);
            this.splModel.Panel1.Controls.Add(this.txtSpecGreen);
            this.splModel.Panel1.Controls.Add(this.chkSpecGreen);
            this.splModel.Panel1.Controls.Add(this.btnAmb);
            this.splModel.Panel1.Controls.Add(this.label4);
            this.splModel.Panel1.Controls.Add(this.txtAmbRed);
            this.splModel.Panel1.Controls.Add(this.chkAmbBlue);
            this.splModel.Panel1.Controls.Add(this.txtAmbBlue);
            this.splModel.Panel1.Controls.Add(this.chkAmbRed);
            this.splModel.Panel1.Controls.Add(this.txtAmbGreen);
            this.splModel.Panel1.Controls.Add(this.chkAmbGreen);
            this.splModel.Panel1.Controls.Add(this.btnDif);
            this.splModel.Panel1.Controls.Add(this.label3);
            this.splModel.Panel1.Controls.Add(this.txtDifRed);
            this.splModel.Panel1.Controls.Add(this.chkDifBlue);
            this.splModel.Panel1.Controls.Add(this.txtDifBlue);
            this.splModel.Panel1.Controls.Add(this.chkDifRed);
            this.splModel.Panel1.Controls.Add(this.txtDifGreen);
            this.splModel.Panel1.Controls.Add(this.chkDifGreen);
            this.splModel.Panel1.Controls.Add(this.label1);
            this.splModel.Panel1.Controls.Add(this.lstMaterialProperties);
            this.splModel.Panel1.Controls.Add(this.txtNumFrames);
            this.splModel.Panel1.Controls.Add(this.lblFrameNum);
            this.splModel.Panel1.Controls.Add(this.txtCurrentFrameNum);
            this.splModel.Panel1.Controls.Add(this.lblFrame);
            this.splModel.Panel1.Controls.Add(this.btnLastFrame);
            this.splModel.Panel1.Controls.Add(this.btnNextFrame);
            this.splModel.Panel1.Controls.Add(this.btnPreviousFrame);
            this.splModel.Panel1.Controls.Add(this.btnFirstFrame);
            this.splModel.Panel1.Controls.Add(this.btnStop);
            this.splModel.Panel1.Controls.Add(this.btnPlay);
            // 
            // splModel.Panel2
            // 
            this.splModel.Panel2.Controls.Add(this.txtModelPreviewScale);
            this.splModel.Panel2.Controls.Add(this.tsModelPreview);
            this.splModel.Panel2.Controls.Add(this.glModelView);
            this.splModel.Size = new System.Drawing.Size(1168, 645);
            this.splModel.SplitterDistance = 308;
            this.splModel.SplitterWidth = 5;
            this.splModel.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 263);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 17);
            this.label2.TabIndex = 79;
            this.label2.Text = "Material has animation?";
            // 
            // chkHasAnim
            // 
            this.chkHasAnim.AutoSize = true;
            this.chkHasAnim.Location = new System.Drawing.Point(229, 264);
            this.chkHasAnim.Margin = new System.Windows.Forms.Padding(4);
            this.chkHasAnim.Name = "chkHasAnim";
            this.chkHasAnim.Size = new System.Drawing.Size(18, 17);
            this.chkHasAnim.TabIndex = 78;
            this.chkHasAnim.UseVisualStyleBackColor = true;
            this.chkHasAnim.CheckedChanged += new System.EventHandler(this.chkHasAnim_CheckedChanged);
            // 
            // btnRemoveFrame
            // 
            this.btnRemoveFrame.Location = new System.Drawing.Point(271, 74);
            this.btnRemoveFrame.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemoveFrame.Name = "btnRemoveFrame";
            this.btnRemoveFrame.Size = new System.Drawing.Size(28, 28);
            this.btnRemoveFrame.TabIndex = 77;
            this.btnRemoveFrame.Text = "-";
            this.btnRemoveFrame.UseVisualStyleBackColor = true;
            this.btnRemoveFrame.Click += new System.EventHandler(this.btnRemoveFrame_Click);
            // 
            // btnAddFrame
            // 
            this.btnAddFrame.Location = new System.Drawing.Point(235, 74);
            this.btnAddFrame.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddFrame.Name = "btnAddFrame";
            this.btnAddFrame.Size = new System.Drawing.Size(28, 28);
            this.btnAddFrame.TabIndex = 76;
            this.btnAddFrame.Text = "+";
            this.btnAddFrame.UseVisualStyleBackColor = true;
            this.btnAddFrame.Click += new System.EventHandler(this.btnAddFrame_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(203, 566);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 17);
            this.label7.TabIndex = 75;
            this.label7.Text = "Alpha";
            // 
            // txtAlpha
            // 
            this.txtAlpha.Location = new System.Drawing.Point(57, 591);
            this.txtAlpha.Margin = new System.Windows.Forms.Padding(4);
            this.txtAlpha.Name = "txtAlpha";
            this.txtAlpha.Size = new System.Drawing.Size(44, 22);
            this.txtAlpha.TabIndex = 74;
            this.txtAlpha.TextChanged += new System.EventHandler(this.txtAlpha_TextChanged);
            // 
            // chkAlpha
            // 
            this.chkAlpha.AutoSize = true;
            this.chkAlpha.Location = new System.Drawing.Point(71, 566);
            this.chkAlpha.Margin = new System.Windows.Forms.Padding(4);
            this.chkAlpha.Name = "chkAlpha";
            this.chkAlpha.Size = new System.Drawing.Size(18, 17);
            this.chkAlpha.TabIndex = 71;
            this.chkAlpha.UseVisualStyleBackColor = true;
            this.chkAlpha.CheckedChanged += new System.EventHandler(this.chkAlpha_CheckedChanged);
            // 
            // btnEmi
            // 
            this.btnEmi.BackColor = System.Drawing.Color.Black;
            this.btnEmi.ForeColor = System.Drawing.Color.White;
            this.btnEmi.Location = new System.Drawing.Point(179, 533);
            this.btnEmi.Margin = new System.Windows.Forms.Padding(4);
            this.btnEmi.Name = "btnEmi";
            this.btnEmi.Size = new System.Drawing.Size(93, 28);
            this.btnEmi.TabIndex = 68;
            this.btnEmi.Text = "#000000";
            this.btnEmi.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(194, 511);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 17);
            this.label6.TabIndex = 67;
            this.label6.Text = "Emission";
            // 
            // txtEmiRed
            // 
            this.txtEmiRed.Location = new System.Drawing.Point(5, 536);
            this.txtEmiRed.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmiRed.Name = "txtEmiRed";
            this.txtEmiRed.Size = new System.Drawing.Size(44, 22);
            this.txtEmiRed.TabIndex = 66;
            this.txtEmiRed.TextChanged += new System.EventHandler(this.txtEmiRed_TextChanged);
            // 
            // chkEmiBlue
            // 
            this.chkEmiBlue.AutoSize = true;
            this.chkEmiBlue.Location = new System.Drawing.Point(122, 511);
            this.chkEmiBlue.Margin = new System.Windows.Forms.Padding(4);
            this.chkEmiBlue.Name = "chkEmiBlue";
            this.chkEmiBlue.Size = new System.Drawing.Size(18, 17);
            this.chkEmiBlue.TabIndex = 65;
            this.chkEmiBlue.UseVisualStyleBackColor = true;
            this.chkEmiBlue.CheckedChanged += new System.EventHandler(this.chkEmiBlue_CheckedChanged);
            // 
            // txtEmiBlue
            // 
            this.txtEmiBlue.Location = new System.Drawing.Point(109, 536);
            this.txtEmiBlue.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmiBlue.Name = "txtEmiBlue";
            this.txtEmiBlue.Size = new System.Drawing.Size(44, 22);
            this.txtEmiBlue.TabIndex = 64;
            this.txtEmiBlue.TextChanged += new System.EventHandler(this.txtEmiBlue_TextChanged);
            // 
            // chkEmiRed
            // 
            this.chkEmiRed.AutoSize = true;
            this.chkEmiRed.Location = new System.Drawing.Point(19, 511);
            this.chkEmiRed.Margin = new System.Windows.Forms.Padding(4);
            this.chkEmiRed.Name = "chkEmiRed";
            this.chkEmiRed.Size = new System.Drawing.Size(18, 17);
            this.chkEmiRed.TabIndex = 63;
            this.chkEmiRed.UseVisualStyleBackColor = true;
            this.chkEmiRed.CheckedChanged += new System.EventHandler(this.chkEmiRed_CheckedChanged);
            // 
            // txtEmiGreen
            // 
            this.txtEmiGreen.Location = new System.Drawing.Point(57, 536);
            this.txtEmiGreen.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmiGreen.Name = "txtEmiGreen";
            this.txtEmiGreen.Size = new System.Drawing.Size(44, 22);
            this.txtEmiGreen.TabIndex = 62;
            this.txtEmiGreen.TextChanged += new System.EventHandler(this.txtEmiGreen_TextChanged);
            // 
            // chkEmiGreen
            // 
            this.chkEmiGreen.AutoSize = true;
            this.chkEmiGreen.Location = new System.Drawing.Point(71, 511);
            this.chkEmiGreen.Margin = new System.Windows.Forms.Padding(4);
            this.chkEmiGreen.Name = "chkEmiGreen";
            this.chkEmiGreen.Size = new System.Drawing.Size(18, 17);
            this.chkEmiGreen.TabIndex = 61;
            this.chkEmiGreen.UseVisualStyleBackColor = true;
            this.chkEmiGreen.CheckedChanged += new System.EventHandler(this.chkEmiGreen_CheckedChanged);
            // 
            // btnSpec
            // 
            this.btnSpec.BackColor = System.Drawing.Color.Black;
            this.btnSpec.ForeColor = System.Drawing.Color.White;
            this.btnSpec.Location = new System.Drawing.Point(179, 478);
            this.btnSpec.Margin = new System.Windows.Forms.Padding(4);
            this.btnSpec.Name = "btnSpec";
            this.btnSpec.Size = new System.Drawing.Size(93, 28);
            this.btnSpec.TabIndex = 60;
            this.btnSpec.Text = "#000000";
            this.btnSpec.UseVisualStyleBackColor = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(193, 456);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 17);
            this.label5.TabIndex = 59;
            this.label5.Text = "Specular";
            // 
            // txtSpecRed
            // 
            this.txtSpecRed.Location = new System.Drawing.Point(5, 481);
            this.txtSpecRed.Margin = new System.Windows.Forms.Padding(4);
            this.txtSpecRed.Name = "txtSpecRed";
            this.txtSpecRed.Size = new System.Drawing.Size(44, 22);
            this.txtSpecRed.TabIndex = 58;
            this.txtSpecRed.TextChanged += new System.EventHandler(this.txtSpecRed_TextChanged);
            // 
            // chkSpecBlue
            // 
            this.chkSpecBlue.AutoSize = true;
            this.chkSpecBlue.Location = new System.Drawing.Point(122, 456);
            this.chkSpecBlue.Margin = new System.Windows.Forms.Padding(4);
            this.chkSpecBlue.Name = "chkSpecBlue";
            this.chkSpecBlue.Size = new System.Drawing.Size(18, 17);
            this.chkSpecBlue.TabIndex = 57;
            this.chkSpecBlue.UseVisualStyleBackColor = true;
            this.chkSpecBlue.CheckedChanged += new System.EventHandler(this.chkSpecBlue_CheckedChanged);
            // 
            // txtSpecBlue
            // 
            this.txtSpecBlue.Location = new System.Drawing.Point(109, 481);
            this.txtSpecBlue.Margin = new System.Windows.Forms.Padding(4);
            this.txtSpecBlue.Name = "txtSpecBlue";
            this.txtSpecBlue.Size = new System.Drawing.Size(44, 22);
            this.txtSpecBlue.TabIndex = 56;
            this.txtSpecBlue.TextChanged += new System.EventHandler(this.txtSpecBlue_TextChanged);
            // 
            // chkSpecRed
            // 
            this.chkSpecRed.AutoSize = true;
            this.chkSpecRed.Location = new System.Drawing.Point(19, 456);
            this.chkSpecRed.Margin = new System.Windows.Forms.Padding(4);
            this.chkSpecRed.Name = "chkSpecRed";
            this.chkSpecRed.Size = new System.Drawing.Size(18, 17);
            this.chkSpecRed.TabIndex = 55;
            this.chkSpecRed.UseVisualStyleBackColor = true;
            this.chkSpecRed.CheckedChanged += new System.EventHandler(this.chkSpecRed_CheckedChanged);
            // 
            // txtSpecGreen
            // 
            this.txtSpecGreen.Location = new System.Drawing.Point(57, 481);
            this.txtSpecGreen.Margin = new System.Windows.Forms.Padding(4);
            this.txtSpecGreen.Name = "txtSpecGreen";
            this.txtSpecGreen.Size = new System.Drawing.Size(44, 22);
            this.txtSpecGreen.TabIndex = 54;
            this.txtSpecGreen.TextChanged += new System.EventHandler(this.txtSpecGreen_TextChanged);
            // 
            // chkSpecGreen
            // 
            this.chkSpecGreen.AutoSize = true;
            this.chkSpecGreen.Location = new System.Drawing.Point(71, 456);
            this.chkSpecGreen.Margin = new System.Windows.Forms.Padding(4);
            this.chkSpecGreen.Name = "chkSpecGreen";
            this.chkSpecGreen.Size = new System.Drawing.Size(18, 17);
            this.chkSpecGreen.TabIndex = 53;
            this.chkSpecGreen.UseVisualStyleBackColor = true;
            this.chkSpecGreen.CheckedChanged += new System.EventHandler(this.chkSpecGreen_CheckedChanged);
            // 
            // btnAmb
            // 
            this.btnAmb.BackColor = System.Drawing.Color.Black;
            this.btnAmb.ForeColor = System.Drawing.Color.White;
            this.btnAmb.Location = new System.Drawing.Point(179, 423);
            this.btnAmb.Margin = new System.Windows.Forms.Padding(4);
            this.btnAmb.Name = "btnAmb";
            this.btnAmb.Size = new System.Drawing.Size(93, 28);
            this.btnAmb.TabIndex = 52;
            this.btnAmb.Text = "#000000";
            this.btnAmb.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(196, 401);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 17);
            this.label4.TabIndex = 51;
            this.label4.Text = "Ambient";
            // 
            // txtAmbRed
            // 
            this.txtAmbRed.Location = new System.Drawing.Point(5, 426);
            this.txtAmbRed.Margin = new System.Windows.Forms.Padding(4);
            this.txtAmbRed.Name = "txtAmbRed";
            this.txtAmbRed.Size = new System.Drawing.Size(44, 22);
            this.txtAmbRed.TabIndex = 50;
            this.txtAmbRed.TextChanged += new System.EventHandler(this.txtAmbRed_TextChanged);
            // 
            // chkAmbBlue
            // 
            this.chkAmbBlue.AutoSize = true;
            this.chkAmbBlue.Location = new System.Drawing.Point(122, 401);
            this.chkAmbBlue.Margin = new System.Windows.Forms.Padding(4);
            this.chkAmbBlue.Name = "chkAmbBlue";
            this.chkAmbBlue.Size = new System.Drawing.Size(18, 17);
            this.chkAmbBlue.TabIndex = 49;
            this.chkAmbBlue.UseVisualStyleBackColor = true;
            this.chkAmbBlue.CheckedChanged += new System.EventHandler(this.chkAmbBlue_CheckedChanged);
            // 
            // txtAmbBlue
            // 
            this.txtAmbBlue.Location = new System.Drawing.Point(109, 426);
            this.txtAmbBlue.Margin = new System.Windows.Forms.Padding(4);
            this.txtAmbBlue.Name = "txtAmbBlue";
            this.txtAmbBlue.Size = new System.Drawing.Size(44, 22);
            this.txtAmbBlue.TabIndex = 48;
            this.txtAmbBlue.TextChanged += new System.EventHandler(this.txtAmbBlue_TextChanged);
            // 
            // chkAmbRed
            // 
            this.chkAmbRed.AutoSize = true;
            this.chkAmbRed.Location = new System.Drawing.Point(19, 401);
            this.chkAmbRed.Margin = new System.Windows.Forms.Padding(4);
            this.chkAmbRed.Name = "chkAmbRed";
            this.chkAmbRed.Size = new System.Drawing.Size(18, 17);
            this.chkAmbRed.TabIndex = 47;
            this.chkAmbRed.UseVisualStyleBackColor = true;
            this.chkAmbRed.CheckedChanged += new System.EventHandler(this.chkAmbRed_CheckedChanged);
            // 
            // txtAmbGreen
            // 
            this.txtAmbGreen.Location = new System.Drawing.Point(57, 426);
            this.txtAmbGreen.Margin = new System.Windows.Forms.Padding(4);
            this.txtAmbGreen.Name = "txtAmbGreen";
            this.txtAmbGreen.Size = new System.Drawing.Size(44, 22);
            this.txtAmbGreen.TabIndex = 46;
            this.txtAmbGreen.TextChanged += new System.EventHandler(this.txtAmbGreen_TextChanged);
            // 
            // chkAmbGreen
            // 
            this.chkAmbGreen.AutoSize = true;
            this.chkAmbGreen.Location = new System.Drawing.Point(71, 401);
            this.chkAmbGreen.Margin = new System.Windows.Forms.Padding(4);
            this.chkAmbGreen.Name = "chkAmbGreen";
            this.chkAmbGreen.Size = new System.Drawing.Size(18, 17);
            this.chkAmbGreen.TabIndex = 45;
            this.chkAmbGreen.UseVisualStyleBackColor = true;
            this.chkAmbGreen.CheckedChanged += new System.EventHandler(this.chkAmbGreen_CheckedChanged);
            // 
            // btnDif
            // 
            this.btnDif.BackColor = System.Drawing.Color.Black;
            this.btnDif.ForeColor = System.Drawing.Color.White;
            this.btnDif.Location = new System.Drawing.Point(179, 368);
            this.btnDif.Margin = new System.Windows.Forms.Padding(4);
            this.btnDif.Name = "btnDif";
            this.btnDif.Size = new System.Drawing.Size(93, 28);
            this.btnDif.TabIndex = 44;
            this.btnDif.Text = "#000000";
            this.btnDif.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(199, 346);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 17);
            this.label3.TabIndex = 43;
            this.label3.Text = "Diffuse";
            // 
            // txtDifRed
            // 
            this.txtDifRed.Location = new System.Drawing.Point(5, 371);
            this.txtDifRed.Margin = new System.Windows.Forms.Padding(4);
            this.txtDifRed.Name = "txtDifRed";
            this.txtDifRed.Size = new System.Drawing.Size(44, 22);
            this.txtDifRed.TabIndex = 42;
            this.txtDifRed.TextChanged += new System.EventHandler(this.txtDifRed_TextChanged);
            // 
            // chkDifBlue
            // 
            this.chkDifBlue.AutoSize = true;
            this.chkDifBlue.Location = new System.Drawing.Point(122, 346);
            this.chkDifBlue.Margin = new System.Windows.Forms.Padding(4);
            this.chkDifBlue.Name = "chkDifBlue";
            this.chkDifBlue.Size = new System.Drawing.Size(18, 17);
            this.chkDifBlue.TabIndex = 41;
            this.chkDifBlue.UseVisualStyleBackColor = true;
            this.chkDifBlue.CheckedChanged += new System.EventHandler(this.chkDifBlue_CheckedChanged);
            // 
            // txtDifBlue
            // 
            this.txtDifBlue.Location = new System.Drawing.Point(109, 371);
            this.txtDifBlue.Margin = new System.Windows.Forms.Padding(4);
            this.txtDifBlue.Name = "txtDifBlue";
            this.txtDifBlue.Size = new System.Drawing.Size(44, 22);
            this.txtDifBlue.TabIndex = 40;
            this.txtDifBlue.TextChanged += new System.EventHandler(this.txtDifBlue_TextChanged);
            // 
            // chkDifRed
            // 
            this.chkDifRed.AutoSize = true;
            this.chkDifRed.Location = new System.Drawing.Point(19, 346);
            this.chkDifRed.Margin = new System.Windows.Forms.Padding(4);
            this.chkDifRed.Name = "chkDifRed";
            this.chkDifRed.Size = new System.Drawing.Size(18, 17);
            this.chkDifRed.TabIndex = 39;
            this.chkDifRed.UseVisualStyleBackColor = true;
            this.chkDifRed.CheckedChanged += new System.EventHandler(this.chkDifRed_CheckedChanged);
            // 
            // txtDifGreen
            // 
            this.txtDifGreen.Location = new System.Drawing.Point(57, 371);
            this.txtDifGreen.Margin = new System.Windows.Forms.Padding(4);
            this.txtDifGreen.Name = "txtDifGreen";
            this.txtDifGreen.Size = new System.Drawing.Size(44, 22);
            this.txtDifGreen.TabIndex = 38;
            this.txtDifGreen.TextChanged += new System.EventHandler(this.txtDifGreen_TextChanged);
            // 
            // chkDifGreen
            // 
            this.chkDifGreen.AutoSize = true;
            this.chkDifGreen.Location = new System.Drawing.Point(71, 346);
            this.chkDifGreen.Margin = new System.Windows.Forms.Padding(4);
            this.chkDifGreen.Name = "chkDifGreen";
            this.chkDifGreen.Size = new System.Drawing.Size(18, 17);
            this.chkDifGreen.TabIndex = 37;
            this.chkDifGreen.UseVisualStyleBackColor = true;
            this.chkDifGreen.CheckedChanged += new System.EventHandler(this.chkDifGreen_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 311);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(267, 17);
            this.label1.TabIndex = 34;
            this.label1.Text = "Material values for frame {currentFrame}:";
            // 
            // lstMaterialProperties
            // 
            this.lstMaterialProperties.FormattingEnabled = true;
            this.lstMaterialProperties.ItemHeight = 16;
            this.lstMaterialProperties.Location = new System.Drawing.Point(5, 133);
            this.lstMaterialProperties.Name = "lstMaterialProperties";
            this.lstMaterialProperties.Size = new System.Drawing.Size(300, 116);
            this.lstMaterialProperties.TabIndex = 31;
            this.lstMaterialProperties.SelectedIndexChanged += new System.EventHandler(this.lstMaterialProperties_SelectedIndexChanged);
            // 
            // txtNumFrames
            // 
            this.txtNumFrames.Location = new System.Drawing.Point(156, 75);
            this.txtNumFrames.Margin = new System.Windows.Forms.Padding(4);
            this.txtNumFrames.Name = "txtNumFrames";
            this.txtNumFrames.ReadOnly = true;
            this.txtNumFrames.Size = new System.Drawing.Size(71, 22);
            this.txtNumFrames.TabIndex = 28;
            // 
            // lblFrameNum
            // 
            this.lblFrameNum.AutoSize = true;
            this.lblFrameNum.Location = new System.Drawing.Point(132, 79);
            this.lblFrameNum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFrameNum.Name = "lblFrameNum";
            this.lblFrameNum.Size = new System.Drawing.Size(12, 17);
            this.lblFrameNum.TabIndex = 27;
            this.lblFrameNum.Text = "/";
            // 
            // txtCurrentFrameNum
            // 
            this.txtCurrentFrameNum.Location = new System.Drawing.Point(52, 75);
            this.txtCurrentFrameNum.Margin = new System.Windows.Forms.Padding(4);
            this.txtCurrentFrameNum.Name = "txtCurrentFrameNum";
            this.txtCurrentFrameNum.Size = new System.Drawing.Size(71, 22);
            this.txtCurrentFrameNum.TabIndex = 26;
            // 
            // lblFrame
            // 
            this.lblFrame.AutoSize = true;
            this.lblFrame.Location = new System.Drawing.Point(0, 79);
            this.lblFrame.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFrame.Name = "lblFrame";
            this.lblFrame.Size = new System.Drawing.Size(52, 17);
            this.lblFrame.TabIndex = 25;
            this.lblFrame.Text = "Frame:";
            // 
            // btnLastFrame
            // 
            this.btnLastFrame.Location = new System.Drawing.Point(165, 39);
            this.btnLastFrame.Margin = new System.Windows.Forms.Padding(4);
            this.btnLastFrame.Name = "btnLastFrame";
            this.btnLastFrame.Size = new System.Drawing.Size(48, 28);
            this.btnLastFrame.TabIndex = 24;
            this.btnLastFrame.Text = "| >";
            this.btnLastFrame.UseVisualStyleBackColor = true;
            this.btnLastFrame.Click += new System.EventHandler(this.btnLastFrame_Click);
            // 
            // btnNextFrame
            // 
            this.btnNextFrame.Location = new System.Drawing.Point(113, 39);
            this.btnNextFrame.Margin = new System.Windows.Forms.Padding(4);
            this.btnNextFrame.Name = "btnNextFrame";
            this.btnNextFrame.Size = new System.Drawing.Size(48, 28);
            this.btnNextFrame.TabIndex = 23;
            this.btnNextFrame.Text = ">";
            this.btnNextFrame.UseVisualStyleBackColor = true;
            this.btnNextFrame.Click += new System.EventHandler(this.btnNextFrame_Click);
            // 
            // btnPreviousFrame
            // 
            this.btnPreviousFrame.Location = new System.Drawing.Point(57, 39);
            this.btnPreviousFrame.Margin = new System.Windows.Forms.Padding(4);
            this.btnPreviousFrame.Name = "btnPreviousFrame";
            this.btnPreviousFrame.Size = new System.Drawing.Size(48, 28);
            this.btnPreviousFrame.TabIndex = 22;
            this.btnPreviousFrame.Text = "<";
            this.btnPreviousFrame.UseVisualStyleBackColor = true;
            this.btnPreviousFrame.Click += new System.EventHandler(this.btnPreviousFrame_Click);
            // 
            // btnFirstFrame
            // 
            this.btnFirstFrame.Location = new System.Drawing.Point(5, 39);
            this.btnFirstFrame.Margin = new System.Windows.Forms.Padding(4);
            this.btnFirstFrame.Name = "btnFirstFrame";
            this.btnFirstFrame.Size = new System.Drawing.Size(48, 28);
            this.btnFirstFrame.TabIndex = 21;
            this.btnFirstFrame.Text = "< |";
            this.btnFirstFrame.UseVisualStyleBackColor = true;
            this.btnFirstFrame.Click += new System.EventHandler(this.btnFirstFrame_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(132, 4);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(81, 28);
            this.btnStop.TabIndex = 20;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(5, 4);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(4);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(119, 28);
            this.btnPlay.TabIndex = 19;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // txtModelPreviewScale
            // 
            this.txtModelPreviewScale.Location = new System.Drawing.Point(121, 1);
            this.txtModelPreviewScale.Margin = new System.Windows.Forms.Padding(4);
            this.txtModelPreviewScale.Name = "txtModelPreviewScale";
            this.txtModelPreviewScale.Size = new System.Drawing.Size(140, 22);
            this.txtModelPreviewScale.TabIndex = 15;
            this.txtModelPreviewScale.TextChanged += new System.EventHandler(this.txtModelPreviewScale_TextChanged);
            // 
            // tsModelPreview
            // 
            this.tsModelPreview.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsModelPreview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblModelPreviewScale});
            this.tsModelPreview.Location = new System.Drawing.Point(0, 0);
            this.tsModelPreview.Name = "tsModelPreview";
            this.tsModelPreview.Size = new System.Drawing.Size(855, 25);
            this.tsModelPreview.TabIndex = 1;
            this.tsModelPreview.Text = "toolStrip1";
            // 
            // lblModelPreviewScale
            // 
            this.lblModelPreviewScale.Name = "lblModelPreviewScale";
            this.lblModelPreviewScale.Size = new System.Drawing.Size(102, 22);
            this.lblModelPreviewScale.Text = "Preview Scale:";
            // 
            // mstrMain
            // 
            this.mstrMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mstrMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnitLoad,
            this.mnitImport,
            this.mnitExport});
            this.mstrMain.Location = new System.Drawing.Point(0, 0);
            this.mstrMain.Name = "mstrMain";
            this.mstrMain.Size = new System.Drawing.Size(1184, 28);
            this.mstrMain.TabIndex = 3;
            this.mstrMain.Text = "menuStrip1";
            // 
            // mnitLoad
            // 
            this.mnitLoad.Name = "mnitLoad";
            this.mnitLoad.Size = new System.Drawing.Size(93, 24);
            this.mnitLoad.Text = "Load BMD";
            this.mnitLoad.Click += new System.EventHandler(this.mnitLoad_Click);
            // 
            // mnitImport
            // 
            this.mnitImport.Name = "mnitImport";
            this.mnitImport.Size = new System.Drawing.Size(104, 24);
            this.mnitImport.Text = "Import BMA";
            this.mnitImport.Click += new System.EventHandler(this.mnitImport_Click);
            // 
            // mnitExport
            // 
            this.mnitExport.Name = "mnitExport";
            this.mnitExport.Size = new System.Drawing.Size(90, 24);
            this.mnitExport.Text = "Save BMA";
            this.mnitExport.Click += new System.EventHandler(this.mnitSave_Click);
            // 
            // glModelView
            // 
            this.glModelView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.glModelView.BackColor = System.Drawing.Color.Black;
            this.glModelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glModelView.Location = new System.Drawing.Point(0, 0);
            this.glModelView.Margin = new System.Windows.Forms.Padding(0);
            this.glModelView.Name = "glModelView";
            this.glModelView.Size = new System.Drawing.Size(855, 645);
            this.glModelView.TabIndex = 0;
            this.glModelView.VSync = false;
            // 
            // MaterialAnimationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 736);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.ssMain);
            this.Controls.Add(this.mstrMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mstrMain;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MaterialAnimationEditor";
            this.Text = "Material Animation (BMA) Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MaterialAnimationEditor_FormClosed);
            this.Load += new System.EventHandler(this.MaterialAnimationEditor_Load);
            this.ssMain.ResumeLayout(false);
            this.ssMain.PerformLayout();
            this.tcMain.ResumeLayout(false);
            this.tpgModel.ResumeLayout(false);
            this.splModel.Panel1.ResumeLayout(false);
            this.splModel.Panel1.PerformLayout();
            this.splModel.Panel2.ResumeLayout(false);
            this.splModel.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splModel)).EndInit();
            this.splModel.ResumeLayout(false);
            this.tsModelPreview.ResumeLayout(false);
            this.tsModelPreview.PerformLayout();
            this.mstrMain.ResumeLayout(false);
            this.mstrMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip ssMain;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpgModel;
        private System.Windows.Forms.SplitContainer splModel;
        private FormControls.ModelGLControlWithMarioSizeReference glModelView;
        private System.Windows.Forms.MenuStrip mstrMain;
        private System.Windows.Forms.ToolStripMenuItem mnitLoad;
        private System.Windows.Forms.ToolStripMenuItem mnitImport;
        private System.Windows.Forms.ToolStripMenuItem mnitExport;
        private System.Windows.Forms.ToolStripStatusLabel lblMainStatus;
        private System.Windows.Forms.TextBox txtModelPreviewScale;
        private System.Windows.Forms.ToolStrip tsModelPreview;
        private System.Windows.Forms.ToolStripLabel lblModelPreviewScale;
        private System.Windows.Forms.TextBox txtNumFrames;
        private System.Windows.Forms.Label lblFrameNum;
        private System.Windows.Forms.TextBox txtCurrentFrameNum;
        private System.Windows.Forms.Label lblFrame;
        private System.Windows.Forms.Button btnLastFrame;
        private System.Windows.Forms.Button btnNextFrame;
        private System.Windows.Forms.Button btnPreviousFrame;
        private System.Windows.Forms.Button btnFirstFrame;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstMaterialProperties;
        private System.Windows.Forms.Button btnRemoveFrame;
        private System.Windows.Forms.Button btnAddFrame;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtAlpha;
        private System.Windows.Forms.CheckBox chkAlpha;
        private System.Windows.Forms.Button btnEmi;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtEmiRed;
        private System.Windows.Forms.CheckBox chkEmiBlue;
        private System.Windows.Forms.TextBox txtEmiBlue;
        private System.Windows.Forms.CheckBox chkEmiRed;
        private System.Windows.Forms.TextBox txtEmiGreen;
        private System.Windows.Forms.CheckBox chkEmiGreen;
        private System.Windows.Forms.Button btnSpec;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSpecRed;
        private System.Windows.Forms.CheckBox chkSpecBlue;
        private System.Windows.Forms.TextBox txtSpecBlue;
        private System.Windows.Forms.CheckBox chkSpecRed;
        private System.Windows.Forms.TextBox txtSpecGreen;
        private System.Windows.Forms.CheckBox chkSpecGreen;
        private System.Windows.Forms.Button btnAmb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAmbRed;
        private System.Windows.Forms.CheckBox chkAmbBlue;
        private System.Windows.Forms.TextBox txtAmbBlue;
        private System.Windows.Forms.CheckBox chkAmbRed;
        private System.Windows.Forms.TextBox txtAmbGreen;
        private System.Windows.Forms.CheckBox chkAmbGreen;
        private System.Windows.Forms.Button btnDif;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDifRed;
        private System.Windows.Forms.CheckBox chkDifBlue;
        private System.Windows.Forms.TextBox txtDifBlue;
        private System.Windows.Forms.CheckBox chkDifRed;
        private System.Windows.Forms.TextBox txtDifGreen;
        private System.Windows.Forms.CheckBox chkDifGreen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkHasAnim;
    }
}