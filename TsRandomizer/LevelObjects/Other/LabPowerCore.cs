using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.L11_Lab.EnvPrefabLabPowerCore")]
	// ReSharper disable once UnusedMember.Global
	class LabPowerCore : LevelObject
	{
		public LabPowerCore(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			if (!seed.Options.LockKeyAmadeus)
				return;
			Dynamic.SilentKill();
		}
	}
}
