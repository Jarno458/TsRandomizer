using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.MessengerNPC")]
	// ReSharper disable once UnusedMember.Global
	class PanoNpc : LevelObject
	{
		bool teleportEnabled = false;
		bool teleportTriggered = false;
		public PanoNpc(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (teleportTriggered)
				return;
			if (Dynamic.IsTalking)
			{
				teleportEnabled = true;
			}
			if (teleportEnabled && Scripts.Count == 0) // Conversation triggered and finished
			{
				LevelReflected.JukeBox.StopSong();
				LevelReflected.RequestChangeLevel(new LevelChangeRequest
				{
					LevelID = 15,
					RoomID = 0,
					IsUsingWarp = true,
					IsUsingWhiteFadeOut = true,
					FadeInTime = 0.5f,
					FadeOutTime = 0.25f
				}); // Return to Pyramid start
				LevelReflected.JukeBox.PlaySong(Timespinner.GameAbstractions.EBGM.Level15);
				teleportTriggered = true;
			}
		}
	}
}
