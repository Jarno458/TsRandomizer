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
		protected readonly SeedOptions SeedOptions;
		protected readonly ItemInfoProvider ItemInfoProvider;
		protected readonly ItemLocationMap ItemLocations;
		protected readonly ItemUnlockingMap UnlockingMap;
		protected readonly bool ProgressionOnly;

		readonly List<ItemInfo> itemsToRemoveFromGame;
		readonly ItemInfo[] itemsToAddToGame;
		readonly ItemInfo[] genericItems;

		protected ItemLocationRandomizer(
			SeedOptions options,
			ItemInfoProvider itemInfoProvider, 
			ItemLocationMap itemLocations, 
			ItemUnlockingMap unlockingMap, 
			bool progressionOnly
		)
		{
			SeedOptions = options;
			ItemInfoProvider = itemInfoProvider;
			ItemLocations = itemLocations;
			UnlockingMap = unlockingMap;
			ProgressionOnly = progressionOnly;

			itemsToRemoveFromGame = new List<ItemInfo>
			{
				ItemInfoProvider.Get(EInventoryUseItemType.MagicMarbles),
				ItemInfoProvider.Get(EInventoryUseItemType.GoldRing),
				ItemInfoProvider.Get(EInventoryUseItemType.GoldNecklace),
				ItemInfoProvider.Get(EInventoryUseItemType.SilverOre),
				ItemInfoProvider.Get(EInventoryUseItemType.EssenceCrystal),
			};

			if(SeedOptions.StartWithJewelryBox)
				itemsToRemoveFromGame.Add(ItemInfoProvider.Get(EInventoryRelicType.JewelryBox));
			if(SeedOptions.StartWithMeyef)
				itemsToRemoveFromGame.Add(ItemInfoProvider.Get(EInventoryFamiliarType.Meyef));
			if(SeedOptions.StartWithTalaria)
				itemsToRemoveFromGame.Add(ItemInfoProvider.Get(EInventoryRelicType.Dash));

			itemsToAddToGame = new[]
			{
				ItemInfoProvider.Get(EInventoryEquipmentType.SelenBangle),
				ItemInfoProvider.Get(EInventoryEquipmentType.GlassPumpkin),
				ItemInfoProvider.Get(EInventoryEquipmentType.EternalCoat),
				ItemInfoProvider.Get(EInventoryEquipmentType.EternalTiara),
				ItemInfoProvider.Get(EInventoryEquipmentType.LibrarianHat),
				ItemInfoProvider.Get(EInventoryEquipmentType.LibrarianRobe),
				ItemInfoProvider.Get(EInventoryEquipmentType.MetalWristband),
				ItemInfoProvider.Get(EInventoryEquipmentType.NelisteEarring),
				ItemInfoProvider.Get(EInventoryEquipmentType.FamiliarEgg),
				ItemInfoProvider.Get(EInventoryEquipmentType.LuckyCoin),
				ItemInfoProvider.Get(EInventoryRelicType.EternalBrooch),
				ItemInfoProvider.Get(EInventoryRelicType.FamiliarAltMeyef),
				ItemInfoProvider.Get(EInventoryRelicType.FamiliarAltCrow),
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
				ItemInfoProvider.Get(EInventoryUseItemType.HiSandBottle),
			};
		}

		protected void PlaceStarterProgressionItems(Random random)
		{
			if (SeedOptions.StartWithTalaria /*|| SeedOptions.Inverted*/)
				GiveOrbsToMom(random, false);
			else 
				PlaceStarterProgressionItem(random);
		}

		void PlaceStarterProgressionItem(Random random)
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
				GiveOrbsToMom(random, true);
			}
			else
			{
				GiveOrbsToMom(random, false);

				if (SeedOptions.StartWithTalaria) return;

				PutStarterProgressionItemInReachableLocation(random, starterProgressionItem);
			}
		}

		void PutStarterProgressionItemInReachableLocation(Random random, ItemInfo starterProgressionItem)
		{
			var starterLocations = ItemLocations
				.Where(l => l.Key.LevelId != 0 && l.Gate.Requires(R.None))
				.ToArray();

			var location = starterLocations.SelectRandom(random);

			PutItemAtLocation(starterProgressionItem, location);
		}

		static bool ShouldGiveLightwall(Random random, ItemInfo starterProgressionItem)
		{
			return (starterProgressionItem.Identifier == new ItemIdentifier(EInventoryOrbType.Barrier, EOrbSlot.Spell))
			       && random.Next(1, 5) == 1;
		}

		protected void GiveOrbsToMom(Random random, bool useLightwallAsSpell)
		{
			var orbTypes = Helper.GetAllOrbs();

			var spellOrbTypes = useLightwallAsSpell
				? orbTypes.Where(orbType => orbType == EInventoryOrbType.Barrier)
				: orbTypes.Where(orbType => orbType != EInventoryOrbType.Barrier);
			var meleeOrbTypes = orbTypes.Where(orbType => orbType != EInventoryOrbType.Pink); //To annoying as each attack consumes aura power

			var spellOrbType = spellOrbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfoProvider.Get(spellOrbType, EOrbSlot.Spell), ItemLocations[ItemKey.TutorialSpellOrb]);

			var meleeOrbType = meleeOrbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfoProvider.Get(meleeOrbType, EOrbSlot.Melee), ItemLocations[ItemKey.TutorialMeleeOrb]);
		}

		protected void PlaceGassMaskInALegalSpot(Random random)
		{
			var levelIdsToAvoid = new List<int>{ 1 };
			R minimalMawRequirements = R.DoubleJump;

			if (true || !SeedOptions.Inverted)
			{
				minimalMawRequirements |= R.GateAccessToPast;

				var isWatermaskRequiredForMaw = UnlockingMap.PyramidKeysUnlock != R.GateMaw
				                                && UnlockingMap.PyramidKeysUnlock != R.GateCavesOfBanishment;

				if (isWatermaskRequiredForMaw)
					minimalMawRequirements |= R.Swimming;

				levelIdsToAvoid.Add(2); //library
				levelIdsToAvoid.Add(9); //xarion skelleton
			}
			else
			{
				minimalMawRequirements |= R.Teleport;
				minimalMawRequirements |= UnlockingMap.PyramidKeysUnlock;
			}

			var posableGassMaskLocations = ItemLocations
				.Where(l =>  !l.IsUsed && !levelIdsToAvoid.Contains(l.Key.LevelId) && l.Gate.CanBeOpenedWith(minimalMawRequirements))
				.ToArray();

			PutItemAtLocation(ItemInfoProvider.Get(EInventoryRelicType.AirMask), posableGassMaskLocations.SelectRandom(random));
		}

		protected void FillRemainingChests(Random random)
		{
			if (ProgressionOnly)
				return;

			var itemlist = GenerateItemList();

			var freeLocations = ItemLocations
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

			FixProgressiveNonProgressionItemsInSameRoom(random);
		}

		void FixProgressiveNonProgressionItemsInSameRoom(Random random)
		{
			var progressiveItemLocationsPerType = ItemLocations
					.Where(l => l.ItemInfo.Unlocks == R.None)
					.Where(l => l.ItemInfo is PogRessiveItemInfo)
					.GroupBy(l => l.ItemInfo);

			foreach (IGrouping<ItemInfo, ItemLocation> progressiveItemLocations in progressiveItemLocationsPerType)
			{
				RoomItemKey roomkey = null;

				foreach (var itemLocation in progressiveItemLocations)
				{
					if(roomkey == null)
						roomkey = itemLocation.Key.ToRoomItemKey();
					else if (roomkey == itemLocation.Key.ToRoomItemKey())
						SwapItemWithOtherNonProgressionItemsNotInRoom(itemLocation, random);
				}
			}
		}

		void SwapItemWithOtherNonProgressionItemsNotInRoom(ItemLocation itemLocation, Random random)
		{
			var roomToAvoid = itemLocation.Key.ToRoomItemKey();

			var newItemLocation = ItemLocations
				.Where(l => l.ItemInfo.Unlocks == R.None && l.Key.ToRoomItemKey() != roomToAvoid)
				.SelectRandom(random);

			var itemInfoToMove = itemLocation.ItemInfo;
			var newItemInfo = newItemLocation.ItemInfo;

			itemLocation.SetItem(newItemInfo);
			newItemLocation.SetItem(itemInfoToMove);
		}

		List<ItemInfo> GenerateItemList()
		{
			var alreadyAssingedItems = ItemLocations
				.Where(l => l.IsUsed)
				.Select(l => l.ItemInfo)
				.ToArray();

			var itemlist = ItemLocations
				.Where(l => l.DefaultItem != null)
				.Select(l => l.DefaultItem)
				.Where(i => i.Identifier.LootType != LootType.ConstOrb
				            && i.Identifier.LootType != LootType.ConstFamiliar
				            && !genericItems.Contains(i))
				.ToList();

			AddOrbs(itemlist);
			AddFamiliers(itemlist);
			AddExtraItems(itemlist);

			itemlist = itemlist
				.Where(i => !alreadyAssingedItems.Contains(i)
				            && !itemsToRemoveFromGame.Contains(i))
				.ToList();
			return itemlist;
		}

		void AddExtraItems(List<ItemInfo> itemlist)
		{
			itemlist.AddRange(itemsToAddToGame);
		}

		void AddFamiliers(List<ItemInfo> itemlist)
		{
			var allFamiliers = ((EInventoryFamiliarType[])Enum.GetValues(typeof(EInventoryFamiliarType)))
				.Where(f => f != EInventoryFamiliarType.None);

			itemlist.AddRange(allFamiliers.Select(familiar => ItemInfoProvider.Get(familiar)));
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
