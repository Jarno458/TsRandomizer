using System;
using System.Collections.Generic;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.Randomisation;

namespace TsRandomizer.IntermediateObjects
{
	class ItemInfoProvider
	{
		readonly ItemUnlockingMap unlockingMap;

		readonly Dictionary<EInventoryUseItemType, ItemInfo> useItems;
		readonly Dictionary<EInventoryRelicType, ItemInfo> relicItems;
		readonly Dictionary<EInventoryEquipmentType, ItemInfo> enquipmentItems;
		readonly Dictionary<EInventoryFamiliarType, ItemInfo> familierItems;
		readonly Dictionary<int, ItemInfo> orbItems;
		readonly Dictionary<EItemType, ItemInfo> statItems;
		
		public ItemInfoProvider(SeedOptions options, ItemUnlockingMap unlockingMap)
		{
			this.unlockingMap = unlockingMap;

			useItems = new Dictionary<EInventoryUseItemType, ItemInfo>();
			relicItems = new Dictionary<EInventoryRelicType, ItemInfo>();
			enquipmentItems = new Dictionary<EInventoryEquipmentType, ItemInfo>();
			familierItems = new Dictionary<EInventoryFamiliarType, ItemInfo>();
			orbItems = new Dictionary<int, ItemInfo>();
			statItems = new Dictionary<EItemType, ItemInfo>();

			LoadCustomItems();
		}
		
		public ItemInfo Get(ItemIdentifier identifier)
		{
			switch (identifier.LootType)
			{
				case LootType.ConstEquipment: return Get((EInventoryEquipmentType)identifier.ItemId);
				case LootType.ConstFamiliar: return Get((EInventoryFamiliarType)identifier.ItemId);
				case LootType.ConstOrb: return Get((EInventoryOrbType)identifier.ItemId, (EOrbSlot)identifier.SubItemId);
				case LootType.ConstRelic: return Get((EInventoryRelicType)identifier.ItemId);
				case LootType.ConstStat: return Get((EItemType)identifier.ItemId);
				case LootType.ConstUseItem: return Get((EInventoryUseItemType)identifier.ItemId);
				default: throw new ArgumentOutOfRangeException($"Loottype {identifier.LootType} is not supported");
			}
		}

		public virtual ItemInfo Get(EInventoryRelicType relicItem) =>
			GetOrAdd(relicItems, relicItem, () => CreateNew(new ItemIdentifier(relicItem)));

		public virtual ItemInfo Get(EInventoryOrbType orbType, EOrbSlot orbSlot) =>
			GetOrAdd(orbItems, GetOrbKey(orbType, orbSlot), () => CreateNew(new ItemIdentifier(orbType, orbSlot)));

		public virtual ItemInfo Get(EInventoryUseItemType useItem) =>
			GetOrAdd(useItems, useItem, () => CreateNew(new ItemIdentifier(useItem)));

		public virtual ItemInfo Get(EInventoryEquipmentType equipmentItem) =>
			GetOrAdd(enquipmentItems, equipmentItem, () => CreateNew(new ItemIdentifier(equipmentItem)));

		public virtual ItemInfo Get(EInventoryFamiliarType familiarItem) =>
			GetOrAdd(familierItems, familiarItem, () => CreateNew(new ItemIdentifier(familiarItem)));

		public virtual ItemInfo Get(EItemType stat) =>
			GetOrAdd(statItems, stat, () => CreateNew(new ItemIdentifier(stat)));

		static int GetOrbKey(EInventoryOrbType orbType, EOrbSlot orbSlot) => ((int)orbType * 10) + (int)orbSlot;

		ItemInfo CreateNew(ItemIdentifier identifier) => new SingleItemInfo(unlockingMap, identifier);

		static ItemInfo GetOrAdd<T>(IDictionary<T, ItemInfo> dictionary, T item, Func<ItemInfo> createNew)
		{
			if (dictionary.ContainsKey(item))
				return dictionary[item];

			var newItem = createNew();
			dictionary[item] = newItem;
			return newItem;
		}

		void LoadCustomItems()
		{
			foreach (var item in CustomItem.GetAllCustomItems(unlockingMap))
				useItems.Add(item.Identifier.UseItem, item);
		}
	}
}