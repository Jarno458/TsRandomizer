using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.ReplacementObjects;
using R = TsRandomizer.Randomisation.Requirement;

namespace TsRandomizer.Randomisation
{
	class ItemLocationMap : LookupDictionairy<ItemKey, ItemLocation>
	{
		internal static readonly R MultipleSmallJumpsOfNpc = R.TimespinnerWheel | R.UpwardDash;
		internal static readonly Gate DoubleJumpOfNpc = (R.DoubleJump & R.TimespinnerWheel) | R.UpwardDash;
		internal static readonly Gate ForwardDashDoubleJump = (R.ForwardDash & R.DoubleJump) | R.UpwardDash;

		public static readonly R LowerLakeDesolationBridge = R.TimeStop | R.ForwardDash | R.GateKittyBoss | R.GateLeftLibrary;
		internal static readonly Gate AccessToPast = 
			(
			   R.TimespinnerWheel & R.TimespinnerSpindle //activateLibraryTimespinner
               & (LowerLakeDesolationBridge & R.CardD) //midLibrary
			) //libraryTimespinner
			| R.GateLakeSirineLeft
			| R.GateAccessToPast
			| R.GateLakeSirineRight
			| R.GateRoyalTowers
			| R.GateCastleRamparts
			| R.GateCastleKeep;

		//past
		internal static readonly Gate LeftSideForestCaves = (AccessToPast & (R.TimespinnerWheel | R.ForwardDash | R.DoubleJump)) | R.GateLakeSirineRight | R.GateLakeSirineLeft;
		internal static readonly Gate UpperLakeSirine = (LeftSideForestCaves & R.TimeStop) | R.GateLakeSirineLeft;
		internal static readonly Gate LowerlakeSirine = (LeftSideForestCaves | R.GateLakeSirineLeft) & R.Swimming;
		internal static readonly Gate LowerCavesOfBanishment = LowerlakeSirine;
		internal static readonly Gate UpperCavesOfBanishment = AccessToPast;
		internal static readonly Gate CastleRamparts = AccessToPast;
		internal static readonly Gate CastleKeep = CastleRamparts;
		internal static readonly Gate RoyalTower = (CastleKeep & R.DoubleJump) | R.GateRoyalTowers;
		internal static readonly Gate MidRoyalTower = RoyalTower & (MultipleSmallJumpsOfNpc | ForwardDashDoubleJump);
		internal static readonly Gate UpperRoyalTower = MidRoyalTower & R.DoubleJump;

		//future
		internal static readonly Gate UpperLakeDesolation = UpperLakeSirine & R.AntiWeed;
		internal static readonly Gate LeftLibrary = UpperLakeDesolation | LowerLakeDesolationBridge | (R.GateMilitairyGate & R.CardD & (R.CardB | (R.CardC & R.CardE)));
		internal static readonly Gate UpperLeftLibrary = LeftLibrary & (R.DoubleJump | R.ForwardDash);
		internal static readonly Gate MidLibrary = (LeftLibrary & R.CardD) | (R.GateMilitairyGate & (R.CardB | (R.CardC & R.CardE)));
		internal static readonly Gate UpperRightSideLibrary = (MidLibrary & (R.CardC | (R.CardB & R.CardE))) | (R.GateMilitairyGate & R.CardE);
		internal static readonly Gate RightSizeLibraryElevator = (MidLibrary & R.CardE & (R.CardC | R.CardB)) | (R.GateMilitairyGate & R.CardE);
		internal static readonly Gate LowerRightSideLibrary = (MidLibrary & R.CardB) | RightSizeLibraryElevator | R.GateMilitairyGate;
		internal static readonly R SealedCavesLeft = R.DoubleJump;
		internal static readonly Gate SealedCavesLower = SealedCavesLeft & R.CardA;
		internal static readonly Gate SealedCavesSirens = (MidLibrary & R.CardB & R.CardE) | R.GateSealedSirensCave;
		internal static readonly Gate KillTwinsAndMaw = LowerlakeSirine & CastleKeep;
		internal static readonly Gate KillAll3MajorBosses = LowerRightSideLibrary & KillTwinsAndMaw & UpperRoyalTower;
		internal static readonly Gate MilitairyFortress = KillAll3MajorBosses;
		internal static readonly Gate MilitairyFortressHangar = MilitairyFortress;
		internal static readonly Gate RightSideMilitairyFortressHangar = MilitairyFortressHangar & R.DoubleJump;
		internal static readonly Gate TheLab = MilitairyFortressHangar & R.CardB;
		internal static readonly Gate TheLabPoweredOff = TheLab & DoubleJumpOfNpc;
		internal static readonly Gate UppereLab = TheLabPoweredOff & ForwardDashDoubleJump;
		internal static readonly Gate EmperorsTower = UppereLab;

		//pyramid
		internal static readonly Gate LeftPyramid = UppereLab & (
			R.TimespinnerWheel & R.TimespinnerSpindle &
			R.TimespinnerPiece1 & R.TimespinnerPiece2 & R.TimespinnerPiece3);
		internal static readonly Gate Nightmare = LeftPyramid & R.UpwardDash;

		public new ItemLocation this[ItemKey key] => GetItemLocationBasedOnKeyOrRoomKey(key);

		public ItemLocationMap() : base(160, l => l.Key)
		{
			AddPresentItemLocations();
			AddPastItemLocations();
			AddPyramidItemLocations();
		}

		void AddPresentItemLocations()
		{
			//tutorial
			Add(ItemKey.TutorialMeleeOrb, ItemInfo.Get(EInventoryOrbType.Blue, EOrbSlot.Melee));
			Add(ItemKey.TutorialSpellOrb, ItemInfo.Get(EInventoryOrbType.Blue, EOrbSlot.Spell));
			//starter lake desolation
			Add(new ItemKey(1, 1, 1528, 144), ItemInfo.Get(EInventoryUseItemType.FuturePotion));
			Add(new ItemKey(1, 15, 264, 144), ItemInfo.Get(EInventoryEquipmentType.OldCoat));
			Add(new ItemKey(1, 25, 296, 176), ItemInfo.Get(EInventoryUseItemType.FutureHiPotion));
			Add(new ItemKey(1, 9, 600, 144 + TimespinnerWheel.YOffset), ItemInfo.Get(EInventoryRelicType.TimespinnerWheel));
			Add(new ItemKey(1, 14, 40, 176), ItemInfo.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation);
			//lower lake desolation
			Add(new ItemKey(1, 2, 1016, 384), ItemInfo.Get(EItemType.MaxSand), R.TimeStop);
			Add(new ItemKey(1, 11, 72, 240), ItemInfo.Get(EItemType.MaxHP), LowerLakeDesolationBridge);
			Add(new ItemKey(1, 3, 56, 176), ItemInfo.Get(EItemType.MaxAura), R.TimeStop);
			//upper lake desolation
			Add(new ItemKey(1, 17, 152, 96), ItemInfo.Get(EInventoryUseItemType.GoldRing), UpperLakeDesolation);
			Add(new ItemKey(1, 21, 200, 144), ItemInfo.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation);
			Add(new ItemKey(1, 20, 232, 96), ItemInfo.Get(EInventoryUseItemType.MagicMarbles), UpperLakeDesolation & R.DoubleJump);
			Add(new ItemKey(1, 20, 168, 240), ItemInfo.Get(EInventoryUseItemType.FuturePotion), UpperLakeDesolation);
			Add(new ItemKey(1, 22, 344, 160), ItemInfo.Get(EInventoryUseItemType.FutureHiPotion), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1320, 189), ItemInfo.Get(EInventoryOrbType.Moon, EOrbSlot.Melee), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1272, 192), ItemInfo.Get(EInventoryEquipmentType.CaptainsCap), UpperLakeDesolation & R.GassMask & KillTwinsAndMaw);
			Add(new ItemKey(1, 18, 1368, 192), ItemInfo.Get(EInventoryEquipmentType.CaptainsJacket), UpperLakeDesolation & R.GassMask & KillTwinsAndMaw);
			Add(new RoomItemKey(1, 5), ItemInfo.Get(EInventoryOrbType.Blade, EOrbSlot.Melee), UpperLakeDesolation | LowerLakeDesolationBridge);
			//libary left
			Add(new ItemKey(2, 60, 328, 160), ItemInfo.Get(EItemType.MaxHP), LeftLibrary);
			Add(new ItemKey(2, 54, 296, 176), ItemInfo.Get(EInventoryRelicType.ScienceKeycardD), LeftLibrary); 
			Add(new ItemKey(2, 44, 680, 368), ItemInfo.Get(EInventoryRelicType.FoeScanner), LeftLibrary);
			Add(new ItemKey(2, 47, 216, 208), ItemInfo.Get(EInventoryUseItemType.Ether), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 152, 208), ItemInfo.Get(EInventoryOrbType.Blade, EOrbSlot.Passive), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 88, 208), ItemInfo.Get(EInventoryOrbType.Blade, EOrbSlot.Spell), LeftLibrary & R.CardD);
			//libary top
			Add(new ItemKey(2, 56, 168, 192), ItemInfo.Get(EInventoryUseItemType.GoldNecklace), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 392, 192), ItemInfo.Get(EInventoryUseItemType.GoldRing), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 616, 192), ItemInfo.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 840, 192), ItemInfo.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 1064, 192), ItemInfo.Get(EInventoryUseItemType.MagicMarbles), UpperLeftLibrary);
			//libary mid
			Add(new ItemKey(2, 34, 232, 1200), ItemInfo.Get(EInventoryUseItemType.Jerky), MidLibrary);
			Add(new ItemKey(2, 40, 344, 176), ItemInfo.Get(EInventoryRelicType.ScienceKeycardC), MidLibrary);
			Add(new ItemKey(2, 32, 328, 160), ItemInfo.Get(EInventoryUseItemType.GoldRing), MidLibrary & R.CardC);
			Add(new ItemKey(2, 7, 232, 144), ItemInfo.Get(EItemType.MaxAura), MidLibrary);
			Add(new ItemKey(2, 25, 328, 192), ItemInfo.Get(EItemType.MaxSand), MidLibrary & R.CardE);
			//libary right, 
			Add(new ItemKey(2, 15, 760, 192), ItemInfo.Get(EInventoryUseItemType.FuturePotion), UpperRightSideLibrary);
			Add(new ItemKey(2, 20, 72, 1200), ItemInfo.Get(EInventoryUseItemType.Jerky), RightSizeLibraryElevator);
			Add(new ItemKey(2, 23, 72, 560), ItemInfo.Get(EInventoryUseItemType.FutureHiPotion), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 1112, 112), ItemInfo.Get(EInventoryUseItemType.FutureHiPotion), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 136, 304), ItemInfo.Get(EInventoryRelicType.ElevatorKeycard), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 11, 104, 192), ItemInfo.Get(EInventoryUseItemType.EssenceCrystal), LowerRightSideLibrary);
			Add(new ItemKey(2, 29, 280, 222 + TimespinnerSpindle.YOffset), ItemInfo.Get(EInventoryRelicType.TimespinnerSpindle), RightSizeLibraryElevator);
			Add(new RoomItemKey(2, 52), ItemInfo.Get(EInventoryRelicType.TimespinnerGear2), RightSizeLibraryElevator & R.CardA);
			//Sealed Caves left
			Add(new ItemKey(9, 10, 248, 848), ItemInfo.Get(EInventoryRelicType.ScienceKeycardB), SealedCavesLeft);
			Add(new ItemKey(9, 19, 664, 704), ItemInfo.Get(EInventoryUseItemType.Antidote), SealedCavesLower & R.TimeStop);
			Add(new ItemKey(9, 39, 88, 192), ItemInfo.Get(EInventoryUseItemType.Antidote), SealedCavesLower);
			Add(new ItemKey(9, 41, 312, 192), ItemInfo.Get(EInventoryUseItemType.GalaxyStone), SealedCavesLower & ForwardDashDoubleJump);
			Add(new ItemKey(9, 42, 328, 192), ItemInfo.Get(EInventoryUseItemType.MagicMarbles), SealedCavesLower);
			Add(new ItemKey(9, 12, 280, 160), ItemInfo.Get(EItemType.MaxHP), SealedCavesLower);
			Add(new ItemKey(9, 48, 104, 160), ItemInfo.Get(EInventoryUseItemType.FutureEther), SealedCavesLower);
			Add(new ItemKey(9, 15, 248, 192), ItemInfo.Get(EInventoryUseItemType.FutureEther), SealedCavesLower & R.DoubleJump);
			Add(new RoomItemKey(9, 13), ItemInfo.Get(EInventoryRelicType.TimespinnerGear3), SealedCavesLower);
			//Sealed Caves (sirens)
			Add(new ItemKey(9, 5, 88, 496), ItemInfo.Get(EItemType.MaxSand), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 3, 1848, 576), ItemInfo.Get(EInventoryEquipmentType.BirdStatue), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 3, 744, 560), ItemInfo.Get(EItemType.MaxAura), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 2, 184, 176), ItemInfo.Get(EInventoryUseItemType.WarpCard), SealedCavesSirens);
			Add(new ItemKey(9, 2, 104, 160), ItemInfo.Get(EInventoryRelicType.WaterMask), SealedCavesSirens);
			//Militairy Fortress
			Add(new ItemKey(10, 3, 264, 128), ItemInfo.Get(EItemType.MaxSand), MilitairyFortress & DoubleJumpOfNpc);
			Add(new ItemKey(10, 11, 296, 192), ItemInfo.Get(EItemType.MaxAura), MilitairyFortress);
			Add(new ItemKey(10, 4, 1064, 176), ItemInfo.Get(EInventoryUseItemType.FutureHiPotion), MilitairyFortressHangar);
			Add(new ItemKey(10, 10, 104, 192), ItemInfo.Get(EInventoryRelicType.AirMask), MilitairyFortressHangar);
			Add(new ItemKey(10, 8, 1080, 176), ItemInfo.Get(EInventoryEquipmentType.LabGlasses), MilitairyFortressHangar);
			Add(new ItemKey(10, 7, 104, 192), ItemInfo.Get(EInventoryUseItemType.PlasmaIV), RightSideMilitairyFortressHangar & R.CardB);
			Add(new ItemKey(10, 7, 152, 192), ItemInfo.Get(EItemType.MaxSand), RightSideMilitairyFortressHangar & R.CardB);
			Add(new ItemKey(10, 18, 280, 189), ItemInfo.Get(EInventoryOrbType.Gun, EOrbSlot.Melee), RightSideMilitairyFortressHangar & (DoubleJumpOfNpc | ForwardDashDoubleJump));
			// The lab
			Add(new ItemKey(11, 36, 312, 192), ItemInfo.Get(EInventoryUseItemType.FoodSynth), TheLab);
			Add(new ItemKey(11, 3, 1528, 192), ItemInfo.Get(EItemType.MaxHP), TheLab & R.DoubleJump);
			Add(new ItemKey(11, 3, 72, 192), ItemInfo.Get(EInventoryUseItemType.FuturePotion), TheLab & R.UpwardDash); //when lab power is only, it only requires DoubleJumpOfNpc, but we cant code for the power state
			Add(new ItemKey(11, 25, 104, 192), ItemInfo.Get(EItemType.MaxAura), TheLab & R.DoubleJump);
			Add(new ItemKey(11, 18, 824, 128), ItemInfo.Get(EInventoryUseItemType.ChaosHeal), TheLabPoweredOff);
			Add(new RoomItemKey(11, 39), ItemInfo.Get(EInventoryOrbType.Eye, EOrbSlot.Melee), TheLabPoweredOff);
			Add(new RoomItemKey(11, 21), ItemInfo.Get(EInventoryRelicType.ScienceKeycardA), UppereLab);
			Add(new RoomItemKey(11, 1), ItemInfo.Get(EInventoryRelicType.Dash), TheLabPoweredOff);
			Add(new ItemKey(11, 6, 328, 192), ItemInfo.Get(EInventoryEquipmentType.LabCoat), UppereLab);
			Add(new ItemKey(11, 27, 296, 160), ItemInfo.Get(EItemType.MaxSand), UppereLab);
			Add(new RoomItemKey(11, 26), ItemInfo.Get(EInventoryRelicType.TimespinnerGear1), TheLabPoweredOff & R.CardA);
			//Emperors tower
			Add(new ItemKey(12, 5, 344, 192), ItemInfo.Get(EItemType.MaxAura), EmperorsTower);
			Add(new ItemKey(12, 3, 200, 160), ItemInfo.Get(EInventoryEquipmentType.LachiemCrown), EmperorsTower & R.UpwardDash);
			Add(new ItemKey(12, 25, 360, 176), ItemInfo.Get(EInventoryEquipmentType.EmpressCoat), EmperorsTower & R.UpwardDash);
			Add(new ItemKey(12, 22, 56, 192), ItemInfo.Get(EItemType.MaxSand), EmperorsTower);
			Add(new ItemKey(12, 9, 344, 928), ItemInfo.Get(EInventoryUseItemType.FutureHiEther), EmperorsTower);
			Add(new ItemKey(12, 19, 72, 192), ItemInfo.Get(EInventoryEquipmentType.FiligreeClasp), EmperorsTower & DoubleJumpOfNpc);
			Add(new ItemKey(12, 13, 120, 176), ItemInfo.Get(EItemType.MaxHP), EmperorsTower);
			Add(new ItemKey(12, 11, 264, 208), ItemInfo.Get(EInventoryRelicType.EmpireBrooch), EmperorsTower); 
			Add(new ItemKey(12, 11, 136, 205), ItemInfo.Get(EInventoryOrbType.Empire, EOrbSlot.Melee), EmperorsTower);
		}

		void AddPastItemLocations()
		{
			//Refugee Camp
			Add(new RoomItemKey(3, 0), ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Melee), AccessToPast); //neliste
			Add(new ItemKey(3, 30, 296, 176), ItemInfo.Get(EInventoryUseItemType.EssenceCrystal), AccessToPast);
			Add(new ItemKey(3, 30, 232, 176), ItemInfo.Get(EInventoryUseItemType.GoldNecklace), AccessToPast);
			Add(new ItemKey(3, 30, 168, 176), ItemInfo.Get(EInventoryRelicType.JewelryBox), AccessToPast);
			//Forest
			Add(new ItemKey(3, 3, 648, 272), ItemInfo.Get(EInventoryUseItemType.Herb), AccessToPast);
			Add(new ItemKey(3, 15, 248, 112), ItemInfo.Get(EItemType.MaxAura), AccessToPast & (DoubleJumpOfNpc | ForwardDashDoubleJump));
			Add(new ItemKey(3, 21, 120, 192), ItemInfo.Get(EItemType.MaxSand), AccessToPast);
			Add(new ItemKey(3, 12, 776, 560), ItemInfo.Get(EInventoryEquipmentType.PointyHat), AccessToPast);
			Add(new ItemKey(3, 11, 392, 608), ItemInfo.Get(EInventoryUseItemType.MagicMarbles), AccessToPast & R.Swimming);
			Add(new ItemKey(3, 5, 184, 192), ItemInfo.Get(EInventoryEquipmentType.Pendulum), AccessToPast & R.Swimming);
			Add(new ItemKey(3, 2, 584, 368), ItemInfo.Get(EInventoryUseItemType.Potion), AccessToPast);
			Add(new ItemKey(4, 20, 264, 160), ItemInfo.Get(EItemType.MaxAura), AccessToPast);
			Add(new ItemKey(3, 29, 248, 192), ItemInfo.Get(EItemType.MaxHP), LeftSideForestCaves);
			//Upper Lake Sirine
			Add(new ItemKey(7, 16, 152, 96), ItemInfo.Get(EInventoryUseItemType.MagicMarbles), UpperLakeSirine);
			Add(new ItemKey(7, 19, 248, 96), ItemInfo.Get(EItemType.MaxAura), UpperLakeSirine & R.DoubleJump);
			Add(new ItemKey(7, 19, 168, 240), ItemInfo.Get(EInventoryEquipmentType.TravelersCloak), UpperLakeSirine);
			Add(new ItemKey(7, 27, 184, 144), ItemInfo.Get(EInventoryFamiliarType.Griffin), UpperLakeSirine);
			Add(new ItemKey(7, 13, 56, 176), ItemInfo.Get(EInventoryUseItemType.WarpCard), UpperLakeSirine);
			Add(new ItemKey(7, 30, 296, 176), ItemInfo.Get(EInventoryRelicType.PyramidsKey), UpperLakeSirine);
			//Lower Lake Sirine
			Add(new ItemKey(7, 3, 440, 1232), ItemInfo.Get(EInventoryUseItemType.Potion), LowerlakeSirine);
			Add(new ItemKey(7, 7, 1432, 576), ItemInfo.Get(EInventoryUseItemType.MagicMarbles), LowerlakeSirine);
			Add(new ItemKey(7, 6, 520, 496), ItemInfo.Get(EInventoryUseItemType.Potion), LowerlakeSirine);
			Add(new ItemKey(7, 11, 88, 240), ItemInfo.Get(EItemType.MaxHP), LowerlakeSirine);
			Add(new ItemKey(7, 2, 1016, 384), ItemInfo.Get(EInventoryUseItemType.Ether), LowerlakeSirine);
			Add(new ItemKey(7, 20, 248, 96), ItemInfo.Get(EItemType.MaxSand), LowerlakeSirine);
			Add(new ItemKey(7, 9, 584, 189), ItemInfo.Get(EInventoryOrbType.Ice, EOrbSlot.Melee), LowerlakeSirine);
			//Caves of Banishment
			Add(new ItemKey(8, 19, 664, 704), ItemInfo.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment & R.DoubleJump);
			Add(new ItemKey(8, 12, 280, 160), ItemInfo.Get(EItemType.MaxHP), LowerCavesOfBanishment);
			Add(new ItemKey(8, 48, 104, 160), ItemInfo.Get(EInventoryUseItemType.Herb), LowerCavesOfBanishment);
			Add(new ItemKey(8, 39, 88, 192), ItemInfo.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment);
			Add(new ItemKey(8, 41, 168, 192), ItemInfo.Get(EInventoryUseItemType.GoldNecklace), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 216, 192), ItemInfo.Get(EInventoryUseItemType.GoldRing), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 264, 192), ItemInfo.Get(EInventoryUseItemType.EssenceCrystal), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 312, 192), ItemInfo.Get(EInventoryUseItemType.MagicMarbles), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 42, 216, 189), ItemInfo.Get(EInventoryOrbType.Wind, EOrbSlot.Melee), LowerCavesOfBanishment);
			Add(new ItemKey(8, 15, 248, 192), ItemInfo.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment & R.DoubleJump);
			Add(new ItemKey(8, 31, 88, 400), ItemInfo.Get(EInventoryUseItemType.MagicMarbles), LowerCavesOfBanishment & R.DoubleJump);
			//Caves of banishment (sirens)
			Add(new ItemKey(8, 4, 664, 144), ItemInfo.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 808, 144), ItemInfo.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 744, 560), ItemInfo.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 3, 1848, 576), ItemInfo.Get(EItemType.MaxAura), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 5, 88, 496), ItemInfo.Get(EItemType.MaxSand), UpperCavesOfBanishment & R.Swimming);
			//Caste Ramparts
			Add(new ItemKey(4, 1, 456, 160), ItemInfo.Get(EItemType.MaxSand), CastleRamparts & MultipleSmallJumpsOfNpc);
			Add(new ItemKey(4, 3, 136, 144), ItemInfo.Get(EItemType.MaxHP), CastleRamparts & R.TimeStop);
			Add(new ItemKey(4, 10, 56, 192), ItemInfo.Get(EInventoryUseItemType.HiPotion), CastleRamparts);
			Add(new ItemKey(4, 11, 344, 192), ItemInfo.Get(EInventoryUseItemType.HiPotion), CastleRamparts);
			Add(new ItemKey(4, 22, 104, 189), ItemInfo.Get(EInventoryOrbType.Iron, EOrbSlot.Melee), CastleRamparts);
			//Caste Keep
			Add(new ItemKey(5, 9, 104, 189), ItemInfo.Get(EInventoryOrbType.Blood, EOrbSlot.Melee), CastleKeep);
			Add(new ItemKey(5, 10, 104, 192), ItemInfo.Get(EInventoryFamiliarType.Sprite), CastleKeep);
			Add(new ItemKey(5, 14, 88, 208), ItemInfo.Get(EInventoryUseItemType.MagicMarbles), CastleKeep & R.PinkOrb & R.DoubleJump);
			Add(new ItemKey(5, 44, 216, 192), ItemInfo.Get(EInventoryUseItemType.Potion), CastleKeep);
			Add(new ItemKey(5, 45, 104, 192), ItemInfo.Get(EItemType.MaxHP), CastleKeep);
			Add(new ItemKey(5, 15, 296, 192), ItemInfo.Get(EItemType.MaxAura), CastleKeep);
			Add(new ItemKey(5, 41, 72, 160), ItemInfo.Get(EInventoryEquipmentType.BuckleHat), CastleKeep);
			Add(new RoomItemKey(5, 5), ItemInfo.Get(EInventoryRelicType.DoubleJump), CastleKeep & R.TimeStop); //sucabus
			Add(new ItemKey(5, 22, 312, 176), ItemInfo.Get(EItemType.MaxSand), CastleKeep & ForwardDashDoubleJump);
			//Royal towers
			Add(new ItemKey(6, 19, 200, 176), ItemInfo.Get(EItemType.MaxAura), RoyalTower & R.DoubleJump);
			Add(new ItemKey(6, 27, 472, 384), ItemInfo.Get(EInventoryUseItemType.MagicMarbles), MidRoyalTower);
			Add(new ItemKey(6, 1, 1512, 288), ItemInfo.Get(EInventoryUseItemType.Potion), UpperRoyalTower);
			Add(new ItemKey(6, 25, 360, 176), ItemInfo.Get(EInventoryUseItemType.HiEther), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 3, 120, 208), ItemInfo.Get(EInventoryFamiliarType.Demon), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 17, 200, 112), ItemInfo.Get(EItemType.MaxHP), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 17, 56, 448), ItemInfo.Get(EInventoryEquipmentType.VileteCrown), UpperRoyalTower);
			Add(new ItemKey(6, 17, 360, 1840), ItemInfo.Get(EInventoryEquipmentType.MidnightCloak), MidRoyalTower);
			Add(new ItemKey(6, 13, 120, 176), ItemInfo.Get(EItemType.MaxSand), UpperRoyalTower);
			Add(new ItemKey(6, 22, 88, 208), ItemInfo.Get(EInventoryUseItemType.Ether), UpperRoyalTower);
			Add(new ItemKey(6, 11, 360, 544), ItemInfo.Get(EInventoryUseItemType.HiPotion), UpperRoyalTower);
			Add(new ItemKey(6, 23, 856, 208), ItemInfo.Get(EInventoryEquipmentType.VileteDress), UpperRoyalTower & R.UpwardDash);
			Add(new ItemKey(6, 14, 136, 208), ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Melee), UpperRoyalTower);
			Add(new ItemKey(6, 14, 184, 205), ItemInfo.Get(EInventoryUseItemType.WarpCard), UpperRoyalTower);
		}

		void AddPyramidItemLocations()
		{
			//ancient pyramid
			Add(new ItemKey(16, 14, 312, 192), ItemInfo.Get(EItemType.MaxSand), LeftPyramid);
			Add(new ItemKey(16, 3, 88, 192), ItemInfo.Get(EItemType.MaxHP), LeftPyramid);
			Add(new ItemKey(16, 22, 200, 192), ItemInfo.Get(EItemType.MaxAura), Nightmare); //only requires  to rach but Nightmate to escape LeftPyramid
			Add(new ItemKey(16, 16, 1512, 144), ItemInfo.Get(EInventoryRelicType.EssenceOfSpace), Nightmare); //only requires  to rach but Nightmate to escape LeftPyramid
																											  //Add(new ItemKey(16, 5, 136, 192), ItemInfo.Get(EInventoryRelicType.EternalBrooch), LeftPyramid); //Post nightmare

			//temporal gyre
			/*var challengeDungion = Nightmare;
			Add(new ItemKey(14, 14, 200, 832), ItemInfo.Dummy, challengeDungion); //transition chest 1
			Add(new ItemKey(14, 17, 200, 832), ItemInfo.Dummy, challengeDungion); //transition chest 2
			Add(new ItemKey(14, 20, 200, 832), ItemInfo.Dummy, challengeDungion); //transition chest 3
			Add(new ItemKey(14, 8, 120, 176), ItemInfo.Dummy, challengeDungion); //Ravenlord pre fight
			Add(new ItemKey(14, 9, 280, 176), ItemInfo.Dummy, challengeDungion); //Ravenlord post fight
			Add(new ItemKey(14, 6, 40, 208), ItemInfo.Dummy, challengeDungion); //ifrid pre fight
			Add(new ItemKey(14, 7, 280, 208), ItemInfo.Dummy, challengeDungion); //ifrid post fight*/
		}

		ItemLocation GetItemLocationBasedOnKeyOrRoomKey(ItemKey key)
		{
			return TryGetValue(key, out var itemLocation)
				? itemLocation
				: TryGetValue(key.ToRoomItemKey(), out var roomItemLocation)
					? roomItemLocation
					: null;
		}

		public bool IsBeatable()
		{
			//gassmask may never be placed in a gass effected place
			//the verry basics to reach maw shouldd also allow you to get gassmask
			var gassmarkLocation = this.First(l => l.ItemInfo == ItemInfo.Get(EInventoryRelicType.AirMask));
			if (gassmarkLocation.Key.LevelId == 1 || !gassmarkLocation.Gate.CanBeOpenedWith(
				    R.DoubleJump | R.GateAccessToPast | R.Swimming)) 
				return false;

			var obtainedRequirements = R.None;
			var itteration = 0;

			do
			{
				itteration++;
				var previusObtainedRequirements = obtainedRequirements;

				obtainedRequirements = GetReachableLocations(obtainedRequirements)
					.Select(l => l.Unlocks)
					.Aggregate(R.None, (current, unlock) => current | unlock);

				if (obtainedRequirements == previusObtainedRequirements)
					return false;

			} while (!CanCompleteGame(obtainedRequirements) && itteration <= ItemUnlockingMap.ProgressionItemCount);

			return true;
		}

		public IEnumerable<ItemLocation> GetReachableLocations(R obtainedRequirements)
		{
			return this.Where(l => l.Gate.CanBeOpenedWith(obtainedRequirements));
		}

		static bool CanCompleteGame(R obtainedRequirements)
		{
			return Nightmare.CanBeOpenedWith(obtainedRequirements);
		}

		public void BaseOnSave(GameSave gameSave)
		{
			foreach (var itemLocation in this)
				itemLocation.BsseOnGameSave(gameSave);
		}

		void Add(ItemKey itemKey, ItemInfo defaultItem)
		{
			Add(new ItemLocation(itemKey, defaultItem));
		}

		void Add(ItemKey itemKey, ItemInfo defaultItem, R requirement)
		{
			Add(new ItemLocation(itemKey, defaultItem, requirement));
		}

		void Add(ItemKey itemKey, ItemInfo defaultItem, Gate gate)
		{
			Add(new ItemLocation(itemKey, defaultItem, gate));
		}
	}
}
