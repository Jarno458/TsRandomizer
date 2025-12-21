using TsRandomizer.Archipelago;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(1, 13)]
	[RoomTriggerTrigger(3, 18)]
	[RoomTriggerTrigger(15, 11)]
	class StarterSaveRooms : RoomTrigger
	{
		public override void OnRoomExit(RoomState roomState)
		{
			if (!roomState.Seed.Options.Archipelago)
				return;

			roomState.Level.GameSave.SetValue(ArchipelagoItemLocationMap.HasSavedKey, true);
		}
	}
}
