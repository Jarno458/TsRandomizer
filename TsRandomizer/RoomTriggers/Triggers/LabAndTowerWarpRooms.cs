namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 0)] // Lab
	[RoomTriggerTrigger(12, 0)] // Tower
	class labAndTowerWarpRooms : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			// Power stays on in Lock Key Amadeus
			// 11_LabPower true = power off
			if (roomState.Seed.Options.LockKeyAmadeus)
				roomState.Level.GameSave.SetValue("11_LabPower", false);
		}
	}
}
