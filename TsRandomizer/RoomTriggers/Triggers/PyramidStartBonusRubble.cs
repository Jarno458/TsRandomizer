namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(16, 2)]
	class PyramidStartBonusRubble : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Seed.Options.PyramidStart)
			{
				// Give bonus item to make up for the single starter chest in the Pyramid
				RoomTriggerHelper.SpawnTreasureChest(roomState.Level, false, 2192, 1552);
				return;
			}
		}
	}
}
