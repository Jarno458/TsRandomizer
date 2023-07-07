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
			var newEnemySpec = new ObjectTileSpecification
			{
				Category = EObjectTileCategory.Enemy,
				Layer = ETileLayerType.Objects,
				IsFlippedHorizontally = enemy.IsImageFacingLeft,
				IsFlippedVertically = enemy.IsFlippedVertically,
				ObjectID = (int)newEnemyInfo.Type,
				Argument = (int)newEnemyInfo.Argument
			};

			var pos = enemy.Position;

			enemy.SilentKill();

			dynamic newEnemy;
			if (newEnemyInfo.Type == EEnemyTileType.LakeBirdEgg)
				newEnemy = newEnemyInfo.Class.CreateInstance(
					false, pos, level, newEnemyInfo.SpriteSheet(level.GCM), -1, newEnemySpec, false);
			else
				newEnemy = newEnemyInfo.Class.CreateInstance(
					false, pos, level, newEnemyInfo.SpriteSheet(level.GCM), -1, newEnemySpec);

			newEnemy.InitializeMob();

			return newEnemy;
		}
	}
}
