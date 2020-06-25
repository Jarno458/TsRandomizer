using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Items.ItemDropPickup")]
	// ReSharper disable once UnusedMember.Global
	class ItemDropPickup : ItemManipulator
	{
		bool hasDroppedLoot;

		public ItemDropPickup(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
		}

		protected override void Initialize()
		{
			if (ItemInfo == null)
				return;

			if (IsPickedUp)
			{
				Object.Kill();
			}
			else
			{
				Object.IsFound = false;
				hasDroppedLoot = false;
			}
			
			Object._itemData = ItemInfo.BestiaryItemDropSpecification;
			Object._category = ItemInfo.LootType.ToEInventoryCategoryType();

			switch (ItemInfo.LootType)
			{
				case LootType.ConstRelic:
					Object._relicType = ItemInfo.Relic;
					break;

				case LootType.ConstUseItem:
					Object._useItemType = ItemInfo.UseItem;
					break;

				case LootType.ConstEquipment:
					Object._equipmentType = ItemInfo.Enquipment;
					break;
			}

			Object.ChangeAnimation(ItemInfo.AnimationIndex);
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || hasDroppedLoot || !Object.IsFound)
				return;

			OnItemPickup();

			hasDroppedLoot = true;
		}
	}
}
