using System.Collections.Generic;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Archipelago;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	class ArchipelagoItemLocationRandomizer : ItemLocationRandomizer
	{
		Client client;

		public ArchipelagoItemLocationRandomizer(
			Seed seed,
			ItemInfoProvider itemInfoProvider, 
			ItemUnlockingMap unlockingMap
		) : base(seed, itemInfoProvider, new ArchipelagoItemLocationMap(itemInfoProvider, unlockingMap, seed.Options), unlockingMap)
		{
		}

		public override ItemLocationMap GenerateItemLocationMap(bool isProgressionOnly)
		{
			var itemLocations = (ArchipelagoItemLocationMap)ItemLocations;

			client = new Client(itemLocations);

			var result = client.Connect(false);
			if (!result.Success)
				return null; //TODO show error to user

			if (isProgressionOnly)
				return itemLocations;

			itemLocations.Client = client;
			itemLocations.CheckedLocations = result.CheckedLocations;

			var items = client.GetAllNonCheckedItems();
			if (items == null)
				return null; //TODO show error to user

			foreach (var itemLocation in itemLocations)
			{
				if (items.TryGetValue(itemLocation.Key, out var itemInfo))
					itemLocation.SetItem(ItemInfoProvider.Get(itemInfo));

				itemLocation.OnPickup = OnItemLocationChecked;
			}

			return itemLocations;
		}

		void OnItemLocationChecked(ItemLocation itemLocation)
		{
			client.UpdateChecks();
		}

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			itemLocation.SetItem(itemInfo);
		}
	}
}
