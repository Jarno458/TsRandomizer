using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.ItemTracker;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Archipelago
{
	class ArchipelagoItemLocationMap : ItemLocationMap
	{
		public const string GameItemIndex = "ArchipelagoGameItemIndex";

		readonly int slot;

		bool firstPass;

		public ArchipelagoItemLocationMap(ItemInfoProvider itemInfoProvider, ItemUnlockingMap itemUnlockingMap, SeedOptions options, int slot)
			: base(itemInfoProvider, itemUnlockingMap, options)
		{
			this.slot = slot;
		}

		public override bool IsBeatable() => true;

		public override void Initialize(GameSave gameSave)
		{
			foreach (var itemLocation in this)
				itemLocation.BaseOnGameSave(gameSave);

			Client.LocationCheckHelper.CheckedLocationsUpdated += MarkCheckedLocations;
			MarkCheckedLocations(Client.LocationCheckHelper.AllLocationsChecked);

			firstPass = true;
		}

		public override void Update(Level level)
		{
			var receivedItem = Client.GetNextItem(level.GameSave.GetSaveInt(GameItemIndex));

			if (firstPass)
			{
				while (receivedItem.HasValue)
				{
					ReceiveItem(receivedItem.Value, level);
					level.GameSave.DataKeyInts[GameItemIndex] = level.GameSave.GetSaveInt(GameItemIndex) + 1;

					receivedItem = Client.GetNextItem(level.GameSave.GetSaveInt(GameItemIndex));
				}

				LoadObtainedProgressionItemsFromSave(level);

				firstPass = false;
			}

			if(!receivedItem.HasValue)
				return;
			
			ReceiveItem(receivedItem.Value, level);
			level.GameSave.DataKeyInts[GameItemIndex] = level.GameSave.GetSaveInt(GameItemIndex) + 1;
		}

		void LoadObtainedProgressionItemsFromSave(Level level)
		{
			var itemsInMap = this.Select(l => l.ItemInfo.Identifier)
				.Distinct().ToHashSet();

			bool updateTracker = false;
			
			foreach (var progressionItem in UnlockingMap.AllProgressionItems)
			{
				if (level.GameSave.HasItem(progressionItem) && !itemsInMap.Contains(progressionItem))
				{
					var item = new SingleItemInfo(UnlockingMap, progressionItem);

					item.OnPickup(level);

					Add(new ExteralItemLocation(item));

					updateTracker = true;
				}
			}

			if(updateTracker)
				ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(this));
		}

		public override ProgressionChain GetProgressionChain() => 
			throw new InvalidOperationException("Progression chains arent supported for Archipelago seeds");

		void ReceiveItem(NetworkItem networkItem, Level level)
		{
			var isLocal = networkItem.Player == slot;
			ItemLocation location;

			if (isLocal)
			{
				//ignore message if its from my slot and a location i already picked up
				if (!TryGetLocation(networkItem, out location) || location.IsPickedUp)
					return;
			}
			else
			{
				location = new ExteralItemLocation();

				Add(location);
			}

			if (!TryGetItemIdentifier(networkItem, out var itemIdentifier))
				return;

			// itemInfoProvider's cache is out of date here when it comes to pyramid unlocks
			var item = new SingleItemInfo(UnlockingMap, itemIdentifier);

			location.SetItem(item);

			location.IsPickedUp = true;
			item.OnPickup(level);

			level.GameSave.AddItem(level, itemIdentifier);

			if (itemIdentifier.LootType == LootType.ConstRelic)
				level.AsDynamic().UnlockRelic(itemIdentifier.Relic);

			if (!firstPass || item.IsProgression)
				level.ShowItemAwardPopup(itemIdentifier);

			if (item.IsProgression)
				ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(this));
		}

		bool TryGetLocation(NetworkItem networkItem, out ItemLocation location)
		{
			try
			{
				location = this[LocationMap.GetItemkey(networkItem.Location)];
				return true;
			}
			catch
			{
				location = null;
				return false;
			}
		}

		bool TryGetItemIdentifier(NetworkItem networkItem, out ItemIdentifier itemIdentifier)
		{
			try
			{
				itemIdentifier = ItemMap.GetItemIdentifier(networkItem.Item);
				return true;
			}
			catch
			{
				itemIdentifier = null;
				return false;
			}
		}

		void MarkCheckedLocations(ICollection<int> locationsChecked)
		{
			foreach (var locationId in locationsChecked)
				if (TryGetValue(LocationMap.GetItemkey(locationId), out var location))
					if (location.ItemInfo is ArchipelagoRemoteItem)
						location.IsPickedUp = true;
		}
	}
}
