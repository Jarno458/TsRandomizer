using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(14, 23)]
	class GyreExitWarp : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			roomState.Level.JukeBox.StopSong();

			roomState.Level.RequestChangeLevel(new LevelChangeRequest
			{
				LevelID = 10,
				RoomID = 0,
				IsUsingWarp = false,
				IsUsingWhiteFadeOut = true,
				FadeInTime = 0.5f,
				FadeOutTime = 0.25f
			}); // Military Hangar crash site

			roomState.Level.JukeBox.PlaySong(EBGM.Level10);
		}
	}
}
