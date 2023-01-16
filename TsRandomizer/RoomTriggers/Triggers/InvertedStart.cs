using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(1, 0)]
	class InvertedStart : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Seed.Options.Inverted || roomState.Level.GameSave.GetSaveBool("TSRandomizerHasTeleportedPlayer")) 
				return;

			roomState.Level.GameSave.SetValue("TSRandomizerHasTeleportedPlayer", true);
			roomState.Level.GameSave.SetValue("HasUsedCityTS", true);
			roomState.Level.GameSave.SetCutsceneTriggered("City1_Frame", true);
			roomState.Level.GameSave.SetCutsceneTriggered("City2_Spindle", true);
			roomState.Level.GameSave.SetCutsceneTriggered("City3_Warp", true);
			roomState.Level.GameSave.SetCutsceneTriggered("LakeDesolation1_Entrance", true); // Fixes music when returning to Lake Desolation later

			roomState.Level.RequestChangeLevel(new LevelChangeRequest { LevelID = 3, RoomID = 28 }); // Waterfall cutscene
		}
	}
}
