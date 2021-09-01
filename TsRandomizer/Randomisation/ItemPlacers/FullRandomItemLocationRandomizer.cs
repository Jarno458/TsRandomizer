using System;
using System.Linq;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	class FullRandomItemLocationRandomizer : ItemLocationRandomizer
	{
		public FullRandomItemLocationRandomizer(
			Seed seed,
			ItemInfoProvider itemInfoProvider, 
			ItemUnlockingMap unlockingMap
		) : base(seed, itemInfoProvider, new ItemLocationMap(itemInfoProvider, unlockingMap, seed.Options), unlockingMap)
		{
		}

		public override ItemLocationMap GenerateItemLocationMap(bool isProgressionOnly)
		{
			var random = new Random((int)Seed.Id);

			AddRandomItemsToLocationMap(random, isProgressionOnly);

			return ItemLocations;
		}

		void AddRandomItemsToLocationMap(Random random, bool isProgressionOnly)
		{
			PlaceStarterProgressionItems(random);

			if(!SeedOptions.GassMaw)
				PlaceGassMaskInALegalSpot(random);

			var alreadyAssingedItems = ItemLocations
				.Where(l => l.IsUsed)
				.Select(l => l.ItemInfo)
				.ToArray();

			var itemsThatUnlockProgression = UnlockingMap.AllProgressionItems
				.Where(i => alreadyAssingedItems.All(x => x.Identifier != i))
				.Select(i => ItemInfoProvider.Get(i))
				.ToList();

			var unusedItemLocations = ItemLocations
				.Where(l => !l.IsUsed)
				.ToList();

			while (itemsThatUnlockProgression.Count > 0)
			{
				var item = itemsThatUnlockProgression.PopRandom(random);
				var location = unusedItemLocations.PopRandom(random);

				PutItemAtLocation(item, location);
			}

			if(!isProgressionOnly)
				FillRemainingChests(random);
		}

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			itemLocation.SetItem(itemInfo);
		}
	}
}
