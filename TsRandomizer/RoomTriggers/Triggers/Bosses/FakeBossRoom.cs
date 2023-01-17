namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(17, 8)]
	[RoomTriggerTrigger(17, 13)]
	class FakeBossRoom : BossRoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState) =>
			SpawnBoss(roomState, TargetBossId);
	}
}
