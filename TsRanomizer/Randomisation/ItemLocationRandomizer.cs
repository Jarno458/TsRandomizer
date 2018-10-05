using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocationRandomizer
	{
		ItemLocationMap itemLocations;
		ProgressionItem availableProgressionItems;
		Random random;
		
		public ItemLocationMap RandonmiseItemLocations(uint seed, ItemLocationMap itemLocationMap)
		{
			random = new Random((int)seed);
			itemLocations = itemLocationMap;
			availableProgressionItems = ProgressionItem.None;

			CalculateTutorial();
			CalculateLakeDesolationPath();
			CalculateLibraryPath();
			FillRemainingChests();

			return itemLocations;
		}

		void CalculateTutorial()
		{
			var orbsTypes = new List<EInventoryOrbType>(Enum.GetValues(typeof(EInventoryOrbType)).Cast<EInventoryOrbType>());
			orbsTypes.Remove(EInventoryOrbType.None);

			var spellOrbType = orbsTypes[random.Next(orbsTypes.Count)];
			itemLocations[ItemKey.TutorialSpellOrb].SetItem(new ItemInfo(spellOrbType, EOrbSlot.Spell));

			if (spellOrbType == EInventoryOrbType.Barrier)
				availableProgressionItems |= ProgressionItem.Lightwall;

			orbsTypes.Remove(EInventoryOrbType.Empire);

			var meleeOrbType = orbsTypes[random.Next(orbsTypes.Count)];
			itemLocations[ItemKey.TutorialMeleeOrb].SetItem(new ItemInfo(meleeOrbType, EOrbSlot.Melee));

		}

		void CalculateLakeDesolationPath()
		{
			var progressionItems = new[]
			{
				ProgressionItem.ForwardDash, ProgressionItem.ForwardDash, ProgressionItem.ForwardDash,
				ProgressionItem.DoubleJump,  ProgressionItem.DoubleJump,
				ProgressionItem.Lightwall,   ProgressionItem.Lightwall,
				ProgressionItem.TimeStop,    ProgressionItem.TimeStop,
				ProgressionItem.UpwardDash
			};

			PutRandomItemInReachableChest(progressionItems);
		}

		void CalculateLibraryPath()
		{
			PutRandomItemInReachableChest(ProgressionItem.CardD);
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
