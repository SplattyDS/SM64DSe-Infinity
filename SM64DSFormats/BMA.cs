using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SM64DSe.ImportExport;

namespace SM64DSe
{
    public class BMA
    {
        public ushort frames;
        public List<MaterialProperties> matProps;

        public BMA(byte[] BMA_data)
        {
            frames = Helper.Read16(BMA_data, 0x0);
            uint matPropCount = Helper.Read32(BMA_data, 0x8);
            uint offset = Helper.Read32(BMA_data, 0xc);

            matProps = new List<MaterialProperties>();
            matProps.Capacity = (int)matPropCount;

            for (int i = 0; i < matPropCount; i++)
            {
                MaterialProperties matProp = new MaterialProperties();

                matProp.materialID = (short)Helper.Read16(BMA_data, offset);
                uint materialNameOffset = Helper.Read32(BMA_data, offset + 0x4);
                matProp.materialName = Helper.ReadString(BMA_data, materialNameOffset);

                matProp.difRedAdv    = (BMA_data[offset + 0x9] & 1) == 1;
                matProp.difGreenAdv  = (BMA_data[offset + 0xd] & 1) == 1;
                matProp.difBlueAdv   = (BMA_data[offset + 0x11] & 1) == 1;
                matProp.ambRedAdv    = (BMA_data[offset + 0x15] & 1) == 1;
                matProp.ambGreenAdv  = (BMA_data[offset + 0x19] & 1) == 1;
                matProp.ambBlueAdv   = (BMA_data[offset + 0x1d] & 1) == 1;
                matProp.specRedAdv   = (BMA_data[offset + 0x21] & 1) == 1;
                matProp.specGreenAdv = (BMA_data[offset + 0x25] & 1) == 1;
                matProp.specBlueAdv  = (BMA_data[offset + 0x29] & 1) == 1;
                matProp.emiRedAdv    = (BMA_data[offset + 0x2d] & 1) == 1;
                matProp.emiGreenAdv  = (BMA_data[offset + 0x31] & 1) == 1;
                matProp.emiBlueAdv   = (BMA_data[offset + 0x35] & 1) == 1;
                matProp.alphaAdv     = (BMA_data[offset + 0x39] & 1) == 1;

                if (matProp.difRedAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0xa);
                    matProp.difRedValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.difRedValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.difGreenAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0xe);
                    matProp.difGreenValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.difGreenValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.difBlueAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0x12);
                    matProp.difBlueValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.difBlueValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.ambRedAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0x16);
                    matProp.ambRedValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.ambRedValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.ambGreenAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0x1a);
                    matProp.ambGreenValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.ambGreenValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.ambBlueAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0x1e);
                    matProp.ambBlueValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.ambBlueValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.specRedAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0x22);
                    matProp.specRedValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.specRedValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.specGreenAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0x26);
                    matProp.specGreenValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.specGreenValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.specBlueAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0x2a);
                    matProp.specBlueValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.specBlueValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.emiRedAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0x2e);
                    matProp.emiRedValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.emiRedValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.emiGreenAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0x32);
                    matProp.emiGreenValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.emiGreenValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.emiBlueAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0x36);
                    matProp.emiBlueValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.emiBlueValues.Add(BMA_data[valueOffset + j]);
                }
                if (matProp.alphaAdv)
                {
                    int valueOffset = Helper.Read16(BMA_data, offset + 0x3a);
                    matProp.alphaValues = new List<byte>(frames);
                    for (int j = 0; j < frames; j++)
                        matProp.alphaValues.Add(BMA_data[valueOffset + j]);
                }

                matProps.Add(matProp);
                offset += 0x3c;
            }
        }

        public INitroROMBlock GetBMA()
        {
            INitroROMBlock BMA_data = new INitroROMBlock();
            BMA_data.m_Data = new byte[1];

            BMA_data.Write16(0x0, frames);
            BMA_data.Write16(0x2, 0x0);
            BMA_data.Write32(0x8, (uint)matProps.Count());

            uint offset = 0x10;
            uint valueOffset = offset + ((uint)matProps.Count() * 0x3c);
            uint nameOffset = valueOffset + (uint)(frames * matProps.Sum(m => m.GetValueSize(frames)));

            foreach (MaterialProperties matProp in matProps)
            {
                BMA_data.Write16(offset, (ushort)matProp.materialID);
                BMA_data.Write16(offset + 0x2, 0x0);
                BMA_data.Write32(offset + 0x4, nameOffset);

                // material name and offset
                int matNameLength = matProp.materialName.Length + 4 - (matProp.materialName.Length % 4);
                BMA_data.WriteString(nameOffset, matProp.materialName, matNameLength);
                BMA_data.Write32(offset + 0x8, nameOffset);
                nameOffset += (uint)matNameLength;

                BMA_data.Write8(offset + 0x8, 0x1);
                BMA_data.Write8(offset + 0x9, (byte)(matProp.difRedAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0xa, (ushort)valueOffset);
                if (matProp.difRedAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.difRedValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0xc, 0x1);
                BMA_data.Write8(offset + 0xd, (byte)(matProp.difGreenAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0xe, (ushort)valueOffset);
                if (matProp.difGreenAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.difGreenValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0x10, 0x1);
                BMA_data.Write8(offset + 0x11, (byte)(matProp.difBlueAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0x12, (ushort)valueOffset);
                if (matProp.difBlueAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.difBlueValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0x14, 0x1);
                BMA_data.Write8(offset + 0x15, (byte)(matProp.ambRedAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0x16, (ushort)valueOffset);
                if (matProp.ambRedAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.ambRedValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0x18, 0x1);
                BMA_data.Write8(offset + 0x19, (byte)(matProp.ambGreenAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0x1a, (ushort)valueOffset);
                if (matProp.ambGreenAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.ambGreenValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0x1c, 0x1);
                BMA_data.Write8(offset + 0x1d, (byte)(matProp.ambBlueAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0x1e, (ushort)valueOffset);
                if (matProp.ambBlueAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.ambBlueValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0x20, 0x1);
                BMA_data.Write8(offset + 0x21, (byte)(matProp.specRedAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0x22, (ushort)valueOffset);
                if (matProp.specRedAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.specRedValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0x24, 0x1);
                BMA_data.Write8(offset + 0x25, (byte)(matProp.specGreenAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0x26, (ushort)valueOffset);
                if (matProp.specGreenAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.specGreenValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0x28, 0x1);
                BMA_data.Write8(offset + 0x29, (byte)(matProp.specBlueAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0x2a, (ushort)valueOffset);
                if (matProp.specBlueAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.specBlueValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0x2c, 0x1);
                BMA_data.Write8(offset + 0x2d, (byte)(matProp.emiRedAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0x2e, (ushort)valueOffset);
                if (matProp.emiRedAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.emiRedValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0x30, 0x1);
                BMA_data.Write8(offset + 0x31, (byte)(matProp.emiGreenAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0x32, (ushort)valueOffset);
                if (matProp.emiGreenAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.emiGreenValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0x34, 0x1);
                BMA_data.Write8(offset + 0x35, (byte)(matProp.emiBlueAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0x36, (ushort)valueOffset);
                if (matProp.emiBlueAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.emiBlueValues.ToArray());
                    valueOffset += frames;
                }

                BMA_data.Write8(offset + 0x38, 0x1);
                BMA_data.Write8(offset + 0x39, (byte)(matProp.alphaAdv ? 0x1 : 0x0));
                BMA_data.Write16(offset + 0x3a, (ushort)valueOffset);
                if (matProp.alphaAdv)
                {
                    BMA_data.WriteBlock(valueOffset, matProp.alphaValues.ToArray());
                    valueOffset += frames;
                }
            }

            return BMA_data;
        }

        public MaterialProperties GetMatProp(string materialName)
        {
            return matProps.FirstOrDefault(m => m.materialName == materialName);
        }

        public void SetFrame(ModelBase.MaterialDef material, string matName, ushort frame)
        {
            if (frame >= frames)
                throw new IndexOutOfRangeException($"The specified frame {frame} is outside the bounds of the array (max: {frames - 1})");

            MaterialProperties matProp = GetMatProp(matName);

            material.m_Diffuse = System.Drawing.Color.FromArgb(
                matProp.difRedAdv   ? matProp.difRedValues[frame]   : material.m_Diffuse.R,
                matProp.difGreenAdv ? matProp.difGreenValues[frame] : material.m_Diffuse.G,
                matProp.difBlueAdv  ? matProp.difBlueValues[frame]  : material.m_Diffuse.B);

            material.m_Ambient = System.Drawing.Color.FromArgb(
                matProp.ambRedAdv   ? matProp.ambRedValues[frame]   : material.m_Ambient.R,
                matProp.ambGreenAdv ? matProp.ambGreenValues[frame] : material.m_Ambient.G,
                matProp.ambBlueAdv  ? matProp.ambBlueValues[frame]  : material.m_Ambient.B);

            material.m_Specular = System.Drawing.Color.FromArgb(
                matProp.specRedAdv   ? matProp.specRedValues[frame]   : material.m_Specular.R,
                matProp.specGreenAdv ? matProp.specGreenValues[frame] : material.m_Specular.G,
                matProp.specBlueAdv  ? matProp.specBlueValues[frame]  : material.m_Specular.B);

            material.m_Emission = System.Drawing.Color.FromArgb(
                matProp.emiRedAdv   ? matProp.emiRedValues[frame]   : material.m_Emission.R,
                matProp.emiGreenAdv ? matProp.emiGreenValues[frame] : material.m_Emission.G,
                matProp.emiBlueAdv  ? matProp.emiBlueValues[frame]  : material.m_Emission.B);

            material.m_Alpha = matProp.alphaAdv ? matProp.alphaValues[frame] : material.m_Alpha;
        }

        public class MaterialProperties
        {
            public short materialID = -1;
            public string materialName;

            public bool difRedAdv = false;
            public bool difGreenAdv = false;
            public bool difBlueAdv = false;
            public bool ambRedAdv = false;
            public bool ambGreenAdv = false;
            public bool ambBlueAdv = false;
            public bool specRedAdv = false;
            public bool specGreenAdv = false;
            public bool specBlueAdv = false;
            public bool emiRedAdv = false;
            public bool emiGreenAdv = false;
            public bool emiBlueAdv = false;
            public bool alphaAdv = false;

            public List<byte> difRedValues;
            public List<byte> difGreenValues;
            public List<byte> difBlueValues;
            public List<byte> ambRedValues;
            public List<byte> ambGreenValues;
            public List<byte> ambBlueValues;
            public List<byte> specRedValues;
            public List<byte> specGreenValues;
            public List<byte> specBlueValues;
            public List<byte> emiRedValues;
            public List<byte> emiGreenValues;
            public List<byte> emiBlueValues;
            public List<byte> alphaValues;

            public int GetValueSize(int frames)
            {
                int totalSize = 0;

                if (difRedAdv)
                    totalSize += frames;
                if (difGreenAdv)
                    totalSize += frames;
                if (difBlueAdv)
                    totalSize += frames;
                if (ambRedAdv)
                    totalSize += frames;
                if (ambGreenAdv)
                    totalSize += frames;
                if (ambBlueAdv)
                    totalSize += frames;
                if (specRedAdv)
                    totalSize += frames;
                if (specGreenAdv)
                    totalSize += frames;
                if (specBlueAdv)
                    totalSize += frames;
                if (emiRedAdv)
                    totalSize += frames;
                if (emiGreenAdv)
                    totalSize += frames;
                if (emiBlueAdv)
                    totalSize += frames;
                if (alphaAdv)
                    totalSize += frames;

                return totalSize;
            }
        }
    }















    public enum PropTypes
    {
        difRed,
        difGreen,
        difBlue,
        ambRed,
        ambGreen,
        ambBlue,
        specRed,
        specGreen,
        specBlue,
        emitRed,
        emitGreen,
        emitBlue,
        alpha,

        num
    };

    public class MaterialAnim
    {
        public class MaterialProperty
        {
            public bool advance = false;
            public List<byte> values = null;

            // used when saving only
            public ushort offset;
        };

        public class MaterialProperties
        {
            public ushort matID;
            public string matName;
            public MaterialProperty[] props;

            public MaterialProperties()
            {
                props = new MaterialProperty[(int)PropTypes.num];
                for (PropTypes i = 0; i < PropTypes.num; i++)
                    props[(int)i] = new MaterialProperty();
            }

            public MaterialProperties(string matName)
            {
                this.matName = matName;
                props = new MaterialProperty[(int)PropTypes.num];
                for (PropTypes i = 0; i < PropTypes.num; i++)
                    props[(int)i] = new MaterialProperty();
            }

            public List<byte> Values(PropTypes type)
            {
                return props[(int)type].values;
            }

            public byte Value(PropTypes type, int index)
            {
                if (Adv(type))
                    return props[(int)type].values[index];
                else
                    return props[(int)type].values[0];
            }

            public bool Adv(PropTypes type)
            {
                return props[(int)type].advance;
            }

            public void SetValue(PropTypes type, int index, byte val)
            {
                props[(int)type].values[index] = val;
            }

            public void InitValue(PropTypes type, byte val)
            {
                props[(int)type].values = new List<byte>(1);
                props[(int)type].values.Add(val);
            }

            public Color GetDif(int frame)
            {
                return Helper.BGR15ToColor(
                Value(PropTypes.difRed, frame),
                Value(PropTypes.difGreen, frame),
                Value(PropTypes.difBlue, frame));
            }

            public Color GetAmb(int frame)
            {
                return Helper.BGR15ToColor(
                Value(PropTypes.ambRed, frame),
                Value(PropTypes.ambGreen, frame),
                Value(PropTypes.ambBlue, frame));
            }

            public Color GetSpec(int frame)
            {
                return Helper.BGR15ToColor(
                Value(PropTypes.specRed, frame),
                Value(PropTypes.specGreen, frame),
                Value(PropTypes.specBlue, frame));
            }

            public Color GetEmit(int frame)
            {
                return Helper.BGR15ToColor(
                Value(PropTypes.emitRed, frame),
                Value(PropTypes.emitGreen, frame),
                Value(PropTypes.emitBlue, frame));
            }
        };

        private ushort numFrames;
        private List<MaterialProperties> matProps;
        public NitroFile file;

        public MaterialAnim(NitroFile file)
        {
            this.file = file;
            numFrames = file.Read16(0x0);
            uint valueOffset = file.Read32(0x4);
            uint numMatProps = file.Read32(0x8);
            uint matPropOffset = file.Read32(0xc);

            matProps = new List<MaterialProperties>();

            for (uint i = 0; i < numMatProps; i++)
            {
                uint curOffset = matPropOffset + i * 0x3c;

                MaterialProperties matProp = new MaterialProperties();
                matProp.matID = file.Read16(curOffset);
                matProp.matName = file.ReadString(file.Read16(curOffset + 0x4), 0);

                for (PropTypes j = 0; j < PropTypes.num; j++)
                {
                    bool adv = file.Read8(curOffset + 0x9 + (uint)j * 0x4) == 1;
                    matProp.props[(int)j] = new MaterialProperty
                    {
                        advance = adv,
                        values = matProp.props[(int)j].values = file.ReadBlock(valueOffset + file.Read16(curOffset + 0xa + (uint)j * 0x4), adv ? numFrames : 1u).ToList()
                    }; 
                }

                matProps.Add(matProp);
            }
        }

        public ushort GetNumFrames()
        {
            return numFrames;
        }

        public void SetNumFrames(ushort newNumFrames)
        {
            if (numFrames == newNumFrames)
                return;

            foreach (MaterialProperties matProp in matProps)
            {
                if (numFrames < newNumFrames)
                {
                    for (PropTypes i = 0; i < PropTypes.num; i++) if (matProp.props[(int)i].advance)
                        matProp.props[(int)i].values.Add(matProp.props[(int)i].values.Last());
                }
                else
                {
                    for (PropTypes i = 0; i < PropTypes.num; i++) if (matProp.props[(int)i].advance)
                        matProp.props[(int)i].values.RemoveAt(matProp.props[(int)i].values.Count() - 1);
                }
            }

            numFrames = newNumFrames;
        }

        public void SaveFile()
        {
            file.Write16(0x0, numFrames);
            file.Write16(0x2, 0x0);
            file.Write32(0x4, 0x10);
            file.Write32(0x8, (uint)matProps.Count());

            List<byte> values = new List<byte>();
            
            foreach (MaterialProperties matProp in matProps)
            {
                for (PropTypes i = 0; i < PropTypes.num; i++)
                {
                    var x = CheckIfListIsInList(matProp.props[(int)i].values, values);
                    if (!x.Item1)
                    {
                        matProp.props[(int)i].offset = (ushort)values.Count();
                        values.AddRange(matProp.props[(int)i].values);
                    }
                    else
                        matProp.props[(int)i].offset = (ushort)x.Item2;
                }
            }

            int count = values.Count();
            values.Capacity = count + (count % 4);
            for (int i = 0; i < count % 4; i++)
                values.Add(0);

            uint matPropOffset = 0x10 + (uint)values.Count();
            uint stringOffset = matPropOffset + 0x3c * (uint)matProps.Count();
            uint curOffset = matPropOffset;

            file.Write32(0xc, matPropOffset);
            file.WriteBlock(0x10, values.ToArray());

            foreach (MaterialProperties matProp in matProps)
            {
                int len = matProp.matName.Length;
                int paddedLen = len + (len % 4 != 0 ? len % 4 : 4);

                file.Write16(curOffset, matProp.matID);
                file.Write16(curOffset + 0x2, 0);
                file.Write32(curOffset + 0x4, stringOffset);
                curOffset += 8;

                for (PropTypes i = 0; i < PropTypes.num; i++)
                {
                    file.Write8(curOffset, 0x01);
                    file.Write8(curOffset + 0x1, (byte)(matProp.props[(int)i].advance ? 1 : 0));
                    file.Write16(curOffset + 0x2, matProp.props[(int)i].offset);
                    curOffset += 4;
                }

                file.WriteString(stringOffset, matProp.matName, paddedLen);

                stringOffset += (uint)paddedLen;
            }

            file.RemoveSpace(stringOffset, (uint)file.m_Data.Length - stringOffset);
            file.SaveChanges();
        }

        // bool: whether arr1 was in arr2
        // uint: first index of arr1 in arr2
        private (bool, uint) CheckIfListIsInList(List<byte> arr1, List<byte> arr2)
        {
            int c1 = arr1.Count();
            int c2 = arr2.Count();

            for (int ind2 = 0; ind2 < c2; ind2++)
            {
                for (int ind1 = 0; ind1 < c1; ind1++)
                {
                    if (arr1[ind1] != arr2[ind2])
                        break;

                    if (ind1 == c1 - 1)
                        return (true, ((uint)ind2 - (uint)c1) + 1);
                }
            }

            return (false, 0);
        }

        public MaterialProperties GetMatPropFromName(string matName)
        {
            foreach (MaterialProperties matProp in matProps) if (matProp.matName == matName)
                return matProp;

            return null;
        }

        public void AddMatProp(MaterialProperties matProp)
        {
            matProps.Add(matProp);
        }

        public void RemoveMatProp(MaterialProperties matProp)
        {
            matProps.Remove(matProp);
        }

        public void EnableProp(MaterialProperties matProp, PropTypes type)
        {
            matProp.props[(int)type].advance = true;
            byte def = matProp.props[(int)type].values[0];
            for (int i = 1; i < numFrames; i++)
                matProp.props[(int)type].values.Add(def);
        }

        public void DisableProp(MaterialProperties matProp, PropTypes type, byte onlyVal)
        {
            matProp.props[(int)type].advance = false;
            matProp.props[(int)type].values = new List<byte>();
            matProp.props[(int)type].values.Add(onlyVal);
        }
    };
}
