using System;
using Timespinner.GameAbstractions.Inventory;

namespace TsRandomizer.Extensions
{
	static class InventoryItemExtensions
	{
		const int ArmorOffset = 100;

		public static EInventoryUseItemType ToEInventoryUseItemType(this EInventoryEquipmentType equipment) =>
			(EInventoryUseItemType)equipment + ArmorOffset;

		public static EInventoryEquipmentType ToEInventoryUseItemType(this EInventoryUseItemType useItem) =>
			(EInventoryEquipmentType)useItem - ArmorOffset;

		public static InventoryUseItem ToInventoryUseItem(this InventoryEquipment equipment) =>
			new InventoryUseItem(equipment.EquipmentType.ToEInventoryUseItemType()) { Count = equipment.Count };

		public static InventoryEquipment ToInventoryEquipment(this InventoryUseItem useItem) =>
			new InventoryEquipment(useItem.UseItemType.ToEInventoryUseItemType()) { Count = useItem.Count };

		public static bool IsEquipment(this InventoryUseItem useItem) =>
			(int)useItem.UseItemType >= ArmorOffset;

		public static int GetAmount(this InventoryItem item)
		{
			item.AsDynamic().StackCount = 999;
			switch (item)
			{
				case InventoryUseItem useItem:
					return useItem.Count;
				case InventoryEquipment equipment:
					return equipment.Count;
				default:
					return 1;
			}
		}
	}
}
