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

			if ((Level.RoomID == 1 && Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessExperiment))) ||
			    (Level.RoomID == 35 && Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessGenza))) ||
			    (Level.RoomID == 37 && Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessResearch))) ||
			    (Level.RoomID == 39 && Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessDynamo))))
			{
				Dynamic.SilentKill();
			}
		}
	}
}
