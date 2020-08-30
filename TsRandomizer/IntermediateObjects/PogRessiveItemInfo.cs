using System;
using System.Linq;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;

namespace TsRandomizer.IntermediateObjects
{
	public class PogRessiveItemInfo : ItemInfo
	{
		public ItemInfo[] Items { get; }

		ItemInfo currentItem;

		public override LootType LootType => currentItem.LootType;
		public override int ItemId => currentItem.ItemId;
		public override EInventoryUseItemType UseItem => currentItem.UseItem;
		public override EInventoryRelicType Relic => currentItem.Relic;
		public override EInventoryEquipmentType Enquipment => currentItem.Enquipment;
		public override EInventoryFamiliarType Familiar => currentItem.Familiar;
		public override EInventoryOrbType OrbType => currentItem.OrbType;
		public override EOrbSlot OrbSlot => currentItem.OrbSlot;
		public override EItemType Stat => currentItem.Stat;
		public override Enum TreasureLootType => currentItem.TreasureLootType;
		public override int AnimationIndex => currentItem.AnimationIndex;
		public override BestiaryItemDropSpecification BestiaryItemDropSpecification => currentItem.BestiaryItemDropSpecification;

		public PogRessiveItemInfo(params ItemInfo[] items)
		{
			Items = items;
			currentItem = items[0];
		}

		public override void SetPickupAction(Action<Level> onPickUp)
		{
			throw new InvalidOperationException("To set pickup actions on progressive items, set the pickup action on item before its assinged to the progressive item");
		}

		public override void OnPickup(Level level)
		{
			currentItem.OnPickup(level);

			if (currentItem != Items.Last())
				currentItem = Items[Array.IndexOf(Items, currentItem) + 1];
		}
	}
}