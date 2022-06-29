using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Doors.TeleportEvent")]
	// ReSharper disable once UnusedMember.Global
	class DoorTeleportEvent : LevelObject
	{
		public DoorTeleportEvent(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize(SeedOptions options)
		{
			// Remove all exits during fights in boss rando
			Level level = (Level)Dynamic._level;
			bool isRandomized = level.GameSave.GetSettings().BossRando.Value;
			if (!isRandomized || !level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			Dynamic.SilentKill();
		}
	}
}
