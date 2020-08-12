using System;
using System.Linq;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	class FullRandomItemLocationRandomizer : ItemLocationRandomizer
	{
		FullRandomItemLocationRandomizer(ItemUnlockingMap unlockingMap, ItemLocationMap itemLocationMap) : base(itemLocationMap, unlockingMap)
		{
		}

		public static void AddRandomItemsToLocationMap(Seed seed, ItemUnlockingMap unlockingMap, ItemLocationMap itemLocationMap)
		{
			var random = new Random(seed);

			new FullRandomItemLocationRandomizer(unlockingMap, itemLocationMap)
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
				.Where(i => !alreadyAssingedItems.Contains(i))
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
			var itemUnlocks = UnlockingMap.GetAllUnlock(itemInfo);

			itemLocation.SetItem(itemInfo, itemUnlocks);
		}
	}
}
