using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SM64DSe.Patcher
{
	public static class ObjectDecompMaker
	{
		public class StaticObjData
		{
			public uint actorID;
			public uint overlayID;
			public uint spawnInfoAddr;

			public StaticObjData(uint actorID, uint overlayID, uint spawnInfoAddr)
			{
				this.actorID = actorID;
				this.overlayID = overlayID;
				this.spawnInfoAddr = spawnInfoAddr;
			}
		}

		public struct Fix12i
		{
			public int val;

			public Fix12i(int val)
			{
				this.val = val;
			}

			public override string ToString()
			{
				float fValue = val / 4096.0f;
				string sValue = fValue.ToString().Replace(',', '.');
				sValue += sValue.Contains('.') ? "_f" : "._f";
				return sValue;
			}
		}

		public class ObjData
		{
			public StaticObjData staticData;
			public string name;
			public string internalName;
			public ushort objectID;

			// SpawnInfo
			public uint constructorAddr;
			public short behavPriority;
			public short renderPriority;
			public uint flags;
			public Fix12i rangeOffsetY;
			public Fix12i range;
			public Fix12i drawDist;
			public Fix12i unk18;

			public uint vtableAddr;
			public uint vCount;
			public uint[] vfuncs = new uint[35]; // highest known: 33 in PathLiftBase

			public Type baseType;

			public uint structSize;
			public Member[] members;

			public enum Type
			{
				ActorBase,
				ActorDerived,
				Scene,
				
				Actor,

				Enemy,
				CapEnemy,

				Platform,
				PathLiftBase,

				NUM_TYPES,
			}

			public static readonly uint[] baseSizes = new uint[(int)Type.NUM_TYPES]
			{
				0x50,
				0x50,
				0x50,

				0xd4,

				0x110,
				0x180,

				0x320,
				0x450,
			};

			public static readonly int[] numKnownVfuncs = new int[(int)Type.NUM_TYPES]
			{
				18,
				18,
				18,

				31,

				31,
				31,

				32,
				33,
			};

			public static readonly uint[] baseConstructorAddr = new uint[(int)Type.NUM_TYPES]
			{
				0x02043dec,
				0, // no constructor
				0, // no constructor

				0x0201150c,

				0x020aed98,
				0x02006554,

				0x020eea50,
				0, // no constructor
			};

			public static readonly uint[] baseVtableAddr = new uint[(int)Type.NUM_TYPES]
			{
				0x02099edc,
				0x0208e4b8,
				0x02092680,

				0x0208e3a4,

				0x021081e4,
				0x02108284,

				0x0210ae38,
				0x0210af70,
			};

			public ObjData(StaticObjData staticData, NitroOverlay overlay)
			{
				this.staticData = staticData;

				var dbRes = ObjectDatabase.m_ObjectInfo.Where(o => o.m_ActorID == staticData.actorID);
				ObjectDatabase.ObjectInfo dbObj;
				if (dbRes.Any())
					dbObj = dbRes.Single();
				else // for actors that don't have an object id
					dbObj = ObjectDatabase.m_OtherActorInfo.Where(o => o.m_ActorID == staticData.actorID).Single();
				
				name = dbObj.m_Name;
				internalName = dbObj.m_InternalName;
				objectID = dbObj.m_ID;

				uint overlayAddr = overlay.GetRAMAddr();
				uint spawnInfoAddr = staticData.spawnInfoAddr;
				uint spawnInfoOffset = spawnInfoAddr - overlayAddr;

				if (spawnInfoOffset > overlay.GetSize())
					throw new Exception($"Actor {staticData.actorID} was not found in its overlay.");

				constructorAddr = overlay.Read32(spawnInfoOffset + 0x00);
				behavPriority = (short)overlay.Read16(spawnInfoOffset + 0x04);
				renderPriority = (short)overlay.Read16(spawnInfoOffset + 0x06);
				flags = overlay.Read32(spawnInfoOffset + 0x08);
				rangeOffsetY = new Fix12i((int)overlay.Read32(spawnInfoOffset + 0x0c));
				range = new Fix12i((int)overlay.Read32(spawnInfoOffset + 0x10));
				drawDist = new Fix12i((int)overlay.Read32(spawnInfoOffset + 0x14));
				unk18 = new Fix12i((int)overlay.Read32(spawnInfoOffset + 0x18));

				// hardcode this for the player because the constructor in the SpawnInfo calls the actual constructor
				if (staticData.actorID == 191)
					constructorAddr = 0x020e68f4;

				vtableAddr = 0;
				uint curAddr = constructorAddr;

				while (vtableAddr == 0)
				{
					uint curOffset = curAddr - overlay.GetRAMAddr();

					if (curOffset > overlay.GetRAMAddr())
						throw new Exception($"No valid vtable pointer for actor {staticData.actorID} found in its overlay.");

					uint curValue = overlay.Read32(curOffset);

					// 0x020aed98 // Enemy::Enemy
					// 0x02006554 // CapEnemy::CapEnemy

					bool isBaseRelated = false;

					for (int i = 0; i < (int)Type.NUM_TYPES; i++)
					{
						if (curValue == baseVtableAddr[i])
						{
							baseType = (Type)i;
							isBaseRelated = true;
						}
						else if (IsBlTo(curValue, curAddr, baseConstructorAddr[i]))
						{
							if (baseType < (Type)i)
								baseType = (Type)i;

							isBaseRelated = true;
						}
					}

					uint curVtableAddr = curValue;

					// is a pointer to something in the overlay?
					if (!isBaseRelated && curVtableAddr >= overlay.GetRAMAddr() && curVtableAddr < overlay.GetRAMAddr() + overlay.GetSize())
					{
						vtableAddr = curVtableAddr;

						// anything past 31 for Actors, 32 for Platforms and 33 for PathLiftBases may not be a virtual, but we have no way to know
						uint i;
						for (i = 0; i < 35; i++)
						{
							uint curVfuncAddr = curVtableAddr + i * 4;

							// if at the very end of the overlay
							if (curVfuncAddr >= overlay.GetRAMAddr() + overlay.GetSize())
								break;

							uint curVfunc = overlay.Read32(curVfuncAddr - overlay.GetRAMAddr());

							// if not a pointer
							if (!(curVfunc >= 0x02000000 && curVfunc < 0x02400000))
								break;

							vfuncs[i] = curVfunc;
						}

						vCount = i;

						if (vCount < numKnownVfuncs[(int)baseType]) // an Actor has at least 31 virtuals
							vtableAddr = 0;
					}

					curAddr += 4;
				}

				curAddr = constructorAddr;
				bool foundEnd = false;
				List<Member> members = new List<Member>();

				while (!foundEnd)
				{
					uint curOffset = curAddr - overlay.GetRAMAddr();

					if (curAddr > overlay.GetRAMAddr() + overlay.GetSize())
						throw new Exception($"No end for the constructor of actor {staticData.actorID} found.");

					uint curValue = overlay.Read32(curOffset);

					foreach (MemberInfo memberInfo in MEMBER_INFOS)
					{
						if (IsBlTo(curValue, curAddr, memberInfo.constructorAddr))
						{
							uint memberOffset = FindValue(overlay, 0, curAddr - 4, constructorAddr, true);
							members.Add(new Member(memberInfo, memberOffset));
						}
					}

					if (IsBlTo(curValue, curAddr, 0x020733a8)) // __cxa_vec_ctor
					{
						uint memberOffset = FindValue(overlay, 0, curAddr - 4, constructorAddr, true);
						uint num = FindValue(overlay, 1, curAddr - 4, constructorAddr);
						uint memberConstructor = FindValue(overlay, 3, curAddr - 4, constructorAddr);
						members.Add(new Member(MEMBER_INFOS.Where(m => m.constructorAddr == memberConstructor).First(), memberOffset, num));
					}

					if (IsBlTo(curValue, curAddr, 0x02043444)) // AbstractBase::operator_new
					{
						structSize = FindValue(overlay, 0, curAddr - 4, constructorAddr);
					}

					// pointer found, there is always at least a pointer to the vtable
					if (curValue >= 0x02000000 && curValue < 0x02400000)
						foundEnd = true;

					curAddr += 4;
				}

				this.members = members.ToArray();

					// notes for header:
					// struct size: constructor+(4 or 8) should be a "mov r0, #size" or "ldr r0, =size"
					// member initialization: either a "bl constructor"
					// member initialization (array): a "bl __cxa_vec_ctor" with "ldr r3, =constructor" and "mov r1, #num"
			}

			public string Write(string objCodeDir, IEnumerable<ObjData> objsWithSharedVtable)
			{
				bool hasSharedVtable = objsWithSharedVtable != null && objsWithSharedVtable.Count() > 0;

				string structName = GetStructName();
				string baseStructName = GetBaseStructName();
				int[] overwrittenVfuncs = GetOverwrittenVfuncs().ToArray();
				int[] newVfuncs = GetNewVfuncs().ToArray();
				VirtualFunctionInfo[] newVfuncInfos = GetNewVfuncInfos(newVfuncs).ToArray();

				List<string> lines = new List<string>(15 + (7 * overwrittenVfuncs.Count()) + (8 * newVfuncs.Count()) + (12 * ((hasSharedVtable ? objsWithSharedVtable.Count() : 0) + 1)));

				lines.Add("#include \"SM64DS_2.h\"");
				if (!ALWAYS_LOADED_STRUCTS.Contains(structName))
					// lines.Add("#include \"Actors/" + structName + ".h\"");
					lines.Add("#include \"" + structName + ".h\"");
				lines.Add("");

				foreach (int overwrittenVfunc in overwrittenVfuncs)
				{
					VirtualFunctionInfo vfuncInfo = VFUNCS[overwrittenVfunc];
					if (vfuncInfo.name == "~")
						continue;
					lines.Add("asm(\"Og" + vfuncInfo.name + " = " + Helper.uintToString(vfuncs[overwrittenVfunc]) + "\");");
				}
				for (int i = 0; i < newVfuncs.Count(); i++)
				{
					VirtualFunctionInfo vfuncInfo = newVfuncInfos[i];
					lines.Add("// asm(\"Og" + vfuncInfo.name + " = " + Helper.uintToString(vfuncs[newVfuncs[i]]) + "\");");
				}

				lines.Add("");
				lines.Add("extern \"C\"");
				lines.Add("{");

				foreach (int overwrittenVfunc in overwrittenVfuncs)
				{
					VirtualFunctionInfo vfuncInfo = VFUNCS[overwrittenVfunc];
					if (vfuncInfo.name == "~")
						continue;
					string parameters = vfuncInfo.parameters == "" ? "" : ", " + vfuncInfo.parameters;
					lines.Add("\t" + vfuncInfo.type + " Og" + vfuncInfo.name + "(" + structName + "* a" + parameters + ");");
				}
				for (int i = 0; i < newVfuncs.Count(); i++)
				{
					VirtualFunctionInfo vfuncInfo = newVfuncInfos[i];
					string parameters = vfuncInfo.parameters == "" ? "" : ", " + vfuncInfo.parameters;
					lines.Add("\t// Unknown virtual(?), check return type and parameters!");
					lines.Add("\t// " + vfuncInfo.type + " Og" + vfuncInfo.name + "(" + structName + "* a" + parameters + ");");
				}

				lines.Add("}");
				lines.Add("");
				lines.Add("namespace");
				lines.Add("{");
				lines.Add("\t");
				lines.Add("}");
				lines.Add("");
				lines.AddRange(GetSpawnInfo());

				if (hasSharedVtable)
				{
					foreach (ObjData objData in objsWithSharedVtable)
						lines.AddRange(objData.GetSpawnInfo(this));
				}

				lines.Add("// SharedFilePtr " + structName + "::modelFile;");
				lines.Add("// SharedFilePtr " + structName + "::animFile;");
				lines.Add("");

				foreach (int overwrittenVfunc in overwrittenVfuncs)
				{
					VirtualFunctionInfo vfuncInfo = VFUNCS[overwrittenVfunc];
					if (vfuncInfo.name == "~")
						continue;
					lines.AddRange(GetVirtualFunctionImpl(vfuncInfo));
				}
				for (int i = 0; i < newVfuncs.Count(); i++)
				{
					VirtualFunctionInfo vfuncInfo = newVfuncInfos[i];
					lines.AddRange(GetVirtualFunctionImpl(vfuncInfo, true));
				}
				
				lines.Add(structName + "::" + structName + "() {}");
				lines.Add(structName + "::~" + structName + "() {}");

				if (!Directory.Exists(objCodeDir + "\\U_" + structName + "\\"))
					Directory.CreateDirectory(objCodeDir + "\\U_" + structName + "\\");

				File.WriteAllText(objCodeDir + "\\U_" + structName + "\\" + structName + ".cpp", string.Join("\r\n", lines));
				return "U_" + structName;
			}

			public void WriteDL(string objCodeDir, IEnumerable<ObjData> objsWithSharedVtable)
			{
				string structName = GetStructName();

				List<string> lines = new List<string>(17 + (objsWithSharedVtable != null ? objsWithSharedVtable.Count() : 0) * 2);

				lines.Add("#include \"SM64DS_2.h\"");
				if (!ALWAYS_LOADED_STRUCTS.Contains(structName))
					// lines.Add("#include \"Actors/" + structName + ".h\"");
					lines.Add("#include \"" + structName + ".h\"");
				lines.Add("");
				lines.Add("void init()");
				lines.Add("{");

				if (objectID != 0xffff)
					lines.Add("\tOBJ_TO_ACTOR_ID_TABLE[" + internalName + "_OBJECT_ID] = " + internalName + "_ACTOR_ID;");
				if (objsWithSharedVtable != null)
					foreach (ObjData obj in objsWithSharedVtable)
						if (obj.objectID != 0xffff)
							lines.Add("\tOBJ_TO_ACTOR_ID_TABLE[" + obj.internalName + "_OBJECT_ID] = " + obj.internalName + "_ACTOR_ID;");
				lines.Add("\t");

				lines.Add("\tACTOR_SPAWN_TABLE[" + internalName + "_ACTOR_ID] = &" + GetSpawnInfoName() + ";");
				if (objsWithSharedVtable != null)
					foreach (ObjData obj in objsWithSharedVtable)
						lines.Add("\tACTOR_SPAWN_TABLE[" + obj.internalName + "_ACTOR_ID] = &" + obj.GetSpawnInfoName(this) + ";");

				lines.Add("\t");
				lines.Add("\t// " + structName + "::modelFile.Construct(\"data/the_model.bmd\"ov0);");
				lines.Add("\t// " + structName + ":: animFile.Construct(\"data/the_animation.bca\"ov0);");
				lines.Add("}");
				lines.Add("");
				lines.Add("void cleanup()");
				lines.Add("{");
				lines.Add("\t");
				lines.Add("}");

				File.WriteAllText(objCodeDir + "\\U_" + structName + "\\DL.cpp", string.Join("\r\n", lines));
			}

			public void WriteHeader(string objCodeDir, IEnumerable<ObjData> objsWithSharedVtable)
			{
				string structName = GetStructName();
				int[] overwrittenVfuncs = GetOverwrittenVfuncs().ToArray();
				int[] newVfuncs = GetNewVfuncs().ToArray();
				VirtualFunctionInfo[] newVfuncInfos = GetNewVfuncInfos(newVfuncs).ToArray();
				string memberList = GetMemberList();

				List<string> lines = new List<string>();

				lines.Add("#pragma once");
				lines.Add("");
				lines.Add("struct " + structName + " : " + GetBaseStructName());
				lines.Add("{");

				if (!string.IsNullOrWhiteSpace(memberList))
				{
					lines.Add(memberList);
					lines.Add("\t");
				}

				lines.Add("\tstatic " + GetSpawnInfoType() + " spawnData;");

				if (objsWithSharedVtable != null)
					foreach (ObjData obj in objsWithSharedVtable)
						lines.Add("\tstatic " + GetSpawnInfoType() + " spawnData" + obj.GetStructName() + ";");

				lines.Add("\t// static SharedFilePtr modelFile;");
				lines.Add("\t// static SharedFilePtr animFile;");
				lines.Add("\t");
				lines.Add("\t" + structName + "();");

				bool writtenDtor = false;
				foreach (int overwrittenVfunc in overwrittenVfuncs)
				{
					VirtualFunctionInfo vfuncInfo = VFUNCS[overwrittenVfunc];
					if (vfuncInfo.name == "~")
					{
						if (!writtenDtor)
							lines.Add("\tvirtual ~" + structName + "() override;");
						writtenDtor = true;
					}
					else
						lines.Add(GetVirtualFunctionDef(vfuncInfo, true));
				}
				for (int i = 0; i < newVfuncs.Count(); i++)
				{
					VirtualFunctionInfo vfuncInfo = newVfuncInfos[i];
					lines.Add(GetVirtualFunctionDef(vfuncInfo, false));
				}

				lines.Add("\t");
				lines.Add("\t// void UpdateModelTransform();");
				lines.Add("};");
				lines.Add("");
				lines.Add("static_assert(sizeof(" + structName + ") == 0x" + Convert.ToString(structSize, 16) + ");");

				File.WriteAllText(objCodeDir + "\\U_" + structName + "\\" + structName + ".h", string.Join("\r\n", lines));
			}

			private string GetMemberString(uint offset)
			{
				return GetMemberString("u32", "unk" + Convert.ToString(offset, 16));
			}
			private string GetMemberString(Member member)
			{
				return GetMemberString(member.typeName, "unk" + Convert.ToString(member.offset, 16), member.num);
			}
			private string GetMemberString(string typeName, string varName, uint num = 1)
			{
				if (num == 1)
					return "\t" + typeName + " " + varName + ";";
				else
					return "\t" + typeName + " " + varName + "[" + num + "];";
			}
			private string AddMemberOffset(string line, int biggestLen, bool addMark, Member member)
			{
				int numSpaces = (biggestLen - line.Length > 0 ? biggestLen - line.Length : 0) + 1;
				string suffix = addMark ? "?" : "";
				return line + new string(' ', numSpaces) + "// 0x" + Convert.ToString(member.offset, 16) + suffix;
			}
			private class MemberPair
			{
				public string line;
				public uint offset;

				public MemberPair(string line, uint offset)
				{
					this.line = line;
					this.offset = offset;
				}
			}
			private string GetMemberList()
			{
				List<MemberPair> lines = new List<MemberPair>();

				for (uint curOffset = baseSizes[(int)baseType]; curOffset < structSize;) // manual increment
				{
					bool foundMemberAtCurOffset = false;

					foreach (Member member in members)
					{
						if (curOffset == member.offset)
						{
							lines.Add(new MemberPair(GetMemberString(member), curOffset));
							curOffset += member.size * member.num;
							foundMemberAtCurOffset = true;
							break;
						}
					}

					if (foundMemberAtCurOffset)
						continue;

					lines.Add(new MemberPair(GetMemberString(curOffset), curOffset));
					curOffset += 4;
				}

				if (!lines.Any())
					return "";

				int biggestLen = lines.Select(l => l.line.Length).Max();

				foreach (Member member in members)
				{
					for (int i = 0; i < lines.Count(); i++)
					{
						if (lines[i].offset != member.offset)
							continue;

						lines[i].line = AddMemberOffset(lines[i].line, biggestLen, false, member);
					}
				}

				return string.Join("\r\n", lines.OrderBy(l => l.offset).Select(l => l.line));
			}
			private string GetSpawnInfoType(ObjData parent = null)
			{
				if (baseType == Type.ActorBase || baseType == Type.ActorDerived || baseType == Type.Scene)
					return "BaseSpawnInfo";
				else
					return "SpawnInfo";
			}
			private string GetSpawnInfoName(ObjData parent = null)
			{
				string structName = parent != null ? parent.GetStructName() : GetStructName();
				string suffix = parent != null ? GetStructName() : "";

				return structName + "::spawnData" + suffix;
			}
			private IEnumerable<string> GetSpawnInfo(ObjData parent = null)
			{
				List<string> lines = new List<string>();

				string spawnInfoName = GetSpawnInfoName(parent);
				string structName = parent != null ? parent.GetStructName() : GetStructName();

				lines.Add(GetSpawnInfoType() + " " + spawnInfoName + " =");
				lines.Add("{");
				// lines.Add("\t&FUN_" + Convert.ToString(constructorAddr, 16).PadLeft(8, '0') + ",");
				lines.Add("\t[]() -> ActorBase* { return new " + structName + "; },");
				lines.Add("\t" + Helper.shortToString(behavPriority) + ",");
				lines.Add("\t" + Helper.shortToString(renderPriority) + ",");
				if (GetSpawnInfoType() == "SpawnInfo")
				{
					lines.Add("\t" + GetFlags() + ",");
					lines.Add("\t" + rangeOffsetY + ",");
					lines.Add("\t" + range + ",");
					lines.Add("\t" + drawDist + ",");
					lines.Add("\t" + unk18 + ",");
				}
				lines.Add("};");
				lines.Add("");

				return lines;
			}
			private string GetVirtualFunctionDef(VirtualFunctionInfo vfuncInfo, bool overrideVfun)
			{
				string suffix = overrideVfun ? " override" : "";
				string comment = !overrideVfun ? "// " : "";
				return "\t" + comment + "virtual " + vfuncInfo.type + " " + vfuncInfo.name + "(" + vfuncInfo.parameters + ")" + suffix + ";";
			}
			private IEnumerable<string> GetVirtualFunctionImpl(VirtualFunctionInfo vfuncInfo, bool comment = false)
			{
				List<string> lines = new List<string>();

				string ret = vfuncInfo.type == "void" ? "" : "return ";

				string parameters = "this";
				if (vfuncInfo.parameters != "")
				{
					// "u32 arg0, u32 arg1" -> "u32","arg0,","u32","arg1"
					string[] splitParams = vfuncInfo.parameters.Split(' ');

					parameters += ",";

					for (int i = 0; i < splitParams.Length; i += 2)
					{
						// splitparams[i] is the param type
						// splitparams[i+1] is the param name (+ ",", if not the last entry)

						parameters += " " + splitParams[i+1];
					}
				}

				string prefix = comment ? "// " : "";

				lines.Add(prefix + vfuncInfo.type + " " + GetStructName() + "::" + vfuncInfo.name + "(" + vfuncInfo.parameters + ")");
				lines.Add(prefix + "{");
				lines.Add("\t" + prefix + ret + "Og" + vfuncInfo.name + "(" + parameters + ");");
				lines.Add(prefix + "}");
				lines.Add("");

				return lines;
			}

			private IEnumerable<int> GetOverwrittenVfuncs()
			{
				// go over all the vfuncs
				// check if current vfunc == basefunc

				uint[] baseVfuncs = GetBaseVfuncs();

				List<int> overwrittenVfuncs = new List<int>();

				for (int i = 0; i < baseVfuncs.Length; i++)
				{
					if (vfuncs[i] != baseVfuncs[i])
						overwrittenVfuncs.Add(i);
				}

				return overwrittenVfuncs;
			}
			private IEnumerable<int> GetNewVfuncs()
			{
				List<int> newVfuncs = new List<int>();

				for (int i = GetBaseVfuncs().Length; i < vCount; i++)
					newVfuncs.Add(i);

				return newVfuncs;
			}
			private IEnumerable<VirtualFunctionInfo> GetNewVfuncInfos(IEnumerable<int> newVfuncs)
			{
				VirtualFunctionInfo[] ret = new VirtualFunctionInfo[newVfuncs.Count()];

				int i = 0;
				foreach (int newVfunc in newVfuncs)
					ret[i++] = new VirtualFunctionInfo("void", "Virtual" + Convert.ToString(newVfunc * 4, 16), "");

				return ret;
			}

			private uint[] GetBaseVfuncs()
			{
				switch (baseType)
				{
					case Type.ActorBase:
						return ActorBase_vtable;
					case Type.ActorDerived:
						return ActorDerived_vtable;
					case Type.Scene:
						return Scene_vtable;
					case Type.Actor:
						return Actor_vtable;
					case Type.Enemy:
						return Enemy_vtable;
					case Type.CapEnemy:
						return CapEnemy_vtable;
					case Type.Platform:
						return Platform_vtable;
					case Type.PathLiftBase:
						return PathLiftBase_vtable;
					default:
						throw new Exception("Unknown actor type for actor " + staticData.actorID);
				}
			}

			private bool IsBlTo(uint instruction, uint addr, uint blToAddr)
			{
				// is a bl (no condition) to something
				if ((instruction & 0xff000000) == 0xeb000000)
				{
					// instruction += ((branchAddr - hookAddr - 8) >> 2) & 0x00ffffff
					// a = (b - c - 8) / 4

					// is a bl to blToAddr
					if ((instruction & 0x00ffffff) == ((blToAddr - addr - 8) >> 2 & 0x00ffffff))
						return true;
				}

				return false;
			}
			// looks for either a "mov rX, #value" or "ldr rX, =value" in reverse direction from startPos until end, can optionally search for "add rX, any, #value"
			private uint FindValue(NitroOverlay overlay, byte register, uint startAddr, uint endAddr, bool searchForAdd = false)
			{
				for (uint curAddr = startAddr; curAddr >= endAddr; curAddr -= 4)
				{
					uint instruction = overlay.Read32(curAddr - overlay.GetRAMAddr());

					// is "mov rX, #value"?
					if ((instruction & 0xfff0f000) == (0xe3a00000 | (register << 12)))
					{
						int invShift = (int)((instruction & 0xf00) >> 8);
						int shift = invShift == 0 ? 0 : (0x10 - invShift) * 2;
						return (instruction & 0xff) << shift;
					}
					// is "ldr rX, =value"
					else if ((instruction & 0xfffff000) == (0xe59f0000 | (register << 12)))
					{
						uint valAddr = curAddr + (instruction & 0xfff) + 8;
						return overlay.Read32(valAddr - overlay.GetRAMAddr());
					}
					// is "add rX, any, #value"
					else if (searchForAdd && (instruction & 0xfff0f000) == (0xe2800000 | (register << 12)))
					{
						int invShift = (int)((instruction & 0xf00) >> 8);
						int shift = invShift == 0 ? 0 : (0x10 - invShift) * 2;
						return (instruction & 0xff) << shift;
					}
				}

				return 0;
			}

			private string GetStructName()
			{
				string ret = name;

				if (ret.StartsWith("1"))
					ret = "One" + ret.Substring(1);

				ret = ret.Replace("(", "").Replace(")", "").Replace("-", "").Replace(".", "").Replace(" ", "").Replace("&", "")
					// .Replace("1", "One").Replace("2", "Two").Replace("3", "Three").Replace("4", "Four").Replace("5", "Five")
					// .Replace("6", "Six").Replace("7", "Seven").Replace("8", "").Replace("9", "Nine").Replace("0", "Zero")
					.Replace("?", "Question").Replace("!", "Exclamation");

				return ret;
			}
			private string GetBaseStructName()
			{
				return baseType.ToString();
			}

			private static readonly string[] FLAGS =
			{
				"NO_BEHAVIOR_IF_OFF_SCREEN",
				"NO_RENDER_IF_OFF_SCREEN",
				"UNK_02",
				"OFF_SCREEN",
				"OFF_SHADOW_RANGE",
				"WRONG_AREA",
				"UNK_06",
				"GRABBABLE",
				"GRABBED",
				"UNK_09",
				"THROWN",
				"UNK_11",
				"UNK_12",
				"DROPPED",
				"IN_PLAYER_HAND",
				"N_CARRY",
				"UNK_16",
				"GOING_TO_YOSHI_MOUTH",
				"IN_YOSHI_MOUTH",
				"BEING_SPIT",
				"UNK_20",
				"UNK_21",
				"UNK_22",
				"UPDATE_WHEN_READING_SIGN",
				"CAN_NO_DAMAGE_SQUISH_PLAYER",
				"CAN_SQUISH",
				"UPDATE_DURING_STAR_CUTSCENE",
				"SPAWN_EVEN_IF_KILLED_BEFORE",
				"AIMABLE_BY_EGG",
				"UPDATE_DURING_CUTSCENE",
				"UPDATE_DURING_EXIT_COURSE",
				"UPDATE_DURING_SAVE",
			};
			private string GetFlags()
			{
				if (flags == 0)
					return "0";

				string flagsWithNames = "";

				for (int i = 0; i < FLAGS.Length; i++)
					if ((flags & (1 << i)) != 0)
						flagsWithNames += $"Actor::{FLAGS[i]} | ";

				flagsWithNames = flagsWithNames.TrimEnd(' ', '|', ' ');

				return flagsWithNames;
			}

			public class VirtualFunctionInfo
			{
				public string type;
				public string name;
				public string parameters;

				public VirtualFunctionInfo(string type, string name, string parameters)
				{
					this.type = type;
					this.name = name;
					this.parameters = parameters;
				}
			}

			private static readonly VirtualFunctionInfo[] VFUNCS =
			{
				new VirtualFunctionInfo("s32", "InitResources", ""),
				new VirtualFunctionInfo("bool", "BeforeInitResources", ""),
				new VirtualFunctionInfo("void", "AfterInitResources", "u32 vfSuccess"),
				new VirtualFunctionInfo("s32", "CleanupResources", ""),
				new VirtualFunctionInfo("bool", "BeforeCleanupResources", ""),
				new VirtualFunctionInfo("void", "AfterCleanupResources", "u32 vfSuccess"),
				new VirtualFunctionInfo("s32", "Behavior", ""),
				new VirtualFunctionInfo("bool", "BeforeBehavior", ""),
				new VirtualFunctionInfo("void", "AfterBehavior", "u32 vfSuccess"),
				new VirtualFunctionInfo("s32", "Render", ""),
				new VirtualFunctionInfo("bool", "BeforeRender", ""),
				new VirtualFunctionInfo("void", "AfterRender", "u32 vfSuccess"),
				new VirtualFunctionInfo("void", "OnPendingDestroy", ""),
				new VirtualFunctionInfo("bool", "Virtual34", "u32 arg0, u32 arg1"),
				new VirtualFunctionInfo("bool", "Virtual38", "u32 arg0, u32 arg1"),
				new VirtualFunctionInfo("bool", "OnHeapCreated", ""),
				new VirtualFunctionInfo("~", "~", ""),
				new VirtualFunctionInfo("~", "~", ""),
				new VirtualFunctionInfo("u32", "OnYoshiTryEat", ""),
				new VirtualFunctionInfo("void", "OnTurnIntoEgg", "Player& player"),
				new VirtualFunctionInfo("bool", "Virtual50", ""),
				new VirtualFunctionInfo("void", "OnGroundPounded", "Actor& groundPounder"),
				new VirtualFunctionInfo("void", "OnAttacked1", "Actor& attacker"),
				new VirtualFunctionInfo("void", "OnAttacked2", "Actor& attacker"),
				new VirtualFunctionInfo("void", "OnKicked", "Actor& kicker"),
				new VirtualFunctionInfo("void", "OnPushed", "Actor& pusher"),
				new VirtualFunctionInfo("void", "OnHitByCannonBlastedChar", "Actor& blaster"),
				new VirtualFunctionInfo("void", "OnHitByMegaChar", "Player& megaChar"),
				new VirtualFunctionInfo("void", "OnHitFromUnderneath", "Actor& attacker"),
				new VirtualFunctionInfo("Fix12i", "OnAimedAtWithEgg", ""),
				new VirtualFunctionInfo("Vector3", "OnAimedAtWithEggReturnVec", ""),
				new VirtualFunctionInfo("void", "Kill", ""),
				new VirtualFunctionInfo("void", "AfterClsn", ""),
			};

			public class MemberInfo
			{
				public string typeName;
				public uint constructorAddr;
				public uint size;

				public MemberInfo(string typeName, uint constructorAddr, uint size)
				{
					this.typeName = typeName;
					this.constructorAddr = constructorAddr;
					this.size = size;
				}
			}

			public class Member : MemberInfo
			{
				public uint offset;
				public uint num;

				public Member(MemberInfo info, uint offset, uint num = 1)
					: base(info.typeName, info.constructorAddr, info.size)
				{
					this.offset = offset;
					this.num = num;
				}
			}

			private static readonly MemberInfo[] MEMBER_INFOS =
			{
				// new MemberInfo("CylinderClsn", 0x020150cc, 0x30), // shouldn't be used
				new MemberInfo("CylinderClsnWithPos", 0x02014878, 0x3c),
				new MemberInfo("MovingCylinderClsn", 0x020149c8, 0x34),
				new MemberInfo("MovingCylinderClsnWithPos", 0x02014a84, 0x40),
				// new MemberInfo("MeshColliderBase", 0x0203969c, 0x20), // shouldn't be used
				new MemberInfo("MeshCollider", 0x02039894, 0x50),
				new MemberInfo("MovingMeshCollider", 0x0203a494, 0x1c8),
				new MemberInfo("ExtendingMeshCollider", 0x0203ab8c, 0x1d0),
				new MemberInfo("ClsnResult", 0x0203816c, 0x28),
				new MemberInfo("RaycastGround", 0x02037570, 0x50),
				new MemberInfo("RaycastLine", 0x020377b0, 0x78),
				new MemberInfo("SphereClsn", 0x02037d18, 0x10c),
				new MemberInfo("WithMeshClsn", 0x02037430, 0x1bc),
				new MemberInfo("ModelBase", 0x02017150, 0x8),
				new MemberInfo("Model", 0x02016d58, 0x50),
				new MemberInfo("ModelAnim", 0x02016958, 0x64),
				new MemberInfo("ModelAnim2", 0x020163a0, 0x78),
				new MemberInfo("ShadowModel", 0x02016068, 0x28),
				new MemberInfo("CommonModel", 0x02016204, 0x3c),
				new MemberInfo("BlendModelAnim", 0x020166d4, 0x70),
				new MemberInfo("Animation", 0x02015cf8, 0x10),
				new MemberInfo("MaterialChanger", 0x02015850, 0x14),
				new MemberInfo("TextureTransformer", 0x02015950, 0x14),
				new MemberInfo("TextureSequence", 0x02015a50, 0x14),
				new MemberInfo("FaderWipe", 0x02017480, 0x60),
				new MemberInfo("PathPtr", 0x0203ad74, 0x8), // called in InitResources, not in constructor
				new MemberInfo("CapIcon", 0x020777ac, 0x1c), // decompiled, should only be used by Cap and DorrieCap
				new MemberInfo("Vector3", 0x0203d384, 0xc), // yes, it has a constructor (and destructor), only used in calls to __cxa_vec_ctor
				new MemberInfo("Vector3_16", 0x0203d73c, 0x6), // ^
			};

			private static readonly uint[] ActorBase_vtable =
			{
				0x02043c80,
				0x02043c78,
				0x02043bf8,
				0x02043bf0,
				0x02043bac,
				0x02043b2c,
				0x02043b24,
				0x02043afc,
				0x02043af8,
				0x02043af0,
				0x02043ac8,
				0x02043ac4,
				0x02043ac0,
				0x0204357c,
				0x0204349c,
				0x02043494,
				uint.MaxValue,
				uint.MaxValue,
			};
			private static readonly uint[] ActorDerived_vtable =
			{
				0x02043c80,
				0x02043c78,
				0x02013ef4,
				0x02043bf0,
				0x02043bac,
				0x02043b2c,
				0x02043b24,
				0x02043afc,
				0x02043af8,
				0x02043af0,
				0x02043ac8,
				0x02043ac4,
				0x02043ac0,
				0x0204357c,
				0x0204349c,
				0x02043494,
				uint.MaxValue,
				uint.MaxValue,
			};
			private static readonly uint[] Scene_vtable =
			{
				0x02043c80,
				0x0202e638,
				0x0202e62c,
				0x02043bf0,
				0x0202e5f0,
				0x0202e5d0,
				0x02043b24,
				0x0202e3d4,
				0x0202e3c8,
				0x02043af0,
				0x0202e3a4,
				0x0202e398,
				0x02043ac0,
				0x0204357c,
				0x0204349c,
				0x02043494,
				uint.MaxValue,
				uint.MaxValue,
			};
			private static readonly uint[] Actor_vtable =
			{
				0x02043c80,
				0x02011268,
				0x02011244,
				0x02043bf0,
				0x02011220,
				0x02011214,
				0x02043b24,
				0x02010fd4,
				0x02010fc8,
				0x02043af0,
				0x02010f78,
				0x02010f6c,
				0x02043ac0,
				0x0204357c,
				0x0204349c,
				0x02043494,
				uint.MaxValue,
				uint.MaxValue,
				0x02010160,
				0x02010154,
				0x0201014c,
				0x02010148,
				0x02010144,
				0x02010140,
				0x0201013c,
				0x02010138,
				0x02010134,
				0x02010130,
				0x0201012c,
				0x02010124,
				0x020100dc,
			};
			private static readonly uint[] Enemy_vtable =
			{
				0x02043c80,
				0x02011268,
				0x02011244,
				0x02043bf0,
				0x02011220,
				0x02011214,
				0x02043b24,
				0x02010fd4,
				0x02010fc8,
				0x02043af0,
				0x02010f78,
				0x02010f6c,
				0x02043ac0,
				0x0204357c,
				0x0204349c,
				0x02043494,
				uint.MaxValue,
				uint.MaxValue,
				0x02010160,
				0x02010154,
				0x0201014c,
				0x02010148,
				0x02010144,
				0x02010140,
				0x0201013c,
				0x02010138,
				0x02010134,
				0x02010130,
				0x0201012c,
				0x02010124,
				0x020100dc,
			};
			private static readonly uint[] CapEnemy_vtable =
			{
				0x02043c80,
				0x02011268,
				0x02011244,
				0x02043bf0,
				0x02011220,
				0x02011214,
				0x02043b24,
				0x02010fd4,
				0x02010fc8,
				0x02043af0,
				0x02010f78,
				0x02010f6c,
				0x02043ac0,
				0x0204357c,
				0x0204349c,
				0x02043494,
				uint.MaxValue,
				uint.MaxValue,
				0x02010160,
				0x02010154,
				0x0201014c,
				0x02010148,
				0x02010144,
				0x02010140,
				0x0201013c,
				0x02010138,
				0x02010134,
				0x02010130,
				0x0201012c,
				0x02010124,
				0x020100dc,
			};
			private static readonly uint[] Platform_vtable =
			{
				0x02043c80,
				0x02011268,
				0x02011244,
				0x02043bf0,
				0x02011220,
				0x02011214,
				0x02043b24,
				0x02010fd4,
				0x02010fc8,
				0x02043af0,
				0x02010f78,
				0x02010f6c,
				0x02043ac0,
				0x0204357c,
				0x0204349c,
				0x02043494,
				uint.MaxValue,
				uint.MaxValue,
				0x02010160,
				0x02010154,
				0x0201014c,
				0x02010148,
				0x02010144,
				0x02010140,
				0x0201013c,
				0x02010138,
				0x02010134,
				0x02010130,
				0x0201012c,
				0x02010124,
				0x020100dc,
				0x020ee55c,
			};
			private static readonly uint[] PathLiftBase_vtable =
			{
				0x02043c80,
				0x02011268,
				0x02011244,
				0x02043bf0,
				0x02011220,
				0x02011214,
				0x02043b24,
				0x02010fd4,
				0x02010fc8,
				0x02043af0,
				0x02010f78,
				0x02010f6c,
				0x02043ac0,
				0x0204357c,
				0x0204349c,
				0x02043494,
				uint.MaxValue,
				uint.MaxValue,
				0x02010160,
				0x02010154,
				0x0201014c,
				0x02010148,
				0x02010144,
				0x02010140,
				0x0201013c,
				0x02010138,
				0x02010134,
				0x02010130,
				0x0201012c,
				0x02010124,
				0x020100dc,
				0x020ee55c,
				0x020eff18,
			};

			// do not use the headers for these, still create them for documentation purposes
			private static readonly string[] ALWAYS_LOADED_STRUCTS =
			{
				"Camera",
				"HUD",
				"Minimap",
				"Player",
				"Number",
				"PowerStar",
				"StarMarker",
			};
		}

		public class OverlayData
		{
			public uint overlayID;
			public NitroOverlay overlay;
			public ObjData[] objDatas; // objs in the overlay

			public OverlayData(uint id)
			{
				overlayID = id;
				overlay = new NitroOverlay(Program.m_ROM, id);

				int count = TheData.Where(d => d.overlayID == id).Count();
				objDatas = new ObjData[count];

				int i = 0;
				foreach (StaticObjData staticObjData in TheData)
				{
					if (staticObjData.overlayID != id)
						continue;

					objDatas[i++] = new ObjData(staticObjData, overlay);
				}
			}
		}

		private static StaticObjData[] TheData =
		{
			new StaticObjData(0, uint.MaxValue, 0x020914a8),
			new StaticObjData(1, 7, 0x02103264),
			new StaticObjData(2, 3, 0x020b1380),
			new StaticObjData(3, uint.MaxValue, 0x0209213c),
			new StaticObjData(4, 3, 0x020b16b4),
			new StaticObjData(5, 5, 0x020c2440),
			new StaticObjData(6, 75, 0x0211c880),
			new StaticObjData(7, 75, 0x0211c788),
			new StaticObjData(8, 3, 0x020b1750),
			new StaticObjData(9, 2, 0x0210ad90),
			new StaticObjData(10, 2, 0x0210b248),
			new StaticObjData(11, 2, 0x02109900),
			new StaticObjData(12, 2, 0x0210991c),
			new StaticObjData(13, 64, 0x0211c4e8),
			new StaticObjData(14, 2, 0x02109d14),
			new StaticObjData(15, 2, 0x02108a24),
			new StaticObjData(16, 2, 0x02108a40),
			new StaticObjData(17, 2, 0x02108a78),
			new StaticObjData(18, 81, 0x02128ca0),
			new StaticObjData(19, 2, 0x02108a5c),
			new StaticObjData(20, 102, 0x0214e3e8),
			new StaticObjData(21, 102, 0x0214e404),
			new StaticObjData(22, 102, 0x0214e3cc),
			new StaticObjData(23, 102, 0x0214e420),
			new StaticObjData(24, 102, 0x0214e458),
			new StaticObjData(25, 102, 0x0214e43c),
			new StaticObjData(26, 2, 0x02108e38),
			new StaticObjData(27, 91, 0x02135298),
			new StaticObjData(28, 95, 0x021373dc),
			new StaticObjData(29, 91, 0x02134bf8),
			new StaticObjData(30, 91, 0x02134c14),
			new StaticObjData(31, 100, 0x02148558),
			new StaticObjData(32, 95, 0x021375cc),
			new StaticObjData(33, 95, 0x021375e8),
			new StaticObjData(34, 12, 0x02112320),
			new StaticObjData(35, 12, 0x021123e4),
			new StaticObjData(36, 10, 0x02112ac0),
			new StaticObjData(37, 10, 0x02112b84),
			new StaticObjData(38, 10, 0x02112c44),
			new StaticObjData(39, 95, 0x02137484),
			new StaticObjData(40, 14, 0x021145e4),
			new StaticObjData(41, 14, 0x0211488c),
			new StaticObjData(42, 15, 0x0211433c),
			new StaticObjData(43, 79, 0x02127f94),
			new StaticObjData(44, 15, 0x021143fc),
			new StaticObjData(45, 15, 0x021148b8),
			new StaticObjData(46, 2, 0x02108a94),
			new StaticObjData(47, 79, 0x0212808c),
			new StaticObjData(48, 79, 0x02128070),
			new StaticObjData(49, 102, 0x0214e118),
			new StaticObjData(50, 15, 0x021147c4),
			new StaticObjData(51, 15, 0x021146f0),
			new StaticObjData(52, 15, 0x0211462c),
			new StaticObjData(53, 15, 0x02114568),
			new StaticObjData(54, 15, 0x0211454c),
			new StaticObjData(55, 91, 0x02135008),
			new StaticObjData(56, 16, 0x021149fc),
			new StaticObjData(57, 16, 0x02114a18),
			new StaticObjData(58, 16, 0x02114adc),
			new StaticObjData(59, 102, 0x0214e0fc),
			new StaticObjData(60, 16, 0x02114ba8),
			new StaticObjData(61, 102, 0x0214e0e0),
			new StaticObjData(62, 17, 0x02111bd4),
			new StaticObjData(63, 18, 0x021138a8),
			new StaticObjData(64, 71, 0x02122eb0),
			new StaticObjData(65, 63, 0x0211ea10),
			new StaticObjData(66, 63, 0x0211eb34),
			new StaticObjData(67, 21, 0x021148f0),
			new StaticObjData(68, 21, 0x02114768),
			new StaticObjData(69, 64, 0x0211bc44),
			new StaticObjData(70, 22, 0x02114380),
			new StaticObjData(71, 22, 0x02113cf4),
			new StaticObjData(72, 64, 0x0211bd08),
			new StaticObjData(73, 22, 0x02113f4c),
			new StaticObjData(74, 22, 0x02113e88),
			new StaticObjData(75, 22, 0x021140ec),
			new StaticObjData(76, 22, 0x02114108),
			new StaticObjData(77, 22, 0x02114010),
			new StaticObjData(78, 64, 0x0211c160),
			new StaticObjData(79, 64, 0x0211c17c),
			new StaticObjData(80, 22, 0x02113dc4),
			new StaticObjData(81, 64, 0x0211bdec),
			new StaticObjData(82, 22, 0x021141cc),
			new StaticObjData(83, 22, 0x021142a0),
			new StaticObjData(84, 23, 0x02111fc0),
			new StaticObjData(85, 24, 0x02113804),
			new StaticObjData(86, 24, 0x02113820),
			new StaticObjData(87, 25, 0x021138f0),
			new StaticObjData(88, 25, 0x021139b0),
			new StaticObjData(89, 26, 0x02113abc),
			new StaticObjData(90, 26, 0x02113c48),
			new StaticObjData(91, 26, 0x02113b80),
			new StaticObjData(92, 27, 0x021137e4),
			new StaticObjData(93, 27, 0x02113800),
			new StaticObjData(94, 29, 0x02113ff4),
			new StaticObjData(95, 29, 0x02113c08),
			new StaticObjData(96, 29, 0x02113ccc),
			new StaticObjData(97, 29, 0x02113d90),
			new StaticObjData(98, 29, 0x0211417c),
			new StaticObjData(99, 29, 0x02113e50),
			new StaticObjData(100, 29, 0x02113f20),
			new StaticObjData(101, 29, 0x021140b8),
			new StaticObjData(102, 30, 0x02115a24),
			new StaticObjData(103, 30, 0x02115950),
			new StaticObjData(104, 33, 0x02112358),
			new StaticObjData(105, 33, 0x0211241c),
			new StaticObjData(106, 32, 0x021138bc),
			new StaticObjData(107, 32, 0x02113980),
			new StaticObjData(108, 65, 0x0211cfe8),
			new StaticObjData(109, 65, 0x0211d004),
			new StaticObjData(110, 65, 0x0211d0c8),
			new StaticObjData(111, 65, 0x0211d1c8),
			new StaticObjData(112, 65, 0x0211d1ac),
			new StaticObjData(113, 65, 0x0211d290),
			new StaticObjData(114, 65, 0x0211d374),
			new StaticObjData(115, 65, 0x0211d390),
			new StaticObjData(116, 65, 0x0211d454),
			new StaticObjData(117, 65, 0x0211d470),
			new StaticObjData(118, 65, 0x0211d544),
			new StaticObjData(119, 35, 0x02112aa4),
			new StaticObjData(120, 35, 0x02112ba8),
			new StaticObjData(121, 35, 0x02112ac0),
			new StaticObjData(122, 35, 0x02112adc),
			new StaticObjData(123, 36, 0x02113e6c),
			new StaticObjData(124, 91, 0x02134fb4),
			new StaticObjData(125, 36, 0x02113c14),
			new StaticObjData(126, 36, 0x02113cd4),
			new StaticObjData(127, 36, 0x02113a74),
			new StaticObjData(128, 95, 0x02137468),
			new StaticObjData(129, 36, 0x02113b50),
			new StaticObjData(130, 36, 0x02113f78),
			new StaticObjData(131, 95, 0x02137604),
			new StaticObjData(132, 36, 0x02113da8),
			new StaticObjData(133, 95, 0x021373f8),
			new StaticObjData(134, 43, 0x021124fc),
			new StaticObjData(135, 43, 0x02112294),
			new StaticObjData(136, 43, 0x02112438),
			new StaticObjData(137, 43, 0x02112368),
			new StaticObjData(138, 91, 0x02134f60),
			new StaticObjData(139, 45, 0x021130d0),
			new StaticObjData(140, 45, 0x02112cd0),
			new StaticObjData(141, 45, 0x02112ffc),
			new StaticObjData(142, 45, 0x02112d98),
			new StaticObjData(143, 95, 0x02137414),
			new StaticObjData(144, 91, 0x02134e9c),
			new StaticObjData(145, 45, 0x02112f2c),
			new StaticObjData(146, 91, 0x02134fec),
			new StaticObjData(147, 91, 0x02134fd0),
			new StaticObjData(148, 45, 0x02112e5c),
			new StaticObjData(149, 95, 0x02137430),
			new StaticObjData(150, 95, 0x0213744c),
			new StaticObjData(151, 47, 0x02112428),
			new StaticObjData(152, 47, 0x0211227c),
			new StaticObjData(153, 47, 0x021124ec),
			new StaticObjData(154, 91, 0x02134f7c),
			new StaticObjData(155, 91, 0x02134f98),
			new StaticObjData(156, 47, 0x02112358),
			new StaticObjData(157, 91, 0x02134e80),
			new StaticObjData(158, 63, 0x0211ea2c),
			new StaticObjData(159, 63, 0x0211ea48),
			new StaticObjData(160, 63, 0x0211ea64),
			new StaticObjData(161, 91, 0x02135150),
			new StaticObjData(162, 25, 0x0211382c),
			new StaticObjData(163, 25, 0x0211373c),
			new StaticObjData(164, 79, 0x02127c5c),
			new StaticObjData(165, 79, 0x02127c40),
			new StaticObjData(166, 60, 0x0211a88c),
			new StaticObjData(167, 60, 0x0211a964),
			new StaticObjData(168, 65, 0x0211cd84),
			new StaticObjData(169, 65, 0x0211cda0),
			new StaticObjData(170, 73, 0x02123168),
			new StaticObjData(171, 73, 0x02123184),
			new StaticObjData(172, 73, 0x021231a0),
			new StaticObjData(173, 56, 0x02113344),
			new StaticObjData(174, 52, 0x021124fc),
			new StaticObjData(175, 52, 0x021125cc),
			new StaticObjData(176, 66, 0x0211ad40),
			new StaticObjData(177, 2, 0x0210aa94),
			new StaticObjData(178, 2, 0x0210aa40),
			new StaticObjData(179, 2, 0x0210aa5c),
			new StaticObjData(180, 2, 0x0210aa78),
			new StaticObjData(181, 84, 0x02130a14),
			new StaticObjData(182, 58, 0x02111a4c),
			new StaticObjData(183, 85, 0x021303dc),
			new StaticObjData(184, 2, 0x02109ad4),
			new StaticObjData(185, 85, 0x0212fe6c),
			new StaticObjData(186, 85, 0x0212ff9c),
			new StaticObjData(187, 85, 0x021300d4),
			new StaticObjData(188, 62, 0x0211db78),
			new StaticObjData(189, 78, 0x02126e28),
			new StaticObjData(190, 62, 0x0211d9bc),
			new StaticObjData(191, 2, 0x0210a704),
			new StaticObjData(192, 2, 0x0210968c),
			new StaticObjData(193, 80, 0x02128174),
			new StaticObjData(194, 98, 0x0213c510),
			new StaticObjData(195, 55, 0x02111abc),
			new StaticObjData(196, 2, 0x021093bc),
			new StaticObjData(197, 2, 0x0210947c),
			new StaticObjData(198, 74, 0x02122e78),
			new StaticObjData(199, 74, 0x02122e94),
			new StaticObjData(200, 84, 0x021308ec),
			new StaticObjData(201, 84, 0x02130908),
			new StaticObjData(202, 84, 0x02130924),
			new StaticObjData(203, 62, 0x0211da74),
			new StaticObjData(204, 62, 0x0211da90),
			new StaticObjData(205, 62, 0x0211dc30),
			new StaticObjData(206, 102, 0x0214e534),
			new StaticObjData(207, 98, 0x0213c67c),
			new StaticObjData(208, 98, 0x0213c74c),
			new StaticObjData(209, 63, 0x0211e770),
			new StaticObjData(210, 63, 0x0211e78c),
			new StaticObjData(211, 63, 0x0211e7c4),
			new StaticObjData(212, 63, 0x0211e7a8),
			new StaticObjData(213, 20, 0x02114898),
			new StaticObjData(214, 64, 0x0211bec8),
			new StaticObjData(215, 64, 0x0211b818),
			new StaticObjData(216, 64, 0x0211b920),
			new StaticObjData(217, 27, 0x021138d8),
			new StaticObjData(218, 73, 0x0212306c),
			new StaticObjData(219, 14, 0x02114750),
			new StaticObjData(220, 100, 0x02147f58),
			new StaticObjData(221, 21, 0x02114824),
			new StaticObjData(222, 79, 0x02127ec4),
			new StaticObjData(223, 81, 0x021289cc),
			new StaticObjData(224, 81, 0x02128a98),
			new StaticObjData(225, 90, 0x021343c8),
			new StaticObjData(226, 90, 0x02134218),
			new StaticObjData(227, 90, 0x02134300),
			new StaticObjData(228, 32, 0x02113800),
			new StaticObjData(229, 85, 0x021301b4),
			new StaticObjData(230, 90, 0x02134144),
			new StaticObjData(231, 91, 0x02135388),
			new StaticObjData(232, 70, 0x02123144),
			new StaticObjData(233, 2, 0x0210d630),
			new StaticObjData(234, 94, 0x02136a34),
			new StaticObjData(235, 85, 0x02130320),
			new StaticObjData(236, 65, 0x0211cb80),
			new StaticObjData(237, 65, 0x0211cc7c),
			new StaticObjData(238, 77, 0x02127a74),
			new StaticObjData(239, 62, 0x0211dd38),
			new StaticObjData(240, 96, 0x02137998),
			new StaticObjData(241, 96, 0x021379b4),
			new StaticObjData(242, 16, 0x021148e0),
			new StaticObjData(243, 22, 0x02114458),
			new StaticObjData(244, 64, 0x0211c3fc),
			new StaticObjData(245, 64, 0x0211c310),
			new StaticObjData(246, 26, 0x02113d30),
			new StaticObjData(247, 26, 0x02113e00),
			new StaticObjData(248, 34, 0x02114498),
			new StaticObjData(249, 63, 0x0211ed10),
			new StaticObjData(250, 84, 0x02130c04),
			new StaticObjData(251, 84, 0x02130ae8),
			new StaticObjData(252, 84, 0x02130b04),
			new StaticObjData(253, 84, 0x02130acc),
			new StaticObjData(254, 2, 0x0210bf70),
			new StaticObjData(255, 71, 0x02122c08),
			new StaticObjData(256, 72, 0x02122a6c),
			new StaticObjData(257, 18, 0x02113998),
			new StaticObjData(258, 27, 0x02113a00),
			new StaticObjData(259, 19, 0x021132ec),
			new StaticObjData(260, 77, 0x02127960),
			new StaticObjData(261, 81, 0x02128be0),
			new StaticObjData(262, 71, 0x02122cf0),
			new StaticObjData(263, 71, 0x02122d0c),
			new StaticObjData(264, 71, 0x02122dc4),
			new StaticObjData(265, 77, 0x02127848),
			new StaticObjData(266, 70, 0x02123254),
			new StaticObjData(267, 30, 0x02115b90),
			new StaticObjData(268, 30, 0x02115bac),
			new StaticObjData(269, 2, 0x021095cc),
			new StaticObjData(270, 70, 0x0212334c),
			new StaticObjData(271, 70, 0x02123424),
			new StaticObjData(272, 72, 0x02122954),
			new StaticObjData(273, 72, 0x02122898),
			new StaticObjData(274, 72, 0x0212279c),
			new StaticObjData(275, 27, 0x02113b2c),
			new StaticObjData(276, 2, 0x02108388),
			new StaticObjData(277, 2, 0x021083a4),
			new StaticObjData(278, 60, 0x0211a610),
			new StaticObjData(279, 60, 0x0211a5f4),
			new StaticObjData(280, 60, 0x0211a7d0),
			new StaticObjData(281, 60, 0x0211ab30),
			new StaticObjData(282, 89, 0x02132b68),
			new StaticObjData(283, 89, 0x02132b84),
			new StaticObjData(284, 60, 0x0211aa68),
			new StaticObjData(285, 102, 0x0214e62c),
			new StaticObjData(286, 2, 0x0210abdc),
			new StaticObjData(287, 2, 0x0210845c),
			new StaticObjData(288, 2, 0x02108790),
			new StaticObjData(289, 2, 0x021087ac),
			new StaticObjData(290, 2, 0x021087c8),
			new StaticObjData(291, 2, 0x02108940),
			new StaticObjData(292, 13, 0x021121dc),
			new StaticObjData(293, 13, 0x021121c0),
			new StaticObjData(294, 13, 0x02112104),
			new StaticObjData(295, 18, 0x02113b10),
			new StaticObjData(296, 2, 0x02109b94),
			new StaticObjData(297, 2, 0x02109c50),
			new StaticObjData(298, 102, 0x0214e134),
			new StaticObjData(299, 98, 0x0213c398),
			new StaticObjData(300, 98, 0x0213c3b4),
			new StaticObjData(301, 44, 0x021115e0),
			new StaticObjData(302, 31, 0x0211190c),
			new StaticObjData(303, 31, 0x02111928),
			new StaticObjData(304, 31, 0x02111944),
			new StaticObjData(305, 31, 0x02111960),
			new StaticObjData(306, 2, 0x021097a0),
			new StaticObjData(307, 80, 0x02128290),
			new StaticObjData(308, 96, 0x02137a6c),
			new StaticObjData(309, 92, 0x021322ac),
			new StaticObjData(310, 80, 0x02127fec),
			new StaticObjData(311, 80, 0x02128008),
			new StaticObjData(312, 81, 0x02128858),
			new StaticObjData(313, 16, 0x02114c68),
			new StaticObjData(314, 39, 0x02111834),
			new StaticObjData(315, 64, 0x0211c5a4),
			new StaticObjData(316, 2, 0x02108ef8),
			new StaticObjData(317, 2, 0x02108f14),
			new StaticObjData(318, 95, 0x021376cc),
			new StaticObjData(319, 2, 0x02108cd0),
			new StaticObjData(320, 2, 0x02108cb4),
			new StaticObjData(321, 2, 0x02108bf4),
			new StaticObjData(322, 2, 0x02108ba0),
			new StaticObjData(323, 2, 0x02108bbc),
			new StaticObjData(324, 2, 0x02108bd8),
			new StaticObjData(325, 20, 0x021148b4),
			new StaticObjData(326, 20, 0x021149f0),
			new StaticObjData(327, 20, 0x02114860),
			new StaticObjData(328, 20, 0x0211487c),
			new StaticObjData(329, 2, 0x0210b00c),
			new StaticObjData(330, 2, 0x0210b0c8),
			new StaticObjData(331, 2, 0x0210b188),
			new StaticObjData(332, uint.MaxValue, 0x02086d78),
			new StaticObjData(333, 2, 0x02108518),
			new StaticObjData(334, 2, 0x0210c210),
			new StaticObjData(335, 2, 0x0210c160),
			new StaticObjData(336, 100, 0x02147e78),
			new StaticObjData(337, 100, 0x02148030),
			new StaticObjData(338, 9, 0x021139f4),
			new StaticObjData(339, 9, 0x02113abc),
			new StaticObjData(340, 102, 0x0214e150),
			new StaticObjData(341, 102, 0x0214e16c),
			new StaticObjData(342, 9, 0x02113b7c),
			new StaticObjData(343, 9, 0x02113934),
			new StaticObjData(344, 100, 0x02148498),
			new StaticObjData(345, 2, 0x02108884),
			new StaticObjData(346, 75, 0x0211c610),
			new StaticObjData(347, 2, 0x0210ac98),
			new StaticObjData(348, 2, 0x021085d4),
			new StaticObjData(349, 2, 0x02108690),
			new StaticObjData(350, 2, 0x0210b47c),
			new StaticObjData(351, 2, 0x0210b560),
			new StaticObjData(352, 2, 0x0210ba60),
			new StaticObjData(353, 100, 0x02148164),
			new StaticObjData(354, 100, 0x021483a8),
			new StaticObjData(355, 18, 0x02113a50),
			new StaticObjData(356, 19, 0x021133a8),
			new StaticObjData(357, 2, 0x0210b324),
			new StaticObjData(358, 2, 0x0210b340),
			new StaticObjData(359, 2, 0x0210c064),
			new StaticObjData(360, uint.MaxValue, 0x0209435c),
			new StaticObjData(361, 6, 0x0213c020),
			new StaticObjData(362, 6, 0x0213cfd8),
			new StaticObjData(363, 6, 0x0213d288),
			new StaticObjData(364, 6, 0x0213e560),
			new StaticObjData(365, 6, 0x0213e508),
			new StaticObjData(366, 6, 0x0213ce0c),
			new StaticObjData(367, 6, 0x0213f69c),
			new StaticObjData(368, 6, 0x0213d910),
			new StaticObjData(369, 6, 0x0213da64),
			new StaticObjData(370, 6, 0x0213badc),
			new StaticObjData(371, 6, 0x0213b814),
			new StaticObjData(372, 6, 0x0213cb64),
			new StaticObjData(373, 6, 0x0213cc7c),
			new StaticObjData(374, 6, 0x0213c214),
			new StaticObjData(375, 6, 0x0213c434),
			new StaticObjData(376, 6, 0x0213ebd0),
			new StaticObjData(377, 6, 0x0213ffb8),
			new StaticObjData(378, 6, 0x0213bedc),
			new StaticObjData(379, 6, 0x0213bc50),
			new StaticObjData(380, 6, 0x0213dc64),
			new StaticObjData(381, 6, 0x0213d580),
			new StaticObjData(382, 6, 0x0213d70c),
			new StaticObjData(383, 6, 0x0213e2f0),
			new StaticObjData(384, 6, 0x0213fab8),
			new StaticObjData(385, 6, 0x0213fbc8),
			new StaticObjData(386, 6, 0x0213c98c),
			new StaticObjData(387, 6, 0x0213f974),
			new StaticObjData(388, 6, 0x0213fd6c),
			new StaticObjData(389, 6, 0x0213c78c),
			new StaticObjData(390, 6, 0x02140114),
		};

		// private static uint[] overlaysToProcess = { 2 };
		private static uint[] overlaysToProcess = { 77, 79, 80, 81, 85, 89, 90, 91, 92, 95, 96, 98, 100, 102 };
		// private static uint[] overlaysToProcess = { 77, 79 };
		private static List<OverlayData> overlayDatas = new List<OverlayData>();

		public static void Run(string objCodeDir)
		{
			foreach (uint overlayID in overlaysToProcess)
				overlayDatas.Add(new OverlayData(overlayID));

			List<(uint, uint)> vtables = new List<(uint, uint)>(); // do not add 2 code files for actors with the same vtable
			List<string> codeDirs = new List<string>();

			codeDirs.Add("generate_file_list  \"code/include/List/FileList.h\"");
			codeDirs.Add("generate_sound_list \"code/include/List/SoundList.h\"");
			codeDirs.Add("generate_actor_list \"code/include/List/ActorList.h\"");
			codeDirs.Add("");

			foreach (OverlayData overlayData in overlayDatas)
			{
				foreach (ObjData objData in overlayData.objDatas)
				{
					uint vtableAddr = objData.vtableAddr;
					uint ovID = objData.staticData.overlayID;
					uint actorID = objData.staticData.actorID;

					if (vtables.Contains((vtableAddr, ovID)))
						continue;

					IEnumerable<ObjData> objsWithSharedVtable = overlayData.objDatas.Where(o => o.vtableAddr == vtableAddr && o.staticData.actorID != actorID);

					if (!objsWithSharedVtable.Any())
						objsWithSharedVtable = null;

					string s = objData.Write(objCodeDir, objsWithSharedVtable);
					objData.WriteDL(objCodeDir, objsWithSharedVtable);
					objData.WriteHeader(objCodeDir, objsWithSharedVtable);

					vtables.Add((vtableAddr, ovID));
					codeDirs.Add("compile test \"code\" \"dynamic/decomp_maker/" + s + "\"");
				}
			}

			File.WriteAllLines(Program.m_ROMPath.Remove(Program.m_ROMPath.LastIndexOf('\\')) + "/compile_test_decomp_maker.ccs", codeDirs);
		}
	}
}
