using System;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.GyreZel")]
	class GyreZel : LevelObject<Monster>
	{
		public GyreZel(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			var swordDamage = (int)Math.Ceiling(TypedObject.Damage * 1.14999997615814);
			Dynamic._swordDamage = swordDamage;

			for (int index = 0; index < 5; ++index)
			{
				var sword = (Appendage)Dynamic._swords[index];
				var damageArea = (DamageArea)sword.AsDynamic()._damageArea;

				damageArea.Power = swordDamage;
			}
		}
	}
}

