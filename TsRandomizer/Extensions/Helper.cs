using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Extensions
{
	static class Helper
	{
		public static EInventoryOrbType[] GetAllOrbs()
		{
			return ((EInventoryOrbType[])Enum.GetValues(typeof(EInventoryOrbType)))
				.Where(o => o != EInventoryOrbType.None && o != EInventoryOrbType.Monske)
				.ToArray();
		}
		public static ELootTier LookupUseItemRarity(EInventoryUseItemType itemType)
		{
			switch (itemType)
			{
				case EInventoryUseItemType.CheveuxFeather:
				case EInventoryUseItemType.CheveuxBreast:
				case EInventoryUseItemType.Drumstick:
				case EInventoryUseItemType.EelMeat:
				case EInventoryUseItemType.Herb:
				case EInventoryUseItemType.WyvernTail:
				case EInventoryUseItemType.PlumpMaggot:
				case EInventoryUseItemType.RottenTail:
				case EInventoryUseItemType.SilverOre:
				case EInventoryUseItemType.SirenInk:
				case EInventoryUseItemType.PlasmaCore:
				case EInventoryUseItemType.PlaceHolderItem1:
					return ELootTier.Trash;
				case EInventoryUseItemType.Potion:
				case EInventoryUseItemType.Ether:
				case EInventoryUseItemType.SandBottle:
				case EInventoryUseItemType.FuturePotion:
				case EInventoryUseItemType.FutureEther:
				case EInventoryUseItemType.Antidote:
				case EInventoryUseItemType.ChaosHeal:
				case EInventoryUseItemType.Jerky:
					return ELootTier.Common;
				case EInventoryUseItemType.HiPotion:
				case EInventoryUseItemType.HiSandBottle:
				case EInventoryUseItemType.HiEther:
				case EInventoryUseItemType.FutureHiPotion:
				case EInventoryUseItemType.FutureHiEther:
				case EInventoryUseItemType.FamiliarTreat:
				case EInventoryUseItemType.OrangeJuice:
					return ELootTier.Uncommon;
				case EInventoryUseItemType.WarpCard:
				case EInventoryUseItemType.CheveuxAuVin:
				case EInventoryUseItemType.FriedCheveux:
				case EInventoryUseItemType.Biscuit:
				case EInventoryUseItemType.FiligreeTea:
				case EInventoryUseItemType.MagicMarbles:
				case EInventoryUseItemType.Spaghetti:
				case EInventoryUseItemType.UnagiRoll:
					return ELootTier.Rare;
				case EInventoryUseItemType.LachiemiSun:
				case EInventoryUseItemType.EmpressCake:
				case EInventoryUseItemType.Casserole:
					return ELootTier.UltraRare;
				default:
					return ELootTier.Uncommon;
			}
		}
		public static ELootTier LookupEquipmentRarity(EInventoryEquipmentType equipmentType)
		{
			switch (equipmentType)
			{
				case EInventoryEquipmentType.OldCoat:
				case EInventoryEquipmentType.Sunglasses:
				case EInventoryEquipmentType.BuckleHat:
				case EInventoryEquipmentType.MetalWristband:
				case EInventoryEquipmentType.MidnightCloak:
				case EInventoryEquipmentType.MotherOfPearl:
					return ELootTier.Trash;
				case EInventoryEquipmentType.PointyHat:
				case EInventoryEquipmentType.AdvisorHat:
				case EInventoryEquipmentType.AdvisorRobe:
				case EInventoryEquipmentType.CalvaryArmor:
				case EInventoryEquipmentType.CalvaryHelmet:
				case EInventoryEquipmentType.CaptainsCap:
				case EInventoryEquipmentType.CaptainsJacket:
				case EInventoryEquipmentType.LeatherArmor:
				case EInventoryEquipmentType.LeatherHelmet:
					return ELootTier.Common;
				case EInventoryEquipmentType.SecurityVisor:
				case EInventoryEquipmentType.SecurityVest:
				case EInventoryEquipmentType.LibrarianHat:
				case EInventoryEquipmentType.LibrarianRobe:
				case EInventoryEquipmentType.EngineerGoggles:
				case EInventoryEquipmentType.SirenHairband:
					return ELootTier.Uncommon;
				case EInventoryEquipmentType.LabGlasses:
				case EInventoryEquipmentType.LabCoat:
				case EInventoryEquipmentType.LachiemCrown:
				case EInventoryEquipmentType.LuckyCoin:
				case EInventoryEquipmentType.DemonHorn:
				case EInventoryEquipmentType.FiligreeClasp:
				case EInventoryEquipmentType.VileteCrown:
				case EInventoryEquipmentType.VileteDress:
					return ELootTier.Rare;
				case EInventoryEquipmentType.Pendulum:
				case EInventoryEquipmentType.BirdStatue:
				case EInventoryEquipmentType.DemonStole:
				case EInventoryEquipmentType.AzureStole:
				case EInventoryEquipmentType.EmpressCoat:
				case EInventoryEquipmentType.NelisteEarring:
				case EInventoryEquipmentType.ShinyRock:
					return ELootTier.UltraRare;
				default:
					return ELootTier.Uncommon;
			}
		}
		public static List<ItemIdentifier> GetAllLoot()
		{
			// Exclude unique and placeholder objects
			var useItems = ((EInventoryUseItemType[])Enum.GetValues(typeof(EInventoryUseItemType)))
				.Where(o => o != EInventoryUseItemType.None
					&& o != EInventoryUseItemType.AlchemistTools
					&& o != EInventoryUseItemType.MagicMarbles
					&& o != EInventoryUseItemType.EssenceCrystal
					&& o != EInventoryUseItemType.GoldNecklace
					&& o != EInventoryUseItemType.MapReveal0
					&& o != EInventoryUseItemType.MapReveal1
					&& o != EInventoryUseItemType.MapReveal2
					&& o != EInventoryUseItemType.RadiationCrystal
					&& o != EInventoryUseItemType.PlasmaIV
					&& o != EInventoryUseItemType.HistoricalDocuments
					&& o != EInventoryUseItemType.FoodSynth
					&& o != EInventoryUseItemType.GalaxyStone
					&& o != EInventoryUseItemType.PlaceHolderItem1
					)
				.ToArray();
			var equipment =
				((EInventoryEquipmentType[])Enum.GetValues(typeof(EInventoryEquipmentType)))
				.Where(o => o != EInventoryEquipmentType.None
					&& o != EInventoryEquipmentType.FamiliarEgg
					&& o != EInventoryEquipmentType.SelenBangle
					&& o != EInventoryEquipmentType.ShinyRock
					&& o != EInventoryEquipmentType.GlassPumpkin
					&& o != EInventoryEquipmentType.EternalCoat
					&& o != EInventoryEquipmentType.EternalTiara
					)
				.ToArray();
			List<ItemIdentifier> loot = new List<ItemIdentifier>();
			foreach (EInventoryUseItemType item in useItems)
				loot.Add(new ItemIdentifier(item));
			foreach (EInventoryEquipmentType item in equipment)
				loot.Add(new ItemIdentifier(item));
			return loot;
		}
	}
}
