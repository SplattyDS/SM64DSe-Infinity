using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SM64DSe.SM64DSFormats
{
    public partial class DL_Editor : Form
    {
        LevelEditorForm lf;

        public DL_Editor(LevelEditorForm lf)
        {
            InitializeComponent();
            
            availableTree.Nodes.Clear();
            ROMFileSelect.LoadFileList(availableTree);
            this.lf = lf;

            for (int i = 0; i < lf.m_Level.m_DynLibIDs.Count; i++)
                currentTree.Nodes.Add(Program.m_ROM.GetFileNameFromID(Program.m_ROM.GetFileIDFromInternalID(lf.m_Level.m_DynLibIDs[i])));
        }

        private void btnRemove_ButtonClick(object sender, EventArgs e)
        {
            if (currentTree.SelectedNode != null)
                currentTree.SelectedNode.Remove();
        }

        private void btnAdd_ButtonClick(object sender, EventArgs e)
        {
            if (availableTree.SelectedNode != null)
                currentTree.Nodes.Add(Program.m_ROM.GetFileNameFromID(Program.m_ROM.GetFileIDFromName(availableTree.SelectedNode.Tag.ToString())));
        }

        private void btnUpdate_ButtonClick(object sender, EventArgs e)
        {
            List<ushort> dylbs = lf.m_Level.m_DynLibIDs;

            dylbs.Clear();

            foreach (TreeNode n in currentTree.Nodes)
                dylbs.Add(Program.m_ROM.GetFileEntries()[Program.m_ROM.GetFileIDFromName(n.Text)].InternalID);

            Close();
        }
    }
}
