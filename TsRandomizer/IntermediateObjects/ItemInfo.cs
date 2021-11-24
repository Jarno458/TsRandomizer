using System;
using System.Collections.Generic;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Randomisation;

namespace TsRandomizer.IntermediateObjects
{
	public abstract class ItemInfo : IEquatable<ItemInfo>
	{
		private List<string> AlwaysRequiredItems = new List<string>
		{
			"TimespinnerWheel",
			"TimespinnerSpindle",
			"TimespinnerGear1",
			"TimespinnerGear2",
			"TimespinnerGear3"
		};
        public abstract ItemIdentifier Identifier { get; }
		public abstract Enum TreasureLootType { get; }
		public abstract int AnimationIndex { get; }
		public abstract BestiaryItemDropSpecification BestiaryItemDropSpecification { get; }
		internal abstract Requirement Unlocks { get; }
		public abstract void OnPickup(Level level);

		public bool IsProgression => Unlocks != Requirement.None;

		public bool IsExplicitlyRequired => AlwaysRequiredItems.Contains(Identifier.ToString());

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

		public override string ToString() => Identifier.ToString();
	}
}