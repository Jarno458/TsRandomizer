using TsRandomizer.Randomisation;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(14, 6)]
	[RoomTriggerTrigger(14, 8)]
	class PreGyreBosses : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			// Reset boss save flags cleared by Gyre portal
			BestiaryManager.RefreshBossSaveFlags(roomState.Level);
		}
	}
}
