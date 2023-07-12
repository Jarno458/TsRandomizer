using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.CavesMushroomTower")]
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.CursedMushroomTower")]
	class MushroomTower : LevelObject<Monster>
	{
		public MushroomTower(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		//fix for enemizer to scale muschroom cloud with the mushroom tower its damage
		protected override void Initialize(Seed seed) =>
			((DamageArea)Dynamic._sporeDamageArea).Power = TypedObject.Damage;
	}
}
