using System;
using System.Collections.Generic;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;

namespace TsRandomizer.IntermediateObjects
{
	public class ItemInfoProvider
	{
		readonly Dictionary<EInventoryUseItemType, ItemInfo> useItems;
		readonly Dictionary<EInventoryRelicType, ItemInfo> relicItems;
		readonly Dictionary<EInventoryEquipmentType, ItemInfo> enquipmentItems;
		readonly Dictionary<EInventoryFamiliarType, ItemInfo> familierItems;
		readonly Dictionary<int, ItemInfo> orbItems;
		readonly Dictionary<EItemType, ItemInfo> statItems;

		readonly Dictionary<ItemInfo, PogRessiveItemInfo> progressiveItems;

		public ItemInfoProvider()
		{
			useItems = new Dictionary<EInventoryUseItemType, ItemInfo>();
			relicItems = new Dictionary<EInventoryRelicType, ItemInfo>();
			enquipmentItems = new Dictionary<EInventoryEquipmentType, ItemInfo>();
			familierItems = new Dictionary<EInventoryFamiliarType, ItemInfo>();
			orbItems = new Dictionary<int, ItemInfo>();
			statItems = new Dictionary<EItemType, ItemInfo>();
			progressiveItems = new Dictionary<ItemInfo, PogRessiveItemInfo>();
		}

		public void EnableProgressiveItems()
		{
			MakeGearsProgressive();
		}

		void MakeGearsProgressive()
		{
			var gear1 = Get(EInventoryRelicType.TimespinnerGear1);
			var gear2 = Get(EInventoryRelicType.TimespinnerGear2);
			var gear3 = Get(EInventoryRelicType.TimespinnerGear3);

			var progressiveItem = new PogRessiveItemInfo(gear1, gear2, gear3);

			progressiveItems.Add(gear1, progressiveItem);
			progressiveItems.Add(gear2, progressiveItem);
			progressiveItems.Add(gear3, progressiveItem);
		}

		public ItemInfo Get(EInventoryUseItemType useItem)
		{
			return GetOrAdd(useItems, useItem, () => new SingleItemInfo(useItem));
		}

		public ItemInfo Get(EInventoryRelicType relicItem)
		{
			var item = GetOrAdd(relicItems, relicItem, () => new SingleItemInfo(relicItem));

			return progressiveItems.TryGetValue(item, out PogRessiveItemInfo progressiveItem)
				? progressiveItem
				: item;
		}

		public ItemInfo Get(EInventoryEquipmentType equipmentItem)
		{
			return GetOrAdd(enquipmentItems, equipmentItem, () => new SingleItemInfo(equipmentItem));
		}

		public ItemInfo Get(EInventoryFamiliarType familiarItem)
		{
			return GetOrAdd(familierItems, familiarItem, () => new SingleItemInfo(familiarItem));
		}

		public ItemInfo Get(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			return GetOrAdd(orbItems, GetOrbKey(orbType, orbSlot), () => new SingleItemInfo(orbType, orbSlot));
		}

		public ItemInfo Get(EItemType stat)
		{
			return GetOrAdd(statItems, stat, () => new SingleItemInfo(stat));
		}

		static int GetOrbKey(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			return ((int)orbType * 10) + (int)orbSlot;
		}

		static ItemInfo GetOrAdd<T>(IDictionary<T, ItemInfo> dictionary, T item, Func<ItemInfo> createNew)
		{
			if (dictionary.ContainsKey(item))
				return dictionary[item];

			var newItem = createNew();
			dictionary[item] = newItem;
			return newItem;
		}
	}
}