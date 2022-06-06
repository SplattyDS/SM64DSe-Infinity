using System;
using System.Collections.Generic;
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
}
