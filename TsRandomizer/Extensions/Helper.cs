using System;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;

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

		public static EInventoryJournalType[] GetAllJournals()
		{
			return ((EInventoryJournalType[])Enum.GetValues(typeof(EInventoryJournalType))).ToArray();
		}

		public static EInventoryEquipmentType[] GetAllEquipment()
		{
			return ((EInventoryEquipmentType[])Enum.GetValues(typeof(EInventoryEquipmentType))).ToArray();
		}

		public static EInventoryRelicType[] GetAllRelics()
		{
			return ((EInventoryRelicType[])Enum.GetValues(typeof(EInventoryRelicType))).ToArray();
		}

		public static EInventoryUseItemType[] GetAllUseItems()
		{
			return ((EInventoryUseItemType[])Enum.GetValues(typeof(EInventoryUseItemType))).ToArray();
		}

		public static EInventoryFamiliarType[] GetAllFamiliars()
		{
			return ((EInventoryFamiliarType[])Enum.GetValues(typeof(EInventoryFamiliarType))).ToArray();
		}

		public static EInventoryCategoryType[] GetAllInventoryCategories()
		{
			return ((EInventoryCategoryType[])Enum.GetValues(typeof(EInventoryCategoryType))).ToArray();
		}
	}
}
