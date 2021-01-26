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
		internal static R OculusRift = R.None;
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
			| R.GateCastleKeep
			| (R.GateCavesOfBanishment & (R.DoubleJump | R.Swimming))
			| (R.GateMaw & R.DoubleJump);

		//past
		internal static readonly Gate LeftSideForestCaves = (AccessToPast & (R.TimespinnerWheel | R.ForwardDash | R.DoubleJump)) | R.GateLakeSirineRight | R.GateLakeSirineLeft;
		internal static readonly Gate UpperLakeSirine = (LeftSideForestCaves & (R.TimeStop | R.Swimming)) | R.GateLakeSirineLeft;
		internal static readonly Gate LowerlakeSirine = (LeftSideForestCaves | R.GateLakeSirineLeft) & R.Swimming;
		internal static readonly Gate LowerCavesOfBanishment = LowerlakeSirine | R.GateCavesOfBanishment | (R.GateMaw & R.DoubleJump);
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
		internal static readonly Gate RightSideLibraryElevator = (MidLibrary & R.CardE & (R.CardC | R.CardB)) | (R.GateMilitairyGate & R.CardE);
		internal static readonly Gate LowerRightSideLibrary = (MidLibrary & R.CardB) | RightSideLibraryElevator | R.GateMilitairyGate;
		internal static readonly R SealedCavesLeft = R.DoubleJump | R.GateSealedCaves;
		internal static readonly Gate SealedCavesLower = SealedCavesLeft & R.CardA;
		internal static readonly Gate SealedCavesSirens = (MidLibrary & R.CardB & R.CardE) | R.GateSealedSirensCave;
		internal static readonly Gate KillMaw = R.DoubleJump & (LowerlakeSirine | R.GateCavesOfBanishment | R.GateMaw);
		internal static readonly Gate KillTwins = CastleKeep & R.TimeStop;
		internal static readonly Gate KillTwinsAndMaw = KillMaw & KillTwins;
		internal static readonly Gate KillAll3MajorBosses = LowerRightSideLibrary & KillTwinsAndMaw & UpperRoyalTower;
		internal static readonly Gate MilitairyFortress = KillAll3MajorBosses;
		internal static readonly Gate MilitairyFortressHangar = MilitairyFortress;
		internal static readonly Gate RightSideMilitairyFortressHangar = MilitairyFortressHangar & R.DoubleJump;
		internal static readonly Gate TheLab = MilitairyFortressHangar & R.CardB;
		internal static readonly Gate TheLabPoweredOff = TheLab & DoubleJumpOfNpc;
		internal static readonly Gate UpperLab = TheLabPoweredOff & ForwardDashDoubleJump;
		internal static readonly Gate EmperorsTower = UpperLab;

		//pyramid
		internal static readonly Gate LeftPyramid = UpperLab & (
			R.TimespinnerWheel & R.TimespinnerSpindle &
			R.TimespinnerPiece1 & R.TimespinnerPiece2 & R.TimespinnerPiece3);
		internal static readonly Gate Nightmare = LeftPyramid & R.UpwardDash;

		public new ItemLocation this[ItemKey key] => GetItemLocationBasedOnKeyOrRoomKey(key);

		readonly ItemInfoProvider itemProvider;
		readonly ItemUnlockingMap unlockingMap;

		string areaName;

		public ItemLocationMap(ItemInfoProvider itemInfoProvider, ItemUnlockingMap itemUnlockingMap, SeedOptions options) 
			: base(CalculateCapacity(options), l => l.Key)
		{
			itemProvider = itemInfoProvider;
			unlockingMap = itemUnlockingMap;

			if (options.RequireEyeOrbRing)
				OculusRift = R.OculusRift;

			AddPresentItemLocations();
			AddPastItemLocations();
			AddPyramidItemLocations();

			if (options.DownloadableItems)
				AddDownloadTerminals();
		}

		static int CalculateCapacity(SeedOptions options)
		{
			var capacity = 160;

			if (options.DownloadableItems)
				capacity += 14;

			return capacity;
		}

		void AddPresentItemLocations()
		{
			areaName = "Tutorial";
			Add(ItemKey.TutorialMeleeOrb, "Yo Momma", itemProvider.Get(EInventoryOrbType.Blue, EOrbSlot.Melee));
			Add(ItemKey.TutorialSpellOrb, "Yo Momma", itemProvider.Get(EInventoryOrbType.Blue, EOrbSlot.Spell));
			areaName = "Lake desolation";
			Add(new ItemKey(1, 1, 1528, 144), "Starter chest 2", itemProvider.Get(EInventoryUseItemType.FuturePotion));
			Add(new ItemKey(1, 15, 264, 144), "Starter chest 3", itemProvider.Get(EInventoryEquipmentType.OldCoat));
			Add(new ItemKey(1, 25, 296, 176), "Starter chest 1", itemProvider.Get(EInventoryUseItemType.FutureHiPotion));
			Add(new ItemKey(1, 9, 600, 144 + TimespinnerWheel.YOffset), "Timespinner Wheel room", itemProvider.Get(EInventoryRelicType.TimespinnerWheel));
			Add(new ItemKey(1, 14, 40, 176), "Forget me not chest", itemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation);
			areaName = "Lower lake desolation";
			Add(new ItemKey(1, 2, 1016, 384), "Chicken chest", itemProvider.Get(EItemType.MaxSand), R.TimeStop);
			Add(new ItemKey(1, 11, 72, 240), "Not so secret room", itemProvider.Get(EItemType.MaxHP), LowerLakeDesolationBridge & OculusRift);
			Add(new ItemKey(1, 3, 56, 176), "Tank chest", itemProvider.Get(EItemType.MaxAura), R.TimeStop);
			areaName = "Upper lake desolation";
			Add(new ItemKey(1, 17, 152, 96), null, itemProvider.Get(EInventoryUseItemType.GoldRing), UpperLakeDesolation);
			Add(new ItemKey(1, 21, 200, 144), null, itemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation & OculusRift);
			Add(new ItemKey(1, 20, 232, 96), "Double jump cave floor", itemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLakeDesolation & R.DoubleJump);
			Add(new ItemKey(1, 20, 168, 240), "Double jump cave platform", itemProvider.Get(EInventoryUseItemType.FuturePotion), UpperLakeDesolation);
			Add(new ItemKey(1, 22, 344, 160), "Fire-Locked sparrow chest", itemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1320, 189), "Crash site pedestal", itemProvider.Get(EInventoryOrbType.Moon, EOrbSlot.Melee), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1272, 192), "Crash site chest 1", itemProvider.Get(EInventoryEquipmentType.CaptainsCap), UpperLakeDesolation & R.GassMask & KillTwinsAndMaw);
			Add(new ItemKey(1, 18, 1368, 192), "Crash site chest 2", itemProvider.Get(EInventoryEquipmentType.CaptainsJacket), UpperLakeDesolation & R.GassMask & KillTwinsAndMaw);
			Add(new RoomItemKey(1, 5), "Kitty Boss", itemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Melee), UpperLakeDesolation | LowerLakeDesolationBridge);
			areaName = "Libary";
			Add(new ItemKey(2, 60, 328, 160), "Basement", itemProvider.Get(EItemType.MaxHP), LeftLibrary);
			Add(new ItemKey(2, 54, 296, 176), "Consolation", itemProvider.Get(EInventoryRelicType.ScienceKeycardD), LeftLibrary);
			Add(new ItemKey(2, 41, 404, 246), "Librarian", itemProvider.Get(EInventoryRelicType.Tablet), LeftLibrary);
			Add(new ItemKey(2, 44, 680, 368), "Reading nook chest", itemProvider.Get(EInventoryRelicType.FoeScanner), LeftLibrary);
			Add(new ItemKey(2, 47, 216, 208), "Storage room chest 1", itemProvider.Get(EInventoryUseItemType.Ether), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 152, 208), "Storage room chest 2", itemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Passive), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 88, 208), "Storage room chest 3", itemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Spell), LeftLibrary & R.CardD);
			areaName = "libary top";
			Add(new ItemKey(2, 56, 168, 192), "Backer room chest 5", itemProvider.Get(EInventoryUseItemType.GoldNecklace), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 392, 192), "Backer room chest 4", itemProvider.Get(EInventoryUseItemType.GoldRing), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 616, 192), "Backer room chest 3", itemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 840, 192), "Backer room chest 2", itemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 1064, 192), "Backer room chest 1", itemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLeftLibrary);
			areaName = "Varndagroth tower left";
			Add(new ItemKey(2, 34, 232, 1200), null, itemProvider.Get(EInventoryUseItemType.Jerky), MidLibrary);
			Add(new ItemKey(2, 40, 344, 176), "Ye olde Timespinner", itemProvider.Get(EInventoryRelicType.ScienceKeycardC), MidLibrary);
			Add(new ItemKey(2, 32, 328, 160), null, itemProvider.Get(EInventoryUseItemType.GoldRing), MidLibrary & R.CardC);
			Add(new ItemKey(2, 7, 232, 144), "Left air vents secret", itemProvider.Get(EItemType.MaxAura), MidLibrary & OculusRift);
			Add(new ItemKey(2, 25, 328, 192), "Left elevator chest", itemProvider.Get(EItemType.MaxSand), MidLibrary & R.CardE);
			areaName = "Varndagroth tower right";
			Add(new ItemKey(2, 15, 760, 192), "Spider heck room", itemProvider.Get(EInventoryUseItemType.FuturePotion), UpperRightSideLibrary);
			Add(new ItemKey(2, 20, 72, 1200), "Right elevator chest", itemProvider.Get(EInventoryUseItemType.Jerky), RightSideLibraryElevator);
			Add(new ItemKey(2, 23, 72, 560), "Elevator card chest", itemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 1112, 112), "Air vents left", itemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 136, 304), "Air Vents right", itemProvider.Get(EInventoryRelicType.ElevatorKeycard), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 11, 104, 192), "Right side bottom floor", itemProvider.Get(EInventoryUseItemType.EssenceCrystal), LowerRightSideLibrary);
			Add(new ItemKey(2, 29, 280, 222 + TimespinnerSpindle.YOffset), "Varndagroth", itemProvider.Get(EInventoryRelicType.TimespinnerSpindle), RightSideLibraryElevator);
			Add(new RoomItemKey(2, 52), "Spider hell", itemProvider.Get(EInventoryRelicType.TimespinnerGear2), RightSideLibraryElevator & R.CardA);
			areaName = "Sealed Caves (Xarion)";
			Add(new ItemKey(9, 10, 248, 848), "Skeleton", itemProvider.Get(EInventoryRelicType.ScienceKeycardB), SealedCavesLeft);
			Add(new ItemKey(9, 19, 664, 704), "Shroom jump room", itemProvider.Get(EInventoryUseItemType.Antidote), SealedCavesLower & R.TimeStop);
			Add(new ItemKey(9, 39, 88, 192), "Double shroom room", itemProvider.Get(EInventoryUseItemType.Antidote), SealedCavesLower);
			Add(new ItemKey(9, 41, 312, 192), "Mini jackpot room", itemProvider.Get(EInventoryUseItemType.GalaxyStone), SealedCavesLower & ForwardDashDoubleJump);
			Add(new ItemKey(9, 42, 328, 192), "Below mini jackpot room", itemProvider.Get(EInventoryUseItemType.MagicMarbles), SealedCavesLower);
			Add(new ItemKey(9, 12, 280, 160), "Sealed cave secret room", itemProvider.Get(EItemType.MaxHP), SealedCavesLower & OculusRift);
			Add(new ItemKey(9, 48, 104, 160), null, itemProvider.Get(EInventoryUseItemType.FutureEther), SealedCavesLower);
			Add(new ItemKey(9, 15, 248, 192), "Top right of H", itemProvider.Get(EInventoryUseItemType.FutureEther), SealedCavesLower & R.DoubleJump);
			Add(new RoomItemKey(9, 13), "Xarion", itemProvider.Get(EInventoryRelicType.TimespinnerGear3), SealedCavesLower);
			areaName = "Sealed Caves (Sirens)";
			Add(new ItemKey(9, 5, 88, 496), "Solo siren chest", itemProvider.Get(EItemType.MaxSand), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 3, 1848, 576), "Big siren room right", itemProvider.Get(EInventoryEquipmentType.BirdStatue), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 3, 744, 560), "Big siren Room left", itemProvider.Get(EItemType.MaxAura), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 2, 184, 176), "Room after sirens chest 2", itemProvider.Get(EInventoryUseItemType.WarpCard), SealedCavesSirens);
			Add(new ItemKey(9, 2, 104, 160), "Room after sirens chest 1", itemProvider.Get(EInventoryRelicType.WaterMask), SealedCavesSirens);
			areaName = "Militairy Fortress";
			Add(new ItemKey(10, 3, 264, 128), "Bomber chest", itemProvider.Get(EItemType.MaxSand), MilitairyFortress & DoubleJumpOfNpc & R.TimespinnerWheel); //can be reached with just upward dash but not with lightwall unless you got timestop
			Add(new ItemKey(10, 11, 296, 192), "Close combat room", itemProvider.Get(EItemType.MaxAura), MilitairyFortress);
			Add(new ItemKey(10, 4, 1064, 176), "Bridge full of soldiers", itemProvider.Get(EInventoryUseItemType.FutureHiPotion), MilitairyFortressHangar);
			Add(new ItemKey(10, 10, 104, 192), "Giantess Room", itemProvider.Get(EInventoryRelicType.AirMask), MilitairyFortressHangar);
			Add(new ItemKey(10, 8, 1080, 176), "Bridge with Giantess", itemProvider.Get(EInventoryEquipmentType.LabGlasses), MilitairyFortressHangar);
			Add(new ItemKey(10, 7, 104, 192), "Military B door chest 2", itemProvider.Get(EInventoryUseItemType.PlasmaIV), RightSideMilitairyFortressHangar & R.CardB);
			Add(new ItemKey(10, 7, 152, 192), "Military B door chest 1", itemProvider.Get(EItemType.MaxSand), RightSideMilitairyFortressHangar & R.CardB);
			Add(new ItemKey(10, 18, 280, 189), "Gun orb pedistal", itemProvider.Get(EInventoryOrbType.Gun, EOrbSlot.Melee), RightSideMilitairyFortressHangar & (DoubleJumpOfNpc | ForwardDashDoubleJump));
			areaName = "The lab";
			Add(new ItemKey(11, 36, 312, 192), "Coffee Break chest", itemProvider.Get(EInventoryUseItemType.FoodSynth), TheLab);
			Add(new ItemKey(11, 3, 1528, 192), "Lower trash right", itemProvider.Get(EItemType.MaxHP), TheLab & R.DoubleJump);
			Add(new ItemKey(11, 3, 72, 192), "Lower trash left", itemProvider.Get(EInventoryUseItemType.FuturePotion), TheLab & R.UpwardDash); //when lab power is only, it only requires DoubleJumpOfNpc, but we cant code for the power state
			Add(new ItemKey(11, 25, 104, 192), "Single turret room", itemProvider.Get(EItemType.MaxAura), TheLab & R.DoubleJump);
			Add(new ItemKey(11, 18, 824, 128), "Trash jump room", itemProvider.Get(EInventoryUseItemType.ChaosHeal), TheLabPoweredOff);
			Add(new RoomItemKey(11, 39), "Dynamo Works", itemProvider.Get(EInventoryOrbType.Eye, EOrbSlot.Melee), TheLabPoweredOff);
			Add(new RoomItemKey(11, 21), "Blob mom", itemProvider.Get(EInventoryRelicType.ScienceKeycardA), UpperLab);
			Add(new RoomItemKey(11, 1), "Experiment #13", itemProvider.Get(EInventoryRelicType.Dash), TheLabPoweredOff);
			Add(new ItemKey(11, 6, 328, 192), null, itemProvider.Get(EInventoryEquipmentType.LabCoat), UpperLab);
			Add(new ItemKey(11, 27, 296, 160), "Lab secret", itemProvider.Get(EItemType.MaxSand), UpperLab & OculusRift);
			Add(new RoomItemKey(11, 26), "Spider hell", itemProvider.Get(EInventoryRelicType.TimespinnerGear1), TheLabPoweredOff & R.CardA);
			areaName = "Emperors tower";
			Add(new ItemKey(12, 5, 344, 192), "After Courtyard - Bottom", itemProvider.Get(EItemType.MaxAura), EmperorsTower);
			Add(new ItemKey(12, 3, 200, 160), "After Courtyard Floor Secret", itemProvider.Get(EInventoryEquipmentType.LachiemCrown), EmperorsTower & R.UpwardDash & OculusRift);
			Add(new ItemKey(12, 25, 360, 176), "After Courtyard - Top", itemProvider.Get(EInventoryEquipmentType.EmpressCoat), EmperorsTower & R.UpwardDash);
			Add(new ItemKey(12, 22, 56, 192), "Galactic Sage Room", itemProvider.Get(EItemType.MaxSand), EmperorsTower);
			Add(new ItemKey(12, 9, 344, 928), "Bottom of Right Tower", itemProvider.Get(EInventoryUseItemType.FutureHiEther), EmperorsTower);
			Add(new ItemKey(12, 19, 72, 192), "Wayyyy up there", itemProvider.Get(EInventoryEquipmentType.FiligreeClasp), EmperorsTower & DoubleJumpOfNpc);
			Add(new ItemKey(12, 13, 120, 176), "Left tower balcony", itemProvider.Get(EItemType.MaxHP), EmperorsTower);
			Add(new ItemKey(12, 11, 264, 208), "Dad's Chambers chest", itemProvider.Get(EInventoryRelicType.EmpireBrooch), EmperorsTower); 
			//TODO: Fix my crashes
			//Add(new ItemKey(12, 11, 136, 205), "Dad's Chambers pedistal", itemProvider.Get(EInventoryOrbType.Empire, EOrbSlot.Melee), EmperorsTower);
		}

		void AddPastItemLocations()
		{
			areaName = "Refugee Camp";
			Add(new RoomItemKey(3, 0), "Neliste's Bra", itemProvider.Get(EInventoryOrbType.Flame, EOrbSlot.Melee), AccessToPast); //neliste
			Add(new ItemKey(3, 30, 296, 176), "Refugee camp storage chest 3", itemProvider.Get(EInventoryUseItemType.EssenceCrystal), AccessToPast);
			Add(new ItemKey(3, 30, 232, 176), "Refugee camp storage chest 2", itemProvider.Get(EInventoryUseItemType.GoldNecklace), AccessToPast);
			Add(new ItemKey(3, 30, 168, 176), "Refugee camp storage chest 1", itemProvider.Get(EInventoryRelicType.JewelryBox), AccessToPast);
			areaName = "Forest";
			Add(new ItemKey(3, 3, 648, 272), "Refugee camp roof", itemProvider.Get(EInventoryUseItemType.Herb), AccessToPast);
			Add(new ItemKey(3, 15, 248, 112), "Bat jump chest", itemProvider.Get(EItemType.MaxAura), AccessToPast & (DoubleJumpOfNpc | ForwardDashDoubleJump));
			Add(new ItemKey(3, 21, 120, 192), "Green platform secret", itemProvider.Get(EItemType.MaxSand), AccessToPast & OculusRift);
			Add(new ItemKey(3, 12, 776, 560), null, itemProvider.Get(EInventoryEquipmentType.PointyHat), AccessToPast);
			Add(new ItemKey(3, 11, 392, 608), "Waterfall chest 1", itemProvider.Get(EInventoryUseItemType.MagicMarbles), AccessToPast & R.Swimming);
			Add(new ItemKey(3, 5, 184, 192), "Waterfall chest 1", itemProvider.Get(EInventoryEquipmentType.Pendulum), AccessToPast & R.Swimming);
			Add(new ItemKey(3, 2, 584, 368), "Batcave", itemProvider.Get(EInventoryUseItemType.Potion), AccessToPast);
			Add(new ItemKey(4, 20, 264, 160), "Bridge Chest", itemProvider.Get(EItemType.MaxAura), AccessToPast);
			Add(new ItemKey(3, 29, 248, 192), null, itemProvider.Get(EItemType.MaxHP), LeftSideForestCaves);
			areaName = "Upper Lake Sirine";
			Add(new ItemKey(7, 16, 152, 96), null, itemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLakeSirine);
			Add(new ItemKey(7, 19, 248, 96), "Double jump cave platform", itemProvider.Get(EItemType.MaxAura), UpperLakeSirine & R.DoubleJump);
			Add(new ItemKey(7, 19, 168, 240), "Double jump cave floor", itemProvider.Get(EInventoryEquipmentType.TravelersCloak), UpperLakeSirine);
			Add(new ItemKey(7, 27, 184, 144), "West lake serene cave secret", itemProvider.Get(EInventoryFamiliarType.Griffin), UpperLakeSirine & OculusRift);
			Add(new ItemKey(7, 13, 56, 176), "Chest behind vines", itemProvider.Get(EInventoryUseItemType.WarpCard), UpperLakeSirine);
			Add(new ItemKey(7, 30, 296, 176), "Pyramid keys room", itemProvider.Get(EInventoryRelicType.PyramidsKey), UpperLakeSirine);
			areaName = "Lower Lake Sirine";
			Add(new ItemKey(7, 3, 440, 1232), null, itemProvider.Get(EInventoryUseItemType.Potion), LowerlakeSirine);
			Add(new ItemKey(7, 7, 1432, 576), null, itemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerlakeSirine);
			Add(new ItemKey(7, 6, 520, 496), null, itemProvider.Get(EInventoryUseItemType.Potion), LowerlakeSirine);
			Add(new ItemKey(7, 11, 88, 240), null, itemProvider.Get(EItemType.MaxHP), LowerlakeSirine & OculusRift);
			Add(new ItemKey(7, 2, 1016, 384), null, itemProvider.Get(EInventoryUseItemType.Ether), LowerlakeSirine);
			Add(new ItemKey(7, 20, 248, 96), null, itemProvider.Get(EItemType.MaxSand), LowerlakeSirine);
			Add(new ItemKey(7, 9, 584, 189), null, itemProvider.Get(EInventoryOrbType.Ice, EOrbSlot.Melee), LowerlakeSirine);
			areaName = "Caves of Banishment (Maw)";
			Add(new ItemKey(8, 19, 664, 704), null, itemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment & R.DoubleJump);
			Add(new ItemKey(8, 12, 280, 160), null, itemProvider.Get(EItemType.MaxHP), LowerCavesOfBanishment & OculusRift);
			Add(new ItemKey(8, 48, 104, 160), null, itemProvider.Get(EInventoryUseItemType.Herb), LowerCavesOfBanishment);
			Add(new ItemKey(8, 39, 88, 192), null, itemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment);
			Add(new ItemKey(8, 41, 168, 192), "Jackpot room chest 1", itemProvider.Get(EInventoryUseItemType.GoldNecklace), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 216, 192), "Jackpot room chest 2", itemProvider.Get(EInventoryUseItemType.GoldRing), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 264, 192), "Jackpot room chest 3", itemProvider.Get(EInventoryUseItemType.EssenceCrystal), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 312, 192), "Jackpot room chest 4", itemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 42, 216, 189), null, itemProvider.Get(EInventoryOrbType.Wind, EOrbSlot.Melee), LowerCavesOfBanishment);
			Add(new ItemKey(8, 15, 248, 192), null, itemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment & R.DoubleJump);
			Add(new ItemKey(8, 31, 88, 400), null, itemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerCavesOfBanishment & R.DoubleJump);
			areaName = "Caves of Banishment (Sirens)";
			Add(new ItemKey(8, 4, 664, 144), null, itemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 808, 144), null, itemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 744, 560), null, itemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 3, 1848, 576), null, itemProvider.Get(EItemType.MaxAura), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 5, 88, 496), null, itemProvider.Get(EItemType.MaxSand), UpperCavesOfBanishment & R.Swimming);
			areaName = "Caste Ramparts";
			Add(new ItemKey(4, 1, 456, 160), "Bomber chest", itemProvider.Get(EItemType.MaxSand), CastleRamparts & MultipleSmallJumpsOfNpc);
			Add(new ItemKey(4, 3, 136, 144), null, itemProvider.Get(EItemType.MaxHP), CastleRamparts & R.TimeStop);
			Add(new ItemKey(4, 10, 56, 192), null, itemProvider.Get(EInventoryUseItemType.HiPotion), CastleRamparts);
			Add(new ItemKey(4, 11, 344, 192), null, itemProvider.Get(EInventoryUseItemType.HiPotion), CastleRamparts);
			Add(new ItemKey(4, 22, 104, 189), null, itemProvider.Get(EInventoryOrbType.Iron, EOrbSlot.Melee), CastleRamparts);
			areaName = "Caste Keep";
			Add(new ItemKey(5, 9, 104, 189), null, itemProvider.Get(EInventoryOrbType.Blood, EOrbSlot.Melee), CastleKeep & OculusRift);
			Add(new ItemKey(5, 10, 104, 192), "Blood orb pedistal", itemProvider.Get(EInventoryFamiliarType.Sprite), CastleKeep);
			Add(new ItemKey(5, 14, 88, 208), "Yas queen room", itemProvider.Get(EInventoryUseItemType.MagicMarbles), CastleKeep & R.PinkOrb & R.DoubleJump);
			Add(new ItemKey(5, 44, 216, 192), null, itemProvider.Get(EInventoryUseItemType.Potion), CastleKeep);
			Add(new ItemKey(5, 45, 104, 192), "Omelette chest", itemProvider.Get(EItemType.MaxHP), CastleKeep);
			Add(new ItemKey(5, 15, 296, 192), null, itemProvider.Get(EItemType.MaxAura), CastleKeep);
			Add(new ItemKey(5, 41, 72, 160), null, itemProvider.Get(EInventoryEquipmentType.BuckleHat), CastleKeep);
			Add(new RoomItemKey(5, 5), "Twins", itemProvider.Get(EInventoryRelicType.DoubleJump), CastleKeep & R.TimeStop); //sucabus
			Add(new ItemKey(5, 22, 312, 176), null, itemProvider.Get(EItemType.MaxSand), CastleKeep & ForwardDashDoubleJump);
			areaName = "Royal towers";
			Add(new ItemKey(6, 19, 200, 176), null, itemProvider.Get(EItemType.MaxAura), RoyalTower & R.DoubleJump & OculusRift);
			Add(new ItemKey(6, 27, 472, 384), null, itemProvider.Get(EInventoryUseItemType.MagicMarbles), MidRoyalTower);
			Add(new ItemKey(6, 1, 1512, 288), null, itemProvider.Get(EInventoryUseItemType.Potion), MidRoyalTower);
			Add(new ItemKey(6, 25, 360, 176), null, itemProvider.Get(EInventoryUseItemType.HiEther), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 3, 120, 208), null, itemProvider.Get(EInventoryFamiliarType.Demon), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 17, 200, 112), null, itemProvider.Get(EItemType.MaxHP), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 17, 56, 448), null, itemProvider.Get(EInventoryEquipmentType.VileteCrown), UpperRoyalTower);
			Add(new ItemKey(6, 17, 360, 1840), null, itemProvider.Get(EInventoryEquipmentType.MidnightCloak), MidRoyalTower);
			Add(new ItemKey(6, 13, 120, 176), null, itemProvider.Get(EItemType.MaxSand), UpperRoyalTower);
			Add(new ItemKey(6, 22, 88, 208), null, itemProvider.Get(EInventoryUseItemType.Ether), UpperRoyalTower);
			Add(new ItemKey(6, 11, 360, 544), null, itemProvider.Get(EInventoryUseItemType.HiPotion), UpperRoyalTower);
			Add(new ItemKey(6, 23, 856, 208), "Statue room", itemProvider.Get(EInventoryEquipmentType.VileteDress), UpperRoyalTower & R.UpwardDash);
			Add(new ItemKey(6, 14, 136, 208), null, itemProvider.Get(EInventoryOrbType.Pink, EOrbSlot.Melee), UpperRoyalTower);
			Add(new ItemKey(6, 14, 184, 205), null, itemProvider.Get(EInventoryUseItemType.WarpCard), UpperRoyalTower);
		}

		void AddPyramidItemLocations()
		{
			areaName = "Ancient Pyramid";
			Add(new ItemKey(16, 14, 312, 192), "Why not it's right there", itemProvider.Get(EItemType.MaxSand), LeftPyramid);
			Add(new ItemKey(16, 3, 88, 192), null, itemProvider.Get(EItemType.MaxHP), LeftPyramid);
			Add(new ItemKey(16, 22, 200, 192), "Pit secret room", itemProvider.Get(EItemType.MaxAura), Nightmare & OculusRift); //only requires LeftPyramid to reach but Nightmate to escape
			Add(new ItemKey(16, 16, 1512, 144), "Regret chest", itemProvider.Get(EInventoryRelicType.EssenceOfSpace), Nightmare & OculusRift); //only requires LeftPyramid to reach but Nightmate to escape
			areaName = "Temporal Gyre";
			/*var challengeDungion = Nightmare;
			Add(new ItemKey(14, 14, 200, 832), ItemInfo.Dummy, challengeDungion); //transition chest 1
			Add(new ItemKey(14, 17, 200, 832), ItemInfo.Dummy, challengeDungion); //transition chest 2
			Add(new ItemKey(14, 20, 200, 832), ItemInfo.Dummy, challengeDungion); //transition chest 3
			Add(new ItemKey(14, 8, 120, 176), ItemInfo.Dummy, challengeDungion); //Ravenlord pre fight
			Add(new ItemKey(14, 9, 280, 176), ItemInfo.Dummy, challengeDungion); //Ravenlord post fight
			Add(new ItemKey(14, 6, 40, 208), ItemInfo.Dummy, challengeDungion); //ifrid pre fight
			Add(new ItemKey(14, 7, 280, 208), ItemInfo.Dummy, challengeDungion); //ifrid post fight*/
		}

		void AddDownloadTerminals()
		{
			areaName = "Libary";
			Add(new ItemKey(2, 44, 792, 592), "Library terminal 1", null, LeftLibrary & R.Tablet);
			Add(new ItemKey(2, 44, 120, 368), "Library terminal 2", null, LeftLibrary & R.Tablet);
			Add(new ItemKey(2, 44, 456, 368), "Library terminal 3", null, LeftLibrary & R.Tablet);
			Add(new ItemKey(2, 58, 152, 208), "V terminal 1", null, LeftLibrary & R.Tablet & R.CardV);
			Add(new ItemKey(2, 58, 232, 208), "V terminal 2", null, LeftLibrary & R.Tablet & R.CardV);
			Add(new ItemKey(2, 58, 312, 208), "V terminal 3", null, LeftLibrary & R.Tablet & R.CardV);
			areaName = "Libary top";
			Add(new ItemKey(2, 44, 568, 176), "Backer room terminal", null, UpperLeftLibrary & R.Tablet);
			areaName = "Varndagroth tower right";
			Add(new ItemKey(2, 18, 200, 192), "Medbay", null, RightSideLibraryElevator & R.CardB & R.Tablet);
			areaName = "The lab";
			Add(new ItemKey(11, 6, 200, 192), null, null, UpperLab & R.Tablet);
			Add(new ItemKey(11, 15, 152, 176), null, null, TheLabPoweredOff & R.Tablet);
			Add(new ItemKey(11, 16, 600, 192), null, null, TheLabPoweredOff & R.Tablet);
			Add(new ItemKey(11, 34, 200, 192), null, null, TheLab & R.Tablet);
			Add(new ItemKey(11, 37, 200, 192), null, null, TheLab & R.Tablet);
			Add(new ItemKey(11, 38, 120, 176), null, null, TheLabPoweredOff & R.Tablet);
		}

		ItemLocation GetItemLocationBasedOnKeyOrRoomKey(ItemKey key)
		{
			return TryGetValue(key, out var itemLocation)
				? itemLocation
				: TryGetValue(key.ToRoomItemKey(), out var roomItemLocation)
					? roomItemLocation
					: null;
		}

		public ProgressionChain GetProgressionChain()
		{
			var obtainedRequirements = R.None;
			IEnumerable<ItemLocation> alreadyKnownLocations = new ItemLocation[0];

			var progressionChain = new ProgressionChain();
			var currentProgressionChain = progressionChain;

			do
			{
				var previusObtainedRequirements = obtainedRequirements;

				var reachableProgressionItemLocations = GetReachableProgressionItemLocatioins(obtainedRequirements);
				obtainedRequirements = GetObtainedRequirements(reachableProgressionItemLocations);

				currentProgressionChain.Sub =
					new ProgressionChain { Locations = reachableProgressionItemLocations.Except(alreadyKnownLocations) };

				currentProgressionChain = currentProgressionChain.Sub;
				alreadyKnownLocations = reachableProgressionItemLocations;

				if (obtainedRequirements == previusObtainedRequirements)
					break;

			} while (true);

			return progressionChain.Sub;
		}

		ItemLocation[] GetReachableProgressionItemLocatioins(R obtainedRequirements)
		{
			return GetReachableLocations(obtainedRequirements)
				.Where(l => l.ItemInfo != null && l.ItemInfo.Unlocks != R.None)
				.ToArray();
		}

		public bool IsBeatable()
		{
			if (!IsGassMaskReachableWithTheMawRequirements()
				|| ProgressiveItemsOfTheSameTypeAreInTheSameRoom()) 
				return false;

			var obtainedRequirements = R.None;

			do
			{
				var previusObtainedRequirements = obtainedRequirements;

				obtainedRequirements = GetObtainedRequirements(obtainedRequirements);

				if (obtainedRequirements == previusObtainedRequirements)
					return false;

			} while (!CanCompleteGame(obtainedRequirements));

			return true;
		}

		bool ProgressiveItemsOfTheSameTypeAreInTheSameRoom()
		{
			var progressiveItemLocationsPerType = this
				.Where(l => l.ItemInfo is PogRessiveItemInfo)
				.GroupBy(l => l.ItemInfo);

			return progressiveItemLocationsPerType.Any(
				progressiveItemLocationPerType => progressiveItemLocationPerType.Any(
					progressiveItemLocation => progressiveItemLocationPerType.Any(
						p => p.Key != progressiveItemLocation.Key && p.Key.ToRoomItemKey() == progressiveItemLocation.Key.ToRoomItemKey())));
		}

		bool IsGassMaskReachableWithTheMawRequirements()
		{
			//gassmask may never be placed in a gass effected place
			//the very basics to reach maw shouldd also allow you to get gassmask
			var gassmaskLocation = this.First(l => l.ItemInfo?.Identifier == new ItemIdentifier(EInventoryRelicType.AirMask));

			var isWatermaskRequiredForMaw = unlockingMap.PyramidKeysUnlock != R.GateMaw 
			                                && unlockingMap.PyramidKeysUnlock != R.GateCavesOfBanishment;

			var gassmaskRequirements = R.DoubleJump | R.GateAccessToPast;

			if (isWatermaskRequiredForMaw)
				gassmaskRequirements |= R.Swimming;

			return gassmaskLocation.Key.LevelId != 1 && gassmaskLocation.Gate.CanBeOpenedWith(gassmaskRequirements);
		}

		R GetObtainedRequirements(R obtainedRequirements)
		{
			var reachableLocations = GetReachableLocations(obtainedRequirements)
				.Where(l => l.ItemInfo != null)
				.ToArray();

			var unlockedRequirements = reachableLocations
				.Where(l => !(l.ItemInfo is PogRessiveItemInfo))
				.Select(l => l.ItemInfo.Unlocks)
				.Aggregate(R.None, (current, unlock) => current | unlock);

			var progressiveItemsPerType = reachableLocations
				.Where(l => l.ItemInfo is PogRessiveItemInfo)
				.GroupBy(l => l.ItemInfo as PogRessiveItemInfo);

			foreach (var progressiveItemsType in progressiveItemsPerType)
			{
				var progressiveItem = progressiveItemsType.Key;
				var clone = progressiveItem.Clone();

				for (var i = 0; i < progressiveItemsType.Count(); i++)
				{
					unlockedRequirements |= clone.Unlocks;

					clone.Next();
				}
			}

			return unlockedRequirements;
		}

		R GetObtainedRequirements(ItemLocation[] reachableLocations)
		{
			var unlockedRequirements = reachableLocations
				.Where(l => !(l.ItemInfo is PogRessiveItemInfo))
				.Select(l => l.ItemInfo.Unlocks)
				.Aggregate(R.None, (current, unlock) => current | unlock);

			var progressiveItemsPerType = reachableLocations
				.Where(l => l.ItemInfo is PogRessiveItemInfo)
				.GroupBy(l => l.ItemInfo as PogRessiveItemInfo);

			foreach (var progressiveItemsType in progressiveItemsPerType)
			{
				var progressiveItem = progressiveItemsType.Key;
				var clone = progressiveItem.Clone();

				for (var i = 0; i < progressiveItemsType.Count(); i++)
				{
					unlockedRequirements |= clone.Unlocks;

					clone.Next();
				}
			}

			return unlockedRequirements;
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
			var progressiveItemInfos = this
				.Where(l => l.ItemInfo is PogRessiveItemInfo)
				.Select(l => (PogRessiveItemInfo)l.ItemInfo);

			foreach (var progressiveItem in progressiveItemInfos)
				progressiveItem.Reset();

			foreach (var itemLocation in this)
				itemLocation.BsseOnGameSave(gameSave);
		}

		void Add(ItemKey itemKey, string name, ItemInfo defaultItem)
		{
			Add(new ItemLocation(itemKey, areaName, name, defaultItem));
		}

		void Add(ItemKey itemKey, string name, ItemInfo defaultItem, R requirement)
		{
			Add(new ItemLocation(itemKey, areaName, name, defaultItem, requirement));
		}

		void Add(ItemKey itemKey, string name, ItemInfo defaultItem, Gate gate)
		{
			Add(new ItemLocation(itemKey, areaName, name, defaultItem, gate));
		}
	}

	class ProgressionChain
	{
		public IEnumerable<ItemLocation> Locations { get; set; }
		public ProgressionChain Sub { get; set; }
	}
}
