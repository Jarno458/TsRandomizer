using System;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;

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
	}
}