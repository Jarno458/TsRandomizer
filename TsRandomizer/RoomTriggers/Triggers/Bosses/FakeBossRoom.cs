using TsRandomizer.Randomisation;
namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(17, 8)]
	[RoomTriggerTrigger(17, 13)]
	class FakeBossRoom : BossRoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Level.GameSave.GetSaveBool("TSRando_IsBossDead_Sorceress"))
			{
				TimeSpinnerGame.Localizer.OverrideKey("q_har_4_can_0",
					"You must be the one who bested Aelana in combat and convinced her to stop this war.");
			}
			else
			{
				TimeSpinnerGame.Localizer.OverrideKey("q_har_4_can_1",
					"You won't stop Aelana, I'll best you in combat!");
			}
			if (TargetBossId == -1)
				TargetBossId = (int)EBossID.Cantoran;
			base.OnRoomLoad(roomState);
			return;
		}
	}
}
