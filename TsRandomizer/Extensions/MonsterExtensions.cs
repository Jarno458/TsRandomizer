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
				Argument = newEnemyInfo.Argument,
				X = (enemy.Position.X - 8) / 16,
				Y = (enemy.Position.Y - 16) / 16
			};

			enemy.SilentKill();

			return (Monster)level.PlaceEvent(newEnemySpec, true);
		}
	}
}
