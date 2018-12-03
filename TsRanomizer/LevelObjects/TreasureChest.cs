using System;
using Timespinner.GameAbstractions.Gameplay;
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
			if (ItemInfo == null)
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
					Reflected._lootEquipmentType = ItemInfo.Enquipment;
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
			if (ItemInfo == null || hasDroppedLoot || !Reflected._hasDroppedLoot)
				return;

			var level = (Level)Reflected._level;
				
			if (ItemInfo.LootType == LootType.Orb)
				level.GameSave.AddItem(ItemInfo);

			ItemInfo.OnPickup(level);
			hasDroppedLoot = true;
		}
	}
}
