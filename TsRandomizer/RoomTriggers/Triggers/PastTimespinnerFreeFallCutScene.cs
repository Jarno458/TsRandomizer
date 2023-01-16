namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(3, 28)]
	class PastTimespinnerFreeFallCutScene : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Seed.Options.Inverted) return;
				RoomTriggerHelper.CreateAndCallCutScene(roomState, "Forest0_Warp");
		}
	}
}
