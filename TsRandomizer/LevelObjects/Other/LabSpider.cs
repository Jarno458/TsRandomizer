using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameAbstractions.GameObjects.LabSpider")]
	class LabSpider : LevelObject<Monster>
	{
		public LabSpider(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings) =>
			((GameEvent)Dynamic._lazer).AsDynamic()._baseDamage = TypedObject.Damage;
	}
}
