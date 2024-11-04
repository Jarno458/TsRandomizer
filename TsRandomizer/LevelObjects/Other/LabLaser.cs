using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.L11_Lab.EnvPrefabLabForceField")]
	// ReSharper disable once UnusedMember.Global
	class LabLaser : LevelObject
	{
		public LabLaser(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			if (!seed.Options.LockKeyAmadeus)
				return;

			Level level = (Level)Dynamic._level;
			// bool laser = level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessGenza));
			if (level.GameSave.GetSaveBool("TSRando_IsLabPoweredOff"))
				// TODO: subdivide into the different items
				Dynamic.SilentKill();
		}
	}
}
