using System;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameObjects.BaseClasses;
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

			Object._treasureLootType = ItemInfo.TreasureLootType;

			switch (ItemInfo.LootType)
			{
				case LootType.ConstUseItem:
					Object._lootUseItemType = ItemInfo.UseItem;
					break;
				case LootType.ConstRelic:
					Object._lootRelicType = ItemInfo.Relic;
					break;
				case LootType.ConstEquipment:
					Object._lootEquipmentType = ItemInfo.Enquipment;
					break;
				case LootType.ConstOrb:
					Object._lootOrbType = ItemInfo.OrbType;
					Object._lootOrbSlot = ItemInfo.OrbSlot;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(ItemInfo.LootType), ItemInfo.LootType, $"lootType cannot be droppd by {nameof(TreasureChest)}");
			}

			var pickedUp = IsPickedUp;
			Object._isOpened = pickedUp;
			Object._hasDroppedLoot = pickedUp;
			hasDroppedLoot = pickedUp;
			if (pickedUp)
				((Appendage)Object._lidAppendage).AsDynamic().ChangeAnimation(Object._animationIndexOffset + 5, 1, 1f, EAnimationType.None);
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || hasDroppedLoot || !Object._hasDroppedLoot)
				return;
		
			if (ItemInfo.LootType == LootType.Orb)
				Level.GameSave.AddItem(ItemInfo);

			OnItemPickup();

			hasDroppedLoot = true;
		}
	}
}
