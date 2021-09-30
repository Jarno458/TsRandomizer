using System;

namespace TsRandomizer.Randomisation
{
	class ItemKey : Roomkey, IEquatable<ItemKey>
	{
		public static ItemKey TutorialMeleeOrb = new ItemKey(0, 4, 0, 0);
		public static ItemKey TutorialSpellOrb = new ItemKey(0, 4, 264, 192);

		public readonly int X;
		public readonly int Y;

		public ItemKey(int levelId, int roomId, int x, int y) : base (levelId, roomId)
		{
			X = x;
			Y = y;
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((ItemKey)obj);
		}

		public bool Equals(ItemKey other)
		{
			if (other is null) return false;

			return LevelId == other.LevelId
				&& RoomId == other.RoomId
				&& X == other.X
				&& Y == other.Y;
		}

		public override int GetHashCode() =>
			LevelId
				+ RoomId << 8
				+ (X ^ Y) << 16;

		public static bool operator ==(ItemKey a, ItemKey b) => a?.Equals(b) ?? false;
		public static bool operator !=(ItemKey a, ItemKey b) => !(a == b);

		public override string ToString() => $"[{LevelId}.{RoomId}.{X}.{Y}]";
	}
}