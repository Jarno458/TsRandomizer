using System;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;

namespace TsRandomizer.IntermediateObjects
{
	public abstract class ItemInfo : IEquatable<ItemInfo>
	{
		public abstract LootType LootType { get; }
		public abstract int ItemId { get; }
		public abstract EInventoryUseItemType UseItem { get; }
		public abstract EInventoryRelicType Relic { get; }
		public abstract EInventoryEquipmentType Enquipment { get; }
		public abstract EInventoryFamiliarType Familiar { get; }
		public abstract EInventoryOrbType OrbType { get; }
		public abstract EOrbSlot OrbSlot { get; }
		public abstract EItemType Stat { get; }
		public abstract Enum TreasureLootType { get; }
		public abstract int AnimationIndex { get; }
		public abstract BestiaryItemDropSpecification BestiaryItemDropSpecification { get; }

		public abstract void SetPickupAction(Action<Level> onPickUp);
		public abstract void OnPickup(Level level);

		public bool Equals(ItemInfo other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return LootType.Equals(other.LootType) 
			       && ItemId.Equals(other.ItemId) 
			       && OrbSlot.Equals(other.OrbSlot);
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
			return (LootType << 20) + (ItemId << 4) + (int)OrbSlot;
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