using System;
using Microsoft.Xna.Framework;
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
		bool doRainbow;
		float rainbow;

		public LabLaser(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			// Lower Trash
			if (Level.RoomID == 3)
				Dynamic.IsTriggerableByMonsters = false;

			if (Level.RoomID > 30)
				doRainbow = seed.Id % Level.RoomID < 3;
			else
				doRainbow = seed.Id % 36 < 3;

			if (!seed.Options.LockKeyAmadeus)
				return;

			if ((Level.RoomID == 1 && Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessExperiment))) ||
			    (Level.RoomID == 35 && Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessGenza))) ||
			    (Level.RoomID == 37 && Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessResearch))) ||
			    (Level.RoomID == 39 && Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessDynamo))))
			{
				Dynamic.SilentKill();

				doRainbow = false;
			}
		}

		protected override void OnUpdate()
		{
			if (!doRainbow)
				return;

			rainbow += 0.01f;

			Dynamic.AuraColor = Rainbow(rainbow);
		}

		public static Color Rainbow(float progress)
		{
			float div = Math.Abs(progress % 1) * 6;
			int ascending = (int)((div % 1) * 255);
			int descending = 255 - ascending;

			switch ((int)div)
			{
				case 0:
					return new Color(255, ascending, 0, 200);
				case 1:
					return new Color(descending, 255, 0, 200);
				case 2:
					return new Color(0, 255, ascending, 200);
				case 3:
					return new Color(0, descending, 255, 200);
				case 4:
					return new Color(ascending, 0, 255, 200);
				default: // case 5:
					return new Color(255, 0, descending, 200);
			}
		}
	}
}
