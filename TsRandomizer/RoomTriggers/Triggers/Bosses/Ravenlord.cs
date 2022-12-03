
namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(14, 4)]
	class Ravenlord : BossRoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{ 
			base.OnRoomLoad(roomState);

			if (roomState.Settings.BossRando.Value)
			{
				RoomTriggerHelper.SpawnMovingPlatform(roomState.Level, 185, 520);
				RoomTriggerHelper.SpawnMovingPlatform(roomState.Level, 615, 520);
				RoomTriggerHelper.SpawnCirclePlatform(roomState.Level, 185, 415, true);
				RoomTriggerHelper.SpawnCirclePlatform(roomState.Level, 615, 415, false);
				RoomTriggerHelper.SpawnMovingPlatform(roomState.Level, 185, 220);
				RoomTriggerHelper.SpawnMovingPlatform(roomState.Level, 615, 220);
			}
		}
	}
}
