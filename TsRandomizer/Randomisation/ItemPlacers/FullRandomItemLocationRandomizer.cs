using System;
using System.Linq;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	class FullRandomItemLocationRandomizer : ItemLocationRandomizer
	{
		FullRandomItemLocationRandomizer(
			SeedOptions options,
			ItemInfoProvider itemInfoProvider, 
			ItemUnlockingMap unlockingMap, 
			ItemLocationMap itemLocationMap, 
			bool progressionOnly
			) : base(options, itemInfoProvider, itemLocationMap, unlockingMap, progressionOnly)
		{
		}

		public static void AddRandomItemsToLocationMap(
			Seed seed, ItemInfoProvider itemInfoProvider, ItemUnlockingMap unlockingMap, ItemLocationMap itemLocationMap, bool progressionOnly)
		{
			var random = new Random((int)seed.Id);

			new FullRandomItemLocationRandomizer(seed.Options, itemInfoProvider, unlockingMap, itemLocationMap, progressionOnly)
				.AddRandomItemsToLocationMap(random);
		}

		void AddRandomItemsToLocationMap(Random random)
		{
			FillTutorial(random);
			PlaceStarterProgressionItem(random);
			PlaceGassMaskInALegalSpot(random);

			var alreadyAssingedItems = ItemLocations
				.Where(l => l.IsUsed)
				.Select(l => l.ItemInfo)
				.ToArray();

			var itemsThatUnlockProgression = UnlockingMap.ItemsThatUnlockProgression
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

			FillRemainingChests(random);
		}

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			itemLocation.SetItem(itemInfo);
		}
	}
}
