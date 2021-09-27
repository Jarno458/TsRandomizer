using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Archipelago;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	class ArchipelagoItemLocationRandomizer : ItemLocationRandomizer
	{
		public const string GameSaveServerKey = "ArchipelagoServer";
		public const string GameSaveUserKey = "ArchipelagoUser";
		public const string GameSavePasswordKey = "ArchipelagoPassword";

		readonly GameSave saveGame;

		ArchipelagoItemLocationMap itemLocations;

		public ArchipelagoItemLocationRandomizer(
			Seed seed,
			ItemInfoProvider itemInfoProvider, 
			ItemUnlockingMap unlockingMap,
			GameSave saveGame
		) : base(seed, itemInfoProvider, unlockingMap)
		{
			this.saveGame = saveGame;

			TimeSpinnerGame.Localizar.OverrideKey("inv_use_MagicMarbles", "Archipelago Item");
			TimeSpinnerGame.Localizar.OverrideKey("inv_use_MagicMarbles_desc", "Item that belongs to a distant timeline somewhere in the Archipelago");
		}

		public override ItemLocationMap GenerateItemLocationMap(bool isProgressionOnly)
		{
			saveGame.DataKeyStrings.TryGetValue(GameSaveServerKey, out var server);
			saveGame.DataKeyStrings.TryGetValue(GameSaveUserKey, out var user);
			saveGame.DataKeyStrings.TryGetValue(GameSavePasswordKey, out var password);

			var result = Client.Connect(server, user, password);
			if (!result.Success)
				return null; //handle correctly

			itemLocations = new ArchipelagoItemLocationMap(ItemInfoProvider, UnlockingMap, Seed.Options);

			Client.ItemLocations = itemLocations;

			if (isProgressionOnly)
				return itemLocations;

			var connected = (Connected)Client.CachedConnectionResult;

			UnlockingMap.SetTeleporterPickupAction((string)connected.SlotData["PyramidKeysGate"]);

			itemLocations.CheckedLocations =  connected.CheckedLocations;

			var uncheckedLocations = new HashSet<ItemKey>(connected.UncheckedLocations);

			foreach (var itemLocation in itemLocations)
			{
				if (connected.PersonalLocations.TryGetValue(itemLocation.Key, out var personalItemInfo))
					itemLocation.SetItem(new SingleItemInfo(UnlockingMap, personalItemInfo)); //avoiding item provider as we cant handle progressive items atm)
				else
					itemLocation.SetItem(new ArchipelagoRemoteItem());

				if (uncheckedLocations.Contains(itemLocation.Key))
					itemLocation.OnPickup = OnItemLocationChecked;
			}

			return itemLocations;
		}

		void OnItemLocationChecked(ItemLocation itemLocation)
		{
			Client.UpdateChecks(itemLocations);

			RemoveRemoteItemsFromInventory();
		}

		void RemoveRemoteItemsFromInventory()
		{
			itemLocations.GameSave.Inventory.UseItemInventory.RemoveItem((int)EInventoryUseItemType.MagicMarbles, 9);
		}

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			itemLocation.SetItem(itemInfo);
		}
	}
}
