using TsRandomizer.IntermediateObjects;
using Archipelago.MultiClient.Net;
using TsRandomizer.Archipelago;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	class ArchipelagoItemLocationRandomizer : ItemLocationRandomizer
	{
		public ArchipelagoItemLocationRandomizer(
			Seed seed,
			ItemInfoProvider itemInfoProvider, 
			ItemUnlockingMap unlockingMap
		) : base(seed, itemInfoProvider, new ArchipelagoItemLocationMap(itemInfoProvider, unlockingMap, seed.Options), unlockingMap)
		{
		}

		public override ItemLocationMap GenerateItemLocationMap(bool isProgressionOnly)
		{
			var client = new Client();

			return ItemLocations;
		}

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			itemLocation.SetItem(itemInfo);
		}
	}
}
