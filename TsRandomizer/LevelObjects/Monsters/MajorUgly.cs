using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.GyreMajorUgly")]
	class MajorUgly : LevelObject<Monster>
	{
		public MajorUgly(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			if (Level.ID != 14 && TypedObject.HP > 1)
				TypedObject.HP /= 2;
		}
	}
}
