using System;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.LabTurret")]
	class LabTurret : LevelObject<Monster>
	{
		public LabTurret(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		//fix for enemizer to scale muschroom cloud with the mushroom tower its damage
		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			var boltDamage = (int)Math.Ceiling(TypedObject.Damage * 1.5);

			for (var index = 0; index < 3; ++index)
			{
				var bolt = (Projectile)Dynamic._bolts[index];

				bolt.AsDynamic()._basePower = boltDamage;
				bolt.Power = boltDamage;
			}
		}
	}
}
