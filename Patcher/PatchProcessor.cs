﻿using System;
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
        #region Compile Command Processing
        private static string RemoveFirstAndLastChars(string s)
        {
            return s.Remove(0, 1).Remove(s.Length - 2, 1);
        }

        public static string Process(DirectoryInfo codeDir, string[] p, bool hideConsoleWindow)
        {
            PatchCompiler.HideConsoleWindow = hideConsoleWindow;

            if (p.Length < 2)
                throw new IndexOutOfRangeException("Not enough arguments supplied.");

            int minLength = p[1] == "ov_info" || p[1] == "make_decomp" ? 3 : p[1] == "dl" || p[1] == "overlay" || p[1] == "levels" ? 5 : 4;

            if (p.Length < minLength)
                throw new IndexOutOfRangeException("Not enough arguments supplied.");

            string sourceDir = string.Join(" ", p).Split('\"').Where(s => !string.IsNullOrWhiteSpace(s)).Last();
            // string sourceDir = RemoveFirstAndLastChars(p[minLength - 1]);
            string codeSubDir = RemoveFirstAndLastChars(p[minLength - 2]);
            

            codeDir = new DirectoryInfo(codeDir.FullName + codeSubDir + "\\");

            switch (p[1])
            {
                case "arm9":
                    return CompileArm9(codeDir, sourceDir);

                case "overlay":
                    uint ovID = Convert.ToUInt32(p[2]);
                    uint addr = new NitroOverlay(Program.m_ROM, ovID).GetRAMAddr();
                    UpdateMakefileSources(codeDir, sourceDir);
                    PatchCompiler.compilePatch(addr, codeDir);
                    MakeOverlay(ovID, codeDir);
                    UpdateSymbols(codeDir, "Symbols from overlay " + ovID);
                    PatchCompiler.cleanPatch(codeDir);
                    return "Successfully compiled overlay " + ovID + ".\n" + sourceDir;

                case "dl":
                    string fileName = RemoveFirstAndLastChars(p[2]);
                    if (!Program.m_ROM.FileExists(fileName))
                        throw new Exception("Couldn't find file '" + fileName + "' in ROM.");

                    byte[] dl = MakeDynamicLibrary(codeDir, sourceDir);
                    if (dl == null)
                        throw new Exception("DL generation failed.");

                    NitroFile file = Program.m_ROM.GetFileFromName(fileName);
                    file.m_Data = dl;
                    file.SaveChanges();

                    PatchCompiler.cleanPatch(codeDir);
                    return "Successfully compiled \n" + fileName + "\n" + sourceDir;

                case "levels":
                    throw new Exception("Should be handled by the form.");

                case "hooks":
                    InsertHooks(codeDir, sourceDir);
                    return "Successfully run hook script\n" + sourceDir;

                case "test":
                    UpdateMakefileSources(codeDir, sourceDir);
                    PatchCompiler.compilePatch(0x02400000, codeDir);
                    if (!File.Exists(codeDir.FullName + "\\newcode.bin")) throw new Exception("Code didn't compile successfully.\nRetry for more details.");
                    PatchCompiler.cleanPatch(codeDir);
                    return "Successfully compiled test\n" + sourceDir;

                case "symbols":
                    UpdateMakefileSources(codeDir, sourceDir);
                    PatchCompiler.compilePatch(0x02400000, codeDir);
                    string symbols = string.Join("\n", GetSymbols(codeDir));
                    PatchCompiler.cleanPatch(codeDir);
                    return "Successfully compiled symbols in " + sourceDir + ":\n" + symbols;

                case "ov_info":
                    OvInfoMaker.Run(sourceDir);
                    return "Successfully written overlay infos.";

                case "make_decomp":
                    ObjectDecompMaker.Run(sourceDir);
                    return "Successfully initialized decomp.";

                default:
                    throw new Exception("Unknown command type '" + p[1] + "'.");
            }
        }
        #endregion

        #region Check Duplicate Symbols
        public static string CheckDuplicateSymbols(string basePath, string symFile)
        {
            IEnumerable<string> symbols = File.ReadAllLines(basePath + symFile);
            symbols = symbols.Where(s => s.Contains(" = 0x")).Select(s => s.Substring(0, s.IndexOf(' ')));

            List<string> checkedSymbols = new List<string>(symbols.Count());
            string duplicateSymbols = "";

            foreach (string symbol in symbols)
            {
                if (checkedSymbols.Contains(symbol) && !symbol.StartsWith("_ZN12_GLOBAL__N_") && !symbol.StartsWith("_ZThn80_N9AnimationD"))
                    duplicateSymbols += symbol + "\n";
                else
                    checkedSymbols.Add(symbol);
            }

            if (string.IsNullOrEmpty(duplicateSymbols))
                return null;

            return duplicateSymbols;
        }
        #endregion

        #region Insert Hooks

        public static void InsertHooks(DirectoryInfo codeDir, string fileName)
        {
            int overlayID = -1;
            int prevOverlayID = -1;

            NitroOverlay overlay = null;

            string[] lines = File.ReadAllLines(codeDir.FullName + "\\" + fileName);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                bool autorw;

                string[] splitLine;
                if (line.Contains("#"))
                    splitLine = line.Substring(0, line.IndexOf('#')).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                else
                    splitLine = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (splitLine.Length == 2 && splitLine[0] == "set_overlay")
				{
                    prevOverlayID = overlayID;
                    overlayID = Convert.ToInt32(splitLine[1], 10);

                    // overlay changed
                    if (overlayID != prevOverlayID)
					{
                        if (overlay != null)
						{
                            autorw = Program.m_ROM.CanRW();
                            if (autorw) Program.m_ROM.EndRW();

                            overlay.SaveChanges();

                            if (autorw) Program.m_ROM.BeginRW();
                        }

                        if (overlayID != -1)
                            overlay = new NitroOverlay(Program.m_ROM, (uint)overlayID);
                        else
                            overlay = null;
					}

                    continue;
                }

                if (splitLine.Length < 3)
                    continue;

                uint hookAddr = Convert.ToUInt32(splitLine[0], 16);

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

                    if (overlayID == -1)
                    {
                        autorw = Program.m_ROM.CanRW();
                        if (!autorw) Program.m_ROM.BeginRW();
                        Program.m_ROM.Write32(hookAddr - 0x02000000, data);
                        if (!autorw) Program.m_ROM.EndRW();
                    }
                    else
					{
                        overlay.Write32(hookAddr - overlay.GetRAMAddr(), data);
					}

                    continue;
                }
                else if (splitLine.Length == 5 && splitLine[1] == "-")
                {
                    uint data = Convert.ToUInt32(splitLine[4], 16);
                    uint hookAddr2 = Convert.ToUInt32(splitLine[2], 16);

                    if (overlayID == -1)
                    {
                        autorw = Program.m_ROM.CanRW();
                        if (!autorw) Program.m_ROM.BeginRW();

                        for (uint addr = hookAddr; addr < hookAddr2; addr += 4)
                            Program.m_ROM.Write32(addr - 0x02000000, data);

                        if (!autorw) Program.m_ROM.EndRW();
                    }
                    else
					{
                        for (uint addr = hookAddr; addr < hookAddr2; addr += 4)
                            overlay.Write32(addr - overlay.GetRAMAddr(), data);
                    }

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

                if (overlayID == -1)
                {
                    autorw = Program.m_ROM.CanRW();
                    if (!autorw) Program.m_ROM.BeginRW();
                    Program.m_ROM.Write32(hookAddr - 0x02000000, instruction);
                    if (!autorw) Program.m_ROM.EndRW();
                }
                else
                {
                    overlay.Write32(hookAddr - overlay.GetRAMAddr(), instruction);
                }
            }


            if (overlay != null)
            {
                bool autorw = Program.m_ROM.CanRW();
                if (autorw) Program.m_ROM.EndRW();

                overlay.SaveChanges();

                if (autorw) Program.m_ROM.BeginRW();
            }
        }
        #endregion

        #region Utility
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

        public static void BackupBuildFiles(string codeDir, string dirName, bool isDL = false)
        {
            /*dirName = dirName.Replace('\\', '-').Replace('/', '-');

            string buildDir = codeDir + "build/";
            string savedBuildDir = codeDir + "build_saved/";
            string savedBuildBuildDir = savedBuildDir + dirName + "-build/";

            if (!Directory.Exists(buildDir))
                return;

            List<string> newcodeFiles = new List<string>(new string[] { "newcode.bin", "newcode.elf", "newcode.sym" });

            if (isDL)
            {
                newcodeFiles.Add("newcode1.bin");
                newcodeFiles.Add("newcode1.elf");
                newcodeFiles.Add("newcode1.sym");
            }

            if (!Directory.Exists(savedBuildDir))
                Directory.CreateDirectory(savedBuildDir);

            if (Directory.Exists(savedBuildBuildDir))
                Directory.Delete(savedBuildBuildDir, true);
            
            Directory.CreateDirectory(savedBuildBuildDir);

            foreach (string file in Directory.GetFiles(buildDir))
                File.Copy(file, Path.Combine(savedBuildBuildDir, Path.GetFileName(file)));

            foreach (string file in newcodeFiles)
            {
                string newFileName = Path.Combine(savedBuildDir, dirName + "-" + file);
                string sourceFileName = codeDir + file;

                if (File.Exists(newFileName))
                    File.Delete(newFileName);

                if (!File.Exists(sourceFileName))
                    continue;

                File.Copy(sourceFileName, newFileName);
            }*/
        }

        public static void RestoreBuildFiles(string codeDir, string dirName, bool isDL = false)
        {
            /*dirName = dirName.Replace('\\', '-').Replace('/', '-');

            string buildDir = codeDir + "build/";
            string savedBuildDir = codeDir + "build_saved/";
            string savedBuildBuildDir = savedBuildDir + dirName + "-build/";

            if (!Directory.Exists(savedBuildDir) || !Directory.Exists(savedBuildBuildDir))
                return;

            List<string> newcodeFiles = new List<string>(new string[] { "newcode.bin", "newcode.elf", "newcode.sym" });

            if (isDL)
            {
                newcodeFiles.Add("newcode1.bin");
                newcodeFiles.Add("newcode1.elf");
                newcodeFiles.Add("newcode1.sym");
            }

            if (Directory.Exists(buildDir))
                Directory.Delete(buildDir, true);
            
            Directory.CreateDirectory(buildDir);

            foreach (string file in Directory.GetFiles(savedBuildBuildDir))
                File.Copy(file, Path.Combine(buildDir, Path.GetFileName(file)));

            foreach (string file in newcodeFiles)
			{
                string sourceFileName = Path.Combine(savedBuildDir, dirName + "-" + file);
                string newFileName = codeDir + file;

                if (File.Exists(newFileName))
                    File.Delete(newFileName);

                if (!File.Exists(sourceFileName))
                    continue;

                File.Copy(sourceFileName, newFileName);
            }*/
        }
        #endregion

        #region Compile Overlay
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
                throw new Exception("Compiling overlay" + ovID + " (" + codeDir.FullName + ") failed:\n" + ex.Message);
            }
            finally
            {
                symStr.Close();
                newOvl.Dispose();
                newOvlR.Close();
            }
        }
        #endregion

        #region Compile DL
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
                throw new Exception("An error occurred while reading newcode.sym:\n" + ex);
            }
            finally
            {
                if (symbolFile != null)
                    symbolFile.Close();
            }

            if (initFuncOffset == 0)
            {
                if (cleanFuncOffset == 0)
                    throw new Exception("Generating DL failed: init and cleanup functions missing.");
                else
                    throw new Exception("Generating DL failed: init function missing.");
            }
            else
                throw new Exception("Generating DL failed: cleanup function missing.");
        }

        public static byte[] MakeDynamicLibrary(DirectoryInfo codeDir, string sourceDir)
        {
            try
            {
                const uint baseAddress = 0x02400000;

                UpdateMakefileSources(codeDir, sourceDir);
                RestoreBuildFiles(codeDir.FullName + '\\', sourceDir, true);

                string make = "(make CODEADDR=0x" + baseAddress.ToString("X8")
                     + " && make CODEADDR=0x" + (baseAddress + 4).ToString("X8")
                     + " TARGET=newcode1)";
                if (PatchCompiler.runProcess(make, codeDir.FullName) != 0)
                    return null;

                BackupBuildFiles(codeDir.FullName + '\\', sourceDir, true);

                byte[] code0 = File.ReadAllBytes(codeDir.FullName + "/newcode.bin");
                byte[] code1 = File.ReadAllBytes(codeDir.FullName + "/newcode1.bin");

                if (code0.Length != code1.Length)
                    throw new Exception("Generating DL failed: code lengths don't match");

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
                        throw new Exception("Generating DL failed: code files don't match for an unknown reason\nnewcode.bin offset: 0x"
                             + i.ToString("X4") + "\nmismatching words: 0x"
                             + word0.ToString("X8") + " and 0x" + word1.ToString("X8"));
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
                throw new Exception("Generating DL failed:\n" + ex.Message);
            }
        }
        #endregion

        #region Update Symbols
        public static void UpdateSymbols(DirectoryInfo codeDir, string title)
        {
            List<string> symbols = File.ReadAllLines(codeDir.FullName + "symbols.x").ToList();
            string[] unformattedSymbols = File.ReadAllLines(codeDir.FullName + "newcode.sym");

            symbols.Add("");
            symbols.Add("/* " + title + ": */");

            List<string> newSymbols = new List<string>();

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
                        newSymbols.Add(symbolName + spaces + "= " + "0x" + Convert.ToString(addr, 16).PadLeft(8, '0').ToLower() + ";");
                    }
                }
            }

            symbols.AddRange(newSymbols.OrderBy(s => parseUHex(s.Substring(s.Length - 9, 8))));

            File.WriteAllLines(codeDir.FullName + "symbols.x", symbols);
        }
        #endregion

        #region Compile Symbols
        public static List<string> GetSymbols(DirectoryInfo codeDir)
        {
            List<string> symbols = new List<string>();
            string[] unformattedSymbols = File.ReadAllLines(codeDir.FullName + "newcode.sym");

            foreach (string symbol in unformattedSymbols)
            {
                string[] data = symbol.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 5 || data.Length == 6 || (data.Length == 4 && (data.Last().StartsWith("nsub_") || data.Last().StartsWith("repl_")) && !symbols.Select(s => s.Split(' ').First()).Contains(data.Last())))
                {
                    string symbolName = data.Last();
                    if (!symbolName.StartsWith(".") && !symbolName.Contains("*ABS*") && !symbolName.Contains(".cpp") && !symbolName.Contains(".o"))
                    {
                        //uint addr = uint.Parse(data[0], System.Globalization.NumberStyles.HexNumber);
                        //string spaces = symbolName.Length >= 82 ? " " : new string(' ', 82 - symbolName.Length);
                        //symbols.Add(symbolName + spaces + "= " + "0x" + Convert.ToString(addr, 16).PadLeft(8, '0').ToLower() + ";");
                        symbols.Add(symbolName);
                    }
                }
            }

            return symbols;
        }
        #endregion

        #region Update Makefile Sources Path
        public static void UpdateMakefileSources(DirectoryInfo codeDir, string sourceDir)
        {
            string[] lines = File.ReadAllLines(codeDir.FullName + "Makefile");

            for (int i = 0; i < lines.Length; i++) if (lines[i].StartsWith("SOURCES  := "))
                    lines[i] = "SOURCES  := " + sourceDir;

            File.WriteAllLines(codeDir.FullName + "Makefile", lines);
        }
        #endregion

        #region Compile arm9
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

			public override string ToString()
			{
                return "(0x" + Convert.ToString(Address, 16).ToLower() + "): 0x" + Convert.ToString(UsedSize, 16).ToLower() + "/0x" + Convert.ToString(Size, 16).ToLower() + " (" + Math.Round((double)UsedSize / Size * 100, 2) + "%)";
			}
		}

        class CodeBlock
        {
            public uint Address = 0x02400000;
            public uint Size = 0;
            public string Directory;
        }

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

        private static List<FreeSection> GetFreeArm9Sections(string path)
		{
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            string[] lines = File.ReadAllLines(path);
            List<FreeSection> sections = new List<FreeSection>();

            foreach (string line in lines)
			{
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                    continue;

                string[] args = line.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                uint address = uint.Parse(args[0].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                uint size = uint.Parse(args[1].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);

                sections.Add(new FreeSection { Address = address, Size = size, Description = args[2] });
            }

            return sections;
		}

        private static string[] GetSubDirsInOrder(string path, string sourceDir)
		{
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            string sourceDirModified = ' ' + sourceDir + '\\';

            string[] lines = File.ReadAllLines(path);
            string[] orderedLines = lines.Where(l => !l.StartsWith("#")).Select(l => sourceDir + '\\' + l.Replace(" ", sourceDirModified)).ToArray();

            return orderedLines;
		}

        private static string CompileArm9(DirectoryInfo codeDir, string sourceDir)
        {
            string ret = "";
            string directoryPath = codeDir.FullName + "\\" + sourceDir;
            string[] subDirs = GetSubDirsInOrder(directoryPath + "/order.txt", sourceDir);

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            codeBlocks = new List<CodeBlock>();

            sections = GetFreeArm9Sections(directoryPath + "/sections.txt");
            sections.Sort((a, b) => a.Address.CompareTo(b.Address));

            // check for overlapping sections
            FreeSection[] overlappingSections = OverlappingSections();
            if (overlappingSections != null)
                throw new Exception("Overlapping sections found:" + string.Join("\n", overlappingSections.Select(s => s.ToString())));

            // combine the free sections
            CombineFreeArm9Sections();

            string curSourceDir = "";

            try
            {
                // precompile all directories to get the size of the code blocks
                foreach (string subDir in subDirs)
                {
                    curSourceDir = subDir;

                    UpdateMakefileSources(codeDir, curSourceDir);
                    RestoreBuildFiles(codeDir.FullName + '\\', curSourceDir);
                    PatchCompiler.compilePatch(0x02400000, codeDir);

                    uint size = (uint)File.ReadAllBytes(codeDir.FullName + "\\newcode.bin").Length;
                    size += size % 4;
                    size += 0x10; // because the size is sometimes incorrect, this is not that expensive and doesn't require changing the entire build system

                    UpdateSymbols(codeDir, "Temporary Symbols from arm9 patch (" + curSourceDir + ")");
                    BackupBuildFiles(codeDir.FullName + '\\', curSourceDir);
                    PatchCompiler.cleanPatch(codeDir);

                    // ret += "Precompiled arm9 section '" + curSourceDir + "'.\n";

                    codeBlocks.Add(new CodeBlock { Directory = subDir, Address = 0x02400000, Size = size });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Precomping arm9 section '" + curSourceDir + "' failed:\n" + ex.Message);
            }

            // reset symbols.x
            File.WriteAllBytes(codeDir.FullName + "/symbols.x", File.ReadAllBytes(codeDir.FullName + "/symbols.bak.x"));

            // sort the codeblocks by size
            codeBlocks.Sort((a, b) => a.Size.CompareTo(b.Size));
            codeBlocks.Reverse();

            // allocate all code blocks on the free arm9 sections
            AllocateCodeBlocks();

            // compile and insert the newly allocated code blocks
            try
            {
                foreach (string subDir in subDirs)
                {
                    CodeBlock codeBlock = codeBlocks.Where(c => c.Directory == subDir).First();
                    curSourceDir = subDir;

                    UpdateMakefileSources(codeDir, curSourceDir);
                    RestoreBuildFiles(codeDir.FullName + '\\', curSourceDir);
                    PatchCompiler.compilePatch(codeBlock.Address, codeDir);

                    byte[] data = File.ReadAllBytes(codeDir.FullName + "\\newcode.bin");

                    if (data.Length > codeBlock.Size)
                        throw new Exception($"{subDir} size was {data.Length}, expected {codeBlock.Size} at most");

                    Array.Resize(ref data, (int)codeBlock.Size);

                    bool autorw = Program.m_ROM.CanRW();
                    if (!autorw) Program.m_ROM.BeginRW();
                    Program.m_ROM.WriteBlock(codeBlock.Address - 0x02000000, data);
                    if (!autorw) Program.m_ROM.EndRW();

                    UpdateSymbols(codeDir, "Symbols from arm9 patch (" + codeBlock.Directory + ")");
                    BackupBuildFiles(codeDir.FullName + '\\', curSourceDir);
                    PatchCompiler.cleanPatch(codeDir);

                    // ret += "Compiled and inserted arm9 section '" + codeBlock.Directory + "' at 0x" + Convert.ToString(codeBlock.Address, 16).ToLower() + " with size 0x" + Convert.ToString(codeBlock.Size, 16).ToLower() + ".\n";
                }

                ret += "\nAll code blocks compiled and inserted.\narm9 section information:";

                foreach (FreeSection section in combinedSections)
                    ret += "\n" + section;

                foreach (FreeSection section in sections)
                    section.UsedSize = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Compiling arm9 section '" + curSourceDir + "' failed:\n" + ex.Message);
            }

            return ret;
        }
		#endregion

		#region Update no$gba Symbols
        public static List<string> UpdateNoCashSymbols(string basePath, string symFile)
		{
            List<string> ret = new List<string>();

            string symPath = Program.m_ROMPath.Replace(".nds", ".sym");

            string[] oldLines = File.ReadAllLines(basePath + symFile);
            bool inComment = false;

            // assumes comments start at the beginning of a line and end at the ending of a line
            for (int i = 0; i < oldLines.Length; i++)
            {
                if (oldLines[i].Contains("/*") || inComment)
                {
                    inComment = true;

                    if (oldLines[i].Contains("*/"))
                        inComment = false;

                    oldLines[i] = "";
                }
            }

            List<string> lines = new List<string>();

            for (int i = 0; i < oldLines.Length; i++)
            {
                string oldLine = oldLines[i];
                if (string.IsNullOrWhiteSpace(oldLine))
                    continue;

                string mangledSymbol = GetSymbolName(oldLine);
                string symbolAddress = GetSymbolAddress(oldLine);
                // Console.WriteLine("Attempting symbol: " + mangledSymbol);
                string demangledSymbol;

                try
                {
                     demangledSymbol = DemangleSymbol(mangledSymbol);
                }
                catch
				{
                    demangledSymbol = mangledSymbol;
                    ret.Add(mangledSymbol);
                }

                lines.Add(symbolAddress + " " + demangledSymbol.Replace("const ", ""));
            }

            File.WriteAllLines(symPath, lines);

            return ret;
        }

        static string GetSymbolName(string line)
        {
            int nameLength = line.IndexOf(' ');
            return line.Substring(0, nameLength);
        }

        static string GetSymbolAddress(string line)
        {
            int addressStart = line.IndexOf('=') + 4; // remove the "= 0x"
            return line.Substring(addressStart, 8).ToUpper();
        }

        static bool IsCharNumeric(char theChar)
        {
            return theChar >= 0x30 && theChar <= 0x39;
        }

        static int CountNumsAt(string theString, int startIndex = 0)
        {
            for (int i = startIndex; i < theString.Length; i++)
            {
                if (!IsCharNumeric(theString[i]))
                    return i - startIndex;
            }

            return 0;
        }

        static int CountAt(string theString, char toCount, int startIndex = 0)
        {
            for (int i = startIndex; i < theString.Length; i++)
            {
                if (theString[i] != toCount)
                    return i - startIndex;
            }

            return 0;
        }

        static string GetStandardType(string args)
        {
            switch (args[0])
            {
                case 'v':
                    return "void";
                case 'b':
                    return "bool";
                case 'c':
                    return "char";
                case 'h':
                    return "u8";
                case 'a':
                    return "s8";
                case 't':
                    return "u16";
                case 's':
                    return "s16";
                case 'j':
                    return "u32";
                case 'i':
                    return "s32";
                case 'y':
                    return "u64";
                /*case '':
                    return "s64";*/
                default:
                    throw new Exception("Unknown argument type '" + args[0] + "'.");
            }
        }

        static int GetIndexOfEndE(string args, bool removeS_check = false)
        {
            int level = 0;

            for (int i = 0; i < args.Length; i++)
            {
                char x = args[i];
                bool y = IsCharNumeric(args[i]);

                if (args[i] == 'E' && level-- == 0)
                    return i;

                if (i != 0 && (args[i] == 'I' || args[i] == 'F' || args[i] == 'N'))
                    level++;

                // skip strings that can give false E's
                if (IsCharNumeric(args[i]) && (i == 0 || ((args[i - 1] != 'S' || removeS_check) && args[i - 1] != 'D' && args[i - 1] != 'C')))
                {
                    int numLength = CountNumsAt(args, i);
                    int num = int.Parse(args.Substring(i, numLength));
                    i += numLength + num - 1;
                }

                // skip the E{x}_{y}_FUN after Ulv in lambdas
                if (args[i] == 'U' && args[i + 1] == 'l' && args[i + 2] == 'v')
                    i += 10;
            }

            return 0;
        }

        static string ReadString(string symbol, ref int i)
        {
            while (IsCharNumeric(symbol[i]))
                i++;

            // i + 1 is the first non-numeric index, i is the length of the number
            int lengthOfString = int.Parse(symbol.Substring(0, i));

            string ret = symbol.Substring(i, lengthOfString);
            i += lengthOfString;

            return ret;
        }

        static string ReadArgs(string args, List<string> savedTypes)
        {
            if (args == "v")
                return "";

            string ret = "";
            bool first = true;

            while (!string.IsNullOrEmpty(args))
            {
                if (!first) ret += ",";

                // {typePrefix}{type}{typeSuffix}, ...
                string typePrefix = "";
                string type = "";
                string typeSuffix = "";

                bool namespaceMode = false;
                bool standard = false;

                if (args[0] == 'N')
                {
                    namespaceMode = true;
                    args = args.Substring(1);
                }

                while (args[0] == 'P' || args[0] == 'R')
                {
                    if (args[0] == 'P')
                    {
                        /*int numOfPtrs = CountAtStart(args, 'P');
                        typeSuffix = new string('*', numOfPtrs);
                        args = args.Substring(numOfPtrs);*/
                        typeSuffix += "*";
                        args = args.Substring(1);
                    }
                    else if (args[0] == 'R')
                    {
                        typeSuffix += "&";
                        args = args.Substring(1);
                    }
                }

                if (args[0] == 'K')
                {
                    typePrefix = "const_";
                    args = args.Substring(1);
                }

                if (args[0] == 'V')
                {
                    typePrefix = "volatile_";
                    args = args.Substring(1);
                }

                if (args[0] == 'N')
                {
                    namespaceMode = true;
                    args = args.Substring(1);
                }

                if (IsCharNumeric(args[0]))
                {
                    /*if (args.StartsWith("5Fix12IiE") || args.StartsWith("5Fix12IsE"))
					{
                        // Fix12i and Fix12s
                        type = "Fix12" + args[7];
                        args = args.Substring(9);

                        // Fix12i adds both Fix12 and Fix12i
                        _S_params.Add("Fix12");
                    }
                    else
					{*/

                    // more complex types like structs
                    bool firstS = true;
                    string curType = "";

                    while (args.Length != 0 && IsCharNumeric(args[0]))
                    {
                        if (!firstS) curType += "::";
                        int nextStringStartIndex = 0;
                        curType += ReadString(args, ref nextStringStartIndex);
                        args = args.Substring(nextStringStartIndex);
                        firstS = false;
                        savedTypes.Add(curType);
                        if (!namespaceMode)
                            break;
                    }

                    type += curType;

                    if (args.Length != 0 && args[0] == 'I')
                    {
                        // template
                        args = args.Substring(1);
                        int templateArgsEnd = GetIndexOfEndE(args);
                        string templateArgs = args.Substring(0, templateArgsEnd);
                        type += "<" + ReadArgs(templateArgs, savedTypes) + ">";
                        args = args.Substring(templateArgsEnd + 1); // + 1 because E

                        // common SM64DS templates
                        type = type.Replace("Fix12<s32>", "Fix12i");
                        type = type.Replace("Fix12<s16>", "Fix12s");

                        savedTypes.Add(type);
                    }
                    else if (args.Length != 0 && (args[0] == 'C' || args[0] == 'D') && IsCharNumeric(args[1]))
                    {
                        // constructor / destructor
                        int startIndex = type.LastIndexOf("::") + 2;
                        if (startIndex < 2)
                            startIndex = 0;

                        type += (args[0] == 'D' ? "::~" : "::") + type.Substring(startIndex);
                        args = args.Substring(2);
                        savedTypes.Add(type);
                    }
                    else if (args.Length != 0 && args[0] == 'a' && args[1] == 'S')
                    {
                        type += "::operator=";
                        args = args.Substring(2);
                        savedTypes.Add(type);
                    }
                    else if (args.Length != 0 && args[0] == 'n' && args[1] == 'w')
                    {
                        type += "::operator_new";
                        args = args.Substring(2);
                        savedTypes.Add(type);
                    }
                }
                else if (args[0] == 'F' || args[0] == 'M')
                {
                    // function pointer
                    string memberName = "";

                    if (args[0] == 'M')
                    {
                        args = args.Substring(2); // MS

                        int numLength = CountNumsAt(args);
                        typeSuffix = "*";

                        if (numLength != 0)
                        {
                            int num = int.Parse(args.Substring(0, numLength));
                            memberName = savedTypes[num + 1];
                            args = args.Substring(numLength + 1); // {x}_
                        }
                        else
                        {
                            memberName = savedTypes[0];
                            args = args.Substring(1); // _
                        }
                    }

                    args = args.Substring(1); // F

                    int funcPtrEnd = GetIndexOfEndE(args);
                    string funData = ReadArgs(args.Substring(0, funcPtrEnd), savedTypes);


                    string funcPrefix = funData.Substring(0, funData.IndexOf(',')); // returnType
                    string funcPtrs = memberName == "" ? "" : $"{memberName}::"; // *
                    string funcSuffix = "(" + funData.Substring(funData.IndexOf(',') + 1) + ")"; // (args)

                    funcSuffix = funcSuffix.Replace("void,", ",");
                    funcSuffix = funcSuffix.Replace(",,", ",");
                    funcSuffix = funcSuffix.Replace("(,", "(");
                    funcSuffix = funcSuffix.Replace(",)", ")");

                    savedTypes.Add(funcPrefix + funcSuffix);

                    for (int i = 0; i < typeSuffix.Length; i++)
                    {
                        funcPtrs += typeSuffix[i];
                        type = funcPrefix + "(" + funcPtrs + ")" + funcSuffix;
                        savedTypes.Add(type);
                    }

                    typeSuffix = typePrefix = "";

                    args = args.Substring(funcPtrEnd + 1);
                }
                else if (args[0] == 'S')
                {
                    args = args.Substring(1);

                    int numLength = CountNumsAt(args);

                    if (numLength != 0)
                    {
                        int num = int.Parse(args.Substring(0, numLength));
                        type = savedTypes[num + 1];
                        args = args.Substring(numLength + 1);
                    }
                    else
                    {
                        type = savedTypes[0];
                        args = args.Substring(1);
                    }
                }
                else
                {
                    standard = true;
                    type = GetStandardType(args);
                    args = args.Substring(1);
                }

                if (args.Length != 0 && args[0] == 'E')
                    args = args.Substring(1);

                if (!standard || typePrefix != "")
                {
                    if (typePrefix != "")
                        savedTypes.Add(typePrefix + type);
                    for (int i = 0; i < typeSuffix.Length; i++)
                        savedTypes.Add(savedTypes[savedTypes.Count - 1] + typeSuffix[i]);
                }

                ret += typePrefix + type + typeSuffix;

                if (namespaceMode && args.StartsWith("Ulv"))
                {
                    savedTypes.Add("lambda"); // the function name gets removed before reading parameters
                    return ret + "::lambda";
                }

                first = false;
            }

            return ret;
        }

        static string DemangleSymbol(string symbol)
        {
            // The symbol is already demangled
            if (!symbol.StartsWith("_Z"))
                return symbol;

            // The symbol is mangled, demangle it
            symbol = symbol.Substring(2);

            if (symbol.StartsWith("N12_GLOBAL__N_1L"))
                symbol = symbol.Substring(16);

            string demangledSymbol;
            List<string> savedTypes = new List<string>();

            if (symbol[0] == 'N')
            {
                int endOfName = GetIndexOfEndE(symbol, true);
                demangledSymbol = ReadArgs(symbol.Substring(0, endOfName), savedTypes);
                symbol = symbol.Substring(endOfName + 1);
            }
            else
            {
                int numLength = CountNumsAt(symbol);
                if (numLength == 0)
                    return symbol;

                int num = int.Parse(symbol.Substring(0, numLength));

                demangledSymbol = ReadArgs(symbol.Substring(0, numLength + num), savedTypes);
                symbol = symbol.Substring(numLength + num);
            }

            if (symbol.Length == 0)
                return demangledSymbol;

            if (symbol == "E")
                return demangledSymbol;
            
            // function parameters
            if (savedTypes.Count != 0)
                savedTypes.RemoveAt(savedTypes.Count - 1); // function name is not a type

            demangledSymbol += "(" + ReadArgs(symbol, savedTypes) + ")";

            return demangledSymbol;
        }
        #endregion
    }
}
