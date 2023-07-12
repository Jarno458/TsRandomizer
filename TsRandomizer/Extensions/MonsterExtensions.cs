using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.LevelObjects;

namespace TsRandomizer.Extensions
{
	static class MonsterExtensions
	{
		public static Monster ReplaceWith(this Monster enemy, Level level, EnemyInfo newEnemyInfo)
		{
			if (enemy.GetType().FullName == newEnemyInfo.ClassName)
				return enemy;

			var newEnemySpec = new ObjectTileSpecification
			{
				Category = EObjectTileCategory.Enemy,
				Layer = ETileLayerType.Objects,
				IsFlippedHorizontally = enemy.IsImageFacingLeft,
				IsFlippedVertically = enemy.IsFlippedVertically,
				ObjectID = (int)newEnemyInfo.Type,
				Argument = newEnemyInfo.Argument,
				X = (enemy.Position.X - 8) / 16,
				Y = GetYPoint(enemy, level, newEnemyInfo)
			};

			if (newEnemyInfo.IsCeilingEnemy)
			{
				var ceiling = level.FindFirstSolidTileInDirection(enemy.Bbox.Center, EDirection.North);

				if (ceiling == null)
					newEnemySpec.Y = 0;
			}

			enemy.SilentKill();

			if (enemy.EnemyType == EEnemyTileType.FleshSpider)
			{ 
				var dynamicOldEnemy = enemy.AsDynamic();
				if (dynamicOldEnemy._argument != 0)
					((object)dynamicOldEnemy._lazer).AsDynamic().OnParentDeath();
			}

			return (Monster)level.PlaceEvent(newEnemySpec, true);
		}

		static int GetYPoint(Monster enemy, Level level, EnemyInfo newEnemyInfo)
		{
			if (!newEnemyInfo.IsCeilingEnemy)
				return (enemy.Position.Y - 16) / 16;

			var ceiling = level.FindFirstSolidTileInDirection(enemy.Bbox.Center, EDirection.North);

			if (ceiling == null)
				return 0;

			return ceiling.Bbox.Bottom / 16;
		}
	}
}
