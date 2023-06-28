using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM64DSe
{
    public class TexAnim
    {
        public int m_Area; // used when rendering level
        public uint m_NumFrames;
        public NitroFile m_File;

        public int m_NumScaleXValues { get { int count = 0; foreach (Def def in m_Defs) { count += def.m_NumScaleXValues; } return count; } }
        public int m_NumScaleYValues { get { int count = 0; foreach (Def def in m_Defs) { count += def.m_NumScaleYValues; } return count; } }
        public int m_NumRotationValues { get { int count = 0; foreach (Def def in m_Defs) { count += def.m_NumRotationValues; } return count; } }
        public int m_NumTranslationXValues { get { int count = 0; foreach (Def def in m_Defs) { count += def.m_NumTranslationXValues; } return count; } }
        public int m_NumTranslationYValues { get { int count = 0; foreach (Def def in m_Defs) { count += def.m_NumTranslationYValues; } return count; } }

        public List<Def> m_Defs;
        public int m_NumDefs { get { return (m_Defs != null) ? m_Defs.Count : 0; } }

        public TexAnim(NitroFile file = null, int area = -1)
        {
            // Address of the animation data
            m_Area = area;
            m_File = file;

            if (file == null)
            {
                m_Defs = new List<Def>();
                return;
            }

            m_NumFrames = file.Read32(0x00);
            uint scaleOffset = file.Read32(0x04);
            uint rotOffset = file.Read32(0x08);
            uint transOffset = file.Read32(0x0c);
            uint numAnims = file.Read32(0x10);

            m_Defs = new List<Def>((int)numAnims);
            uint animOffsetStatic = file.Read32(0x14);

            for (uint animOffset = animOffsetStatic; animOffset < animOffsetStatic + 0x1c * numAnims; animOffset += 0x1c)
            {
                Def def = new Def();
                def.m_MaterialName = file.ReadString(file.Read32(animOffset + 0x04), -1);

                uint numScaleX = file.Read16(animOffset + 0x08);
                uint numScaleY = file.Read16(animOffset + 0x0C);
                uint numRot = file.Read16(animOffset + 0x10);
                uint numTransX = file.Read16(animOffset + 0x14);
                uint numTransY = file.Read16(animOffset + 0x18);

                uint scaleXIndex = file.Read16(animOffset + 0x0A);
                uint scaleYIndex = file.Read16(animOffset + 0x0E);
                uint rotIndex = file.Read16(animOffset + 0x12);
                uint transXIndex = file.Read16(animOffset + 0x16);
                uint transYIndex = file.Read16(animOffset + 0x1A);

                def.m_ScaleXValues = new List<float>((int)numScaleX);
                def.m_ScaleYValues = new List<float>((int)numScaleY);
                def.m_RotationValues = new List<float>((int)numRot);
                def.m_TranslationXValues = new List<float>((int)numTransX);
                def.m_TranslationYValues = new List<float>((int)numTransY);

                uint scaleXStartOffset = scaleOffset + (scaleXIndex * 4);
                uint scaleYStartOffset = scaleOffset + (scaleYIndex * 4);
                uint rotationStartOffset = rotOffset + (rotIndex * 2);
                uint translationXStartOffset = transOffset + (transXIndex * 4);
                uint translationYStartOffset = transOffset + (transYIndex * 4);

                for (uint offs = scaleXStartOffset; offs < scaleXStartOffset + numScaleX * 4; offs += 4)
                    def.m_ScaleXValues.Add((int)file.Read32(offs) / 4096.0f);
                for (uint offs = scaleYStartOffset; offs < scaleYStartOffset + numScaleY * 4; offs += 4)
                    def.m_ScaleYValues.Add((int)file.Read32(offs) / 4096.0f);
                for (uint offs = rotationStartOffset; offs < rotationStartOffset + numRot * 2; offs += 2)
                    def.m_RotationValues.Add((short)file.Read16(offs) / 4096.0f * 360.0f);
                for (uint offs = translationXStartOffset; offs < translationXStartOffset + numTransX * 4; offs += 4)
                    def.m_TranslationXValues.Add((int)file.Read32(offs) / 4096.0f);
                for (uint offs = translationYStartOffset; offs < translationYStartOffset + numTransY * 4; offs += 4)
                    def.m_TranslationYValues.Add((int)file.Read32(offs) / 4096.0f);

                m_Defs.Add(def);
            }
        }

        public void Save()
        {
            if (m_File == null)
                return;

            var x = m_Defs.Select(d => d.m_CombinedScaleValuesInt.ToList()).ToList();
            var y = m_Defs.Select(d => d.m_RotationValuesInt.ToList()).ToList();
            var z = m_Defs.Select(d => d.m_CombinedTranslationValuesInt.ToList()).ToList();

            List<int> scaleValues = CombineTransformValues(x);
            List<int> rotationValues = CombineTransformValues(y);
            List<int> translationValues = CombineTransformValues(z);

            uint scaleOffset = 0x18; // after the header
            uint rotationOffset = scaleOffset + (uint)(scaleValues.Count * 4);
            uint translationOffset = rotationOffset + (uint)(rotationValues.Count * 2);
            uint animationsOffset = translationOffset + (uint)(translationValues.Count * 4);

            uint numAnims = (uint)m_Defs.Count;
            uint stringsOffset = animationsOffset + (numAnims * 0x1c);

            m_File.Write32(0x00, m_NumFrames);
            m_File.Write32(0x04, scaleOffset);
            m_File.Write32(0x08, rotationOffset);
            m_File.Write32(0x0c, translationOffset);
            m_File.Write32(0x10, numAnims);
            m_File.Write32(0x14, animationsOffset);

            for (int i = 0; i < scaleValues.Count; i++)
                m_File.Write32(scaleOffset + ((uint)i * 4), (uint)scaleValues[i]);

            for (int i = 0; i < rotationValues.Count; i++)
                m_File.Write16(rotationOffset + ((uint)i * 2), (ushort)rotationValues[i]);

            for (int i = 0; i < translationValues.Count; i++)
                m_File.Write32(translationOffset + ((uint)i * 4), (uint)translationValues[i]);

            for (int i = 0; i < m_Defs.Count; i++)
            {
                Def def = m_Defs[i];
                uint curAnimOffset = animationsOffset + (uint)(0x1c * i);

                List<int> currScaleXValues = def.m_ScaleXValuesInt;
                List<int> currScaleYValues = def.m_ScaleYValuesInt;
                List<int> currRotationValues = def.m_RotationValuesInt;
                List<int> currTranslationXValues = def.m_TranslationXValuesInt;
                List<int> currTranslationYValues = def.m_TranslationYValuesInt;

                m_File.Write32(curAnimOffset + 0x00, 0x0000ffff);
                m_File.Write32(curAnimOffset + 0x04, 0x00000000); // mat name offset, currently unknown
                m_File.Write16(curAnimOffset + 0x08, (ushort)def.m_ScaleXValues.Count);
                m_File.Write16(curAnimOffset + 0x0a, (ushort)Helper.FindSubList(scaleValues, currScaleXValues));
                m_File.Write16(curAnimOffset + 0x0c, (ushort)def.m_ScaleYValues.Count);
                m_File.Write16(curAnimOffset + 0x0e, (ushort)Helper.FindSubList(scaleValues, currScaleYValues));
                m_File.Write16(curAnimOffset + 0x10, (ushort)def.m_RotationValues.Count);
                m_File.Write16(curAnimOffset + 0x12, (ushort)Helper.FindSubList(rotationValues, currRotationValues));
                m_File.Write16(curAnimOffset + 0x14, (ushort)def.m_TranslationXValues.Count);
                m_File.Write16(curAnimOffset + 0x16, (ushort)Helper.FindSubList(translationValues, currTranslationXValues));
                m_File.Write16(curAnimOffset + 0x18, (ushort)def.m_TranslationYValues.Count);
                m_File.Write16(curAnimOffset + 0x1a, (ushort)Helper.FindSubList(translationValues, currTranslationYValues));
            }

            uint curStringOffset = stringsOffset;

            for (int i = 0; i < m_Defs.Count; i++)
            {
                Def def = m_Defs[i];
                uint curAnimOffset = animationsOffset + (uint)(0x1c * i);

                int stringLength = def.m_MaterialName.Length + 1;
                stringLength += stringLength % 4;

                m_File.WriteString(curStringOffset, def.m_MaterialName, stringLength);
                m_File.Write32(curAnimOffset + 0x04, curStringOffset);

                curStringOffset += (uint)stringLength;
            }

            m_File.SaveChanges();
        }

        public string GetDescription()
        {
            if (m_Area > 0)
                return $"Area: {m_Area}, Frames: {m_NumFrames}";

            return $"Frames: {m_NumFrames}";
        }

        public static float AnimationValue(List<float> values, int frame, int length)
        {
            if (values.Count > 0 && length > 0)
            {
                if (frame % length < values.Count)
                    return values[frame % length];
                else
                    return values.Last();
            }
            else
                return 0;
        }

        public static List<int> CombineTransformValues(List<List<int>> vals)
        {
            vals.RemoveAll(x => x.Count == 0); //get rid of empty animations
            for (int i = vals.Count - 1; i >= 0; --i)
                if (vals.GetRange(0, i).Any(x => x.SequenceEqual(vals[i])))
                    vals.RemoveAt(i);

            List<int> ret = new List<int>();
            vals.ForEach(x => ret.AddRange(x));
            return ret;
            //Further compression can be done, but the algorithm is complicated and not worth it right now.
            //To get a taste of how complicated it would be, look at the palette compression algorithm for
            //a Tex4x4 texture.
        }

        public class Def
        {
            public string m_MaterialName;
            public List<float> m_ScaleXValues;
            public List<float> m_ScaleYValues;
            public List<float> m_RotationValues;
            public List<float> m_TranslationXValues;
            public List<float> m_TranslationYValues;

            public int m_NumScaleXValues { get { return (m_ScaleXValues != null) ? m_ScaleXValues.Count : 0; } }
            public int m_NumScaleYValues { get { return (m_ScaleYValues != null) ? m_ScaleYValues.Count : 0; } }
            public int m_NumRotationValues { get { return (m_RotationValues != null) ? m_RotationValues.Count : 0; } }
            public int m_NumTranslationXValues { get { return (m_TranslationXValues != null) ? m_TranslationXValues.Count : 0; } }
            public int m_NumTranslationYValues { get { return (m_TranslationYValues != null) ? m_TranslationYValues.Count : 0; } }

            public List<int> m_ScaleXValuesInt { get { return Helper.FloatListTo20_12IntList(m_ScaleXValues); } }
            public List<int> m_ScaleYValuesInt { get { return Helper.FloatListTo20_12IntList(m_ScaleYValues); } }
            public List<int> m_RotationValuesInt { get { return Helper.FloatListToRotationIntList(m_RotationValues); } }
            public List<int> m_TranslationXValuesInt { get { return Helper.FloatListTo20_12IntList(m_TranslationXValues); } }
            public List<int> m_TranslationYValuesInt { get { return Helper.FloatListTo20_12IntList(m_TranslationYValues); } }

            public List<float> m_CombinedTranslationValues
            {
                get
                {
                    List<float> translations = new List<float>();
                    translations.AddRange(m_TranslationXValues);
                    translations.AddRange(m_TranslationYValues);
                    return translations;
                }
            }

            public List<float> m_CombinedScaleValues
            {
                get
                {
                    List<float> translations = new List<float>();
                    translations.AddRange(m_ScaleXValues);
                    translations.AddRange(m_ScaleYValues);
                    return translations;
                }
            }

            public List<int> m_CombinedTranslationValuesInt { get { return Helper.FloatListTo20_12IntList(m_CombinedTranslationValues); } }
            public List<int> m_CombinedScaleValuesInt { get { return Helper.FloatListTo20_12IntList(m_CombinedScaleValues); } }
        }
    }
}
