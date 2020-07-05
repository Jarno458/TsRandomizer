using System;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation.ItemPlacers
{
	class FullRandomItemLocationRandomizer : ItemLocationRandomizer
	{
		readonly ItemUnlockingMap unlockingMap;

		FullRandomItemLocationRandomizer(ItemUnlockingMap unlockingMap, ItemLocationMap itemLocationMap) : base(itemLocationMap)
		{
			this.unlockingMap = unlockingMap;
		}

		public static void AddRandomItemsToLocationMap(Seed seed, ItemUnlockingMap unlockingMap, ItemLocationMap itemLocationMap)
		{
			var random = new Random(seed);

			new FullRandomItemLocationRandomizer(unlockingMap, itemLocationMap)
				.AddRandomItemsToLocationMap(random);
		}

		void AddRandomItemsToLocationMap(Random random)
		{
			CalculateTutorial(random);
			PlaceGassMaskInALegalSpot(random);

			var itemsThatUnlockProgression = unlockingMap.Map.Keys
				.Where(i => i != ItemInfo.Get(EInventoryRelicType.AirMask))
				.ToList();

			while (itemsThatUnlockProgression.Count > 0)
			{
				var item = itemsThatUnlockProgression.SelectRandom(random);

				var location = GetUnusedItemLocation(random);

				PutItemAtLocation(item, location);

				itemsThatUnlockProgression.Remove(item);
			}

			FillRemainingChests();
		}

		ItemLocation GetUnusedItemLocation(Random random)
		{
			var locations = ItemLocations
				.Where(l => !l.IsUsed);

			return locations.SelectRandom(random);
		}

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			var itemUnlocks = unlockingMap.GetAllUnlock(itemInfo);

			itemLocation.SetItem(itemInfo, itemUnlocks);
		}
	}
}
