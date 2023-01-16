namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(8, 7)]
	class Maw : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			if (roomState.Seed.Options.GasMaw && !roomState.Settings.BossRando.Value)
				RoomTriggerHelper.FillRoomWithGas(roomState.Level);
		}
	}
}
