using System;
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
		readonly ItemInfoProvider itemInfoProvider;
		bool firstPass;

		public GameSave GameSave { get; private set; }

		public ItemKey[] CheckedLocations { get; set; }

		public ArchipelagoItemLocationMap(ItemInfoProvider itemInfoProvider, ItemUnlockingMap itemUnlockingMap, SeedOptions options) : base(itemInfoProvider, itemUnlockingMap, options)
		{
			this.itemInfoProvider = itemInfoProvider;
		}

		public override bool IsBeatable() => true;

		public override void Initialize(GameSave gameSave)
		{
			GameSave = gameSave;

			base.Initialize(gameSave);

			foreach (var locationKey in CheckedLocations)
				this[locationKey].SetPickedUp();

			//foreach (var receiedItem in Client.GetReceivedItems())
			//	RecieveItem(receiedItem.ItemIdentifier, receiedItem.PlayerFrom, null);

			firstPass = true;
		}

		public override void Update(Level level)
		{
			foreach (var receiedItem in Client.GetReceivedItems())
				RecieveItem(receiedItem.ItemIdentifier, receiedItem.PlayerFrom, level);

			firstPass = false;
		}

		public override ProgressionChain GetProgressionChain()
		{
			throw new InvalidOperationException("Progression chains arent supported for Archipelago seeds");
		}

		void RecieveItem(ItemIdentifier itemIdentifier, int playerFrom, Level level)
		{
			var item = itemInfoProvider.Get(itemIdentifier);
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
