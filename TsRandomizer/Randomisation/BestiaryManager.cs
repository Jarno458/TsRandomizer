using System;
using Timespinner.Core;
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
		public RoomItemKey RoomKey;
		public int HP;
		public int XP;
		public int TouchDamage;
		public Timespinner.GameAbstractions.EBGM Song;
		public SpriteSheet Sprite;
		public Type BossType;
	}

	static class BestiaryManager
	{
		public static BossAttributes GetBossAttributes(Level level, int bossId)
		{
			switch (bossId)
			{
				case 65:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Feline Sentry",
						RoomKey = new RoomItemKey(1, 5),
						HP = 475,
						XP = 50,
						TouchDamage = 17,
						Song = Timespinner.GameAbstractions.EBGM.Boss01,
						Sprite = level.GCM.SpRoboKitty,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.RoboKitty.RoboKittyBoss")
					};
				case 73:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Emperor Nuvius",
						RoomKey = new RoomItemKey(12, 20),
						HP = 3500,
						XP = 666,
						TouchDamage = 80,
						Song = Timespinner.GameAbstractions.EBGM.Boss01,
						Sprite = level.GCM.SpEmperor,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Emperor.EmperorBoss")
					};
				case 74:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Emperor Vol Terrilis",
						RoomKey = new RoomItemKey(13, 1),
						HP = 4000,
						XP = 777,
						TouchDamage = 85,
						Song = Timespinner.GameAbstractions.EBGM.Boss05A,
						Sprite = level.GCM.SpEmperor,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Emperor.EmperorBoss")
					};
				case 75:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Prince Nuvius",
						RoomKey = new RoomItemKey(13, 0),
						HP = 2500,
						XP = 350,
						TouchDamage = 70,
						Song = Timespinner.GameAbstractions.EBGM.Boss05B,
						Sprite = level.GCM.SpEmperor
						,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Emperor.EmperorBoss")
					};
				case 76:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Xarion",
						RoomKey = new RoomItemKey(9, 7),
						HP = 3500,
						XP = 550,
						TouchDamage = 75,
						Song = Timespinner.GameAbstractions.EBGM.Boss11,
						Sprite = level.GCM.SpEmperor,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Z_Xarion")
					};
				case 77:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Ravenlord",
						RoomKey = new RoomItemKey(14, 4),
						HP = 5000,
						XP = 680,
						TouchDamage = 95,
						Song = Timespinner.GameAbstractions.EBGM.Boss13,
						Sprite = level.GCM.SpEmperor,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Z_Raven")
					};
				case 78:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Ifrit",
						RoomKey = new RoomItemKey(14, 5),
						HP = 5000,
						XP = 700,
						TouchDamage = 95,
						Song = Timespinner.GameAbstractions.EBGM.Boss12,
						Sprite = level.GCM.SpZelBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Z_Zel")
					};
				case 79: 
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Sandman",
						RoomKey = new RoomItemKey(7, 5),
						HP = 5000,
						XP = 800,
						TouchDamage = 90,
						Song = Timespinner.GameAbstractions.EBGM.Boss15,
						Sprite = level.GCM.SpSandmanBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Emperor.EmperorBoss")
					};
				case 80:
					return new BossAttributes
					{
						Index = bossId,
						VisibleName = "Nightmare",
						RoomKey = new RoomItemKey(12, 26),
						HP = 6666,
						XP = 0,
						TouchDamage = 111,
						Song = Timespinner.GameAbstractions.EBGM.Boss16,
						Sprite = level.GCM.SpNightmareBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.OtherBosses.NightmareBoss")
					};
				default: 
					return new BossAttributes
					{ 
						Index = 10,
						VisibleName = "Baby Cheveur",
						RoomKey = new RoomItemKey(7, 5),
						HP = 50,
						XP = 9,
						TouchDamage = 25,
						Song = Timespinner.GameAbstractions.EBGM.Boss01,
						Sprite = level.GCM.SpBirdBoss,
						BossType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Emperor.EmperorBoss")
					};
			}
		}

		public static void UpdateBestiary(Level level, SeedOptions seedOptions, SettingCollection gameSettings)
		{
			TimeSpinnerGame.Localizer.OverrideKey("inv_use_PlaceHolderItem1", "Nothing");
			TimeSpinnerGame.Localizer.OverrideKey("inv_use_PlaceHolderItem1_desc", "You thought you picked something up, but it turned out to be nothing.");

			var bestiary = level.GCM.Bestiary;
			Random random = new Random(seedOptions.GetHashCode());
			foreach (var bestiaryEntry in bestiary.BestiaryEntries)
			{
				if (gameSettings.ShowBestiary.Value)
				{
					level.GameSave.SetValue(string.Format(bestiaryEntry.Key.Replace("Enemy_", "KILL_")), 1);
				}
				bestiaryEntry.VisibleDescription = bestiaryEntry.Key + " ~ " + bestiaryEntry.TouchDamage + "~" + bestiaryEntry.EnemyType;

				foreach (var loot in bestiaryEntry.LootTable)
				{
					if (gameSettings.LootPool.Value == "Random")
					{
						var item = Helper.GetAllLoot().SelectRandom(random);
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
				}
			}
		}
	}
}
