using System;
using Timespinner.GameAbstractions.Inventory;

namespace TsRanodmizer.IntermediateObjects
{
	class ItemInfo
	{
		public static ItemInfo Dummy = new ItemInfo(EInventoryEquipmentType.DemonHorn);

		public LootType LootType { get; protected set; }
		public int ItemId { get; protected set; }

		public Enum TreasureLootType => LootType.ToETreasureLootType();

		public EInventoryUseItemType UseItem => (EInventoryUseItemType)ItemId;
		public EInventoryRelicType Relic => (EInventoryRelicType)ItemId;
		public EInventoryEquipmentType Emquipment => (EInventoryEquipmentType)ItemId;
		public EInventoryOrbType OrbType => (EInventoryOrbType)(ItemId % 100);
		public EOrbSlot OrbSlot => (EOrbSlot)(ItemId / 100);

		public ItemInfo(EInventoryUseItemType useItem)
		{
			LootType = LootType.ConstUseItem;
			ItemId = (int)useItem;
		}

		public ItemInfo(EInventoryRelicType relicType)
		{
			LootType = LootType.Relic;
			ItemId = (int)relicType;
		}

		public ItemInfo(EInventoryEquipmentType enquipment)
		{
			LootType = LootType.Equipment;
			ItemId = (int)enquipment;
		}

		public ItemInfo(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			LootType = LootType.Orb;
			ItemId = (int)orbType + 100 * (int)orbSlot;
		}
	}
}