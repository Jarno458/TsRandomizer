namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(3, 28)]
	class PastTimespinnerFreeFallCutScene : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Seed.StartingEra != Era.Past) return;
				RoomTriggerHelper.CreateAndCallCutScene(roomState, "Forest0_Warp");
		}
	}
}
