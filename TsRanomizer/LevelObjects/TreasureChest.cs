using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.Events;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
	class TreasureChest : LevelObject<TreasureChestEvent>
    {
        public TreasureChest(TreasureChestEvent treasureChest, ItemInfo itemInfo) : base(treasureChest, itemInfo)
        {
        }

		protected override void OnChangeRoom()
        {
	        if (ItemInfo == null || ItemInfo == ItemInfo.Dummy)
				return;

            ObjectPrivate._treasureLootType = ItemInfo.TreasureLootType;

            switch (ItemInfo.LootType)
            {
                case LootType.ConstUseItem:
                    ObjectPrivate._lootUseItemType = ItemInfo.UseItem;
                    break;
                case LootType.ConstRelic:
                    ObjectPrivate._lootRelicType = ItemInfo.Relic;
                    break;
                case LootType.ConstEquipment:
                    ObjectPrivate._lootEquipmentType = ItemInfo.Emquipment;
                    break;
                case LootType.ConstOrb:
                    ObjectPrivate._lootOrbType = ItemInfo.OrbType;
                    ObjectPrivate._lootOrbSlot = ItemInfo.OrbSlot;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ItemInfo.LootType), ItemInfo.LootType, $"lootType cannot be droppd by {nameof(TreasureChest)}");
            }
        }

        protected override void OnUpdate()
        {
	        if (ItemInfo == null || ItemInfo == ItemInfo.Dummy)
		        return;

	        var level = (Level)ObjectPrivate._level;
	        var gameSave = level.GameSave;
	        var inventory = gameSave.Inventory;
	        var orbInventory = inventory.OrbInventory;
	        var orbCollection = orbInventory.Inventory;
			var lootOrbType = (int)(object)ObjectPrivate._lootOrbType;
			
			if (LootType.FromETreasureLootType(ObjectPrivate._treasureLootType) == LootType.Orb
                && ObjectPrivate._hasDroppedLoot
                && !orbCollection.ContainsKey(lootOrbType))
            {
	            orbInventory.AddItem(lootOrbType);

                if (ObjectPrivate._lootOrbSlot == EOrbSlot.Spell || ObjectPrivate._lootOrbSlot == EOrbSlot.All)
	                orbCollection[lootOrbType].IsSpellUnlocked = true;
                if (ObjectPrivate._lootOrbSlot == EOrbSlot.Passive || ObjectPrivate._lootOrbSlot == EOrbSlot.All)
	                orbCollection[lootOrbType].IsPassiveUnlocked = true;
            }
        }
    }
}
