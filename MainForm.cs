/*
    Copyright 2012 Kuribo64
    This file is part of SM64DSe.
    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.
    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.vis
    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Web;
using SM64DSe.ImportExport.LevelImportExport;
using System.Globalization;

namespace SM64DSe
{
    public partial class MainForm : Form
    {
        private System.Diagnostics.Process nitroStudio = null;

        private void LoadROM(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("The specified file doesn't exist.", Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Program.m_ROMPath != "")
            {
                while (Program.m_LevelEditors.Count > 0)
                    Program.m_LevelEditors[0].Close();


                Program.m_ROM.EndRW();
            }

            Program.m_ROMPath = filename;
            Program.m_ROM = new NitroROM(Program.m_ROMPath);

            if (Program.m_ROM.m_Version != NitroROM.Version.EUR)
            {
                MessageBox.Show("Please use a European ROM of SM64DS with this version of the editor.", "Invalid ROM version");
                Close();
            }

            Program.m_ROM.BeginRW();

            if (Program.m_ROM.NeedsPatch())
            {
                DialogResult res = MessageBox.Show(
                    "This ROM needs to be patched before the editor can work with it.\n\n" +
                    "Do you want to first make a backup of it in case the patching\n" +
                    "operation goes wrong somehow?",
                    Program.AppTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    sfdSaveFile.FileName = Program.m_ROMPath.Substring(0, Program.m_ROMPath.Length - 4) + "_bak.nds";
                    if (sfdSaveFile.ShowDialog(this) == DialogResult.OK)
                    {
                        Program.m_ROM.EndRW();
                        File.Copy(Program.m_ROMPath, sfdSaveFile.FileName, true);
                    }
                }
                else if (res == DialogResult.Cancel)
                {
                    Program.m_ROM.EndRW();
                    Program.m_ROMPath = "";
                    return;
                }

                // switch to buffered RW mode (faster for patching)
                Program.m_ROM.EndRW();
                Program.m_ROM.BeginRW(true);

                try { Program.m_ROM.Patch(); }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "An error occured while patching your ROM.\n" +
                        "No changes have been made to your ROM.\n" +
                        "Try using a different ROM. If the error persists, report it to Mega-Mario, with the details below:\n\n" +
                        ex.Message + "\n" +
                        ex.StackTrace,
                        Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine(ex.StackTrace);
                    Program.m_ROM.EndRW(false);
                    Program.m_ROMPath = "";
                    return;
                }

                Program.m_ROM.EndRW();

                // this 
                // this should fix the overlays having an incorrect file id
                Program.m_ROM.StartFilesystemEdit();
                Program.m_ROM.SaveFilesystem();
            }
            else
                Program.m_ROM.EndRW();

            Program.m_ROM.BeginRW();
            Program.m_ROM.LoadTables();
            Program.m_ROM.EndRW();

            // Program.m_ShaderCache = new ShaderCache();

            btnRefresh.Enabled = true;
            cbLevelListDisplay.Enabled = true;

            if (cbLevelListDisplay.SelectedIndex == -1)
                cbLevelListDisplay.SelectedIndex = 0;
            else
                btnRefresh.PerformClick();

            tvFileList.Nodes.Clear();
            ROMFileSelect.LoadFileList(tvFileList);
            tvARM9Overlays.Nodes.Clear();
            ROMFileSelect.LoadOverlayList(tvARM9Overlays);

            btnASMHacking.Enabled = true;
            btnTools.Enabled = true;
            btnMore.Enabled = true;
            btnFileEditors.Enabled = true;
            btnLZCompressWithHeader.Enabled = true;
            btnLZDecompressWithHeader.Enabled = true;
            btnLZForceCompression.Enabled = true;
            btnLZForceDecompression.Enabled = true;
            btnEditLevelNamesOverlays.Enabled = true;
        }

        public MainForm(string[] args)
        {
            InitializeComponent();
            Text = Program.AppTitle + " " + Program.AppVersion + " " + Program.AppDate;
            Program.m_ROMPath = "";
            Program.m_LevelEditors = new List<LevelEditorForm>();

            slStatusLabel.Text = "Ready";
            ObjectDatabase.Initialize();

            if (args.Length >= 1) LoadROM(args[0]);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ObjectDatabase.LoadFallback();
            try { ObjectDatabase.Load(); }
            catch { }

            if (!Properties.Settings.Default.AutoUpdateODB)
                return;

            ObjectDatabase.m_WebClient.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(this.ODBDownloadProgressChanged);
            //ObjectDatabase.m_WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.ODBDownloadDone);
            ObjectDatabase.m_WebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.ODBDownloadDone);

            ObjectDatabase.Update(false);
        }

        private void ODBDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!spbStatusProgress.Visible)
            {
                slStatusLabel.Text = "Updating object database...";
                spbStatusProgress.Visible = true;
            }

            spbStatusProgress.Value = e.ProgressPercentage;
        }

        private void ODBDownloadDone(object sender, DownloadStringCompletedEventArgs e)
        {
            spbStatusProgress.Visible = false;

            if (e.Cancelled || (e.Error != null))
            {
                slStatusLabel.Text = "Object database update " + (e.Cancelled ? "cancelled" : "failed") + ".";
            }
            else
            {
                if (e.Result == "noupdate")
                {
                    slStatusLabel.Text = "Object database already up to date.";
                }
                else
                {
                    slStatusLabel.Text = "Object database updated.";

                    try
                    {
                        File.WriteAllText("objectdb.xml", e.Result);
                    }
                    catch
                    {
                        slStatusLabel.Text = "Object database update failed.";
                    }
                }
            }

            try { ObjectDatabase.Load(); }
            catch { }
        }

        private void btnOpenROM_Click(object sender, EventArgs e)
        {
            if (ofdOpenFile.ShowDialog(this) == DialogResult.OK)
                LoadROM(ofdOpenFile.FileName);
        }

        private void OpenLevel(int levelid)
        {
            if ((levelid < 0) || (levelid >= 52))
                return;

            foreach (LevelEditorForm lvledit in Program.m_LevelEditors)
            {
                if (lvledit.m_LevelID == levelid)
                {
                    lvledit.Focus();
                    return;
                }
            }

            // try
            {
                LevelEditorForm newedit = new LevelEditorForm(Program.m_ROM, levelid);
                newedit.Show();
                Program.m_LevelEditors.Add(newedit);
            }
            /*catch (Exception ex)
            {
                MessageBox.Show("The following error occured while opening the level:\n" + ex.Message,
                    Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }


        private void btnEditLevel_Click(object sender, EventArgs e)
        {
            OpenLevel(lbxLevels.SelectedIndex);
        }

        private void lbxLevels_DoubleClick(object sender, EventArgs e)
        {
            OpenLevel(lbxLevels.SelectedIndex);
        }

        private void lbxLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEditLevel.Enabled = (lbxLevels.SelectedIndex != -1);
            btnEditCollisionMap.Enabled = (lbxLevels.SelectedIndex != -1);
        }

        private void btnDumpObjInfo_Click(object sender, EventArgs e)
        {
            if (Program.m_ROM.m_Version != NitroROM.Version.EUR)
            {
                MessageBox.Show("Only compatible with EUR ROMs.", Program.AppTitle);
                return;
            }

            DumpObjectInfo();
        }

        private void btnUpdateODB_Click(object sender, EventArgs e)
        {
            ObjectDatabase.Update(true);
        }

        private void btnHalp_Click(object sender, EventArgs e)
        {
            string msg = Program.AppTitle + " " + Program.AppVersion + " " + Program.AppDate + "\n\n" +
                "A level editor for Super Mario 64 DS 2: The New Stars being developed by Splatterboy.\n" +
                "\n" +
                "Based on SM64DSe Ultimate by Gota7 and jupahe64 (with DL generation code by pants64DS)\n" +
                "\n" +
                "Original SM64DSe coding and design by Arisotura (StapleButter), with help from others:\n" +
                "- Treeki: the overlay decompression (Jap77), the object list and other help\n" +
                "- Dirbaio: other help\n" +
                "- blank: help with generating collision\n" +
                "- Josh65536: ASM hacking template v2, BCA optimisation, level editor enhancements and other help\n" +
                "- Fiachra: former developer and maintainer\n" +
                "\n" +
                Program.AppTitle + " is free software. If you paid for it, notify someone about it.\n" +
                "\n" +
                "Visit the SM64DS Hacking Discord for more details.";

            MessageBox.Show(msg, "About " + Program.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEditCollisionMap_Click(object sender, EventArgs e)
        {
            uint overlayID = Program.m_ROM.GetLevelOverlayID(lbxLevels.SelectedIndex);
            NitroOverlay currentOverlay = new NitroOverlay(Program.m_ROM, overlayID);
            NitroFile currentKCL = Program.m_ROM.GetFileFromInternalID(currentOverlay.Read16((uint)(0x6A)));
            if (!Properties.Settings.Default.UseSimpleModelAndCollisionMapImporters)
            {
                ModelAndCollisionMapEditor kclForm =
                    new ModelAndCollisionMapEditor(null, currentKCL.m_Name, 1f, ModelAndCollisionMapEditor.StartMode.CollisionMap);
                kclForm.Show();
            }
            else
            {
                KCLEditorForm kclForm = new KCLEditorForm(currentKCL);
                kclForm.Show();
            }
        }

        private string m_SelectedFile;
        private string m_SelectedOverlay;

        private void tvFileList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            m_SelectedFile = e.Node == null || e.Node.Tag == null ? "" : e.Node.Tag.ToString();

            if (m_SelectedFile == "")
                return;
            
            Console.WriteLine(m_SelectedFile);

            string status;
            if (Program.m_ROM.GetFileIDFromName(this.m_SelectedFile) != ushort.MaxValue)
                status = m_SelectedFile.Last() == '/' ?
                    string.Format("Directory, ID = 0x{0:x4}", Program.m_ROM.GetDirIDFromName(m_SelectedFile.TrimEnd('/'))) :
                    string.Format("File, ID = 0x{0:x4}, Ov0ID = 0x{1:x4}",
                        Program.m_ROM.GetFileIDFromName(m_SelectedFile),
                        Program.m_ROM.GetFileEntries()[Program.m_ROM.GetFileIDFromName(m_SelectedFile)].InternalID);
            else
                status = "";
            slStatusLabel.Text = status;

            UpdateOpenFileButton();
        }

        private void btnExtractRaw_Click(object sender, EventArgs e)
        {
            if (m_SelectedFile == null || m_SelectedFile.Equals(""))
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileName(m_SelectedFile);
            if (sfd.ShowDialog() == DialogResult.Cancel)
                return;

            System.IO.File.WriteAllBytes(sfd.FileName, Program.m_ROM.GetFileFromName(m_SelectedFile).m_Data);
        }

        private void btnReplaceRaw_Click(object sender, EventArgs e)
        {
            if (m_SelectedFile == null || m_SelectedFile.Equals(""))
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;

            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            file.Clear();
            file.WriteBlock(0, System.IO.File.ReadAllBytes(ofd.FileName));
            file.SaveChanges();
        }

        private void btnLZDecompressWithHeader_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            // NitroFile automatically decompresses on load if LZ77 header present
            file.SaveChanges();
        }

        private void btnLZForceDecompression_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            try
            {
                file.ForceDecompression();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error trying to force decompression of \"" + file.m_Name + "\", " +
                    "this file may not use LZ77 compression (no header)\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
            file.SaveChanges();
        }

        private void btnLZCompressWithHeader_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            try
            {
                file.Compress();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error trying to compress the file \"" + file.m_Name + "\" with " +
                    "LZ77 compression (with header)\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
            file.SaveChanges();
        }

        private void btnLZForceCompression_Click(object sender, EventArgs e)
        {
            NitroFile file = Program.m_ROM.GetFileFromName(m_SelectedFile);
            try
            {
                file.ForceCompression();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error trying to compress the file \"" + file.m_Name + "\" with " +
                    "LZ77 compression (no header)\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
            file.SaveChanges();
        }

        private void btnDecompressOverlay_Click(object sender, EventArgs e)
        {
            uint ovlID = uint.Parse(m_SelectedOverlay.Substring(8));
            NitroOverlay ovl = new NitroOverlay(Program.m_ROM, ovlID);
            ovl.SaveChanges();
        }

        private void btnExtractOverlay_Click(object sender, EventArgs e)
        {
            if (m_SelectedOverlay == null)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = m_SelectedOverlay;
            if (sfd.ShowDialog() == DialogResult.Cancel)
                return;

            uint ovlID = uint.Parse(m_SelectedOverlay.Substring(8));
            System.IO.File.WriteAllBytes(sfd.FileName, new NitroOverlay(Program.m_ROM, ovlID).m_Data);
        }

        private void btnReplaceOverlay_Click(object sender, EventArgs e)
        {
            if (m_SelectedOverlay == null)
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;

            uint ovlID = uint.Parse(m_SelectedOverlay.Substring(8));
            NitroOverlay ovl = new NitroOverlay(Program.m_ROM, ovlID);
            ovl.Clear();
            ovl.WriteBlock(0, System.IO.File.ReadAllBytes(ofd.FileName));
            ovl.SaveChanges();
        }

        private void tvARM9Overlays_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
                m_SelectedOverlay = null;
            else
                m_SelectedOverlay = e.Node.Tag.ToString();
        }

        private void mnitToolsModelAndCollisionMapImporter_Click(object sender, EventArgs e)
        {
            new ModelAndCollisionMapEditor().Show();
        }

        private void mnitToolsCollisionMapEditor_Click(object sender, EventArgs e)
        {
            new ModelAndCollisionMapEditor(ModelAndCollisionMapEditor.StartMode.CollisionMap).Show();
        }

        private void mnitToolsModelAnimationEditor_Click(object sender, EventArgs e)
        {
            AnimationEditorForm animationEditorForm = new AnimationEditorForm();
            animationEditorForm.Show();
        }

        private void mnitToolsTextEditor_Click(object sender, EventArgs e)
        {
            new TextEditorForm().Show();
        }

        private void mnitToolsBTPEditor_Click(object sender, EventArgs e)
        {
            new TextureEditorForm().Show();
        }

        private void mnitCodeCompiler_Click(object sender, EventArgs e)
        {
            new CodeCompilerForm().Show();
        }

        private void platformEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Templates.PlatformTemplateForm().Show();
        }

        private void cbLevelListDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRefresh.PerformClick();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            lbxLevels.Items.Clear();

            int i = 0;
            List<string> ids = new List<string>();
            List<string> internalNames = new List<string>();
            List<string> names = new List<string>();

            foreach (string lvlName in Strings.LevelNames())
            {
                ids.Add("[" + i + "]");
                internalNames.Add(Program.m_ROM.GetInternalLevelNameFromID(i));
                names.Add(lvlName);
                i++;
            }
            i = 0;
            if (cbLevelListDisplay.SelectedIndex == 0)
            {
                int maxIdLen = ids.Select(x => x.Length).Max();
                int maxInternalLen = internalNames.Select(x => x.Length).Max() + 16;
                foreach (string lvlName in Strings.LevelNames())
                {
                    string id = ids[i];
                    while (id.Length < maxIdLen + 1)
                        id += " ";

                    string internalN = internalNames[i];
                    int numTabs = ((maxInternalLen + (maxInternalLen % 8)) - (internalN.Length + (8 - internalN.Length % 8))) / 8;

                    for (int j = 0; j < numTabs; j++)
                        internalN += "\t";

                    lbxLevels.Items.Add(id + "\t" + internalN + names[i]);
                    i++;
                }
            }
            else if (cbLevelListDisplay.SelectedIndex == 1)
            {
                foreach (string lvlName in Strings.LevelNames())
                {
                    lbxLevels.Items.Add(lvlName);
                    i++;
                }
            }
            else if (cbLevelListDisplay.SelectedIndex == 2)
            {
                foreach (string lvlName in Strings.ShortLvlNames())
                {
                    lbxLevels.Items.Add(i + "\t[" + internalNames[i] + "]");
                    i++;
                }
            }
            else if (cbLevelListDisplay.SelectedIndex == 3)
            {
                int maxInternalLen = internalNames.Select(x => x.Length).Max() + 8;

                foreach (string lvlName in Strings.ShortLvlNames())
                {
                    string trimmedName = lvlName.Trim();
                    int numTabs = ((maxInternalLen + (maxInternalLen % 8)) - (trimmedName.Length + (8 - trimmedName.Length % 8))) / 8;

                    for (int j = 0; j < numTabs; j++)
                        trimmedName += "\t";

                    lbxLevels.Items.Add(trimmedName + " [" + internalNames[i] + "]");
                    i++;
                }
            }
            else
            {
                int hubCounter = 1;
                foreach (string lvlName in Strings.ShortLvlNames())
                {
                    ushort selectorId = Program.m_ROM.GetActSelectorIdByLevelID(i);
                    string lvlString = "";
                    if (selectorId < 29)
                    {
                        lvlString = Program.m_ROM.GetInternalLevelNameFromID(i);
                        while (lvlString.StartsWith(" "))
                            lvlString = lvlString.Remove(0, 1);

                        if (selectorId < 16)
                        {
                            if (lvlString.StartsWith((selectorId + 1).ToString()))
                                lvlString = lvlString.Remove(0, selectorId.ToString().Length + 1);
                        }
                        while (lvlString.StartsWith(" "))
                            lvlString = lvlString.Remove(0, 1);


                        string optimizedLvlString = "";
                        char lastChar = ' ';
                        foreach (char c in lvlString)
                        {
                            string letter = c.ToString();
                            if (lastChar == ' ')
                                optimizedLvlString = optimizedLvlString + letter.ToUpper();
                            else
                                optimizedLvlString = optimizedLvlString + letter.ToLower();
                            lastChar = c;
                        }
                        lvlString = optimizedLvlString;
                    }
                    else if (selectorId == 29)
                    {
                        lvlString = "Part " + hubCounter + " of the Hubworld";
                        hubCounter++;
                    }
                    else if (selectorId == 255)
                    {
                        lvlString = "TestMap";
                    }
                    else
                    {
                        lvlString = "Cant find a Levelname for ActSelectorID " + i;
                    }
                    lbxLevels.Items.Add(lvlString);
                    i++;
                }
            }
        }

        private void extendItcmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.m_ROM.BeginRW();
            uint arm9Size = Program.m_ROM.Read32(0x2c);
            Program.m_ROM.EndRW();
            
            if (arm9Size == 0x9f038)
            {
                MessageBox.Show("The arm9.bin ITCM has already been extended.", "ITCM already extended.");
                return;
            }    

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = "bin";
            openFileDialog.Filter = "Binary ARM code file|*.bin";
            openFileDialog.Title = "Select arm9.bin ITCM extension.";

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                Program.m_ROM.StartFilesystemEdit();
                Program.m_ROM.SaveFilesystem(openFileDialog.FileName);
            }
        }

        private void kuppaScriptEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KuppaScriptEditor kse = new KuppaScriptEditor();
            kse.Show();
        }

        private void editFileSystemToolStripMenuItem_Click(object sender, EventArgs e) {
            if (Program.m_ROM.m_Version != NitroROM.Version.EUR) {
                MessageBox.Show("This is for EUR ROMs only!");
                return;
            }
            if (new FilesystemEditorForm(this).ShowDialog() != DialogResult.OK)
                return;
            this.LoadROM(Program.m_ROMPath);
        }

        private void editOverlaysToolStripMenuItem_Click(object sender, EventArgs e) {
            if (Program.m_ROM.m_Version != NitroROM.Version.EUR) {
                MessageBox.Show("This is for EUR ROMs only!");
                return;
            }
            new OverlayEditor().ShowDialog();
        }

        private void importPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "SM64DSe Patches(*.fss;*.ccs;*.hs)|*.fss;*ccs;*hs";
            o.RestoreDirectory = true;
            if (o.ShowDialog() != DialogResult.OK)
                return;

            string basePath = Path.GetDirectoryName(o.FileName) + "\\";

            if (o.FileName.EndsWith(".hs"))
            {
                Patcher.PatchProcessor.InsertHooks(new DirectoryInfo(basePath), Path.GetFileName(o.FileName));
                return;
            }

            new PatchViewerForm(o.FileName).ShowDialog();

            tvFileList.Nodes.Clear();
            ROMFileSelect.LoadFileList(tvFileList);
            tvARM9Overlays.Nodes.Clear();
            ROMFileSelect.LoadOverlayList(tvARM9Overlays);
        }

        private void tsToolBar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void particleTextureSPTEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ParticleEditorForm().Show();
        }

        private void particleArchiveSPAEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ParticleViewerForm().Show();
        }

        private void materialAnimationBMAEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MaterialAnimationEditor().Show();
        }

        private void textureAnimationBTAEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // new TextureAnimationEditor().Show();
        }

        private void OnCloseNitroStudio(object sender, EventArgs e)
        {
            if (nitroStudio == null)
                return;

            if (!File.Exists(Application.StartupPath + "\\NitroStudio\\_temp.sdat"))
            {
                MessageBox.Show("Error! No sdat found after closing Nitro Studio 2.", "No SDAT found.");
            }
            else
			{
                NitroFile sdat = Program.m_ROM.GetFileFromName("data/sound_data.sdat");
                sdat.m_Data = File.ReadAllBytes(Application.StartupPath + "\\NitroStudio\\_temp.sdat");
                sdat.SaveChanges();
                File.Delete(Application.StartupPath + "\\NitroStudio\\_temp.sdat");
            }
            
            nitroStudio = null;

            tsToolBar.Invoke(new MethodInvoker(delegate { SDATEditorToolStripMenuItem.Text = "SDAT Editor (Nitro Studio 2)"; }));
        }

        private void SDATEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nitroStudio != null)
            {
                File.Delete(Application.StartupPath + "\\NitroStudio\\_temp.sdat");

                nitroStudio.Kill();
                nitroStudio = null;

                SDATEditorToolStripMenuItem.Text = "SDAT Editor (Nitro Studio 2)";
                return;
            }

            OpenSDAT();
        }

        private void OpenSDAT()
        {
            File.WriteAllBytes(Application.StartupPath + "\\NitroStudio\\_temp.sdat", Program.m_ROM.GetFileFromName("data/sound_data.sdat").m_Data);

            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
            start.Arguments = Application.StartupPath + "\\NitroStudio\\_temp.sdat";
            start.FileName = Application.StartupPath + "\\NitroStudio\\NitroStudio2.exe";
            start.WorkingDirectory = Application.StartupPath + "\\NitroStudio";

            nitroStudio = System.Diagnostics.Process.Start(start);
            nitroStudio.EnableRaisingEvents = true;
            nitroStudio.Exited += OnCloseNitroStudio;

            SDATEditorToolStripMenuItem.Text = "Quit Nitro Studio 2 without saving";

            // Console.WriteLine(Application.StartupPath + "\\NitroStudio\\NitroStudio2.exe " + Application.StartupPath + "\\NitroStudio\\_temp.sdat");
        }

        private void btnEditLevelNamesOverlays_Click(object sender, EventArgs e)
        {
            new LevelNameOverlayEditorForm().ShowDialog();
        }

        private void btnEditObjectDB_Click(object sender, EventArgs e)
        {
            new ObjectDatabaseEdtiorForm().ShowDialog();
            ObjectDatabase.LoadFallback();
            try { ObjectDatabase.Load(); }
            catch { }
        }

        private void checkLevelReqToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 52; i++)
                CheckLevelRequirements(ref i);
        }

        private void CheckLevelRequirements(ref int levelID)
        {
            Level level = new Level(levelID);
            level.DetermineAvailableObjects();

            if (!level.ContainsObjectsIncompatibleWithBankSettings())
                return;

            bool levelChanged = false, banksChanged = false;

            foreach (KeyValuePair<uint, LevelObject> obj in level.m_LevelObjects)
            {
                if (!level.m_ObjAvailable[obj.Value.ID])
                {
                    ObjectDatabase.ObjectInfo objinfo = ObjectDatabase.m_ObjectInfo[obj.Value.ID];

                    string name = !string.IsNullOrWhiteSpace(objinfo.m_Name) ? objinfo.m_Name : objinfo.m_InternalName;
                    string message = "Incompatible object found in level " + levelID + ": " + name +
                        "\nDo you want to apply the following changes?\n";

                    if (objinfo.m_BankRequirement != 0)
                    {
                        message += "Change bank " + objinfo.m_NumBank + " from  " + level.m_LevelSettings.ObjectBanks[objinfo.m_NumBank] + " to " + objinfo.m_BankSetting + " (can make other objects incompatible)\n";

                        if (MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            levelChanged = banksChanged = true;
                            level.m_LevelSettings.ObjectBanks[objinfo.m_NumBank] = (uint)objinfo.m_BankSetting;
                            level.DetermineAvailableObjects();
                        }
                    }
                    else if (objinfo.m_dlRequirements != null && objinfo.m_dlRequirements.Length > 0)
                    {
                        List<string> unavailableDLs = objinfo.m_dlRequirements.ToList();

                        IEnumerable<string> levelDLs;
                        if (level.m_DynLibIDs == null)
                            levelDLs = new string[0];
                        else
                            levelDLs = level.m_DynLibIDs.Select(d => Program.m_ROM.GetFileFromInternalID(d).m_Name);

                        foreach (string unavailableDL in unavailableDLs)
                        {
                            if (levelDLs.Contains(unavailableDL))
                                unavailableDLs.Remove(unavailableDL);
                        }

                        message += "Add the following DLs (creates a file if it doesn't exist):\n";
                        foreach (string unavailableDL in unavailableDLs)
                            message += unavailableDL + "\n";

                        if (MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            levelChanged = true;

                            foreach (string unavailableDL in unavailableDLs)
                            {
                                ushort ov0ID = Program.m_ROM.GetOv0IDFromName(unavailableDL);

                                if (ov0ID == 0xffff)
                                {
                                    int lastSlash = unavailableDL.LastIndexOf('/');
                                    string dirName = unavailableDL.Substring(0, lastSlash + 1);
                                    string fileName = unavailableDL.Substring(lastSlash + 1);

                                    Program.m_ROM.StartFilesystemEdit();
                                    Program.m_ROM.AddFile(dirName, fileName, new byte[] { 0xde, 0xad, 0xbe, 0xef }, tvFileList.Nodes[0]);
                                    Program.m_ROM.SaveFilesystem();

                                    ov0ID = Program.m_ROM.GetOv0IDFromName(unavailableDL);
                                    if (ov0ID == 0xffff)
                                        throw new Exception("File (" + unavailableDL + ") could not be added.");
                                }

                                level.m_DynLibIDs.Add(ov0ID);
                            }

                            level.DetermineAvailableObjects();
                        }
                    }
                }
            }

            if (levelChanged)
                level.SaveChanges();

            if (banksChanged)
                levelID--;
        }

        string m_SavedFile = null;

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Trying to open: " + m_SelectedFile);

            if (m_SelectedFile.EndsWith(".bmd"))
            {
                if (string.IsNullOrWhiteSpace(m_SavedFile))
                    new ModelAndCollisionMapEditor(m_SelectedFile, null, 0.008f, ModelAndCollisionMapEditor.StartMode.ModelAndCollisionMap, true).Show();
                else if (m_SavedFile.EndsWith(".btp"))
                    new TextureEditorForm(m_SavedFile, m_SelectedFile).Show();
                else if (m_SavedFile.EndsWith(".bca"))
                    new AnimationEditorForm(m_SavedFile, m_SelectedFile).Show();
                else if (m_SavedFile.EndsWith(".bma"))
                    new TextureEditorForm(m_SavedFile, m_SelectedFile).Show();
                /*else if (m_SavedFile.EndsWith(".bta"))
                    new TextureAnimationEditorForm(m_SavedFile, m_SelectedFile).Show();*/
            }
            else if (m_SelectedFile.EndsWith(".kcl"))
                new ModelAndCollisionMapEditor(null, m_SelectedFile, 1f, ModelAndCollisionMapEditor.StartMode.CollisionMap).Show();
            else if (m_SelectedFile.EndsWith(".spt"))
                new ParticleEditorForm(m_SelectedFile).Show();
            else if (m_SelectedFile.EndsWith(".spa"))
                new ParticleViewerForm(m_SelectedFile).Show();
            else if (m_SelectedFile.EndsWith(".sdat"))
                OpenSDAT();
            /*else if (m_SelectedFile.EndsWith(".lvl"))
                new LevelEditorForm().Show();
            else if (m_SelectedFile.EndsWith(".mesg"))
                new TextEditorForm().Show();*/

            if (m_SelectedFile.EndsWith(".bca") || m_SelectedFile.EndsWith(".btp") || m_SelectedFile.EndsWith(".bma") /*|| m_SelectedFile.EndsWith(".bta")*/)
                m_SavedFile = m_SavedFile != null ? null : m_SelectedFile;
            else
                m_SavedFile = null;

            UpdateOpenFileButton();
        }

        private void UpdateOpenFileButton()
        {
            if (!string.IsNullOrWhiteSpace(m_SavedFile))
            {
                if (m_SelectedFile.EndsWith(".bmd"))
                {
                    btnOpenFile.Text = "Open " + m_SavedFile.Substring(m_SavedFile.Length - 3, 3) + " with model";
                    btnOpenFile.Enabled = true;
                }
                else
                {
                    btnOpenFile.Text = "Cancel opening " + m_SavedFile.Substring(m_SavedFile.Length - 3, 3);
                    btnOpenFile.Enabled = true;
                }
            }
            else if (string.IsNullOrWhiteSpace(m_SelectedFile))
            {
                btnOpenFile.Text = "Open file";
                btnOpenFile.Enabled = false;
            }
            else if (m_SelectedFile.EndsWith(".bmd"))
            {
                btnOpenFile.Text = "Open model";
                btnOpenFile.Enabled = true;
            }
            else if (m_SelectedFile.EndsWith(".kcl"))
            {
                btnOpenFile.Text = "Open collision map";
                btnOpenFile.Enabled = true;
            }
            else if (m_SelectedFile.EndsWith(".spt"))
            {
                btnOpenFile.Text = "Open particle texture";
                btnOpenFile.Enabled = true;
            }
            else if (m_SelectedFile.EndsWith(".spa"))
            {
                btnOpenFile.Text = "Open particle archive";
                btnOpenFile.Enabled = true;
            }
            else if (m_SelectedFile.EndsWith(".btp"))
            {
                btnOpenFile.Text = "Open texture sequence";
                btnOpenFile.Enabled = true;
            }
            else if (m_SelectedFile.EndsWith(".bca"))
            {
                btnOpenFile.Text = "Open model animation";
                btnOpenFile.Enabled = true;
            }
            else if (m_SelectedFile.EndsWith(".bma"))
            {
                btnOpenFile.Text = "Open material animation";
                btnOpenFile.Enabled = true;
            }
            else if (m_SelectedFile.EndsWith(".bta"))
            {
                btnOpenFile.Text = "Open texture animation";
                btnOpenFile.Enabled = false;
            }
            else if (m_SelectedFile.EndsWith(".sdat"))
            {
                btnOpenFile.Text = "Open sound data";
                btnOpenFile.Enabled = true;
            }
            else if (m_SelectedFile.EndsWith(".lvl"))
            {
                btnOpenFile.Text = "Open sound data";
                btnOpenFile.Enabled = false;
            }
            else if (m_SelectedFile.EndsWith(".mesg"))
            {
                btnOpenFile.Text = "Open sound data";
                btnOpenFile.Enabled = false;
            }
            else
            {
                btnOpenFile.Text = "Open file";
                btnOpenFile.Enabled = false;
            }
        }
    }
}