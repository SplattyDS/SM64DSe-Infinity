using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SM64DSe.Patcher
{
	public static class KuppaScriptFixer
	{
		private class PointerInfo
		{
			public string ksCodeFile;

			public string pointerName; // the symbol, from code file
			public uint pointerAddr; // from symbols.x

			public string cutscene; // the symbol, from code file
			public uint cutsceneAddr; // from symbols.x
			public int indexInCutscene; // from code file

			public PointerInfo(string ksCodeFile, string pointerName, string cutscene, int indexInCutscene)
			{
				this.pointerName = pointerName;
				this.cutscene = cutscene;
				this.indexInCutscene = indexInCutscene;
			}
		}

		private static List<PointerInfo> pointerInfos;
		private static char[] BREAK_NAME_CHARS = { ',', ';', '(', ')', '{', '}', ':', ' ', '\t' };

		public static void Run(string codeDirPath, string dirPath, string[] ksCodeFileNames)
		{
			string symbolsFIlePath = codeDirPath + "\\symbols.x";

			pointerInfos = new List<PointerInfo>();

			foreach (string ksCodeFileName in ksCodeFileNames)
			{
				string ksCodeFilePath = dirPath + "\\" + ksCodeFileName;
				string[] ksCodeFileLines = File.ReadAllLines(ksCodeFilePath);

				string curCutscene = "";
				int pointersInCutscene = 0;
				bool inScript = false;

				for (int i = 0; i < ksCodeFileLines.Length - 1; i++)
				{
					if (ksCodeFileLines[i + 1].Contains("NewScript()."))
					{
						curCutscene = ((string[])ksCodeFileLines[i].Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)))[2];
						pointersInCutscene = 0;
						inScript = true;
					}

					if (ksCodeFileLines[i].Contains("End();"))
						inScript = false;

					// not in script
					if (!inScript)
						continue;

					int numPointers = ksCodeFileLines[i].Count(c => c == '&');

					// not a pointer
					if (numPointers == 0)
						continue;

					int prevPointerIndex = -1;

					for (int j = 0; j < numPointers; j++)
					{
						int pointerStart = ksCodeFileLines[i].IndexOf('&', prevPointerIndex + 1) + 1;
						int pointerLength = ksCodeFileLines[i].Length - pointerStart;

						foreach (char breakChar in BREAK_NAME_CHARS)
						{
							int breakLength = ksCodeFileLines[i].IndexOf(breakChar, pointerStart) - pointerStart;

							if (breakLength < pointerLength)
								pointerLength = breakLength;
						}

						string pointerName = ksCodeFileLines[i].Substring(pointerStart, pointerLength);

						pointerInfos.Add(new PointerInfo(ksCodeFileName, pointerName, curCutscene, pointersInCutscene++));
					}
				}
			}

			string[] symbols = File.ReadAllLines(symbolsFIlePath);

			foreach (string symbol in symbols)
			{
				if (string.IsNullOrWhiteSpace(symbol))
					continue;

				string symbolName = GetSymbolName(symbol);
				uint symbolAddr = GetSymbolAddress(symbol);

				foreach (PointerInfo pointerInfo in pointerInfos)
				{
					if (pointerInfo.cutscene == symbolName)
						pointerInfo.cutsceneAddr = symbolAddr;

					if (pointerInfo.pointerName == symbolName)
						pointerInfo.pointerAddr = symbolAddr;
				}
			}

			bool autorw = Program.m_ROM.CanRW();
			if (!autorw) Program.m_ROM.BeginRW();

			foreach (PointerInfo pointerInfo in pointerInfos)
			{
				uint romOffset = pointerInfo.cutsceneAddr - 0x02000000;
				int curPointerID = -1;

				while (true)
				{
					if (Program.m_ROM.Read32(romOffset) == 0x0b00b1e5)
						curPointerID++;

					if (curPointerID == pointerInfo.indexInCutscene)
					{
						Program.m_ROM.Write32(romOffset, pointerInfo.pointerAddr);
						break;
					}
					
					romOffset++;
				}
			}

			if (!autorw) Program.m_ROM.EndRW();

			// every pointer is 0x0b00b1e5
			// create a list of all pointers (with cutscene start address and index) from the code file
			// then obtain the pointer addresses from symbols.x
			// then write the pointer addresses to arm9.bin using the cutscene start address and index
		}

		static string GetSymbolName(string line)
		{
			int nameLength = line.IndexOf(' ');
			return line.Substring(0, nameLength);
		}

		static uint GetSymbolAddress(string line)
		{
			int addressStart = line.IndexOf('=') + 4; // remove the "= 0x"
			string address = line.Substring(addressStart, 8).ToUpper();
			return Convert.ToUInt32(address, 16);
		}
	}
}
