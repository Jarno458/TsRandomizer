using System;
using System.Collections.Generic;
using System.Reflection;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;

namespace TsRanodmizer.IntermediateObjects
{
	class ItemInfo : IEquatable<ItemInfo>
	{
		static readonly MethodInfo GetIconFromUseItemMethod;
		static readonly MethodInfo GetIconFromOrbMethod;
		static readonly MethodInfo GetIconFromRelicMethod;
		static readonly MethodInfo GetIconFromEnquipmentMethod;
		static readonly MethodInfo GetIconFromFamilierMethod;

		static readonly Dictionary<EInventoryUseItemType, ItemInfo> UseItems;
		static readonly Dictionary<EInventoryRelicType, ItemInfo> RelicItems;
		static readonly Dictionary<EInventoryEquipmentType, ItemInfo> EnquipmentItems;
		static readonly Dictionary<EInventoryFamiliarType, ItemInfo> FamilierItems;
		static readonly Dictionary<int, ItemInfo> OrbItems;
		static readonly Dictionary<EItemType, ItemInfo> StatItems;

		public static ItemInfo Dummy;
		
		static ItemInfo()
		{
			var inventoryItemType = TimeSpinnerType.Get("Timespinner.GameAbstractions.Inventory.InventoryItem");
			GetIconFromUseItemMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryUseItemType));
			GetIconFromOrbMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryOrbType), typeof(EOrbSlot));
			GetIconFromRelicMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryRelicType));
			GetIconFromEnquipmentMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryEquipmentType));
			GetIconFromFamilierMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryFamiliarType));

			UseItems = new Dictionary<EInventoryUseItemType, ItemInfo>();
			RelicItems = new Dictionary<EInventoryRelicType, ItemInfo>();
			EnquipmentItems = new Dictionary<EInventoryEquipmentType, ItemInfo>();
			FamilierItems = new Dictionary<EInventoryFamiliarType, ItemInfo>();
			OrbItems = new Dictionary<int, ItemInfo>();
			StatItems = new Dictionary<EItemType, ItemInfo>();

			Dummy = new ItemInfo(EInventoryEquipmentType.DemonHorn);
		}

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

		public static ItemInfo Get(EItemType stat)
		{
			return GetOrAdd(StatItems, stat, () => new ItemInfo(stat));
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

		public EInventoryUseItemType UseItem => (EInventoryUseItemType)ItemId;
		public EInventoryRelicType Relic => (EInventoryRelicType)ItemId;
		public EInventoryEquipmentType Enquipment => (EInventoryEquipmentType)ItemId;
		public EInventoryFamiliarType Familiar => (EInventoryFamiliarType)ItemId;
		public EInventoryOrbType OrbType => (EInventoryOrbType)ItemId;
		public EOrbSlot OrbSlot => (EOrbSlot)ItemSubId;
		public EItemType Stat => (EItemType)ItemId;

		public Enum TreasureLootType => LootType.ToETreasureLootType();
		public int AnimationIndex => GetAnimationIndex();
		public BestiaryItemDropSpecification BestiaryItemDropSpecification => GetBestiaryItemDropSpecification();

		Action<Level> PickupAction { get; set; }

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

		ItemInfo(EItemType stat)
		{
			LootType = LootType.Stat;
			ItemId = (int)stat;
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
					return (int)GetIconFromStat(Stat);
				case LootType.ConstUseItem:
					return (int)GetIconFromUseItemMethod.InvokeStatic(UseItem) - 1; 
				default:
					throw new ArgumentOutOfRangeException($"LootType {LootType} isnt a valid loot type");
			}
		}

		object GetIconFromStat(EItemType itemType)
		{
			switch (itemType)
			{
				case EItemType.MaxHP:
					return 24;
				case EItemType.MaxAura:
					return 25;
				case EItemType.MaxSand:
					return 26;
				default:
					throw new ArgumentOutOfRangeException($"Stat {Stat} isnt a valid stat boost type");
			}
		}

		BestiaryItemDropSpecification GetBestiaryItemDropSpecification()
		{
			return new BestiaryItemDropSpecification
			{
				Category = (int)LootType.ToEInventoryCategoryType(),
				Item = ItemId
			};
		}

		public bool Equals(ItemInfo other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return LootType.Equals(other.LootType) 
			       && ItemId.Equals(other.ItemId) 
			       && ItemSubId.Equals(other.ItemSubId);
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((ItemInfo)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = LootType.GetHashCode();
				hashCode = (hashCode * 397) ^ ((ItemId << 4) + ItemSubId);
				return hashCode;
			}
		}

		public static bool operator ==(ItemInfo a, ItemInfo b)
		{
			if (a is null && b is null)
				return true;

			return a?.Equals(b) ?? false;
		}

		public static bool operator !=(ItemInfo a, ItemInfo b)
		{
			return !(a == b);
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
					return UseItem.ToString();
				case LootType.ConstStat:
					return Stat.ToString();
				default:
					throw new NotImplementedException($"Loottype {LootType}.ToString() isnt implemented");
			}
		}
	}
}