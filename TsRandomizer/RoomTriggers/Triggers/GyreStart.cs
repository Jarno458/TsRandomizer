using Timespinner.GameAbstractions;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(14, 11)]
	class GyreStart : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			// Play Gyre music in gyre
			roomState.Level.JukeBox.PlaySong(EBGM.Level14);

			// Set Gyre enemies
			roomState.Level.AsDynamic().SetLevelSaveInt("GyreDungeonSeed", (int)roomState.Level.GameSave.GetSeed().Value.Id);

			// Reset boss save flags cleared by Gyre portal
			BestiaryManager.RefreshBossSaveFlags(roomState.Level); 

			 // Set last warp room visited to military hangar warp (instead of crash site)
			 roomState.Level.GameSave.LastWarpLevel = 10;
			 roomState.Level.GameSave.LastWarpRoom = 12;
		}
	}
}
