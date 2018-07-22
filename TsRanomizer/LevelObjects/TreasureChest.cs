using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.Events;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
	class TreasureChest : LevelObject<TreasureChestEvent>
	{
		bool hasDroppedLoot;

		public TreasureChest(TreasureChestEvent treasureChest, ItemInfo itemInfo) : base(treasureChest, itemInfo)
		{
		}

		protected override void Initialize()
		{
			if (ItemInfo == null || ItemInfo == ItemInfo.Dummy)
				return;

			Reflected._treasureLootType = ItemInfo.TreasureLootType;

			switch (ItemInfo.LootType)
			{
				case LootType.ConstUseItem:
					Reflected._lootUseItemType = ItemInfo.UseItem;
					break;
				case LootType.ConstRelic:
					Reflected._lootRelicType = ItemInfo.Relic;
					break;
				case LootType.ConstEquipment:
					Reflected._lootEquipmentType = ItemInfo.Emquipment;
					break;
				case LootType.ConstOrb:
					Reflected._lootOrbType = ItemInfo.OrbType;
					Reflected._lootOrbSlot = ItemInfo.OrbSlot;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(ItemInfo.LootType), ItemInfo.LootType, $"lootType cannot be droppd by {nameof(TreasureChest)}");
			}
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || ItemInfo == ItemInfo.Dummy)
				return;

			if(hasDroppedLoot)
				return;

			if (ItemInfo.LootType == LootType.Orb && Reflected._hasDroppedLoot)
			{
				var gameSave = ((Level)Reflected._level).GameSave;
				gameSave.AddOrb(ItemInfo.OrbType, ItemInfo.OrbSlot);
				hasDroppedLoot = true;
			}
		}
	}
}
