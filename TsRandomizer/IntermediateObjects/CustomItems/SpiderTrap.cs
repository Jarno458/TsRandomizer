using System;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class SpiderTrap : Trap
	{
		static readonly Type LabSpiderType = TimeSpinnerType.Get("Timespinner.GameAbstractions.GameObjects.LabSpider");

		public SpiderTrap(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.SpiderTrap) {}

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);

			var enemyTile = new ObjectTileSpecification {
				Category = EObjectTileCategory.Enemy,
				Layer = ETileLayerType.Objects,
				ObjectID = (int)EEnemyTileType.FleshSpider,
				Argument = 1
			};

			var lunaisPos = level.MainHero.LastPosition;
			var sprite = level.GCM.SpLabSpider;

			var rightWall = level.FindFirstSolidTileInDirection(new Point(lunaisPos.X, lunaisPos.Y - 16), EDirection.East);
			var rightX = (rightWall == null) ? lunaisPos.X + 112 : Math.Min((rightWall.DictKey.X - 1) * 16, lunaisPos.X + 112);

			Monster enemy = ((Monster) LabSpiderType.CreateInstance(false, new Point(rightX, lunaisPos.Y - 16), level, sprite, -1, enemyTile));

			// scale down to base spider values
			enemy.ScaleTo(16, 20, 20, 4);
			enemy.AsDynamic()._isAggroed = true;
			level.AsDynamic().RequestAddObject(enemy);

			var leftWall = level.FindFirstSolidTileInDirection(new Point(lunaisPos.X, lunaisPos.Y - 16), EDirection.West);
			var leftX = (leftWall == null) ? lunaisPos.X - 112 : Math.Max((leftWall.DictKey.X + 1) * 16, lunaisPos.X - 112);
			
			enemy = ((Monster) LabSpiderType.CreateInstance(false, new Point(leftX, lunaisPos.Y - 16), level, sprite, -1, enemyTile));

			enemy.ScaleTo(16, 20, 20, 4);
			enemy.AsDynamic()._isAggroed = true;
			level.AsDynamic().RequestAddObject(enemy);
		}
	}
}
