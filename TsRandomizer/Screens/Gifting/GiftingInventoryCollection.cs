using System;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.Screens.Gifting
{
	class GiftingInventoryCollection : InventoryUseItemCollection
	{
		readonly Func<InventoryItem, bool> onItemSelected;

		public GiftingInventoryCollection(Func<InventoryItem, bool> onItemSelected)
		{
			this.onItemSelected = onItemSelected;
		}

		public void AddItem(EInventoryUseItemType item) => AddItem(item, 1);
		public void AddItem(EInventoryUseItemType item, int count) => AddItem((int)item, count);
		public void AddItem(EInventoryEquipmentType item) => AddItem(item, 1);
		public void AddItem(EInventoryEquipmentType item, int count) => AddItem((int)item.ToEInventoryUseItemType(), count);
		
		public override void RefreshItemNameAndDescriptions()
		{
			// ReSharper disable once SuggestVarOrType_SimpleTypes
			foreach (InventoryUseItem useItem in Inventory.Values)
			{
				if (!useItem.IsEquipment()) 
					continue;

				var equipment = useItem.ToInventoryEquipment();
				var dynamicInventoryItem = useItem.AsDynamic();
				dynamicInventoryItem.NameKey = equipment.NameKey;
				dynamicInventoryItem.DescriptionKey = equipment.DescriptionKey;
			}

			base.RefreshItemNameAndDescriptions();
		}

		public bool OnUseItemSelected(InventoryUseItem useItem) =>
			!useItem.IsEquipment() 
				? onItemSelected(useItem) 
				: onItemSelected(useItem.ToInventoryEquipment());
	}
}