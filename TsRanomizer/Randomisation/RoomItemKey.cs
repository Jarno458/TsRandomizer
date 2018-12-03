namespace TsRanodmizer.Randomisation
{
	class RoomItemKey : ItemKey
	{
		public RoomItemKey(int levelId, int roomId) : base(levelId, roomId, -1, -1)
		{
		}
	}

	static class RoomItemKeyExtensions
	{
		public static RoomItemKey ToRoomItemKey(this ItemKey key)
		{
			return new RoomItemKey(key.LevelId, key.RoomId);
		}
	}
}
