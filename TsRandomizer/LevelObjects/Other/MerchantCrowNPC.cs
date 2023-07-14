using System;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.MerchantCrowNPC")]
	class MerchantCrowNpc : LevelObject
	{
		readonly MerchantInventory merchandiseInventory = new MerchantInventory();

		public MerchantCrowNpc(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			var gameSettings = Level.GameSave.GetSettings();
			var fillType = gameSettings.ShopFill.Value;
			if (fillType == "Vanilla")
				return;

			PlayerInventory inventory = Dynamic._level.GameSave.Inventory;

			// Only sell warp shards if Twin Pyramid Key is acquired (and allowed in settings)
			if ((gameSettings.ShopWarpShards.Value || fillType == "Default") && inventory.RelicInventory.IsRelicActive(EInventoryRelicType.PyramidsKey))
				merchandiseInventory.AddItem(EInventoryUseItemType.WarpCard);

			if (fillType == "Empty")
			{
				Dynamic._merchandiseInventory = merchandiseInventory;
				return;
			}
			if (fillType == "Random")
			{
				var random = new Random((int)seed.Id + 1);
				for (var i = 0; i < 8; i++)
				{
					var item = Helper.GetAllLoot().SelectRandom(random);
					// Give half of the items to each era. Needs to be done after the random advances
					// to keep the inventory consistent for the seed
					if ((Dynamic._isInPresent && (i % 2 == 0) || !Dynamic._isInPresent && (i % 2 != 0)))
						continue;
					if (item.LootType == LootType.Equipment)
						merchandiseInventory.AddItem((EInventoryEquipmentType)item.ItemId);
					else
						merchandiseInventory.AddItem((EInventoryUseItemType)item.ItemId);
				}
				Dynamic._merchandiseInventory = merchandiseInventory;
				return;
			}

			// Default case, streamlined inventory for randomizer players
			if (Dynamic._isInPresent)
			{
				merchandiseInventory.AddItem(EInventoryUseItemType.FuturePotion);
				merchandiseInventory.AddItem(EInventoryUseItemType.FutureEther);
			}
			else
			{
				merchandiseInventory.AddItem(EInventoryUseItemType.Potion);
				merchandiseInventory.AddItem(EInventoryUseItemType.Ether);
			}
			merchandiseInventory.AddItem(EInventoryUseItemType.Biscuit);
			merchandiseInventory.AddItem(EInventoryUseItemType.Antidote);
			merchandiseInventory.AddItem(EInventoryUseItemType.SandBottle);
			merchandiseInventory.AddItem(EInventoryUseItemType.ChaosHeal);

			Dynamic._merchandiseInventory = merchandiseInventory;
		}
	}
}
