using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	class ArchipelagoItemLocationRandomizer : ItemLocationRandomizer
	{
		public const string GameSaveServerKey = "ArchipelagoServer";
		public const string GameSaveUserKey = "ArchipelagoUser";
		public const string GameSavePasswordKey = "ArchipelagoPassword";
		public const string GameSaveConnectionId = "ArchipelagoConnectionId";
		public const string GameSavePyramidsKeysUnlock = "ArchipelagoPyramidsKeysUnlock";
		public const string GameSavePersonalItemIds = "ArchipelagoPersonalItemIds";

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
			saveGame.DataKeyStrings.TryGetValue(GameSaveConnectionId, out var connectionId);
			saveGame.DataKeyStrings.TryParsePersonalItems(GameSavePersonalItemIds, out var personalLocations);
			saveGame.DataKeyStrings.TryParsePyramidKeysUnlock(GameSavePyramidsKeysUnlock, out var pyramidKeysUnlock);

			var result = Client.Connect(server, user, password, connectionId);

			if (!result.Successful)
				throw new ConnectionFailedException((LoginFailure)result, server, user, password);

			itemLocations = new ArchipelagoItemLocationMap(ItemInfoProvider, UnlockingMap, Seed.Options);

			if (isProgressionOnly)
				return itemLocations;

			UnlockingMap.SetTeleporterPickupAction(pyramidKeysUnlock);

			foreach (var itemLocation in itemLocations)
			{
				if (personalLocations.TryGetValue(itemLocation.Key, out var personalItemInfo))
					itemLocation.SetItem(new SingleItemInfo(UnlockingMap, personalItemInfo)); //avoiding item provider as we cant handle progressive items atm
				else
					itemLocation.SetItem(new ArchipelagoRemoteItem());

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
			saveGame.Inventory.UseItemInventory.RemoveItem((int)EInventoryUseItemType.MagicMarbles, 999);
		}

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			itemLocation.SetItem(itemInfo);
		}
	}
}
