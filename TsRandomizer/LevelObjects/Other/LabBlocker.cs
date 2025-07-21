using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;


namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.L11_Lab.EnvPrefabLabPlayerBlocker")]
	// ReSharper disable once UnusedMember.Global
	class LabBlocker : LevelObject
	{
		public LabBlocker(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			if (!seed.Options.LockKeyAmadeus && Level.GameSave.GetSaveBool("11_LabPower"))
				Dynamic.SilentKill();
		}
	}
}
