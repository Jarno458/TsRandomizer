namespace TsRandomizer.Randomisation
{
	class RoomItemKey : ItemKey
	{
		public RoomItemKey(int levelId, int roomId) : base(levelId, roomId, -1, -1)
		{
		}
	}

	static class RoomItemKeyExtensions
	{
		public static RoomItemKey ToRoomItemKey(this ItemKey key) => new RoomItemKey(key.LevelId, key.RoomId);
	}
}
