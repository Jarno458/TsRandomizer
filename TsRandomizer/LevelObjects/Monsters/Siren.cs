﻿using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.CavesSiren")]
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.CursedSiren")]
	class Siren : LevelObject<Monster>
	{
		public Siren(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		//fix for enemizer to scale muschroom cloud with the mushroom tower its damage
		protected override void Initialize(Seed seed, SettingCollection settings) =>
			((DamageArea)Dynamic._inkDamageArea).Power = TypedObject.Damage;
	}
}
