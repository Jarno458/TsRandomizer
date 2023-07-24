using System;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies._04_Ramparts.CastleLargeSoldier")]
	class CastleLargeSoldier : LevelObject<Monster>
	{
		public CastleLargeSoldier(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		//fix for enemizer to scale muschroom cloud with the mushroom tower its damage
		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			var hammerDamage = (int)Math.Ceiling(TypedObject.Damage * 1.25);

			Dynamic._hammerDamage = hammerDamage;

			((DamageArea)Dynamic._hammerDamageArea).Power = hammerDamage;
		}
	}
}
