using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using SM64DSe.SM64DSFormats;

namespace SM64DSe
{
    public partial class SPLC_Form : Form
    {
        enum Columns
        {
            TEXTURE = 0,
            WATER,
            VIEW_ID,
            BEHAV_1,
            CAM_BEHAV,
            BEHAV_2,
            CAM_THROUGH,
            TOXIC,
            UNK_26,
            PAD_1,
            WIND_ID,
            PAD_2
        }
        struct ColStruct
        {
            public readonly string m_Name;
            public readonly int m_Shift;
            public readonly ulong m_And;
            public readonly string[] m_TypeNames;
            public ColStruct(string name, int shift, ulong and, string[] typeNames)
            {
                m_Name = name;
                m_Shift = shift;
                m_And = and;
                m_TypeNames = typeNames;
            }

            public ulong GetFlag(ulong flags) { return flags >> m_Shift & m_And; }
            public void SetFlag(ref ulong flags, ulong val)
                { flags = flags & ~(m_And << m_Shift) | (val & m_And) << m_Shift; }
        }
        ColStruct[] columns = new ColStruct[12]
        {
            new ColStruct ("Texture"     ,  0, 0x1fuL, new string[]{"Basic/Rocky ",
                                                                    "Path-Textured ",
                                                                    "Grassy ",
                                                                    "Puddle ",
                                                                    "Rocky ",
                                                                    "Wooden ",
                                                                    "Snowy ",
                                                                    "Icy ",
                                                                    "Sandy ",
                                                                    "Flowery ",
                                                                    "Rock (Merry Go-Round Music) ",
                                                                    "Wood (Merry Go-Round Music) ",
                                                                    "Grate-Meshed ",
                                                                    "Weirdly Textured (No Sound) ",
                                                                    "Weirdly Textured (No Sound) ",
                                                                    "Weirdly Textured (No Sound) ",
                                                                    "Echoed Basic/Rock ",
                                                                    "Echoed Path ",
                                                                    "Echoed Grass ",
                                                                    "Echoed Puddle ",
                                                                    "Echoed Rock ",
                                                                    "Echoed Wood ",
                                                                    "Echoed Snow ",
                                                                    "Echoed Ice ",
                                                                    "Echoed Sand ",
                                                                    "Echoed Flowers ",
                                                                    "Echoed Rock 2 ",
                                                                    "Echoed Wood 2 ",
                                                                    "Echoed Grate Mesh ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",
                                                                    "Weirdly Textured ",}),
            new ColStruct ("Water"       ,  5, 0x01uL, new string[]{"",
                                                                    "Water-Defining "}),
            new ColStruct ("View ID"     ,  6, 0x3fuL, new string[]{}),
            new ColStruct ("Traction"    , 12, 0x07uL, new string[]{"Slippery, Crawlable ",
                                                                    "Unslippable ",
                                                                    "Unslippable, No Steep ",
                                                                    "Slippery-Sloped ",
                                                                    "Slippery ",
                                                                    "Slide ",
                                                                    "Slippery, Non-Crawlable ",
                                                                    "Slippery, Non-Crawlable? "}),
            new ColStruct ("Camera Type" , 15, 0x0fuL, new string[]{"Normal (Re-Adjusts Rotation) ",
                                                                    "Go-Behind ",
                                                                    "Zoom-Out-And-Go-Behind ",
                                                                    "Go-Behind ",
                                                                    "Go-Behind ",
                                                                    "Weird ",
                                                                    "Normal ",
                                                                    "Go-Behind For Bosses ",
                                                                    "Go-Behind ",
                                                                    "8-Directional ",
                                                                    "Non-Rotating ",
                                                                    "Close-Up-And-Personal ",
                                                                    "Go-Behind ",
                                                                    "Go-Behind ",
                                                                    "Go-Behind ",
                                                                    "Go-Behind "}),
            new ColStruct ("Behavior"    , 19, 0x1fuL, new string[]{"Surface ",
                                                                    "Lava ",
                                                                    "Weird Surface ",
                                                                    "Hanging Mesh ",
                                                                    "Death Plane, Lose Life ",
                                                                    "Death Plane, No Lose Life ",
                                                                    "Jump-Limiting Surface ",
                                                                    "Slow Half-Quicksand ",
                                                                    "Slow Bottomless Quicksand ",
                                                                    "Instant Quicksand ",
                                                                    "Weird Surface ",
                                                                    "Wind Animation/Sound ",
                                                                    "Weird Surface ",
                                                                    "Enter ? Switch ",
                                                                    "No Stuck In Floor ",
                                                                    "Start Line ",
                                                                    "Finish Line (Star ID 2) ",
                                                                    "Vanish-Luigi-Transparent Surface ",
                                                                    "Get-Off-Of-Me Surface (Endless Stairs) ",
                                                                    "Gust Plane (Up) ",
                                                                    "Crawl Transparent ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface ",
                                                                    "Weird Surface "}),
            new ColStruct ("Camera Through"        , 24, 0x01uL, new string[]{"",
                                                                    "Go-Through " }),
            new ColStruct ("Toxic"       , 25, 0x01uL, new string[]{"",
                                                                    "Toxic "}),
            new ColStruct ("Unk << 26"   , 26, 0x01uL, new string[]{}),
            new ColStruct ("Pad << 27"   , 27, 0x1fuL, new string[]{}),
            new ColStruct ("Wind Path ID", 32, 0xffuL, new string[]{}),
            new ColStruct ("Pad << 40"   , 40, 0xffffffuL, new string[]{}),
        };

        SPLC m_SPLC;

        public SPLC_Form(SPLC SPLC)
        {
            InitializeComponent();

            this.m_SPLC = SPLC;

            for (int i = 0; i < 52; i++)
                cbxLevels.Items.Add(i + " - " + Strings.LevelNames()[i]);

            LoadSPLCData();
        }
        
        private void LoadSPLCData()
        {
            txtNumEntries.Text = "" + m_SPLC.Count;

            gridSPLCData.RowCount = m_SPLC.Count;

            if (gridSPLCData.ColumnCount != columns.Length + 1)
            {
                gridSPLCData.ColumnCount = columns.Length;
                
                // Set column widths
                gridSPLCData.RowHeadersWidth = 54;
                for (int i = 0; i < columns.Length; i++)
                {
                    gridSPLCData.Columns[i].Width = 54;
                    gridSPLCData.Columns[i].HeaderText = columns[i].m_Name;
                }

                DataGridViewTextBoxColumn cmb = new DataGridViewTextBoxColumn();
                cmb.HeaderText = "Type/Description";
                cmb.ReadOnly = true;
                cmb.Width = 500;
                gridSPLCData.Columns.Add(cmb);
            }

            for (int i = 0; i < m_SPLC.Count; i++)
            {
                gridSPLCData.Rows[i].HeaderCell.Value = "" + i;
                for (int j = 0; j < columns.Length; j++)
                {
                    gridSPLCData.Rows[i].Cells[j].Value = columns[j].GetFlag(m_SPLC[i].flags);
                }

                // Fill in Type/Description column
                gridSPLCData.Rows[i].Cells[columns.Length].Value =
                    columns[(int)Columns.WATER].m_TypeNames[m_SPLC[i].m_Water] +
                    columns[(int)Columns.TOXIC].m_TypeNames[m_SPLC[i].m_Toxic] +
                    (m_SPLC[i].m_WindID != 0xff ? "Windy " : "") +
                    columns[(int)Columns.TEXTURE].m_TypeNames[m_SPLC[i].m_Texture] +
                    columns[(int)Columns.BEHAV_1].m_TypeNames[m_SPLC[i].m_Traction] +
                    columns[(int)Columns.BEHAV_2].m_TypeNames[m_SPLC[i].m_Behav] +
                    "w/ " +
                    columns[(int)Columns.CAM_THROUGH].m_TypeNames[m_SPLC[i].m_CamThrough] +
                    columns[(int)Columns.CAM_BEHAV].m_TypeNames[m_SPLC[i].m_CamBehav] +
                    "Camera" +
                    (m_SPLC[i].m_ViewID != 0x3f ? ", View ID " + m_SPLC[i].m_ViewID.ToString() : "") +
                    (m_SPLC[i].m_WindID != 0xff ? ", Wind Path ID " + m_SPLC[i].m_WindID.ToString() : "");
            }

        }

        void gridSPLCData_CellEndEdit(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex != columns.Length)
                {
                    ulong splc = m_SPLC[e.RowIndex].flags;

                    columns[e.ColumnIndex].SetFlag(ref splc,
                        ulong.Parse(gridSPLCData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()));

                    SPLC.Entry temp = new SPLC.Entry();
                    temp.flags = splc;
                    m_SPLC[e.RowIndex] = temp;
                }
                else
                {
                    return;
                }

                LoadSPLCData();
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox(ex).ShowDialog();
                return;
            }
        }

        private void CopySPLC(int sourceLevel)
        {
            NitroOverlay otherOVL = new NitroOverlay(Program.m_ROM, Program.m_ROM.GetLevelOverlayID(sourceLevel));

            uint other_splc_addr = otherOVL.ReadPointer(0x60);
            ushort other_splc_num = otherOVL.Read16(other_splc_addr + 0x06);
            uint other_splc_size = (uint)(8 + (other_splc_num * 8));

            m_SPLC = new SPLC();
            for(int i = 0; i < other_splc_num; ++i)
            {
                ulong flags   = otherOVL.Read32((uint)(other_splc_addr + 8 + 8 * i + 0));
                flags |= (ulong)otherOVL.Read32((uint)(other_splc_addr + 8 + 8 * i + 4)) << 32;
                SPLC.Entry splc = new SPLC.Entry();
                splc.flags = flags;
                m_SPLC.Add(splc);
            }

            LoadSPLCData();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (cbxLevels.SelectedIndex != -1)
                CopySPLC(cbxLevels.SelectedIndex);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SPLC.Entry splc = new SPLC.Entry();
            splc.flags = 0x000000ff00000fc0;

            if (gridSPLCData.SelectedRows.Count == 0)
                m_SPLC.Add(splc);
            else
                m_SPLC.m_Entries.Insert(gridSPLCData.SelectedRows[0].Index, splc);
            
            // Reload data
            LoadSPLCData();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (gridSPLCData.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to delete.");
            else
            {
                m_SPLC.m_Entries.RemoveAt(gridSPLCData.SelectedRows[0].Index);
                // Reload data
                LoadSPLCData();
            }
        }

        private void btnShiftUp_Click(object sender, EventArgs e)
        {
            if (gridSPLCData.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to move.");
            else if (gridSPLCData.SelectedRows[0].Index != 0)
            {
                SPLC.Entry temp = m_SPLC[gridSPLCData.SelectedRows[0].Index];
                m_SPLC[gridSPLCData.SelectedRows[0].Index] = m_SPLC[gridSPLCData.SelectedRows[0].Index - 1];
                m_SPLC[gridSPLCData.SelectedRows[0].Index - 1] = temp;
            }
            LoadSPLCData();
        }

        private void btnShiftDown_Click(object sender, EventArgs e)
        {
            if (gridSPLCData.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row to move.");
            else if (gridSPLCData.SelectedRows[0].Index != gridSPLCData.Rows.Count - 1)
            {
                SPLC.Entry temp = m_SPLC[gridSPLCData.SelectedRows[0].Index];
                m_SPLC[gridSPLCData.SelectedRows[0].Index] = m_SPLC[gridSPLCData.SelectedRows[0].Index + 1];
                m_SPLC[gridSPLCData.SelectedRows[0].Index + 1] = temp;
            }
            LoadSPLCData();
        }
    }
}
