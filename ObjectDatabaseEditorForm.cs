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

            public override string ToString()
            {
                string displayName = !string.IsNullOrWhiteSpace(name) ? name : internalName;
                return objectID.ToString().PadLeft(3) + " - " + displayName;
            }
        }

        private List<ObjectInfo> m_ObjectInfos;

        private bool m_UpdatingTextBoxes = false;
        private bool m_UpdatingListBox = false;

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

                            m_ObjectInfos.Add(info);
                        }
                    }
                }
            }

            lstObjects.Items.Clear();
            for (int i = 0; i < m_ObjectInfos.Count; i++)
                lstObjects.Items.Add(m_ObjectInfos[i]);
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

            ObjectInfo info = (ObjectInfo)lstObjects.SelectedItem;

            m_UpdatingTextBoxes = true;
            nudObjectID.Value = info.objectID;
            nudActorID.Value = info.actorID;
            txtObjectName.Text = info.name;
            txtInternalName.Text = info.internalName;
            txtBankReq.Text = info.bankReq;
            txtDlReq.Text = info.dlReq;
            txtDescription.Text = info.description;

            m_UpdatingTextBoxes = false;
        }

        private void txtObjectName_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || lstObjects.SelectedIndex < 0)
                return;

            ObjectInfo info = (ObjectInfo)lstObjects.SelectedItem;
            info.name = txtObjectName.Text;
            lstObjects.Items[lstObjects.SelectedIndex] = lstObjects.SelectedItem;
        }

        private void nudObjectID_ValueChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || lstObjects.SelectedIndex < 0)
                return;

            ObjectInfo info = (ObjectInfo)lstObjects.SelectedItem;
            info.objectID = (int)nudObjectID.Value;
            lstObjects.Items[lstObjects.SelectedIndex] = lstObjects.SelectedItem;
        }

        private void txtInternalName_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || lstObjects.SelectedIndex < 0)
                return;

            ObjectInfo info = (ObjectInfo)lstObjects.SelectedItem;
            info.internalName = txtInternalName.Text;
            lstObjects.Items[lstObjects.SelectedIndex] = lstObjects.SelectedItem;
        }

        private void nudActorID_ValueChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || lstObjects.SelectedIndex < 0)
                return;

            ObjectInfo info = (ObjectInfo)lstObjects.SelectedItem;
            info.actorID = (int)nudActorID.Value;
        }

        private void txtBankReq_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || lstObjects.SelectedIndex < 0)
                return;

            ObjectInfo info = (ObjectInfo)lstObjects.SelectedItem;
            info.bankReq = txtBankReq.Text;
        }

        private void txtDlReq_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || lstObjects.SelectedIndex < 0)
                return;

            ObjectInfo info = (ObjectInfo)lstObjects.SelectedItem;
            info.dlReq = txtDlReq.Text;
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || lstObjects.SelectedIndex < 0)
                return;

            ObjectInfo info = (ObjectInfo)lstObjects.SelectedItem;
            info.description = txtDescription.Text;
        }
    }
}
