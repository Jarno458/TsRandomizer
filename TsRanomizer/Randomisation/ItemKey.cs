using System;

namespace TsRanodmizer.Randomisation
{
	class ItemKey : IEquatable<ItemKey>
	{
		public static ItemKey TutorialMeleeOrb = new ItemKey(0, 4, 0, 0);
		public static ItemKey TutorialSpellOrb = new ItemKey(0, 4, 264, 192);

		public readonly int LevelId;
		public readonly int RoomId;
		public readonly int X;
		public readonly int Y;

		public ItemKey(int levelId, int roomId, int x, int y)
		{
			LevelId = levelId;
			RoomId = roomId;
			X = x;
			Y = y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ItemKey)obj);
		}

		public bool Equals(ItemKey other)
		{
			if (ReferenceEquals(null, other)) return false;

			return LevelId == other.LevelId
						 && RoomId == other.RoomId
						 && X == other.X
						 && Y == other.Y;
		}

		public override int GetHashCode()
		{
			return LevelId
					 + RoomId << 8
					 + (X ^ Y) << 16;
		}

		public override string ToString()
		{
			return $"[{LevelId}.{RoomId}.{X}.{Y}]";
		}
	}
}