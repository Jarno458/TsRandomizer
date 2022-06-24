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
		internal R MawGasMask;

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
				Add(new ExternalItemLocation(itemInfoProvider.Get(EInventoryRelicType.Dash)));
		}

		void SetupGates()
		{
			OculusRift = (SeedOptions.EyeSpy)
				? R.OculusRift
				: R.None;

			MawGasMask = (SeedOptions.GasMaw)
				? R.GasMask
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
				| (MawGasMask & (R.GateCavesOfBanishment | R.GateMaw)
				| R.GateCavesOfBanishment & R.Swimming
				| R.GateMaw & R.DoubleJump & R.Swimming);

			MultipleSmallJumpsOfNpc = (Gate)(R.TimespinnerWheel | R.UpwardDash);
			DoubleJumpOfNpc = (R.DoubleJump & R.TimespinnerWheel) | R.UpwardDash;
			ForwardDashDoubleJump = (R.ForwardDash & R.DoubleJump) | R.UpwardDash;

			//past
			LeftSideForestCaves = 
				(AccessToPast & (R.TimeStop | R.ForwardDash)) 
				| R.GateLakeSereneRight 
				| R.GateLakeSereneLeft
				| R.GateCavesOfBanishment & R.Swimming
				| R.GateMaw & R.DoubleJump & R.Swimming;
			UpperLakeSirine = (LeftSideForestCaves & (R.TimeStop | R.Swimming)) | R.GateLakeSereneLeft;
			LowerLakeSirine = (LeftSideForestCaves | R.GateLakeSereneLeft) & R.Swimming;
			LowerCavesOfBanishment = LowerLakeSirine | R.GateCavesOfBanishment | (R.GateMaw & R.DoubleJump);
			UpperCavesOfBanishment = AccessToPast;
			CastleRamparts = AccessToPast;
			CastleKeep = CastleRamparts;
			RoyalTower = (CastleKeep & R.DoubleJump) | R.GateRoyalTowers;
			MidRoyalTower = RoyalTower & (MultipleSmallJumpsOfNpc | ForwardDashDoubleJump);
			UpperRoyalTower = MidRoyalTower & R.DoubleJump;
			KillMaw = (LowerLakeSirine | R.GateCavesOfBanishment | R.GateMaw) & MawGasMask;
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
			Add(ItemKey.TutorialMeleeOrb, "Tutorial: Yo Momma 1", ItemProvider.Get(EInventoryOrbType.Blue, EOrbSlot.Melee));
			Add(ItemKey.TutorialSpellOrb, "Tutorial: Yo Momma 2", ItemProvider.Get(EInventoryOrbType.Blue, EOrbSlot.Spell));
			areaName = "Lake Desolation";
			Add(new ItemKey(1, 1, 1528, 144), "Lake Desolation: Starter chest 2", ItemProvider.Get(EInventoryUseItemType.FuturePotion), AccessToLakeDesolation);
			Add(new ItemKey(1, 15, 264, 144), "Lake Desolation: Starter chest 3", ItemProvider.Get(EInventoryEquipmentType.OldCoat), AccessToLakeDesolation);
			Add(new ItemKey(1, 25, 296, 176), "Lake Desolation: Starter chest 1", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), AccessToLakeDesolation);
			Add(new ItemKey(1, 9, 600, 144 + TimespinnerWheel.YOffset), "Lake Desolation (Lower): Timespinner Wheel room", ItemProvider.Get(EInventoryRelicType.TimespinnerWheel), AccessToLakeDesolation);
			Add(new ItemKey(1, 14, 40, 176), "Lake Desolation: Forget me not chest", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation);
			areaName = "Lower Lake Desolation";
			Add(new ItemKey(1, 2, 1016, 384), "Lake Desolation (Lower): Chicken chest", ItemProvider.Get(EItemType.MaxSand), AccessToLakeDesolation & R.TimeStop);
			Add(new ItemKey(1, 11, 72, 240), "Lake Desolation (Lower): Not so secret room", ItemProvider.Get(EItemType.MaxHP), LowerLakeDesolationBridge & OculusRift);
			Add(new ItemKey(1, 3, 56, 176), "Lake Desolation (Upper): Tank chest", ItemProvider.Get(EItemType.MaxAura), AccessToLakeDesolation & R.TimeStop);
			areaName = "Upper Lake Desolation";
			Add(new ItemKey(1, 17, 152, 96), "Lake Desolation (Upper): Oxygen recovery room", ItemProvider.Get(EInventoryUseItemType.GoldRing), UpperLakeDesolation);
			Add(new ItemKey(1, 21, 200, 144), "Lake Desolation (Upper): Secret room", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation & OculusRift);
			Add(new ItemKey(1, 20, 232, 96), "Lake Desolation (Upper): Double jump cave platform", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLakeDesolation & R.DoubleJump);
			Add(new ItemKey(1, 20, 168, 240), "Lake Desolation (Upper): Double jump cave floor", ItemProvider.Get(EInventoryUseItemType.FuturePotion), UpperLakeDesolation);
			Add(new ItemKey(1, 22, 344, 160), "Lake Desolation (Upper): Sparrow chest", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1320, 189), "Lake Desolation (Upper): Crash site pedestal", ItemProvider.Get(EInventoryOrbType.Moon, EOrbSlot.Melee), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1272, 192), "Lake Desolation (Upper): Crash site chest 1", ItemProvider.Get(EInventoryEquipmentType.CaptainsCap), UpperLakeDesolation & R.GasMask & KillMaw);
			Add(new ItemKey(1, 18, 1368, 192), "Lake Desolation (Upper): Crash site chest 2", ItemProvider.Get(EInventoryEquipmentType.CaptainsJacket), UpperLakeDesolation & R.GasMask & KillMaw);
			Add(new RoomItemKey(1, 5), "Lake Desolation: Kitty Boss", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Melee), UpperLakeDesolation | LowerLakeDesolationBridge);
			areaName = "Library";
			Add(new ItemKey(2, 60, 328, 160), "Library: Basement", ItemProvider.Get(EItemType.MaxHP), LeftLibrary);
			Add(new ItemKey(2, 54, 296, 176), "Library: Warp gate", ItemProvider.Get(EInventoryRelicType.ScienceKeycardD), LeftLibrary);
			Add(new ItemKey(2, 41, 404, 246), "Library: Librarian", ItemProvider.Get(EInventoryRelicType.Tablet), LeftLibrary);
			Add(new ItemKey(2, 44, 680, 368), "Library: Reading nook chest", ItemProvider.Get(EInventoryRelicType.FoeScanner), LeftLibrary);
			Add(new ItemKey(2, 47, 216, 208), "Library: Storage room chest 1", ItemProvider.Get(EInventoryUseItemType.Ether), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 152, 208), "Library: Storage room chest 2", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Passive), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 88, 208), "Library: Storage room chest 3", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Spell), LeftLibrary & R.CardD);
			areaName = "Library Top";
			Add(new ItemKey(2, 56, 168, 192), "Library: Backer room chest 5", ItemProvider.Get(EInventoryUseItemType.GoldNecklace), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 392, 192), "Library: Backer room chest 4", ItemProvider.Get(EInventoryUseItemType.GoldRing), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 616, 192), "Library: Backer room chest 3", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 840, 192), "Library: Backer room chest 2", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 1064, 192), "Library: Backer room chest 1", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLeftLibrary);
			areaName = "Varndagroth Tower Left";
			Add(new ItemKey(2, 34, 232, 1200), "Varndagroth Towers (Left): Elevator Key not required", ItemProvider.Get(EInventoryUseItemType.FiligreeTea), MidLibrary); //Default item is Jerky, got replaced by FiligreeTea
			Add(new ItemKey(2, 40, 344, 176), "Varndagroth Towers (Left): Ye olde Timespinner", ItemProvider.Get(EInventoryRelicType.ScienceKeycardC), MidLibrary);
			Add(new ItemKey(2, 32, 328, 160), "Varndagroth Towers (Left): Bottom floor", ItemProvider.Get(EInventoryUseItemType.GoldRing), MidLibrary & R.CardC);
			Add(new ItemKey(2, 7, 232, 144), "Varndagroth Towers (Left): Air vents secret", ItemProvider.Get(EItemType.MaxAura), MidLibrary & OculusRift);
			Add(new ItemKey(2, 25, 328, 192), "Varndagroth Towers (Left): Elevator chest", ItemProvider.Get(EItemType.MaxSand), MidLibrary & R.CardE);
			areaName = "Varndagroth Tower Right";
			Add(new ItemKey(2, 15, 760, 192), "Varndagroth Towers: Bridge", ItemProvider.Get(EInventoryUseItemType.FuturePotion), UpperRightSideLibrary);
			Add(new ItemKey(2, 20, 72, 1200), "Varndagroth Towers (Right): Elevator chest", ItemProvider.Get(EInventoryUseItemType.Jerky), RightSideLibraryElevator);
			Add(new ItemKey(2, 23, 72, 560), "Varndagroth Towers (Right): Elevator card chest", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 1112, 112), "Varndagroth Towers (Right): Air vents right chest", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 136, 304), "Varndagroth Towers (Right): Air vents left chest", ItemProvider.Get(EInventoryRelicType.ElevatorKeycard), UpperRightSideLibrary & (R.CardE | R.DoubleJump)); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 11, 104, 192), "Varndagroth Towers (Right): Bottom floor", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), LowerRightSideLibrary);
			Add(new RoomItemKey(2, 29), "Varndagroth Towers (Right): Varndagroth", ItemProvider.Get(EInventoryRelicType.TimespinnerSpindle), RightSideLibraryElevator & R.CardC);
			Add(new RoomItemKey(2, 52), "Varndagroth Towers (Right): Spider Hell", ItemProvider.Get(EInventoryRelicType.TimespinnerGear2), RightSideLibraryElevator & R.CardA);
			areaName = "Sealed Caves (Xarion)";
			Add(new ItemKey(9, 10, 248, 848), "Sealed Caves (Xarion): Skeleton", ItemProvider.Get(EInventoryRelicType.ScienceKeycardB), SealedCavesLeft);
			Add(new ItemKey(9, 19, 664, 704), "Sealed Caves (Xarion): Shroom jump room", ItemProvider.Get(EInventoryUseItemType.Antidote), SealedCavesLower & R.TimeStop);
			Add(new ItemKey(9, 39, 88, 192), "Sealed Caves (Xarion): Double shroom room", ItemProvider.Get(EInventoryUseItemType.Antidote), SealedCavesLower);
			Add(new ItemKey(9, 41, 312, 192), "Sealed Caves (Xarion): Mini jackpot room", ItemProvider.Get(EInventoryUseItemType.GalaxyStone), SealedCavesLower & ForwardDashDoubleJump);
			Add(new ItemKey(9, 42, 328, 192), "Sealed Caves (Xarion): Below mini jackpot room", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), SealedCavesLower);
			Add(new ItemKey(9, 12, 280, 160), "Sealed Caves (Xarion): Secret room", ItemProvider.Get(EItemType.MaxHP), SealedCavesLower & OculusRift);
			Add(new ItemKey(9, 48, 104, 160), "Sealed Caves (Xarion): Bottom left room", ItemProvider.Get(EInventoryUseItemType.FutureEther), SealedCavesLower);
			Add(new ItemKey(9, 15, 248, 192), "Sealed Caves (Xarion): Last chance before Xarion", ItemProvider.Get(EInventoryUseItemType.FutureEther), SealedCavesLower & R.DoubleJump);
			Add(new RoomItemKey(9, 13), "Sealed Caves (Xarion): Xarion", ItemProvider.Get(EInventoryRelicType.TimespinnerGear3), SealedCavesLower);
			areaName = "Sealed Caves (Sirens)";
			Add(new ItemKey(9, 5, 88, 496), "Sealed Caves (Sirens): Water hook", ItemProvider.Get(EItemType.MaxSand), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 3, 1848, 576), "Sealed Caves (Sirens): Siren room underwater right", ItemProvider.Get(EInventoryEquipmentType.BirdStatue), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 3, 744, 560), "Sealed Caves (Sirens): Siren room underwater left", ItemProvider.Get(EItemType.MaxAura), SealedCavesSirens & R.Swimming);
			Add(new ItemKey(9, 2, 184, 176), "Sealed Caves (Sirens): Cave after sirens chest 1", ItemProvider.Get(EInventoryUseItemType.WarpCard), SealedCavesSirens);
			Add(new ItemKey(9, 2, 104, 160), "Sealed Caves (Sirens): Cave after sirens chest 2", ItemProvider.Get(EInventoryRelicType.WaterMask), SealedCavesSirens);
			areaName = "Military Fortress";
			Add(new ItemKey(10, 3, 264, 128), "Military Fortress: Bomber chest", ItemProvider.Get(EItemType.MaxSand), MilitaryFortress & DoubleJumpOfNpc & R.TimespinnerWheel); //can be reached with just upward dash but not with lightwall unless you got timestop
			Add(new ItemKey(10, 11, 296, 192), "Military Fortress: Close combat room", ItemProvider.Get(EItemType.MaxAura), MilitaryFortress);
			Add(new ItemKey(10, 4, 1064, 176), "Military Fortress: Soldiers bridge", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), MilitaryFortressHangar);
			Add(new ItemKey(10, 10, 104, 192), "Military Fortress: Giantess room", ItemProvider.Get(EInventoryRelicType.AirMask), MilitaryFortressHangar);
			Add(new ItemKey(10, 8, 1080, 176), "Military Fortress: Giantess bridge", ItemProvider.Get(EInventoryEquipmentType.LabGlasses), MilitaryFortressHangar);
			Add(new ItemKey(10, 7, 104, 192), "Military Fortress: B door chest 2", ItemProvider.Get(EInventoryUseItemType.PlasmaIV), RightSideMilitaryFortressHangar & R.CardB);
			Add(new ItemKey(10, 7, 152, 192), "Military Fortress: B door chest 1", ItemProvider.Get(EItemType.MaxSand), RightSideMilitaryFortressHangar & R.CardB);
			Add(new ItemKey(10, 18, 280, 189), "Military Fortress: Pedestal", ItemProvider.Get(EInventoryOrbType.Gun, EOrbSlot.Melee), RightSideMilitaryFortressHangar & (DoubleJumpOfNpc | ForwardDashDoubleJump));
			areaName = "The Lab";
			Add(new ItemKey(11, 36, 312, 192), "Lab: Coffee break", ItemProvider.Get(EInventoryUseItemType.FoodSynth), TheLab);
			Add(new ItemKey(11, 3, 1528, 192), "Lab: Lower trash right", ItemProvider.Get(EItemType.MaxHP), TheLab & R.DoubleJump);
			Add(new ItemKey(11, 3, 72, 192), "Lab: Lower trash left", ItemProvider.Get(EInventoryUseItemType.FuturePotion), TheLab & R.UpwardDash); //when lab power is on, it only requires DoubleJumpOfNpc, but we cant code for the power state
			Add(new ItemKey(11, 25, 104, 192), "Lab: Below lab entrance", ItemProvider.Get(EItemType.MaxAura), TheLab & R.DoubleJump);
			Add(new ItemKey(11, 18, 824, 128), "Lab: Trash jump room", ItemProvider.Get(EInventoryUseItemType.ChaosHeal), TheLabPoweredOff);
			Add(new RoomItemKey(11, 39), "Lab: Dynamo Works", ItemProvider.Get(EInventoryOrbType.Eye, EOrbSlot.Melee), TheLabPoweredOff);
			Add(new RoomItemKey(11, 21), "Lab: Genza (Blob Mom)", ItemProvider.Get(EInventoryRelicType.ScienceKeycardA), UpperLab);
			Add(new RoomItemKey(11, 1), "Lab: Experiment #13", ItemProvider.Get(EInventoryRelicType.Dash), TheLabPoweredOff);
			Add(new ItemKey(11, 6, 328, 192), "Lab: Download and chest room chest", ItemProvider.Get(EInventoryEquipmentType.LabCoat), UpperLab);
			Add(new ItemKey(11, 27, 296, 160), "Lab: Lab secret", ItemProvider.Get(EItemType.MaxSand), UpperLab & OculusRift);
			Add(new RoomItemKey(11, 26), "Lab: Spider Hell", ItemProvider.Get(EInventoryRelicType.TimespinnerGear1), TheLabPoweredOff & R.CardA);
			areaName = "Emperor's Tower";
			Add(new ItemKey(12, 5, 344, 192), "Emperor's Tower: Courtyard bottom chest", ItemProvider.Get(EItemType.MaxAura), EmperorsTower);
			Add(new ItemKey(12, 3, 200, 160), "Emperor's Tower: Courtyard floor secret", ItemProvider.Get(EInventoryEquipmentType.LachiemCrown), EmperorsTower & R.UpwardDash & OculusRift);
			Add(new ItemKey(12, 25, 360, 176), "Emperor's Tower: Courtyard upper chest", ItemProvider.Get(EInventoryEquipmentType.EmpressCoat), EmperorsTower & R.UpwardDash);
			Add(new ItemKey(12, 22, 56, 192), "Emperor's Tower: Galactic sage room", ItemProvider.Get(EItemType.MaxSand), EmperorsTower);
			Add(new ItemKey(12, 9, 344, 928), "Emperor's Tower: Bottom right tower", ItemProvider.Get(EInventoryUseItemType.FutureHiEther), EmperorsTower);
			Add(new ItemKey(12, 19, 72, 192), "Emperor's Tower: Wayyyy up there", ItemProvider.Get(EInventoryEquipmentType.FiligreeClasp), EmperorsTower & DoubleJumpOfNpc);
			Add(new ItemKey(12, 13, 120, 176), "Emperor's Tower: Left tower balcony", ItemProvider.Get(EItemType.MaxHP), EmperorsTower);
			Add(new ItemKey(12, 11, 264, 208), "Emperor's Tower: Emperor's Chambers chest", ItemProvider.Get(EInventoryRelicType.EmpireBrooch), EmperorsTower);
			Add(new ItemKey(12, 11, 136, 205), "Emperor's Tower: Emperor's Chambers pedestal", ItemProvider.Get(EInventoryOrbType.Empire, EOrbSlot.Melee), EmperorsTower);
		}

		void AddCantoran()
		{
			areaName = "Upper Lake Serene";
			Add(new RoomItemKey(7, 5), "Lake Serene: Cantoran", ItemProvider.Get(EInventoryOrbType.Barrier, EOrbSlot.Melee), LeftSideForestCaves);
		}

		void AddPastItemLocations()
		{
			areaName = "Refugee Camp";
			Add(new RoomItemKey(3, 0), "Refugee Camp: Neliste\'s Bra", ItemProvider.Get(EInventoryOrbType.Flame, EOrbSlot.Melee), AccessToPast);
			Add(new ItemKey(3, 30, 296, 176), "Refugee Camp: Storage chest 3", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), AccessToPast);
			Add(new ItemKey(3, 30, 232, 176), "Refugee Camp: Storage chest 2", ItemProvider.Get(EInventoryUseItemType.GoldNecklace), AccessToPast);
			Add(new ItemKey(3, 30, 168, 176), "Refugee Camp: Storage chest 1", ItemProvider.Get(EInventoryRelicType.JewelryBox), AccessToPast);
			areaName = "Forest";
			Add(new ItemKey(3, 3, 648, 272), "Forest: Refugee camp roof", ItemProvider.Get(EInventoryUseItemType.Herb), AccessToPast);
			Add(new ItemKey(3, 15, 248, 112), "Forest: Bat jump ledge", ItemProvider.Get(EItemType.MaxAura), AccessToPast & (DoubleJumpOfNpc | ForwardDashDoubleJump | (R.TimeStop & R.ForwardDash)));
			Add(new ItemKey(3, 21, 120, 192), "Forest: Green platform secret", ItemProvider.Get(EItemType.MaxSand), AccessToPast & OculusRift);
			Add(new ItemKey(3, 12, 776, 560), "Forest: Rats guarded chest", ItemProvider.Get(EInventoryEquipmentType.PointyHat), AccessToPast);
			Add(new ItemKey(3, 11, 392, 608), "Forest: Waterfall chest 1", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), AccessToPast & R.Swimming);
			Add(new ItemKey(3, 5, 184, 192), "Forest: Waterfall chest 2", ItemProvider.Get(EInventoryEquipmentType.Pendulum), AccessToPast & R.Swimming);
			Add(new ItemKey(3, 2, 584, 368), "Forest: Batcave", ItemProvider.Get(EInventoryUseItemType.Potion), AccessToPast);
			Add(new ItemKey(4, 20, 264, 160), "Castle Ramparts: In the moat", ItemProvider.Get(EItemType.MaxAura), AccessToPast);
			Add(new ItemKey(3, 29, 248, 192), "Forest: Before Serene single bat cave", ItemProvider.Get(EItemType.MaxHP), LeftSideForestCaves);
			areaName = "Upper Lake Serene";
			Add(new ItemKey(7, 16, 152, 96), "Lake Serene (Upper): Rat nest", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLakeSirine);
			Add(new ItemKey(7, 19, 248, 96), "Lake Serene (Upper): Double jump cave platform", ItemProvider.Get(EItemType.MaxAura), UpperLakeSirine & R.DoubleJump);
			Add(new ItemKey(7, 19, 168, 240), "Lake Serene (Upper): Double jump cave floor", ItemProvider.Get(EInventoryEquipmentType.TravelersCloak), UpperLakeSirine);
			Add(new ItemKey(7, 27, 184, 144), "Lake Serene (Upper): Cave secret", ItemProvider.Get(EInventoryFamiliarType.Griffin), UpperLakeSirine & OculusRift);
			Add(new RoomItemKey(7, 28), "Lake Serene: Before Big Bird", ItemProvider.Get(EInventoryUseItemType.AlchemistTools), UpperLakeSirine);
			Add(new ItemKey(7, 13, 56, 176), "Lake Serene: Behind the vines", ItemProvider.Get(EInventoryUseItemType.WarpCard), UpperLakeSirine);
			Add(new ItemKey(7, 30, 296, 176), "Lake Serene: Pyramid keys room", ItemProvider.Get(EInventoryRelicType.PyramidsKey), UpperLakeSirine);
			Add(new ItemKey(7, 3, 120, 204), "Lake Serene (Upper): Chicken ledge", null, UpperLakeSirine);
			areaName = "Lower Lake Serene";
			Add(new ItemKey(7, 3, 440, 1232), "Lake Serene (Lower): Deep dive", ItemProvider.Get(EInventoryUseItemType.Potion), LowerLakeSirine);
			Add(new ItemKey(7, 7, 1432, 576), "Lake Serene (Lower): Under the eels", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerLakeSirine);
			Add(new ItemKey(7, 6, 520, 496), "Lake Serene (Lower): Water spikes room", ItemProvider.Get(EInventoryUseItemType.Potion), LowerLakeSirine);
			Add(new ItemKey(7, 11, 88, 240), "Lake Serene (Lower): Underwater secret", ItemProvider.Get(EItemType.MaxHP), LowerLakeSirine & OculusRift);
			Add(new ItemKey(7, 2, 1016, 384), "Lake Serene (Lower): T chest", ItemProvider.Get(EInventoryUseItemType.Ether), LowerLakeSirine);
			Add(new ItemKey(7, 20, 248, 96), "Lake Serene (Lower): Past the eels", ItemProvider.Get(EItemType.MaxSand), LowerLakeSirine);
			Add(new ItemKey(7, 9, 584, 189), "Lake Serene (Lower): Underwater pedestal", ItemProvider.Get(EInventoryOrbType.Ice, EOrbSlot.Melee), LowerLakeSirine);
			areaName = "Caves of Banishment (Maw)";
			Add(new ItemKey(8, 19, 664, 704), "Caves of Banishment (Maw): Shroom jump room", ItemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment & R.DoubleJump);
			Add(new ItemKey(8, 12, 280, 160), "Caves of Banishment (Maw): Secret room", ItemProvider.Get(EItemType.MaxHP), LowerCavesOfBanishment & OculusRift);
			Add(new ItemKey(8, 48, 104, 160), "Caves of Banishment (Maw): Bottom left room", ItemProvider.Get(EInventoryUseItemType.Spaghetti), LowerCavesOfBanishment); //Default item is Herb but got replaced by Spaghetti
			Add(new ItemKey(8, 39, 88, 192), "Caves of Banishment (Maw): Single shroom room", ItemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment);
			Add(new ItemKey(8, 41, 168, 192), "Caves of Banishment (Maw): Jackpot room chest 1", ItemProvider.Get(EInventoryUseItemType.GoldNecklace), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 216, 192), "Caves of Banishment (Maw): Jackpot room chest 2", ItemProvider.Get(EInventoryUseItemType.GoldRing), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 264, 192), "Caves of Banishment (Maw): Jackpot room chest 3", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 41, 312, 192), "Caves of Banishment (Maw): Jackpot room chest 4", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerCavesOfBanishment & ForwardDashDoubleJump);
			Add(new ItemKey(8, 42, 216, 189), "Caves of Banishment (Maw): Pedestal", ItemProvider.Get(EInventoryOrbType.Wind, EOrbSlot.Melee), LowerCavesOfBanishment);
			Add(new ItemKey(8, 15, 248, 192), "Caves of Banishment (Maw): Last chance before Maw", ItemProvider.Get(EInventoryUseItemType.SilverOre), LowerCavesOfBanishment & R.DoubleJump);
			Add(new RoomItemKey(8, 21), "Caves of Banishment (Maw): Plasma Crystal", ItemProvider.Get(EInventoryUseItemType.RadiationCrystal), LowerCavesOfBanishment & (MawGasMask | R.ForwardDash));
			Add(new ItemKey(8, 31, 88, 400), "Caves of Banishment (Maw): Mineshaft", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), LowerCavesOfBanishment & MawGasMask);
			areaName = "Caves of Banishment (Sirens)";
			Add(new ItemKey(8, 4, 664, 144), "Caves of Banishment (Sirens): Wyvern room", ItemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 808, 144), "Caves of Banishment (Sirens): Siren room above water chest", ItemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 744, 560), "Caves of Banishment (Sirens): Siren room underwater left chest", ItemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 3, 1848, 576), "Caves of Banishment (Sirens): Siren room underwater right chest", ItemProvider.Get(EItemType.MaxAura), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 3, 1256, 544), "Caves of Banishment (Sirens): Siren room underwater right ground", ItemProvider.Get(EInventoryUseItemType.SilverOre), UpperCavesOfBanishment & R.Swimming);
			Add(new ItemKey(8, 5, 88, 496), "Caves of Banishment (Sirens): Water hook", ItemProvider.Get(EItemType.MaxSand), UpperCavesOfBanishment & R.Swimming);
			areaName = "Castle Ramparts";
			Add(new ItemKey(4, 1, 456, 160), "Castle Ramparts: Bomber chest", ItemProvider.Get(EItemType.MaxSand), CastleRamparts & MultipleSmallJumpsOfNpc);
			Add(new ItemKey(4, 3, 136, 144), "Castle Ramparts: Freeze the engineer", ItemProvider.Get(EItemType.MaxHP), CastleRamparts & (R.TimeStop | R.ForwardDash));
			Add(new ItemKey(4, 10, 56, 192), "Castle Ramparts: Giantess guarded room", ItemProvider.Get(EInventoryUseItemType.HiPotion), CastleRamparts);
			Add(new ItemKey(4, 11, 344, 192), "Castle Ramparts: Knight and archer guarded room", ItemProvider.Get(EInventoryUseItemType.HiPotion), CastleRamparts);
			Add(new ItemKey(4, 22, 104, 189), "Castle Ramparts: Pedestal", ItemProvider.Get(EInventoryOrbType.Iron, EOrbSlot.Melee), CastleRamparts);
			areaName = "Castle Keep";
			Add(new ItemKey(5, 9, 104, 189), "Castle Keep: Basement secret pedestal", ItemProvider.Get(EInventoryOrbType.Blood, EOrbSlot.Melee), CastleKeep & OculusRift);
			Add(new ItemKey(5, 10, 104, 192), "Castle Keep: Clean the castle basement", ItemProvider.Get(EInventoryFamiliarType.Sprite), CastleKeep);
			Add(new ItemKey(5, 14, 88, 208), "Castle Keep: Yas queen room", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), CastleKeep & R.PinkOrb & R.DoubleJump);
			Add(new ItemKey(5, 44, 216, 192), "Castle Keep: Giantess guarded chest", ItemProvider.Get(EInventoryUseItemType.Potion), CastleKeep);
			Add(new ItemKey(5, 45, 104, 192), "Castle Keep: Omelette chest", ItemProvider.Get(EItemType.MaxHP), CastleKeep);
			Add(new ItemKey(5, 15, 296, 192), "Castle Keep: Just an egg", ItemProvider.Get(EItemType.MaxAura), CastleKeep);
			Add(new ItemKey(5, 41, 72, 160), "Castle Keep: Under the twins", ItemProvider.Get(EInventoryEquipmentType.BuckleHat), CastleKeep);
			Add(new ItemKey(5, 20, 504, 48), "Castle Keep: Advisor jump", null, CastleKeep & R.TimeStop);
			Add(new RoomItemKey(5, 5), "Castle Keep: Twins", ItemProvider.Get(EInventoryRelicType.DoubleJump), CastleKeep & R.TimeStop);
			Add(new ItemKey(5, 22, 312, 176), "Castle Keep: Royal guard tiny room", ItemProvider.Get(EItemType.MaxSand), CastleKeep & ((R.TimeStop & R.ForwardDash) | R.DoubleJump));
			areaName = "Royal Towers";
			Add(new ItemKey(6, 19, 200, 176), "Royal Towers: Floor secret", ItemProvider.Get(EItemType.MaxAura), RoyalTower & R.DoubleJump & OculusRift);
			Add(new ItemKey(6, 27, 472, 384), "Royal Towers: Pre-climb gap", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), MidRoyalTower);
			Add(new ItemKey(6, 1, 1512, 288), "Royal Towers: Long balcony", ItemProvider.Get(EInventoryUseItemType.Potion), MidRoyalTower);
			Add(new ItemKey(6, 25, 360, 176), "Royal Towers: Past bottom struggle juggle", ItemProvider.Get(EInventoryUseItemType.HiEther), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 3, 120, 208), "Royal Towers: Bottom struggle juggle", ItemProvider.Get(EInventoryFamiliarType.Demon), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 17, 200, 112), "Royal Towers: Top struggle juggle", ItemProvider.Get(EItemType.MaxHP), UpperRoyalTower & DoubleJumpOfNpc);
			Add(new ItemKey(6, 17, 56, 448), "Royal Towers: No struggle required", ItemProvider.Get(EInventoryEquipmentType.VileteCrown), UpperRoyalTower);
			Add(new ItemKey(6, 17, 360, 1840), "Royal Towers: Right tower freebie", ItemProvider.Get(EInventoryEquipmentType.MidnightCloak), MidRoyalTower);
			Add(new ItemKey(6, 13, 120, 176), "Royal Towers: Left tower small balcony", ItemProvider.Get(EItemType.MaxSand), UpperRoyalTower);
			Add(new ItemKey(6, 22, 88, 208), "Royal Towers: Left tower royal guard", ItemProvider.Get(EInventoryUseItemType.Ether), UpperRoyalTower);
			Add(new ItemKey(6, 11, 360, 544), "Royal Towers: Before Aelana", ItemProvider.Get(EInventoryUseItemType.HiPotion), UpperRoyalTower);
			Add(new ItemKey(6, 23, 856, 208), "Royal Towers: Aelana's attic", ItemProvider.Get(EInventoryEquipmentType.VileteDress), UpperRoyalTower & R.UpwardDash);
			Add(new ItemKey(6, 14, 136, 208), "Royal Towers: Aelana's chest", ItemProvider.Get(EInventoryOrbType.Pink, EOrbSlot.Melee), UpperRoyalTower);
			Add(new ItemKey(6, 14, 184, 205), "Royal Towers: Aelana's pedestal", ItemProvider.Get(EInventoryUseItemType.WarpCard), UpperRoyalTower);
		}

		void AddPyramidItemLocations()
		{
			areaName = "Ancient Pyramid";
			Add(new ItemKey(16, 14, 312, 192), "Ancient Pyramid: Why not it's right there", ItemProvider.Get(EItemType.MaxSand), LeftPyramid);
			Add(new ItemKey(16, 3, 88, 192), "Ancient Pyramid: Conviction guarded room", ItemProvider.Get(EItemType.MaxHP), LeftPyramid);
			Add(new ItemKey(16, 22, 200, 192), "Ancient Pyramid: Pit secret room", ItemProvider.Get(EItemType.MaxAura), LeftPyramid & OculusRift);
			Add(new ItemKey(16, 16, 1512, 144), "Ancient Pyramid: Regret chest", ItemProvider.Get(EInventoryRelicType.EssenceOfSpace), LeftPyramid & OculusRift);
			Add(new ItemKey(16, 5, 136, 192), "Ancient Pyramid: Nightmare Door chest", ItemProvider.Get(EInventoryEquipmentType.SelenBangle), Nightmare);
		}

		void AddGyreItemLocations()
		{
			areaName = "Temporal Gyre";
			// Wheel is not strictly required, but is in logic for anti-frustration against Nethershades
			Add(new ItemKey(14, 14, 200, 832), "Temporal Gyre: Chest 1", null, TemporalGyre);
			Add(new ItemKey(14, 17, 200, 832), "Temporal Gyre: Chest 2", null, TemporalGyre);
			Add(new ItemKey(14, 20, 200, 832), "Temporal Gyre: Chest 3", null, TemporalGyre);
			Add(new ItemKey(14, 8, 120, 176), "Ravenlord: Pre fight", null, RavenlordsLair);
			Add(new ItemKey(14, 9, 200, 125), "Ravenlord: Post fight (pedestal)", null, RavenlordsLair);
			Add(new ItemKey(14, 9, 280, 176), "Ravenlord: Post fight (chest)", null, RavenlordsLair);
			// Ifrit is a strong early boss, access to the past is required as a safety check so that they do not block past access
			Add(new ItemKey(14, 6, 40, 208), "Ifrit: Pre fight", null, IfritsLair);
			Add(new ItemKey(14, 7, 200, 205), "Ifrit: Post fight (pedestal)", null, IfritsLair);
			Add(new ItemKey(14, 7, 280, 208), "Ifrit: Post fight (chest)", null, IfritsLair);
		}

		void AddDownloadTerminals()
		{
			areaName = "Library";
			Add(new ItemKey(2, 44, 792, 592), "Library: Terminal 1 (Windaria)", null, LeftLibrary & R.Tablet);
			Add(new ItemKey(2, 44, 120, 368), "Library: Terminal 2 (Lachiem)", null, LeftLibrary & R.Tablet);
			Add(new ItemKey(2, 44, 456, 368), "Library: Terminal 3 (Emporer Nuvius)", null, LeftLibrary & R.Tablet);
			Add(new ItemKey(2, 58, 152, 208), "Library: V terminal 1 (War of the Sisters)", null, LeftLibrary & R.Tablet & R.CardV);
			Add(new ItemKey(2, 58, 232, 208), "Library: V terminal 2 (Lake Desolation Map)", null, LeftLibrary & R.Tablet & R.CardV);
			Add(new ItemKey(2, 58, 312, 208), "Library: V terminal 3 (Vilete)", null, LeftLibrary & R.Tablet & R.CardV);
			areaName = "Library top";
			Add(new ItemKey(2, 44, 568, 176), "Library: Backer room terminal (Vandagray Metropolis Map)", null, UpperLeftLibrary & R.Tablet);
			areaName = "Varndagroth Tower right";
			Add(new ItemKey(2, 18, 200, 192), "Varndagroth Towers (Right): Medbay terminal (Bleakness Research)", null, RightSideLibraryElevator & R.CardB & R.Tablet);
			areaName = "The lab";
			Add(new ItemKey(11, 6, 200, 192), "Lab: Download and chest room terminal (Experiment #13)", null, UpperLab & R.Tablet);
			Add(new ItemKey(11, 15, 152, 176), "Lab: Middle terminal (Amadeus Laboratory Map)", null, TheLabPoweredOff & R.Tablet);
			Add(new ItemKey(11, 16, 600, 192), "Lab: Sentry platform terminal (Origins)", null, TheLabPoweredOff & R.Tablet);
			Add(new ItemKey(11, 34, 200, 192), "Lab: Experiment 13 terminal (W.R.E.C Farewell)", null, TheLab & R.Tablet);
			Add(new ItemKey(11, 37, 200, 192), "Lab: Left terminal (Biotechnology)", null, TheLab & R.Tablet);
			Add(new ItemKey(11, 38, 120, 176), "Lab: Right terminal (Experiment #11)", null, TheLabPoweredOff & R.Tablet);
		}

		void AddLoreLocations()
		{
			// Memories
			areaName = "LakeDesolation";
			Add(new ItemKey(1, 10, 312, 81), "Lake Desolation: Memory - Coyote Jump (Time Messenger)", null, LowerLakeDesolationBridge);
			areaName = "Library";
			Add(new ItemKey(2, 5, 200, 145), "Library: Memory - Waterway (A Message)", null, LeftLibrary);
			Add(new ItemKey(2, 45, 344, 145), "Library: Memory - Library Gap (Lachiemi Sun)", null, UpperLeftLibrary);
			Add(new ItemKey(2, 51, 88, 177), "Library: Memory - Mr. Hat Portrait (Moonlit Night)", null, UpperLeftLibrary);
			areaName = "Varndagroth Tower Left";
			Add(new ItemKey(2, 25, 216, 145), "Varndagroth Towers (Left): Memory - Elevator (Nomads)", null, MidLibrary & R.CardE);
			areaName = "Varndagroth Tower Right";
			Add(new ItemKey(2, 46, 200, 145), "Varndagroth Towers: Memory - Siren Elevator (Childhood)", null, MidLibrary & R.CardB);
			Add(new ItemKey(2, 11, 200, 161), "Varndagroth Towers (Right): Memory - Bottom (Faron)", null, LowerRightSideLibrary);
			areaName = "Military Hangar";
			Add(new ItemKey(10, 3, 536, 97), "Military Fortress: Memory - Bomber Climb (A Solution)", null, MilitaryFortress & DoubleJumpOfNpc & R.TimespinnerWheel);
			areaName = "The Lab";
			Add(new ItemKey(11, 7, 248, 129), "Lab: Memory - Genza's Secret Stash 1 (An Old Friend)", null, TheLab & OculusRift);
			Add(new ItemKey(11, 7, 296, 129), "Lab: Memory - Genza's Secret Stash 2 (Twilight Dinner)", null, TheLab & OculusRift);
			areaName = "Emperor's Tower";
			Add(new ItemKey(12, 19, 56, 145), "Emperor's Tower: Memory - Way Up There (Final Circle)", null, EmperorsTower & DoubleJumpOfNpc);
			// Letters
			areaName = "Forest";
			Add(new ItemKey(3, 12, 472, 161), "Forest: Journal - Rats (Lachiem Expedition)", null, AccessToPast);
			Add(new ItemKey(3, 15, 328, 97), "Forest: Journal - Bat Jump Ledge (Peace Treaty)", null, AccessToPast & (DoubleJumpOfNpc | ForwardDashDoubleJump | (R.TimeStop & R.ForwardDash)));
			areaName = "Castle Ramparts";
			Add(new ItemKey(4, 18, 456, 497), "Castle Ramparts: Journal - Floating in Moat (Prime Edicts)", null, CastleRamparts);
			Add(new ItemKey(4, 11, 360, 161), "Castle Ramparts: Journal - Archer + Knight (Declaration of Independence)", null, CastleRamparts);
			areaName = "Castle Keep";
			Add(new ItemKey(5, 41, 184, 177), "Castle Keep: Journal - Under the Twins (Letter of Reference)", null, CastleKeep);
			Add(new ItemKey(5, 44, 264, 161), "Castle Keep: Journal - Castle Loop Giantess (Political Advice)", null, CastleKeep);
			Add(new ItemKey(5, 14, 568, 177), "Royal Towers: Journal - Aelana\'s Room (Diplomatic Missive)", null, CastleKeep & R.PinkOrb & R.DoubleJump);
			areaName = "Royal Towers";
			Add(new ItemKey(6, 17, 344, 433), "Royal Towers: Journal - Top Struggle Juggle Base (War of the Sisters)", null, UpperRoyalTower);
			Add(new ItemKey(6, 14, 136, 177), "Royal Towers: Journal - Aelana Boss (Stained Letter)", null, UpperRoyalTower);
			Add(new ItemKey(6, 25, 152, 145), "Royal Towers: Journal - Near Bottom Struggle Juggle (Mission Findings)", null, UpperRoyalTower & DoubleJumpOfNpc);
			areaName = "Caves of Banishment (Maw)";
			Add(new ItemKey(8, 36, 136, 145), "Caves of Banishment (Maw): Journal - Lower Left Caves (Naivety)", null, LowerCavesOfBanishment);
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
			if ((!SeedOptions.GasMaw && !IsGasMaskReachableWithTheMawRequirements())
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

		bool IsGasMaskReachableWithTheMawRequirements()
		{
			//Gasmask may never be placed in a Gas effected place
			//the very basics to reach maw should also allow you to get Gasmask
			//unless we run inverted, then we can garantee the user has the pyramid keys before entering lake desolation
			var GasmaskLocation = this.First(l => l.ItemInfo?.Identifier == new ItemIdentifier(EInventoryRelicType.AirMask));

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

			return !levelIdsToAvoid.Contains(GasmaskLocation.Key.LevelId) && GasmaskLocation.Gate.CanBeOpenedWith(mawRequirements);
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
