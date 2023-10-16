using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Doors.TeleportEvent")]
	// ReSharper disable once UnusedMember.Global
	class DoorTeleportEvent : LevelObject
	{
		public DoorTeleportEvent(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			// Remove all exits during fights in boss rando
			Level level = (Level)Dynamic._level;
			bool isRandomized = level.GameSave.GetSettings().BossRando.Value != "Off";
			if (!isRandomized || !level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

#if !DEBUG
			Dynamic.SilentKill();
#endif
		}
	}
}
