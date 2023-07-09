using System.Collections.Generic;
using Timespinner.Core.Specifications;

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
		IceMage,
		FireMage,
		FlyingIceMage,
		LargeCheveux,
		PresentLargeSoldier,
		PastBomber,
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
		XarionBossHand
	}

	class EnemyInfo
	{
		public static Dictionary<EnemyType, EnemyInfo> Get = new Dictionary<EnemyType, EnemyInfo> {
			{ EnemyType.JumpingCheveuxTank, new EnemyInfo(EEnemyTileType.CheveuxTank) },
			{ EnemyType.CheveuxTank, new EnemyInfo(EEnemyTileType.RedCheveux) },
			{ EnemyType.HelicopterCheveux, new EnemyInfo(EEnemyTileType.FlyingCheveux) },
			{ EnemyType.WormFlower, new EnemyInfo(EEnemyTileType.WormFlower) },
			{ EnemyType.WormFlowerWalker, new EnemyInfo(EEnemyTileType.WormFlowerWalker) },
			{ EnemyType.RoombaCat, new EnemyInfo(EEnemyTileType.DiscStatue, "Timespinner.GameAbstractions.GameObjects") },
			{ EnemyType.ForestBabyCheveux, new EnemyInfo(EEnemyTileType.ForestBabyCheveux) },
			{ EnemyType.ForestWormFlower, new EnemyInfo(EEnemyTileType.ForestWormFlower) },
			{ EnemyType.PastMushroomTower, new EnemyInfo(EEnemyTileType.CavesMushroomTower, 0, "CavesMushroomTower") },
			{ EnemyType.PresentMushroomTower, new EnemyInfo(EEnemyTileType.CavesMushroomTower, 1, "CursedMushroomTower") },
			{ EnemyType.PastLargeSoldier, new EnemyInfo(EEnemyTileType.CastleLargeSoldier, "Timespinner.GameObjects.Enemies._04_Ramparts") },
			{ EnemyType.PresentLargeSoldier, new EnemyInfo(EEnemyTileType.FortressLargeSoldier, "Timespinner.GameObjects.Enemies._10_Fortress") },
			{ EnemyType.ChargingCheveux, new EnemyInfo(EEnemyTileType.KeepWarCheveux) },
			{ EnemyType.FireMage, new EnemyInfo(EEnemyTileType.KeepAristocrat, 0, "KeepAristocrat") },
			{ EnemyType.IceMage, new EnemyInfo(EEnemyTileType.KeepAristocrat, 1, "TowerIceMage") },
			{ EnemyType.FlyingIceMage, new EnemyInfo(EEnemyTileType.KeepAristocrat, 2, "EmpAristocrat") },
			{ EnemyType.LargeCheveux, new EnemyInfo(EEnemyTileType.LakeCheveux) },
			{ EnemyType.PresentBomber, new EnemyInfo(EEnemyTileType.FortressEngineer) },
			{ EnemyType.PastBomber, new EnemyInfo(EEnemyTileType.CastleEngineer) },
			{ EnemyType.PastSiren, new EnemyInfo(EEnemyTileType.CavesSiren, 0, "CavesSiren") },
			{ EnemyType.PresentSiren, new EnemyInfo(EEnemyTileType.CavesSiren, 1, "CursedSiren") },
			{ EnemyType.PastShieldKnight, new EnemyInfo(EEnemyTileType.CastleShieldKnight) },
			{ EnemyType.PresentShieldKnight, new EnemyInfo(EEnemyTileType.FortressKnight) },
			{ EnemyType.PastArcher, new EnemyInfo(EEnemyTileType.CastleArcher) },
			{ EnemyType.PresentArcher, new EnemyInfo(EEnemyTileType.FortressGunner) },
			{ EnemyType.Granadier, new EnemyInfo(EEnemyTileType.CitySecurityGuard) },
			{ EnemyType.Rat, new EnemyInfo(EEnemyTileType.ForestRodent) },
			{ EnemyType.PastSlime, new EnemyInfo(EEnemyTileType.CavesSlime, 0, "CavesSlime") },
			{ EnemyType.PresentSlime, new EnemyInfo(EEnemyTileType.CavesSlime, 1, "CursedSlime") },
			{ EnemyType.Spider, new EnemyInfo(EEnemyTileType.FleshSpider, 0, "Timespinner.GameAbstractions.GameObjects", "FleshSpider") },
			{ EnemyType.HellSpider, new EnemyInfo(EEnemyTileType.FleshSpider, 1, "Timespinner.GameAbstractions.GameObjects", "LabSpider") },
			{ EnemyType.Egg, new EnemyInfo(EEnemyTileType.LakeBirdEgg, "Timespinner.GameObjects") },
			{ EnemyType.Fly, new EnemyInfo(EEnemyTileType.LakeFly) },
			{ EnemyType.PastLargeDemon, new EnemyInfo(EEnemyTileType.TowerRoyalGuard, 0, "TowerRoyalGuard") },
			{ EnemyType.PresentLargeDemon, new EnemyInfo(EEnemyTileType.TowerRoyalGuard, 1, "EmpRoyalGuard") },
			{ EnemyType.PastMoth, new EnemyInfo(EEnemyTileType.ForestMoth, 0, "ForestMoth") },
			{ EnemyType.PresentMoth, new EnemyInfo(EEnemyTileType.ForestMoth, 1, "CursedMoth") },
			{ EnemyType.PastDemon, new EnemyInfo(EEnemyTileType.KeepDemon, 0, "KeepDemon") },
			{ EnemyType.PresentDemon, new EnemyInfo(EEnemyTileType.KeepDemon, 1, "EmpDemon") },
			{ EnemyType.SealDog, new EnemyInfo(EEnemyTileType.KickstarterFoe, 0, "GyreMajorUgly") },
			{ EnemyType.MeteorSparrow, new EnemyInfo(EEnemyTileType.KickstarterFoe, 1, "GyreMeteorSparrow") },
			{ EnemyType.ScyteDemon, new EnemyInfo(EEnemyTileType.KickstarterFoe, 2, "GyreKain") },
			{ EnemyType.Ryshia, new EnemyInfo(EEnemyTileType.KickstarterFoe, 4, "GyreRyshia") },
			{ EnemyType.TomeOrbGuy, new EnemyInfo(EEnemyTileType.KickstarterFoe, 5, "GyreZel") },
			{ EnemyType.PlasmaPod, new EnemyInfo(EEnemyTileType.TowerPlasmaPod) },
			{ EnemyType.CeilingStar, new EnemyInfo(EEnemyTileType.CeilingStar, true) },
			{ EnemyType.Bat, new EnemyInfo(EEnemyTileType.ForestPlantBat, true) },
			{ EnemyType.PastCeilingTentacle, new EnemyInfo(EEnemyTileType.CavesSporeVine, 0, "CavesSporeVine", true) },
			{ EnemyType.PresentCeilingTentacle, new EnemyInfo(EEnemyTileType.CavesSporeVine, 1, "CursedSporeVine", true) },
			{ EnemyType.PastWaterDrake, new EnemyInfo(EEnemyTileType.CavesCopperWyvern, 0, "CavesCopperWyvern") },
			{ EnemyType.PresentWaterDrake, new EnemyInfo(EEnemyTileType.CavesCopperWyvern, 1, "CursedCopperWyvern") },
			{ EnemyType.PastSnail, new EnemyInfo(EEnemyTileType.CavesSnail, 0, "CavesSnail") },
			{ EnemyType.PresentSnail, new EnemyInfo(EEnemyTileType.CavesSnail, 1, "CursedSnail") },
			{ EnemyType.Conviction, new EnemyInfo(EEnemyTileType.TempleFoe, 0, "Timespinner.GameObjects.Enemies._16_Temple", "TempleConviction") },
			{ EnemyType.Zeal, new EnemyInfo(EEnemyTileType.TempleFoe, 1, "Timespinner.GameObjects.Enemies._16_Temple", "TempleZeal") },
			{ EnemyType.Justice, new EnemyInfo(EEnemyTileType.TempleFoe, 2, "Timespinner.GameObjects.Enemies._16_Temple", "TempleJustice") },
			{ EnemyType.Nethershade, new EnemyInfo(EEnemyTileType.KickstarterFoe, 3, "GyreNethershade") },
			{ EnemyType.Turret, new EnemyInfo(EEnemyTileType.LabTurret) },
			{ EnemyType.LabDemon, new EnemyInfo(EEnemyTileType.LabChild) },
			{ EnemyType.Eel, new EnemyInfo(EEnemyTileType.LakeEel) },
			{ EnemyType.PastAnemone, new EnemyInfo(EEnemyTileType.LakeAnemone) },
			{ EnemyType.PresentAnemone, new EnemyInfo(EEnemyTileType.CursedAnemone) },
			{ EnemyType.XarionBossHand, new EnemyInfo(EEnemyTileType.XarionBoss, 0, "Timespinner.GameObjects.Bosses.Z_Xarion", "XarionBossHand") },
		};

		public readonly EEnemyTileType Type;
		public readonly int Argument;
		public readonly string ClassName;
		public readonly bool IsCeilingEnemy;

		public EnemyInfo(EEnemyTileType type, bool isCeilingEnemy = false) 
			: this(type, 0, type.ToString(), isCeilingEnemy)
		{
		}

		public EnemyInfo(EEnemyTileType type, string classPath, bool isCeilingEnemy = false)
			: this(type, 0, classPath, type.ToString(), isCeilingEnemy)
		{
		}

		public EnemyInfo(EEnemyTileType type, int argument, string className, bool isCeilingEnemy = false)
			: this(type, argument, "Timespinner.GameObjects.Enemies", className, isCeilingEnemy)
		{
		}

		public EnemyInfo(EEnemyTileType type, int argument, string classPath, string className, bool isCeilingEnemy = false)
		{
			Type = type;
			Argument = argument;
			ClassName = $"{classPath}.{className}";
			IsCeilingEnemy = isCeilingEnemy;
		}
	}
}
