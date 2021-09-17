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
			client = new Client((ArchipelagoItemLocationMap)ItemLocations);

			var result = client.Connect(true);
			if (!result.Success)
				return null; //TODO show error to user

			var items = client.GetAllItems();

			foreach (var itemInfo in items)
				ItemLocations[itemInfo.Key].SetItem(ItemInfoProvider.Get(itemInfo.Value));

			foreach (var itemLocation in ItemLocations)
				itemLocation.OnPickup = OnItemLocationChecked;

			return ItemLocations;
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
