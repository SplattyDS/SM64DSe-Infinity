using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SM64DSe
{
    public partial class PatchViewerForm : Form
    {
        class SeqArcListItem
        {
            public string Name;
            public ushort ID;
            public ushort SeqArcID;
        }

        class CommandInfo
        {
            public enum State
            {
                WAITING,
                SUCCESS,
                FAILED,
                FAILED_FS,
            }

            public CommandInfo(string command)
            {
                this.command = command;
            }

            public override string ToString()
            {
                return "[" + GetStateChar() + "] " + command;
            }

            public SolidBrush GetTextColor()
            {
                switch (state)
                {
                    case State.SUCCESS:
                        return new SolidBrush(Color.Green);
                    case State.FAILED:
                        return new SolidBrush(Color.Red);
                    case State.FAILED_FS:
                        return new SolidBrush(Color.Orange);
                    case State.WAITING:
                    default:
                        return new SolidBrush(Color.Black);
                }
            }

            private string GetStateChar()
            {
                switch (state)
                {
                    case State.SUCCESS:
                        return "V";
                    case State.FAILED:
                    case State.FAILED_FS:
                        return "X";
                    case State.WAITING:
                    default:
                        return "-";
                }
            }

            public string command = "";
            public string description = "";
            public State state = State.WAITING;
        }

        private static readonly string[] fileHeaderStart =
        {
            "#pragma once",
            "",
            "consteval u16 GetFileIdFromName(const char* name, bool ov0 = true)",
            "{",
            "\tstruct FileInfo",
            "\t{",
            "\t\tu16 fileID;",
            "\t\tu16 ov0ID;",
            "\t\tconst char* name;",
            "\t};",
            "\t",
            "\tconstexpr FileInfo files[] =",
            "\t{",
        };

        private static readonly string[] fileHeaderEnd =
        {
            "\t};",
            "\t",
            "\tconst FileInfo* begin = std::begin(files);",
            "\tconst FileInfo* end = std::end(files);",
            "\tconst FileInfo* prevMid = nullptr;",
            "\t",
            "\twhile (true)",
            "\t{",
            "\t\tconst FileInfo* mid = begin + (end - begin >> 1);",
            "\t\tif (void FileNotFound(); mid == prevMid) FileNotFound();",
            "\t\tprevMid = mid;",
            "\t\t",
            "\t\tconst char* c0 = name;",
            "\t\tconst char* c1 = mid->name;",
            "\t\t",
            "\t\tfor(; *c0 == *c1; ++c0, ++c1)",
            "\t\t\tif (*c0 == '\\0') return ov0 ? mid->ov0ID : mid-> fileID;",
            "\t\t",
            "\t\tif (*c0 < *c1) end = mid; else begin = mid;",
            "\t}",
            "}",
            "",
            "consteval u16 operator\"\"_fid(const char* name, std::size_t)",
            "{",
            "\treturn GetFileIdFromName(name, false);",
            "}",
            "",
            "consteval u16 operator\"\"_ov0(const char* name, std::size_t)",
            "{",
            "\treturn GetFileIdFromName(name, true);",
            "}",
        };

        private static readonly string[] soundHeaderStart =
        {
            "#pragma once",
            "",
            "#include \"SM64DS_Common.h\"",
            "#include \"Sound.h\"",
            "",
            "struct SoundIDs",
            "{",
            "\tu16 seqArcID;",
            "\tu16 seqID;",
            "};",
            "",
            "consteval SoundIDs GetSoundInfo(const char* name)",
            "{",
            "\tstruct SoundInfo",
            "\t{",
            "\t\tSoundIDs ids;",
            "\t\tconst char* name;",
            "\t};",
            "\t",
            "\tconstexpr SoundInfo files[] =",
            "\t{",
        };

        private static readonly string[] soundHeaderEnd =
        {
            "\t};",
            "\t",
            "\tconst SoundInfo* begin = std::begin(files);",
            "\tconst SoundInfo* end = std::end(files);",
            "\tconst SoundInfo* prevMid = nullptr;",
            "\t",
            "\twhile (true)",
            "\t{",
            "\t\tconst SoundInfo* mid = begin + (end - begin >> 1);",
            "\t\tif (void SoundNotFound(); mid == prevMid) SoundNotFound();",
            "\t\tprevMid = mid;",
            "\t\t",
            "\t\tconst char* c0 = name;",
            "\t\tconst char* c1 = mid->name;",
            "\t\t",
            "\t\tfor(; *c0 == *c1; ++c0, ++c1)",
            "\t\t\tif (*c0 == '\\0') return mid->ids;",
            "\t\t",
            "\t\tif (*c0 < *c1) end = mid; else begin = mid;",
            "\t}",
            "}",
            "",
            "namespace Sound",
            "{",
            "\t[[gnu::always_inline]]",
            "\tinline void Play(SoundIDs ids, Vector3 camSpacePos)",
            "\t{",
            "\t\tPlay(ids.seqArcID, ids.seqID, camSpacePos);",
            "\t}",
            "\t",
            "\t[[gnu::always_inline]]",
            "\tinline void Play2D(SoundIDs ids)",
            "\t{",
            "\t\tPlay2D(ids.seqArcID, ids.seqID);",
            "\t}",
            "\t",
            "\t[[gnu::always_inline]]",
            "\tinline u32 PlayLong(u32 soundID, SoundIDs ids, Vector3 camSpacePos, u32 arg4)",
            "\t{",
            "\t\treturn PlayLong(soundID, ids.seqArcID, ids.seqID, camSpacePos, arg4);",
            "\t}",
            "}",
            "",
            "consteval SoundIDs operator\"\"_s(const char* name, std::size_t)",
            "{",
            "\treturn GetSoundInfo(name);",
            "}",
        };

        private CommandInfo[] commandInfos;
        private string basePath;
        private string patchPath;
        private bool filesystemEditStarted;
        private bool hideConsoleWindow;

        private string ToHex(ushort num)
        {
            return "0x" + Convert.ToString(num, 16).PadLeft(4, '0').ToLower();
        }

        public PatchViewerForm(string patchPath)
        {
            InitializeComponent();
            LoadPatch(patchPath);
        }

        private void lstCommands_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            CommandInfo item = (CommandInfo)(sender as ListBox).Items[e.Index];

            e.DrawBackground();
            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
            g.DrawString(item.ToString(), e.Font, item.GetTextColor(), new PointF(e.Bounds.X, e.Bounds.Y));

            e.DrawFocusRectangle();
        }

        private void lstCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCommandInfo.Text = lstCommands.SelectedIndex < 0 ? "" : commandInfos[lstCommands.SelectedIndex].description;
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            hideConsoleWindow = false;
            Thread thread = new Thread(ApplyPatch);
            thread.IsBackground = true;
            thread.Start();
        }

        private void PatchViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanCloseWindow())
            {
                System.Media.SystemSounds.Hand.Play();
                e.Cancel = true;
            }
        }

        private void btnReloadScript_Click(object sender, EventArgs e)
        {
            if (!CanCloseWindow())
			{
                System.Media.SystemSounds.Hand.Play();
                return;
            }

            LoadPatch(patchPath);
        }

        private void btnImportScript_Click(object sender, EventArgs e)
        {
            if (!CanCloseWindow())
            {
                System.Media.SystemSounds.Hand.Play();
                return;
            }

            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "SM64DSe Patches(*.fss;*.ccs)|*.fss;*ccs";
            o.RestoreDirectory = true;
            if (o.ShowDialog() != DialogResult.OK)
                return;

            LoadPatch(o.FileName);
        }

        private bool CanCloseWindow()
		{
            IEnumerable<CommandInfo.State> states = commandInfos.Select(c => c.state);
            return !states.Contains(CommandInfo.State.WAITING) || states.Contains(CommandInfo.State.FAILED);
        }

        private void UpdateForm(int refreshIndex)
        {
            pbProgress.Invoke(new MethodInvoker(delegate { pbProgress.Value = commandInfos.Where(c => c.state == CommandInfo.State.SUCCESS || c.state == CommandInfo.State.FAILED_FS).Count(); }));
            lblProgress.Invoke(new MethodInvoker(delegate { lblProgress.Text = $"Progress: {pbProgress.Value * 100 / pbProgress.Maximum}%"; }));
            lstCommands.Invoke(new MethodInvoker(delegate { lstCommands.Items[refreshIndex] = lstCommands.Items[refreshIndex]; }));
            txtCommandInfo.Invoke(new MethodInvoker(delegate { txtCommandInfo.Text = lstCommands.SelectedIndex < 0 ? "" : commandInfos[lstCommands.SelectedIndex].description; }));
            bool enableRetryButton = commandInfos.Select(c => c.state == CommandInfo.State.FAILED).Contains(true);
            btnRetry.GetCurrentParent().Invoke(new MethodInvoker(delegate { btnRetry.Enabled = enableRetryButton; }));
        }

        private void LoadPatch(string patchPath)
		{
            this.patchPath = patchPath;
            basePath = Path.GetDirectoryName(patchPath) + "\\";

            List<string> infosList = File.ReadAllLines(patchPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

            for (int i = infosList.Count - 1; i >= 0; i--)
            {
                if (infosList[i].Contains("#"))
                    infosList[i] = infosList[i].Substring(0, infosList[i].IndexOf('#'));

                if (string.IsNullOrWhiteSpace(infosList[i]))
                    infosList.Remove(infosList[i]);
            }

            commandInfos = infosList.Select(l => new CommandInfo(l)).ToArray();
            hideConsoleWindow = true;

            lstCommands.Items.Clear();
            lstCommands.Items.AddRange(commandInfos);

            pbProgress.Minimum = pbProgress.Value = 0;
            pbProgress.Maximum = commandInfos.Count();
            pbProgress.Step = 1;
            lblProgress.BackColor = Color.Transparent;

            Thread thread = new Thread(ApplyPatch);
            thread.IsBackground = true;
            thread.Start();
        }

        private void ApplyPatch()
        {
            filesystemEditStarted = false;

            TreeView dummyTree = new TreeView();
            ROMFileSelect.LoadFileList(dummyTree);
            TreeNode dummyNode = dummyTree.Nodes[0];

            //Each line.
            for (int i = 0; i < commandInfos.Length; i++)
            {
                CommandInfo info = commandInfos[i];

                if (info.state == CommandInfo.State.SUCCESS)
                    continue;

                //Get parameters.
                string t = info.command;
                string[] p = t.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (p.Length == 0)
                    continue;

                try
                {
                    ProcessCommand(p, info, dummyNode);
                    if (info.state != CommandInfo.State.FAILED_FS)
                        info.state = CommandInfo.State.SUCCESS;
                    UpdateForm(i);
                    hideConsoleWindow = true;
                }
                catch (Exception ex)
                {
                    info.description = ex.Message;
                    info.state = CommandInfo.State.FAILED;
                    UpdateForm(i);
                    System.Media.SystemSounds.Hand.Play();
                    break;
                }
            }

            if (filesystemEditStarted)
                Program.m_ROM.SaveFilesystem(); filesystemEditStarted = false;
        }

        private void ProcessCommand(string[] p, CommandInfo info, TreeNode dummyNode)
        {
            //Switch command.
            switch (p[0].ToLower())
            {
                // restore the symbols from the backup
                case "load_backup_sym":
                    if (filesystemEditStarted)
                        Program.m_ROM.SaveFilesystem(); filesystemEditStarted = false;

                    File.WriteAllBytes(basePath + "symbols.x", File.ReadAllBytes(basePath + "symbols.bak.x"));

                    info.description = basePath + "symbols.x\nhas been reset with symbols from\n" + basePath + "symbols.bak.x";

                    break;

                case "check_duplicate_sym":
                    string duplicateSymbols = Patcher.PatchProcessor.CheckDuplicateSymbols(new DirectoryInfo(basePath));
                    if (duplicateSymbols != null)
                        throw new Exception("Duplicate symbols found:\n" + duplicateSymbols);

                    info.description = "No duplicate symbols found.";

                    break;

                case "update_nocash_sym":
                    Patcher.PatchProcessor.UpdateNoCashSymbols(new DirectoryInfo(basePath));

                    info.description = "Updated no$gba symbols (" + Program.m_ROMPath.Replace(".nds", ".sym") + ").";

                    break;
                    // also add 'Reload Script' and 'Import Script' buttons so you don't have to reopen the window

                case "generate_file_list":
                    if (filesystemEditStarted)
                        Program.m_ROM.SaveFilesystem(); filesystemEditStarted = false;

                    if (p.Length < 2)
                        throw new IndexOutOfRangeException("Not enough arguments supplied.");

                    List<NitroROM.FileEntry> sortedFiles = Program.m_ROM.m_FileEntries.ToList();
                    sortedFiles.Sort((a, b) => string.CompareOrdinal(a.FullName, b.FullName));

                    List<string> lines = new List<string>();
                    lines.Capacity = fileHeaderStart.Length + fileHeaderEnd.Length + Program.m_ROM.m_FileEntries.Length;
                    lines.AddRange(fileHeaderStart);

                    foreach (NitroROM.FileEntry file in sortedFiles) if (file.InternalID != 0xffff)
                            lines.Add("\t\t{ " + ToHex(file.ID) + ", " + ToHex(file.InternalID) + ", " + '"' + file.FullName + '"' + " },");

                    lines.AddRange(fileHeaderEnd);

                    string fileListHeaderLoc = basePath + p[1].Remove(0, 1).Remove(p[1].Length - 2, 1);
                    File.WriteAllLines(fileListHeaderLoc, lines);

                    info.description = "File list generated at:\n" + fileListHeaderLoc;

                    break;

                case "generate_sound_list":
                    if (filesystemEditStarted)
                        Program.m_ROM.SaveFilesystem(); filesystemEditStarted = false;

                    if (p.Length < 2)
                        throw new IndexOutOfRangeException("Not enough arguments supplied.");

                    SM64DSFormats.SDAT soundData = new SM64DSFormats.SDAT(Program.m_ROM.GetFileFromName("data/sound_data.sdat"));
                    List<SeqArcListItem> sequences = new List<SeqArcListItem>();
                    ushort seqArcID = 0;
                    ushort seqID = 0;

                    foreach (SM64DSFormats.SDAT.SequenceArchive seqArc in soundData.m_SeqArcs)
                    {
                        foreach (SM64DSFormats.SDAT.SequenceArchive.Sequence seq in seqArc.m_Sequences)
                        {
                            sequences.Add(new SeqArcListItem { Name = seq.m_Filename, ID = seqID, SeqArcID = seqArcID });
                            seqID++;
                        }

                        seqID = 0;
                        seqArcID++;
                    }

                    sequences.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));

                    lines = new List<string>();
                    lines.Capacity = soundHeaderStart.Length + soundHeaderEnd.Length + sequences.Count;
                    lines.AddRange(soundHeaderStart);

                    foreach (SeqArcListItem sequence in sequences) if (sequence.Name != "SYMB²")
                            lines.Add("\t\t{ " + ToHex(sequence.SeqArcID) + ", " + ToHex(sequence.ID) + ", " + '"' + sequence.Name + '"' + " },");

                    lines.AddRange(soundHeaderEnd);

                    string soundListHeaderLoc = basePath + p[1].Remove(0, 1).Remove(p[1].Length - 2, 1);
                    File.WriteAllLines(soundListHeaderLoc, lines);

                    info.description = "Sound list generated at:\n" + soundListHeaderLoc;

                    break;

                case "generate_actor_list":
                    if (p.Length < 2)
                        throw new IndexOutOfRangeException("Not enough arguments supplied.");

                    string actorListHeaderLoc = basePath + p[1].Remove(0, 1).Remove(p[1].Length - 2, 1);
                    File.WriteAllLines(actorListHeaderLoc, ObjectDatabase.ToCPP());

                    info.description = "Actor list generated at:\n" + actorListHeaderLoc;

                    break;

                case "compile":
                    if (filesystemEditStarted)
                        Program.m_ROM.SaveFilesystem(); filesystemEditStarted = false;

                    info.description = Patcher.PatchProcessor.Process(new DirectoryInfo(basePath), p, hideConsoleWindow);

                    break;

                case "add_or_replace":
                    try
                    {
                        p[1] = p[1].Remove(0, 1).Remove(p[1].Length - 2, 1);
                        p[2] = p[2].Remove(0, 1).Remove(p[2].Length - 2, 1);
                        p[3] = p[3].Remove(0, 1).Remove(p[3].Length - 2, 1);

                        byte[] data = File.ReadAllBytes(basePath + p[3]);

                        if (Program.m_ROM.FileExists(p[1] + p[2]))
                        {
                            if (filesystemEditStarted)
                                Program.m_ROM.SaveFilesystem(); filesystemEditStarted = false;

                            Program.m_ROM.ReinsertFile(Program.m_ROM.GetFileIDFromName(p[1] + p[2]), data);

                            info.description = "Replaced file\n" + p[1] + p[2] + "\nwith data of\n" + basePath + p[3];
                        }
                        else
                        {
                            if (!filesystemEditStarted)
                                Program.m_ROM.StartFilesystemEdit(); filesystemEditStarted = true;

                            Program.m_ROM.AddFile(p[1], p[2], data, dummyNode);

                            info.description = "Added file\n" + p[1] + p[2] + "\nwith data of \n" + basePath + p[3];
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "File couldn't be added / replaced:\n" + ex.Message;
                        info.state = CommandInfo.State.FAILED_FS;
                        break;
                    }

                case "replace":
                    try
                    {
                        p[1] = p[1].Remove(0, 1).Remove(p[1].Length - 2, 1);
                        p[2] = p[2].Remove(0, 1).Remove(p[2].Length - 2, 1);
                        p[3] = p[3].Remove(0, 1).Remove(p[3].Length - 2, 1);

                        byte[] data = File.ReadAllBytes(basePath + p[3]);

                        if (Program.m_ROM.FileExists(p[1] + p[2]))
                        {
                            if (filesystemEditStarted)
                                Program.m_ROM.SaveFilesystem(); filesystemEditStarted = false;

                            Program.m_ROM.ReinsertFile(Program.m_ROM.GetFileIDFromName(p[1] + p[2]), data);

                            info.description = "Replaced file\n" + p[1] + p[2] + "\nwith data of\n" + basePath + p[3];
                        }
                        else
                            throw new Exception("File not found:\n" + p[1] + p[2]);

                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "File couldn't be replaced:\n" + ex.Message;
                        info.state = CommandInfo.State.FAILED_FS;
                        break;
                    }
                    
                case "rename":
                    try
                    {
                        if (!filesystemEditStarted)
                            Program.m_ROM.StartFilesystemEdit(); filesystemEditStarted = true;

                        p[1] = p[1].Remove(0, 1).Remove(p[1].Length - 2, 1);
                        p[2] = p[2].Remove(0, 1).Remove(p[2].Length - 2, 1);

                        Program.m_ROM.RenameFile(p[1], p[2], dummyNode);

                        info.description = "Renamed file\n" + p[1] + "\nto\n" + p[2];
                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "File couldn't be renamed:\n" + ex.Message;
                        info.state = CommandInfo.State.FAILED_FS;
                        break;
                    }
                    
                case "remove":
                    try
                    {
                        if (!filesystemEditStarted)
                            Program.m_ROM.StartFilesystemEdit(); filesystemEditStarted = true;

                        p[1] = p[1].Remove(0, 1).Remove(p[1].Length - 2, 1);

                        Program.m_ROM.RemoveFile(p[1], dummyNode);

                        info.description = "Removed file\n" + p[1];
                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "File couldn't be removed:\n" + ex.Message;
                        info.state = CommandInfo.State.FAILED_FS;
                        break;
                    }

                case "add_dir":
                    try
                    {
                        if (!filesystemEditStarted)
                            Program.m_ROM.StartFilesystemEdit(); filesystemEditStarted = true;

                        p[1] = p[1].Remove(0, 1).Remove(p[1].Length - 2, 1);
                        p[2] = p[2].Remove(0, 1).Remove(p[2].Length - 2, 1);

                        if (Program.m_ROM.GetDirIDFromName(p[1] + p[2]) != 0)
                            throw new Exception("Directory\n" + p[1] + p[2] + "\nalready exists.");

                        Program.m_ROM.AddDir(p[1], p[2], dummyNode);

                        info.description = "Added directory\n" + p[2] + "\nto\n" + p[1];
                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "Directory couldn't be added:\n" + ex.Message;
                        info.state = CommandInfo.State.FAILED_FS;
                        break;
                    }

                case "rename_dir":
                    try
                    {
                        if (!filesystemEditStarted)
                            Program.m_ROM.StartFilesystemEdit(); filesystemEditStarted = true;

                        p[1] = p[1].Remove(0, 1).Remove(p[1].Length - 2, 1);
                        p[2] = p[2].Remove(0, 1).Remove(p[2].Length - 2, 1);

                        Program.m_ROM.RenameDir(p[1], p[2], dummyNode);

                        info.description = "Renamed directory\n" + p[1] + "\nto\n" + p[2];
                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "Directory couldn't be renamed:\n" + ex.Message;
                        info.state = CommandInfo.State.FAILED_FS;
                        break;
                    }

                case "remove_dir":
                    try
                    {
                        if (!filesystemEditStarted)
                            Program.m_ROM.StartFilesystemEdit(); filesystemEditStarted = true;

                        p[1] = p[1].Remove(0, 1).Remove(p[1].Length - 2, 1);

                        Program.m_ROM.RemoveDir(p[1], dummyNode);

                        info.description = "Removed directory\n" + p[1];
                        break;
                    }
                    catch (Exception ex)
                    {
                        info.description = "Directory couldn't be removed:\n" + ex.Message;
                        info.state = CommandInfo.State.FAILED_FS;
                        break;
                    }
            }
        }
	}
}
