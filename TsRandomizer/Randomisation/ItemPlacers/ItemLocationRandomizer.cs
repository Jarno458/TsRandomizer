using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using R = TsRandomizer.Randomisation.Requirement;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	abstract class ItemLocationRandomizer
	{
		protected readonly Seed Seed;
		protected readonly SeedOptions SeedOptions;
		protected readonly ItemInfoProvider ItemInfoProvider;
		protected readonly ItemUnlockingMap UnlockingMap;

		readonly List<ItemInfo> itemsToRemoveFromGame;
		readonly ItemInfo[] itemsToAddToGame;
		readonly ItemInfo[] genericItems;

		protected ItemLocationRandomizer(Seed seed, ItemInfoProvider itemInfoProvider, ItemUnlockingMap unlockingMap)
		{
			Seed = seed;
			SeedOptions = seed.Options;
			ItemInfoProvider = itemInfoProvider;
			UnlockingMap = unlockingMap;

			itemsToRemoveFromGame = new List<ItemInfo>
			{
				ItemInfoProvider.Get(EInventoryUseItemType.MagicMarbles),
				ItemInfoProvider.Get(EInventoryUseItemType.GoldRing),
				ItemInfoProvider.Get(EInventoryUseItemType.GoldNecklace),
				ItemInfoProvider.Get(EInventoryUseItemType.SilverOre),
				ItemInfoProvider.Get(EInventoryUseItemType.EssenceCrystal),
			};

			if (SeedOptions.StartWithJewelryBox)
				itemsToRemoveFromGame.Add(ItemInfoProvider.Get(EInventoryRelicType.JewelryBox));
			if (SeedOptions.StartWithMeyef)
				itemsToRemoveFromGame.Add(ItemInfoProvider.Get(EInventoryFamiliarType.Meyef));
			if (SeedOptions.StartWithTalaria)
				itemsToRemoveFromGame.Add(ItemInfoProvider.Get(EInventoryRelicType.Dash));

			itemsToAddToGame = new[]
			{
				ItemInfoProvider.Get(EInventoryEquipmentType.GlassPumpkin),
				ItemInfoProvider.Get(EInventoryEquipmentType.EternalCoat),
				ItemInfoProvider.Get(EInventoryEquipmentType.EternalTiara),
				ItemInfoProvider.Get(EInventoryEquipmentType.LibrarianHat),
				ItemInfoProvider.Get(EInventoryEquipmentType.LibrarianRobe),
				ItemInfoProvider.Get(EInventoryEquipmentType.MetalWristband),
				ItemInfoProvider.Get(EInventoryEquipmentType.NelisteEarring),
				ItemInfoProvider.Get(EInventoryEquipmentType.Sunglasses),
				ItemInfoProvider.Get(EInventoryEquipmentType.TrendyJacket),
				ItemInfoProvider.Get(EInventoryEquipmentType.FamiliarEgg),
				ItemInfoProvider.Get(EInventoryEquipmentType.LuckyCoin),
				ItemInfoProvider.Get(EInventoryEquipmentType.ShinyRock),
				ItemInfoProvider.Get(EInventoryRelicType.EternalBrooch),
				ItemInfoProvider.Get(EInventoryRelicType.FamiliarAltMeyef),
				ItemInfoProvider.Get(EInventoryRelicType.FamiliarAltCrow)
			};

			genericItems = new[]
			{
				ItemInfoProvider.Get(EInventoryUseItemType.Potion),
				ItemInfoProvider.Get(EInventoryUseItemType.HiPotion),
				ItemInfoProvider.Get(EInventoryUseItemType.FuturePotion),
				ItemInfoProvider.Get(EInventoryUseItemType.FutureHiPotion),
				ItemInfoProvider.Get(EInventoryUseItemType.Ether),
				ItemInfoProvider.Get(EInventoryUseItemType.HiEther),
				ItemInfoProvider.Get(EInventoryUseItemType.FutureEther),
				ItemInfoProvider.Get(EInventoryUseItemType.FutureHiEther),
				ItemInfoProvider.Get(EInventoryUseItemType.ChaosHeal),
				ItemInfoProvider.Get(EInventoryUseItemType.Antidote),
				ItemInfoProvider.Get(EInventoryUseItemType.SandBottle),
				ItemInfoProvider.Get(EInventoryUseItemType.HiSandBottle)
			};
		}

		public abstract ItemLocationMap GenerateItemLocationMap(bool isProgressionOnly);

		protected void PlaceStarterProgressionItems(Random random, ItemLocationMap itemLocations)
		{
			if (SeedOptions.StartWithTalaria || SeedOptions.Inverted)
				GiveOrbsToMom(random, itemLocations, false);
			else
				PlaceStarterProgressionItem(random, itemLocations);
		}

		void PlaceStarterProgressionItem(Random random, ItemLocationMap itemLocations)
		{
			var starterProgressionItems = new List<ItemInfo> {
				ItemInfoProvider.Get(EInventoryRelicType.Dash),
				ItemInfoProvider.Get(EInventoryRelicType.Dash),
				ItemInfoProvider.Get(EInventoryRelicType.DoubleJump),
				ItemInfoProvider.Get(EInventoryRelicType.DoubleJump),
				ItemInfoProvider.Get(EInventoryRelicType.TimespinnerWheel),
				ItemInfoProvider.Get(EInventoryRelicType.TimespinnerWheel),
				ItemInfoProvider.Get(EInventoryRelicType.PyramidsKey),
			};

			if (!SeedOptions.ProgressiveVerticalMovement)
			{
				starterProgressionItems.Add(ItemInfoProvider.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell));
				starterProgressionItems.Add(ItemInfoProvider.Get(EInventoryRelicType.EssenceOfSpace));
			}

			var starterProgressionItem = starterProgressionItems.SelectRandom(random);

			var shouldGiveLightwallAsSpell = ShouldGiveLightwall(random, starterProgressionItem);

			if (shouldGiveLightwallAsSpell)
			{
				GiveOrbsToMom(random, itemLocations, true);
			}
			else
			{
				GiveOrbsToMom(random, itemLocations, false);

				if (SeedOptions.StartWithTalaria) return;

				PutStarterProgressionItemInReachableLocation(random, itemLocations, starterProgressionItem);
			}
		}

		void PutStarterProgressionItemInReachableLocation(Random random, ItemLocationMap itemLocations, ItemInfo starterProgressionItem)
		{
			var starterLocations = itemLocations
				.Where(l => l.Key.LevelId != 0 && l.Gate.Requires(R.None))
				.ToArray();

			var location = starterLocations.SelectRandom(random);

			PutItemAtLocation(starterProgressionItem, location);
		}

		static bool ShouldGiveLightwall(Random random, ItemInfo starterProgressionItem) =>
			(starterProgressionItem.Identifier == new ItemIdentifier(EInventoryOrbType.Barrier, EOrbSlot.Spell))
				&& random.Next(1, 5) == 1;

		protected void GiveOrbsToMom(Random random, ItemLocationMap itemLocations, bool useLightwallAsSpell)
		{
			var orbTypes = Helper.GetAllOrbs();

			var spellOrbTypes = useLightwallAsSpell
				? orbTypes.Where(orbType => orbType == EInventoryOrbType.Barrier)
				: orbTypes.Where(orbType => orbType != EInventoryOrbType.Barrier);
			var meleeOrbTypes = orbTypes.Where(orbType => orbType != EInventoryOrbType.Pink); //To annoying as each attack consumes aura power

			var spellOrbType = spellOrbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfoProvider.Get(spellOrbType, EOrbSlot.Spell), itemLocations[ItemKey.TutorialSpellOrb]);

			var meleeOrbType = meleeOrbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfoProvider.Get(meleeOrbType, EOrbSlot.Melee), itemLocations[ItemKey.TutorialMeleeOrb]);
		}

		protected void PlaceGasMaskInALegalSpot(Random random, ItemLocationMap itemLocations)
		{
			var levelIdsToAvoid = new List<int>(3) { 1 };
			var minimalMawRequirements = R.None;

			if (!SeedOptions.Inverted)
			{
				minimalMawRequirements |= R.GateAccessToPast;

				//for non inverted seeds we dont know pyramid keys are required as it can be a classic past seed
				/*var isWatermaskRequiredForMaw = UnlockingMap.PyramidKeysUnlock != R.GateMaw
				                                && UnlockingMap.PyramidKeysUnlock != R.GateCavesOfBanishment;

				if (isWatermaskRequiredForMaw)
					minimalMawRequirements |= R.Swimming;*/

				levelIdsToAvoid.Add(2); //library

				if (UnlockingMap.PyramidKeysUnlock != R.GateSealedCaves)
					levelIdsToAvoid.Add(9); //xarion skelleton
			}
			else
			{
				minimalMawRequirements |= R.Swimming;
				minimalMawRequirements |= UnlockingMap.PyramidKeysUnlock;
			}

			var GasMaskLocation = itemLocations
				.Where(l => !l.IsUsed
							&& !levelIdsToAvoid.Contains(l.Key.LevelId)
							&& l.Gate.CanBeOpenedWith(minimalMawRequirements))
				.SelectRandom(random);

			PutItemAtLocation(ItemInfoProvider.Get(EInventoryRelicType.AirMask), GasMaskLocation);
		}

		protected void FillRemainingChests(Random random, ItemLocationMap itemLocations)
		{
			var itemlist = GenerateItemList(itemLocations);

			var freeLocations = itemLocations
				.Where(l => !l.IsUsed)
				.ToList();

			do
			{
				var location = freeLocations.PopRandom(random);
				var item = itemlist.Count > 0
					? itemlist.PopRandom(random)
					: genericItems.SelectRandom(random);

				PutItemAtLocation(item, location);

			} while (freeLocations.Count > 0);

			FixProgressiveNonProgressionItemsInSameRoom(random, itemLocations);
		}

		void FixProgressiveNonProgressionItemsInSameRoom(Random random, ItemLocationMap itemLocations)
		{
			var progressiveItemLocationsPerType = itemLocations
					.Where(l => l.ItemInfo.Unlocks == R.None)
					.Where(l => l.ItemInfo is ProgressiveItemInfo)
					.GroupBy(l => l.ItemInfo);

			foreach (var progressiveItemLocations in progressiveItemLocationsPerType)
			{
				RoomItemKey roomkey = null;

				foreach (var itemLocation in progressiveItemLocations)
				{
					if (roomkey == null)
						roomkey = itemLocation.Key.ToRoomItemKey();
					else if (roomkey == itemLocation.Key.ToRoomItemKey())
						SwapItemWithOtherNonProgressionItemsNotInRoom(random, itemLocation, itemLocations);
				}
			}
		}

		void SwapItemWithOtherNonProgressionItemsNotInRoom(Random random, ItemLocation itemLocation, ItemLocationMap itemLocations)
		{
			var roomToAvoid = itemLocation.Key.ToRoomItemKey();

			var newItemLocation = itemLocations
				.Where(l => l.ItemInfo.Unlocks == R.None && l.Key.ToRoomItemKey() != roomToAvoid)
				.SelectRandom(random);

			var itemInfoToMove = itemLocation.ItemInfo;
			var newItemInfo = newItemLocation.ItemInfo;

			itemLocation.SetItem(newItemInfo);
			newItemLocation.SetItem(itemInfoToMove);
		}

		List<ItemInfo> GenerateItemList(ItemLocationMap itemLocations)
		{
			var alreadyAssingedItems = itemLocations
				.Where(l => l.IsUsed)
				.Select(l => l.ItemInfo)
				.ToArray();

			var itemlist = itemLocations
				.Where(l => l.DefaultItem != null)
				.Select(l => l.DefaultItem)
				.Where(i => i.Identifier.LootType != LootType.ConstOrb
							&& i.Identifier.LootType != LootType.ConstFamiliar
							&& !genericItems.Contains(i))
				.ToList();

			AddOrbs(itemlist);
			AddFamiliars(itemlist);
			AddExtraItems(itemlist);

			itemlist = itemlist
				.Where(i => !alreadyAssingedItems.Contains(i)
							&& !itemsToRemoveFromGame.Contains(i))
				.ToList();
			return itemlist;
		}

		void AddExtraItems(List<ItemInfo> itemlist) => itemlist.AddRange(itemsToAddToGame);

		void AddFamiliars(List<ItemInfo> itemlist)
		{
			var allFamiliars = ((EInventoryFamiliarType[])Enum.GetValues(typeof(EInventoryFamiliarType)))
				.Where(f => f != EInventoryFamiliarType.None);

			itemlist.AddRange(allFamiliars.Select(familiar => ItemInfoProvider.Get(familiar)));
		}

		void AddOrbs(List<ItemInfo> itemlist)
		{
			var allOrbs = Helper.GetAllOrbs();

			foreach (var orb in allOrbs)
			{
				itemlist.Add(ItemInfoProvider.Get(orb, EOrbSlot.Melee));
				itemlist.Add(ItemInfoProvider.Get(orb, EOrbSlot.Spell));
				itemlist.Add(ItemInfoProvider.Get(orb, EOrbSlot.Passive));
			}
		}

		protected abstract void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation);
	}
}
