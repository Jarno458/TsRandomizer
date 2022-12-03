using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(14, 24)]
	class PostRavenlordWarp : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.SeedOptions.GyreArchives)
				return;

			roomState.Level.JukeBox.StopSong();

			roomState.Level.RequestChangeLevel(new LevelChangeRequest
			{
				LevelID = 11,
				RoomID = 4,
				IsUsingWarp = true,
				IsUsingWhiteFadeOut = true,
				FadeInTime = 0.5f,
				FadeOutTime = 0.25f
			}); // Ravenlord to Historical Documents room

			roomState.Level.JukeBox.PlaySong(EBGM.Level11);
		}
	}
}
