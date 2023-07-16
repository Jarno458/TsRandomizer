using System;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.TowerRoyalGuard")]
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.EmpRoyalGuard")]
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.KeepDemon")]
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.EmpDemon")]
	class Demon : LevelObject<Monster>
	{
		public Demon(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings) =>
			((DamageArea)Dynamic._projectile).Power = (int)Math.Ceiling(TypedObject.Damage * 1.25);
	}
}
