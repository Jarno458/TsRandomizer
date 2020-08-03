using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation.ItemPlacers
{
	abstract class ItemLocationRandomizer
	{
		protected readonly ItemLocationMap ItemLocations;
		protected readonly ItemUnlockingMap UnlockingMap;

		static readonly ItemInfo[] ItemsToRemoveFromGame =
		{
			ItemInfo.Get(EInventoryUseItemType.MagicMarbles),
			ItemInfo.Get(EInventoryUseItemType.GoldRing),
			ItemInfo.Get(EInventoryUseItemType.GoldNecklace),
			ItemInfo.Get(EInventoryUseItemType.SilverOre),
			ItemInfo.Get(EInventoryUseItemType.EssenceCrystal),
		};

		static readonly ItemInfo[] GenericItems =
		{
			ItemInfo.Get(EInventoryUseItemType.Potion),
			ItemInfo.Get(EInventoryUseItemType.HiPotion),
			ItemInfo.Get(EInventoryUseItemType.FuturePotion),
			ItemInfo.Get(EInventoryUseItemType.FutureHiPotion),
			ItemInfo.Get(EInventoryUseItemType.Ether),
			ItemInfo.Get(EInventoryUseItemType.HiEther),
			ItemInfo.Get(EInventoryUseItemType.FutureEther),
			ItemInfo.Get(EInventoryUseItemType.FutureHiEther),
			ItemInfo.Get(EInventoryUseItemType.ChaosHeal),
			ItemInfo.Get(EInventoryUseItemType.Antidote)
		};

		protected ItemLocationRandomizer(ItemLocationMap itemLocations, ItemUnlockingMap unlockingMap)
		{
			ItemLocations = itemLocations;
			UnlockingMap = unlockingMap;
		}

		protected void FillTutorial(Random random)
		{
			var orbTypes = ((EInventoryOrbType[])Enum.GetValues(typeof(EInventoryOrbType)))
					.Where(orbType => orbType != EInventoryOrbType.None //not an actual orb to use
					                  && orbType != EInventoryOrbType.Monske) //no implemented, yields None orb
					.ToList();

			var spellOrbTypes = orbTypes.Where(orbType => orbType != EInventoryOrbType.Barrier); //To OP to give as starter spell
			var meleeOrbTypes = orbTypes.Where(orbType => orbType != EInventoryOrbType.Pink); //To annoying as each attack consumes aura power

			var spellOrbType = spellOrbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfo.Get(spellOrbType, EOrbSlot.Spell), ItemLocations[ItemKey.TutorialSpellOrb]);

			var meleeOrbType = meleeOrbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfo.Get(meleeOrbType, EOrbSlot.Melee), ItemLocations[ItemKey.TutorialMeleeOrb]);
		}

		protected void PlaceGassMaskInALegalSpot(Random random)
		{
			var minimalMawRequirements =
				Requirement.DoubleJump | Requirement.GateAccessToPast | Requirement.Swimming;

			var posableGassMaskLocations = ItemLocations
				.Where(l => l.Key.LevelId != 1 && !l.IsUsed && l.Gate.CanBeOpenedWith(minimalMawRequirements))
				.ToArray();

			PutItemAtLocation(ItemInfo.Get(EInventoryRelicType.AirMask), posableGassMaskLocations.SelectRandom(random));
		}

		protected void FillRemainingChests(Random random)
		{
			var alreadyAssingedItems = ItemLocations
				.Where(l => l.IsUsed)
				.Select(l => l.ItemInfo);
			
			var itemlist = ItemLocations
				.Select(l => l.DefaultItem)
				.Where(i => i.LootType != LootType.ConstOrb 
				            && i.LootType != LootType.ConstFamiliar 
				            && !ItemsToRemoveFromGame.Contains(i) 
				            && !GenericItems.Contains(i))
				.ToList();

			AddOrbs(itemlist);
			AddFamiliers(itemlist);

			itemlist = itemlist
				.Except(alreadyAssingedItems)
				.ToList();

			var freeLocations = ItemLocations
				.Where(l => !l.IsUsed)
				.ToList();

			do
			{
				var location = freeLocations.PopRandom(random);
				var item = itemlist.Count > 0
					? itemlist.PopRandom(random)
					: GenericItems.SelectRandom(random);

				PutItemAtLocation(item, location);

			} while (freeLocations.Count > 0);
		}

		static void AddFamiliers(List<ItemInfo> itemlist)
		{
			var allFamiliers = ((EInventoryFamiliarType[])Enum.GetValues(typeof(EInventoryFamiliarType)))
				.Where(o => o != EInventoryFamiliarType.None && o != EInventoryFamiliarType.Meyef);

			foreach (var familiar in allFamiliers)
				itemlist.Add(ItemInfo.Get(familiar));
		}

		static void AddOrbs(List<ItemInfo> itemlist)
		{
			var allOrbs = ((EInventoryOrbType[]) Enum.GetValues(typeof(EInventoryOrbType)))
				.Where(o => o != EInventoryOrbType.None && o != EInventoryOrbType.Monske);

			foreach (var orb in allOrbs)
			{
				itemlist.Add(ItemInfo.Get(orb, EOrbSlot.Melee));
				itemlist.Add(ItemInfo.Get(orb, EOrbSlot.Spell));
				itemlist.Add(ItemInfo.Get(orb, EOrbSlot.Passive));
			}
		}

		protected abstract void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation);
	}
}
