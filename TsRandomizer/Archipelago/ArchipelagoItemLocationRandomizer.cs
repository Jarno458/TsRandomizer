using System;
using Archipelago.MultiClient.Net;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.Randomisation;
using TsRandomizer.Randomisation.ItemPlacers;

namespace TsRandomizer.Archipelago
{
	class ArchipelagoItemLocationRandomizer : ItemLocationRandomizer
	{
		public const string GameSaveServerKey = "ArchipelagoServer";
		public const string GameSaveUserKey = "ArchipelagoUser";
		public const string GameSavePasswordKey = "ArchipelagoPassword";
		public const string GameSaveConnectionId = "ArchipelagoConnectionId";
		public const string GameSavePyramidsKeysUnlock = "ArchipelagoPyramidsKeysUnlock";
		public const string GameSavePastPyramidsKeysUnlock = "GameSavePastPyramidsKeysUnlock";
		public const string GameSavePresentPyramidsKeysUnlock = "GameSavePresentPyramidsKeysUnlock";
		public const string GameSaveTimePyramidsKeysUnlock = "GameSaveTimePyramidsKeysUnlock";
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
		}

		public override ItemLocationMap GenerateItemLocationMap(bool isProgressionOnly)
		{
			saveGame.DataKeyStrings.TryGetValue(GameSaveServerKey, out var server);
			saveGame.DataKeyStrings.TryGetValue(GameSaveUserKey, out var user);
			saveGame.DataKeyStrings.TryGetValue(GameSavePasswordKey, out var password);
			saveGame.DataKeyStrings.TryGetValue(GameSaveConnectionId, out var connectionId);
			saveGame.DataKeyStrings.TryParsePersonalItems(GameSavePersonalItemIds, out var personalLocations);

			var result = Client.Connect(server, user, password, connectionId);

			if (!result.Successful)
				throw new ConnectionFailedException((LoginFailure)result, server, user, password);

			var loginSuccessful = (LoginSuccessful)result;
			var slotdataParser = new SlotDataParser(loginSuccessful);
			var slotSeed = slotdataParser.GetSeed();

			if (slotSeed.Id != Seed.Id || slotSeed.Options.Flags != Seed.Options.Flags)
			{
				Client.Disconnect();
				throw new Exception("This save does not belong to the current multiworld hosted on the server");
			}

			itemLocations = new ArchipelagoItemLocationMap(ItemInfoProvider, UnlockingMap, Seed);

			if (isProgressionOnly)
				return itemLocations;

			foreach (var itemLocation in itemLocations)
			{
				itemLocation.SetItem(personalLocations.TryGetValue(itemLocation.Key, out var personalItemInfo)
					? ItemInfoProvider.Get(personalItemInfo)
					: ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.ArchipelagoItem)));

				itemLocation.OnPickup = _ => Client.UpdateChecks(itemLocations);
			}

			return itemLocations;
		}

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation) => 
			itemLocation.SetItem(itemInfo);
	}
}
