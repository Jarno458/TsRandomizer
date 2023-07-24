using System;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies._10_Fortress.FortressLargeSoldier")]
	class FortressLargeSoldier : LevelObject<Monster>
	{
		public FortressLargeSoldier(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		//fix for enemizer to scale muschroom cloud with the mushroom tower its damage
		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			var hammerDamage = (int)Math.Ceiling(TypedObject.Damage * 1.25);

			Dynamic._hammerDamage = hammerDamage;

			((DamageArea)Dynamic._hammerDamageArea).Power = hammerDamage;

			for (var index = 0; index < 10; ++index)
			{
				var spikeDamage = (int)Math.Ceiling(TypedObject.Damage * 1.10000002384186);

				var spike = (DamageArea)Dynamic._spikes[index];

				spike.AsDynamic()._spikeDamage = spikeDamage;
				spike.Power = spikeDamage;
			}
		}
	}
}
