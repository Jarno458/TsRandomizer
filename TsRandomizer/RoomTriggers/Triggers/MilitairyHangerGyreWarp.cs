namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(10, 0)]
	class MilitairyHangerGyreWarp : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			// Spawn warp after ship crashes
			if (!roomState.Seed.Options.GyreArchives || !roomState.Level.GameSave.GetSaveBool("IsPastCleared"))
				return;

			// Military Hangar crash site to Gyre
			RoomTriggerHelper.SpawnGyreWarp(roomState.Level, 340, 180); 
		}
	}
}
