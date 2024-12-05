using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Doors.BlastDoorEvent")]
	// ReSharper disable once UnusedMember.Global
	class BlastDoorEvent : LevelObject
	{
		public BlastDoorEvent(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			if (seed.Options.LockKeyAmadeus)
				Dynamic.CloseAndLock();
		}
	}
}
