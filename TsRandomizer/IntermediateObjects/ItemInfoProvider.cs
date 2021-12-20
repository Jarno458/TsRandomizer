using System;
using System.Collections.Generic;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
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

		readonly Dictionary<ItemInfo, ProgressiveItemInfo> progressiveItems;

		public ItemInfoProvider(SeedOptions options, ItemUnlockingMap unlockingMap)
		{
			this.unlockingMap = unlockingMap;

			useItems = new Dictionary<EInventoryUseItemType, ItemInfo>();
			relicItems = new Dictionary<EInventoryRelicType, ItemInfo>();
			enquipmentItems = new Dictionary<EInventoryEquipmentType, ItemInfo>();
			familierItems = new Dictionary<EInventoryFamiliarType, ItemInfo>();
			orbItems = new Dictionary<int, ItemInfo>();
			statItems = new Dictionary<EItemType, ItemInfo>();

			progressiveItems = new Dictionary<ItemInfo, ProgressiveItemInfo>();

			MakeGearsProgressive();
			MakeBroochProgressive();

			if (options.ProgressiveKeycard)
				MakeKeycardsProgressive();

			if (options.ProgressiveVerticalMovement)
				MakeVerticalMovementProgressive();
		}

		void MakeGearsProgressive()
		{
			var gear1 = Get(EInventoryRelicType.TimespinnerGear1);
			var gear2 = Get(EInventoryRelicType.TimespinnerGear2);
			var gear3 = Get(EInventoryRelicType.TimespinnerGear3);

			var progressiveItem = new ProgressiveItemInfo(gear1, gear2, gear3);

			progressiveItems.Add(gear1, progressiveItem);
			progressiveItems.Add(gear2, progressiveItem);
			progressiveItems.Add(gear3, progressiveItem);
		}

		void MakeBroochProgressive()
		{
			var empireBrooch = Get(EInventoryRelicType.EmpireBrooch);
			var godestBrooch = Get(EInventoryRelicType.EternalBrooch);

			var progressiveItem = new ProgressiveItemInfo(empireBrooch, godestBrooch);

			progressiveItems.Add(empireBrooch, progressiveItem);
			progressiveItems.Add(godestBrooch, progressiveItem);
		}

		void MakeKeycardsProgressive()
		{
			var cardA = Get(EInventoryRelicType.ScienceKeycardA);
			var cardB = Get(EInventoryRelicType.ScienceKeycardB);
			var cardC = Get(EInventoryRelicType.ScienceKeycardC);
			var cardD = Get(EInventoryRelicType.ScienceKeycardD);

			var progressiveItem = new ProgressiveItemInfo(cardD, cardC, cardB, cardA);

			progressiveItems.Add(cardA, progressiveItem);
			progressiveItems.Add(cardB, progressiveItem);
			progressiveItems.Add(cardC, progressiveItem);
			progressiveItems.Add(cardD, progressiveItem);
		}

		void MakeVerticalMovementProgressive()
		{
			var doubleJump = Get(EInventoryRelicType.DoubleJump);
			var lightwall = Get(EInventoryOrbType.Barrier, EOrbSlot.Spell);
			var celestialSash = Get(EInventoryRelicType.EssenceOfSpace);

			var progressiveItem = new ProgressiveItemInfo(doubleJump, lightwall, celestialSash);

			progressiveItems.Add(doubleJump, progressiveItem);
			progressiveItems.Add(lightwall, progressiveItem);
			progressiveItems.Add(celestialSash, progressiveItem);
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

		public ItemInfo Get(EInventoryRelicType relicItem)
		{
			var item = GetOrAdd(relicItems, relicItem, () => CreateNew(new ItemIdentifier(relicItem)));

			return progressiveItems.TryGetValue(item, out var progressiveItem)
				? progressiveItem
				: item;
		}

		public ItemInfo Get(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			var orb = GetOrAdd(orbItems, GetOrbKey(orbType, orbSlot), () => CreateNew(new ItemIdentifier(orbType, orbSlot)));

			return progressiveItems.TryGetValue(orb, out var progressiveItem)
				? progressiveItem
				: orb;
		}

		public ItemInfo Get(EInventoryUseItemType useItem) =>
			GetOrAdd(useItems, useItem, () => CreateNew(new ItemIdentifier(useItem)));

		public ItemInfo Get(EInventoryEquipmentType equipmentItem) =>
			GetOrAdd(enquipmentItems, equipmentItem, () => CreateNew(new ItemIdentifier(equipmentItem)));

		public ItemInfo Get(EInventoryFamiliarType familiarItem) =>
			GetOrAdd(familierItems, familiarItem, () => CreateNew(new ItemIdentifier(familiarItem)));

		public ItemInfo Get(EItemType stat) =>
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
	}
}