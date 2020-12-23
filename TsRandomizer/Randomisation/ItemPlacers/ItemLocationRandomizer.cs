using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	abstract class ItemLocationRandomizer
	{
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

			if(options.StartWithJewelryBox)
				itemsToRemoveFromGame.Add(ItemInfoProvider.Get(EInventoryRelicType.JewelryBox));

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

		protected void FillTutorial(Random random)
		{
			var orbTypes = Helper.GetAllOrbs();

			var spellOrbTypes = orbTypes.Where(orbType => orbType != EInventoryOrbType.Barrier); //To OP to give as starter spell
			var meleeOrbTypes = orbTypes.Where(orbType => orbType != EInventoryOrbType.Pink); //To annoying as each attack consumes aura power

			var spellOrbType = spellOrbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfoProvider.Get(spellOrbType, EOrbSlot.Spell), ItemLocations[ItemKey.TutorialSpellOrb]);

			var meleeOrbType = meleeOrbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfoProvider.Get(meleeOrbType, EOrbSlot.Melee), ItemLocations[ItemKey.TutorialMeleeOrb]);
		}

		protected void PlaceStarterProgressionItem(Random random)
		{
			var starterLocations = ItemLocations
				.Where(l => l.Key.LevelId != 0 && l.Gate.Requires(Requirement.None))
				.ToArray();

			var starterProgressionItems = new [] {
				ItemInfoProvider.Get(EInventoryRelicType.Dash),
				ItemInfoProvider.Get(EInventoryRelicType.Dash),
				ItemInfoProvider.Get(EInventoryRelicType.DoubleJump),
				ItemInfoProvider.Get(EInventoryRelicType.DoubleJump),
				ItemInfoProvider.Get(EInventoryRelicType.TimespinnerWheel),
				ItemInfoProvider.Get(EInventoryRelicType.TimespinnerWheel),
				ItemInfoProvider.Get(EInventoryRelicType.PyramidsKey),
				ItemInfoProvider.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell),
				ItemInfoProvider.Get(EInventoryRelicType.EssenceOfSpace)
			};

			var item = starterProgressionItems.SelectRandom(random);
			var location = starterLocations.SelectRandom(random);

			PutItemAtLocation(item, location);
		}

		protected void PlaceGassMaskInALegalSpot(Random random)
		{
			var isWatermaskRequiredForMaw = UnlockingMap.PyramidKeysUnlock != Requirement.GateMaw
			                                && UnlockingMap.PyramidKeysUnlock != Requirement.GateCavesOfBanishment;

			var minimalMawRequirements = Requirement.DoubleJump | Requirement.GateAccessToPast;

			if (isWatermaskRequiredForMaw)
				minimalMawRequirements |= Requirement.Swimming;

			var posableGassMaskLocations = ItemLocations
				.Where(l => l.Key.LevelId != 1 && !l.IsUsed && l.Gate.CanBeOpenedWith(minimalMawRequirements))
				.ToArray();

			PutItemAtLocation(ItemInfoProvider.Get(EInventoryRelicType.AirMask), posableGassMaskLocations.SelectRandom(random));
		}

		protected void FillRemainingChests(Random random)
		{
			if (ProgressionOnly)
				return;

			var alreadyAssingedItems = ItemLocations
				.Where(l => l.IsUsed)
				.Select(l => l.ItemInfo)
				.ToArray();
			
			var itemlist = ItemLocations
				.Where(l => l.DefaultItem != null)
				.Select(l => l.DefaultItem)
				.Where(i => i.Identifier.LootType != LootType.ConstOrb 
				            && i.Identifier.LootType != LootType.ConstFamiliar 
				            && !itemsToRemoveFromGame.Contains(i) 
				            && !genericItems.Contains(i))
				.ToList();

			AddOrbs(itemlist);
			AddFamiliers(itemlist);
			AddExtraItems(itemlist);

			itemlist = itemlist
				.Where(i => !alreadyAssingedItems.Contains(i))
				.ToList();

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
		}

		void AddExtraItems(List<ItemInfo> itemlist)
		{
			itemlist.AddRange(itemsToAddToGame);
		}

		void AddFamiliers(List<ItemInfo> itemlist)
		{
			var allFamiliers = ((EInventoryFamiliarType[])Enum.GetValues(typeof(EInventoryFamiliarType)))
				.Where(o => o != EInventoryFamiliarType.None && o != EInventoryFamiliarType.Meyef);

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
