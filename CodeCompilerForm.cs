﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe
{
    public partial class CodeCompilerForm : Form
    {
        public CodeCompilerForm()
        {
            InitializeComponent();
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            if (!Patcher.PatchMaker.PatchToSupportBigASMHacks())
                return;

            //code and patcher borrowed from NSMBe and edited.
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(txtFolder.Text);
            uint addr = 0x02400000;
            if (btnOverlay.Checked)
                addr = new NitroOverlay(Program.m_ROM, uint.Parse(txtOverlayId.Text)).GetRAMAddr();
            else if (btnInjection.Checked)
                throw new NotImplementedException();
            else if (!btnDynamicLibrary.Checked)
                addr = uint.Parse(txtOffset.Text, System.Globalization.NumberStyles.HexNumber);
            
            Patcher.PatchMaker pm = new Patcher.PatchMaker(dir, addr);

            byte[] ret = null;

            if (btnDynamicLibrary.Checked)
            {
                ret = pm.makeDynamicLibrary();
            }
            else if (btnOverlay.Checked)
            {
                pm.compilePatch();
                pm.makeOverlay(uint.Parse(txtOverlayId.Text));
                return;
            }
            else if (btnInjection.Checked)
            {
                throw new NotImplementedException();
            }
            else
            {
                pm.compilePatch();
                ret = pm.generatePatch();
            }

            if (ret == null) { return; }

            bool isOut = btnExternal.Checked;
            if (isOut)
            {
                string file = txtOutput.Text;
                if (file == "")
                    return;
                
                System.IO.File.WriteAllBytes(file, ret);
            }
            else if (txtInput.Text != "")
            {
                var file = Program.m_ROM.GetFileFromName(txtInput.Text);
                file.m_Data = ret;
                file.SaveChanges();
            }
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(txtFolder.Text);
            Patcher.PatchCompiler.cleanPatch(dir);
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = System.IO.Path.GetDirectoryName(Program.m_ROMPath);
            folderBrowserDialog.ShowDialog();
            txtFolder.Text = folderBrowserDialog.SelectedPath;
        }

        private void btnSelectExternal_Click(object sender, EventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            f.RestoreDirectory = true;
            if (f.ShowDialog() == DialogResult.OK) {
                txtOutput.Text = f.FileName;
            }
        }

        private void btnSelectInternal_Click(object sender, EventArgs e)
        {
            ROMFileSelect r = new ROMFileSelect();
            r.ShowDialog();
            txtInput.Text = r.m_SelectedFile;
        }

        private void btnGeneric_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void btnDynamicLibrary_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void btnInjection_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void btnOverlay_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void btnInternal_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void btnExternal_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            txtOffset.Enabled = !btnInjection.Checked && !btnDynamicLibrary.Checked && !btnOverlay.Checked;
            txtOverlayId.Enabled = !btnInjection.Checked && btnOverlay.Checked;
            btnInternal.Enabled = txtInput.Enabled = btnInternal.Checked && !btnInjection.Checked && !btnOverlay.Checked;
            btnExternal.Enabled = txtOutput.Enabled = btnExternal.Checked && !btnInjection.Checked && !btnOverlay.Checked;
            btnSelectInternal.Enabled = !btnExternal.Checked && !btnInjection.Checked;
            btnSelectExternal.Enabled = btnExternal.Checked && !btnInjection.Checked;
            txtFolder.Enabled = btnSelectFolder.Enabled = !btnInjection.Checked;
        }
    }
}
