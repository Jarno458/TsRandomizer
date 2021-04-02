using System;
using System.Collections.Generic;

namespace TsRandomizer.Randomisation
{
	class Roomkey : IEquatable<Roomkey>, IEqualityComparer<Roomkey>
	{
		public readonly int LevelId;
		public readonly int RoomId;

		public Roomkey(int levelId, int roomId)
		{
			LevelId = levelId;
			RoomId = roomId;
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((Roomkey)obj);
		}

		public bool Equals(Roomkey other)
		{
			if (other is null) return false;

			return LevelId == other.LevelId
			       && RoomId == other.RoomId;
		}

		public override int GetHashCode() =>
			LevelId + RoomId << 8;

		public override string ToString() =>
			$"[{LevelId}.{RoomId}]";


		public bool Equals(Roomkey x, Roomkey y) =>
			x?.Equals(y) ?? false;

		public int GetHashCode(Roomkey obj) =>
			obj.GetHashCode();
	}
}


