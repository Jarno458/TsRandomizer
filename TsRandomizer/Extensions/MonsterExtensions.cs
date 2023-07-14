using System;
using Microsoft.Xna.Framework;
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
				IsFlippedVertically = newEnemyInfo.IsCeilingEnemy,
				ObjectID = (int)newEnemyInfo.Type,
				Argument = newEnemyInfo.Argument,
				X = (enemy.Position.X - 8) / 16,
				Y = GetYPoint(enemy, level, newEnemyInfo)
			};

			enemy.Yeet();

			return (Monster)level.PlaceEvent(newEnemySpec, true);
		}

		public static void Yeet(this Monster enemy)
		{
			enemy.SilentKill();

			switch (enemy.EnemyType)
			{
				case EEnemyTileType.FleshSpider:
					var dynamicOldEnemy1 = enemy.AsDynamic();
					if (dynamicOldEnemy1._argument != 0)
						((object)dynamicOldEnemy1._lazer).AsDynamic().OnParentDeath();
					break;
				case EEnemyTileType.KickstarterFoe:
					var dynamicOldEnemy2 = enemy.AsDynamic();
					if (dynamicOldEnemy2._argument == 2)
						((object)dynamicOldEnemy2._scythe).AsDynamic().KillScythe();
					break;
			}
		}

		public static bool IsOnCeiling(this Monster enemy)
		{
			switch (enemy.EnemyType)
			{
				case EEnemyTileType.CeilingStar:
				case EEnemyTileType.ForestPlantBat:
					return true;
				case EEnemyTileType.FleshSpider:
				case EEnemyTileType.LakeAnemone:
				case EEnemyTileType.CursedAnemone:
				case EEnemyTileType.CavesSporeVine:
					return enemy.IsFlippedVertically;
				default:
					return false;
			}
		}

		static int GetYPoint(Monster enemy, Level level, EnemyInfo newEnemyInfo)
		{
			int tileY;

			if (newEnemyInfo.IsCeilingEnemy || enemy.IsOnCeiling())
			{
				var ceiling = level.FindFirstSolidTileInDirection(new Point(enemy.Bbox.Center.X, enemy.Bbox.Bottom), EDirection.North);

				if (ceiling == null)
					return level.RoomSize16.Y - 1;

				tileY = ceiling.DictKey.Y + 1;
			}
			else
			{
				var floor = level.FindFirstSolidTileInDirection(new Point(enemy.Bbox.Center.X, enemy.Bbox.Top), EDirection.South);

				if (floor == null)
					return level.RoomSize16.Y - 1;

				tileY = floor.DictKey.Y - 1;
			}

			var enenyY = enemy.Bbox.Y / 16;

			return newEnemyInfo.IsCeilingEnemy
				? tileY
				: enemy.IsOnCeiling()
					? Math.Max(tileY, enenyY) 
					: Math.Min(tileY, enenyY);
		}

		public static void ScaleTo(this Monster newEnemy, Monster enemy)
		{
			var dynamicNewEnemy = newEnemy.AsDynamic();

			dynamicNewEnemy._damageCaused = enemy.Damage;
			newEnemy.MaxHP = enemy.MaxHP;
			newEnemy.HP = enemy.HP;
			dynamicNewEnemy._experienceGiven = enemy.ExperienceGiven;
		}
	}
}
