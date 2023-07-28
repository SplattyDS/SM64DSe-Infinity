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
using System.Numerics;
using System.Globalization;

namespace SM64DSe
{
    public partial class ObjectDatabaseEdtiorForm : Form
    {
        public class ObjectInfo
        {
            public class Renderer
			{
                public string type;

                public Renderer(string type = null)
                {
                    this.type = type;
                }
            }

            public class NormalBMD_Renderer : Renderer
            {
                public string fileName = "data/normal_obj/b_coin_switch/b_coin_switch.bmd";
                public float scale = 0.008f;

                public NormalBMD_Renderer() { this.type = "NormalBMD"; }

                public NormalBMD_Renderer(string fileName, float scale)
				{
                    this.type = "NormalBMD";
                    this.fileName = fileName;
                    this.scale = scale;
				}
            }

            public class NormalKCL_Renderer : Renderer
            {
                public string fileName = "data/normal_obj/b_coin_switch/b_coin_switch.kcl";
                public float scale = 1.0f;

                public NormalKCL_Renderer() { this.type = "NormalKCL"; }

                public NormalKCL_Renderer(string fileName, float scale)
                {
                    this.type = "NormalKCL";
                    this.fileName = fileName;
                    this.scale = scale;
                }
            }

            public class DoubleBMD_Renderer : Renderer
            {
                public string fileName1 = "data/enemy/killer/killer_body.bmd";
                public string fileName2 = "data/enemy/killer/killer_face.bmd";
                public float scale = 0.008f;
                public Vector3 offset1 = new Vector3(0);
                public Vector3 offset2 = new Vector3(0);

                public DoubleBMD_Renderer() { this.type = "DoubleBMD"; }

                public DoubleBMD_Renderer(string fileName1, string fileName2, float scale, Vector3 offset1, Vector3 offset2)
                {
                    this.type = "DoubleBMD";
                    this.fileName1 = fileName1;
                    this.fileName2 = fileName2;
                    this.scale = scale;
                    this.offset1 = offset1;
                    this.offset2 = offset2;
                }
            }

            public class Kurumajiku_Renderer : Renderer
            {
                public string fileName1 = "data/special_obj/km1_kuruma/km1_kuruma.bmd";
                public string fileName2 = "data/special_obj/km1_kuruma/km1_kurumajiku.bmd";
                public float scale = 1f;

                public Kurumajiku_Renderer() { this.type = "Kurumajiku"; }

                public Kurumajiku_Renderer(string fileName1, string fileName2, float scale)
                {
                    this.type = "Kurumajiku";
                    this.fileName1 = fileName1;
                    this.fileName2 = fileName2;
                    this.scale = scale;
                }
            }

            public class Pole_Renderer : Renderer
            {
                public Color border = Color.FromArgb(0x00, 0x00, 0xff);
                public Color fill = Color.FromArgb(0x00, 0x00, 0x40);

                public Pole_Renderer() { this.type = "Pole"; }

                public Pole_Renderer(Color border, Color fill)
                {
                    this.type = "Pole";
                    this.border = border;
                    this.fill = fill;
                }
            }

            public class ColorCube_Renderer : Renderer
            {
                public Color border = Color.FromArgb(0x00, 0x00, 0xff);
                public Color fill = Color.FromArgb(0x00, 0x00, 0x40);

                public ColorCube_Renderer() { this.type = "ColorCube"; }

                public ColorCube_Renderer(Color border, Color fill)
                {
                    this.type = "ColorCube";
                    this.border = border;
                    this.fill = fill;
                }
            }

            public class Player_Renderer : Renderer
            {
                public float scale = 0.008f;
                public string animation = "wait.bca";

                public Player_Renderer() { this.type = "Player"; }

                public Player_Renderer(float scale, string animation)
                {
                    this.type = "Player";
                    this.scale = scale;
                    this.animation = animation;
                }
            }

            public class Luigi_Renderer : Renderer
            {
                public float scale = 0.008f;

                public Luigi_Renderer() { this.type = "Luigi"; }

                public Luigi_Renderer(float scale)
                {
                    this.type = "Luigi";
                    this.scale = scale;
                }
            }

            public int objectID = -2;
            public string name = "";
            public string internalName = "";
            public int actorID = -2;
            public string description = "";
            public string bankReq = "";
            public string dlReq = "";
            public Renderer renderer = null; // + params

            public static bool displayObjectID = true;
            public static bool displayInternalName = false;

            public override string ToString()
            {
                string displayName = !string.IsNullOrWhiteSpace(name) && !displayInternalName ? name : internalName;
                return (displayObjectID ? objectID.ToString().PadLeft(3) : actorID.ToString().PadLeft(3)) + " - " + displayName;
            }
        }

        private List<ObjectInfo> m_ObjectInfos;

        private bool m_UpdatingTextBoxes = false;
        private bool m_UpdatingListBox = false;

        private ObjectInfo m_LastSelectedObjectInfo = null;

        private string m_FileFilter1 = "";
        private string m_FileFilter2 = "";
        private string m_StartFolder1 = "";
        private string m_StartFolder2 = "";
        private bool m_OnlyPlayerAnimFiles = false;

        private float FloatFromString(string s)
		{
            return float.Parse(s, CultureInfo.InvariantCulture);
		}

        private Vector3 VecFromString(string s)
        {
            string[] vals = s.Split(',');

            float x = float.Parse(vals[0], CultureInfo.InvariantCulture);
            float y = float.Parse(vals[1], CultureInfo.InvariantCulture);
            float z = float.Parse(vals[2], CultureInfo.InvariantCulture);

            return new Vector3(x, y, z);
        }

        private Color ColorFromString(string s)
        {
            uint rgb = uint.Parse(s, NumberStyles.HexNumber) | 0xff000000;
            return Color.FromArgb((int)rgb);
        }

        private string FloatToString(float f)
        {
            return f.ToString(CultureInfo.InvariantCulture);
        }

        private string VecToString(Vector3 v)
        {
            return FloatToString(v.X) + "," + FloatToString(v.Y) + "," + FloatToString(v.Z);
        }

        private string ColorToString(Color c)
        {
            return (c.ToArgb() & 0xffffff).ToString("X6");
        }

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

                            ObjectInfo.Renderer renderer;

                            switch (type)
                            {
                                case "NormalBMD":
                                    string file = reader.GetAttribute("file");
                                    float scale = FloatFromString(reader.GetAttribute("scale"));

                                    renderer = new ObjectInfo.NormalBMD_Renderer(file, scale);
                                    break;

                                case "NormalKCL":
                                    file = reader.GetAttribute("file");
                                    scale = FloatFromString(reader.GetAttribute("scale"));

                                    renderer = new ObjectInfo.NormalKCL_Renderer(file, scale);
                                    break;

                                case "DoubleBMD":
                                    string file1 = reader.GetAttribute("file1");
                                    string file2 = reader.GetAttribute("file2");
                                    scale = FloatFromString(reader.GetAttribute("scale"));
                                    Vector3 offset1 = VecFromString(reader.GetAttribute("offset1"));
                                    Vector3 offset2 = VecFromString(reader.GetAttribute("offset2"));

                                    renderer = new ObjectInfo.DoubleBMD_Renderer(file1, file2, scale, offset1, offset2);
                                    break;

                                case "Kurumajiku":
                                    file1 = reader.GetAttribute("file1");
                                    file2 = reader.GetAttribute("file2");
                                    scale = FloatFromString(reader.GetAttribute("scale"));

                                    renderer = new ObjectInfo.Kurumajiku_Renderer(file1, file2, scale);
                                    break;

                                case "Pole":
                                    Color border = ColorFromString(reader.GetAttribute("border"));
                                    Color fill = ColorFromString(reader.GetAttribute("fill"));

                                    renderer = new ObjectInfo.Pole_Renderer(border, fill);
                                    break;

                                case "ColorCube":
                                    border = ColorFromString(reader.GetAttribute("border"));
                                    fill = ColorFromString(reader.GetAttribute("fill"));

                                    renderer = new ObjectInfo.ColorCube_Renderer(border, fill);
                                    break;

                                case "Player":
                                    scale = FloatFromString(reader.GetAttribute("scale"));
                                    string animation = reader.GetAttribute("animation");

                                    renderer = new ObjectInfo.Player_Renderer(scale, animation);
                                    break;

                                case "Luigi":
                                    scale = FloatFromString(reader.GetAttribute("scale"));
                                    renderer = new ObjectInfo.Luigi_Renderer(scale);
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
                                    renderer = new ObjectInfo.Renderer(type);
                                    break;

                                default:
                                    throw new Exception("Unknown renderer for '" + info.name + "' (id = " + info.objectID + ").");
                            }

                            info.renderer = renderer;

                            m_ObjectInfos.Add(info);
                        }
                    }
                }
            }

            FillListBox();
        }


        private void ModifyObjectInfos(string path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = Path.Combine(Application.StartupPath, "objectdb.xml");

            using (XmlReader reader = XmlReader.Create(path))
            {
                reader.MoveToContent();
                // reader.ReadStartElement("database");
                int i = 0;

                ObjectInfo info = null;
                ObjectInfo tempInfo = null;

                while (reader.Read())
                {
                    if (!reader.NodeType.Equals(XmlNodeType.Element))
                        continue;

                    switch (reader.LocalName)
					{
                        case "object":
						    {
                                if (info != null)
                                    ApplyTempToRes(tempInfo, info);

                                info = null;
                                tempInfo = new ObjectInfo();

                                int objectID = Convert.ToInt32(reader.GetAttribute("id"));
                                tempInfo.objectID = objectID;

                                if (objectID != -1)
                                    info = m_ObjectInfos.Where(o => o.objectID == objectID).Single();

                                break;
                            }
                        case "name":
                            {
                                tempInfo.name = reader.ReadElementContentAsString();
                                break;
                            }
                        case "internalname":
                            {
                                tempInfo.internalName = reader.ReadElementContentAsString();
                                break;
                            }
                        case "actorid":
                            {
                                int actorID = reader.ReadElementContentAsInt();

                                if (info == null)
                                    info = m_ObjectInfos.Where(o => o.actorID == actorID).Single();

                                break;
                            }
                        case "description":
                            {
                                tempInfo.description = reader.ReadElementContentAsString();
                                break;
                            }
                        case "bankreq":
                            {
                                tempInfo.bankReq = reader.ReadElementContentAsString();
                                break;
                            }
                        case "dlreq":
                            {
                                tempInfo.dlReq = reader.ReadElementContentAsString();
                                break;
                            }
                        case "renderer":
                            {
                                string type = reader.GetAttribute("type");
                                ObjectInfo.Renderer renderer;

                                switch (type)
                                {
                                    case "NormalBMD":
                                        string file = reader.GetAttribute("file");
                                        float scale = FloatFromString(reader.GetAttribute("scale"));

                                        renderer = new ObjectInfo.NormalBMD_Renderer(file, scale);
                                        break;

                                    case "NormalKCL":
                                        file = reader.GetAttribute("file");
                                        scale = FloatFromString(reader.GetAttribute("scale"));

                                        renderer = new ObjectInfo.NormalKCL_Renderer(file, scale);
                                        break;

                                    case "DoubleBMD":
                                        string file1 = reader.GetAttribute("file1");
                                        string file2 = reader.GetAttribute("file2");
                                        scale = FloatFromString(reader.GetAttribute("scale"));
                                        Vector3 offset1 = VecFromString(reader.GetAttribute("offset1"));
                                        Vector3 offset2 = VecFromString(reader.GetAttribute("offset2"));

                                        renderer = new ObjectInfo.DoubleBMD_Renderer(file1, file2, scale, offset1, offset2);
                                        break;

                                    case "Kurumajiku":
                                        file1 = reader.GetAttribute("file1");
                                        file2 = reader.GetAttribute("file2");
                                        scale = FloatFromString(reader.GetAttribute("scale"));

                                        renderer = new ObjectInfo.Kurumajiku_Renderer(file1, file2, scale);
                                        break;

                                    case "Pole":
                                        Color border = ColorFromString(reader.GetAttribute("border"));
                                        Color fill = ColorFromString(reader.GetAttribute("fill"));

                                        renderer = new ObjectInfo.Pole_Renderer(border, fill);
                                        break;

                                    case "ColorCube":
                                        border = ColorFromString(reader.GetAttribute("border"));
                                        fill = ColorFromString(reader.GetAttribute("fill"));

                                        renderer = new ObjectInfo.ColorCube_Renderer(border, fill);
                                        break;

                                    case "Player":
                                        scale = FloatFromString(reader.GetAttribute("scale"));
                                        string animation = reader.GetAttribute("animation");

                                        renderer = new ObjectInfo.Player_Renderer(scale, animation);
                                        break;

                                    case "Luigi":
                                        scale = FloatFromString(reader.GetAttribute("scale"));
                                        renderer = new ObjectInfo.Luigi_Renderer(scale);
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
                                        renderer = new ObjectInfo.Renderer(type);
                                        break;

                                    default:
                                        throw new Exception("Unknown renderer for '" + info.name + "' (id = " + info.objectID + ").");
                                }

                                tempInfo.renderer = renderer;

                                break;
                            }
                    }
                }

                if (info != null)
                    ApplyTempToRes(tempInfo, info);
            }

            FillListBox();

            /*else
            {
                info = m_ObjectInfos.Where(o => o.objectID == objectID).Single();
            }

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

            ObjectInfo.Renderer renderer;


            info.renderer = renderer;

            m_ObjectInfos.Add(info);*/
        }

        private void ApplyTempToRes(ObjectInfo temp, ObjectInfo res)
		{
            if (!string.IsNullOrWhiteSpace(temp.name))
                res.name = temp.name;

            if (!string.IsNullOrWhiteSpace(temp.internalName))
                res.internalName = temp.internalName;

            if (temp.actorID != -2)
                res.actorID = temp.actorID;

            if (!string.IsNullOrWhiteSpace(temp.description))
                res.description = temp.description;

            if (!string.IsNullOrWhiteSpace(temp.bankReq))
                res.bankReq = temp.bankReq;

            if (!string.IsNullOrWhiteSpace(temp.dlReq))
                res.dlReq = temp.dlReq;

            if (temp.renderer != null)
                res.renderer = temp.renderer;
        }

        private void SaveObjectInfos()
        {
            // order objects
            m_ObjectInfos.Sort((a, b) => CompareIDs(a.actorID, b.actorID));
            m_ObjectInfos.Sort((a, b) => CompareIDs(a.objectID, b.objectID));

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

                    writer.WriteStartElement("renderer");
                    writer.WriteAttributeString("type", info.renderer.type);
                    switch (info.renderer.type)
                    {
                        case "NormalBMD":
                            ObjectInfo.NormalBMD_Renderer BMDRenderer = (ObjectInfo.NormalBMD_Renderer)info.renderer;

                            writer.WriteAttributeString("file", BMDRenderer.fileName);
                            writer.WriteAttributeString("scale", FloatToString(BMDRenderer.scale));
                            
                            break;

                        case "NormalKCL":
                            ObjectInfo.NormalKCL_Renderer KCLRenderer = (ObjectInfo.NormalKCL_Renderer)info.renderer;

                            writer.WriteAttributeString("file", KCLRenderer.fileName);
                            writer.WriteAttributeString("scale", FloatToString(KCLRenderer.scale));

                            break;

                        case "DoubleBMD":
                            ObjectInfo.DoubleBMD_Renderer doubleBMDRenderer = (ObjectInfo.DoubleBMD_Renderer)info.renderer;

                            writer.WriteAttributeString("file1", doubleBMDRenderer.fileName1);
                            writer.WriteAttributeString("file2", doubleBMDRenderer.fileName2);
                            writer.WriteAttributeString("scale", FloatToString(doubleBMDRenderer.scale));
                            writer.WriteAttributeString("offset1", VecToString(doubleBMDRenderer.offset1));
                            writer.WriteAttributeString("offset2", VecToString(doubleBMDRenderer.offset2));

                            break;

                        case "Kurumajiku":
                            ObjectInfo.Kurumajiku_Renderer kurumajikuRenderer = (ObjectInfo.Kurumajiku_Renderer)info.renderer;

                            writer.WriteAttributeString("file1", kurumajikuRenderer.fileName1);
                            writer.WriteAttributeString("file2", kurumajikuRenderer.fileName2);
                            writer.WriteAttributeString("scale", FloatToString(kurumajikuRenderer.scale));
                            
                            break;

                        case "Pole":
                            ObjectInfo.Pole_Renderer poleRenderer = (ObjectInfo.Pole_Renderer)info.renderer;

                            writer.WriteAttributeString("border", ColorToString(poleRenderer.border));
                            writer.WriteAttributeString("fill", ColorToString(poleRenderer.fill));

                            break;

                        case "ColorCube":
                            ObjectInfo.ColorCube_Renderer colorCubeRenderer = (ObjectInfo.ColorCube_Renderer)info.renderer;

                            writer.WriteAttributeString("border", ColorToString(colorCubeRenderer.border));
                            writer.WriteAttributeString("fill", ColorToString(colorCubeRenderer.fill));

                            break;

                        case "Player":
                            ObjectInfo.Player_Renderer playerRenderer = (ObjectInfo.Player_Renderer)info.renderer;

                            writer.WriteAttributeString("scale", FloatToString(playerRenderer.scale));
                            writer.WriteAttributeString("animation", playerRenderer.animation);
                            
                            break;

                        case "Luigi":
                            ObjectInfo.Luigi_Renderer luigiRenderer = (ObjectInfo.Luigi_Renderer)info.renderer;

                            writer.WriteAttributeString("scale", FloatToString(luigiRenderer.scale));

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
                            throw new Exception("Unknown renderer for '" + info.name + "' (id = " + info.objectID + ").");
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

            ObjectInfo.displayObjectID = true;
            ObjectInfo.displayInternalName = false;

            FillCmbRenderer();

            try
            {
                LoadObjectInfos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong:\n{ex}", "Invalid XML", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                Close();
            }
        }

        private void ObjectDatabaseEdtiorForm_Load(object sender, System.EventArgs e)
        {
            lstObjects.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ObjectInfo[] bak = new ObjectInfo[m_ObjectInfos.Count];
            m_ObjectInfos.CopyTo(bak);

            try
            {
                SaveObjectInfos();
                FillListBox(txtSearch.Text);
            }
            catch (Exception ex)
            {
                m_ObjectInfos = bak.ToList();
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

            ObjectInfo[] bak = new ObjectInfo[m_ObjectInfos.Count];
            m_ObjectInfos.CopyTo(bak);

            try
            {
                LoadObjectInfos(ofd.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong:\n{ex}", "Invalid XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_ObjectInfos = bak.ToList();
            }
        }

        private void btnbtnApplyXML_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a XML to apply";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.Cancel) return;

            ObjectInfo[] bak = new ObjectInfo[m_ObjectInfos.Count];
            m_ObjectInfos.CopyTo(bak);

            try
            {
                ModifyObjectInfos(ofd.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong:\n{ex}", "Invalid XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_ObjectInfos = bak.ToList();
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
            cmbRenderer.SelectedIndex = cmbRenderer.Items.IndexOf(info.renderer.type);

            UpdateRenderer(info.renderer);

            m_UpdatingTextBoxes = false;
        }

        private void nudObjectID_ValueChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.objectID = (int)nudObjectID.Value;

            if (ObjectInfo.displayObjectID)
                FillListBox(txtSearch.Text);
        }

        private void nudActorID_ValueChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.actorID = (int)nudActorID.Value;

            if (!ObjectInfo.displayObjectID)
                FillListBox(txtSearch.Text);
        }

        private void txtObjectName_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.name = txtObjectName.Text;
            RefreshListBoxLabels();
        }

        private void txtInternalName_TextChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            m_LastSelectedObjectInfo.internalName = txtInternalName.Text;
            RefreshListBoxLabels();
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

        private void cmbRenderer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            if (cmbRenderer.Text == m_LastSelectedObjectInfo.renderer.type)
                return;

            ObjectInfo.Renderer renderer;

            switch (cmbRenderer.Text)
            {
                case "NormalBMD":
                    renderer = new ObjectInfo.NormalBMD_Renderer();
                    break;

                case "NormalKCL":
                    renderer = new ObjectInfo.NormalKCL_Renderer();
                    break;

                case "DoubleBMD":
                    renderer = new ObjectInfo.DoubleBMD_Renderer();
                    break;

                case "Kurumajiku":
                    renderer = new ObjectInfo.Kurumajiku_Renderer();
                    break;

                case "Pole":
                    renderer = new ObjectInfo.Pole_Renderer();
                    break;

                case "ColorCube":
                    renderer = new ObjectInfo.ColorCube_Renderer();
                    break;

                case "Player":
                    renderer = new ObjectInfo.Player_Renderer();
                    break;

                case "Luigi":
                    renderer = new ObjectInfo.Luigi_Renderer();
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
                    renderer = new ObjectInfo.Renderer(cmbRenderer.Text);
                    break;

                default:
                    throw new Exception(cmbRenderer.Text + " is not a valid renderer.");
            }

            m_LastSelectedObjectInfo.renderer = renderer;

            UpdateRenderer(renderer);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            FillListBox(txtSearch.Text);
        }

        private void btnBorder_Click(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            Color? newColor = GetColorDialogueResult(btnBorder);

            if (newColor == null)
                return;

            if (cmbRenderer.Text == "Pole")
			{
                ObjectInfo.Pole_Renderer renderer = (ObjectInfo.Pole_Renderer)m_LastSelectedObjectInfo.renderer;
                renderer.border = (Color)newColor;

            }
            else if (cmbRenderer.Text == "ColorCube")
			{
                ObjectInfo.ColorCube_Renderer renderer = (ObjectInfo.ColorCube_Renderer)m_LastSelectedObjectInfo.renderer;
                renderer.border = (Color)newColor;
            }
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            if (m_UpdatingTextBoxes || m_LastSelectedObjectInfo == null)
                return;

            Color? newColor = GetColorDialogueResult(btnFill);

            if (newColor == null)
                return;

            if (cmbRenderer.Text == "Pole")
            {
                ObjectInfo.Pole_Renderer renderer = (ObjectInfo.Pole_Renderer)m_LastSelectedObjectInfo.renderer;
                renderer.fill = (Color)newColor;

            }
            else if (cmbRenderer.Text == "ColorCube")
            {
                ObjectInfo.ColorCube_Renderer renderer = (ObjectInfo.ColorCube_Renderer)m_LastSelectedObjectInfo.renderer;
                renderer.fill = (Color)newColor;
            }
        }

        private Color? GetColorDialogueResult(Button button)
        {
            ColorDialog colourDialogue = new ColorDialog();
            DialogResult result = colourDialogue.ShowDialog(this);

            if (result != DialogResult.OK)
                return null;
            
            Color color = colourDialogue.Color;
            SetColorButtonValue(button, color);
            return color;
        }

        private void SetColorButtonValue(Button button, Color color)
        {
            string hexColourString = Helper.GetHexColourString(color);
            float luma = 0.2126f * color.R + 0.7152f * color.G + 0.0722f * color.B;

            button.Text = hexColourString;
            button.BackColor = color;
            button.ForeColor = luma < 50 ? Color.White : Color.Black;
        }

        private void UpdateRenderer(ObjectInfo.Renderer renderer)
        {
            SetRendererColorControlsVis(false);
            SetRendererControlsVis(false);
            m_OnlyPlayerAnimFiles = false;

            switch (renderer.type)
            {
                case "NormalBMD":
                    ObjectInfo.NormalBMD_Renderer normalBMDRenderer = (ObjectInfo.NormalBMD_Renderer)renderer;

                    lblScale.Visible = nudScale.Visible = true;
                    lblRenderer1.Visible = txtRenderer1.Visible = btnRenderer1.Visible = true;

                    nudScale.Value = (decimal)normalBMDRenderer.scale;
                    lblRenderer1.Text = "File:";
                    txtRenderer1.Text = normalBMDRenderer.fileName;

                    m_StartFolder1 = normalBMDRenderer.fileName;
                    m_FileFilter1 = ".bmd";

                    break;

                case "NormalKCL":
                    ObjectInfo.NormalKCL_Renderer normalKCLRenderer = (ObjectInfo.NormalKCL_Renderer)renderer;

                    lblScale.Visible = nudScale.Visible = true;
                    lblRenderer1.Visible = txtRenderer1.Visible = btnRenderer1.Visible = true;

                    nudScale.Value = (decimal)normalKCLRenderer.scale;
                    lblRenderer1.Text = "File:";
                    txtRenderer1.Text = normalKCLRenderer.fileName;

                    m_StartFolder1 = normalKCLRenderer.fileName;
                    m_FileFilter1 = ".kcl";

                    break;

                case "DoubleBMD":
                    ObjectInfo.DoubleBMD_Renderer doubleBMDRenderer = (ObjectInfo.DoubleBMD_Renderer)renderer;

                    lblScale.Visible = nudScale.Visible = true;
                    lblRenderer1.Visible = txtRenderer1.Visible = btnRenderer1.Visible = true;
                    lblRenderer2.Visible = txtRenderer2.Visible = btnRenderer2.Visible = true;
                    lblOffset1.Visible = nudOffset1X.Visible = nudOffset1Y.Visible = nudOffset1Z.Visible = true;
                    lblOffset2.Visible = nudOffset2X.Visible = nudOffset2Y.Visible = nudOffset2Z.Visible = true;

                    nudScale.Value = (decimal)doubleBMDRenderer.scale;
                    lblRenderer1.Text = "File 1:";
                    txtRenderer1.Text = doubleBMDRenderer.fileName1;
                    lblRenderer2.Text = "File 2:";
                    txtRenderer2.Text = doubleBMDRenderer.fileName2;
                    nudOffset1X.Value = (decimal)doubleBMDRenderer.offset1.X;
                    nudOffset1Y.Value = (decimal)doubleBMDRenderer.offset1.Y;
                    nudOffset1Z.Value = (decimal)doubleBMDRenderer.offset1.Z;
                    nudOffset2X.Value = (decimal)doubleBMDRenderer.offset2.X;
                    nudOffset2Y.Value = (decimal)doubleBMDRenderer.offset2.Y;
                    nudOffset2Z.Value = (decimal)doubleBMDRenderer.offset2.Z;

                    m_StartFolder1 = doubleBMDRenderer.fileName1;
                    m_StartFolder2 = doubleBMDRenderer.fileName2;
                    m_FileFilter1 = ".bmd";
                    m_FileFilter2 = ".bmd";

                    break;

                case "Kurumajiku":
                    ObjectInfo.Kurumajiku_Renderer kurumajikuRenderer = (ObjectInfo.Kurumajiku_Renderer)renderer;

                    lblScale.Visible = nudScale.Visible = true;
                    lblRenderer1.Visible = txtRenderer1.Visible = btnRenderer1.Visible = true;
                    lblRenderer2.Visible = txtRenderer2.Visible = btnRenderer2.Visible = true;

                    nudScale.Value = (decimal)kurumajikuRenderer.scale;
                    lblRenderer1.Text = "File 1:";
                    txtRenderer1.Text = kurumajikuRenderer.fileName1;
                    lblRenderer2.Text = "File 2:";
                    txtRenderer2.Text = kurumajikuRenderer.fileName2;

                    m_StartFolder1 = kurumajikuRenderer.fileName1;
                    m_StartFolder2 = kurumajikuRenderer.fileName2;
                    m_FileFilter1 = ".bmd";
                    m_FileFilter2 = ".bmd";

                    break;

                case "Pole":
                    ObjectInfo.Pole_Renderer poleRenderer = (ObjectInfo.Pole_Renderer)renderer;

                    SetRendererColorControlsVis(true);
                    SetColorButtonValue(btnBorder, poleRenderer.border);
                    SetColorButtonValue(btnFill, poleRenderer.fill);

                    break;

                case "ColorCube":
                    ObjectInfo.ColorCube_Renderer colorCubeRenderer = (ObjectInfo.ColorCube_Renderer)renderer;
                    
                    SetRendererColorControlsVis(true);
                    SetColorButtonValue(btnBorder, colorCubeRenderer.border);
                    SetColorButtonValue(btnFill, colorCubeRenderer.fill);

                    break;

                case "Player":
                    ObjectInfo.Player_Renderer playerRenderer = (ObjectInfo.Player_Renderer)renderer;

                    lblScale.Visible = nudScale.Visible = true;
                    lblRenderer1.Visible = txtRenderer1.Visible = btnRenderer1.Visible = true;

                    nudScale.Value = (decimal)playerRenderer.scale;
                    lblRenderer1.Text = "Animation:";
                    txtRenderer1.Text = playerRenderer.animation;

                    m_StartFolder1 = "data/player/";
                    m_FileFilter1 = ".bca";
                    m_OnlyPlayerAnimFiles = true;

                    break;

                case "Luigi":
                    ObjectInfo.Luigi_Renderer luigiRenderer = (ObjectInfo.Luigi_Renderer)renderer;

                    lblScale.Visible = nudScale.Visible = true;

                    nudScale.Value = (decimal)luigiRenderer.scale;

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
                    throw new Exception(cmbRenderer.Text + " is not a valid renderer.");
            }
        }

        private void SetRendererControlsVis(bool vis)
        {
            lblScale.Visible = nudScale.Visible = vis;
            lblRenderer1.Visible = txtRenderer1.Visible = btnRenderer1.Visible = vis;
            lblRenderer2.Visible = txtRenderer2.Visible = btnRenderer2.Visible = vis;
            lblOffset1.Visible = nudOffset1X.Visible = nudOffset1Y.Visible = nudOffset1Z.Visible = vis;
            lblOffset2.Visible = nudOffset2X.Visible = nudOffset2Y.Visible = nudOffset2Z.Visible = vis;
        }

        private void SetRendererColorControlsVis(bool vis)
		{
            lblBorder.Visible = lblFill.Visible = btnBorder.Visible = btnFill.Visible = vis;
		}

        private int CompareIDs(int a, int b)
        {
            if (a == -1) return b == -1 ? 0 : 1; // a == b => 0, -1 > b => 1
            if (b == -1) return -1; // a < -1 => -1
            return a.CompareTo(b);
        }

        private void FillListBox(string search = null)
        {
            if (ObjectInfo.displayObjectID)
            {
                m_ObjectInfos.Sort((a, b) => CompareIDs(a.actorID, b.actorID));
                m_ObjectInfos.Sort((a, b) => CompareIDs(a.objectID, b.objectID));
                // m_ObjectInfos.Sort((a, b) => a.actorID.CompareTo(b.actorID));
                // m_ObjectInfos.Sort((a, b) => a.objectID.CompareTo(b.objectID));
            }
            else
            {
                m_ObjectInfos.Sort((a, b) => CompareIDs(a.objectID, b.objectID));
                m_ObjectInfos.Sort((a, b) => CompareIDs(a.actorID, b.actorID));
                // m_ObjectInfos.Sort((a, b) => a.objectID.CompareTo(b.objectID));
                // m_ObjectInfos.Sort((a, b) => a.actorID.CompareTo(b.actorID));
            }

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

        private void FillCmbRenderer()
		{
            cmbRenderer.Items.Clear();
            cmbRenderer.Items.Add("NormalBMD");
            cmbRenderer.Items.Add("NormalKCL");
            cmbRenderer.Items.Add("DoubleBMD");
            cmbRenderer.Items.Add("Kurumajiku");
            cmbRenderer.Items.Add("Pole");
            cmbRenderer.Items.Add("ColorCube");
            cmbRenderer.Items.Add("Player");
            cmbRenderer.Items.Add("Luigi");
            cmbRenderer.Items.Add("ChainedChomp");
            cmbRenderer.Items.Add("Goomboss");
            cmbRenderer.Items.Add("Tree");
            cmbRenderer.Items.Add("Painting");
            cmbRenderer.Items.Add("UnchainedChomp");
            cmbRenderer.Items.Add("Fish");
            cmbRenderer.Items.Add("Butterfly");
            cmbRenderer.Items.Add("Star");
            cmbRenderer.Items.Add("BowserSkyPlatform");
            cmbRenderer.Items.Add("BigSnowman");
            cmbRenderer.Items.Add("Toxbox");
            cmbRenderer.Items.Add("Pokey");
            cmbRenderer.Items.Add("FlPuzzle");
            cmbRenderer.Items.Add("FlameThrower");
            cmbRenderer.Items.Add("C1Trap");
            cmbRenderer.Items.Add("Wiggler");
            cmbRenderer.Items.Add("Koopa");
            cmbRenderer.Items.Add("KoopaShell");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int newID = m_ObjectInfos.OrderBy(o => o.objectID).Last().objectID + 1;

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

            FillListBox(txtSearch.Text);

            txtBankReq.Text = txtDescription.Text = txtDlReq.Text = txtInternalName.Text = txtObjectName.Text = "";
            nudActorID.Value = nudObjectID.Value = 0;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            ObjectInfo.displayObjectID = !ObjectInfo.displayObjectID;

            if (ObjectInfo.displayObjectID)
                btnSort.Text = "Sort by: Object ID";
            else
                btnSort.Text = "Sort by: Actor ID";

            FillListBox(txtSearch.Text);
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            ObjectInfo.displayInternalName = !ObjectInfo.displayInternalName;

            if (ObjectInfo.displayInternalName)
                btnDisplay.Text = "Display: Internal Name";
            else
                btnDisplay.Text = "Display: Object Name";

            RefreshListBoxLabels();
        }

		private void btnRenderer1_Click(object sender, EventArgs e)
        {
            if (Program.m_ROM == null)
            {
                MessageBox.Show("This button can only be used when a ROM has been loaded.", "No ROM loaded!");
                return;
            }

            ROMFileSelect romFileSelect = new ROMFileSelect();
            romFileSelect.ReInitialize("Select a file", new string[] { m_FileFilter1 }, m_StartFolder1);
            
            DialogResult result = romFileSelect.ShowDialog();
            
            if (result != DialogResult.OK)
                return;

            if (m_OnlyPlayerAnimFiles)
			{
                if (!romFileSelect.m_SelectedFile.StartsWith("data/player/") || !romFileSelect.m_SelectedFile.EndsWith(".bca"))
				{
                    MessageBox.Show("Invalid player animation selected.", "Invalid player animation selected.");
                    return;
				}

                txtRenderer1.Text = romFileSelect.m_SelectedFile.Replace("data/player/", "");
            }
            else
			{
                m_StartFolder1 = romFileSelect.m_SelectedFile;
                txtRenderer1.Text = romFileSelect.m_SelectedFile;
            }
        }

		private void btnRenderer2_Click(object sender, EventArgs e)
        {
            if (Program.m_ROM == null)
			{
                MessageBox.Show("This button can only be used when a ROM has been loaded.", "No ROM loaded!");
                return;
			}

            ROMFileSelect romFileSelect = new ROMFileSelect();
            romFileSelect.ReInitialize("Select a file", new string[] { m_FileFilter2 }, m_StartFolder2);

            DialogResult result = romFileSelect.ShowDialog();

            if (result != DialogResult.OK)
                return;

            m_StartFolder2 = romFileSelect.m_SelectedFile;
            txtRenderer2.Text = romFileSelect.m_SelectedFile;
        }
	}
}
