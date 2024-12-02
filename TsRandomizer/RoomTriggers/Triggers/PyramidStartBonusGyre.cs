namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(14, 0)]
	class PyramidStartBonusGyre : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Seed.Options.PyramidStart)
			{
				// Give bonus item to make up for the single starter chest in the Pyramid
				RoomTriggerHelper.SpawnTreasureChest(roomState.Level, false, 240, 192);
			}
		}
	}
}