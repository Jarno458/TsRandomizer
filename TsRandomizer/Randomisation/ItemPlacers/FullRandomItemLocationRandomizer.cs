﻿using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.Settings;
using R = TsRandomizer.Randomisation.Requirement;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	class FullRandomItemLocationRandomizer : ItemLocationRandomizer
	{
		ItemLocationMap itemLocations;
		SettingCollection settings;

		readonly List<ItemInfo> itemsToRemoveFromGame;
		readonly List<ItemInfo> itemsToAddToGame;
		readonly List<ItemInfo> genericItems;
		readonly List<ItemInfo> traps;

		public FullRandomItemLocationRandomizer(
			Seed seed,
			SettingCollection settings,
			ItemInfoProvider itemInfoProvider,
			ItemUnlockingMap unlockingMap
		) : base(seed, itemInfoProvider, unlockingMap)
		{
			this.settings = settings;

			itemsToRemoveFromGame = new List<ItemInfo>
			{
				//orb upgrade items
				ItemInfoProvider.Get(EInventoryUseItemType.MagicMarbles),
				ItemInfoProvider.Get(EInventoryUseItemType.GoldRing),
				ItemInfoProvider.Get(EInventoryUseItemType.GoldNecklace),
				//quest items
				ItemInfoProvider.Get(EInventoryUseItemType.SilverOre),
				ItemInfoProvider.Get(EInventoryUseItemType.EssenceCrystal),
				ItemInfoProvider.Get(EInventoryEquipmentType.ShinyRock),
				ItemInfoProvider.Get(EInventoryUseItemType.FoodSynth),
				ItemInfoProvider.Get(EInventoryUseItemType.GalaxyStone),
				ItemInfoProvider.Get(EInventoryUseItemType.PlasmaIV)
			};

			if (SeedOptions.StartWithJewelryBox)
				itemsToRemoveFromGame.Add(ItemInfoProvider.Get(EInventoryRelicType.JewelryBox));
			if (SeedOptions.StartWithMeyef)
				itemsToRemoveFromGame.Add(ItemInfoProvider.Get(EInventoryFamiliarType.Meyef));

			itemsToAddToGame = new List<ItemInfo>
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
				ItemInfoProvider.Get(EInventoryRelicType.EternalBrooch),
				ItemInfoProvider.Get(EInventoryRelicType.FamiliarAltMeyef),
				ItemInfoProvider.Get(EInventoryRelicType.FamiliarAltCrow)
			};

			if (SeedOptions.UnchainedKeys)
			{
				itemsToAddToGame.Add(ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.TimewornWarpBeacon))); // Past
				itemsToAddToGame.Add(ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.ModernWarpBeacon))); // Present
				if (SeedOptions.EnterSandman)
					itemsToAddToGame.Add(ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.MysteriousWarpBeacon))); // Pyramid
			}

			genericItems = new List<ItemInfo>
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

			traps = new List<ItemInfo>(8);
			//i hate it, we should not be using settings in here
			if (settings.SparrowTrap.Value)
				traps.Add(itemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.MeteorSparrowTrap)));
			if (settings.PoisonTrap.Value)
				traps.Add(itemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.PoisonTrap)));
			if (settings.ChaosTrap.Value)
				traps.Add(itemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.ChaosTrap)));
			if (settings.NeurotoxinTrap.Value)
				traps.Add(itemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.NeurotoxinTrap)));
			if (settings.BeeTrap.Value)
				traps.Add(itemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.BeeTrap)));
			if (settings.ThrowStunTrap.Value)
				traps.Add(itemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.ThrowStunTrap)));
			if (settings.SpiderTrap.Value)
				traps.Add(itemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.SpiderTrap)));
			if (settings.LightsOutTrap.Value)
				traps.Add(itemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.LightsOutTrap)));
			if (settings.PalmPunchTrap.Value)
				traps.Add(itemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.PalmPunchTrap)));
		}

		public override ItemLocationMap GenerateItemLocationMap(bool isProgressionOnly)
		{
			var random = new Random((int)Seed.Id);

			itemLocations = new ItemLocationMap(ItemInfoProvider, UnlockingMap, Seed);

			AddRandomItemsToLocationMap(random, isProgressionOnly);

			return itemLocations;
		}

		void AddRandomItemsToLocationMap(Random random, bool isProgressionOnly)
		{
			PlaceStarterProgressionItems(random);

			if (!SeedOptions.GasMaw)
				PlaceGasMaskInALegalSpot(random);

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

			if (!isProgressionOnly)
			{
				if (settings.NothingVenture.Value)
					FillWithNothing();
				else
					FillRemainingChests(random);
			}
		}

		protected void PlaceStarterProgressionItems(Random random)
		{
			if (Seed.StartingEra == Era.Past || (Seed.StartingEra == Era.Present && (SeedOptions.StartWithTalaria || Seed.FloodFlags.LakeDesolation))) 
				GiveOrbsToMom(random, false);
			else
				PlaceStarterProgressionInLakeDesolationItem(random);
		}
		
		void PlaceStarterProgressionInLakeDesolationItem(Random random)
		{
			var starterProgressionItems = new List<ItemInfo> {
				ItemInfoProvider.Get(EInventoryRelicType.DoubleJump),
				ItemInfoProvider.Get(EInventoryRelicType.DoubleJump),
			};

			if (!SeedOptions.PyramidStart)
			{
				starterProgressionItems.Add(ItemInfoProvider.Get(EInventoryRelicType.Dash));
				starterProgressionItems.Add(ItemInfoProvider.Get(EInventoryRelicType.Dash));
				starterProgressionItems.Add(ItemInfoProvider.Get(EInventoryRelicType.TimespinnerWheel));
				starterProgressionItems.Add(ItemInfoProvider.Get(EInventoryRelicType.TimespinnerWheel));
			}

			if (!SeedOptions.ProgressiveVerticalMovement)
			{
				starterProgressionItems.Add(ItemInfoProvider.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell));
				starterProgressionItems.Add(ItemInfoProvider.Get(EInventoryRelicType.EssenceOfSpace));
			}

			if (SeedOptions.UnchainedKeys)
			{
				starterProgressionItems.Add(ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.TimewornWarpBeacon)));
				starterProgressionItems.Add(ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.TimewornWarpBeacon)));
				starterProgressionItems.Add(ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.ModernWarpBeacon)));
				starterProgressionItems.Add(ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.ModernWarpBeacon)));

				if (SeedOptions.EnterSandman)
				{
					starterProgressionItems.Add(ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.MysteriousWarpBeacon)));
					starterProgressionItems.Add(ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.MysteriousWarpBeacon)));
				}
			}
			else
			{
				starterProgressionItems.Add(ItemInfoProvider.Get(EInventoryRelicType.PyramidsKey));
			}

			var starterProgressionItem = starterProgressionItems.SelectRandom(random);

			if (ShouldGiveLightwall(random, starterProgressionItem))
			{
				GiveOrbsToMom(random, true);
			}
			else
			{
				GiveOrbsToMom(random, false);

				PutStarterProgressionItemInReachableLocation(random, starterProgressionItem);
			}
			
			if (SeedOptions.PyramidStart)
				ProvidePyramidExit(random, starterProgressionItem);
		}
		
		void ProvidePyramidExit(Random random, ItemInfo starterProgressionItem)
		{
			List<ItemInfo> pyramidExitItems;

			if (SeedOptions.UnchainedKeys)
			{
				pyramidExitItems = new List<ItemInfo>
				{
					ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.TimewornWarpBeacon)),
					ItemInfoProvider.Get(CustomItem.GetIdentifier(CustomItemType.ModernWarpBeacon))
				};
			}
			else
			{
				pyramidExitItems = new List<ItemInfo> {
					ItemInfoProvider.Get(EInventoryRelicType.PyramidsKey),
				};
			}

			if (pyramidExitItems.Any(i => i == starterProgressionItem))
				return;

			var pyramidLocations = itemLocations
				.Where(l => (l.Key.LevelId == 15 || l.Key.LevelId == 16) && !l.IsUsed)
				.Concat(itemLocations[new ItemKey(14, 0, 240, 192)]) // Temporal Gyre: Forest Entrance
				.Where(l => l.Gate.CanBeOpenedWith(starterProgressionItem.Unlocks))
				.ToArray();

			var pyramidExitItem = pyramidExitItems.SelectRandom(random);
			var location = pyramidLocations.SelectRandom(random);

			PutItemAtLocation(pyramidExitItem, location);
		}

		void PutStarterProgressionItemInReachableLocation(Random random, ItemInfo starterProgressionItem)
		{
			var starterLocations = itemLocations
				.Where(l => l.Key.LevelId != 0 && l.Gate.CanBeOpenedWith(R.None))
				.ToArray();

			var location = starterLocations.SelectRandom(random);

			PutItemAtLocation(starterProgressionItem, location);
		}

		static bool ShouldGiveLightwall(Random random, ItemInfo starterProgressionItem) =>
			(starterProgressionItem.Identifier == new ItemIdentifier(EInventoryOrbType.Barrier, EOrbSlot.Spell))
				&& random.Next(1, 5) == 1;

		protected void GiveOrbsToMom(Random random, bool useLightwallAsSpell)
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

		protected void PlaceGasMaskInALegalSpot(Random random)
		{
			var levelIdsToAvoid = new List<int>(3) { 1 };
			var minimalMawRequirements = R.None;

			if (Seed.StartingEra == Era.Present)
			{
				minimalMawRequirements |= R.GateAccessToPast;

				//for non inverted seeds we dont know if the twin pyramid key is required as it can be a classic past seed
				/*var isWatermaskRequiredForMaw = UnlockingMap.PyramidKeysUnlock != R.GateMaw
				                                && UnlockingMap.PyramidKeysUnlock != R.GateCavesOfBanishment;

				if (isWatermaskRequiredForMaw)
					minimalMawRequirements |= R.Swimming;*/

				levelIdsToAvoid.Add(2); //library
				levelIdsToAvoid.Add(9); //xarion skelleton
			}
			else
			{
				if (!Seed.FloodFlags.DryLakeSerene)
					minimalMawRequirements |= R.Swimming;

				var pastUnlock = SeedOptions.UnchainedKeys
					? UnlockingMap.GetAllUnlock(CustomItem.GetIdentifier(CustomItemType.ModernWarpBeacon))
					: UnlockingMap.GetAllUnlock(new ItemIdentifier(EInventoryRelicType.PyramidsKey));

				minimalMawRequirements |= pastUnlock;
			}

			var gasMaskLocation = itemLocations
				.Where(l => !l.IsUsed
							&& !levelIdsToAvoid.Contains(l.Key.LevelId)
							&& l.Gate.CanBeOpenedWith(minimalMawRequirements))
				.SelectRandom(random);

			PutItemAtLocation(ItemInfoProvider.Get(EInventoryRelicType.AirMask), gasMaskLocation);
		}

		protected void FillRemainingChests(Random random)
		{
			var itemlist = GenerateItemList();

			var freeLocations = itemLocations
				.Where(l => !l.IsUsed)
				.ToList();

			if (itemlist.Count > freeLocations.Count)
				throw new Exception($"Not enough locations to place all items, locations {freeLocations.Count}, items: {itemlist.Count}");

			//item pool
			do
			{
				var location = freeLocations.PopRandom(random);
				var item = itemlist.PopRandom(random);

				PutItemAtLocation(item, location);

			} while (itemlist.Count > 0);

			//traps
			if (SeedOptions.TrappedChests && traps.Any())
			{
				var trapChance = 15d;
				var trapsToPlace = Math.Ceiling((freeLocations.Count / 100d) * trapChance);
				for (int i = 0; i < trapsToPlace; i++)
				{
					if (!freeLocations.Any())
						break;

					var location = freeLocations.PopRandom(random);
					var item = traps.SelectRandom(random);

					PutItemAtLocation(item, location);
				}
			}

			//filler
			while (freeLocations.Count > 0)
			{
				var location = freeLocations.PopRandom(random);
				var item = genericItems.SelectRandom(random);

				PutItemAtLocation(item, location);
			}

			FixProgressiveNonProgressionItemsInSameRoom(random);
		}

		protected void FillWithNothing()
		{
			var freeLocations = itemLocations
				.Where(l => !l.IsUsed)
				.ToList();

			var nothing = ItemInfoProvider.Get(EInventoryUseItemType.PlaceHolderItem1);

			foreach (var freeLocation in freeLocations)
				PutItemAtLocation(nothing, freeLocation);
		}

		void FixProgressiveNonProgressionItemsInSameRoom(Random random)
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
						SwapItemWithOtherNonProgressionItemsNotInRoom(random, itemLocation);
				}
			}
		}

		void SwapItemWithOtherNonProgressionItemsNotInRoom(Random random, ItemLocation itemLocation)
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

		List<ItemInfo> GenerateItemList()
		{
			var alreadyAssingedItems = itemLocations
				.Where(l => l.IsUsed)
				.Select(l => l.ItemInfo)
				.ToList();

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

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation) => itemLocation.SetItem(itemInfo);
	}
}
