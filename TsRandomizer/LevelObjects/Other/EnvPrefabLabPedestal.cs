using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.EnvPrefabLabPedestal")]
	class EnvPrefabLabPedestal : LevelObject
	{
		public EnvPrefabLabPedestal(Mobile typedObject) : base(typedObject)
		{
			typedObject.SilentKill();
		}
	}
}
