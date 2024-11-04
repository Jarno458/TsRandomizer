namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 1)] // Experiment 13
	[RoomTriggerTrigger(11, 35)] // Genza
	[RoomTriggerTrigger(11, 37)] // Research Wing
	// 11, 39 also has a laser but is covered as Dynamo Works
	class LabLaserRooms : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			// TODO: don't set flags, only use this for experiment spawning, etc.
		}
	}
}
