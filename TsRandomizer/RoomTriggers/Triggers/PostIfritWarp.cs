using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(14, 25)]
	class PostIfritWarp: RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.SeedOptions.GyreArchives) 
				return;

			roomState.Level.JukeBox.StopSong();

			roomState.Level.RequestChangeLevel(new LevelChangeRequest {
				LevelID = 2,
				RoomID = 51,
				IsUsingWarp = true,
				IsUsingWhiteFadeOut = true,
				FadeInTime = 0.5f,
				FadeOutTime = 0.25f
			}); // Ifrit to Portrait room

			roomState.Level.JukeBox.PlaySong(EBGM.Library);
		}
	}
}
