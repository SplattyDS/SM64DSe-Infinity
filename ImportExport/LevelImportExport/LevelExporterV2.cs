using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SM64DSe.ImportExport.LevelImportExport
{
    class LevelExporterV2 : LevelExporterV1
    {
        protected override string VERSION { get { return "2"; } }

        protected override void WriteSPLCToXML(XmlWriter writer, Level level)
        {
            writer.WriteStartElement("SPLC");

            foreach (SPLC.Entry entry in level.m_SPLC)
            {
                writer.WriteStartElement("Entry");

                writer.WriteElementString("TerrainType", entry.m_Texture.ToString());
                writer.WriteElementString("Water", BoolToString(entry.m_Water > 0));
                writer.WriteElementString("ViewID", entry.m_ViewID.ToString());
                writer.WriteElementString("Traction", entry.m_Traction.ToString());
                writer.WriteElementString("CameraBehaviour", entry.m_CamBehav.ToString());
                writer.WriteElementString("Behaviour", entry.m_Behav.ToString());
                writer.WriteElementString("TransparentToCamera", BoolToString(entry.m_CamThrough > 0));
                writer.WriteElementString("Toxic", BoolToString(entry.m_Toxic > 0));
                writer.WriteElementString("Unknown26", entry.m_Unk26.ToString());
                writer.WriteElementString("Padding1", entry.m_Pad1.ToString());
                writer.WriteElementString("WindID", entry.m_WindID.ToString());
                writer.WriteElementString("Padding2", entry.m_Pad2.ToString());

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
        
        protected virtual void WriteIntArray(XmlWriter writer, List<int> values, string elementName = "IntArray")
        {
            writer.WriteStartElement(elementName);
            int length = values.Count;
            writer.WriteAttributeString("length", length.ToString());
            for (int i = 0; i < length; i++)
            {
                writer.WriteElementString("value", values[i].ToString());
            }
            writer.WriteEndElement();
        }

        protected string BoolToString(bool value)
        {
            return value.ToString().ToLower();
        }
    }
}
