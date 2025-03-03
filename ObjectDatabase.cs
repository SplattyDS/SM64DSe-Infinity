﻿/*
    Copyright 2012 Kuribo64

    This file is part of SM64DSe.

    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.

    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Net;
using System.Windows.Forms;

namespace SM64DSe
{
    public static class ObjectDatabase
    {
        public class ObjectInfo
        {
            public struct ParamInfo
            {
                public string m_Name;
                public int m_Offset, m_Length;
                public string m_Type, m_Values;
                public string m_Description;
            }

            public string GetBasicInfo()
            {
                return (m_Name + "\n" + m_InternalName + "\n" + m_Description);
            }

            public ushort m_ID;
            public ushort m_ActorID;
            public string m_Name;
            public string m_InternalName;
            public string m_Description;

            public int m_BankRequirement;
            public int m_NumBank, m_BankSetting;

            public string[] m_dlRequirements;

            public string[] m_Renderer;

            public string m_IsCustomModelPath;

            public ParamInfo[] m_ParamInfo;
        }

        public static ObjectInfo[] m_ObjectInfo = null;
        public static ObjectInfo[] m_OtherActorInfo = null;
        public static uint m_Timestamp;
        public static WebClient m_WebClient;


        public static void Initialize()
        {
            m_ObjectInfo = new ObjectInfo[65536];
            for (int i = 0; i < 65536; i++)
                m_ObjectInfo[i] = new ObjectInfo();

            m_OtherActorInfo = new ObjectInfo[65536];
            for (int i = 0; i < 65536; i++)
                m_OtherActorInfo[i] = new ObjectInfo();

            m_WebClient = new WebClient();
        }

        public static void Load()
        {
            FileStream fs = null; XmlReader xr = null;
            try 
            { 
                fs = File.OpenRead("objectdb.xml"); 
                xr = XmlReader.Create(fs);

                m_Timestamp = uint.MaxValue;
            }
            catch
            {
                if (xr != null) xr.Close();
                if (fs != null) fs.Close();

                m_Timestamp = 1;
                throw new Exception("Failed to open objectdb.xml");
            }

            int i = 0;

            while (xr.ReadToFollowing("object"))
            {
                string temp;

                xr.MoveToAttribute("id");
                int id = 0; int.TryParse(xr.Value, out id);

                ObjectInfo oinfo;

                if (id == -1)
                {
                    oinfo = m_OtherActorInfo[i++];
                }
                else
				{
                    if ((id < 0) || (id > m_ObjectInfo.Length))
                        continue;

                    oinfo = m_ObjectInfo[id];
                }

                oinfo.m_ID = (ushort)id;

                xr.ReadToFollowing("name");
                oinfo.m_Name = xr.ReadElementContentAsString();
                xr.ReadToFollowing("internalname");
                oinfo.m_InternalName = xr.ReadElementContentAsString();
                if (oinfo.m_InternalName.StartsWith("@CUSTOM%")) { oinfo.m_IsCustomModelPath = oinfo.m_InternalName.Substring(8); } else { oinfo.m_IsCustomModelPath = null; }
                
                if (oinfo.m_Name == "")
                    oinfo.m_Name = oinfo.m_InternalName;

                xr.ReadToFollowing("actorid");
                temp = xr.ReadElementContentAsString();
                ushort.TryParse(temp, out oinfo.m_ActorID);

                xr.ReadToFollowing("description");
                oinfo.m_Description = xr.ReadElementContentAsString();

                xr.ReadToFollowing("bankreq");
                temp = xr.ReadElementContentAsString();
                if (temp == "none")
                    oinfo.m_BankRequirement = 0;
                else
                {
                    oinfo.m_BankRequirement = 1;
                    try
                    {
                        oinfo.m_NumBank = int.Parse(temp.Substring(0, temp.IndexOf('=')));
                        oinfo.m_BankSetting = int.Parse(temp.Substring(temp.IndexOf('=') + 1));
                    }
                    catch { oinfo.m_BankRequirement = 2; }
                }

                xr.ReadToFollowing("dlreq");
                temp = xr.ReadElementContentAsString();
                if (temp == "none")
                    oinfo.m_dlRequirements = null;
                else
                    oinfo.m_dlRequirements = temp.Split(' ');

                xr.ReadToFollowing("renderer");
                string type = xr.GetAttribute("type");
                string renderer = type + "";

                switch (type)
                {
                    case "NormalBMD":
                    case "NormalKCL":
                        renderer += ' ' + xr.GetAttribute("file") + ' ' + xr.GetAttribute("scale");
                        break;
                    case "DoubleBMD":
                        renderer += ' ' + xr.GetAttribute("file1") + ' ' + xr.GetAttribute("file2") + ' ' + xr.GetAttribute("scale");
                        string offset1 = xr.GetAttribute("offset1");
                        if (!string.IsNullOrEmpty(offset1))
                            renderer += ' ' + offset1 + ' ' + xr.GetAttribute("offset2");
                        break;
                    case "Kurumajiku":
                        renderer += ' ' + xr.GetAttribute("file1") + ' ' + xr.GetAttribute("file2") + ' ' + xr.GetAttribute("scale");
                        break;
                    case "Pole":
                    case "ColorCube":
                        renderer += ' ' + xr.GetAttribute("border") + ' ' + xr.GetAttribute("fill");
                        break;
                    case "Player":
                        renderer += ' ' + xr.GetAttribute("scale") + ' ' + xr.GetAttribute("animation");
                        break;
                    case "Luigi":
                        renderer += ' ' + xr.GetAttribute("scale");
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
                        MessageBox.Show("Unknown renderer for '" + oinfo.m_Name + "' (id = " + oinfo.m_ID + ").");
                        break;
                }

                oinfo.m_Renderer = renderer.Split(' ');

                List<ObjectInfo.ParamInfo> paramlist = new List<ObjectInfo.ParamInfo>();
                while (xr.ReadToNextSibling("param"))
                {
                    ObjectInfo.ParamInfo pinfo = new ObjectInfo.ParamInfo();

                    xr.ReadToFollowing("name");
                    pinfo.m_Name = xr.ReadElementContentAsString();

                    xr.ReadToFollowing("offset");
                    temp = xr.ReadElementContentAsString();
                    int.TryParse(temp, out pinfo.m_Offset);
                    xr.ReadToFollowing("length");
                    temp = xr.ReadElementContentAsString();
                    int.TryParse(temp, out pinfo.m_Length);

                    xr.ReadToFollowing("type");
                    pinfo.m_Type = xr.ReadElementContentAsString();
                    xr.ReadToFollowing("values");
                    pinfo.m_Values = xr.ReadElementContentAsString();

                    xr.ReadToFollowing("description");
                    pinfo.m_Description = xr.ReadElementContentAsString();

                    paramlist.Add(pinfo);
                }
                oinfo.m_ParamInfo = paramlist.ToArray();
            }

            xr.Close();
            fs.Close();
        }

        public static void LoadFallback()
        {
            StringReader sr = new StringReader(Properties.Resources.obj_list);

            String curline;
            Regex lineregex = new Regex("0x([\\dabcdef]+) == (.*?) \\(0x([\\dabcdef]+)\\)");
            
            while ((curline = sr.ReadLine()) != null)
            {
                Match stuff = lineregex.Match(curline);

                int id = int.Parse(stuff.Groups[1].Value, NumberStyles.HexNumber);
                ObjectInfo oinfo = m_ObjectInfo[id];

                oinfo.m_ID = (ushort)id;
                oinfo.m_Name = stuff.Groups[2].Value;
                oinfo.m_InternalName = stuff.Groups[2].Value;
                oinfo.m_ActorID = ushort.Parse(stuff.Groups[3].Value, NumberStyles.HexNumber);

                oinfo.m_Description = "";
                oinfo.m_BankRequirement = 2;
                oinfo.m_dlRequirements = null;
                oinfo.m_ParamInfo = new ObjectInfo.ParamInfo[0];
            }

            sr.Close();
        }

        public static void Update(bool force)
        {
            string ts = force ? "" : "?ts=" + m_Timestamp.ToString();
            m_WebClient.DownloadStringAsync(new Uri(Program.ServerURL + "download_objdb.php" + ts));
        }

        private static readonly string[] HEADER_START =
        {
            "#pragma once",
            "",
            "enum ActorIDs",
            "{",
        };

        private static readonly string[] HEADER_MID =
        {
            "};",
            "",
            "enum ObjectIDs",
            "{",
        };

        private static readonly string[] HEADER_END =
        {
            "};",
        };

        class CppInfo
        {
            public string m_InternalName;
            public int m_ObjectID;
            public int m_ActorID;

            public string GetLine(bool actorID, int maxLength)
            {
                string name = m_InternalName + (actorID ? "_ACTOR_ID" : "_OBJECT_ID");
                return '\t' + name + new string(' ', maxLength - name.Length) + " = " + (actorID ? m_ActorID : m_ObjectID) + ",";
            }
        }

        public static List<string> ToCPP()
        {
            FileStream fs = null; XmlReader xr = null;
            try
            {
                fs = File.OpenRead("objectdb.xml");
                xr = XmlReader.Create(fs);

                m_Timestamp = uint.MaxValue;
            }
            catch
            {
                if (xr != null) xr.Close();
                if (fs != null) fs.Close();

                m_Timestamp = 1;
                throw new Exception("Failed to open objectdb.xml");
            }

            List<CppInfo> infos = new List<CppInfo>();
            int longestNameLength = 0;

            while (xr.ReadToFollowing("object"))
            {
                CppInfo info = new CppInfo();

                xr.MoveToAttribute("id");
                info.m_ObjectID = Convert.ToInt32(xr.Value);
                
                xr.ReadToFollowing("internalname");
                info.m_InternalName = xr.ReadElementContentAsString();

                int length = info.m_InternalName.Length;
                if (length > longestNameLength)
                    longestNameLength = length;

                xr.ReadToFollowing("actorid");
                info.m_ActorID = Convert.ToInt32(xr.ReadElementContentAsString());

                infos.Add(info);
            }

            xr.Close();
            fs.Close();

            List<string> lines = new List<string>();

            lines.AddRange(HEADER_START);

            infos.Sort((a, b) => a.m_ActorID.CompareTo(b.m_ActorID));
            foreach (CppInfo info in infos)
            {
                if (info.m_ActorID == -1) continue;
                lines.Add(info.GetLine(true, longestNameLength + 9));
            }

            lines.AddRange(HEADER_MID);

            infos.Sort((a, b) => a.m_ObjectID.CompareTo(b.m_ObjectID));
            foreach (CppInfo info in infos)
            {
                if (info.m_ObjectID == -1) continue;
                lines.Add(info.GetLine(false, longestNameLength + 10));
            }

            lines.AddRange(HEADER_END);

            return lines;
        }
    }
}
