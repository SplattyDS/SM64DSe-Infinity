using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SM64DSe.Patcher
{
	public static class OvInfoMaker
	{
		public class StaticObjData
		{
			public int actorID;
			public uint overlayID;
			public uint spawnInfoAddr;

			public StaticObjData(int actorID, uint overlayID, uint spawnInfoAddr)
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
			public uint constructorAddr;
			public short behavPriority;
			public short renderPriority;
			public uint flags;
			public Fix12i rangeOffsetY;
			public Fix12i range;
			public Fix12i drawDist;
			public Fix12i unk18;

			public ObjData(StaticObjData staticData, NitroOverlay overlay)
			{
				this.staticData = staticData;

				var dbRes = ObjectDatabase.m_ObjectInfo.Where(o => (uint)o.m_ActorID == staticData.actorID);
				if (dbRes.Any())
					name = dbRes.Single().m_Name;
				else // for actors that don't have an object id
					name = ObjectDatabase.m_OtherActorInfo.Where(o => (uint)o.m_ActorID == staticData.actorID).Single().m_Name;

				uint overlayAddr = overlay.GetRAMAddr();
				uint spawnInfoAddr = staticData.spawnInfoAddr;
				uint spawnInfoOffset = spawnInfoAddr - overlayAddr;

				if (spawnInfoOffset > overlay.GetSize())
					throw new Exception($"Actor 0x{staticData.actorID.ToString("X")} was not found in its overlay.");

				constructorAddr = overlay.Read32(spawnInfoOffset + 0x00);
				behavPriority = (short)overlay.Read16(spawnInfoOffset + 0x04);
				renderPriority = (short)overlay.Read16(spawnInfoOffset + 0x06);
				flags = overlay.Read32(spawnInfoOffset + 0x08);
				rangeOffsetY = new Fix12i((int)overlay.Read32(spawnInfoOffset + 0x0c));
				range = new Fix12i((int)overlay.Read32(spawnInfoOffset + 0x10));
				drawDist = new Fix12i((int)overlay.Read32(spawnInfoOffset + 0x14));
				unk18 = new Fix12i((int)overlay.Read32(spawnInfoOffset + 0x18));
			}

			public string GetStructName()
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

			public string GetFlags()
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

			public void Write(string ovInfoDir)
			{
				List<string> lines = new List<string>(2 + 17 * objDatas.Length);

				foreach (ObjData objData in objDatas)
					lines.Add(objData.staticData.actorID + " " + objData.GetStructName());

				lines.Add("");

				foreach (ObjData objData in objDatas)
					lines.Add(Helper.uintToString(objData.staticData.spawnInfoAddr));

				lines.Add("");

				foreach (ObjData objData in objDatas)
				{
					lines.Add("SpawnInfo " + objData.GetStructName() + "::spawnData =");
					lines.Add("{");
					lines.Add("\t&FUN_" + Convert.ToString(objData.constructorAddr, 16).PadLeft(8, '0') + ",");
					lines.Add("\t" + Helper.shortToString(objData.behavPriority) + ",");
					lines.Add("\t" + Helper.shortToString(objData.renderPriority) + ",");
					lines.Add("\t" + objData.GetFlags() + ",");
					lines.Add("\t" + objData.rangeOffsetY + ",");
					lines.Add("\t" + objData.range + ",");
					lines.Add("\t" + objData.drawDist + ",");
					lines.Add("\t" + objData.unk18 + ",");
					lines.Add("};");
					lines.Add("");
				}

				foreach (ObjData objData in objDatas)
				{
					lines.Add(objData.GetStructName() + ":");
					lines.Add("funlst");
					lines.Add("");
				}

				lines.RemoveAt(lines.Count() - 1);

				File.WriteAllLines(ovInfoDir + "\\ov" + overlayID + ".txt", lines);
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

		private static uint[] overlaysToProcess = { 79, 80, 81, 85, 89, 90, 91, 92, 95, 96, 98, 100, 102, 2 };
		private static List<OverlayData> overlayDatas = new List<OverlayData>();

		public static void Run(string ovInfoDir)
		{
			foreach (uint overlayID in overlaysToProcess)
				overlayDatas.Add(new OverlayData(overlayID));

			foreach (OverlayData overlayData in overlayDatas)
				overlayData.Write(ovInfoDir);
		}
	}
}
