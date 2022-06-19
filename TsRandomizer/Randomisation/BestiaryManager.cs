using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Settings;

namespace TsRandomizer.Randomisation
{
	struct BossAttributes
	{
		public int Index;
		public string VisibleName;
		public string SaveName;
		public RoomItemKey BossRoom;
		public RoomItemKey ReturnRoom;
		public Point Position;
		public int HP;
		public int XP;
		public int TouchDamage;
		public int[] Minions;
		public Timespinner.GameAbstractions.EBGM Song;
		public SpriteSheet Sprite;
		public Type BossType;
		public int Argument;
		public bool IsFacingLeft;
		public bool ShouldSpawn;
		public int TileId;
	}

	struct MinionAttributes
	{
		public int Index;
		public string VisibleName;
		public int HP;
		public int TouchDamage;
	}

	enum EBossID
	{
		FelineSentry = 65,
		Varndagroth,
		AzureQueen,
		GoldenIdol,
		Aelana,
		Maw,
		Cantoran,
		Genza,
		Nuvius,
		Vol,
		Prince,
		Xarion,
		Ravenlord,
		Ifrit,
		Sandman,
		Nightmare
	}

	enum EMinionID
	{
		IncubusPuppet = 81,
		SuccubusPuppet,
		Incubus,
		Succubus,
		MawWheel,
		XarionHand,
		ShieldKnight
	}



	static class BestiaryManager
	{
		public static int[] GetValidBosses(Level level)
		{
			var validBosses = Enum.GetValues(typeof(EBossID)).Cast<int>().ToList();
			var seed = level.GameSave.GetSeed();
			if (seed.HasValue && !seed.Value.Options.Cantoran)
				validBosses.Remove((int)EBossID.Cantoran);
			if (seed.HasValue && !seed.Value.Options.GyreArchives)
			{
				validBosses.Remove((int)EBossID.Ravenlord);
				validBosses.Remove((int)EBossID.Ifrit);
			}
			return validBosses.ToArray();
		}
		public static BossAttributes GetBossAttributes(Level level, int bossId)
		{
			switch (bossId)
			{
				case (int)EBossID.FelineSentry:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Feline Sentry",
						SaveName = "IsBossDead_RoboKitty",
						BossRoom = new RoomItemKey(1, 5),
						ReturnRoom = new RoomItemKey(1, 5),
						Position = new Point(289, -52),
						HP = 475,
						XP = 50,
						TouchDamage = 17,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss01,
						Sprite = level.GCM.SpRoboKitty,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.RoboKitty.RoboKittyBoss"),
						Argument = 0,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.RoboKittyBoss
					};
				case (int)EBossID.Varndagroth:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Varndagroth",
						SaveName = "IsBossDead_Varndagroth",
						BossRoom = new RoomItemKey(2, 29),
						ReturnRoom = new RoomItemKey(2, 29),
						Position = new Point(200, 174),
						HP = 800,
						XP = 100,
						TouchDamage = 25,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss02,
						Sprite = level.GCM.SpVarndagroth,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Varndagroth.VarndagrothBoss"),
						Argument = 0,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.VarndagrothBoss
					};
				case (int)EBossID.AzureQueen:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Azure Queen",
						SaveName = "IsBossDead_Bird",
						BossRoom = new RoomItemKey(7, 0),
						ReturnRoom = new RoomItemKey(7, 0),
						Position = new Point(131, 213),
						HP = 1600,
						XP = 200,
						TouchDamage = 40,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss07,
						Sprite = level.GCM.SpBirdBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Bird.GodBirdBoss"),
						Argument = 0,
						IsFacingLeft = false,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.BirdBoss
					};
				case (int)EBossID.GoldenIdol:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Golden Idol",
						SaveName = "IsBossDead_Demon",
						BossRoom = new RoomItemKey(5, 5),
						ReturnRoom = new RoomItemKey(5, 5),
						Position = new Point(200, 176),
						HP = 2000,
						XP = 250,
						TouchDamage = 46,
						Minions = new int[] { (int)EMinionID.Incubus, (int)EMinionID.IncubusPuppet, (int)EMinionID.Succubus, (int)EMinionID.SuccubusPuppet },
						Song = Timespinner.GameAbstractions.EBGM.Boss05B,
						Sprite = level.GCM.SpDemonBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.DemonBoss"),
						Argument = 0,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.IncubusBoss
					};
				case (int)EBossID.Aelana:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Aelana",
						SaveName = "IsBossDead_Sorceress",
						BossRoom = new RoomItemKey(6, 15),
						ReturnRoom = new RoomItemKey(6, 15),
						Position = new Point(136, 208),
						HP = 2250,
						XP = 300,
						TouchDamage = 48,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss06,
						Sprite = level.GCM.SpAelana,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.AelanaBoss"),
						Argument = 0,
						IsFacingLeft = false,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.AelanaBoss
					};
				case (int)EBossID.Maw:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "The Maw",
						SaveName = "IsBossDead_Maw",
						BossRoom = new RoomItemKey(8, 7),
						ReturnRoom = new RoomItemKey(8, 13),
						Position = new Point(46, 176),
						HP = 2250,
						XP = 366,
						TouchDamage = 52,
						Minions = new int[] { (int)EMinionID.MawWheel },
						Song = Timespinner.GameAbstractions.EBGM.Boss08,
						Sprite = level.GCM.SpMawBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.MawBoss"),
						Argument = 0,
						IsFacingLeft = false,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.MawBoss
					};
				case (int)EBossID.Cantoran:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Cantoran",
						SaveName = "IsBossDead_Cantoran",
						BossRoom = new RoomItemKey(7, 5),
						ReturnRoom = new RoomItemKey(7, 5),
						Position = new Point(184, 224),
						HP = 2250,
						XP = 300,
						TouchDamage = 54,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss06,
						Sprite = level.GCM.SpCantoranBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.CantoranBoss"),
						Argument = 0,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.CantoranBoss
					};
				case (int)EBossID.Genza:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Genza",
						SaveName = "IsBossDead_Shapeshift",
						BossRoom = new RoomItemKey(11, 21),
						ReturnRoom = new RoomItemKey(11, 21),
						Position = new Point(136, 224),
						HP = 3000,
						XP = 500,
						TouchDamage = 60,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss11,
						Sprite = level.GCM.SpShapeshifter,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.ShapeshifterBoss"),
						Argument = 0,
						IsFacingLeft = false,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.ShapeshiftBoss
					};
				case (int)EBossID.Nuvius:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Emperor Nuvius",
						SaveName = "IsBossDead_Emperor",
						BossRoom = new RoomItemKey(12, 20),
						ReturnRoom = new RoomItemKey(12, 20),
						Position = new Point(344, 208),
						HP = 3500,
						XP = 666,
						TouchDamage = 80,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss12,
						Sprite = level.GCM.SpEmperor,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Emperor.EmperorBoss"),
						Argument = 0,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.EmperorBoss
					};
				case (int)EBossID.Vol:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Emperor Vol Terrilis",
						SaveName = "IsTerrilisDead",
						BossRoom = new RoomItemKey(13, 1),
						ReturnRoom = new RoomItemKey(15, 0),
						Position = new Point(200, 200),
						HP = 4000,
						XP = 777,
						TouchDamage = 85,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss12,
						Sprite = level.GCM.SpEmperorVilete,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Emperor.EmperorBoss"),
						Argument = 1,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.EmperorBoss
					};
				case (int)EBossID.Prince:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Prince Nuvius",
						SaveName = "IsPrinceDead",
						BossRoom = new RoomItemKey(13, 0),
						ReturnRoom = new RoomItemKey(15, 0),
						Position = new Point(392, 208),
						HP = 2500,
						XP = 350,
						TouchDamage = 70,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss12,
						Sprite = level.GCM.SpEmperorWinderia,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Emperor.EmperorBoss"),
						Argument = 2,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.EmperorBoss
					};
				case (int)EBossID.Xarion:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Xarion",
						SaveName = "IsBossDead_Xarion",
						BossRoom = new RoomItemKey(9, 7),
						ReturnRoom = new RoomItemKey(9, 7),
						Position = new Point(256, 217),
						HP = 3500,
						XP = 550,
						TouchDamage = 75,
						Minions = new int[] { (int)EMinionID.XarionHand },
						Song = Timespinner.GameAbstractions.EBGM.Boss02,
						Sprite = level.GCM.SpXarionBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameAbstractions.GameObjects.XarionBoss"),
						Argument = 0,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.XarionBoss
					};
				case (int)EBossID.Ravenlord:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Ravenlord",
						SaveName = "IsBossDead_Raven",
						BossRoom = new RoomItemKey(14, 4),
						ReturnRoom = new RoomItemKey(14, 4),
						Position = new Point(408, 320),
						HP = 5000,
						XP = 680,
						TouchDamage = 95,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss02,
						Sprite = level.GCM.SpRavenBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Z_Raven.RavenBoss"),
						Argument = 0,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.RavenBoss
					};
				case (int)EBossID.Ifrit:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Ifrit",
						SaveName = "IsBossDead_Zel",
						BossRoom = new RoomItemKey(14, 5),
						ReturnRoom = new RoomItemKey(14, 5),
						Position = new Point(308, 176),
						HP = 5000,
						XP = 700,
						TouchDamage = 95,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss08,
						Sprite = level.GCM.SpZelBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameAbstractions.GameObjects.ZelBoss"),
						Argument = 0,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.ZelBoss
					};
				case (int)EBossID.Sandman: 
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Sandman",
						SaveName = "IsBossDead_Sandman",
						BossRoom = new RoomItemKey(16, 4),
						ReturnRoom = new RoomItemKey(16, 26),
						Position = new Point(494, 224),
						HP = 5000,
						XP = 800,
						TouchDamage = 90,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss15,
						Sprite = level.GCM.SpSandmanBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameAbstractions.GameObjects.SandmanBoss"),
						Argument = 0,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.SandmanBoss
					};
				case (int)EBossID.Nightmare:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Nightmare",
						SaveName = "IsBossDead_Nightmare",
						BossRoom = new RoomItemKey(16, 26),
						ReturnRoom = new RoomItemKey(16, 27),
						Position = new Point(376, 176),
						HP = 6666,
						XP = 0,
						TouchDamage = 111,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss16,
						Sprite = level.GCM.SpNightmareBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.OtherBosses.NightmareBoss"),
						Argument = 0,
						IsFacingLeft = true,
						ShouldSpawn = false,
						TileId = (int)EEnemyTileType.NightmareBoss
					};
				default:
					return new BossAttributes
					{
						Index = 10,
						VisibleName = "Baby Cheveur",
						SaveName = "IsPinkBirdDead",
						BossRoom = new RoomItemKey(17, 13),
						ReturnRoom = new RoomItemKey(7, 5),
						Position = new Point(200, 200),
						HP = 50,
						XP = 9,
						TouchDamage = 25,
						Minions = new int[] { },
						Song = Timespinner.GameAbstractions.EBGM.Boss01,
						Sprite = level.GCM.SpLakeBirdEgg,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.LakeBirdEgg"),
						Argument = 1,
						IsFacingLeft = true,
						ShouldSpawn = true,
						TileId = (int)EEnemyTileType.LakeBirdEgg
					};
			}
		}

		public static MinionAttributes GetMinionAttributes(Level level, int minionId)
		{
			switch (minionId)
			{
				case (int)EMinionID.Incubus:
					return new MinionAttributes
					{
						Index = minionId,
						VisibleName = "Incubus",
						HP = 250,
						TouchDamage = 40
					};
				case (int)EMinionID.IncubusPuppet:
					return new MinionAttributes
					{
						Index = minionId,
						VisibleName = "Incubus Puppet",
						HP = 200,
						TouchDamage = 40
					};
				case (int)EMinionID.Succubus:
					return new MinionAttributes
					{
						Index = minionId,
						VisibleName = "Succubus",
						HP = 250,
						TouchDamage = 40
					};
				case (int)EMinionID.SuccubusPuppet:
					return new MinionAttributes
					{
						Index = minionId,
						VisibleName = "Succubus Puppet",
						HP = 200,
						TouchDamage = 40
					};
				case (int)EMinionID.MawWheel:
					return new MinionAttributes
					{
						Index = minionId,
						VisibleName = "Maw Wheel",
						HP = 32,
						TouchDamage = 48
					};
				case (int)EMinionID.XarionHand:
					return new MinionAttributes
					{
						Index = minionId,
						VisibleName = "Xarion's Hand",
						HP = 1000,
						TouchDamage = 75
					};
				default:
					return new MinionAttributes
					{
						Index = minionId,
						VisibleName = "Shield Knight",
						HP = 1,
						TouchDamage = 0
					};
			}
		}

		public static BossAttributes GetReplacedBoss(Level level, int vanillaBossId)
		{
			if (!level.GameSave.GetSettings().BossRando.Value)
			{
				return GetBossAttributes(level, vanillaBossId);
			}
			int[] validBosses = GetValidBosses(level);

			Random random = new Random((int)level.GameSave.GetSeed().Value.Id);
			int[] replacedBosses = validBosses.OrderBy(x => random.Next()).ToArray();

			int bossIndex = Array.IndexOf(validBosses, vanillaBossId, 0);
			if (bossIndex == -1)
				return GetBossAttributes(level,vanillaBossId);

			int replacedBossId = replacedBosses[bossIndex];

			BossAttributes replacedBossInfo = GetBossAttributes(level, replacedBossId);

			return replacedBossInfo;
		}

		public static void SetBossKillSave(Level level, int vanillaBossId)
		{
			BossAttributes vanillaBossInfo = GetBossAttributes(level, vanillaBossId);
			level.GameSave.SetValue($"TSRando_{vanillaBossInfo.SaveName}", true);
			level.GameSave.SetValue("IsFightingBoss", false);
			RefreshBossSaveFlags(level);
		}

		public static void ClearBossSaveFlags(Level level, bool valueToSet)
		{
			int[] validBosses = GetValidBosses(level);
			foreach (int bossIndex in validBosses)
			{
				BossAttributes bossInfo = GetBossAttributes(level, bossIndex);
				level.GameSave.SetValue(bossInfo.SaveName, valueToSet);
			}
			level.GameSave.SetValue("IsVileteSaved", false);
			level.GameSave.SetCutsceneTriggered("LakeSerene0_Seykis", true);
			level.GameSave.SetValue("IsCantoranActive", true);
			level.GameSave.SetValue("IsEndingABCleared", false);
			level.GameSave.SetValue("IsLabTSReady", false);
		}

		public static void RefreshBossSaveFlags(Level level)
		{
			// Iterate through all bosses and set their kill flag to reflect boss location, not actual boss
			int[] validBosses = GetValidBosses(level);
			int pastBossesKilled = 0;
			bool labTSUsed = false;
			foreach (int bossIndex in validBosses)
			{
				BossAttributes bossInfo = GetBossAttributes(level, bossIndex);
				bool isBossDead = level.GameSave.GetSaveBool($"TSRando_{bossInfo.SaveName}");
				level.GameSave.SetValue(bossInfo.SaveName, isBossDead);

				int[] pastBosses = new int[] { (int)EBossID.GoldenIdol, (int)EBossID.Aelana, (int)EBossID.Maw };
				if (isBossDead && Array.Exists(pastBosses, index => index == bossIndex))
					pastBossesKilled++;

				if (isBossDead && (bossIndex == (int)EBossID.Vol || bossIndex == (int)EBossID.Prince))
					labTSUsed = true;
			}
			level.GameSave.SetValue("IsPastCleared", pastBossesKilled == 3);
			level.GameSave.SetValue("IsVileteSaved", level.GameSave.GetSaveBool("TSRando_IsVileteSaved"));

			bool isPinkBirdDead = level.GameSave.GetSaveBool("TSRando_IsPinkBirdDead");
			bool isCantoranDead = level.GameSave.GetSaveBool("TSRando_IsBossDead_Cantoran") || !level.GameSave.GetSeed().Value.Options.Cantoran;
			level.GameSave.SetCutsceneTriggered("LakeSerene0_Seykis", isPinkBirdDead);
			level.GameSave.SetValue("IsCantoranActive",  isPinkBirdDead && !isCantoranDead);

			level.GameSave.SetValue("IsEndingABCleared", level.GameSave.GetSaveBool("TSRando_IsBossDead_Emperor"));
			level.GameSave.SetValue("IsLabTSReady", !labTSUsed && level.GameSave.GetSaveBool("TSRando_IsLabTSReady"));

			// Clear the following save flags for vanilla boss seeds
			if (!level.GameSave.GetSettings().BossRando.Value)
			{
				level.GameSave.SetValue("IsBossDead_Emperor", false);
				level.GameSave.SetValue("IsBossDead_Sandman", false);
				level.GameSave.SetValue("IsBossDead_Nightmare", false);
			}
		}

		public static BossAttributes GetVanillaBoss(Level level, int replacedBossId)
		{
			if (!level.GameSave.GetSettings().BossRando.Value)
			{
				return GetBossAttributes(level, replacedBossId);
			}
			int[] validBosses = GetValidBosses(level);

			Random random = new Random((int)level.GameSave.GetSeed().Value.Id);
			int[] replacedBosses = validBosses.OrderBy(x => random.Next()).ToArray();

			int bossIndex = Array.IndexOf(replacedBosses, replacedBossId, 0);
			if (bossIndex == -1)
				return GetBossAttributes(level, replacedBossId);

			int vanillaBossId = validBosses[bossIndex];
			BossAttributes vanillaBossInfo = GetBossAttributes(level, vanillaBossId);

			return vanillaBossInfo;
		}

		public static void UpdateBestiary(Level level, SettingCollection gameSettings)
		{
			TimeSpinnerGame.Localizer.OverrideKey("inv_use_PlaceHolderItem1", "Nothing");
			TimeSpinnerGame.Localizer.OverrideKey("inv_use_PlaceHolderItem1_desc", "You thought you picked something up, but it turned out to be nothing.");

			level.GCM.AsDynamic()._bestiary = BestiarySpecification.FromCompressedFile("./Content/Bestiary.dat");
			var bestiary = level.GCM.Bestiary;
			Random random = new Random((int)level.GameSave.GetSeed().Value.Id);
			int[] validBosses = GetValidBosses(level);
			foreach (var bestiaryEntry in bestiary.BestiaryEntries)
			{
				if (gameSettings.ShowBestiary.Value)
				{
					level.GameSave.SetValue(string.Format(bestiaryEntry.Key.Replace("Enemy_", "KILL_")), 1);
				}
				if (gameSettings.BossRando.Value && validBosses.Contains(bestiaryEntry.Index))
				{
					BossAttributes replacedBossInfo = GetBossAttributes(level, bestiaryEntry.Index);
					BossAttributes vanillaBossInfo = GetVanillaBoss(level, bestiaryEntry.Index);
					bestiaryEntry.VisibleName = $"{replacedBossInfo.VisibleName} as {vanillaBossInfo.VisibleName}";
					if (gameSettings.BossScaling.Value)
					{
						bestiaryEntry.HP = vanillaBossInfo.HP;
						bestiaryEntry.TouchDamage = vanillaBossInfo.TouchDamage;
						bestiaryEntry.Exp = vanillaBossInfo.XP;
						if (replacedBossInfo.Minions.Length > 0)
							foreach (int minionId in replacedBossInfo.Minions)
							{
								MinionAttributes minionInfo = GetMinionAttributes(level, minionId);
								var minionEntry = bestiary.BestiaryEntries[minionId];
								minionEntry.VisibleName = minionInfo.VisibleName;
								minionEntry.HP = (int)((float)vanillaBossInfo.HP/ replacedBossInfo.HP * minionInfo.HP);
								minionEntry.TouchDamage = (int)((float)vanillaBossInfo.TouchDamage / replacedBossInfo.TouchDamage * minionInfo.TouchDamage);
							}
					}
				}
				bestiaryEntry.VisibleDescription = $"ATK - {bestiaryEntry.TouchDamage}";

				int dropSlot = 0;
				foreach (var loot in bestiaryEntry.LootTable)
				{
					var item = Helper.GetAllLoot().SelectRandom(random);
					switch (gameSettings.DropRateCategory.Value)
					{
						case "Fixed":
							loot.DropRate = (int)gameSettings.DropRate.Value;
							break;
						case "Tiered":
							if (item.LootType == LootType.Equipment)
								loot.DropRate = (int)Helper.LookupEquipmentRarity((EInventoryEquipmentType)item.ItemId);
							else
								loot.DropRate = (int)Helper.LookupUseItemRarity((EInventoryUseItemType)item.ItemId);
							break;
						case "Random":
							Array lootTiers = Enum.GetValues(typeof(ELootTier));
							if (gameSettings.LootTierDistro.Value == "Full Random")
								loot.DropRate = (int)lootTiers.GetValue(random.Next(lootTiers.Length));
							else
							{
								int offset = 0;
								if (gameSettings.LootTierDistro.Value == "Inverted Weight")
									offset = 4;
								int rarityRoll = random.Next(21);
								switch (rarityRoll)
								{
									case int roll when (roll >= 20):
										loot.DropRate = (int)lootTiers.GetValue(Math.Abs(4 - offset));
										break;
									case int roll when (roll >= 16):
										loot.DropRate = (int)lootTiers.GetValue(Math.Abs(3 - offset));
										break;
									case int roll when (roll >= 13):
										loot.DropRate = (int)lootTiers.GetValue(Math.Abs(2 - offset));
										break;
									case int roll when (roll >= 8):
										loot.DropRate = (int)lootTiers.GetValue(Math.Abs(1 - offset));
										break;
									case int roll when (roll >= 0):
										loot.DropRate = (int)lootTiers.GetValue(Math.Abs(0 - offset));
										break;
									default:
										loot.DropRate = (int)ELootTier.Uncommon;
										break;
								}
							}
							break;
						case "Vanilla":
						default:
							break;
					}
					bestiaryEntry.VisibleDescription += $"\nItem {dropSlot + 1} base drop rate: {loot.DropRate}%";
					if (gameSettings.ShowDrops.Value)
					{
						level.GameSave.SetValue(string.Format(bestiaryEntry.Key.Replace("Enemy_", "DROP_" + dropSlot + "_")), true);
					}
					if (gameSettings.LootPool.Value == "Random")
					{
						loot.Item = item.ItemId;
						if (item.LootType == LootType.Equipment)
							loot.Category = (int)EInventoryCategoryType.Equipment;
						else
							loot.Category = (int)EInventoryCategoryType.UseItem;
					}
					else if (gameSettings.LootPool.Value == "Empty")
					{
						loot.DropRate = 0;
						loot.Item = (int)EInventoryUseItemType.PlaceHolderItem1;
						loot.Category = (int)EInventoryCategoryType.UseItem;
					}
					dropSlot++;
				}
			}
		}
	}
}
