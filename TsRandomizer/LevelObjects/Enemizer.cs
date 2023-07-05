using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.Core;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects
{
	class Enemizer
	{
		static readonly Type BossType = TimeSpinnerType.Get("Timespinner.GameObjects.BaseClasses.BossClass");

		static readonly EnemyInfo[] Enemies = {
			new EnemyInfo(EEnemyTileType.CheveuxTank),
			new EnemyInfo(EEnemyTileType.RedCheveux, s => s.SpCheveuxTank),
			new EnemyInfo(EEnemyTileType.FlyingCheveux, s => s.SpCheveuxFlying),
			new EnemyInfo(EEnemyTileType.KickstarterFoe, 0, "GyreMajorUgly"),
			new EnemyInfo(EEnemyTileType.KickstarterFoe, 1, "GyreMeteorSparrow"),
			new EnemyInfo(EEnemyTileType.KickstarterFoe, 2, "GyreKain"),
			new EnemyInfo(EEnemyTileType.KickstarterFoe, 3, "GyreNethershade"),
			new EnemyInfo(EEnemyTileType.KickstarterFoe, 4, "GyreRyshia"),
			new EnemyInfo(EEnemyTileType.KickstarterFoe, 5, "GyreZel"),
			new EnemyInfo(EEnemyTileType.TempleFoe, 0, "Timespinner.GameObjects.Enemies._16_Temple", "TempleConviction"),
			new EnemyInfo(EEnemyTileType.TempleFoe, 1, "Timespinner.GameObjects.Enemies._16_Temple", "TempleZeal", s => s.SpTempleConviction),
			new EnemyInfo(EEnemyTileType.TempleFoe, 2, "Timespinner.GameObjects.Enemies._16_Temple", "TempleJustice", s => s.SpTempleConviction),
			new EnemyInfo(EEnemyTileType.TempleFoe, 3, "Timespinner.GameObjects.Enemies._16_Temple", "TemplePride", s => s.SpTempleConviction),
			new EnemyInfo(EEnemyTileType.CavesSlime, 0, "CavesSlime"),
			new EnemyInfo(EEnemyTileType.CavesSlime, 1, "CursedSlime", s => s.SpCavesSlime),
			new EnemyInfo(EEnemyTileType.FortressEngineer),
			new EnemyInfo(EEnemyTileType.CavesCopperWyvern, 0, "CavesCopperWyvern", s => s.SpCopperWyvern),
			new EnemyInfo(EEnemyTileType.CavesCopperWyvern, 1, "CursedCopperWyvern"),
			new EnemyInfo(EEnemyTileType.CavesSiren, 0, "CavesSiren", s => s.SpSiren),
			new EnemyInfo(EEnemyTileType.CavesSiren, 1, "CursedSiren"),
			new EnemyInfo(EEnemyTileType.KeepDemon, 0, "KeepDemon"),
			new EnemyInfo(EEnemyTileType.KeepDemon, 1, "EmpDemon"),
			new EnemyInfo(EEnemyTileType.CastleShieldKnight, 0, "CastleShieldKnight"),
			new EnemyInfo(EEnemyTileType.CastleShieldKnight, 1, "ViletianLancer", s => s.SpCastleShieldKnight), //Might not be an actual enemy
			new EnemyInfo(EEnemyTileType.CastleArcher),
			//new EnemyInfo(EEnemyTileType.WormFlower), // Crashes
			new EnemyInfo(EEnemyTileType.WormFlowerWalker, s => s.SpWormFlower),
			new EnemyInfo(EEnemyTileType.CeilingStar),
			new EnemyInfo(EEnemyTileType.FleshSpider, 0, "Timespinner.GameAbstractions.GameObjects", "FleshSpider"),
			new EnemyInfo(EEnemyTileType.FleshSpider, 1, "Timespinner.GameAbstractions.GameObjects", "LabSpider"),
			new EnemyInfo(EEnemyTileType.DiscStatue, "Timespinner.GameAbstractions.GameObjects"), //WTF
			new EnemyInfo(EEnemyTileType.CitySecurityGuard),
			new EnemyInfo(EEnemyTileType.ForestBabyCheveux),
			new EnemyInfo(EEnemyTileType.ForestMoth, 0, "ForestMoth"),
			new EnemyInfo(EEnemyTileType.ForestMoth, 1, "CursedMoth"),
			new EnemyInfo(EEnemyTileType.ForestPlantBat),
			new EnemyInfo(EEnemyTileType.ForestRodent),
			new EnemyInfo(EEnemyTileType.ForestWormFlower),
			new EnemyInfo(EEnemyTileType.CavesMushroomTower, 0, "CavesMushroomTower"),
			new EnemyInfo(EEnemyTileType.CavesMushroomTower, 1, "CursedMushroomTower"),
			new EnemyInfo(EEnemyTileType.CavesSporeVine, 0, "CavesSporeVine"),
			new EnemyInfo(EEnemyTileType.CavesSporeVine, 1, "CursedSporeVine"),
			new EnemyInfo(EEnemyTileType.CavesSnail, 0, "CavesSnail"),
			new EnemyInfo(EEnemyTileType.CavesSnail, 1, "CursedSnail"),
			new EnemyInfo(EEnemyTileType.CastleLargeSoldier, "Timespinner.GameObjects.Enemies._04_Ramparts"),
			new EnemyInfo(EEnemyTileType.CastleEngineer),
			new EnemyInfo(EEnemyTileType.KeepWarCheveux),
			new EnemyInfo(EEnemyTileType.KeepAristocrat, 0, "KeepAristocrat"),
			new EnemyInfo(EEnemyTileType.KeepAristocrat, 1, "TowerIceMage"),
			new EnemyInfo(EEnemyTileType.KeepAristocrat, 2, "EmpAristocrat"),
			new EnemyInfo(EEnemyTileType.TowerPlasmaPod),
			new EnemyInfo(EEnemyTileType.TowerRoyalGuard, 0, "TowerRoyalGuard", s => s.SpTowerDemonMage),
			new EnemyInfo(EEnemyTileType.TowerRoyalGuard, 1, "EmpRoyalGuard"),
			new EnemyInfo(EEnemyTileType.LakeBirdEgg, "Timespinner.GameObjects"),
			new EnemyInfo(EEnemyTileType.LakeCheveux),
			new EnemyInfo(EEnemyTileType.LakeFly),
			new EnemyInfo(EEnemyTileType.FortressKnight),
			new EnemyInfo(EEnemyTileType.FortressGunner),
			new EnemyInfo(EEnemyTileType.LabTurret),
			new EnemyInfo(EEnemyTileType.LabChild),
			new EnemyInfo(EEnemyTileType.LabAdult),
			new EnemyInfo(EEnemyTileType.FortressLargeSoldier, "Timespinner.GameObjects.Enemies._10_Fortress"),
			new EnemyInfo(EEnemyTileType.ForestBabyCheveux),
			new EnemyInfo(EEnemyTileType.ForestBabyCheveux),
		};

		static readonly EnemyInfo[] UnderwaterEnemies = Enemies.Concat(new [] {
			new EnemyInfo(EEnemyTileType.LakeEel),
			new EnemyInfo(EEnemyTileType.LakeAnemone),
			new EnemyInfo(EEnemyTileType.CursedAnemone, s => s.SpCursedAnemone), //Flies to top of screen
		}).ToArray();

		/*TODO
		 TempleFoe is wierd
		Plantbat breaks on floor
		Snail missing face
		 
		 
		 
		 
		 
		 */

		public static void RandomizeEnemies(
			Level level, dynamic levelReflected, int levelId, int roomId, IEnumerable<Monster> enemies, Seed seed)
		{
			if (levelId == 5 && roomId == 1)
				return;

			var random = new Random((int)(seed.Id + (levelId * 100) + roomId));

			foreach (var enemy in enemies)
			{
				if (enemy.EnemyType == EEnemyTileType.JunkSpawner || enemy.GetType().IsSubclassOf(BossType))
					continue;

				var pos = enemy.Position;
				enemy.SilentKill();

				var newEnemyInfo = (enemy.IsInWater)
					? UnderwaterEnemies.SelectRandom(random)
					: Enemies.SelectRandom(random);

				ScreenManager.Console.AddDebugLine($"[LVL:{levelId},ROOM:{roomId}] Replacing {enemy.EnemyType} with {newEnemyInfo.ClassName}");

				var newEnemySpec = new ObjectTileSpecification
				{
					Category = EObjectTileCategory.Enemy,
					Layer = ETileLayerType.Objects,
					IsFlippedHorizontally = enemy.IsImageFacingLeft,
					IsFlippedVertically = enemy.IsFlippedVertically,
					ObjectID = (int)newEnemyInfo.Type
				};

				Monster newEnemy;
				if (newEnemyInfo.Type == EEnemyTileType.LakeBirdEgg)
					newEnemy = (Monster)newEnemyInfo.Class.CreateInstance(
						false, pos, level, newEnemyInfo.SpriteSheet(level.GCM), -1, newEnemySpec, false);
				else
					newEnemy = (Monster)newEnemyInfo.Class.CreateInstance(
						false, pos, level, newEnemyInfo.SpriteSheet(level.GCM), -1, newEnemySpec);

				levelReflected.RequestAddObject(newEnemy);
			}
		}
	}

	class EnemyInfo
	{
		public readonly EEnemyTileType Type;
		public readonly int? Argument;
		public readonly string ClassName;
		public readonly Type Class;
		public readonly Func<GCM, SpriteSheet> SpriteSheet;

		public EnemyInfo(EEnemyTileType type, Func<GCM, SpriteSheet> spriteSheet = null) : this(type, 0, type.ToString(), spriteSheet)
		{
		}

		public EnemyInfo(EEnemyTileType type, string classPath, Func<GCM, SpriteSheet> spriteSheet = null) 
			: this(type, 0, classPath, type.ToString(), spriteSheet)
		{
		}

		public EnemyInfo(EEnemyTileType type, int argument, string className, Func<GCM, SpriteSheet> spriteSheet = null)
			: this(type, argument, "Timespinner.GameObjects.Enemies", className, spriteSheet)
		{
		}

		public EnemyInfo(EEnemyTileType type, int argument, string classPath, string className, Func<GCM, SpriteSheet> spriteSheet = null)
		{
			Type = type;
			Argument = argument;
			ClassName = className;

			if (spriteSheet != null)
				SpriteSheet = spriteSheet;
			else
				SpriteSheet = gcm => (SpriteSheet)typeof(GCM).GetField("Sp" + className).GetValue(gcm);

			Class = TimeSpinnerType.Get($"{classPath}.{className}");
		}
	}
}
