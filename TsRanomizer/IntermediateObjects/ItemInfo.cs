using System;
using System.Collections.Generic;
using System.Reflection;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.Extensions;

namespace TsRanodmizer.IntermediateObjects
{
	class ItemInfo : IEquatable<ItemInfo>
	{
		static readonly Type InventoryItemType = TimeSpinnerType
			.Get("Timespinner.GameAbstractions.Inventory.InventoryItem");
		static readonly MethodInfo GetIconFromUseItemMethod = InventoryItemType
			.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryUseItemType));
		static readonly MethodInfo GetIconFromOrbMethod = InventoryItemType
			.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryOrbType), typeof(EOrbSlot));
		static readonly MethodInfo GetIconFromRelicMethod = InventoryItemType
			.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryRelicType));
		static readonly MethodInfo GetIconFromEnquipmentMethod = InventoryItemType
			.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryEquipmentType));
		static readonly MethodInfo GetIconFromFamilierMethod = InventoryItemType
			.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryFamiliarType));
		
		public static ItemInfo Dummy = new ItemInfo(EInventoryEquipmentType.DemonHorn);

		static readonly Dictionary<EInventoryUseItemType, ItemInfo> UseItems = new Dictionary<EInventoryUseItemType, ItemInfo>();
		static readonly Dictionary<EInventoryRelicType, ItemInfo> RelicItems = new Dictionary<EInventoryRelicType, ItemInfo>();
		static readonly Dictionary<EInventoryEquipmentType, ItemInfo> EnquipmentItems = new Dictionary<EInventoryEquipmentType, ItemInfo>();
		static readonly Dictionary<EInventoryFamiliarType, ItemInfo> FamilierItems = new Dictionary<EInventoryFamiliarType, ItemInfo>();
		static readonly Dictionary<int, ItemInfo> OrbItems = new Dictionary<int, ItemInfo>();

		public static ItemInfo Get(EInventoryUseItemType useItem)
		{
			return GetOrAdd(UseItems, useItem, () => new ItemInfo(useItem));
		}

		public static ItemInfo Get(EInventoryRelicType relicItem)
		{
			return GetOrAdd(RelicItems, relicItem, () => new ItemInfo(relicItem));
		}

		public static ItemInfo Get(EInventoryEquipmentType equipmentItem)
		{
			return GetOrAdd(EnquipmentItems, equipmentItem, () => new ItemInfo(equipmentItem));
		}

		public static ItemInfo Get(EInventoryFamiliarType familiarItem)
		{
			return GetOrAdd(FamilierItems, familiarItem, () => new ItemInfo(familiarItem));
		}

		public static ItemInfo Get(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			return GetOrAdd(OrbItems, GetOrbKey(orbType, orbSlot), () => new ItemInfo(orbType, orbSlot));
		}

		static int GetOrbKey(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			return ((int)orbType * 10) + (int)orbSlot;
		}

		static ItemInfo GetOrAdd<T>(Dictionary<T, ItemInfo> dictionary, T item, Func<ItemInfo> createNew)
		{
			if (dictionary.ContainsKey(item))
				return dictionary[item];

			var newItem = createNew();
			dictionary[item] = newItem;
			return newItem;
		}
		
		public LootType LootType { get; }
		public int ItemId { get; }
		public int ItemSubId { get; }
		Action<Level> PickupAction { get; set; }

		public Enum TreasureLootType => LootType.ToETreasureLootType();

		public EInventoryUseItemType UseItem => (EInventoryUseItemType)ItemId;
		public EInventoryRelicType Relic => (EInventoryRelicType)ItemId;
		public EInventoryEquipmentType Enquipment => (EInventoryEquipmentType)ItemId;
		public EInventoryFamiliarType Familiar => (EInventoryFamiliarType)ItemId;
		public EInventoryOrbType OrbType => (EInventoryOrbType)ItemId;
		public EOrbSlot OrbSlot => (EOrbSlot)ItemSubId;

		public int AnimationIndex => GetAnimationIndex();

		ItemInfo(EInventoryUseItemType useItem)
		{
			LootType = LootType.UseItem;
			ItemId = (int)useItem;
		}

		ItemInfo(EInventoryRelicType relicType)
		{
			LootType = LootType.Relic;
			ItemId = (int)relicType;
		}

		ItemInfo(EInventoryEquipmentType enquipment)
		{
			LootType = LootType.Equipment;
			ItemId = (int)enquipment;
		}

		ItemInfo(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			LootType = LootType.Orb;
			ItemId = (int)orbType;
			ItemSubId = (int)orbSlot;
		}

		ItemInfo(EInventoryFamiliarType familiar)
		{
			LootType = LootType.Familiar;
			ItemId = (int)familiar;
		}

		public void SetPickupAction(Action<Level> onPickUp)
		{
			PickupAction = onPickUp;
		}

		public void OnPickup(Level level)
		{
			PickupAction?.Invoke(level);
		}

		int GetAnimationIndex()
		{
			switch (LootType)
			{
				case LootType.ConstOrb:
					return (int)GetIconFromOrbMethod.InvokeStatic(OrbType, OrbSlot) -1;
				case LootType.ConstEquipment:
					return (int)GetIconFromEnquipmentMethod.InvokeStatic(Enquipment) - 1;
				case LootType.ConstFamiliar:
					return (int)GetIconFromFamilierMethod.InvokeStatic(Familiar) - 1; 
				case LootType.ConstRelic:
					return (int)GetIconFromRelicMethod.InvokeStatic(Relic) - 1; 
				case LootType.ConstStat:
					return -1;
				case LootType.ConstUseItem:
					return (int)GetIconFromUseItemMethod.InvokeStatic(UseItem) - 1; 
				default:
					throw new ArgumentOutOfRangeException($"LootType {LootType} isnt a valid loot type");
			}
		}

		public bool Equals(ItemInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return LootType.Equals(other.LootType) 
			       && ItemId == other.ItemId 
			       && ItemSubId == other.ItemSubId;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ItemInfo) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = LootType.GetHashCode();
				hashCode = (hashCode * 397) ^ ((ItemId * 1000) + ItemSubId);
				return hashCode;
			}
		}

		public override string ToString()
		{
			switch (LootType)
			{
				case LootType.ConstEquipment:
					return Enquipment.ToString();
				case LootType.ConstFamiliar:
					return Familiar.ToString();
				case LootType.ConstOrb:
					return $"{OrbSlot}{OrbType}";
				case LootType.ConstRelic:
					return Relic.ToString();
				case LootType.ConstUseItem:
					return UseItems.ToString();
				default:
					throw new NotImplementedException($"Loottype {LootType}.ToString() isnt implemented");
			}
		}
	}
}