namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(16, 2)]
	class PyramidStartBonusRubble : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (true) //(roomState.Seed.Options.PyramidStart && !roomState.RoomItemLocation.IsPickedUp)
			{
				// Give bonus item to make up for the single starter chest in the Pyramid
				RoomTriggerHelper.SpawnTreasureChest(roomState.Level, false, 2192, 1552);
				return;
			}
		}
	}
}
