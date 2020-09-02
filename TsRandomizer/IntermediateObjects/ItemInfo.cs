using System;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Randomisation;

namespace TsRandomizer.IntermediateObjects
{
	public abstract class ItemInfo : IEquatable<ItemInfo>
	{
		public abstract ItemIdentifier Identifier { get; }

		public abstract Enum TreasureLootType { get; }
		public abstract int AnimationIndex { get; }
		public abstract BestiaryItemDropSpecification BestiaryItemDropSpecification { get; }
		internal abstract Requirement Unlocks { get; }
		public abstract void OnPickup(Level level);

		public bool Equals(ItemInfo other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;

			return Identifier.Equals(other.Identifier);
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((ItemInfo)obj);
		}

		public override int GetHashCode() => Identifier.GetHashCode();

		public static bool operator ==(ItemInfo a, ItemInfo b)
		{
			if (a is null && b is null)
				return true;

			return a?.Equals(b) ?? false;
		}

		public static bool operator !=(ItemInfo a, ItemInfo b) => !(a == b);

		public override string ToString()
		{
			switch (Identifier.LootType)
			{
				case LootType.ConstEquipment:
					return Identifier.Enquipment.ToString();
				case LootType.ConstFamiliar:
					return Identifier.Familiar.ToString();
				case LootType.ConstOrb:
					return $"{Identifier.OrbSlot}{Identifier.OrbType}";
				case LootType.ConstRelic:
					return Identifier.Relic.ToString();
				case LootType.ConstUseItem:
					return Identifier.UseItem.ToString();
				case LootType.ConstStat:
					return Identifier.Stat.ToString();
				default:
					throw new NotImplementedException($"Loottype {Identifier.LootType}.ToString() isnt implemented");
			}
		}
	}
}