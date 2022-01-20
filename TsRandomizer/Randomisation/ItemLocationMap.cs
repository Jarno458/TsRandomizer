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
	class ItemLocationMap : LookupDictionary<ItemKey, ItemLocation>
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
		internal Gate LowerLakeSirine;
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
		internal Gate IfritsLair;
		internal Gate MidLibrary;
		internal Gate UpperRightSideLibrary;
		internal Gate RightSideLibraryElevator;
		internal Gate LowerRightSideLibrary;
		internal Gate SealedCavesLeft;
		internal Gate SealedCavesLower;
		internal Gate SealedCavesSirens;
		internal Gate MilitaryFortress;
		internal Gate RavenlordsLair;
		internal Gate MilitaryFortressHangar;
		internal Gate RightSideMilitaryFortressHangar;
		internal Gate TheLab;
		internal Gate TheLabPoweredOff;
		internal Gate UpperLab;
		internal Gate EmperorsTower;
		//pyramid
		internal Gate TemporalGyre;
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

			if (options.Cantoran)
				AddCantoran();

			if (options.LoreChecks)
				AddLoreLocations();

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
			LeftSideForestCaves = (AccessToPast & (R.TimeStop | R.ForwardDash)) | R.GateLakeSereneRight | R.GateLakeSereneLeft;
			UpperLakeSirine = (LeftSideForestCaves & (R.TimeStop | R.Swimming)) | R.GateLakeSereneLeft;
			LowerLakeSirine = (LeftSideForestCaves | R.GateLakeSereneLeft) & R.Swimming;
			LowerCavesOfBanishment = LowerLakeSirine | R.GateCavesOfBanishment | (R.GateMaw & R.DoubleJump);
			UpperCavesOfBanishment = AccessToPast;
			CastleRamparts = AccessToPast;
			CastleKeep = CastleRamparts;
			RoyalTower = (CastleKeep & R.DoubleJump) | R.GateRoyalTowers;
			MidRoyalTower = RoyalTower & (MultipleSmallJumpsOfNpc | ForwardDashDoubleJump);
			UpperRoyalTower = MidRoyalTower & R.DoubleJump;
			KillMaw = (LowerLakeSirine | R.GateCavesOfBanishment | R.GateMaw) & MawGassMask;
			var killTwins = CastleKeep & R.TimeStop;
			var killAelana = UpperRoyalTower;

			//future
			UpperLakeDesolation = AccessToLakeDesolation & UpperLakeSirine & R.AntiWeed;
			LeftLibrary = UpperLakeDesolation | LowerLakeDesolationBridge | R.GateLeftLibrary | R.GateKittyBoss | (R.GateSealedSirensCave & R.CardE) | (R.GateMilitaryGate & (R.CardB | R.CardE));
			MidLibrary = (LeftLibrary & R.CardD) | (R.GateSealedSirensCave & R.CardE) | (R.GateMilitaryGate & (R.CardB | R.CardE));
			UpperLeftLibrary = LeftLibrary & (R.DoubleJump | R.ForwardDash);
			IfritsLair = UpperLeftLibrary & R.Kobo & AccessToPast;
			UpperRightSideLibrary = (MidLibrary & (R.CardC | (R.CardB & R.CardE))) | ((R.GateMilitaryGate | R.GateSealedSirensCave) & R.CardE);
			RightSideLibraryElevator = R.CardE & ((MidLibrary & (R.CardC | R.CardB)) | R.GateMilitaryGate | R.GateSealedSirensCave);
			LowerRightSideLibrary = (MidLibrary & R.CardB) | RightSideLibraryElevator | R.GateMilitaryGate | (R.GateSealedSirensCave & R.CardE);
			SealedCavesLeft = (AccessToLakeDesolation & R.DoubleJump) | R.GateSealedCaves;
			SealedCavesLower = SealedCavesLeft & R.CardA;
			SealedCavesSirens = (MidLibrary & R.CardB & R.CardE) | R.GateSealedSirensCave;
			MilitaryFortress = LowerRightSideLibrary & KillMaw & killTwins & killAelana;
			MilitaryFortressHangar = MilitaryFortress;
			RightSideMilitaryFortressHangar = MilitaryFortressHangar & R.DoubleJump;
			TheLab = MilitaryFortressHangar & R.CardB;
			TheLabPoweredOff = TheLab & DoubleJumpOfNpc;
			UpperLab = TheLabPoweredOff & ForwardDashDoubleJump;
			RavenlordsLair = UpperLab & R.MerchantCrow;
			EmperorsTower = UpperLab;

			//pyramid
			TemporalGyre = MilitaryFortress & R.TimespinnerWheel;
			LeftPyramid = UpperLab & (
				R.TimespinnerWheel & R.TimespinnerSpindle &
				R.TimespinnerPiece1 & R.TimespinnerPiece2 & R.TimespinnerPiece3);
			Nightmare = LeftPyramid & R.UpwardDash;
		}

		static int CalculateCapacity(SeedOptions options)
		{
			var capacity = 166;

			if (options.StartWithTalaria)
				capacity += 1;
			if (options.DownloadableItems)
				capacity += 14;
			if (options.GyreArchives)
				capacity += 9;
			if (options.Cantoran)
				capacity += 1;
			if (options.LoreChecks)
				capacity += 22;

			return capacity;
		}

		void AddPresentItemLocations()
		{
			areaName = "Tutorial";
			Add(ItemKey.TutorialMeleeOrb, "Yo Momma", ItemProvider.Get(EInventoryOrbType.Blue, EOrbSlot.Melee));
			Add(ItemKey.TutorialSpellOrb, "Yo Momma", ItemProvider.Get(EInventoryOrbType.Blue, EOrbSlot.Spell));
			areaName = "Lake Desolation";
			Add(new ItemKey(1, 1, 1528, 144), "Starter chest 2", ItemProvider.Get(EInventoryUseItemType.FuturePotion), AccessToLakeDesolation);
			Add(new ItemKey(1, 15, 264, 144), "Starter chest 3", ItemProvider.Get(EInventoryEquipmentType.OldCoat), AccessToLakeDesolation);
			Add(new ItemKey(1, 25, 296, 176), "Starter chest 1", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), AccessToLakeDesolation);
			Add(new ItemKey(1, 9, 600, 144 + TimespinnerWheel.YOffset), "Timespinner Wheel room", ItemProvider.Get(EInventoryRelicType.TimespinnerWheel), AccessToLakeDesolation);
			Add(new ItemKey(1, 14, 40, 176), "Forget me not chest", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation);
			areaName = "Lower Lake Desolation";
			Add(new ItemKey(1, 2, 1016, 384), "Chicken chest", ItemProvider.Get(EItemType.MaxSand), AccessToLakeDesolation & R.TimeStop);
			Add(new ItemKey(1, 11, 72, 240), "Not so secret room", ItemProvider.Get(EItemType.MaxHP), LowerLakeDesolationBridge & OculusRift);
			Add(new ItemKey(1, 3, 56, 176), "Tank chest", ItemProvider.Get(EItemType.MaxAura), AccessToLakeDesolation & R.TimeStop);
			areaName = "Upper Lake Desolation";
			Add(new ItemKey(1, 17, 152, 96), "Upper desolation Oxygen recovery room", ItemProvider.Get(EInventoryUseItemType.GoldRing), UpperLakeDesolation);
			Add(new ItemKey(1, 21, 200, 144), "Upper desolation secret", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation & OculusRift);
			Add(new ItemKey(1, 20, 232, 96), "Upper desolation double jump cave platform", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLakeDesolation & R.DoubleJump);
			Add(new ItemKey(1, 20, 168, 240), "Upper desolation double jump cave floor", ItemProvider.Get(EInventoryUseItemType.FuturePotion), UpperLakeDesolation);
			Add(new ItemKey(1, 22, 344, 160), "Fire-Locked sparrow chest", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1320, 189), "Crash site pedestal", ItemProvider.Get(EInventoryOrbType.Moon, EOrbSlot.Melee), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1272, 192), "Crash site chest 1", ItemProvider.Get(EInventoryEquipmentType.CaptainsCap), UpperLakeDesolation & R.GassMask & KillMaw);
			Add(new ItemKey(1, 18, 1368, 192), "Crash site chest 2", ItemProvider.Get(EInventoryEquipmentType.CaptainsJacket), UpperLakeDesolation & R.GassMask & KillMaw);
			Add(new RoomItemKey(1, 5), "Kitty Boss", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Melee), UpperLakeDesolation | LowerLakeDesolationBridge);
			areaName = "Library";
			Add(new ItemKey(2, 60, 328, 160), "Library Basement", ItemProvider.Get(EItemType.MaxHP), LeftLibrary);
			Add(new ItemKey(2, 54, 296, 176), "Library warp gate", ItemProvider.Get(EInventoryRelicType.ScienceKeycardD), LeftLibrary);
			Add(new ItemKey(2, 41, 404, 246), "Librarian", ItemProvider.Get(EInventoryRelicType.Tablet), LeftLibrary);
			Add(new ItemKey(2, 44, 680, 368), "Reading nook chest", ItemProvider.Get(EInventoryRelicType.FoeScanner), LeftLibrary);
			Add(new ItemKey(2, 47, 216, 208), "Storage room chest 1", ItemProvider.Get(EInventoryUseItemType.Ether), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 152, 208), "Storage room chest 2", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Passive), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 88, 208), "Storage room chest 3", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Spell), LeftLibrary & R.CardD);
			areaName = "Library Top";
			Add(new ItemKey(2, 56, 168, 192), "Backer room chest 5", ItemProvider.Get(EInventoryUseItemType.GoldNecklace), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 392, 192), "Backer room chest 4", ItemProvider.Get(EInventoryUseItemType.GoldRing), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 616, 192), "Backer room chest 3", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 840, 192), "Backer room chest 2", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 1064, 192), "Backer room chest 1", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLeftLibrary);
			areaName = "Varndagroth Tower Left";
			Add(new ItemKey(2, 34, 232, 1200), "Elevator Key not required", ItemProvider.Get(EInventoryUseItemType.FiligreeTea), MidLibrary); //Default item is Jerky, got replaced by FiligreeTea
			Add(new ItemKey(2, 40, 344, 176), "Ye olde Timespinner", ItemProvider.Get(EInventoryRelicType.ScienceKeycardC), MidLibrary);
			Add(new ItemKey(2, 32, 328, 160), "Varndagroth left bottom floor", ItemProvider.Get(EInventoryUseItemType.GoldRing), MidLibrary & R.CardC);
			Add(new ItemKey(2, 7, 232, 144), "Left air vents secret", ItemProvider.Get(EItemType.MaxAura), MidLibrary & OculusRift);
			Add(new ItemKey(2, 25, 328, 192), "Left elevator chest", ItemProvider.Get(EItemType.MaxSand), MidLibrary & R.CardE);
			areaName = "Varndagroth Tower Right";
			Add(new ItemKey(2, 15, 760, 192), "Varndagroth bridge", ItemProvider.Get(EInventoryUseItemType.FuturePotion), UpperRightSideLibrary);
			Add(new ItemKey(2, 20, 72, 1200), "Right Varndagroth elevator chest", ItemProvider.Get(EInventoryUseItemType.Jerky), RightSideLibraryElevator);
			Add(new ItemKey(2, 23, 72, 560), "Elevator card chest", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 1112, 112), "Air vents right", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 136, 304), "Air Vents left", ItemProvider.Get(EInventoryRelicType.ElevatorKeycard), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 11, 104, 192), "Varndagroth right bottom floor", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), LowerRightSideLibrary);
			Add(new ItemKey(2, 29, 280, 222 + TimespinnerSpindle.YOffset), "Varndagroth", ItemProvider.Get(EInventoryRelicType.TimespinnerSpindle), RightSideLibraryElevator & R.CardC);
			Add(new RoomItemKey(2, 52), "Spider hell", ItemProvider.Get(EInventoryRelicType.TimespinnerGear2), RightSideLibraryElevator & R.CardA);
			areaName = "Sealed Caves (Xarion)";
			Add(new ItemKey(9, 10, 248, 848), "Skeleton", ItemProvider.Get(EInventoryRelicType.ScienceKeycardB), SealedCavesLeft);
			Add(new ItemKey(9, 19, 664, 704), "Sealed cave shroom jump room", ItemProvider.Get(EInventoryUseItemType.Antidote), SealedCavesLower & R.TimeStop);
			Add(new ItemKey(9, 39, 88, 192), "Sealed cave double shroom room", ItemProvider.Get(EInventoryUseItemType.Antidote), SealedCavesLower);
			Add(new ItemKey(9, 41, 312, 192), "Sealed cave Mini jackpot room", ItemProvider.Get(EInventoryUseItemType.GalaxyStone), SealedCavesLower & ForwardDashDoubleJump);
			Add(new ItemKey(9, 42, 328, 192), "Below sealed cave mini jackpot room", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), SealedCavesLower);
			Add(new ItemKey(9, 12, 280, 160), "Sealed cave secret room", ItemProvider.Get(EItemType.MaxHP), SealedCavesLower & OculusRift);
			Add(new ItemKey(9, 48, 104, 160), "Sealed cave bottom left", ItemProvider.Get(EInventoryUseItemType.FutureEther), SealedCavesLower);
			Add(new ItemKey(9, 15, 248, 192), "Last chance before Xarion", ItemProvider.Get(EInventoryUseItemType.FutureEther), SealedCavesLower & R.DoubleJump);
			Add(new RoomItemKey(9, 13), "Xarion", ItemProvider.Get(EInventoryRelicType.TimespinnerGear3), SealedCavesLower);
			areaName = "Sealed Caves (Sirens)";
			Add(new ItemKey(9, 5, 88, 496), "Upper sealed cave water hook", ItemProvider.Get(EItemType.MaxSand), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 3, 1848, 576), "Upper sealed cave siren room right", ItemProvider.Get(EInventoryEquipmentType.BirdStatue), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 3, 744, 560), "Upper sealed cave siren room left", ItemProvider.Get(EItemType.MaxAura), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 2, 184, 176), "Upper sealed cave after sirens chest 1", ItemProvider.Get(EInventoryUseItemType.WarpCard), SealedCavesSirens);
			Add(new ItemKey(9, 2, 104, 160), "Upper sealed cave after sirens chest 2", ItemProvider.Get(EInventoryRelicType.WaterMask), SealedCavesSirens);
			areaName = "Military Fortress";
			Add(new ItemKey(10, 3, 264, 128), "Military bomber chest", ItemProvider.Get(EItemType.MaxSand), MilitaryFortress & DoubleJumpOfNpc & R.TimespinnerWheel); //can be reached with just upward dash but not with lightwall unless you got timestop
			Add(new ItemKey(10, 11, 296, 192), "Close combat room", ItemProvider.Get(EItemType.MaxAura), MilitaryFortress);
			Add(new ItemKey(10, 4, 1064, 176), "Military soldiers bridge", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), MilitaryFortressHangar);
			Add(new ItemKey(10, 10, 104, 192), "Military giantess room", ItemProvider.Get(EInventoryRelicType.AirMask), MilitaryFortressHangar);
			Add(new ItemKey(10, 8, 1080, 176), "Military giantess bridge", ItemProvider.Get(EInventoryEquipmentType.LabGlasses), MilitaryFortressHangar);
			Add(new ItemKey(10, 7, 104, 192), "Military B door chest 2", ItemProvider.Get(EInventoryUseItemType.PlasmaIV), RightSideMilitaryFortressHangar & R.CardB);
			Add(new ItemKey(10, 7, 152, 192), "Military B door chest 1", ItemProvider.Get(EItemType.MaxSand), RightSideMilitaryFortressHangar & R.CardB);
			Add(new ItemKey(10, 18, 280, 189), "Military pedestal", ItemProvider.Get(EInventoryOrbType.Gun, EOrbSlot.Melee), RightSideMilitaryFortressHangar & (DoubleJumpOfNpc | ForwardDashDoubleJump));
			areaName = "The Lab";
			Add(new ItemKey(11, 36, 312, 192), "Coffee break", ItemProvider.Get(EInventoryUseItemType.FoodSynth), TheLab);
			Add(new ItemKey(11, 3, 1528, 192), "Lower trash right", ItemProvider.Get(EItemType.MaxHP), TheLab & R.DoubleJump);
			Add(new ItemKey(11, 3, 72, 192), "Lower trash left", ItemProvider.Get(EInventoryUseItemType.FuturePotion), TheLab & R.UpwardDash); //when lab power is on, it only requires DoubleJumpOfNpc, but we cant code for the power state
			Add(new ItemKey(11, 25, 104, 192), "Below lab entrance", ItemProvider.Get(EItemType.MaxAura), TheLab & R.DoubleJump);
			Add(new ItemKey(11, 18, 824, 128), "Trash jump room", ItemProvider.Get(EInventoryUseItemType.ChaosHeal), TheLabPoweredOff);
			Add(new RoomItemKey(11, 39), "Dynamo Works", ItemProvider.Get(EInventoryOrbType.Eye, EOrbSlot.Melee), TheLabPoweredOff);
			Add(new RoomItemKey(11, 21), "Blob mom", ItemProvider.Get(EInventoryRelicType.ScienceKeycardA), UpperLab);
			Add(new RoomItemKey(11, 1), "Experiment #13", ItemProvider.Get(EInventoryRelicType.Dash), TheLabPoweredOff);
			Add(new ItemKey(11, 6, 328, 192), "Download and chest room", ItemProvider.Get(EInventoryEquipmentType.LabCoat), UpperLab);
			Add(new ItemKey(11, 27, 296, 160), "Lab secret", ItemProvider.Get(EItemType.MaxSand), UpperLab & OculusRift);
			Add(new RoomItemKey(11, 26), "Spider hell", ItemProvider.Get(EInventoryRelicType.TimespinnerGear1), TheLabPoweredOff & R.CardA);
			areaName = "Emperor's Tower";
			Add(new ItemKey(12, 5, 344, 192), "Dad's bottom", ItemProvider.Get(EItemType.MaxAura), EmperorsTower);
			Add(new ItemKey(12, 3, 200, 160), "Dad's courtyard floor secret", ItemProvider.Get(EInventoryEquipmentType.LachiemCrown), EmperorsTower & R.UpwardDash & OculusRift);
			Add(new ItemKey(12, 25, 360, 176), "Dad's courtyard chest", ItemProvider.Get(EInventoryEquipmentType.EmpressCoat), EmperorsTower & R.UpwardDash);
			Add(new ItemKey(12, 22, 56, 192), "Galactic sage room", ItemProvider.Get(EItemType.MaxSand), EmperorsTower);
			Add(new ItemKey(12, 9, 344, 928), "Bottom of Dad's right tower", ItemProvider.Get(EInventoryUseItemType.FutureHiEther), EmperorsTower);
			Add(new ItemKey(12, 19, 72, 192), "Wayyyy up there", ItemProvider.Get(EInventoryEquipmentType.FiligreeClasp), EmperorsTower & DoubleJumpOfNpc);
			Add(new ItemKey(12, 13, 120, 176), "Dad's left tower balcony", ItemProvider.Get(EItemType.MaxHP), EmperorsTower);
			Add(new ItemKey(12, 11, 264, 208), "Dad's Chambers chest", ItemProvider.Get(EInventoryRelicType.EmpireBrooch), EmperorsTower);
			Add(new ItemKey(12, 11, 136, 205), "Dad's Chambers pedestal", ItemProvider.Get(EInventoryOrbType.Empire, EOrbSlot.Melee), EmperorsTower);
		}

		void AddCantoran()
		{
			areaName = "Upper Lake Serene";
			Add(new RoomItemKey(7, 5), "Cantoran", ItemProvider.Get(EInventoryOrbType.Barrier, EOrbSlot.Melee), LeftSideForestCaves);
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
			Add(new ItemKey(3, 15, 248, 112), "Forest bat jump ledge", ItemProvider.Get(EItemType.MaxAura), AccessToPast & (DoubleJumpOfNpc | ForwardDashDoubleJump | (R.TimeStop & R.ForwardDash)));
			Add(new ItemKey(3, 21, 120, 192), "Forest green platform secret", ItemProvider.Get(EItemType.MaxSand), AccessToPast & OculusRift);
			Add(new ItemKey(3, 12, 776, 560), "Forest rats guarded chest", ItemProvider.Get(EInventoryEquipmentType.PointyHat), AccessToPast);
			Add(new ItemKey(3, 11, 392, 608), "Waterfall chest 1", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), AccessToPast & R.Swimming);
			Add(new ItemKey(3, 5, 184, 192), "Waterfall chest 2", ItemProvider.Get(EInventoryEquipmentType.Pendulum), AccessToPast & R.Swimming);
			Add(new ItemKey(3, 2, 584, 368), "Forest batcave", ItemProvider.Get(EInventoryUseItemType.Potion), AccessToPast);
			Add(new ItemKey(4, 20, 264, 160), "In the moat", ItemProvider.Get(EItemType.MaxAura), AccessToPast);
			Add(new ItemKey(3, 29, 248, 192), "Before Serene single bat cave", ItemProvider.Get(EItemType.MaxHP), LeftSideForestCaves);
			areaName = "Upper Lake Serene";
			Add(new ItemKey(7, 16, 152, 96), "Upper Serene rat nest", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLakeSirine);
			Add(new ItemKey(7, 19, 248, 96), "Upper Serene double jump cave platform", ItemProvider.Get(EItemType.MaxAura), UpperLakeSirine & R.DoubleJump);
			Add(new ItemKey(7, 19, 168, 240), "Upper Serene double jump cave floor", ItemProvider.Get(EInventoryEquipmentType.TravelersCloak), UpperLakeSirine);
			Add(new ItemKey(7, 27, 184, 144), "Upper Serene cave secret", ItemProvider.Get(EInventoryFamiliarType.Griffin), UpperLakeSirine & OculusRift);
			Add(new RoomItemKey(7, 28), "Before Big Bird", ItemProvider.Get(EInventoryUseItemType.AlchemistTools), UpperLakeSirine);
			Add(new ItemKey(7, 13, 56, 176), "Serene behind the vines", ItemProvider.Get(EInventoryUseItemType.WarpCard), UpperLakeSirine);
			Add(new ItemKey(7, 30, 296, 176), "Pyramid keys room", ItemProvider.Get(EInventoryRelicType.PyramidsKey), UpperLakeSirine);
			Add(new ItemKey(7, 3, 120, 204), "Chicken ledge", null, UpperLakeSirine);
			areaName = "Lower Lake Serene";
			Add(new ItemKey(7, 3, 440, 1232), "Deep dive", ItemProvider.Get(EInventoryUseItemType.Potion), LowerLakeSirine);
			Add(new ItemKey(7, 7, 1432, 576), "Under the eels", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerLakeSirine);
			Add(new ItemKey(7, 6, 520, 496), "Water spikes room", ItemProvider.Get(EInventoryUseItemType.Potion), LowerLakeSirine);
			Add(new ItemKey(7, 11, 88, 240), "Underwater secret", ItemProvider.Get(EItemType.MaxHP), LowerLakeSirine & OculusRift);
			Add(new ItemKey(7, 2, 1016, 384), "T chest", ItemProvider.Get(EInventoryUseItemType.Ether), LowerLakeSirine);
			Add(new ItemKey(7, 20, 248, 96), "Past the eels", ItemProvider.Get(EItemType.MaxSand), LowerLakeSirine);
			Add(new ItemKey(7, 9, 584, 189), "Underwater pedestal", ItemProvider.Get(EInventoryOrbType.Ice, EOrbSlot.Melee), LowerLakeSirine);
			areaName = "Caves of Banishment (Maw)";
			Add(new ItemKey(8, 19, 664, 704), "Banishment shroom jump room", ItemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment & R.DoubleJump);
			Add(new ItemKey(8, 12, 280, 160), "Banishment secret room", ItemProvider.Get(EItemType.MaxHP), LowerCavesOfBanishment & OculusRift);
			Add(new ItemKey(8, 48, 104, 160), "Banishment bottom left", ItemProvider.Get(EInventoryUseItemType.Spaghetti), LowerCavesOfBanishment); //Default item is Herb but got replaced by Spaghetti
			Add(new ItemKey(8, 39, 88, 192), "Banishment single shroom room", ItemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment);
			Add(new ItemKey(8, 41, 168, 192), "Banishment jackpot room chest 1", ItemProvider.Get(EInventoryUseItemType.GoldNecklace), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 216, 192), "Banishment jackpot room chest 2", ItemProvider.Get(EInventoryUseItemType.GoldRing), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 264, 192), "Banishment jackpot room chest 3", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 312, 192), "Banishment jackpot room chest 4", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 42, 216, 189), "Banishment pedestal", ItemProvider.Get(EInventoryOrbType.Wind, EOrbSlot.Melee), LowerCavesOfBanishment);
			Add(new ItemKey(8, 15, 248, 192), "Last chance before Maw", ItemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment & R.DoubleJump);
			Add(new RoomItemKey(8, 21), "Plasma Crystal", ItemProvider.Get(EInventoryUseItemType.RadiationCrystal), LowerCavesOfBanishment & (MawGassMask | R.ForwardDash));
			Add(new ItemKey(8, 31, 88, 400), "Mineshaft", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerCavesOfBanishment & MawGassMask);
			areaName = "Caves of Banishment (Sirens)";
			Add(new ItemKey(8, 4, 664, 144), "Wyvern room", ItemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 808, 144), "Upper banishment above sirens", ItemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 744, 560), "Under banishment sirens left", ItemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 3, 1848, 576), "Under banishment sirens right", ItemProvider.Get(EItemType.MaxAura), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 3, 1256, 544), "Under banishment sirens right ground", ItemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 5, 88, 496), "Banishment water hook", ItemProvider.Get(EItemType.MaxSand), UpperCavesOfBanishment & R.Swimming);
			areaName = "Castle Ramparts";
			Add(new ItemKey(4, 1, 456, 160), "Castle bomber chest", ItemProvider.Get(EItemType.MaxSand), CastleRamparts & MultipleSmallJumpsOfNpc);
			Add(new ItemKey(4, 3, 136, 144), "Ramparts Freeze the engineer", ItemProvider.Get(EItemType.MaxHP), CastleRamparts & (R.TimeStop | R.ForwardDash));
			Add(new ItemKey(4, 10, 56, 192), "Ramparts Giantess guarded room", ItemProvider.Get(EInventoryUseItemType.HiPotion), CastleRamparts);
			Add(new ItemKey(4, 11, 344, 192), "Ramparts Knight and archer guarded room", ItemProvider.Get(EInventoryUseItemType.HiPotion), CastleRamparts);
			Add(new ItemKey(4, 22, 104, 189), "Ramparts pedestal", ItemProvider.Get(EInventoryOrbType.Iron, EOrbSlot.Melee), CastleRamparts);
			areaName = "Castle Keep";
			Add(new ItemKey(5, 9, 104, 189), "Castle basement secret pedestal", ItemProvider.Get(EInventoryOrbType.Blood, EOrbSlot.Melee), CastleKeep & OculusRift);
			Add(new ItemKey(5, 10, 104, 192), "Clean the castle basement", ItemProvider.Get(EInventoryFamiliarType.Sprite), CastleKeep);
			Add(new ItemKey(5, 14, 88, 208), "Yas queen room", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), CastleKeep & R.PinkOrb & R.DoubleJump);
			Add(new ItemKey(5, 44, 216, 192), "Castle basement giantess", ItemProvider.Get(EInventoryUseItemType.Potion), CastleKeep);
			Add(new ItemKey(5, 45, 104, 192), "Omelette chest", ItemProvider.Get(EItemType.MaxHP), CastleKeep);
			Add(new ItemKey(5, 15, 296, 192), "Just an egg", ItemProvider.Get(EItemType.MaxAura), CastleKeep);
			Add(new ItemKey(5, 41, 72, 160), "Under the twins", ItemProvider.Get(EInventoryEquipmentType.BuckleHat), CastleKeep);
			Add(new ItemKey(5, 20, 504, 48), "Advisor jump", null, CastleKeep & R.TimeStop);
			Add(new RoomItemKey(5, 5), "Twins", ItemProvider.Get(EInventoryRelicType.DoubleJump), CastleKeep & R.TimeStop);
			Add(new ItemKey(5, 22, 312, 176), "Royal guard tiny room", ItemProvider.Get(EItemType.MaxSand), CastleKeep & ((R.TimeStop & R.ForwardDash) | R.DoubleJump));
			areaName = "Royal Towers";
			Add(new ItemKey(6, 19, 200, 176), "Royal towers floor secret", ItemProvider.Get(EItemType.MaxAura), RoyalTower & R.DoubleJump & OculusRift);
			Add(new ItemKey(6, 27, 472, 384), "Royal towers pre-climb gap", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), MidRoyalTower);
			Add(new ItemKey(6, 1, 1512, 288), "Royal towers long balcony", ItemProvider.Get(EInventoryUseItemType.Potion), MidRoyalTower);
			Add(new ItemKey(6, 25, 360, 176), "Next to bottom royal tower struggle juggle", ItemProvider.Get(EInventoryUseItemType.HiEther), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 3, 120, 208), "Bottom royal tower struggle juggle", ItemProvider.Get(EInventoryFamiliarType.Demon), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 17, 200, 112), "Top royal tower struggle juggle", ItemProvider.Get(EItemType.MaxHP), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 17, 56, 448), "No struggle required", ItemProvider.Get(EInventoryEquipmentType.VileteCrown), UpperRoyalTower);
			Add(new ItemKey(6, 17, 360, 1840), "Right tower freebie", ItemProvider.Get(EInventoryEquipmentType.MidnightCloak), MidRoyalTower);
			Add(new ItemKey(6, 13, 120, 176), "Royal towers left small balcony", ItemProvider.Get(EItemType.MaxSand), UpperRoyalTower);
			Add(new ItemKey(6, 22, 88, 208), "Royal tower left royal guard", ItemProvider.Get(EInventoryUseItemType.Ether), UpperRoyalTower);
			Add(new ItemKey(6, 11, 360, 544), "Before Aelana", ItemProvider.Get(EInventoryUseItemType.HiPotion), UpperRoyalTower);
			Add(new ItemKey(6, 23, 856, 208), "Aelana's attic", ItemProvider.Get(EInventoryEquipmentType.VileteDress), UpperRoyalTower & R.UpwardDash);
			Add(new ItemKey(6, 14, 136, 208), "Aelana's chest", ItemProvider.Get(EInventoryOrbType.Pink, EOrbSlot.Melee), UpperRoyalTower);
			Add(new ItemKey(6, 14, 184, 205), "Aelana's pedestal", ItemProvider.Get(EInventoryUseItemType.WarpCard), UpperRoyalTower);
		}

		void AddPyramidItemLocations()
		{
			areaName = "Ancient Pyramid";
			Add(new ItemKey(16, 14, 312, 192), "Why not it's right there", ItemProvider.Get(EItemType.MaxSand), LeftPyramid);
			Add(new ItemKey(16, 3, 88, 192), "Conviction guarded room", ItemProvider.Get(EItemType.MaxHP), LeftPyramid);
			Add(new ItemKey(16, 22, 200, 192), "Pit secret room", ItemProvider.Get(EItemType.MaxAura), LeftPyramid & OculusRift);
			Add(new ItemKey(16, 16, 1512, 144), "Regret chest", ItemProvider.Get(EInventoryRelicType.EssenceOfSpace), LeftPyramid & OculusRift);
			Add(new ItemKey(16, 5, 136, 192), "Nightmare Door chest", ItemProvider.Get(EInventoryEquipmentType.SelenBangle), Nightmare);
		}

		void AddGyreItemLocations()
		{
			areaName = "Temporal Gyre";
			// Wheel is not strictly required, but is in logic for anti-frustration against Nethershades
			Add(new ItemKey(14, 14, 200, 832), "Gyre Chest 1", null, TemporalGyre);
			Add(new ItemKey(14, 17, 200, 832), "Gyre Chest 2", null, TemporalGyre);
			Add(new ItemKey(14, 20, 200, 832), "Gyre Chest 3", null, TemporalGyre);
			Add(new ItemKey(14, 8, 120, 176), "Ravenlord Entry", null, RavenlordsLair);
			Add(new ItemKey(14, 9, 200, 125), "Ravenlord Pedestal", null, RavenlordsLair);
			Add(new ItemKey(14, 9, 280, 176), "Ravenlord Exit", null, RavenlordsLair);
			// Ifrit is a strong early boss, access to the past is required as a safety check so that they do not block past access
			Add(new ItemKey(14, 6, 40, 208), "Ifrit Entry", null, IfritsLair);
			Add(new ItemKey(14, 7, 200, 205), "Ifrit Pedestal", null, IfritsLair);
			Add(new ItemKey(14, 7, 280, 208), "Ifrit Exit", null, IfritsLair);
		}

		void AddDownloadTerminals()
		{
			areaName = "Library";
			Add(new ItemKey(2, 44, 792, 592), "Library terminal 1", null, LeftLibrary & R.Tablet);
			Add(new ItemKey(2, 44, 120, 368), "Library terminal 2", null, LeftLibrary & R.Tablet);
			Add(new ItemKey(2, 44, 456, 368), "Library terminal 3", null, LeftLibrary & R.Tablet);
			Add(new ItemKey(2, 58, 152, 208), "V terminal 1", null, LeftLibrary & R.Tablet & R.CardV);
			Add(new ItemKey(2, 58, 232, 208), "V terminal 2", null, LeftLibrary & R.Tablet & R.CardV);
			Add(new ItemKey(2, 58, 312, 208), "V terminal 3", null, LeftLibrary & R.Tablet & R.CardV);
			areaName = "Library top";
			Add(new ItemKey(2, 44, 568, 176), "Backer room terminal", null, UpperLeftLibrary & R.Tablet);
			areaName = "Varndagroth Tower right";
			Add(new ItemKey(2, 18, 200, 192), "Medbay", null, RightSideLibraryElevator & R.CardB & R.Tablet);
			areaName = "The lab";
			Add(new ItemKey(11, 6, 200, 192), "Chest and download terminal", null, UpperLab & R.Tablet);
			Add(new ItemKey(11, 15, 152, 176), "Lab terminal middle", null, TheLabPoweredOff & R.Tablet);
			Add(new ItemKey(11, 16, 600, 192), "Sentry platform terminal", null, TheLabPoweredOff & R.Tablet);
			Add(new ItemKey(11, 34, 200, 192), "Experiment 13 terminal", null, TheLab & R.Tablet);
			Add(new ItemKey(11, 37, 200, 192), "Lab terminal left", null, TheLab & R.Tablet);
			Add(new ItemKey(11, 38, 120, 176), "Lab terminal right", null, TheLabPoweredOff & R.Tablet);
		}

		void AddLoreLocations()
		{
			// Memories
			areaName = "LakeDesolation";
			Add(new ItemKey(1, 10, 312, 81), "Memory - Coyote Jump (Time Messenger)", null, LowerLakeDesolationBridge);
			areaName = "Library";
			Add(new ItemKey(2, 5, 200, 145), "Memory - Waterway (A Message)", null, LeftLibrary);
			Add(new ItemKey(2, 45, 344, 145), "Memory - Library Gap (Lachiemi Sun)", null, UpperLeftLibrary);
			Add(new ItemKey(2, 51, 88, 177), "Memory - Mr. Hat Portrait (Moonlit Night)", null, UpperLeftLibrary);
			areaName = "Varndagroth Tower Left";
			Add(new ItemKey(2, 25, 216, 145), "Memory - Left Elevator (Nomads)", null, MidLibrary & R.CardE);
			areaName = "Varndagroth Tower Right";
			Add(new ItemKey(2, 46, 200, 145), "Memory - Siren Elevator (Childhood)", null, MidLibrary & R.CardB);
			Add(new ItemKey(2, 11, 200, 161), "Memory - Varndagroth Right Bottom (Faron)", null, LowerRightSideLibrary);
			areaName = "Military Hangar";
			Add(new ItemKey(10, 3, 536, 97), "Memory - Bomber Climb (A Solution)", null, MilitaryFortress & DoubleJumpOfNpc & R.TimespinnerWheel);
			areaName = "The Lab";
			Add(new ItemKey(11, 7, 248, 129), "Memory - Genza's Secret Stash 1 (An Old Friend)", null, TheLab & OculusRift);
			Add(new ItemKey(11, 7, 296, 129), "Memory - Genza's Secret Stash 2 (Twilight Dinner)", null, TheLab & OculusRift);
			areaName = "Emperor's Tower";
			Add(new ItemKey(12, 19, 56, 145), "Memory - Way Up There (Final Circle)", null, EmperorsTower & DoubleJumpOfNpc);
			// Letters
			areaName = "Forest";
			Add(new ItemKey(3, 12, 472, 161), "Letter - Forest Rats (Lachiem Expedition)", null, AccessToPast);
			Add(new ItemKey(3, 15, 328, 97), "Letter - Forest Bat Jump Ledge (Peace Treaty)", null, AccessToPast & (DoubleJumpOfNpc | ForwardDashDoubleJump | (R.TimeStop & R.ForwardDash)));
			areaName = "Castle Ramparts";
			Add(new ItemKey(4, 18, 456, 497), "Letter - Floating in Moat (Prime Edicts)", null, CastleRamparts);
			Add(new ItemKey(4, 11, 360, 161), "Letter - Archer + Knight (Declaration of Independence)", null, CastleRamparts);
			areaName = "Castle Keep";
			Add(new ItemKey(5, 41, 184, 177), "Letter - Under the Twins (Letter of Reference)", null, CastleKeep);
			Add(new ItemKey(5, 44, 264, 161), "Letter - Castle Loop Giantess (Political Advice)", null, CastleKeep);
			Add(new ItemKey(5, 14, 568, 177), "Letter - Aleana's Room (Diplomatic Missive)", null, CastleKeep & R.PinkOrb & R.DoubleJump);
			areaName = "Royal Towers";
			Add(new ItemKey(6, 17, 344, 433), "Letter - Top Struggle Juggle Base (War of the Sisters)", null, UpperRoyalTower);
			Add(new ItemKey(6, 14, 136, 177), "Letter - Aleana Boss (Stained Letter)", null, UpperRoyalTower);
			Add(new ItemKey(6, 25, 152, 145), "Letter - Near Bottom Struggle Juggle (Mission Findings)", null, UpperRoyalTower & DoubleJumpOfNpc);
			areaName = "Caves of Banishment (Maw)";
			Add(new ItemKey(8, 36, 136, 145), "Letter - Lower Left Maw Caves (Naïvety)", null, LowerCavesOfBanishment);
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

				var reachableProgressionItemLocations = GetReachableProgressionItemLocations(obtainedRequirements);
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

		ItemLocation[] GetReachableProgressionItemLocations(R obtainedRequirements)
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

			var pickedUpSingleItemLocationUnlocks = pickedUpProgressionItemLocations
				.Where(l => !(l.ItemInfo is ProgressiveItemInfo))
				.Select(l => l.ItemInfo.Unlocks);

			var pickedUpProgressiveItemLocationUnlocks = pickedUpProgressionItemLocations
				.Where(l => l.ItemInfo is ProgressiveItemInfo)
				.Select(l => ((ProgressiveItemInfo)l.ItemInfo)
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
				.Where(l => l.ItemInfo is ProgressiveItemInfo)
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
				.Where(l => !(l.ItemInfo is ProgressiveItemInfo))
				.Select(l => l.ItemInfo.Unlocks)
				.Aggregate(R.None, (current, unlock) => current | unlock);

			var progressiveItemsPerType = reachableLocations
				.Where(l => l.ItemInfo is ProgressiveItemInfo)
				.GroupBy(l => l.ItemInfo as ProgressiveItemInfo);

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
				.Where(l => !(l.ItemInfo is ProgressiveItemInfo))
				.Select(l => l.ItemInfo.Unlocks)
				.Aggregate(R.None, (current, unlock) => current | unlock);

			var progressiveItemsPerType = reachableLocations
				.Where(l => l.ItemInfo is ProgressiveItemInfo)
				.GroupBy(l => l.ItemInfo as ProgressiveItemInfo);

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
			=> this.Where(l => l.Gate.CanBeOpenedWith(obtainedRequirements));

		bool CanCompleteGame(R obtainedRequirements)
			=> Nightmare.CanBeOpenedWith(obtainedRequirements);

		public virtual void Update(Level level)
		{
		}

		public virtual void Initialize(GameSave gameSave)
		{
			var progressiveItemInfos = this
				.Where(l => l.ItemInfo is ProgressiveItemInfo)
				.Select(l => (ProgressiveItemInfo)l.ItemInfo);

			foreach (var progressiveItem in progressiveItemInfos)
				progressiveItem.Reset();

			foreach (var itemLocation in this)
				itemLocation.BaseOnGameSave(gameSave);
		}

		protected void Add(ItemKey itemKey, string name, ItemInfo defaultItem)
			=> Add(new ItemLocation(itemKey, areaName, name, defaultItem));

		protected void Add(ItemKey itemKey, string name, ItemInfo defaultItem, Gate gate)
			=> Add(new ItemLocation(itemKey, areaName, name, defaultItem, gate));
	}

	class ProgressionChain
	{
		public IEnumerable<ItemLocation> Locations { get; set; }
		public ProgressionChain Sub { get; set; }
	}
}
