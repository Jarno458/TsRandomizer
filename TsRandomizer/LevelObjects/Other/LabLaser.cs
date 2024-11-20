using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
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

			if ((level.RoomID == 1 && level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessExperiment))) ||
					(level.RoomID == 35 && level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessGenza))) ||
					(level.RoomID == 37 && level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessResearch))) ||
					(level.RoomID == 39 && level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessDynamo))))
				Dynamic.SilentKill();
		}
	}
}
