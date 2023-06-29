using System;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class MeteorSparrowTrap : Trap
	{
		static readonly Type MeteorSparrowType = TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.GyreMeteorSparrow");

		public MeteorSparrowTrap(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.MeteorSparrowTrap) {}

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);

			var enemyTile = new ObjectTileSpecification {
				Category = EObjectTileCategory.Enemy,
				Layer = ETileLayerType.Objects,
				ObjectID = (int)EEnemyTileType.FlyingCheveux
			};

			var lunaisPos = level.MainHero.LastPosition;
			var sprite = level.GCM.SpGyreMeteorSparrow;
			
			var enemy = MeteorSparrowType.CreateInstance(false, new Point(lunaisPos.X + 100, lunaisPos.Y - 50), level, sprite, -1, enemyTile);

			enemy.AsDynamic()._isAggroed = true;
			level.AsDynamic().RequestAddObject(enemy);
			
			enemy = MeteorSparrowType.CreateInstance(false, new Point(lunaisPos.X - 100, lunaisPos.Y - 50), level, sprite, -1, enemyTile);

			enemy.AsDynamic()._isAggroed = true;
			level.AsDynamic().RequestAddObject(enemy);
		}
	}
}