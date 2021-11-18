using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
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
		internal R OculusRift;
		internal R MawGassMask;

		internal Gate AccessToPast;
		internal Gate AccessToLakeDesolation;

		internal Gate MultipleSmallJumpsOfNpc;
		internal Gate DoubleJumpOfNpc;
		internal Gate ForwardDashDoubleJump;
		internal Gate LowerLakeDesolationBridge;

		//past
		internal Gate LeftSideForestCaves;
		internal Gate UpperLakeSirine;
		internal Gate LowerlakeSirine;
		internal Gate LowerCavesOfBanishment;
		internal Gate UpperCavesOfBanishment;
		internal Gate CastleRamparts;
		internal Gate CastleKeep;
		internal Gate RoyalTower;
		internal Gate MidRoyalTower;
		internal Gate UpperRoyalTower;
		internal Gate KillMaw;
		//future
		internal Gate UpperLakeDesolation;
		internal Gate LeftLibrary;
		internal Gate UpperLeftLibrary;
		internal Gate MidLibrary;
		internal Gate UpperRightSideLibrary;
		internal Gate RightSideLibraryElevator;
		internal Gate LowerRightSideLibrary;
		internal Gate SealedCavesLeft;
		internal Gate SealedCavesLower;
		internal Gate SealedCavesSirens;
		internal Gate MilitairyFortress;
		internal Gate MilitairyFortressHangar;
		internal Gate RightSideMilitairyFortressHangar;
		internal Gate TheLab;
		internal Gate TheLabPoweredOff;
		internal Gate UpperLab;
		internal Gate EmperorsTower;
		//pyramid
		internal Gate LeftPyramid;
		internal Gate Nightmare;

		public new ItemLocation this[ItemKey key] => GetItemLocationBasedOnKeyOrRoomKey(key);

		protected readonly ItemInfoProvider ItemProvider;
		protected readonly ItemUnlockingMap UnlockingMap;
		protected readonly SeedOptions SeedOptions;

		string areaName;

		public ItemLocationMap(ItemInfoProvider itemInfoProvider, ItemUnlockingMap itemUnlockingMap, SeedOptions options) 
			: base(CalculateCapacity(options), l => l.Key)
		{
			ItemProvider = itemInfoProvider;
			UnlockingMap = itemUnlockingMap;
			SeedOptions = options;

			SetupGates();

			AddPresentItemLocations();
			AddPastItemLocations();
			AddPyramidItemLocations();

			if (options.GyreArchives)
				AddGyreItemLocations();

			if (options.DownloadableItems)
				AddDownloadTerminals();

			if (options.StartWithTalaria)
				Add(new ExteralItemLocation(itemInfoProvider.Get(EInventoryRelicType.Dash)));
		}

		void SetupGates()
		{
			OculusRift = (SeedOptions.RequireEyeOrbRing)
				? R.OculusRift
				: R.None;

			MawGassMask = (SeedOptions.GassMaw)
				? R.GassMask
				: R.None;

			AccessToLakeDesolation = (!SeedOptions.Inverted)
				? (Gate)R.None
				: R.GateLakeDesolation
				| R.GateKittyBoss
				| R.GateLeftLibrary
				| R.GateSealedCaves
				| (R.GateSealedSirensCave & R.CardE)
				| (R.GateMilitaryGate & (R.CardE | R.CardB));

			LowerLakeDesolationBridge = AccessToLakeDesolation & (R.TimeStop | R.ForwardDash | R.GateKittyBoss | R.GateLeftLibrary);


			AccessToPast = (SeedOptions.Inverted)
				? (Gate)R.None
				: (
					R.TimespinnerWheel & R.TimespinnerSpindle 
					& (
						(LowerLakeDesolationBridge & R.CardD) 
						| (R.GateSealedSirensCave & R.CardE) 
						| (R.GateMilitaryGate & (R.CardB | R.CardE))
					)
				) //libraryTimespinner
				| R.GateLakeSereneLeft
				| R.GateAccessToPast
				| R.GateLakeSereneRight
				| R.GateRoyalTowers
				| R.GateCastleRamparts
				| R.GateCastleKeep
				| (MawGassMask & (R.GateCavesOfBanishment | R.GateMaw));

			MultipleSmallJumpsOfNpc = (Gate)(R.TimespinnerWheel | R.UpwardDash);
			DoubleJumpOfNpc = (R.DoubleJump & R.TimespinnerWheel) | R.UpwardDash;
			ForwardDashDoubleJump = (R.ForwardDash & R.DoubleJump) | R.UpwardDash;

			//past
			LeftSideForestCaves = (AccessToPast & (R.TimespinnerWheel | R.ForwardDash | R.DoubleJump)) | R.GateLakeSereneRight | R.GateLakeSereneLeft;
			UpperLakeSirine = (LeftSideForestCaves & (R.TimeStop | R.Swimming)) | R.GateLakeSereneLeft;
			LowerlakeSirine = (LeftSideForestCaves | R.GateLakeSereneLeft) & R.Swimming;
			LowerCavesOfBanishment = LowerlakeSirine | R.GateCavesOfBanishment | (R.GateMaw & R.DoubleJump);
			UpperCavesOfBanishment = AccessToPast;
			CastleRamparts = AccessToPast;
			CastleKeep = CastleRamparts;
			RoyalTower = (CastleKeep & R.DoubleJump) | R.GateRoyalTowers;
			MidRoyalTower = RoyalTower & (MultipleSmallJumpsOfNpc | ForwardDashDoubleJump);
			UpperRoyalTower = MidRoyalTower & R.DoubleJump;
			KillMaw = (LowerlakeSirine | R.GateCavesOfBanishment | R.GateMaw) & MawGassMask;
			var killTwins = CastleKeep & R.TimeStop;
			var killAelana = UpperRoyalTower;

			//future
			UpperLakeDesolation = AccessToLakeDesolation & UpperLakeSirine & R.AntiWeed;
			LeftLibrary = UpperLakeDesolation | LowerLakeDesolationBridge | R.GateLeftLibrary | R.GateKittyBoss | (R.GateSealedSirensCave & R.CardE) | (R.GateMilitaryGate & (R.CardB | R.CardE));
			MidLibrary = (LeftLibrary & R.CardD) | (R.GateSealedSirensCave & R.CardE) | (R.GateMilitaryGate & (R.CardB | R.CardE));
			UpperLeftLibrary = LeftLibrary & (R.DoubleJump | R.ForwardDash);
			UpperRightSideLibrary = (MidLibrary & (R.CardC | (R.CardB & R.CardE))) | ((R.GateMilitaryGate | R.GateSealedSirensCave) & R.CardE);
			RightSideLibraryElevator = R.CardE & ((MidLibrary & (R.CardC | R.CardB)) | R.GateMilitaryGate | R.GateSealedSirensCave);
			LowerRightSideLibrary = (MidLibrary & R.CardB) | RightSideLibraryElevator | R.GateMilitaryGate | (R.GateSealedSirensCave & R.CardE);
			SealedCavesLeft = (AccessToLakeDesolation & R.DoubleJump) | R.GateSealedCaves;
			SealedCavesLower = SealedCavesLeft & R.CardA;
			SealedCavesSirens = (MidLibrary & R.CardB & R.CardE) | R.GateSealedSirensCave;
			MilitairyFortress = LowerRightSideLibrary & KillMaw & killTwins & killAelana;
			MilitairyFortressHangar = MilitairyFortress;
			RightSideMilitairyFortressHangar = MilitairyFortressHangar & R.DoubleJump;
			TheLab = MilitairyFortressHangar & R.CardB;
			TheLabPoweredOff = TheLab & DoubleJumpOfNpc;
			UpperLab = TheLabPoweredOff & ForwardDashDoubleJump;
			EmperorsTower = UpperLab;

			//pyramid
			LeftPyramid = UpperLab & (
	            R.TimespinnerWheel & R.TimespinnerSpindle &
	            R.TimespinnerPiece1 & R.TimespinnerPiece2 & R.TimespinnerPiece3);
			Nightmare = LeftPyramid & R.UpwardDash;
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
			Add(ItemKey.TutorialMeleeOrb, "Yo Momma", ItemProvider.Get(EInventoryOrbType.Blue, EOrbSlot.Melee));
			Add(ItemKey.TutorialSpellOrb, "Yo Momma", ItemProvider.Get(EInventoryOrbType.Blue, EOrbSlot.Spell));
			areaName = "Lake desolation";
			Add(new ItemKey(1, 1, 1528, 144), "Starter chest 2", ItemProvider.Get(EInventoryUseItemType.FuturePotion), AccessToLakeDesolation);
			Add(new ItemKey(1, 15, 264, 144), "Starter chest 3", ItemProvider.Get(EInventoryEquipmentType.OldCoat), AccessToLakeDesolation);
			Add(new ItemKey(1, 25, 296, 176), "Starter chest 1", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), AccessToLakeDesolation);
			Add(new ItemKey(1, 9, 600, 144 + TimespinnerWheel.YOffset), "Timespinner Wheel room", ItemProvider.Get(EInventoryRelicType.TimespinnerWheel), AccessToLakeDesolation);
			Add(new ItemKey(1, 14, 40, 176), "Forget me not chest", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation);
			areaName = "Lower lake desolation";
			Add(new ItemKey(1, 2, 1016, 384), "Chicken chest", ItemProvider.Get(EItemType.MaxSand), AccessToLakeDesolation & R.TimeStop);
			Add(new ItemKey(1, 11, 72, 240), "Not so secret room", ItemProvider.Get(EItemType.MaxHP), LowerLakeDesolationBridge & OculusRift);
			Add(new ItemKey(1, 3, 56, 176), "Tank chest", ItemProvider.Get(EItemType.MaxAura), AccessToLakeDesolation & R.TimeStop);
			areaName = "Upper lake desolation";
			Add(new ItemKey(1, 17, 152, 96), "Oxygen recovery room", ItemProvider.Get(EInventoryUseItemType.GoldRing), UpperLakeDesolation);
			Add(new ItemKey(1, 21, 200, 144), "Lake secret", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation & OculusRift);
			Add(new ItemKey(1, 20, 232, 96), "Double jump cave floor", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLakeDesolation & R.DoubleJump);
			Add(new ItemKey(1, 20, 168, 240), "Double jump cave platform", ItemProvider.Get(EInventoryUseItemType.FuturePotion), UpperLakeDesolation);
			Add(new ItemKey(1, 22, 344, 160), "Fire-Locked sparrow chest", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1320, 189), "Crash site pedestal", ItemProvider.Get(EInventoryOrbType.Moon, EOrbSlot.Melee), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1272, 192), "Crash site chest 1", ItemProvider.Get(EInventoryEquipmentType.CaptainsCap), UpperLakeDesolation & R.GassMask & KillMaw);
			Add(new ItemKey(1, 18, 1368, 192), "Crash site chest 2", ItemProvider.Get(EInventoryEquipmentType.CaptainsJacket), UpperLakeDesolation & R.GassMask & KillMaw);
			Add(new RoomItemKey(1, 5), "Kitty Boss", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Melee), UpperLakeDesolation | LowerLakeDesolationBridge);
			areaName = "Libary";
			Add(new ItemKey(2, 60, 328, 160), "Basement", ItemProvider.Get(EItemType.MaxHP), LeftLibrary);
			Add(new ItemKey(2, 54, 296, 176), "Consolation", ItemProvider.Get(EInventoryRelicType.ScienceKeycardD), LeftLibrary);
			Add(new ItemKey(2, 41, 404, 246), "Librarian", ItemProvider.Get(EInventoryRelicType.Tablet), LeftLibrary);
			Add(new ItemKey(2, 44, 680, 368), "Reading nook chest", ItemProvider.Get(EInventoryRelicType.FoeScanner), LeftLibrary);
			Add(new ItemKey(2, 47, 216, 208), "Storage room chest 1", ItemProvider.Get(EInventoryUseItemType.Ether), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 152, 208), "Storage room chest 2", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Passive), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 88, 208), "Storage room chest 3", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Spell), LeftLibrary & R.CardD);
			areaName = "Libary top";
			Add(new ItemKey(2, 56, 168, 192), "Backer room chest 5", ItemProvider.Get(EInventoryUseItemType.GoldNecklace), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 392, 192), "Backer room chest 4", ItemProvider.Get(EInventoryUseItemType.GoldRing), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 616, 192), "Backer room chest 3", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 840, 192), "Backer room chest 2", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 1064, 192), "Backer room chest 1", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLeftLibrary);
			areaName = "Varndagroth tower left";
			Add(new ItemKey(2, 34, 232, 1200), "Elevator Key not required", ItemProvider.Get(EInventoryUseItemType.FiligreeTea), MidLibrary); //Default item is Jerky, got replaced by FiligreeTea
			Add(new ItemKey(2, 40, 344, 176), "Ye olde Timespinner", ItemProvider.Get(EInventoryRelicType.ScienceKeycardC), MidLibrary);
			Add(new ItemKey(2, 32, 328, 160), "C Keycard chest", ItemProvider.Get(EInventoryUseItemType.GoldRing), MidLibrary & R.CardC);
			Add(new ItemKey(2, 7, 232, 144), "Left air vents secret", ItemProvider.Get(EItemType.MaxAura), MidLibrary & OculusRift);
			Add(new ItemKey(2, 25, 328, 192), "Left elevator chest", ItemProvider.Get(EItemType.MaxSand), MidLibrary & R.CardE);
			areaName = "Varndagroth tower right";
			Add(new ItemKey(2, 15, 760, 192), "Spider heck room", ItemProvider.Get(EInventoryUseItemType.FuturePotion), UpperRightSideLibrary);
			Add(new ItemKey(2, 20, 72, 1200), "Right elevator chest", ItemProvider.Get(EInventoryUseItemType.Jerky), RightSideLibraryElevator);
			Add(new ItemKey(2, 23, 72, 560), "Elevator card chest", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 1112, 112), "Air vents left", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 136, 304), "Air Vents right", ItemProvider.Get(EInventoryRelicType.ElevatorKeycard), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 11, 104, 192), "Right side bottom floor", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), LowerRightSideLibrary);
			Add(new ItemKey(2, 29, 280, 222 + TimespinnerSpindle.YOffset), "Varndagroth", ItemProvider.Get(EInventoryRelicType.TimespinnerSpindle), RightSideLibraryElevator & R.CardC);
			Add(new RoomItemKey(2, 52), "Spider hell", ItemProvider.Get(EInventoryRelicType.TimespinnerGear2), RightSideLibraryElevator & R.CardA);
			areaName = "Sealed Caves (Xarion)";
			Add(new ItemKey(9, 10, 248, 848), "Skeleton", ItemProvider.Get(EInventoryRelicType.ScienceKeycardB), SealedCavesLeft);
			Add(new ItemKey(9, 19, 664, 704), "Shroom jump room", ItemProvider.Get(EInventoryUseItemType.Antidote), SealedCavesLower & R.TimeStop);
			Add(new ItemKey(9, 39, 88, 192), "Double shroom room", ItemProvider.Get(EInventoryUseItemType.Antidote), SealedCavesLower);
			Add(new ItemKey(9, 41, 312, 192), "Mini jackpot room", ItemProvider.Get(EInventoryUseItemType.GalaxyStone), SealedCavesLower & ForwardDashDoubleJump);
			Add(new ItemKey(9, 42, 328, 192), "Below mini jackpot room", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), SealedCavesLower);
			Add(new ItemKey(9, 12, 280, 160), "Sealed cave secret room", ItemProvider.Get(EItemType.MaxHP), SealedCavesLower & OculusRift);
			Add(new ItemKey(9, 48, 104, 160), "Below Sealed cave secret", ItemProvider.Get(EInventoryUseItemType.FutureEther), SealedCavesLower);
			Add(new ItemKey(9, 15, 248, 192), "Last chance before Xarion", ItemProvider.Get(EInventoryUseItemType.FutureEther), SealedCavesLower & R.DoubleJump);
			Add(new RoomItemKey(9, 13), "Xarion", ItemProvider.Get(EInventoryRelicType.TimespinnerGear3), SealedCavesLower);
			areaName = "Sealed Caves (Sirens)";
			Add(new ItemKey(9, 5, 88, 496), "Solo siren chest", ItemProvider.Get(EItemType.MaxSand), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 3, 1848, 576), "Big siren room right", ItemProvider.Get(EInventoryEquipmentType.BirdStatue), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 3, 744, 560), "Big siren Room left", ItemProvider.Get(EItemType.MaxAura), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 2, 184, 176), "Room after sirens chest 2", ItemProvider.Get(EInventoryUseItemType.WarpCard), SealedCavesSirens);
			Add(new ItemKey(9, 2, 104, 160), "Room after sirens chest 1", ItemProvider.Get(EInventoryRelicType.WaterMask), SealedCavesSirens);
			areaName = "Militairy Fortress";
			Add(new ItemKey(10, 3, 264, 128), "Bomber chest", ItemProvider.Get(EItemType.MaxSand), MilitairyFortress & DoubleJumpOfNpc & R.TimespinnerWheel); //can be reached with just upward dash but not with lightwall unless you got timestop
			Add(new ItemKey(10, 11, 296, 192), "Close combat room", ItemProvider.Get(EItemType.MaxAura), MilitairyFortress);
			Add(new ItemKey(10, 4, 1064, 176), "Bridge full of soldiers", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), MilitairyFortressHangar);
			Add(new ItemKey(10, 10, 104, 192), "Giantess Room", ItemProvider.Get(EInventoryRelicType.AirMask), MilitairyFortressHangar);
			Add(new ItemKey(10, 8, 1080, 176), "Bridge with Giantess", ItemProvider.Get(EInventoryEquipmentType.LabGlasses), MilitairyFortressHangar);
			Add(new ItemKey(10, 7, 104, 192), "Military B door chest 2", ItemProvider.Get(EInventoryUseItemType.PlasmaIV), RightSideMilitairyFortressHangar & R.CardB);
			Add(new ItemKey(10, 7, 152, 192), "Military B door chest 1", ItemProvider.Get(EItemType.MaxSand), RightSideMilitairyFortressHangar & R.CardB);
			Add(new ItemKey(10, 18, 280, 189), "Military pedestal", ItemProvider.Get(EInventoryOrbType.Gun, EOrbSlot.Melee), RightSideMilitairyFortressHangar & (DoubleJumpOfNpc | ForwardDashDoubleJump));
			areaName = "The lab";
			Add(new ItemKey(11, 36, 312, 192), "Coffee Break chest", ItemProvider.Get(EInventoryUseItemType.FoodSynth), TheLab);
			Add(new ItemKey(11, 3, 1528, 192), "Lower trash right", ItemProvider.Get(EItemType.MaxHP), TheLab & R.DoubleJump);
			Add(new ItemKey(11, 3, 72, 192), "Lower trash left", ItemProvider.Get(EInventoryUseItemType.FuturePotion), TheLab & R.UpwardDash); //when lab power is on, it only requires DoubleJumpOfNpc, but we cant code for the power state
			Add(new ItemKey(11, 25, 104, 192), "Single turret room", ItemProvider.Get(EItemType.MaxAura), TheLab & R.DoubleJump);
			Add(new ItemKey(11, 18, 824, 128), "Trash jump room", ItemProvider.Get(EInventoryUseItemType.ChaosHeal), TheLabPoweredOff);
			Add(new RoomItemKey(11, 39), "Dynamo Works", ItemProvider.Get(EInventoryOrbType.Eye, EOrbSlot.Melee), TheLabPoweredOff);
			Add(new RoomItemKey(11, 21), "Blob mom", ItemProvider.Get(EInventoryRelicType.ScienceKeycardA), UpperLab);
			Add(new RoomItemKey(11, 1), "Experiment #13", ItemProvider.Get(EInventoryRelicType.Dash), TheLabPoweredOff);
			Add(new ItemKey(11, 6, 328, 192), "Download and chest room", ItemProvider.Get(EInventoryEquipmentType.LabCoat), UpperLab);
			Add(new ItemKey(11, 27, 296, 160), "Lab secret", ItemProvider.Get(EItemType.MaxSand), UpperLab & OculusRift);
			Add(new RoomItemKey(11, 26), "Spider hell", ItemProvider.Get(EInventoryRelicType.TimespinnerGear1), TheLabPoweredOff & R.CardA);
			areaName = "Emperors tower";
			Add(new ItemKey(12, 5, 344, 192), "After Courtyard - Bottom", ItemProvider.Get(EItemType.MaxAura), EmperorsTower);
			Add(new ItemKey(12, 3, 200, 160), "After Courtyard Floor Secret", ItemProvider.Get(EInventoryEquipmentType.LachiemCrown), EmperorsTower & R.UpwardDash & OculusRift);
			Add(new ItemKey(12, 25, 360, 176), "After Courtyard - Top", ItemProvider.Get(EInventoryEquipmentType.EmpressCoat), EmperorsTower & R.UpwardDash);
			Add(new ItemKey(12, 22, 56, 192), "Galactic Sage Room", ItemProvider.Get(EItemType.MaxSand), EmperorsTower);
			Add(new ItemKey(12, 9, 344, 928), "Bottom of Right Tower", ItemProvider.Get(EInventoryUseItemType.FutureHiEther), EmperorsTower);
			Add(new ItemKey(12, 19, 72, 192), "Wayyyy up there", ItemProvider.Get(EInventoryEquipmentType.FiligreeClasp), EmperorsTower & DoubleJumpOfNpc);
			Add(new ItemKey(12, 13, 120, 176), "Left tower balcony", ItemProvider.Get(EItemType.MaxHP), EmperorsTower);
			Add(new ItemKey(12, 11, 264, 208), "Dad's Chambers chest", ItemProvider.Get(EInventoryRelicType.EmpireBrooch), EmperorsTower); 
			Add(new ItemKey(12, 11, 136, 205), "Dad's Chambers pedestal", ItemProvider.Get(EInventoryOrbType.Empire, EOrbSlot.Melee), EmperorsTower);
		}

		void AddPastItemLocations()
		{
			areaName = "Refugee Camp";
			Add(new RoomItemKey(3, 0), "Neliste's Bra", ItemProvider.Get(EInventoryOrbType.Flame, EOrbSlot.Melee), AccessToPast);
			Add(new ItemKey(3, 30, 296, 176), "Refugee camp storage chest 3", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), AccessToPast);
			Add(new ItemKey(3, 30, 232, 176), "Refugee camp storage chest 2", ItemProvider.Get(EInventoryUseItemType.GoldNecklace), AccessToPast);
			Add(new ItemKey(3, 30, 168, 176), "Refugee camp storage chest 1", ItemProvider.Get(EInventoryRelicType.JewelryBox), AccessToPast);
			areaName = "Forest";
			Add(new ItemKey(3, 3, 648, 272), "Refugee camp roof", ItemProvider.Get(EInventoryUseItemType.Herb), AccessToPast);
			Add(new ItemKey(3, 15, 248, 112), "Bat jump chest", ItemProvider.Get(EItemType.MaxAura), AccessToPast & (DoubleJumpOfNpc | ForwardDashDoubleJump | (R.TimeStop & R.ForwardDash)));
			Add(new ItemKey(3, 21, 120, 192), "Green platform secret", ItemProvider.Get(EItemType.MaxSand), AccessToPast & OculusRift);
			Add(new ItemKey(3, 12, 776, 560), "Rats guarded chest", ItemProvider.Get(EInventoryEquipmentType.PointyHat), AccessToPast);
			Add(new ItemKey(3, 11, 392, 608), "Waterfall chest 1", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), AccessToPast & R.Swimming);
			Add(new ItemKey(3, 5, 184, 192), "Waterfall chest 2", ItemProvider.Get(EInventoryEquipmentType.Pendulum), AccessToPast & R.Swimming);
			Add(new ItemKey(3, 2, 584, 368), "Batcave", ItemProvider.Get(EInventoryUseItemType.Potion), AccessToPast);
			Add(new ItemKey(4, 20, 264, 160), "Bridge Chest", ItemProvider.Get(EItemType.MaxAura), AccessToPast);
			Add(new ItemKey(3, 29, 248, 192), "Solitary bat room", ItemProvider.Get(EItemType.MaxHP), LeftSideForestCaves);
			areaName = "Upper Lake Sirine";
			Add(new ItemKey(7, 16, 152, 96), "Rat nest", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLakeSirine);
			Add(new ItemKey(7, 19, 248, 96), "Double jump cave platform", ItemProvider.Get(EItemType.MaxAura), UpperLakeSirine & R.DoubleJump);
			Add(new ItemKey(7, 19, 168, 240), "Double jump cave floor", ItemProvider.Get(EInventoryEquipmentType.TravelersCloak), UpperLakeSirine);
			Add(new ItemKey(7, 27, 184, 144), "West lake serene cave secret", ItemProvider.Get(EInventoryFamiliarType.Griffin), UpperLakeSirine & OculusRift);
			Add(new ItemKey(7, 13, 56, 176), "Chest behind vines", ItemProvider.Get(EInventoryUseItemType.WarpCard), UpperLakeSirine);
			Add(new ItemKey(7, 30, 296, 176), "Pyramid keys room", ItemProvider.Get(EInventoryRelicType.PyramidsKey), UpperLakeSirine);
			areaName = "Lower Lake Sirine";
			Add(new ItemKey(7, 3, 440, 1232), "Deep dive", ItemProvider.Get(EInventoryUseItemType.Potion), LowerlakeSirine);
			Add(new ItemKey(7, 7, 1432, 576), "Under the eels", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerlakeSirine);
			Add(new ItemKey(7, 6, 520, 496), "Water spikes room", ItemProvider.Get(EInventoryUseItemType.Potion), LowerlakeSirine);
			Add(new ItemKey(7, 11, 88, 240), "Underwater secret", ItemProvider.Get(EItemType.MaxHP), LowerlakeSirine & OculusRift);
			Add(new ItemKey(7, 2, 1016, 384), "T chest", ItemProvider.Get(EInventoryUseItemType.Ether), LowerlakeSirine);
			Add(new ItemKey(7, 20, 248, 96), "Past the eels", ItemProvider.Get(EItemType.MaxSand), LowerlakeSirine);
			Add(new ItemKey(7, 9, 584, 189), "Underwater pedestal", ItemProvider.Get(EInventoryOrbType.Ice, EOrbSlot.Melee), LowerlakeSirine);
			areaName = "Caves of Banishment (Maw)";
			Add(new ItemKey(8, 19, 664, 704), "Mushroom double jump", ItemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment & R.DoubleJump);
			Add(new ItemKey(8, 12, 280, 160), "Caves of banishment secret room", ItemProvider.Get(EItemType.MaxHP), LowerCavesOfBanishment & OculusRift);
			Add(new ItemKey(8, 48, 104, 160), "Below caves of banishment secret", ItemProvider.Get(EInventoryUseItemType.Spaghetti), LowerCavesOfBanishment); //Default item is Herb but got replaced by Spaghetti
			Add(new ItemKey(8, 39, 88, 192), "Single shroom room", ItemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment);
			Add(new ItemKey(8, 41, 168, 192), "Jackpot room chest 1", ItemProvider.Get(EInventoryUseItemType.GoldNecklace), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 216, 192), "Jackpot room chest 2", ItemProvider.Get(EInventoryUseItemType.GoldRing), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 264, 192), "Jackpot room chest 3", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 312, 192), "Jackpot room chest 4", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 42, 216, 189), "Banishment pedestal", ItemProvider.Get(EInventoryOrbType.Wind, EOrbSlot.Melee), LowerCavesOfBanishment);
			Add(new ItemKey(8, 15, 248, 192), "Last chance before Maw", ItemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment & R.DoubleJump);
			Add(new ItemKey(8, 31, 88, 400), "Mineshaft", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerCavesOfBanishment & MawGassMask);
			areaName = "Caves of Banishment (Sirens)";
			Add(new ItemKey(8, 4, 664, 144), "Wyvern room", ItemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 808, 144), "Above water sirens", ItemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 744, 560), "Underwater sirens left", ItemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 3, 1848, 576), "Underwater sirens right", ItemProvider.Get(EItemType.MaxAura), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 5, 88, 496), "water hook", ItemProvider.Get(EItemType.MaxSand), UpperCavesOfBanishment & R.Swimming);
			areaName = "Caste Ramparts";
			Add(new ItemKey(4, 1, 456, 160), "Bomber chest", ItemProvider.Get(EItemType.MaxSand), CastleRamparts & MultipleSmallJumpsOfNpc);
			Add(new ItemKey(4, 3, 136, 144), "Freeze the engineer", ItemProvider.Get(EItemType.MaxHP), CastleRamparts & (R.TimeStop | R.ForwardDash));
			Add(new ItemKey(4, 10, 56, 192), "Giantess guarded room", ItemProvider.Get(EInventoryUseItemType.HiPotion), CastleRamparts);
			Add(new ItemKey(4, 11, 344, 192), "Knight and archer guarded room", ItemProvider.Get(EInventoryUseItemType.HiPotion), CastleRamparts);
			Add(new ItemKey(4, 22, 104, 189), "Castle pedestal", ItemProvider.Get(EInventoryOrbType.Iron, EOrbSlot.Melee), CastleRamparts);
			areaName = "Caste Keep";
			Add(new ItemKey(5, 9, 104, 189), "Basement secret pedestal", ItemProvider.Get(EInventoryOrbType.Blood, EOrbSlot.Melee), CastleKeep & OculusRift);
			Add(new ItemKey(5, 10, 104, 192), "Break the wall", ItemProvider.Get(EInventoryFamiliarType.Sprite), CastleKeep);
			Add(new ItemKey(5, 14, 88, 208), "Yas queen room", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), CastleKeep & R.PinkOrb & R.DoubleJump);
			Add(new ItemKey(5, 44, 216, 192), "Basement hammer", ItemProvider.Get(EInventoryUseItemType.Potion), CastleKeep);
			Add(new ItemKey(5, 45, 104, 192), "Omelette chest", ItemProvider.Get(EItemType.MaxHP), CastleKeep);
			Add(new ItemKey(5, 15, 296, 192), "Just an egg", ItemProvider.Get(EItemType.MaxAura), CastleKeep);
			Add(new ItemKey(5, 41, 72, 160), "Out of the way", ItemProvider.Get(EInventoryEquipmentType.BuckleHat), CastleKeep);
			Add(new RoomItemKey(5, 5), "Twins", ItemProvider.Get(EInventoryRelicType.DoubleJump), CastleKeep & R.TimeStop);
			Add(new ItemKey(5, 22, 312, 176), "Royal guard tiny room", ItemProvider.Get(EItemType.MaxSand), CastleKeep & ((R.TimeStop & R.ForwardDash) | R.DoubleJump));
			areaName = "Royal towers";
			Add(new ItemKey(6, 19, 200, 176), "Royal towers floor secret", ItemProvider.Get(EItemType.MaxAura), RoyalTower & R.DoubleJump & OculusRift);
			Add(new ItemKey(6, 27, 472, 384), "Above the gap", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), MidRoyalTower);
			Add(new ItemKey(6, 1, 1512, 288), "Under the ice mage", ItemProvider.Get(EInventoryUseItemType.Potion), MidRoyalTower);
			Add(new ItemKey(6, 25, 360, 176), "Next to easy struggle juggle room", ItemProvider.Get(EInventoryUseItemType.HiEther), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 3, 120, 208), "Easy struggle juggle", ItemProvider.Get(EInventoryFamiliarType.Demon), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 17, 200, 112), "Hard struggle juggle", ItemProvider.Get(EItemType.MaxHP), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 17, 56, 448), "No struggle required", ItemProvider.Get(EInventoryEquipmentType.VileteCrown), UpperRoyalTower);
			Add(new ItemKey(6, 17, 360, 1840), "Right tower freebie", ItemProvider.Get(EInventoryEquipmentType.MidnightCloak), MidRoyalTower);
			Add(new ItemKey(6, 13, 120, 176), "Above the ice mage", ItemProvider.Get(EItemType.MaxSand), UpperRoyalTower);
			Add(new ItemKey(6, 22, 88, 208), "Royal guard big room", ItemProvider.Get(EInventoryUseItemType.Ether), UpperRoyalTower);
			Add(new ItemKey(6, 11, 360, 544), "Before Aelana", ItemProvider.Get(EInventoryUseItemType.HiPotion), UpperRoyalTower);
			Add(new ItemKey(6, 23, 856, 208), "Statue room", ItemProvider.Get(EInventoryEquipmentType.VileteDress), UpperRoyalTower & R.UpwardDash);
			Add(new ItemKey(6, 14, 136, 208), "Aelana's pedestal", ItemProvider.Get(EInventoryOrbType.Pink, EOrbSlot.Melee), UpperRoyalTower);
			Add(new ItemKey(6, 14, 184, 205), "After Aelana", ItemProvider.Get(EInventoryUseItemType.WarpCard), UpperRoyalTower);
		}

		void AddPyramidItemLocations()
		{
			areaName = "Ancient Pyramid";
			Add(new ItemKey(16, 14, 312, 192), "Why not it's right there", ItemProvider.Get(EItemType.MaxSand), LeftPyramid);
			Add(new ItemKey(16, 3, 88, 192), "Conviction guarded room", ItemProvider.Get(EItemType.MaxHP), LeftPyramid);
			Add(new ItemKey(16, 22, 200, 192), "Pit secret room", ItemProvider.Get(EItemType.MaxAura), Nightmare & OculusRift); //only requires LeftPyramid to reach but Nightmate to escape
			Add(new ItemKey(16, 16, 1512, 144), "Regret chest", ItemProvider.Get(EInventoryRelicType.EssenceOfSpace), Nightmare & OculusRift); //only requires LeftPyramid to reach but Nightmate to escape
			areaName = "Temporal Gyre"; // Main path is in pyramid logic, boss rooms are behind GyreArchives flag
			Add(new ItemKey(14, 14, 200, 832), "Gyre Chest 1", null, Nightmare);
			Add(new ItemKey(14, 17, 200, 832), "Gyre Chest 2", null, Nightmare);
			Add(new ItemKey(14, 20, 200, 832), "Gyre Chest 3", null, Nightmare);
		}

		void AddGyreItemLocations()
		{
			areaName = "Temporal Gyre";
			Add(new ItemKey(14, 8, 120, 176), "Ravenlord Entry", null, UpperLab & R.MerchantCrow);
			Add(new ItemKey(14, 9, 200, 125), "Ravenlord Pedestal", null, UpperLab & R.MerchantCrow);
			Add(new ItemKey(14, 9, 280, 176), "Ravenlord Exit", null, UpperLab & R.MerchantCrow);
			Add(new ItemKey(14, 6, 40, 208), "Ifrit Entry", null, UpperLeftLibrary & R.Kobo);
			Add(new ItemKey(14, 7, 200, 205), "Ifrit Pedestal", null, UpperLeftLibrary & R.Kobo);
			Add(new ItemKey(14, 7, 280, 208), "Ifrit Exit", null, UpperLeftLibrary & R.Kobo);
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
			Add(new ItemKey(11, 6, 200, 192), "Chest and download terminal", null, UpperLab & R.Tablet);
			Add(new ItemKey(11, 15, 152, 176), "Lab terminal middle", null, TheLabPoweredOff & R.Tablet);
			Add(new ItemKey(11, 16, 600, 192), "Sentry platform terminal", null, TheLabPoweredOff & R.Tablet);
			Add(new ItemKey(11, 34, 200, 192), "Experiment 13 terminal", null, TheLab & R.Tablet);
			Add(new ItemKey(11, 37, 200, 192), "Lab terminal left", null, TheLab & R.Tablet);
			Add(new ItemKey(11, 38, 120, 176), "Lab terminal right", null, TheLabPoweredOff & R.Tablet);
		}

		ItemLocation GetItemLocationBasedOnKeyOrRoomKey(ItemKey key)
		{
			return TryGetValue(key, out var itemLocation)
				? itemLocation
				: TryGetValue(key.ToRoomItemKey(), out var roomItemLocation)
					? roomItemLocation
					: null;
		}

		public virtual ProgressionChain GetProgressionChain()
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

		public R GetAvailableRequirementsBasedOnObtainedItems()
		{
			var pickedUpProgressionItemLocations = this
				.Where(l => l.IsPickedUp && l.ItemInfo.Unlocks != R.None)
				.ToArray();

			var ca = this.Where(l => l.ItemInfo.Identifier == new ItemIdentifier(EInventoryRelicType.ScienceKeycardA))
				.ToArray();
			var cb = this.Where(l => l.ItemInfo.Identifier == new ItemIdentifier(EInventoryRelicType.ScienceKeycardB))
				.ToArray();
			var cc = this.Where(l => l.ItemInfo.Identifier == new ItemIdentifier(EInventoryRelicType.ScienceKeycardC))
				.ToArray();
			var cd = this.Where(l => l.ItemInfo.Identifier == new ItemIdentifier(EInventoryRelicType.ScienceKeycardD))
				.ToArray();

			var pickedUpSingleItemLocationUnlocks = pickedUpProgressionItemLocations
				.Where(l => !(l.ItemInfo is PogRessiveItemInfo))
				.Select(l => l.ItemInfo.Unlocks);

			var pickedUpProgressiveItemLocationUnlocks = pickedUpProgressionItemLocations
				.Where(l => l.ItemInfo is PogRessiveItemInfo)
				.Select(l => ((PogRessiveItemInfo)l.ItemInfo)
					.GetAllUnlockedItems()
					.Select(i => i.Unlocks)
					.Aggregate(R.None, (a, b) => a | b));

			return pickedUpSingleItemLocationUnlocks.Concat(pickedUpProgressiveItemLocationUnlocks)
				.Aggregate(R.None, (a, b) => a | b);
		}

		public virtual bool IsBeatable()
		{
			if ((!SeedOptions.GassMaw && !IsGassMaskReachableWithTheMawRequirements())
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
			//the very basics to reach maw should also allow you to get gassmask
			//unless we run inverted, then we can garantee the user has the pyramid keys before entering lake desolation
			var gassmaskLocation = this.First(l => l.ItemInfo?.Identifier == new ItemIdentifier(EInventoryRelicType.AirMask));

			var levelIdsToAvoid = new List<int>(3) { 1 }; //lake desolation
			var mawRequirements = R.None;

			if (!SeedOptions.Inverted)
			{
				mawRequirements |= R.GateAccessToPast;

				//for non inverted seeds we dont know pyramid keys are required as it can be a classic past seed
				/*var isWatermaskRequiredForMaw = unlockingMap.PyramidKeysUnlock != R.GateMaw
				                                && unlockingMap.PyramidKeysUnlock != R.GateCavesOfBanishment;

				if (isWatermaskRequiredForMaw)
					mawRequirements |= R.Swimming;*/

				levelIdsToAvoid.Add(2); //library

				//if(unlockingMap.PyramidKeysUnlock != R.GateSealedCaves)
					levelIdsToAvoid.Add(9); //xarion skelleton
			}
			else
			{
				mawRequirements |= R.Swimming;
				mawRequirements |= UnlockingMap.PyramidKeysUnlock;
			}

			return !levelIdsToAvoid.Contains(gassmaskLocation.Key.LevelId) && gassmaskLocation.Gate.CanBeOpenedWith(mawRequirements);
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

		static R GetObtainedRequirements(ItemLocation[] reachableLocations)
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

		bool CanCompleteGame(R obtainedRequirements)
		{
			return Nightmare.CanBeOpenedWith(obtainedRequirements);
		}

		public virtual void Update(Level level)
		{
		}

		public virtual void Initialize(GameSave gameSave)
		{
			var progressiveItemInfos = this
				.Where(l => l.ItemInfo is PogRessiveItemInfo)
				.Select(l => (PogRessiveItemInfo)l.ItemInfo);

			foreach (var progressiveItem in progressiveItemInfos)
				progressiveItem.Reset();

			foreach (var itemLocation in this)
				itemLocation.BsseOnGameSave(gameSave);
		}

		protected void Add(ItemKey itemKey, string name, ItemInfo defaultItem)
		{
			Add(new ItemLocation(itemKey, areaName, name, defaultItem));
		}

		protected void Add(ItemKey itemKey, string name, ItemInfo defaultItem, Gate gate)
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
