using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.LabAdult")]
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.LabChild")]
	// ReSharper disable once UnusedMember.Global
	class LabExperiment: LevelObject<Monster>
	{

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
		}

		public LabExperiment(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
			typedObject.InitializeMob();
		}



		protected override void OnUpdate()
		{
		}
	}
}
