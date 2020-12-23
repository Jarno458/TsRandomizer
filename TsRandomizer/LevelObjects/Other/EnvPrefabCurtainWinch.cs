using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.EnvPrefabCurtainWinch")]
	// ReSharper disable once UnusedMember.Global
	class EnvPrefabCurtainWinch : LevelObject
	{
		public EnvPrefabCurtainWinch(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize(SeedOptions options)
		{
			Object._isDrawbridgeUp = !LevelReflected.GetLevelSaveBool("HasWinchBeenUsed") ? false : LevelReflected.GetLevelSaveBool("IsDrawbridgeRaised");
			Object._isRotatingClockwise = Object._isDrawbridgeUp;
		}
	}
}
