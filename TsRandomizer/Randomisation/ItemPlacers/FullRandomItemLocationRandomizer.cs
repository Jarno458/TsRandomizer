using System;
using System.Linq;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	class FullRandomItemLocationRandomizer : ItemLocationRandomizer
	{
		ItemLocationMap itemLocations;

		public FullRandomItemLocationRandomizer(
			Seed seed,
			ItemInfoProvider itemInfoProvider, 
			ItemUnlockingMap unlockingMap
		) : base(seed, itemInfoProvider, unlockingMap)
		{
		}

		public override ItemLocationMap GenerateItemLocationMap(bool isProgressionOnly)
		{
			var random = new Random((int)Seed.Id);

			itemLocations = new ItemLocationMap(ItemInfoProvider, UnlockingMap, Seed.Options);

			AddRandomItemsToLocationMap(random, isProgressionOnly);

			return itemLocations;
		}

		void AddRandomItemsToLocationMap(Random random, bool isProgressionOnly)
		{
			PlaceStarterProgressionItems(random, itemLocations);

			if(!SeedOptions.GasMaw)
				PlaceGasMaskInALegalSpot(random, itemLocations);

			var alreadyAssingedItems = itemLocations
				.Where(l => l.IsUsed)
				.Select(l => l.ItemInfo)
				.ToArray();

			var itemsThatUnlockProgression = UnlockingMap.AllProgressionItems
				.Where(i => alreadyAssingedItems.All(x => x.Identifier != i))
				.Select(i => ItemInfoProvider.Get(i))
				.ToList();

			var unusedItemLocations = itemLocations
				.Where(l => !l.IsUsed)
				.ToList();

			while (itemsThatUnlockProgression.Count > 0)
			{
				var item = itemsThatUnlockProgression.PopRandom(random);
				var location = unusedItemLocations.PopRandom(random);

				PutItemAtLocation(item, location);
			}

			if(!isProgressionOnly)
				FillRemainingChests(random, itemLocations);
		}

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation) => itemLocation.SetItem(itemInfo);
	}
}
