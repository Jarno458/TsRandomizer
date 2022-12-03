namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(16, 1)]
	class PostNightmareChest : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			// Allow the post-Nightmare chest to spawn
			roomState.Level.GameSave.SetValue("IsGameCleared", true);
			roomState.Level.GameSave.SetValue("IsEndingCDCleared", true);
		}
	}
}
