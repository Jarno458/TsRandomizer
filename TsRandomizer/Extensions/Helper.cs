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
		public static List<ItemIdentifier> GetAllLoot()
		{
			// Exclude unique and placeholder objects
			var useItems = ((EInventoryUseItemType[])Enum.GetValues(typeof(EInventoryUseItemType)))
				.Where(o => o != EInventoryUseItemType.None
					&& o != EInventoryUseItemType.PlaceHolderItem1
					&& o != EInventoryUseItemType.AlchemistTools
					&& o != EInventoryUseItemType.MagicMarbles
					&& o != EInventoryUseItemType.MapReveal0
					&& o != EInventoryUseItemType.MapReveal1
					&& o != EInventoryUseItemType.MapReveal2
					&& o != EInventoryUseItemType.RadiationCrystal
					&& o != EInventoryUseItemType.PlasmaIV
					&& o != EInventoryUseItemType.HistoricalDocuments
					&& o != EInventoryUseItemType.FoodSynth
					&& o != EInventoryUseItemType.GalaxyStone
					)
				.ToArray();
			var equipment =
				((EInventoryEquipmentType[])Enum.GetValues(typeof(EInventoryEquipmentType)))
				.Where(o => o != EInventoryEquipmentType.None
					&& o != EInventoryEquipmentType.FamiliarEgg
					&& o != EInventoryEquipmentType.SelenBangle
					&& o != EInventoryEquipmentType.ShinyRock
					&& o != EInventoryEquipmentType.GlassPumpkin
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
