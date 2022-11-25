using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using SM64DSe.SM64DSFormats;
using SM64DSe.ImportExport.Writers.InternalWriters;
using SM64DSe.ImportExport;
using SM64DSe.ImportExport.Loaders.InternalLoaders;
using System.Xml;
using System.IO;

namespace SM64DSe
{
    public partial class ObjectDatabaseEdtiorForm : Form
    {
        class ObjectInfo
        {
            public int objectID;
            public string name;
            public string internalName;
            public int actorID;
            public string description;
            public string bankReq;
            public string dlReq;
            public string renderer; // + params

            public override string ToString()
            {
                string displayName = !string.IsNullOrWhiteSpace(name) ? name : internalName;
                return objectID.ToString().PadLeft(3) + " - " + displayName;
            }
        }

        private List<ObjectInfo> m_ObjectInfos;

        private bool m_UpdatingTextBoxes = false;
        private bool m_UpdatingListBox = false;

        private ObjectInfo m_LastSelectedObjectInfo = null;

        private void LoadObjectInfos(string path = null)
        {
            m_ObjectInfos = new List<ObjectInfo>();

            if (string.IsNullOrWhiteSpace(path))
                path = Path.Combine(Application.StartupPath, "objectdb.xml");

            using (XmlReader reader = XmlReader.Create(path))
            {
                reader.MoveToContent();
                // reader.ReadStartElement("database");
                int i = 0;
                while (reader.Read())
                {
                    if (reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        if (reader.LocalName == "object")
                        {
                            ObjectInfo info = new ObjectInfo();

                            info.objectID = Convert.ToInt32(reader.GetAttribute("id"));

                            // reader.ReadToDescendant("category");
                            // info.category = reader.ReadElementContentAsInt();

                            reader.ReadToDescendant("name");
                            info.name = reader.ReadElementContentAsString();

                            reader.ReadToNextSibling("internalname");
                            info.internalName = reader.ReadElementContentAsString();

                            reader.ReadToNextSibling("actorid");
                            info.actorID = reader.ReadElementContentAsInt();

                            reader.ReadToNextSibling("description");
                            info.description = reader.ReadElementContentAsString();

                            reader.ReadToNextSibling("bankreq");
                            info.bankReq = reader.ReadElementContentAsString();

                            reader.ReadToNextSibling("dlreq");
                            info.dlReq = reader.ReadElementContentAsString();

                            reader.ReadToNextSibling("renderer");
                            string type = reader.GetAttribute("type");
                            string renderer = type + "";
                            
                            switch (type)
                            {
                                case "NormalBMD":
                                case "NormalKCL":
                                    renderer += ' ' + reader.GetAttribute("file") + ' ' + reader.GetAttribute("scale");
                                    break;
                                case "DoubleBMD":
                                    renderer += ' ' + reader.GetAttribute("file1") + ' ' + reader.GetAttribute("file2") + ' ' + reader.GetAttribute("scale");
                                    string offset1 = reader.GetAttribute("offset1");
                                    if (!string.IsNullOrEmpty(offset1))
                                        renderer += ' ' + offset1 + ' ' + reader.GetAttribute("offset2");
                                    break;
                                case "Kurumajiku":
                                    renderer += ' ' + reader.GetAttribute("file1") + ' ' + reader.GetAttribute("file2") + ' ' + reader.GetAttribute("scale");
                                    break;
                                case "Pole":
                                case "ColorCube":
                                    renderer += ' ' + reader.GetAttribute("border") + ' ' + reader.GetAttribute("fill");
                                    break;
                                case "Player":
                                    renderer += ' ' + reader.GetAttribute("scale") + ' ' + reader.GetAttribute("animation");
                                    break;
                                case "Luigi":
                                    renderer += ' ' + reader.GetAttribute("scale");
                                    break;
                                case "ChainedChomp":
                                case "Goomboss":
                                case "Tree":
                                case "Painting":
                                case "UnchainedChomp":
                                case "Fish":
                                case "Butterfly":
                                case "Star":
                                case "BowserSkyPlatform":
                                case "BigSnowman":
                                case "Toxbox":
                                case "Pokey":
                                case "FlPuzzle":
                                case "FlameThrower":
                                case "C1Trap":
                                case "Wiggler":
                                case "Koopa":
                                case "KoopaShell":
                                    // no params
                                    break;
                                default:
                                    MessageBox.Show("Unknown renderer for '" + info.name + "' (id = " + info.objectID + ").");
                                    break;
                            }

                            info.renderer = renderer;

                            m_ObjectInfos.Add(info);
                        }
                    }
                }
            }

            FillListBox();
        }

        private void SaveObjectInfos()
        {
            // save the XML
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(Path.Combine(Application.StartupPath, "objectdb.xml"), settings))
            {
                writer.WriteStartDocument();
                writer.WriteComment("SM64DS Editor Infinity");
                writer.WriteStartElement("database");

                foreach (ObjectInfo info in m_ObjectInfos)
                {
                    writer.WriteStartElement("object");
                    writer.WriteAttributeString("id", info.objectID.ToString());

                    // writer.WriteElementString("category", info.category.ToString());
                    writer.WriteElementString("name", info.name);
                    writer.WriteElementString("internalname", info.internalName);
                    writer.WriteElementString("actorid", info.actorID.ToString());
                    writer.WriteElementString("description", info.description);
                    writer.WriteElementString("bankreq", info.bankReq);
                    writer.WriteElementString("dlreq", info.dlReq);

                    string[] renderer = info.renderer.Split(' ');
                    writer.WriteStartElement("renderer");
                    writer.WriteAttributeString("type", renderer[0]);
                    switch (renderer[0])
                    {
                        case "NormalBMD":
                        case "NormalKCL":
                            writer.WriteAttributeString("file", renderer[1]);
                            writer.WriteAttributeString("scale", renderer[2]);
                            break;
                        case "DoubleBMD":
                            writer.WriteAttributeString("file1", renderer[1]);
                            writer.WriteAttributeString("file2", renderer[2]);
                            writer.WriteAttributeString("scale", renderer[3]);

                            if (renderer.Length > 4)
                            {
                                writer.WriteAttributeString("offset1", renderer[4]);
                                writer.WriteAttributeString("offset2", renderer[5]);
                            }

                            break;
                        case "Kurumajiku":
                            writer.WriteAttributeString("file1", renderer[1]);
                            writer.WriteAttributeString("file2", renderer[2]);
                            writer.WriteAttributeString("scale", renderer[3]);
                            break;
                        case "Pole":
                        case "ColorCube":
                            writer.WriteAttributeString("border", renderer[1]);
                            writer.WriteAttributeString("fill", renderer[2]);
                            break;
                        case "Player":
                            writer.WriteAttributeString("scale", renderer[1]);
                            writer.WriteAttributeString("animation", renderer[2]);
                            break;
                        case "Luigi":
                            writer.WriteAttributeString("scale", renderer[1]);
                            break;
                        case "ChainedChomp":
                        case "Goomboss":
                        case "Tree":
                        case "Painting":
                        case "UnchainedChomp":
                        case "Fish":
                        case "Butterfly":
                        case "Star":
                        case "BowserSkyPlatform":
                        case "BigSnowman":
                        case "Toxbox":
                        case "Pokey":
                        case "FlPuzzle":
                        case "FlameThrower":
                        case "C1Trap":
                        case "Wiggler":
                        case "Koopa":
                        case "KoopaShell":
                            // no params
                            break;
                        default:
                            MessageBox.Show("Unknown renderer for '" + info.name + "' (id = " + info.objectID + ").");
                            break;
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public ObjectDatabaseEdtiorForm()
        {
            InitializeComponent();

            try
            {
                LoadObjectInfos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong:\n{ex}", "Invalid XML", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
            }
        }

        private void ObjectDatabaseEdtiorForm_Load(object sender, System.EventArgs e)
        {
            lstObjects.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveObjectInfos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong:\n{ex}", "Saving failed", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnImportXML_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a XML to import";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.Cancel) return;

            try
            {
                LoadObjectInfos(ofd.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong:\n{ex}", "Invalid XML", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
            }
        }

        private void lstObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_UpdatingListBox || lstObjects.SelectedIndex < 0)
                return;

            m_LastSelectedObjectInfo = (ObjectInfo)lstObjects.SelectedItem;

            ObjectInfo info = m_LastSelectedObjectInfo;

            m_UpdatingTextBoxes = true;

            nudObjectID.Value = info.objectID;
            nudActorID.Value = info.actorID;
            txtObjectName.Text = info.name;
            txtInternalName.Text = info.internalName;
            txtBankReq.Text = info.bankReq;
            txtDlReq.Text = info.dlReq;
            txtDescription.Text = info.description;
            txtRenderer.Text = info.renderer;

            m_UpdatingTextBoxes = false;
        }

        private void txtObjectName_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.name = txtObjectName.Text;
            RefreshListBoxLabels();
        }

        private void nudObjectID_ValueChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.objectID = (int)nudObjectID.Value;
            SortObjectsByObjectID();
            FillListBox(txtSearch.Text);
        }

        private void txtInternalName_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.internalName = txtInternalName.Text;
            RefreshListBoxLabels();
        }

        private void nudActorID_ValueChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.actorID = (int)nudActorID.Value;
        }

        private void txtBankReq_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.bankReq = txtBankReq.Text;
        }

        private void txtDlReq_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.dlReq = txtDlReq.Text;
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.description = txtDescription.Text;
        }

        private void txtRenderer_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.renderer = txtRenderer.Text;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            FillListBox(txtSearch.Text);
        }

        private void FillListBox(string search = null)
        {
            lstObjects.Items.Clear();
            for (int i = 0; i < m_ObjectInfos.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(search) || m_ObjectInfos[i].ToString().ToLower().Contains(search.ToLower()))
                    lstObjects.Items.Add(m_ObjectInfos[i]);
            }
        }

        private void RefreshListBoxLabels()
        {
            for (int i = 0; i < lstObjects.Items.Count; i++)
                lstObjects.Items[i] = lstObjects.Items[i];
        }

        private void SortObjectsByObjectID()
        {
            m_ObjectInfos = m_ObjectInfos.OrderBy(o => o.objectID).ToList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SortObjectsByObjectID();
            int newID = m_ObjectInfos.Last().objectID + 1;

            ObjectInfo newObj = new ObjectInfo { objectID = newID, actorID = newID, name = txtSearch.Text, internalName = txtSearch.Text.ToUpper().Replace(' ', '_').Replace("(", "").Replace(")", "").Replace("-", "").Replace("__", "_"), description = "", bankReq = "none", dlReq = "none" };
            m_ObjectInfos.Add(newObj);

            FillListBox(txtSearch.Text);

            lstObjects.SelectedIndex = lstObjects.Items.IndexOf(newObj);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (m_LastSelectedObjectInfo == null)
                return;

            m_ObjectInfos.Remove(m_LastSelectedObjectInfo);
            m_LastSelectedObjectInfo = null;

            SortObjectsByObjectID();
            FillListBox(txtSearch.Text);

            txtBankReq.Text = txtDescription.Text = txtDlReq.Text = txtInternalName.Text = txtObjectName.Text = "";
            nudActorID.Value = nudObjectID.Value = 0;
        }
    }
}
