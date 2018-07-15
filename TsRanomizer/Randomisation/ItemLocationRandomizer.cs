using System;
using System.Linq;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocationRandomizer
	{
		ItemLocationMap itemLocations;
		ProgressionItem availableProgressionItems;
		Random random;

		public ItemLocationMap RandonmiseItemLocations(int seed, ItemLocationMap itemLocationMap)
		{
			random = new Random(seed);
			itemLocations = itemLocationMap;
			availableProgressionItems = ProgressionItem.None;

			CalculateLakeDesolationPath();
			CalculateLibraryPath();
			FillRemainingChests();

			return itemLocations;
		}

		void CalculateLakeDesolationPath()
		{
			var progressionItems = new []
			{
				ProgressionItem.ForwardDash, ProgressionItem.ForwardDash, ProgressionItem.ForwardDash,
				ProgressionItem.DoubleJump,  ProgressionItem.DoubleJump,
				ProgressionItem.Lightwall,   ProgressionItem.Lightwall,
				ProgressionItem.TimeStop,    ProgressionItem.TimeStop,
				ProgressionItem.UpwardDash
			};

			PutRandomItemInReachableChest(progressionItems);

			//if(itemLocation.ItemInfo.IsProgressive) //TODO: ability to decide item at runtime rather then roomload

			availableProgressionItems |= ProgressionItem.KittyBoss;
		}

		void CalculateLibraryPath()
		{
			PutRandomItemInReachableChest(ProgressionItem.CardD);
			var progressionItem = 
				PutRandomItemInReachableChest(ProgressionItem.CardB, ProgressionItem.CardC);

			//If boss is required we need the elevator key, otherwise we need it with CardB
			PutRandomItemInReachableChest(ProgressionItem.CardE);
		}

		void FillRemainingChests()
		{
			foreach (var itemLocation in itemLocations)
				if (!itemLocation.IsUsed)
					itemLocation.SetItem(ItemInfo.Dummy);
		}

		ProgressionItem PutRandomItemInReachableChest(params ProgressionItem[] items)
		{
			var progressionItem = SelectRandomProgressionItem(items);
			PutRandomItemInReachableChest(progressionItem);
			return progressionItem;
		}

		void PutRandomItemInReachableChest(ProgressionItem progressionItem)
		{
			var item = ItemInfo.FromProgressionItem(progressionItem);
			var reachableItemLocations = GetUnusedReachableItemLocations();
			var itemLocation = reachableItemLocations[random.Next(reachableItemLocations.Length)];

			itemLocation.SetItem(item);

			availableProgressionItems |= progressionItem;
		}
		
		ProgressionItem SelectRandomProgressionItem(params ProgressionItem[] items)
		{
			return items[random.Next(items.Length)];
		}

		ItemLocation[] GetUnusedReachableItemLocations()
		{
			return itemLocations
				.Where(c => !c.IsUsed && c.Gate.CanOpen(availableProgressionItems))
				.ToArray();
		}
	}
}
