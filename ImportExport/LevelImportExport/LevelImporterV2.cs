using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SM64DSe.ImportExport.LevelImportExport
{
    class LevelImporterV2 : LevelImporterV1
    {
        public LevelImporterV2() 
            : base() { }

        protected override void ReadSPLCDataFromXML(XmlReader reader, Level level)
        {
            SPLC.Entry entry = new SPLC.Entry();

            while (reader.Read())
            {
                reader.MoveToContent();

                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    switch (reader.LocalName)
                    {
                        case "Entry":
                            entry = new SPLC.Entry();
                            break;
                        case "TerrainType":
                            entry.m_Texture = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "Water":
                            entry.m_Water = (ulong)(reader.ReadElementContentAsBoolean() ? 1 : 0);
                            break;
                        case "ViewID":
                            entry.m_ViewID = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "Traction":
                            entry.m_Traction = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "CameraBehaviour":
                            entry.m_CamBehav = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "Behaviour":
                            entry.m_Behav = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "TransparentToCamera":
                            entry.m_CamThrough = (ulong)(reader.ReadElementContentAsBoolean() ? 1 : 0);
                            break;
                        case "Toxic":
                            entry.m_Toxic = (ulong)(reader.ReadElementContentAsBoolean() ? 1 : 0);
                            break;
                        case "Unknown26":
                            entry.m_Unk26 = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "Padding1":
                            entry.m_Pad1 = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "WindID":
                            entry.m_WindID = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "Padding2":
                            entry.m_Pad2 = (ulong)reader.ReadElementContentAsLong();
                            break;
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement))
                {
                    if (reader.LocalName.Equals("Entry"))
                    {
                        level.m_SPLC.Add(entry);
                    }
                    else if (reader.LocalName.Equals("SPLC"))
                    {
                        return;
                    }
                }
            }
        }

        protected override List<float> ReadTextureAnimationTranslationTable(XmlReader reader)
        {
            return ReadIntArray(reader, "TranslationTable")
                .ConvertAll(x => x / 4096f);
        }

        protected override List<float> ReadTextureAnimationRotationTable(XmlReader reader)
        {
            return ReadShortArray(reader, "RotationTable")
                .ConvertAll(x => x / 4096.0f * 360.0f);
        }

        protected override List<float> ReadTextureAnimationScaleTable(XmlReader reader)
        {
            return ReadIntArray(reader, "ScaleTable")
                .ConvertAll(x => x / 4096f);
        }

        private List<int> ReadIntArray(XmlReader reader, string element)
        {
            int length = int.Parse(reader.GetAttribute("length"));

            List<int> values = new List<int>(length);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("value"))
                {
                    values.Add(reader.ReadElementContentAsInt());
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals(element))
                {
                    return values;
                }
            }

            return values;
        }

        private List<short> ReadShortArray(XmlReader reader, string element)
        {
            int length = int.Parse(reader.GetAttribute("length"));

            List<short> values = new List<short>(length);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("value"))
                {
                    values.Add(short.Parse(reader.ReadElementContentAsString()));
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals(element))
                {
                    return values;
                }
            }

            return values;
        }
    }
}
