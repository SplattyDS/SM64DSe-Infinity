using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SM64DSe
{
    public class ScriptInfo
    {
        public static ScriptInfo[] KUPPA_SCRIPTS =
        {
            new ScriptInfo(0x020890a0, "Intro 1.0"),
            new ScriptInfo(0x02088a94, "Intro 1.1"),
            new ScriptInfo(0x02089af8, "Intro 1.2"),
            new ScriptInfo(0x020876d4, "Intro 2"),
            new ScriptInfo(0x02088610, "Ending 1.0"),
            new ScriptInfo(0x02087740, "Ending 1.1"),
            new ScriptInfo(0x02089608, "Ending 2.0"),
            new ScriptInfo(0x02088340, "Ending 2.1"),
            new ScriptInfo(0x0208771c, "Ending 2.2"),
            new ScriptInfo(0x02087f40, "Ending 2.3"),
            new ScriptInfo(0x020876fc, "Ending 2.4"),
            new ScriptInfo(0x0208894c, "Ending 2.5"),
            new ScriptInfo(0x02088ccc, "Ending 2.6"),
            new ScriptInfo(0x02088ba0, "Ending 2.7"),
            new ScriptInfo(0x02089850, "Ending 2.8"),
            new ScriptInfo(0x02088a18, "Ending 2.9"),
            new ScriptInfo(0x02088ef0, "Ending 2.10"),
            new ScriptInfo(0x02089dcc, "Ending 2.11"),
            new ScriptInfo(0x02087c00, "Ending 3.0 (BOB)"),
            new ScriptInfo(0x02087bcc, "Ending 3.1 (WF)"),
            new ScriptInfo(0x02087b98, "Ending 3.2 (JRB)"),
            new ScriptInfo(0x02087c34, "Ending 3.3 (CCM-S)"),
            new ScriptInfo(0x02087b64, "Ending 3.4 (BBH)"),
            new ScriptInfo(0x02087b30, "Ending 3.5 (HMC)"),
            new ScriptInfo(0x02087afc, "Ending 3.6 (THI-C)"),
            new ScriptInfo(0x02087ac8, "Ending 3.7 (LLL-V)"),
            new ScriptInfo(0x02087a94, "Ending 3.8 (SSL)"),
            new ScriptInfo(0x02087a60, "Ending 3.9 (DDD)"),
            new ScriptInfo(0x02087a2c, "Ending 3.10 (SL)"),
            new ScriptInfo(0x020879f8, "Ending 3.11 (WDW)"),
            new ScriptInfo(0x020879c4, "Ending 3.12 (TTM)"),
            new ScriptInfo(0x02087990, "Ending 3.13 (THI-H)"),
            new ScriptInfo(0x0208795c, "Ending 3.14 (TTC)"),
            new ScriptInfo(0x02087928, "Ending 3.15 (RR)"),
            new ScriptInfo(0x020878f4, "Ending 3.16 (SA)"),
            new ScriptInfo(0x020878c0, "Ending 3.17 (BTW)"),
            new ScriptInfo(0x0208788c, "Ending 3.18 (DDD 2)"),
            new ScriptInfo(0x02087858, "Ending 3.19 (CCM)"),
            new ScriptInfo(0x02088fb8, "Ending 4.0"),
            new ScriptInfo(0x02088388, "Star 1"),
            new ScriptInfo(0x020888f0, "Star 2"),
            new ScriptInfo(0x0208776c, "Star 3"),
            new ScriptInfo(0x020889b0, "Star 4"),
            new ScriptInfo(0x0208776c, "Star 5"),
            new ScriptInfo(0x02088b14, "Star 6"),
        };

        public uint address;
        public uint length;
        public string name;

        public ScriptInfo(uint address, string name)
        {
            this.address = address;
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }

        public string GetVariableName()
		{
            return "KS_" + name.Replace('.', '_').Replace(' ', '_').Replace('-', '_').Replace("(", "").Replace(")", "");
        }

        public string GetScriptCode()
        {
            Program.m_ROM.BeginRW();

            uint romOffset = address - 0x02000000;

            List<string> ret = new List<string>();

            ret.Add($"constinit auto {GetVariableName()} =");
            ret.Add("\tNewScript().");

            byte length = ReadByte(romOffset, 0);

            while (length != 0)
            {
                byte type = ReadByte(romOffset, 1);

                switch (type)
                {
                    // IT_PLAYER
                    case 0x0:
                    case 0x1:
                    case 0x2:
                    case 0x3:
                        ret.Add('\t' + PlayerInstruction(romOffset, type));
                        break;
                    // IT_CAMERA
                    case 0x4:
                        ret.Add('\t' + CameraInstruction(romOffset));
                        break;
                    // IT_END_AFTER_TOUCH
                    case 0x5:
                        ret.Add($"\tEndAfterTouch() ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_CHANGE_SCRIPT
                    case 0x6:
                        ret.Add($"\tChangeScript({ReadScriptPointer(romOffset, 6)}) ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_CHANGE_MUSIC
                    case 0x7:
                        ret.Add($"\tChangeMusic({ReadUint(romOffset, 6)}) ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_PLAY_SEQARC_1
                    case 0x8:
                        ret.Add($"\tPlaySoundSSAR1({ReadUint(romOffset, 6)}) ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_PLAY_SEQARC_2
                    case 0x9:
                        ret.Add($"\tPlaySoundSSAR2({ReadUint(romOffset, 6)}) ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_DISPLAY_MESSAGE
                    case 0xa:
                        ret.Add($"\tDisplayMessage({ReadShort(romOffset, 6)}) ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_CHANGE_LEVEL
                    case 0xb:
                        ret.Add($"\tChangeLevel({ReadByte(romOffset, 6)}, {ReadByte(romOffset, 7)}, {ReadByte(romOffset, 8)}, {ReadScriptPointer(romOffset, 9)}) ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_FADE_TO_WHITE
                    case 0xc:
                        ret.Add($"\tFadeToWhite() ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_FADE_FROM_WHITE
                    case 0xd:
                        ret.Add($"\tFadeFromWhite() ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_FADE_TO_BLACK
                    case 0xe:
                        ret.Add($"\tFadeToBlack() ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_FADE_FROM_WHITE_BROKEN
                    case 0xf:
                        ret.Add($"\tFadeFromWhiteBroken() ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_FADE_TO_BLACK_AND_BACK
                    case 0x10:
                        ret.Add($"\tFadeToBlackAndBack() ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_ENABLE_WATERFALL_SFX
                    case 0x11:
                        ret.Add($"\tEnableWaterfallSFX() ({ReadFrameData(romOffset, 2)}).");
                        break;
                    // IT_CUTSCENE_OBJECT
                    case 0x12:
                    case 0x13:
                    case 0x14:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                    case 0x18:
                    case 0x19:
                    case 0x1a:
                    case 0x1b:
                    case 0x1c:
                    case 0x1d:
                    case 0x1e:
                    case 0x1f:
                    case 0x20:
                    case 0x21:
                    case 0x22:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                    case 0x27:
                    case 0x28:
                    case 0x29:
                    case 0x2a:
                    case 0x2b:
                    case 0x2c:
                    case 0x2d:
                    case 0x2e:
                    case 0x2f:
                        ret.Add('\t' + ObjectInstruction(romOffset, type));
                        break;
                    default:
                        throw new Exception($"Unknown instruction type: {type}");
                }

                romOffset += length;
                length = ReadByte(romOffset, 0);
            }

            ret.Add("\tEnd();");

            Program.m_ROM.EndRW();

            this.length = romOffset - (address - 0x02000000);
            this.length += 4 - (this.length % 4);

            return string.Join("\r\n", ret);
        }

        public string PlayerInstruction(uint instructionOffset, byte playerID)
        {
            byte func = ReadByte(instructionOffset, 6);
            uint paramOffset = instructionOffset + 7;

            switch (func)
			{
                case 0:
                    return $"SetPlayerPosAndAngleY<{GetPlayerName(playerID)}>({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}, {ReadDeg(paramOffset, 6)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 1:
                    return $"SendPlayerInput<{GetPlayerName(playerID)}>({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}, {ReadFix12s(paramOffset, 6)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 2:
                    return $"OrrPlayerFlags<{GetPlayerName(playerID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 3:
                    return $"MakePlayerLieDown<{GetPlayerName(playerID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 4:
                    return $"PlayPlayerVoice<{GetPlayerName(playerID)}>({ReadUint(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 5:
                    return $"PlayerPlaySoundSSAR0<{GetPlayerName(playerID)}>({ReadUint(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 6:
                    return $"PlayerPlaySoundSSAR3<{GetPlayerName(playerID)}>({ReadUint(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 7:
                    return $"PlayerHoldButtons<{GetPlayerName(playerID)}>({ReadButton(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 8:
                    return $"GivePlayerWingsAndDrop<{GetPlayerName(playerID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 9:
                    return $"HurtPlayer<{GetPlayerName(playerID)}>({ReadDeg(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 10:
                    return $"AnimatePlayerCap<{GetPlayerName(playerID)}>({ReadByte(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 11:
                    return $"TurnPlayerDec<{GetPlayerName(playerID)}>({ReadDeg(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 12:
                    return $"MovePlayerForward<{GetPlayerName(playerID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 13:
                    return $"KillPlayer<{GetPlayerName(playerID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                default:
                    throw new Exception("Unknown player function id: " + func);
            }
		}

        public string CameraInstruction(uint instructionOffset)
        {
            byte func = ReadByte(instructionOffset, 6);
            uint paramOffset = instructionOffset + 7;

            switch (func)
            {
                case 0:
                    return $"SetCamTarget({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 1:
                    return $"SetCamPos({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 2:
                    return $"SetCamTargetAndPos({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}, {ReadShort(paramOffset, 6)}, {ReadShort(paramOffset, 8)}, {ReadShort(paramOffset, 10)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 3:
                    return $"SetCamFOV({ReadUshort(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 4:
                    return $"AdjustCamFOV({ReadUshort(paramOffset, 0)}, {ReadUshort(paramOffset, 2)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 5:
                    return $"AdjustCamScreenSize({ReadByte(paramOffset, 0)}, {ReadByte(paramOffset, 1)}, {ReadByte(paramOffset, 2)}, {ReadByte(paramOffset, 3)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 6:
                    return $"UnkCubicInterpolation({ReadPointer(paramOffset, 0)}, {ReadPointer(paramOffset, 4)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 7:
                    return $"CamInstruction7() ({ReadFrameData(instructionOffset, 2)}).";
                case 8:
                    return $"CamInstruction8() ({ReadFrameData(instructionOffset, 2)}).";
                case 9:
                    return $"CamInstruction9() ({ReadFrameData(instructionOffset, 2)}).";
                case 10:
                    return $"CamInstruction10() ({ReadFrameData(instructionOffset, 2)}).";
                case 11:
                    return $"CamInstruction11() ({ReadFrameData(instructionOffset, 2)}).";
                case 12:
                    return $"CamInstruction12() ({ReadFrameData(instructionOffset, 2)}).";
                case 13:
                    return $"SetStoredFix12({ReadFix12i(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 14:
                    return $"AdjustStoredFix12({ReadFix12i(paramOffset, 0)}, {ReadFix12i(paramOffset, 4)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 15:
                    return $"AdjustCamTargetDec({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}, {ReadFix12s(paramOffset, 6)}, {ReadFix12s(paramOffset, 8)}, {ReadFix12s(paramOffset, 10)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 16:
                    return $"AdjustCamPosDec({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}, {ReadFix12s(paramOffset, 6)}, {ReadFix12s(paramOffset, 8)}, {ReadFix12s(paramOffset, 10)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 17:
                    return $"ResetCamToPause() ({ReadFrameData(instructionOffset, 2)}).";
                case 18:
                    return $"SetCamTargetRelativeDec({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}, {ReadByte(paramOffset, 6)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 19:
                    return $"AdjustCamByOwnerAngleDec({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}, {ReadByte(paramOffset, 6)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 20:
                    return $"CamPosAngleRelativeDec({ReadUshort(paramOffset, 0)}, {ReadByte(paramOffset, 2)}, {ReadDeg(paramOffset, 3)}, {ReadByte(paramOffset, 5)}, {ReadDeg(paramOffset, 6, true)}, {ReadByte(paramOffset, 8)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 21:
                    return $"SpinCamTarget({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 22:
                    return $"SpinCamAroundOwnerPos({ReadFix12i(paramOffset, 0)}, {ReadShort(paramOffset, 4)}, {ReadShort(paramOffset, 6)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 23:
                    return $"AdjustCamFOVDec({ReadUshort(paramOffset, 0)}, {ReadByte(paramOffset, 2)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 24:
                    return $"AdjustCamFOVIfBigger({ReadUshort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 25:
                    return $"CamInstruction25() ({ReadFrameData(instructionOffset, 2)}).";
                case 26:
                    return $"CamApproachPlayerFromTop() ({ReadFrameData(instructionOffset, 2)}).";
                case 27:
                    return $"SetCamTargetAndPosRotatedFromOwner({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}, {ReadShort(paramOffset, 6)}, {ReadShort(paramOffset, 8)}, {ReadShort(paramOffset, 10)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 28:
                    return $"CamInstruction28() ({ReadFrameData(instructionOffset, 2)}).";
                case 29:
                    return $"CamInstruction29() ({ReadFrameData(instructionOffset, 2)}).";
                case 30:
                    return $"CamInstruction30() ({ReadFrameData(instructionOffset, 2)}).";
                case 31:
                    return $"CamInstruction31() ({ReadFrameData(instructionOffset, 2)}).";
                case 32:
                    return $"CamInstruction32() ({ReadFrameData(instructionOffset, 2)}).";
                case 33:
                    return $"CamInstruction33() ({ReadFrameData(instructionOffset, 2)}).";
                case 34:
                    return $"CamInstruction34() ({ReadFrameData(instructionOffset, 2)}).";
                case 35:
                    return $"CamInstruction35() ({ReadFrameData(instructionOffset, 2)}).";
                case 36:
                    return $"CamInstruction36() ({ReadFrameData(instructionOffset, 2)}).";
                case 37:
                    return $"CamInstruction37() ({ReadFrameData(instructionOffset, 2)}).";
                case 38:
                    return $"CamInstruction38() ({ReadFrameData(instructionOffset, 2)}).";
                default:
                    throw new Exception("Unknown camera function id: " + func);
            }
		}

        public string ObjectInstruction(uint instructionOffset, byte objectID)
		{
            byte func = ReadByte(instructionOffset, 6);
            uint paramOffset = instructionOffset + 7;

            switch (func)
            {
                case 0:
                    return $"MoveLakituIntro<{GetObjectName(objectID)}>({ReadPointer(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 1:
                    return $"RotateLakituIntro<{GetObjectName(objectID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 2:
                    return $"MoveLakituEnding<{GetObjectName(objectID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 3:
                    return $"MovePeachIntro<{GetObjectName(objectID)}>({ReadByte(paramOffset, 0)}, {ReadByte(paramOffset, 1)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 4:
                    return $"UpdatePeachLetter<{GetObjectName(objectID)}>({ReadByte(paramOffset, 0)}, {ReadByte(paramOffset, 1)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 5:
                    return $"MoveWarpPipe<{GetObjectName(objectID)}>({ReadBool(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 6:
                    return $"UpdateBigStar<{GetObjectName(objectID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 7:
                    return $"FUN_020f71c4<{GetObjectName(objectID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 8:
                    return $"FUN_020f7038<{GetObjectName(objectID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 9:
                    return $"FUN_020f7020<{GetObjectName(objectID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 10:
                    return $"FUN_020f6e48<{GetObjectName(objectID)}>({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}, {ReadDeg(paramOffset, 6)}, {ReadDeg(paramOffset, 8)}, {ReadDeg(paramOffset, 10)}, {ReadByte(paramOffset, 12)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 11:
                    return $"FUN_020f6c60<{GetObjectName(objectID)}>({ReadBool(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 12:
                    return $"FUN_020f6c34<{GetObjectName(objectID)}>({ReadByte(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 13:
                    return $"FUN_020f6c24<{GetObjectName(objectID)}>({ReadByte(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 14:
                    return $"FUN_020f6bc0<{GetObjectName(objectID)}>({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}, {ReadDeg(paramOffset, 6)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 15:
                    return $"FUN_020f6b4c<{GetObjectName(objectID)}>({ReadShort(paramOffset, 0)}, {ReadShort(paramOffset, 2)}, {ReadShort(paramOffset, 4)}, {ReadFix12s(paramOffset, 6)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 16:
                    return $"FUN_020f6b28<{GetObjectName(objectID)}>({ReadUint(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 17:
                    return $"FUN_020f6ae4<{GetObjectName(objectID)}>({ReadUint(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 18:
                    return $"FUN_020f6ab8<{GetObjectName(objectID)}>({ReadUint(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 19:
                    return $"FUN_020f6a9c<{GetObjectName(objectID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 20:
                    return $"InitEndingManager<{GetObjectName(objectID)}>({ReadByte(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 21:
                    return $"CallEndingInstruction<{GetObjectName(objectID)}>({ReadByte(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                case 22:
                    return $"InitIntroManager<{GetObjectName(objectID)}>() ({ReadFrameData(instructionOffset, 2)}).";
                case 23:
                    return $"SetCutsceneID<{GetObjectName(objectID)}>({ReadByte(paramOffset, 0)}) ({ReadFrameData(instructionOffset, 2)}).";
                default:
                    throw new Exception("Unknown player function id: " + func);
            }
        }

        public string GetPlayerName(byte playerID)
		{
            switch (playerID)
			{
                case 0:
                    return "Mario";
                case 1:
                    return "Luigi";
                case 2:
                    return "Wario";
                case 3:
                    return "Yoshi";
                default:
                    throw new Exception("Unknown player id: " + playerID);
			}
		}

        public string GetObjectName(byte objectID)
		{
            switch (objectID)
			{
                case 18:
                    return "LakituBro";
                case 19:
                    return "Peach";
                case 20:
                    return "Pipe_Mario";
                case 21:
                    return "Pipe_Luigi";
                case 22:
                    return "Pipe_Wario";
                case 23:
                    return "PeachLetter";
                case 24:
                    return "Star";
                case 25:
                    return "IntroClouds";
                case 26:
                    return "Bird_0";
                case 27:
                    return "Bird_1";
                case 28:
                    return "Bird_2";
                case 29:
                    return "Bird_3";
                case 30:
                    return "Bird_4";
                case 31:
                    return "Bird_5";
                case 32:
                    return "Bird_6";
                case 33:
                    return "Bird_7";
                case 34:
                    return "Bird_8";
                case 35:
                    return "Bird_9";
                case 36:
                    return "Bird_10";
                case 37:
                    return "Bird_11";
                case 38:
                    return "Bird_12";
                case 39:
                    return "Bird_13";
                case 40:
                    return "Bird_14";
                case 41:
                    return "Bird_15";
                case 42:
                    return "Bird_16";
                case 43:
                    return "Bird_17";
                case 44:
                    return "Bird_18";
                case 45:
                    return "Bird_19";
                case 46:
                    return "Ending";
                case 47:
                    return "MinimapClouds";
                default:
                    throw new Exception("Unknown object id: " + objectID);
			}
		}

        public byte ReadByte(uint instructionOffset, uint offset)
        {
            return Program.m_ROM.Read8(instructionOffset + offset);
        }

        public ushort ReadUshort(uint instructionOffset, uint offset)
        {
            return Program.m_ROM.Read16(instructionOffset + offset);
        }

        public short ReadShort(uint instructionOffset, uint offset)
        {
            return (short)Program.m_ROM.Read16(instructionOffset + offset);
        }

        public int ReadInt(uint instructionOffset, uint offset)
        {
            return (int)Program.m_ROM.Read32(instructionOffset + offset);
        }

        public uint ReadUint(uint instructionOffset, uint offset)
        {
            return Program.m_ROM.Read32(instructionOffset + offset);
        }

        public string ReadBool(uint instructionOffset, uint offset)
		{
            return ReadByte(instructionOffset, offset) != 0 ? "true" : "false";
		}

        public string ReadFix12s(uint instructionOffset, uint offset)
        {
            float fval = ReadShort(instructionOffset, offset) / 4096.0f;
            fval = RoundUp(fval, 4);
            string ret = fval.ToString().Replace(',', '.');
            ret += ret.Contains('.') ? "_fs" : "._fs";
            return ret;
		}

        public string ReadFix12i(uint instructionOffset, uint offset)
        {
            float fval = ReadInt(instructionOffset, offset) / 4096.0f;
            fval = RoundUp(fval, 4);
            string ret = fval.ToString().Replace(',', '.');
            ret += ret.Contains('.') ? "_f" : "._f";
            return ret;
		}

        public string ReadDeg(uint instructionOffset, uint offset, bool skipMinusOne = false)
        {
            short sval = ReadShort(instructionOffset, offset);

            if (skipMinusOne && sval == -1)
                return "-1";

            float fval = sval * 180.0f / 32768.0f;
            string ret = fval.ToString().Replace(',', '.');
            ret += ret.Contains('.') ? "_deg" : "._deg";
            return ret;
        }

        public string ReadButton(uint instructionOffset, uint offset)
        {
            ushort val = ReadUshort(instructionOffset, offset);
            string ret = "";

            if ((val & (1 << 0)) != 0)
                ret += " | Input::A";
            if ((val & (1 << 1)) != 0)
                ret += " | Input::B";
            if ((val & (1 << 2)) != 0)
                ret += " | Input::Select";
            if ((val & (1 << 3)) != 0)
                ret += " | Input::Start";
            if ((val & (1 << 4)) != 0)
                ret += " | Input::Right";
            if ((val & (1 << 5)) != 0)
                ret += " | Input::Left";
            if ((val & (1 << 6)) != 0)
                ret += " | Input::Up";
            if ((val & (1 << 7)) != 0)
                ret += " | Input::Down";
            if ((val & (1 << 8)) != 0)
                ret += " | Input::R";
            if ((val & (1 << 9)) != 0)
                ret += " | Input::L";
            if ((val & (1 << 10)) != 0)
                ret += " | Input::X";
            if ((val & (1 << 11)) != 0)
                ret += " | Input::Y";

            return ret.TrimStart(' ', '|', ' ');
        }

        public string ReadPointer(uint instructionOffset, uint offset, string type = "void")
		{
            uint addr = ReadUint(instructionOffset, offset);

            if (addr == 0)
                return "nullptr";

            return $"&DAT_{Convert.ToString(addr, 16).ToLower().PadLeft(8, '0')}";
		}

        public string ReadFrameData(uint instructionOffset, uint offset)
        {
            short minFrame = ReadShort(instructionOffset, offset);
            short maxFrame = ReadShort(instructionOffset, offset + 2);

            if (minFrame == maxFrame)
                return $"{minFrame}";

            return $"{minFrame}, {maxFrame}";
        }

        public string ReadScriptPointer(uint instructionOffset, uint offset)
        {
            uint addr = ReadUint(instructionOffset, offset);

            foreach (ScriptInfo script in KUPPA_SCRIPTS)
			{
                if (addr == script.address)
                    return $"&{script.GetVariableName()}_Ptr";
			}

            throw new Exception($"Unknown script at: 0x{Convert.ToString(addr, 16).ToLower()}");
        }

        public static float RoundUp(float input, int places)
        {
            // make 0.09985352 round up to 0.1 instead of 0.0999
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            double roundedUp = Math.Ceiling(input * multiplier) / multiplier;

            double lastDigit = roundedUp - Math.Truncate(roundedUp);
            lastDigit *= 10000;
            lastDigit = Math.Truncate(lastDigit);
            lastDigit %= 10;

            if (lastDigit == 9)
                roundedUp += 0.0001;
            if (lastDigit == 8)
                roundedUp += 0.0002;

            return (float)roundedUp;
        }
    }

    public partial class KuppaScriptDecompilerForm : Form
    {
        public KuppaScriptDecompilerForm()
        {
            InitializeComponent();

            lstScripts.Items.AddRange(ScriptInfo.KUPPA_SCRIPTS);
            lstScripts.Items.Add("All");
        }

		private void lstScripts_SelectedIndexChanged(object sender, EventArgs e)
		{
            if (lstScripts.SelectedItem == null)
                return;

            if (lstScripts.SelectedIndex == lstScripts.Items.Count - 1)
            {
                string text = "extern \"C\"\r\n{";

                for (int i = 0; i < ScriptInfo.KUPPA_SCRIPTS.Count(); i++)
				{
                    text += $"\r\n\textern void* {ScriptInfo.KUPPA_SCRIPTS[i].GetVariableName()}_Ptr;";
                }

                text += "\r\n\t\r\n\tvoid InitKuppaScriptPointers();\r\n}\r\n";

                for (int i = 0; i < ScriptInfo.KUPPA_SCRIPTS.Count(); i++)
				{
                    text += $"\r\n\r\n{ScriptInfo.KUPPA_SCRIPTS[i].GetScriptCode()}";
                }

                text += "\r\n\r\nextern \"C\"\r\n{\r\n\tvoid InitKuppaScriptPointers()\r\n\t{";

                for (int i = 0; i < ScriptInfo.KUPPA_SCRIPTS.Count(); i++)
                {
                    text += $"\r\n\t\t{ScriptInfo.KUPPA_SCRIPTS[i].GetVariableName()}_Ptr = &{ScriptInfo.KUPPA_SCRIPTS[i].GetVariableName()};";
                }

                text += "\r\n\t}\r\n}";
                
                text += "\r\n";
                for (int i = 0; i < ScriptInfo.KUPPA_SCRIPTS.Count(); i++)
                    text += $"\r\n0x{Convert.ToString(ScriptInfo.KUPPA_SCRIPTS[i].address, 16).ToLower().PadLeft(8, '0')}, 0x{Convert.ToString(ScriptInfo.KUPPA_SCRIPTS[i].length, 16).ToLower().PadLeft(8, '0')}, \"{ScriptInfo.KUPPA_SCRIPTS[i].name}\"";

                txtCode.Text = text;

                return;
            }

            ScriptInfo script = (ScriptInfo)lstScripts.SelectedItem;

            txtCode.Text = script.GetScriptCode();
        }
	}
}
