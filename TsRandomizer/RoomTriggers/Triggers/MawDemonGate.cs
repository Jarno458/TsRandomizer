using TsRandomizer.Randomisation;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(8, 13)]
	class MawDemonGate : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Level.GameSave.GetSaveBool("TSRando_IsBossDead_Maw"))
				return;

			roomState.Level.GameSave.SetValue("TSRando_IsVileteSaved", true);

			if (roomState.Settings.BossRando.Value != "Off" && !roomState.Level.GameSave.GetSaveBool("IsVileteSaved"))
				RoomTriggerHelper.CreateAndCallCutScene(roomState, "CavesPast6_MawBoom");

			BestiaryManager.RefreshBossSaveFlags(roomState.Level);
		}
	}
}
