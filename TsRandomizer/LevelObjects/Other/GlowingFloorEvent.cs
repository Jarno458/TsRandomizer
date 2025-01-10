using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.L11_Lab.EnvPrefabLabWinderia")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.L11_Lab.EnvPrefabLabVilete")]
	// ReSharper disable once UnusedMember.Global
	class GlowingFloorEvent : LevelObject
	{
		bool teleportEnabled;
		bool teleportTriggered;

		public GlowingFloorEvent(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
			if (Dynamic.Level.ID == 16)
			{
				Dynamic._textPromptOffsetX = -20;
				Dynamic._textPromptText = "Exit the pyramid";

				return;
			}
			else if (Dynamic.Level.ID == 10)
			{
				// Hangar risky warp softlock
				Dynamic._textPromptOffsetX = -20;
				Dynamic._textPromptText = "Return to Lab";

				return;
			}
			else if (Dynamic.Level.ID == 11 && Dynamic.Level.RoomID == 16)
			{
				// Upper Lab risky warp softlock
				Dynamic._textPromptOffsetX = -20;
				Dynamic._textPromptText = "Return to Tower";

				return;
			}
			else if (Dynamic.Level.ID == 7)
			{
				// Cantoran
				Dynamic._textPromptOffsetX = -20;
				var bossName = "Cantoran";
				if (Level.GameSave.GetSettings().BossRando.Value != "Off")
				{
					BossAttributes boss = BestiaryManager.GetReplacedBoss(Level, (int)EBossID.Cantoran);
					bossName = boss.VisibleName;
				}
				Dynamic._textPromptText = $"Fight {bossName}";

				return;
			}
			else if (Dynamic.Level.ID == 11 && Level.GameSave.GetSettings().BossRando.Value != "Off")
			{
				// Emperors
				int bossId = Dynamic.PrefabType.ToString() == "L11_SwitchWinderia" ? (int)EBossID.Prince : (int)EBossID.Vol;

				BossAttributes boss = BestiaryManager.GetReplacedBoss(Level, bossId);

				Dynamic._textPromptOffsetX = -20;
				Dynamic._textPromptText = $"Fight {boss.VisibleName}";
			}
		}

		protected override void OnUpdate()
		{
			Roomkey room = new Roomkey(Dynamic.Level.ID, Dynamic.Level.RoomID);
			if ((room.LevelId != 16 && room.LevelId != 7 && room.LevelId != 11 && room.LevelId != 10) || (room.LevelId == 11 && room.RoomId == 21))
				return;
			if (teleportTriggered)
				return;
			if (Scripts.Count != 0)
			{
				Scripts.Clear();
				teleportEnabled = true;
			}
			// Pyramid Entrance
			int warpLevel = 15;
			int warpRoom = 0;
			var song = Timespinner.GameAbstractions.EBGM.Level15;
			// Cantoran Room
			if (room.LevelId == 7)
			{
				warpLevel = 17;
				warpRoom = 8;
				song = Timespinner.GameAbstractions.EBGM.Boss06;
			}
			// Hangar
			if (room.LevelId == 10)
			{
				warpLevel = 10;
				warpRoom = 2;
				song = Timespinner.GameAbstractions.EBGM.Level10;
			}
			// Dad's Tower
			if (room.LevelId == 11 && room.RoomId == 16)
			{
				warpLevel = 12;
				warpRoom = 0;
				song = Timespinner.GameAbstractions.EBGM.None;
			}
			// Dynamo Works
			if (room.LevelId == 11 && room.RoomId == 2)
			{
				warpLevel = 11;
				warpRoom = 39;
				song = Timespinner.GameAbstractions.EBGM.Level11;
			}

			if (teleportEnabled && Scripts.Count == 0)
			{
				teleportTriggered = true;
				LevelReflected.JukeBox.StopSong();
				LevelReflected.RequestChangeLevel(new LevelChangeRequest
				{
					LevelID = warpLevel,
					RoomID = warpRoom,
					IsUsingWarp = true,
					IsUsingWhiteFadeOut = true,
					FadeInTime = 0.5f,
					FadeOutTime = 0.25f
				}); // Return to Pyramid start
				LevelReflected.JukeBox.PlaySong(song);
			}
		}
	}
}
