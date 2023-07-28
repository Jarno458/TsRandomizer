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
			if (enemy.GetType().FullName == newEnemyInfo.ClassName && enemy.AsDynamic()._argument == newEnemyInfo.Argument)
				return enemy;

			var newEnemySpec = new ObjectTileSpecification
			{
				Category = EObjectTileCategory.Enemy,
				Layer = ETileLayerType.Objects,
				IsFlippedHorizontally = ShouldFlipHorizontally(enemy, level, newEnemyInfo),
				IsFlippedVertically = newEnemyInfo.IsCeilingEnemy,
				ObjectID = (int)newEnemyInfo.TileType,
				Argument = newEnemyInfo.Argument,
				X = (enemy.Position.X - 8) / 16,
				Y = GetYPoint(enemy, level, newEnemyInfo)
			};

			var newEnemy = level.PlaceEvent(newEnemySpec, true);

			//newEnemy.Scale = 2f;
			//newEnemy.AsDynamic()._doesDrawAggroBbox = true;
			//newEnemy.AsDynamic().DoesDrawBoundingBox = true;

			if (enemy.IsFrozen)
				newEnemy.Freeze();

			enemy.Yeet();

			return newEnemy as Monster;
		}

		static bool ShouldFlipHorizontally(Monster enemy, Level level, EnemyInfo newEnemyInfo)
		{
			switch (newEnemyInfo.Type)
			{
				case EnemyType.Turret:
					return level.MainHero.Position.X > enemy.Position.X;
				case EnemyType.PastLogThrower:
					return false;
				default: 
					return enemy.IsImageFacingLeft;
			}
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
					return 0;

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

		public static void ScaleTo(this Monster newEnemy, Monster enemy) =>
			newEnemy.ScaleTo(enemy.Damage, enemy.MaxHP, enemy.HP, enemy.ExperienceGiven);

		public static void ScaleTo(this Monster newEnemy, int damage, int maxHp, int hp, int xp)
		{
			var dynamicNewEnemy = newEnemy.AsDynamic();

			dynamicNewEnemy._damageCaused = damage;
			newEnemy.MaxHP = maxHp;
			newEnemy.HP = hp;
			dynamicNewEnemy._experienceGiven = xp;

			if (newEnemy.EnemyType == EEnemyTileType.KickstarterFoe &&
			    newEnemy.GetType().FullName == EnemyInfo.Get[EnemyType.Nethershade].ClassName)
			{
				dynamicNewEnemy._baseDamage = damage;
				dynamicNewEnemy.ToggleTimeState(false);
			}
		}
	}
}
