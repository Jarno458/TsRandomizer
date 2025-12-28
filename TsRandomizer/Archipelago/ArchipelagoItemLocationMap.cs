using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.ItemTracker;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;
using ItemInfo = Archipelago.MultiClient.Net.Models.ItemInfo;

namespace TsRandomizer.Archipelago
{
	class ArchipelagoItemLocationMap : ItemLocationMap
	{
		public const string GameItemIndex = "ArchipelagoGameItemIndex";
		public const string HasSavedKey = "TSRando_IsSaved";

		bool firstPass;
		DateTime nextTick = DateTime.MinValue;

		HashSet<ItemKey> personalLocationItemKeys;

		public ArchipelagoItemLocationMap(ItemInfoProvider itemInfoProvider, ItemUnlockingMap itemUnlockingMap, Seed seed)
			: base(itemInfoProvider, itemUnlockingMap, seed)
		{
		}

		public override bool IsBeatable() => true;

		public override void Initialize(GameSave gameSave)
		{
			foreach (var itemLocation in this)
				itemLocation.BaseOnGameSave(gameSave);

			gameSave.DataKeyStrings.TryParsePersonalItems(ArchipelagoItemLocationRandomizer.GameSavePersonalItemIds, out var personalLocations);
			personalLocationItemKeys = personalLocations.Keys.ToHashSet();

			Client.LocationCheckHelper.CheckedLocationsUpdated += MarkCheckedLocations;
			MarkCheckedLocations(Client.LocationCheckHelper.AllLocationsChecked);

			Client.DataStorage.TrackHints(OnHintReceived);

			firstPass = true;
		}

		public override void Update(Level level, GameplayScreen gameplayScreen)
		{
			if (firstPass)
			{
				ReceiveStarterInventory(level, gameplayScreen);

				LoadObtainedProgressionItemsFromSave(level, gameplayScreen);

				firstPass = false;
			}

			if (!level.GameSave.GetSaveBool(HasSavedKey))
				return;

			if (DateTime.UtcNow < nextTick)
				return;

			nextTick = DateTime.UtcNow.AddMilliseconds(200);
				
			var receivedItem = Client.GetNextItem(level.GameSave.GetSaveInt(GameItemIndex));
			if(receivedItem == null)
				return;
			
			ReceiveItem(receivedItem, level, gameplayScreen);

			level.GameSave.DataKeyInts[GameItemIndex] = level.GameSave.GetSaveInt(GameItemIndex) + 1; //do not use ++ here as it may be uninitialized
		}

		void ReceiveStarterInventory(Level level, GameplayScreen gameplayScreen)
		{
			var receivedItem = Client.GetNextItem(level.GameSave.GetSaveInt(GameItemIndex));

			while (receivedItem != null && receivedItem.LocationId == -2) //starting inventory
			{
				ReceiveItem(receivedItem, level, gameplayScreen);

				level.GameSave.DataKeyInts[GameItemIndex] = level.GameSave.GetSaveInt(GameItemIndex) + 1; //do not use ++ here as it may be uninitialized

				receivedItem = Client.GetNextItem(level.GameSave.GetSaveInt(GameItemIndex));
			}
		}

		void LoadObtainedProgressionItemsFromSave(Level level, GameplayScreen gameplayScreen)
		{
			var itemsInMap = this.Select(l => l.ItemInfo.Identifier).Distinct().ToHashSet();

			var updateTracker = false;
			
			foreach (var progressionItem in UnlockingMap.AllProgressionItems)
			{
				if (level.GameSave.HasItem(progressionItem) && !itemsInMap.Contains(progressionItem))
				{
					var item = new SingleItemInfo(UnlockingMap, progressionItem);

					item.OnPickup(level, gameplayScreen);

					Add(new ExternalItemLocation(item));

					updateTracker = true;
				}
			}

			if(updateTracker)
				ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(this));
		}

		public override ProgressionChain GetProgressionChain() => 
			throw new InvalidOperationException("Progression chains aren't supported for Archipelago seeds");

		void ReceiveItem(ItemInfo networkItem, Level level, GameplayScreen gameplayScreen)
		{
			if (TryGetLocation(networkItem, out var location) && networkItem.Player == Client.Slot)
			{
				//ignore message if its from my slot and a location i already picked up
				if (location.IsPickedUp && personalLocationItemKeys.Contains(location.Key))
					return;
			}
			else
			{
				location = new ExternalItemLocation();

				Add(location);
			}

			if (!TryGetItemIdentifier(networkItem, out var itemIdentifier))
				return;

			var item = ItemProvider.Get(itemIdentifier);

			location.SetItem(item);

			location.IsPickedUp = true;
			item.OnPickup(level, gameplayScreen);

			level.GameSave.AddItem(level, itemIdentifier);

			if (itemIdentifier.LootType == LootType.ConstRelic)
				level.AsDynamic().UnlockRelic(itemIdentifier.Relic);

			if (!firstPass || item.IsProgression)
			{
                if (item is CustomItem customItem)
                        gameplayScreen.ShowItemPickupBar(customItem.Name);
				else
					level.ShowItemAwardPopup(itemIdentifier);
			}

			if (item.IsProgression)
				ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(this));
		}

		void OnHintReceived(Hint[] hints)
		{
			foreach (var hint in hints)
				if (hint.FindingPlayer == Client.Slot && TryGetLocation(hint.LocationId, out var location))
					location.IsHinted = true;
		}

		bool TryGetLocation(ItemInfo networkItem, out ItemLocation location) =>
			TryGetLocation(networkItem.LocationId, out location);

		bool TryGetLocation(long locationId, out ItemLocation location)
		{
			try
			{
				location = this[LocationMap.GetItemkey(locationId)];
				return true;
			}
			catch
			{
				location = null;
				return false;
			}
		}

		bool TryGetItemIdentifier(ItemInfo networkItem, out ItemIdentifier itemIdentifier)
		{
			try
			{
				itemIdentifier = ItemMap.GetItemIdentifier(networkItem.ItemId);
				return true;
			}
			catch
			{
				itemIdentifier = null;
				return false;
			}
		}

		void MarkCheckedLocations(ReadOnlyCollection<long> locationsChecked)
		{
			foreach (var locationId in locationsChecked)
				if (TryGetValue(LocationMap.GetItemkey(locationId), out var location))
					if (location.ItemInfo is ArchipelagoItem)
						location.IsPickedUp = true;
		}
	}
}
