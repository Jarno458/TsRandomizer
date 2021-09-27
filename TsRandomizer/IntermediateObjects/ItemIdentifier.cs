using System;
using System.Reflection;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;

namespace TsRandomizer.IntermediateObjects
{
	public class ItemIdentifier : IEquatable<ItemIdentifier>
	{
		public LootType LootType { get; }
		public int ItemId { get; }
		public int SubItemId { get; }

		public EInventoryUseItemType UseItem => (EInventoryUseItemType)ItemId;
		public EInventoryRelicType Relic => (EInventoryRelicType)ItemId;
		public EInventoryEquipmentType Enquipment => (EInventoryEquipmentType)ItemId;
		public EInventoryFamiliarType Familiar => (EInventoryFamiliarType)ItemId;
		public EInventoryOrbType OrbType => (EInventoryOrbType)ItemId;
		public EOrbSlot OrbSlot => (EOrbSlot)SubItemId;
		public EItemType Stat => (EItemType)ItemId;

		static readonly MethodInfo GetIconFromUseItemMethod;
		static readonly MethodInfo GetIconFromOrbMethod;
		static readonly MethodInfo GetIconFromRelicMethod;
		static readonly MethodInfo GetIconFromEnquipmentMethod;
		static readonly MethodInfo GetIconFromFamilierMethod;

		static ItemIdentifier()
		{
			var inventoryItemType = TimeSpinnerType.Get("Timespinner.GameAbstractions.Inventory.InventoryItem");
			GetIconFromUseItemMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryUseItemType));
			GetIconFromOrbMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryOrbType), typeof(EOrbSlot));
			GetIconFromRelicMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryRelicType));
			GetIconFromEnquipmentMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryEquipmentType));
			GetIconFromFamilierMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryFamiliarType));
		}

		public ItemIdentifier(EInventoryUseItemType useItem)
		{
			LootType = LootType.UseItem;
			ItemId = (int)useItem;
		}

		public ItemIdentifier(EInventoryRelicType relicType)
		{
			LootType = LootType.Relic;
			ItemId = (int)relicType;
		}

		public ItemIdentifier(EInventoryEquipmentType enquipment)
		{
			LootType = LootType.Equipment;
			ItemId = (int)enquipment;
		}

		public ItemIdentifier(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			LootType = LootType.Orb;
			ItemId = (int)orbType;
			SubItemId = (int)orbSlot;
		}

		public ItemIdentifier(EInventoryFamiliarType familiar)
		{
			LootType = LootType.Familiar;
			ItemId = (int)familiar;
		}

		public ItemIdentifier(EItemType stat)
		{
			LootType = LootType.Stat;
			ItemId = (int)stat;
		}

		public int GetAnimationIndex()
		{
			switch (LootType)
			{
				case LootType.ConstOrb:
					return (int)GetIconFromOrbMethod.InvokeStatic(OrbType, OrbSlot) - 1;
				case LootType.ConstEquipment:
					return (int)GetIconFromEnquipmentMethod.InvokeStatic(Enquipment) - 1;
				case LootType.ConstFamiliar:
					return (int)GetIconFromFamilierMethod.InvokeStatic(Familiar) - 1;
				case LootType.ConstRelic:
					return (int)GetIconFromRelicMethod.InvokeStatic(Relic) - 1;
				case LootType.ConstStat:
					return (int)GetIconFromStat();
				case LootType.ConstUseItem:
					return (int)GetIconFromUseItemMethod.InvokeStatic(UseItem) - 1;
				default:
					throw new ArgumentOutOfRangeException($"LootType {LootType} isnt a valid loot type");
			}
		}

		int GetIconFromStat()
		{
			switch (Stat)
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

		public BestiaryItemDropSpecification GetBestiaryItemDropSpecification()
		{
			return new BestiaryItemDropSpecification
			{
				Category = (int)LootType.ToEInventoryCategoryType(),
				Item = ItemId
			};
		}

		public bool Equals(ItemIdentifier other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;

			return LootType.Equals(other.LootType) 
			       && ItemId == other.ItemId 
			       && SubItemId == other.SubItemId;
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (GetType() != obj.GetType()) return false;

			return Equals((ItemIdentifier)obj);
		}

		public static bool operator ==(ItemIdentifier a, ItemIdentifier b)
		{
			if (a is null && b is null)
				return true;

			return a?.Equals(b) ?? false;
		}

		public static bool operator !=(ItemIdentifier a, ItemIdentifier b) => !(a == b);

		public override int GetHashCode() => (LootType << 20) + (ItemId << 4) + SubItemId;

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