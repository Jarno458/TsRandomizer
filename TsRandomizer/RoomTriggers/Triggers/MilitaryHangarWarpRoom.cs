using TsRandomizer.Randomisation;
namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(10, 12)]
	class MilitaryHangarWarpRoom : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			// Trigger reload of the laser gates
			BestiaryManager.RefreshBossSaveFlags(roomState.Level);
		}
	}
}
