using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
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

		static readonly EnemyInfo[] LargeGroundedEnemies = {
			new EnemyInfo(EEnemyTileType.CheveuxTank), //actually jumping Cheveux
			new EnemyInfo(EEnemyTileType.KickstarterFoe, 0, "GyreMajorUgly"),
			new EnemyInfo(EEnemyTileType.WormFlower),
			new EnemyInfo(EEnemyTileType.WormFlowerWalker, s => s.SpWormFlower),
			new EnemyInfo(EEnemyTileType.DiscStatue, "Timespinner.GameAbstractions.GameObjects"), //Cat on roomba
			new EnemyInfo(EEnemyTileType.ForestBabyCheveux),
			new EnemyInfo(EEnemyTileType.ForestWormFlower),
			new EnemyInfo(EEnemyTileType.CavesMushroomTower, 0, "CavesMushroomTower"),
			new EnemyInfo(EEnemyTileType.CavesMushroomTower, 1, "CursedMushroomTower"),
			new EnemyInfo(EEnemyTileType.CastleLargeSoldier, "Timespinner.GameObjects.Enemies._04_Ramparts"),
			new EnemyInfo(EEnemyTileType.KeepWarCheveux),
			new EnemyInfo(EEnemyTileType.KeepAristocrat, 1, "TowerIceMage"),
			new EnemyInfo(EEnemyTileType.LakeCheveux),
			new EnemyInfo(EEnemyTileType.FortressLargeSoldier, "Timespinner.GameObjects.Enemies._10_Fortress"),
		};

		static readonly EnemyInfo[] NormalGroundedEnemies = {
			new EnemyInfo(EEnemyTileType.RedCheveux, s => s.SpCheveuxTank),
			new EnemyInfo(EEnemyTileType.FortressEngineer),
			new EnemyInfo(EEnemyTileType.CavesSiren, 0, "CavesSiren", s => s.SpSiren),
			new EnemyInfo(EEnemyTileType.CavesSiren, 1, "CursedSiren"),
			new EnemyInfo(EEnemyTileType.CastleShieldKnight, 0, "CastleShieldKnight"),
			new EnemyInfo(EEnemyTileType.CastleArcher),
			new EnemyInfo(EEnemyTileType.CitySecurityGuard),
			new EnemyInfo(EEnemyTileType.ForestRodent),
			new EnemyInfo(EEnemyTileType.CastleEngineer),
			new EnemyInfo(EEnemyTileType.KeepAristocrat, 0, "KeepAristocrat"),
			new EnemyInfo(EEnemyTileType.FortressKnight),
			new EnemyInfo(EEnemyTileType.FortressGunner),
			new EnemyInfo(EEnemyTileType.CavesSlime, 0, "CavesSlime"),
			new EnemyInfo(EEnemyTileType.CavesSlime, 1, "CursedSlime", s => s.SpCavesSlime),
		};

		static readonly EnemyInfo[] SmallGroundedEnemies = {
			new EnemyInfo(EEnemyTileType.FleshSpider, 0, "Timespinner.GameAbstractions.GameObjects", "FleshSpider"),
			new EnemyInfo(EEnemyTileType.LakeBirdEgg, "Timespinner.GameObjects"),
		};

		static readonly EnemyInfo[] GroundedEnemies = 
			LargeGroundedEnemies
			.Concat(NormalGroundedEnemies)
			.Concat(SmallGroundedEnemies)
			.ToArray();

		static readonly EnemyInfo[] FlyingEnemies = {
			new EnemyInfo(EEnemyTileType.LakeFly),
			new EnemyInfo(EEnemyTileType.TowerRoyalGuard, 0, "TowerRoyalGuard", s => s.SpTowerDemonMage),
			new EnemyInfo(EEnemyTileType.TowerRoyalGuard, 1, "EmpRoyalGuard"),
			new EnemyInfo(EEnemyTileType.ForestMoth, 0, "ForestMoth"),
			new EnemyInfo(EEnemyTileType.ForestMoth, 1, "CursedMoth"),
			new EnemyInfo(EEnemyTileType.KeepDemon, 0, "KeepDemon"),
			new EnemyInfo(EEnemyTileType.KeepDemon, 1, "EmpDemon"),
			new EnemyInfo(EEnemyTileType.FlyingCheveux, s => s.SpCheveuxFlying),
			new EnemyInfo(EEnemyTileType.KickstarterFoe, 2, "GyreKain"),
			new EnemyInfo(EEnemyTileType.KickstarterFoe, 4, "GyreRyshia"),
			new EnemyInfo(EEnemyTileType.KickstarterFoe, 5, "GyreZel"),
			new EnemyInfo(EEnemyTileType.KickstarterFoe, 1, "GyreMeteorSparrow"),
			new EnemyInfo(EEnemyTileType.TowerPlasmaPod)
		};

		static readonly EnemyInfo[] CeilingEnemies = {
			new EnemyInfo(EEnemyTileType.CeilingStar),
			new EnemyInfo(EEnemyTileType.ForestPlantBat),
			new EnemyInfo(EEnemyTileType.CavesSporeVine, 0, "CavesSporeVine"),
			new EnemyInfo(EEnemyTileType.CavesSporeVine, 1, "CursedSporeVine"),
			new EnemyInfo(EEnemyTileType.CavesCopperWyvern, 0, "CavesCopperWyvern", s => s.SpCopperWyvern),
			new EnemyInfo(EEnemyTileType.CavesCopperWyvern, 1, "CursedCopperWyvern"),
		};

		static readonly EnemyInfo[] OtherEnemies = {
			new EnemyInfo(EEnemyTileType.CavesSnail, 0, "CavesSnail"), //to large for timestop
			new EnemyInfo(EEnemyTileType.CavesSnail, 1, "CursedSnail"), //to large for timestop
			new EnemyInfo(EEnemyTileType.FleshSpider, 1, "Timespinner.GameAbstractions.GameObjects", "LabSpider"), //path blocking lazer
			new EnemyInfo(EEnemyTileType.TempleFoe, 0, "Timespinner.GameObjects.Enemies._16_Temple", "TempleConviction"), //flying, vertically stationary
			new EnemyInfo(EEnemyTileType.TempleFoe, 1, "Timespinner.GameObjects.Enemies._16_Temple", "TempleZeal", s => s.SpTempleConviction), //timestop immunity
			new EnemyInfo(EEnemyTileType.TempleFoe, 2, "Timespinner.GameObjects.Enemies._16_Temple", "TempleJustice", s => s.SpTempleConviction), //timestop immunity
			//new EnemyInfo(EEnemyTileType.KickstarterFoe, 3, "GyreNethershade"), //timestop immunity, not visable
			new EnemyInfo(EEnemyTileType.LabTurret), //timestop immunity
			new EnemyInfo(EEnemyTileType.LabChild), //timestop immunity
			new EnemyInfo(EEnemyTileType.KeepAristocrat, 2, "EmpAristocrat"), //hard to controll as he always knocks himzelf away from you
		};

		static readonly EnemyInfo[] Enemies = GroundedEnemies
			.Concat(FlyingEnemies)
			.Concat(CeilingEnemies)
			.Concat(OtherEnemies)
			.ToArray();

		static readonly EnemyInfo[] UnderwaterEnemies = Enemies.Concat(new[] {
			new EnemyInfo(EEnemyTileType.LakeEel),
			new EnemyInfo(EEnemyTileType.LakeAnemone),
			new EnemyInfo(EEnemyTileType.CursedAnemone, s => s.SpCursedAnemone), //Flies to top of screen
		}).ToArray();

		static readonly LookupDictionary<Roomkey, RoomSpecificEnemies> HardcodedEnemies
			= new LookupDictionary<Roomkey, RoomSpecificEnemies>(k => k.RoomKey) {
				new RoomSpecificEnemies(1, 10, 200, 240, GroundedEnemies.Concat(FlyingEnemies)), //lake desolation bridge
				new RoomSpecificEnemies(9, 19, 608, 928, LargeGroundedEnemies.Concat(FlyingEnemies)), //Sealed caves mushroom tower jump
                new RoomSpecificEnemies(1, 3, 312, 320, NormalGroundedEnemies.Concat(FlyingEnemies)), //lake desolation ledge near save
				new RoomSpecificEnemies(7, 3, 312, 320, NormalGroundedEnemies.Concat(FlyingEnemies)), //lake serene ledge near save
				new RoomSpecificEnemies(1, 2, 936, 656, //lake serene ledge near save
					FlyingEnemies.Concat(new [] {
						new EnemyInfo(EEnemyTileType.CheveuxTank), //actually jumping Cheveux
						new EnemyInfo(EEnemyTileType.ForestBabyCheveux),
						new EnemyInfo(EEnemyTileType.CastleLargeSoldier, "Timespinner.GameObjects.Enemies._04_Ramparts"),
						new EnemyInfo(EEnemyTileType.KeepAristocrat, 1, "TowerIceMage"),
						new EnemyInfo(EEnemyTileType.LakeCheveux),
						new EnemyInfo(EEnemyTileType.FortressLargeSoldier, "Timespinner.GameObjects.Enemies._10_Fortress"),
					})
				),
				new RoomSpecificEnemies(3, 4, 776, 112, FlyingEnemies), //forest green bridge jump
				new RoomSpecificEnemies(4, 1, new []{ new Point(368, 160), new Point(432, 160) }), //castle boomers
				new RoomSpecificEnemies(10, 3, new []{ new Point(352, 128), new Point(448, 128) }), //militairy fortress boomers
				new RoomSpecificEnemies(7, 5, 128, 224), //fluffy bird pre cantoran
				new RoomSpecificEnemies(3, 15, new []{ new Point(360, 338), new Point(920, 66) }, //double bat cave jump
					FlyingEnemies.Concat(new [] {
						new EnemyInfo(EEnemyTileType.ForestBabyCheveux),
					})
				), 
				new RoomSpecificEnemies(4, 3, 192, 144, //castle scare the engineer
					FlyingEnemies.Concat(new [] {
						new EnemyInfo(EEnemyTileType.CheveuxTank), //actually jumping Cheveux
						new EnemyInfo(EEnemyTileType.WormFlowerWalker, s => s.SpWormFlower),
						new EnemyInfo(EEnemyTileType.DiscStatue, "Timespinner.GameAbstractions.GameObjects"), //Cat on roomba
						new EnemyInfo(EEnemyTileType.ForestBabyCheveux),
						new EnemyInfo(EEnemyTileType.CastleLargeSoldier, "Timespinner.GameObjects.Enemies._04_Ramparts"),
						new EnemyInfo(EEnemyTileType.KeepAristocrat, 1, "TowerIceMage"),
						new EnemyInfo(EEnemyTileType.LakeCheveux),
						new EnemyInfo(EEnemyTileType.FortressLargeSoldier, "Timespinner.GameObjects.Enemies._10_Fortress"),
						new EnemyInfo(EEnemyTileType.RedCheveux, s => s.SpCheveuxTank),
						new EnemyInfo(EEnemyTileType.FortressEngineer),
						new EnemyInfo(EEnemyTileType.CitySecurityGuard),
						new EnemyInfo(EEnemyTileType.ForestRodent),
						new EnemyInfo(EEnemyTileType.CastleEngineer),
						new EnemyInfo(EEnemyTileType.KeepAristocrat, 0, "KeepAristocrat"),
						new EnemyInfo(EEnemyTileType.CavesSlime, 0, "CavesSlime"),
						new EnemyInfo(EEnemyTileType.CavesSlime, 1, "CursedSlime", s => s.SpCavesSlime),
						new EnemyInfo(EEnemyTileType.FleshSpider, 0, "Timespinner.GameAbstractions.GameObjects", "FleshSpider")
					})
				),
				new RoomSpecificEnemies(5, 20, 584, 192, GroundedEnemies.Concat(FlyingEnemies)), // fire mage before twins
				new RoomSpecificEnemies(6, 2, 168, 496, // small demon in mid royal towers
					FlyingEnemies.Concat(new [] {
						new EnemyInfo(EEnemyTileType.ForestBabyCheveux),
					})
				),
				new RoomSpecificEnemies(6, 1, 1544, 96, // ice mage courtyard jump
					FlyingEnemies.Concat(new [] {
						new EnemyInfo(EEnemyTileType.CheveuxTank), //actually jumping Cheveux
						new EnemyInfo(EEnemyTileType.WormFlowerWalker, s => s.SpWormFlower),
						new EnemyInfo(EEnemyTileType.DiscStatue, "Timespinner.GameAbstractions.GameObjects"), //Cat on roomba
						new EnemyInfo(EEnemyTileType.ForestBabyCheveux),
						new EnemyInfo(EEnemyTileType.CastleLargeSoldier, "Timespinner.GameObjects.Enemies._04_Ramparts"),
						new EnemyInfo(EEnemyTileType.KeepAristocrat, 1, "TowerIceMage"),
						new EnemyInfo(EEnemyTileType.LakeCheveux),
						new EnemyInfo(EEnemyTileType.FortressLargeSoldier, "Timespinner.GameObjects.Enemies._10_Fortress"),
						new EnemyInfo(EEnemyTileType.RedCheveux, s => s.SpCheveuxTank),
						new EnemyInfo(EEnemyTileType.FortressEngineer),
						new EnemyInfo(EEnemyTileType.CitySecurityGuard),
						new EnemyInfo(EEnemyTileType.ForestRodent),
						new EnemyInfo(EEnemyTileType.CastleEngineer),
						new EnemyInfo(EEnemyTileType.KeepAristocrat, 0, "KeepAristocrat"),
						new EnemyInfo(EEnemyTileType.CavesSlime, 0, "CavesSlime"),
						new EnemyInfo(EEnemyTileType.CavesSlime, 1, "CursedSlime", s => s.SpCavesSlime),
						new EnemyInfo(EEnemyTileType.FleshSpider, 0, "Timespinner.GameAbstractions.GameObjects", "FleshSpider")
					})
				),
				new RoomSpecificEnemies(6, 17, 104, 448, FlyingEnemies), // top struggle juggle
				new RoomSpecificEnemies(6, 10, 200, 288, // lower struggle juggle
					FlyingEnemies.Concat(new [] {
						new EnemyInfo(EEnemyTileType.ForestBabyCheveux),
						new EnemyInfo(EEnemyTileType.KeepAristocrat, 1, "TowerIceMage"),
					})
				)
			};


/*TODO
Conviction uses wrong sprite
Plantbat breaks on floor
scyte guy missing face 7 5 128 224
rysha missing face
Lab turret faces wrong way
*/

		public static void RandomizeEnemies(
			Level level, dynamic levelReflected, int levelId, int roomId, IEnumerable<Monster> enemies, Seed seed)
		{
			var random = new Random((int)(seed.Id + (levelId * 100) + roomId));

			foreach (var enemy in enemies)
			{
				if (enemy.EnemyType == EEnemyTileType.JunkSpawner 
				|| enemy.EnemyType == EEnemyTileType.LabAdult
				|| enemy.GetType().IsSubclassOf(BossType))
					continue;

				var pos = enemy.Position;

				EnemyInfo newEnemyInfo;
				if (HardcodedEnemies.TryGetValue(new Roomkey(levelId, roomId), out var hardcodedEnemy) 
				    && hardcodedEnemy.Positions.Contains(pos))
				{
					if (hardcodedEnemy.Enemies.Length == 0)
						continue;

					newEnemyInfo = hardcodedEnemy.Enemies.SelectRandom(random);
				}
				else
				{
					newEnemyInfo = (enemy.IsInWater)
						? UnderwaterEnemies.SelectRandom(random)
						: Enemies.SelectRandom(random);
				}

				enemy.SilentKill();

				ScreenManager.Console.AddDebugLine($"[LVL:{levelId},ROOM:{roomId}] Replacing {enemy.EnemyType} with {newEnemyInfo.ClassName}");

				var newEnemySpec = new ObjectTileSpecification
				{
					Category = EObjectTileCategory.Enemy,
					Layer = ETileLayerType.Objects,
					IsFlippedHorizontally = enemy.IsImageFacingLeft,
					IsFlippedVertically = enemy.IsFlippedVertically,
					ObjectID = (int)newEnemyInfo.Type
				};

				dynamic newEnemy;
				if (newEnemyInfo.Type == EEnemyTileType.LakeBirdEgg)
					newEnemy = newEnemyInfo.Class.CreateInstance(
						false, pos, level, newEnemyInfo.SpriteSheet(level.GCM), -1, newEnemySpec, false);
				else
					newEnemy = newEnemyInfo.Class.CreateInstance(
						false, pos, level, newEnemyInfo.SpriteSheet(level.GCM), -1, newEnemySpec);

				newEnemy.InitializeMob();

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

	class RoomSpecificEnemies
	{
		public Roomkey RoomKey;
		public Point[] Positions;
		public EnemyInfo[] Enemies;

		public RoomSpecificEnemies(int levelId, int roomId, int x, int y, IEnumerable<EnemyInfo> validEnemies) 
			: this(levelId, roomId, x, y, validEnemies.ToArray())
		{
		}

		public RoomSpecificEnemies(int levelId, int roomId, Point[] positions, IEnumerable<EnemyInfo> validEnemies)
			: this(levelId, roomId, positions, validEnemies.ToArray())
		{
		}

		public RoomSpecificEnemies(int levelId, int roomId, int x, int y, params EnemyInfo[] validEnemies)
			: this(levelId, roomId, new[] { new Point(x, y) }, validEnemies)
		{
		}

		public RoomSpecificEnemies(int levelId, int roomId, Point[] positions, params EnemyInfo[] validEnemies)
		{
			RoomKey = new Roomkey(levelId, roomId);
			Positions = positions;
			Enemies = validEnemies;
		}
	}
}
