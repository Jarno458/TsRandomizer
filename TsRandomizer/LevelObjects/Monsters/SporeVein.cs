using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.CursedSporeVine")]
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.CavesSporeVine")]
	class SporeVein : LevelObject<Monster>
	{
		public SporeVein(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings) =>
			((DamageArea)Dynamic._vineDamageArea).Power = TypedObject.Damage;
	}
}
