using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Settings;

namespace TsRandomizer.Randomisation
{
	static class BestiaryManager
	{
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

				int dropSlot = 0;
				foreach (var loot in bestiaryEntry.LootTable)
				{
					if (gameSettings.ShowDrops.Value)
					{
						level.GameSave.SetValue(string.Format(bestiaryEntry.Key.Replace("Enemy_", "DROP_" + dropSlot + "_")), true);
					}
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
					dropSlot++;
				}
			}
		}
	}
}
