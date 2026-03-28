
namespace SM64DSe
{
    partial class KuppaScriptDecompilerForm
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
			this.lstScripts = new System.Windows.Forms.ListBox();
			this.txtCode = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// lstScripts
			// 
			this.lstScripts.Font = new System.Drawing.Font("Consolas", 8F);
			this.lstScripts.FormattingEnabled = true;
			this.lstScripts.ItemHeight = 15;
			this.lstScripts.Location = new System.Drawing.Point(12, 31);
			this.lstScripts.Name = "lstScripts";
			this.lstScripts.Size = new System.Drawing.Size(185, 589);
			this.lstScripts.TabIndex = 20;
			this.lstScripts.SelectedIndexChanged += new System.EventHandler(this.lstScripts_SelectedIndexChanged);
			// 
			// txtCode
			// 
			this.txtCode.Location = new System.Drawing.Point(203, 31);
			this.txtCode.Name = "txtCode";
			this.txtCode.Size = new System.Drawing.Size(1119, 598);
			this.txtCode.TabIndex = 28;
			this.txtCode.Text = "";
			// 
			// KuppaScriptDecompilerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1334, 641);
			this.Controls.Add(this.txtCode);
			this.Controls.Add(this.lstScripts);
			this.Name = "KuppaScriptDecompilerForm";
			this.Text = "Kuppa Script Decompiler";
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListBox lstScripts;
        private System.Windows.Forms.RichTextBox txtCode;
	}
}