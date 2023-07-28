using System.Collections.Generic;
using Timespinner.Core.Specifications;
using TsRandomizer.LevelObjects;

namespace TsRandomizer.LevelObjects
{
	public enum EnemyType
	{
		JumpingCheveuxTank,
		CheveuxTank,
		HelicopterCheveux,
		WormFlower,
		WormFlowerWalker,
		RoombaCat,
		ForestBabyCheveux,
		ForestWormFlower,
		PastMushroomTower,
		PresentMushroomTower,
		PastLargeSoldier,
		ChargingCheveux,
		WildCheveux,
		IceMage,
		FireMage,
		FlyingIceMage,
		LargeCheveux,
		PresentLargeSoldier,
		PastEngineer,
		PastBomber,
		PastLogThrower,
		PresentEngineer,
		PresentBomber,
		PastSiren,
		PresentSiren,
		PastShieldKnight,
		PresentShieldKnight,
		PastArcher,
		PresentArcher,
		Granadier,
		Rat,
		PastSlime,
		PresentSlime,
		Spider,
		HellSpider,
		CeilingSpider,
		CeilingHellSpider,
		Egg,
		Fly,
		PastLargeDemon,
		PresentLargeDemon,
		PastMoth,
		PresentMoth,
		PastDemon,
		PresentDemon,
		SealDog,
		MeteorSparrow,
		ScyteDemon,
		Ryshia,
		TomeOrbGuy,
		PlasmaPod,
		CeilingStar,
		Bat,
		PastCeilingTentacle,
		PresentCeilingTentacle,
		PastWaterDrake,
		PresentWaterDrake,
		PastSnail,
		PresentSnail,
		Conviction,
		Zeal,
		Justice,
		Nethershade,
		Turret,
		LabDemon,
		Eel,
		PastAnemone,
		PresentAnemone,
		PastFloatingAnemone,
		PresentFloatingAnemone,
		PastCeilingAnemone,
		PresentCeilingAnemone,
		XarionBossHand,
		TrashSpawner
	}

	class EnemyInfo
	{
		public static LookupDictionary<EnemyType, EnemyInfo> Get = new LookupDictionary<EnemyType, EnemyInfo>(e => e.Type) {
			new EnemyInfo(EnemyType.JumpingCheveuxTank, EEnemyTileType.CheveuxTank),
			new EnemyInfo(EnemyType.CheveuxTank, EEnemyTileType.RedCheveux),
			new EnemyInfo(EnemyType.HelicopterCheveux, EEnemyTileType.FlyingCheveux),
			new EnemyInfo(EnemyType.WormFlower, EEnemyTileType.WormFlower),
			new EnemyInfo(EnemyType.WormFlowerWalker, EEnemyTileType.WormFlowerWalker),
			new EnemyInfo(EnemyType.RoombaCat, EEnemyTileType.DiscStatue, "Timespinner.GameAbstractions.GameObjects"),
			new EnemyInfo(EnemyType.ForestBabyCheveux, EEnemyTileType.ForestBabyCheveux),
			new EnemyInfo(EnemyType.ForestWormFlower, EEnemyTileType.ForestWormFlower),
			new EnemyInfo(EnemyType.PastMushroomTower, EEnemyTileType.CavesMushroomTower, 0, "CavesMushroomTower"),
			new EnemyInfo(EnemyType.PresentMushroomTower, EEnemyTileType.CavesMushroomTower, 1, "CursedMushroomTower"),
			new EnemyInfo(EnemyType.PastLargeSoldier, EEnemyTileType.CastleLargeSoldier, "Timespinner.GameObjects.Enemies._04_Ramparts"),
			new EnemyInfo(EnemyType.PresentLargeSoldier, EEnemyTileType.FortressLargeSoldier, "Timespinner.GameObjects.Enemies._10_Fortress"),
			new EnemyInfo(EnemyType.ChargingCheveux, EEnemyTileType.KeepWarCheveux, 0),
			new EnemyInfo(EnemyType.WildCheveux, EEnemyTileType.KeepWarCheveux, 1),
			new EnemyInfo(EnemyType.FireMage, EEnemyTileType.KeepAristocrat, 0, "KeepAristocrat"),
			new EnemyInfo(EnemyType.IceMage, EEnemyTileType.KeepAristocrat, 1, "TowerIceMage"),
			new EnemyInfo(EnemyType.FlyingIceMage, EEnemyTileType.KeepAristocrat, 2, "EmpAristocrat"),
			new EnemyInfo(EnemyType.LargeCheveux, EEnemyTileType.LakeCheveux),
			new EnemyInfo(EnemyType.PresentEngineer, EEnemyTileType.FortressEngineer, 0),
			new EnemyInfo(EnemyType.PresentBomber, EEnemyTileType.FortressEngineer, 1),
			new EnemyInfo(EnemyType.PastEngineer, EEnemyTileType.CastleEngineer, 0),
			new EnemyInfo(EnemyType.PastBomber, EEnemyTileType.CastleEngineer, 1),
			new EnemyInfo(EnemyType.PastLogThrower, EEnemyTileType.CastleEngineer, 2),
			new EnemyInfo(EnemyType.PastSiren, EEnemyTileType.CavesSiren, 0, "CavesSiren"),
			new EnemyInfo(EnemyType.PresentSiren, EEnemyTileType.CavesSiren, 1, "CursedSiren"),
			new EnemyInfo(EnemyType.PastShieldKnight, EEnemyTileType.CastleShieldKnight),
			new EnemyInfo(EnemyType.PresentShieldKnight, EEnemyTileType.FortressKnight),
			new EnemyInfo(EnemyType.PastArcher, EEnemyTileType.CastleArcher),
			new EnemyInfo(EnemyType.PresentArcher, EEnemyTileType.FortressGunner),
			new EnemyInfo(EnemyType.Granadier, EEnemyTileType.CitySecurityGuard),
			new EnemyInfo(EnemyType.Rat, EEnemyTileType.ForestRodent),
			new EnemyInfo(EnemyType.PastSlime, EEnemyTileType.CavesSlime, 0, "CavesSlime"),
			new EnemyInfo(EnemyType.PresentSlime, EEnemyTileType.CavesSlime, 1, "CursedSlime"),
			new EnemyInfo(EnemyType.Spider, EEnemyTileType.FleshSpider, 0, "Timespinner.GameAbstractions.GameObjects", "FleshSpider"),
			new EnemyInfo(EnemyType.HellSpider, EEnemyTileType.FleshSpider, 1, "Timespinner.GameAbstractions.GameObjects", "LabSpider"),
			new EnemyInfo(EnemyType.CeilingSpider, EEnemyTileType.FleshSpider, 0, "Timespinner.GameAbstractions.GameObjects", "FleshSpider", true),
			new EnemyInfo(EnemyType.CeilingHellSpider, EEnemyTileType.FleshSpider, 1, "Timespinner.GameAbstractions.GameObjects", "LabSpider", true),
			new EnemyInfo(EnemyType.Egg, EEnemyTileType.LakeBirdEgg, "Timespinner.GameObjects"),
			new EnemyInfo(EnemyType.Fly, EEnemyTileType.LakeFly),
			new EnemyInfo(EnemyType.PastLargeDemon, EEnemyTileType.TowerRoyalGuard, 0, "TowerRoyalGuard"),
			new EnemyInfo(EnemyType.PresentLargeDemon, EEnemyTileType.TowerRoyalGuard, 1, "EmpRoyalGuard"),
			new EnemyInfo(EnemyType.PastMoth, EEnemyTileType.ForestMoth, 0, "ForestMoth"),
			new EnemyInfo(EnemyType.PresentMoth, EEnemyTileType.ForestMoth, 1, "CursedMoth"),
			new EnemyInfo(EnemyType.PastDemon, EEnemyTileType.KeepDemon, 0, "KeepDemon"),
			new EnemyInfo(EnemyType.PresentDemon, EEnemyTileType.KeepDemon, 1, "EmpDemon"),
			new EnemyInfo(EnemyType.SealDog, EEnemyTileType.KickstarterFoe, 0, "GyreMajorUgly"),
			new EnemyInfo(EnemyType.MeteorSparrow, EEnemyTileType.KickstarterFoe, 1, "GyreMeteorSparrow"),
			new EnemyInfo(EnemyType.ScyteDemon, EEnemyTileType.KickstarterFoe, 2, "GyreKain"),
			new EnemyInfo(EnemyType.Ryshia, EEnemyTileType.KickstarterFoe, 4, "GyreRyshia"),
			new EnemyInfo(EnemyType.TomeOrbGuy, EEnemyTileType.KickstarterFoe, 5, "GyreZel"),
			new EnemyInfo(EnemyType.PlasmaPod, EEnemyTileType.TowerPlasmaPod),
			new EnemyInfo(EnemyType.CeilingStar, EEnemyTileType.CeilingStar, true),
			new EnemyInfo(EnemyType.Bat, EEnemyTileType.ForestPlantBat, true),
			new EnemyInfo(EnemyType.PastCeilingTentacle, EEnemyTileType.CavesSporeVine, 0, "CavesSporeVine", true),
			new EnemyInfo(EnemyType.PresentCeilingTentacle, EEnemyTileType.CavesSporeVine, 1, "CursedSporeVine", true),
			new EnemyInfo(EnemyType.PastWaterDrake, EEnemyTileType.CavesCopperWyvern, 0, "CavesCopperWyvern"),
			new EnemyInfo(EnemyType.PresentWaterDrake, EEnemyTileType.CavesCopperWyvern, 1, "CursedCopperWyvern"),
			new EnemyInfo(EnemyType.PastSnail, EEnemyTileType.CavesSnail, 0, "CavesSnail"),
			new EnemyInfo(EnemyType.PresentSnail, EEnemyTileType.CavesSnail, 1, "CursedSnail"),
			new EnemyInfo(EnemyType.Conviction, EEnemyTileType.TempleFoe, 0, "Timespinner.GameObjects.Enemies._16_Temple", "TempleConviction"),
			new EnemyInfo(EnemyType.Zeal, EEnemyTileType.TempleFoe, 1, "Timespinner.GameObjects.Enemies._16_Temple", "TempleZeal"),
			new EnemyInfo(EnemyType.Justice, EEnemyTileType.TempleFoe, 2, "Timespinner.GameObjects.Enemies._16_Temple", "TempleJustice"),
			new EnemyInfo(EnemyType.Nethershade, EEnemyTileType.KickstarterFoe, 3, "GyreNethershade"),
			new EnemyInfo(EnemyType.Turret, EEnemyTileType.LabTurret),
			new EnemyInfo(EnemyType.LabDemon, EEnemyTileType.LabChild),
			new EnemyInfo(EnemyType.Eel, EEnemyTileType.LakeEel),
			new EnemyInfo(EnemyType.PastAnemone, EEnemyTileType.LakeAnemone, 1),
			new EnemyInfo(EnemyType.PresentAnemone, EEnemyTileType.CursedAnemone, 1),
			new EnemyInfo(EnemyType.PastFloatingAnemone, EEnemyTileType.LakeAnemone, 0),
			new EnemyInfo(EnemyType.PresentFloatingAnemone, EEnemyTileType.CursedAnemone, 0),
			new EnemyInfo(EnemyType.PastCeilingAnemone, EEnemyTileType.LakeAnemone, 1, true),
			new EnemyInfo(EnemyType.PresentCeilingAnemone, EEnemyTileType.CursedAnemone, 1, true),
			new EnemyInfo(EnemyType.XarionBossHand, EEnemyTileType.XarionBoss, 0, "Timespinner.GameObjects.Bosses.Z_Xarion", "XarionBossHand"),
			//new EnemyInfo(EnemyType.TrashSpawner, EEnemyTileType.JunkSpawner, 0, "Timespinner.GameObjects.Events", "JunkSpawnerEvent") //not properly scaled
		};

		public readonly EnemyType Type;
		public readonly EEnemyTileType TileType;
		public readonly int Argument;
		public readonly string ClassName;
		public readonly bool IsCeilingEnemy;

		public EnemyInfo(EnemyType type, EEnemyTileType tileType, bool isCeilingEnemy = false) 
			: this(type, tileType, 0, isCeilingEnemy)
		{
		}

		public EnemyInfo(EnemyType type, EEnemyTileType tileType, int argument, bool isCeilingEnemy = false)
			: this(type, tileType, argument, tileType.ToString(), isCeilingEnemy)
		{
		}

		public EnemyInfo(EnemyType type, EEnemyTileType tileType, string classPath, bool isCeilingEnemy = false)
			: this(type, tileType, 0, classPath, tileType.ToString(), isCeilingEnemy)
		{
		}

		public EnemyInfo(EnemyType type, EEnemyTileType tileType, int argument, string className, bool isCeilingEnemy = false)
			: this(type, tileType, argument, "Timespinner.GameObjects.Enemies", className, isCeilingEnemy)
		{
		}

		public EnemyInfo(EnemyType type, EEnemyTileType tileType, int argument, string classPath, string className, bool isCeilingEnemy = false)
		{
			Type = type;
			TileType = tileType;
			Argument = argument;
			ClassName = $"{classPath}.{className}";
			IsCeilingEnemy = isCeilingEnemy;
		}
	}
}
