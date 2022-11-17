using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SM64DSe.Patcher
{
    public static class PatchProcessor
    {
        public static void Process(DirectoryInfo codeDir, string[] p)
        {
            if (p.Length < 2)
                return;

            int minLength = p[1] == "arm9" || p[1] == "hooks" || p[1] == "test" || p[1] == "symbols" ? 3 : 4;

            if (p.Length < minLength)
                return;

            string sourceDir = p[minLength - 1].Remove(0, 1).Remove(p[minLength - 1].Length - 2, 1);

            switch (p[1])
            {
                case "arm9":
                    CompileArm9(codeDir, sourceDir);
                    break;

                case "overlay":
                    uint ovID = Convert.ToUInt32(p[2]);
                    uint addr = new NitroOverlay(Program.m_ROM, ovID).GetRAMAddr();
                    UpdateMakefileSources(codeDir, sourceDir);
                    PatchCompiler.compilePatch(addr, codeDir);
                    MakeOverlay(ovID, codeDir);
                    UpdateSymbols(codeDir, "Symbols from overlay " + ovID);
                    PatchCompiler.cleanPatch(codeDir);
                    break;

                case "dl":
                    string fileName = p[2].Remove(0, 1).Remove(p[2].Length - 2, 1);
                    if (!Program.m_ROM.FileExists(fileName))
                        return;

                    byte[] dl = MakeDynamicLibrary(codeDir, sourceDir);
                    if (dl == null)
                        return;

                    NitroFile file = Program.m_ROM.GetFileFromName(fileName);
                    file.m_Data = dl;
                    file.SaveChanges();

                    PatchCompiler.cleanPatch(codeDir);
                    break;

                case "hooks":
                    InsertHooks(codeDir, sourceDir);
                    break;

                case "test":
                    UpdateMakefileSources(codeDir, sourceDir);
                    PatchCompiler.compilePatch(0x02400000, codeDir);
                    PatchCompiler.cleanPatch(codeDir);
                    break;

                case "symbols":
                    UpdateMakefileSources(codeDir, sourceDir);
                    PatchCompiler.compilePatch(0x02400000, codeDir);
                    break;

                default:
                    return;
            }
        }

        public static string CheckDuplicateSymbols(DirectoryInfo codeDir)
        {
            IEnumerable<string> symbols = File.ReadAllLines(codeDir.FullName + "\\symbols.x");
            symbols = symbols.Where(s => s.Contains(" = 0x")).Select(s => s.Substring(0, s.IndexOf(' ')));

            List<string> checkedSymbols = new List<string>(symbols.Count());
            string duplicateSymbols = "";

            foreach (string symbol in symbols)
            {
                if (checkedSymbols.Contains(symbol) && !symbol.StartsWith("_ZThn80_N9AnimationD"))
                    duplicateSymbols += symbol + "\n";
                else
                    checkedSymbols.Add(symbol);
            }

            if (string.IsNullOrEmpty(duplicateSymbols))
                return null;

            return duplicateSymbols;
        }

        public static void InsertHooks(DirectoryInfo codeDir, string fileName)
        {
            string[] lines = File.ReadAllLines(codeDir.FullName + "\\" + fileName);
            
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] splitLine;
                if (line.Contains("#"))
                    splitLine = line.Substring(0, line.IndexOf('#')).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                else
                    splitLine = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (splitLine.Length < 3)
                    continue;

                uint hookAddr = Convert.ToUInt32(splitLine[0], 16);
                bool autorw;

                // data
                if (splitLine.Length == 3)
                {
                    uint data;

                    if (splitLine[2].StartsWith("0x") && splitLine[2].Length == 10)
                        data = Convert.ToUInt32(splitLine[2], 16);
                    else if (splitLine[2].Equals("nop"))
                        data = 0xe1a00000;
                    else
                        data = GetBranchAddr(codeDir, splitLine[2]);

                    autorw = Program.m_ROM.CanRW();
                    if (!autorw) Program.m_ROM.BeginRW();
                    Program.m_ROM.Write32(hookAddr - 0x02000000, data);
                    if (!autorw) Program.m_ROM.EndRW();
                    continue;
                }
                else if (splitLine.Length == 5 && splitLine[1] == "-")
                {
                    uint data = Convert.ToUInt32(splitLine[4], 16);
                    uint hookAddr2 = Convert.ToUInt32(splitLine[2], 16);

                    autorw = Program.m_ROM.CanRW();
                    if (!autorw) Program.m_ROM.BeginRW();
                    
                    for (uint addr = hookAddr; addr < hookAddr2; addr += 4)
                        Program.m_ROM.Write32(addr - 0x02000000, data);

                    if (!autorw) Program.m_ROM.EndRW();
                    continue;
                }

                uint branchAddr;

                if (splitLine[3].StartsWith("0x") && splitLine[3].Length == 10)
                    branchAddr = Convert.ToUInt32(splitLine[3], 16);
                else
                    branchAddr = GetBranchAddr(codeDir, splitLine[3]);

                string branchInstruction;
                string branchCondition;

                if (splitLine[2].Length < 3)
                {
                    branchInstruction = splitLine[2];
                    branchCondition = "";
                }
                else
                {
                    branchInstruction = splitLine[2].Substring(0, splitLine[2].Length - 2);
                    branchCondition = splitLine[2].Substring(splitLine[2].Length - 2, 2);
                }

                uint instruction;

                if (branchCondition == "eq") instruction = 0x0u << 28;
                else if (branchCondition == "ne") instruction = 0x1u << 28;
                else if (branchCondition == "cs") instruction = 0x2u << 28;
                else if (branchCondition == "cc") instruction = 0x3u << 28;
                else if (branchCondition == "mi") instruction = 0x4u << 28;
                else if (branchCondition == "pl") instruction = 0x5u << 28;
                else if (branchCondition == "vs") instruction = 0x6u << 28;
                else if (branchCondition == "vc") instruction = 0x7u << 28;
                else if (branchCondition == "hi") instruction = 0x8u << 28;
                else if (branchCondition == "ls") instruction = 0x9u << 28;
                else if (branchCondition == "ge") instruction = 0xau << 28;
                else if (branchCondition == "lt") instruction = 0xbu << 28;
                else if (branchCondition == "gt") instruction = 0xcu << 28;
                else if (branchCondition == "le") instruction = 0xdu << 28;
                else if (branchCondition == "nv") instruction = 0xfu << 28;
                else instruction = 0xeu << 28; // no condition

                if (branchInstruction == "bl") instruction += 0xbu << 24;
                else instruction += 0xau << 24; // regular b

                instruction += ((branchAddr - hookAddr - 8) >> 2) & 0x00ffffff;

                autorw = Program.m_ROM.CanRW();
                if (!autorw) Program.m_ROM.BeginRW();
                Program.m_ROM.Write32(hookAddr - 0x02000000, instruction);
                if (!autorw) Program.m_ROM.EndRW();
            }
        }

        private static uint GetBranchAddr(DirectoryInfo codeDir, string symbol)
        {
            string[] lines = File.ReadAllLines(codeDir.FullName + "\\symbols.x");

            foreach (string line in lines)
            {
                string[] splitLine = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (splitLine.Length < 3 || splitLine[0] != symbol)
                    continue;
                
                return Convert.ToUInt32(splitLine[2].Remove(splitLine[2].Length - 1), 16); // get rid of the ';'
            }

            throw new Exception("Symbol not found: " + symbol);
        }

        public static uint parseUHex(string s)
        {
            return uint.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }

        public static int parseHex(string s)
        {
            return int.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }

        public static void alignStream(Stream stream, int modulus)
        {
            byte[] zero = { 0x00 };
            while (stream.Position % modulus != 0)
                stream.Write(zero, 0, 1);
        }

        public static uint getDestOfBranch(int branchOpcode, uint srcAddr)
        {
            unchecked
            {
                return (uint)(((branchOpcode & 0x00ffffff) << 8 >> 6) + 8 + srcAddr);
            }
        }

        public static void MakeOverlay(uint ovID, DirectoryInfo codeDir)
        {
            FileInfo f = new FileInfo(codeDir.FullName + "/newcode.bin");
            if (!f.Exists) return;
            FileStream fs = f.OpenRead();
            FileInfo symFile = new FileInfo(codeDir.FullName + "/newcode.sym");
            StreamReader symStr = symFile.OpenText();

            byte[] newdata = new byte[fs.Length];
            fs.Read(newdata, 0, (int)fs.Length);
            fs.Close();


            BinaryWriter newOvl = new BinaryWriter(new MemoryStream());
            BinaryReader newOvlR = new BinaryReader(newOvl.BaseStream);

            try
            {
                newOvl.Write(newdata);
                alignStream(newOvl.BaseStream, 4);

                uint staticInitCount = 0;

                while (!symStr.EndOfStream)
                {
                    string line = symStr.ReadLine();

                    if (line.Contains("_Z4initv")) //gcc name mangling of init()
                    {
                        uint addr = (uint)parseHex(line.Substring(0, 8));
                        newOvl.Write(addr);
                        ++staticInitCount;
                    }
                }

                NitroOverlay ovl = new NitroOverlay(Program.m_ROM, ovID);
                newOvl.BaseStream.Position = 0;
                ovl.SetInitializer(ovl.GetRAMAddr() + (uint)newOvl.BaseStream.Length - 4 * staticInitCount,
                    4 * staticInitCount);
                ovl.SetSize((uint)newOvl.BaseStream.Length);
                ovl.WriteBlock(0, newOvlR.ReadBytes((int)newOvl.BaseStream.Length));
                ovl.SaveChanges();
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox("Error", ex).ShowDialog();
                return;
            }
            finally
            {
                symStr.Close();
                newOvl.Dispose();
                newOvlR.Close();
            }
        }

        private static (uint, uint)? GetInitAndCleanup(DirectoryInfo codeDir)
        {
            StreamReader symbolFile = null;
            uint initFuncOffset = 0;
            uint cleanFuncOffset = 0;

            try
            {
                symbolFile = new StreamReader(new FileStream(codeDir + "/newcode.sym", FileMode.Open));

                while (!symbolFile.EndOfStream)
                {
                    string line = symbolFile.ReadLine();

                    if (line.Length < 32)
                        continue;

                    string symbol = line.Substring(31);

                    if (symbol == " _Z4initv")
                    {
                        initFuncOffset = uint.Parse(line.Substring(0, 8),
                            System.Globalization.NumberStyles.HexNumber);

                        if (cleanFuncOffset != 0)
                            return (initFuncOffset, cleanFuncOffset);
                    }
                    else if (symbol == " _Z7cleanupv")
                    {
                        cleanFuncOffset = uint.Parse(line.Substring(0, 8),
                            System.Globalization.NumberStyles.HexNumber);

                        if (initFuncOffset != 0)
                            return (initFuncOffset, cleanFuncOffset);
                    }
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox("An error occurred while reading newcode.sym", ex).ShowDialog();

                return null;
            }
            finally
            {
                if (symbolFile != null)
                    symbolFile.Close();
            }

            if (initFuncOffset == 0)
            {
                if (cleanFuncOffset == 0)
                    MessageBox.Show("Generating DL failed: init and cleanup functions missing");
                else
                    MessageBox.Show("Generating DL failed: init function missing");
            }
            else
                MessageBox.Show("Generating DL failed: cleanup function missing");

            return null;
        }

        public static byte[] MakeDynamicLibrary(DirectoryInfo codeDir, string sourceDir)
        {
            try
            {
                const uint baseAddress = 0x02400000;

                UpdateMakefileSources(codeDir, sourceDir);
                string make = "(make CODEADDR=0x" + baseAddress.ToString("X8")
                     + " && make CODEADDR=0x" + (baseAddress + 4).ToString("X8")
                     + " TARGET=newcode1)";
                if (PatchCompiler.runProcess(make, codeDir.FullName) != 0)
                    return null;

                byte[] code0 = File.ReadAllBytes(codeDir.FullName + "/newcode.bin");
                byte[] code1 = File.ReadAllBytes(codeDir.FullName + "/newcode1.bin");

                if (code0.Length != code1.Length)
                {
                    MessageBox.Show("Generating DL failed: code lengths don't match");

                    return null;
                }

                MemoryStream outputStream = new MemoryStream();
                BinaryWriter output = new BinaryWriter(outputStream);
                List<ushort> relocations = new List<ushort>();

                output.Write((ulong)0);
                output.Write((ulong)0);

                uint alignedCodeSize = (uint)code0.Length & ~3U;
                for (ushort i = 0; i < alignedCodeSize; i += 4)
                {
                    uint word0 = BitConverter.ToUInt32(code0, i);
                    uint word1 = BitConverter.ToUInt32(code1, i);

                    if (word0 == word1)
                    {
                        output.Write(word0);
                    }
                    else if (word0 + 4 == word1) // word0 and word1 are pointers
                    {
                        output.Write(word0 - baseAddress + 0x10);

                        relocations.Add(i);
                    }
                    else if (word0 == word1 + 1 && word0 >> 24 == word1 >> 24) // word0 and word1 are branches
                    {
                        uint destAddr = getDestOfBranch((int)word0, baseAddress + i);

                        output.Write((destAddr >> 2) | (word0 & 0xff000000));

                        relocations.Add(i);
                    }
                    else
                    {
                        MessageBox.Show("Generating DL failed: code files don't match for an unknown reason\nnewcode.bin offset: 0x"
                             + i.ToString("X4") + "\nmismatching words: 0x"
                             + word0.ToString("X8") + " and 0x" + word1.ToString("X8"));

                        return null;
                    }
                }

                for (uint i = alignedCodeSize; i < code0.Length; ++i)
                    output.Write(code0[i]);

                alignStream(output.BaseStream, 4);

                var relocationOffset = output.BaseStream.Position;
                var addresses = GetInitAndCleanup(codeDir);
                if (addresses == null) return null;

                uint initFuncOffset = (((uint, uint))addresses).Item1 - baseAddress + 0x10;
                uint cleanFuncOffset = (((uint, uint))addresses).Item2 - baseAddress + 0x10;

                output.Seek(0, SeekOrigin.Begin);

                output.Write((ushort)relocations.Count);
                output.Write((ushort)relocationOffset);
                output.Write((ushort)initFuncOffset);
                output.Write((ushort)cleanFuncOffset);

                output.Seek(0, SeekOrigin.End);

                foreach (ushort relocation in relocations)
                    output.Write((ushort)(relocation + 0x10));

                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox("Generating DL failed:", ex).ShowDialog();

                return null;
            }
        }

        public static void UpdateSymbols(DirectoryInfo codeDir, string title)
        {
            List<string> symbols = File.ReadAllLines(codeDir.FullName + "symbols.x").ToList();
            string[] unformattedSymbols = File.ReadAllLines(codeDir.FullName + "newcode.sym");

            symbols.Add("");
            symbols.Add("/* " + title + ": */");

            foreach (string symbol in unformattedSymbols)
            {
                string[] data = symbol.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 5 || data.Length == 6 || (data.Length == 4 && (data.Last().StartsWith("nsub_") || data.Last().StartsWith("repl_")) && !symbols.Select(s => s.Split(' ').First()).Contains(data.Last())))
                {
                    string symbolName = data.Last();
                    if (!symbolName.StartsWith(".") && !symbolName.Contains("*ABS*") && !symbolName.Contains(".cpp") && !symbolName.Contains(".o"))
                    {
                        uint addr = uint.Parse(data[0], System.Globalization.NumberStyles.HexNumber);
                        string spaces = symbolName.Length >= 82 ? " " : new string(' ', 82 - symbolName.Length);
                        symbols.Add(symbolName + spaces + "= " + "0x" + Convert.ToString(addr, 16).PadLeft(8, '0').ToLower() + ";");
                    }
                }
            }
            
            File.WriteAllLines(codeDir.FullName + "symbols.x", symbols);
        }

        public static void UpdateMakefileSources(DirectoryInfo codeDir, string sourceDir)
        {
            string[] lines = File.ReadAllLines(codeDir.FullName + "Makefile");

            for (int i = 0; i < lines.Length; i++) if (lines[i].StartsWith("SOURCES  := "))
                    lines[i] = "SOURCES  := " + sourceDir + " libfat_source";

            File.WriteAllLines(codeDir.FullName + "Makefile", lines);
        }

        class FreeSection
        {
            public uint Address; // subtract 0x02004000 to get offset in arm9.bin
            public uint Size;
            public string Description; // to keep track of what these sections are
            public uint UsedSize = 0;

            public uint GetEndOffset()
            {
                return Address + Size;
            }

            public uint GetFirstFreeOffset()
            {
                return Address + UsedSize;
            }
        }

        class CodeBlock
        {
            public uint Address = 0x02400000;
            public uint Size = 0;
            public string Directory;
        }

        private static readonly FreeSection[] freeArm9SectionsC =
        {
            new FreeSection { Address = 0x02075f14, Size = 0x0000c210, Description = "ROM Embedded SPA File" },
            //new FreeSection { Address = 0x0202cc0c, Size = 0x00000a84, Description = "Stage::InitResources" },
            //new FreeSection { Address = 0x0202c9a8, Size = 0x00000264, Description = "Stage::CleanupResources" },
            //new FreeSection { Address = 0x0202bbbc, Size = 0x000006f0, Description = "Stage::Behavior" },
            //new FreeSection { Address = 0x0202b8a4, Size = 0x00000318, Description = "Stage::Render" },
        };

        private static List<FreeSection> sections;
        private static List<FreeSection> combinedSections;
        private static List<CodeBlock> codeBlocks;

        static FreeSection[] OverlappingSections()
        {
            for (int i = 0; i < sections.Count() - 1; i++)
            {
                uint start1 = sections[i].Address;
                uint end1 = sections[i].GetEndOffset();
                uint start2 = sections[i + 1].Address;
                uint end2 = sections[i + 1].GetEndOffset();

                if ((start1 < start2 && start2 < end1) || (start1 < end2 && end2 < end1))
                    return new FreeSection[] { sections[i], sections[i + 1] };
            }

            return null;
        }

        static void CombineFreeArm9Sections()
        {
            combinedSections = new List<FreeSection>();
            int numToCombine = 1;
            int combineStartIndex = 0;

            for (int i = 0; i < sections.Count(); i++)
            {
                // is next section right after the current one?
                if (i < sections.Count() - 1 && sections[i].GetEndOffset() == sections[i + 1].Address)
                {
                    numToCombine++;
                    continue;
                }

                // next section is not right after the current one, merge the previous sections that need to be merged
                FreeSection combinedSection = new FreeSection();
                combinedSection.Address = sections[combineStartIndex].Address;

                uint newSectionSize = 0;
                string newSectionDescription = "";
                for (int j = combineStartIndex; j < combineStartIndex + numToCombine; j++)
                {
                    newSectionSize += sections[j].Size;
                    newSectionDescription += sections[j].Description + (j != combineStartIndex + numToCombine - 1 ? ", " : "");
                }

                combinedSection.Size = newSectionSize;
                combinedSection.Description = newSectionDescription;

                combinedSections.Add(combinedSection);

                numToCombine = 1;
                combineStartIndex = i + 1;
            }

            combinedSections.Sort((a, b) => a.Address.CompareTo(b.Address));
        }

        static FreeSection FindFirstSection(uint minSize)
        {
            foreach (FreeSection section in combinedSections)
            {
                if (section.UsedSize + minSize < section.Size)
                    return section;
            }

            throw new Exception("Not enough space to allocate all blocks.");
        }

        static void AllocateCodeBlocks()
        {
            foreach (CodeBlock codeBlock in codeBlocks)
            {
                FreeSection section = FindFirstSection(codeBlock.Size);
                codeBlock.Address = section.GetFirstFreeOffset();
                section.UsedSize += codeBlock.Size;
            }

            codeBlocks.Sort((a, b) => a.Address.CompareTo(b.Address));
        }

        private static void CompileArm9(DirectoryInfo codeDir, string sourceDir)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(codeDir.FullName + "\\" + sourceDir);
            codeBlocks = new List<CodeBlock>();

            sections = freeArm9SectionsC.ToList();
            sections.Sort((a, b) => a.Address.CompareTo(b.Address));

            // check for overlapping sections
            FreeSection[] overlappingSections = OverlappingSections();
            if (overlappingSections != null)
            {
                Console.WriteLine("Error! Overlapping sections found:");
                for (int i = 0; i < overlappingSections.Length; i++)
                    Console.WriteLine(overlappingSections[i]);

                throw new Exception("Error! Overlapping sections found!");
            }

            // combine the free sections
            CombineFreeArm9Sections();

            // precompile all directories to get the size of the code blocks
            foreach (DirectoryInfo curDir in directoryInfo.GetDirectories())
            {
                string curSourceDir = sourceDir + "\\" + curDir.Name;

                UpdateMakefileSources(codeDir, curSourceDir);
                PatchCompiler.compilePatch(0x02400000, codeDir);
                uint size = (uint)File.ReadAllBytes(codeDir.FullName + "\\newcode.bin").Length;
                size += size % 4;
                PatchCompiler.cleanPatch(codeDir);

                codeBlocks.Add(new CodeBlock { Directory = curSourceDir, Address = 0x02400000, Size = size });
            }

            // sort the codeblocks by size
            codeBlocks.Sort((a, b) => a.Size.CompareTo(b.Size));
            codeBlocks.Reverse();

            // allocate all code blocks on the free arm9 sections
            AllocateCodeBlocks();
            
            // compile and insert the newly allocated code blocks
            foreach (CodeBlock codeBlock in codeBlocks)
            {
                UpdateMakefileSources(codeDir, codeBlock.Directory);
                PatchCompiler.compilePatch(codeBlock.Address, codeDir);
                byte[] data = File.ReadAllBytes(codeDir.FullName + "\\newcode.bin");

                bool autorw = Program.m_ROM.CanRW();
                if (!autorw) Program.m_ROM.BeginRW();
                Program.m_ROM.WriteBlock(codeBlock.Address - 0x02000000, data);
                if (!autorw) Program.m_ROM.EndRW();

                UpdateSymbols(codeDir, "Symbols from arm9 patch (" + codeBlock.Directory + ")");
                PatchCompiler.cleanPatch(codeDir);
            }
        }
    }
}
