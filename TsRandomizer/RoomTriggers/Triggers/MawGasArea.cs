namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(8, 6)]
	[RoomTriggerTrigger(8, 13)]
	[RoomTriggerTrigger(8, 21)]
	[RoomTriggerTrigger(8, 33)]
	class MawGasArea : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Seed.Options.GasMaw)
				RoomTriggerHelper.FillRoomWithGas(roomState.Level);
		}
	}
}
