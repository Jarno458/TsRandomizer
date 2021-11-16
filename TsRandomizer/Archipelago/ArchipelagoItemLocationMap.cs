using System;
using System.Linq;
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

		bool firstPass;

		public ArchipelagoItemLocationMap(ItemInfoProvider itemInfoProvider, ItemUnlockingMap itemUnlockingMap, SeedOptions options)
			: base(itemInfoProvider, itemUnlockingMap, options)
		{
		}

		public override bool IsBeatable() => true;

		public override void Initialize(GameSave gameSave)
		{
			foreach (var itemLocation in this)
				itemLocation.BsseOnGameSave(gameSave);

			firstPass = true;
		}

		public override void Update(Level level)
		{
			ItemIdentifier receivedItem = Client.GetNextItem(level.GameSave.GetSaveInt(GameItemIndex));

			if (firstPass)
			{
				while (receivedItem != null)
				{
					RecieveItem(receivedItem, level);
					level.GameSave.DataKeyInts[GameItemIndex] = level.GameSave.GetSaveInt(GameItemIndex) + 1;

					receivedItem = Client.GetNextItem(level.GameSave.GetSaveInt(GameItemIndex));
				}

				LoadObtainedProgressionItemsFromSave(level);

				firstPass = false;
			}

			if(receivedItem == null)
				return;
			
			RecieveItem(receivedItem, level);
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

		public override ProgressionChain GetProgressionChain()
		{
			throw new InvalidOperationException("Progression chains arent supported for Archipelago seeds");
		}

		void RecieveItem(ItemIdentifier itemIdentifier, Level level)
		{
			// itemInfoProvider's cache is out of date here when it comes to pyramid unlocks
			var item = new SingleItemInfo(UnlockingMap, itemIdentifier);

			item.OnPickup(level);

			level.GameSave.AddItem(level, itemIdentifier);

			if (itemIdentifier.LootType == LootType.ConstRelic)
				level.AsDynamic().UnlockRelic(itemIdentifier.Relic);

			if(!firstPass || item.IsProgression)
				level.ShowItemAwardPopup(itemIdentifier);

			if (item.IsProgression)
			{
				Add(new ExteralItemLocation(item));
				ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(this));
			}
		}
	}
}
