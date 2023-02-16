using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.GyreMajorUgly")]
	class MajorUgly : LevelObject<Monster>
	{
		public MajorUgly(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed)
		{
			if (Level.ID != 7)
				return;

			TypedObject.HP /= 2;
		}  
	}
}
