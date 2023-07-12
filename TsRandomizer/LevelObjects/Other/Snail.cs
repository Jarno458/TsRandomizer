using System;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.CavesSnail")]
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.CursedSnail")]
	class Snail : LevelObject<Monster>
	{
		public Snail(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		//fix for enemizer to scale muschroom cloud with the mushroom tower its damage
		protected override void Initialize(Seed seed) =>
			((DamageArea)Dynamic._spitDamageArea).Power = (int)Math.Ceiling(TypedObject.Damage * 0.899999976158142);
	}
}
