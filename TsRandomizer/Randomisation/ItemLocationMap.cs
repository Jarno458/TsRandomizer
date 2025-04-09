using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.ReplacementObjects;
using TsRandomizer.Screens;
using R = TsRandomizer.Randomisation.Requirement;

namespace TsRandomizer.Randomisation
{
	class ItemLocationMap : LookupDictionary<ItemKey, ItemLocation>
	{
		static R NeedSwimming(bool floodArea) =>
			floodArea ? R.Swimming : R.Free;

		internal R OculusRift;
		internal R MawGasMask;
		internal R LanternCube;

		internal Gate MultipleSmallJumpsOfNpc;
		internal Gate DoubleJumpOfNpc;
		internal Gate ForwardDashDoubleJump;
		internal Gate LakeDesolationRight;

		//past
		internal Gate RefugeeCamp;
		internal Gate LeftSideForestCaves;
		internal Gate UpperLakeSirine;
		internal Gate LowerLakeSirine;
		internal Gate CavesOfBanishment;
		internal Gate CavesOfBanishmentFlooded;
		internal Gate UpperCavesOfBanishment;
		internal Gate CastleRamparts;
		internal Gate CastleKeep;
		internal Gate CastleBasement;
		internal Gate RoyalTower;
		internal Gate MidRoyalTower;
		internal Gate UpperRoyalTower;
		internal Gate KillMaw;
		//future
		internal Gate LakeDesolationLeft;
		internal Gate UpperLakeDesolation;
		internal Gate LeftLibrary;
		internal Gate UpperLeftLibrary;
		internal Gate IfritsLair;
		internal Gate MidLibrary;
		internal Gate UpperRightSideLibrary;
		internal Gate RightSideLibraryElevator;
		internal Gate LowerRightSideLibrary;
		internal Gate SealedCavesSkeleton;
		internal Gate SealedCaves;
		internal Gate SealedCavesSirens;
		internal Gate MilitaryFortress;
		internal Gate RavenlordsLair;
		internal Gate MilitaryFortressHangar;
		internal Gate LabEntrance;
		internal Gate MainLab;
		internal Gate MainLabFlooded;
		internal Gate LabResearchWing;
		internal Gate UpperLab;
		internal Gate EmperorsTowerCourtyard;
		internal Gate EmperorsTower;
		//pyramid
		internal Gate TemporalGyre;
		internal Gate OldGyreEntrance;
		internal Gate PyramidEntrance;
		internal Gate MidPyramid;
		internal Gate RightPyramid;
		internal Gate Nightmare;

		public new ItemLocation this[ItemKey key] => GetItemLocationBasedOnKeyOrRoomKey(key);

		protected readonly ItemInfoProvider ItemProvider;
		protected readonly ItemUnlockingMap UnlockingMap;
		protected readonly SeedOptions SeedOptions;
		protected readonly RisingTides FloodsFlags;
		protected readonly Era StartingEra;

		string areaName;

		public ItemLocationMap(ItemInfoProvider itemInfoProvider, ItemUnlockingMap itemUnlockingMap, Seed seed)
			: base(CalculateCapacity(seed.Options), l => l.Key)
		{
			ItemProvider = itemInfoProvider;
			UnlockingMap = itemUnlockingMap;
			SeedOptions = seed.Options;
			FloodsFlags = seed.FloodFlags;
			StartingEra = seed.StartingEra;

			SetupGates();

			AddPresentItemLocations();
			AddPastItemLocations();
			AddPyramidItemLocations();

			if (SeedOptions.GyreArchives)
				AddGyreItemLocations();

			if (SeedOptions.DownloadableItems)
				AddDownloadTerminals();

			if (SeedOptions.Cantoran)
				AddCantoran();

			if (SeedOptions.LoreChecks)
				AddLoreLocations();

			if (SeedOptions.PureTorcher)
				AddLanternLocations();

			if (SeedOptions.PyramidStart)
				AddPyramidStartLocations();

			if (SeedOptions.StartWithTalaria)
				Add(new ExternalItemLocation(itemInfoProvider.Get(EInventoryRelicType.Dash)));

			if (SeedOptions.UnchainedKeys)
				Add(new ExternalItemLocation(itemInfoProvider.Get(EInventoryRelicType.PyramidsKey)));
		}

		void SetupGates()
		{
			MultipleSmallJumpsOfNpc = R.TimespinnerWheel | R.UpwardDash;
			DoubleJumpOfNpc = (R.DoubleJump & R.TimespinnerWheel) | R.UpwardDash;
			ForwardDashDoubleJump = (R.ForwardDash & R.DoubleJump) | R.UpwardDash;

			OculusRift = (SeedOptions.EyeSpy)
				? R.OculusRift
				: R.Free;

			LanternCube = (SeedOptions.FindTheFlame)
				? R.LanternCube
				: R.Free;

			MawGasMask = (SeedOptions.GasMaw)
				? R.GasMask
				: R.Free;

			var pastRoutesToRefugeeCamp =
				(StartingEra == Era.Past)
					? R.Free
					: R.GateRefugeeCamp
					| R.GateLakeSereneLeft
					| R.GateAccessToPast
					| R.GateLakeSereneRight
					| R.GateRoyalTowers
					| R.GateCastleRamparts
					| R.GateCastleKeep
					| ((R.GateCavesOfBanishment | R.GateMaw) & (MawGasMask | R.ForwardDash) & NeedSwimming(FloodsFlags.Maw))  //through shaft
					| ((R.GateCavesOfBanishment | (R.GateMaw & R.DoubleJump)) & NeedSwimming(!FloodsFlags.DryLakeSerene)); // though left entrance;

			var labToMilitaryFortress =
				R.GateLabEntrance & (FloodsFlags.Lab ? R.Swimming : DoubleJumpOfNpc)
				| R.GateDadsTower & (FloodsFlags.Lab ? R.Swimming : DoubleJumpOfNpc) & (SeedOptions.LockKeyAmadeus ? R.LabGenza : R.Free);

			var refugeeCampToMaw =
				(
					(
						(FloodsFlags.LakeSereneBridge ? R.Free : R.TimeStop | R.ForwardDash)
						| R.GateLakeSereneLeft
						| R.GateLakeSereneRight
					) //LeftSideForestCaves
					& NeedSwimming(!FloodsFlags.DryLakeSerene) //LowerLakeSirine
					& (FloodsFlags.DryLakeSerene ? R.DoubleJump : R.Free) //CavesOfBanishment
				)
				| R.GateCavesOfBanishment 
				| R.GateMaw
				& NeedSwimming(FloodsFlags.Maw)
				& MawGasMask;
				
			var militaryFortressToLakeDesolation =
				labToMilitaryFortress
				& (
					SeedOptions.PrismBreak 
						? R.LaserA & R.LaserI & R.LaserM
						: (StartingEra == Era.Past ? R.Free : pastRoutesToRefugeeCamp)
						  & R.TimeStop // Refugee camp -> kill Twins
						  & (MultipleSmallJumpsOfNpc | ForwardDashDoubleJump) & R.DoubleJump // Refugee camp -> kill Aelana 
						  & refugeeCampToMaw // Refugee camp -> kill Maw
				) // militaryLazerGate
				& (R.CardE | R.CardB);

			LakeDesolationLeft = (StartingEra == Era.Present)
				? R.Free
				: R.GateLakeDesolation
				  | R.GateKittyBoss
				  | R.GateLeftLibrary
				  | R.GateSealedCaves
				  | R.GateXarion
				  | (R.GateSealedSirensCave & R.CardE)
				  | (R.GateMilitaryGate & (R.CardE | R.CardB))
				  | militaryFortressToLakeDesolation;

			LakeDesolationRight =
				(LakeDesolationLeft & (FloodsFlags.LakeDesolation ? R.Free : R.TimeStop | R.ForwardDash))
				| R.GateKittyBoss
				| R.GateLeftLibrary
				| (R.GateSealedSirensCave & R.CardE)
				| (R.GateMilitaryGate & (R.CardE | R.CardB))
				| militaryFortressToLakeDesolation;

			if (StartingEra != Era.Present && SeedOptions.BackToTheFuture) {
				LakeDesolationLeft |= R.TimespinnerWheel & R.TimespinnerSpindle;
				LakeDesolationRight |= R.TimespinnerWheel & R.TimespinnerSpindle;
			}

			RefugeeCamp = (StartingEra == Era.Past)
				? R.Free
				: (
					R.TimespinnerWheel & R.TimespinnerSpindle
					& (
						(LakeDesolationRight & R.CardD)
						| (R.GateSealedSirensCave & R.CardE)
						| (R.GateMilitaryGate & (R.CardB | R.CardE))
                     )
				  ) //libraryTimespinner
				  | pastRoutesToRefugeeCamp;

			//past
			LeftSideForestCaves =
				(RefugeeCamp & (FloodsFlags.LakeSereneBridge ? R.Free : (R.TimeStop | R.ForwardDash)))
				| R.GateLakeSereneRight
				| R.GateLakeSereneLeft
				| R.GateCavesOfBanishment & NeedSwimming(!FloodsFlags.DryLakeSerene)
				| R.GateMaw & R.DoubleJump & NeedSwimming(!FloodsFlags.DryLakeSerene);
			UpperLakeSirine = (LeftSideForestCaves & (FloodsFlags.DryLakeSerene ? R.Free : (R.TimeStop | R.Swimming))) | R.GateLakeSereneLeft;
			LowerLakeSirine = LeftSideForestCaves & NeedSwimming(!FloodsFlags.DryLakeSerene);
			CavesOfBanishment = (LowerLakeSirine & (FloodsFlags.DryLakeSerene ? R.DoubleJump : R.Free)) | R.GateCavesOfBanishment | (R.GateMaw & R.DoubleJump);
			CavesOfBanishmentFlooded = CavesOfBanishment & NeedSwimming(FloodsFlags.Maw);
			UpperCavesOfBanishment = RefugeeCamp;
			CastleRamparts = (SeedOptions.GateKeep) ? RefugeeCamp & (R.DrawbridgeKey | R.UpwardDash | R.GateCastleKeep) : RefugeeCamp;
			CastleKeep = CastleRamparts;
			CastleBasement = CastleKeep & NeedSwimming(FloodsFlags.Basement);
			RoyalTower = (CastleKeep & R.DoubleJump & (SeedOptions.RoyalRoadblock ? R.PinkOrb : R.Free)) | R.GateRoyalTowers;
			MidRoyalTower = RoyalTower & (MultipleSmallJumpsOfNpc | ForwardDashDoubleJump);
			UpperRoyalTower = MidRoyalTower & R.DoubleJump;
			KillMaw = CavesOfBanishmentFlooded & MawGasMask;
			var killTwins = CastleKeep & R.TimeStop;
			var killAelana = UpperRoyalTower;
			var militaryLazerGate = (SeedOptions.PrismBreak)
				? R.LaserA & R.LaserI & R.LaserM
				: killTwins & killAelana & KillMaw;
			
			//future
			UpperLakeDesolation = LakeDesolationLeft & UpperLakeSirine & R.Fire;
			LeftLibrary = UpperLakeDesolation | LakeDesolationRight;
			MidLibrary = (LeftLibrary & R.CardD) | (R.GateSealedSirensCave & R.CardE) | (R.GateMilitaryGate & (R.CardB | R.CardE));
			UpperLeftLibrary = LeftLibrary & (R.DoubleJump | R.ForwardDash);
			IfritsLair = UpperLeftLibrary & R.Kobo & RefugeeCamp;
			UpperRightSideLibrary = (MidLibrary & (R.CardC | (R.CardB & R.CardE))) | ((R.GateMilitaryGate | R.GateSealedSirensCave) & R.CardE);
			RightSideLibraryElevator = R.CardE & ((MidLibrary & (R.CardC | R.CardB)) | R.GateMilitaryGate | R.GateSealedSirensCave);
			LowerRightSideLibrary = (MidLibrary & R.CardB) | RightSideLibraryElevator | R.GateMilitaryGate | (R.GateSealedSirensCave & R.CardE);
			SealedCavesSkeleton = (LakeDesolationLeft & (FloodsFlags.LakeDesolation ? R.Free : R.DoubleJump)) | R.GateSealedCaves | R.GateXarion;
			SealedCaves = (SealedCavesSkeleton & R.CardA) | R.GateXarion;
			SealedCavesSirens = (MidLibrary & R.CardB & R.CardE) | R.GateSealedSirensCave;
			MilitaryFortress = (LowerRightSideLibrary & militaryLazerGate) | labToMilitaryFortress;
			MilitaryFortressHangar = MilitaryFortress & R.TimeStop | labToMilitaryFortress;
			LabEntrance = R.GateLabEntrance | MilitaryFortressHangar & (FloodsFlags.Lab ? R.Swimming : R.DoubleJump);
			MainLabFlooded = LabEntrance & R.CardB & NeedSwimming(FloodsFlags.Lab);
			MainLab = MainLabFlooded;
			LabResearchWing = MainLabFlooded & (SeedOptions.LockKeyAmadeus ? R.LabResearch : DoubleJumpOfNpc);
			UpperLab = R.GateDadsTower | MainLab & ForwardDashDoubleJump & (SeedOptions.LockKeyAmadeus ? R.LabGenza : R.Free);
			RavenlordsLair = UpperLab & R.MerchantCrow;
			EmperorsTowerCourtyard = UpperLab;
			EmperorsTower = EmperorsTowerCourtyard & R.DoubleJump;

			//pyramid
			var completeTimespinner = R.TimespinnerPiece1 & R.TimespinnerPiece2 & R.TimespinnerPiece3 & R.TimespinnerSpindle & R.TimespinnerWheel;
			TemporalGyre = MilitaryFortress & militaryLazerGate & R.TimespinnerWheel;
			PyramidEntrance = (SeedOptions.PyramidStart)
				? R.Free
				: (UpperLab & completeTimespinner)
				| R.GateGyre 
				| R.GateLeftPyramid 
				| (R.GateRightPyramid & R.DoubleJump);
			OldGyreEntrance = (PyramidEntrance & R.UpwardDash) | R.GateGyre;
			MidPyramid = PyramidEntrance & R.DoubleJump;
			RightPyramid =
				(MidPyramid & (FloodsFlags.PyramidShaft ? R.Free : R.UpwardDash))
				| R.GateRightPyramid;
			Nightmare = RightPyramid & completeTimespinner & NeedSwimming(FloodsFlags.BackPyramid);
		}

		static int CalculateCapacity(SeedOptions options)
		{
			var capacity = 166;

			if (options.StartWithTalaria)
				capacity += 1;
			if (options.UnchainedKeys)
				capacity += 1;
			if (options.DownloadableItems)
				capacity += 14;
			if (options.GyreArchives)
				capacity += 9;
			if (options.Cantoran)
				capacity += 1;
			if (options.LoreChecks)
				capacity += 22;
			if (options.PureTorcher)
				capacity += 527;
			if (options.PyramidStart)
				capacity += 3;

			return capacity;
		}

		void AddPresentItemLocations()
		{
			areaName = "Tutorial";
			Add(ItemKey.TutorialMeleeOrb, "Tutorial: Yo Momma 1", ItemProvider.Get(EInventoryOrbType.Blue, EOrbSlot.Melee));
			Add(ItemKey.TutorialSpellOrb, "Tutorial: Yo Momma 2", ItemProvider.Get(EInventoryOrbType.Blue, EOrbSlot.Spell));
			areaName = "Lake Desolation";
			Add(new ItemKey(1, 1, 1528, 144), "Lake Desolation: Starter chest 2", ItemProvider.Get(EInventoryUseItemType.FuturePotion), LakeDesolationLeft);
			Add(new ItemKey(1, 15, 264, 144), "Lake Desolation: Starter chest 3", ItemProvider.Get(EInventoryEquipmentType.OldCoat), LakeDesolationLeft);
			Add(new ItemKey(1, 25, 296, 176), "Lake Desolation: Starter chest 1", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), LakeDesolationLeft);
			Add(new ItemKey(1, 9, 600, 144 + TimespinnerWheel.YOffset), "Lake Desolation (Lower): Timespinner Wheel room", ItemProvider.Get(EInventoryRelicType.TimespinnerWheel), LakeDesolationLeft);
			Add(new ItemKey(1, 14, 40, 176), "Lake Desolation: Forget me not chest", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation);
			areaName = "Lower Lake Desolation";
			Add(new ItemKey(1, 2, 1016, 384), "Lake Desolation (Lower): Chicken chest", ItemProvider.Get(EItemType.MaxSand), LakeDesolationLeft & R.TimeStop);
			Add(new ItemKey(1, 11, 72, 240), "Lake Desolation (Lower): Not so secret room", ItemProvider.Get(EItemType.MaxHP), LakeDesolationRight & OculusRift);
			Add(new ItemKey(1, 3, 56, 176), "Lake Desolation (Upper): Tank chest", ItemProvider.Get(EItemType.MaxAura), LakeDesolationLeft & R.TimeStop);
			areaName = "Upper Lake Desolation";
			Add(new ItemKey(1, 17, 152, 96), "Lake Desolation (Upper): Oxygen recovery room", ItemProvider.Get(EInventoryUseItemType.GoldRing), UpperLakeDesolation);
			Add(new ItemKey(1, 21, 200, 144), "Lake Desolation (Upper): Secret room", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLakeDesolation & OculusRift);
			Add(new ItemKey(1, 20, 232, 96), "Lake Desolation (Upper): Double jump cave platform", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLakeDesolation & R.DoubleJump);
			Add(new ItemKey(1, 20, 168, 240), "Lake Desolation (Upper): Double jump cave floor", ItemProvider.Get(EInventoryUseItemType.FuturePotion), UpperLakeDesolation);
			Add(new ItemKey(1, 22, 344, 160), "Lake Desolation (Upper): Sparrow chest", ItemProvider.Get(EInventoryUseItemType.FutureHiPotion), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1320, 189), "Lake Desolation (Upper): Crash site pedestal", ItemProvider.Get(EInventoryOrbType.Moon, EOrbSlot.Melee), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1272, 192), "Lake Desolation (Upper): Crash site chest 1", ItemProvider.Get(EInventoryEquipmentType.CaptainsCap), UpperLakeDesolation & R.GasMask & KillMaw);
			Add(new ItemKey(1, 18, 1368, 192), "Lake Desolation (Upper): Crash site chest 2", ItemProvider.Get(EInventoryEquipmentType.CaptainsJacket), UpperLakeDesolation & R.GasMask & KillMaw);
			Add(new RoomItemKey(1, 5), "Lake Desolation: Kitty Boss", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Melee), UpperLakeDesolation | LakeDesolationRight);
			areaName = "Library";
			Add(new ItemKey(2, 60, 328, 160), "Library: Basement", ItemProvider.Get(EItemType.MaxHP), LeftLibrary);
			Add(new ItemKey(2, 54, 296, 176), "Library: Warp gate", ItemProvider.Get(EInventoryRelicType.ScienceKeycardD), LeftLibrary);
			Add(new ItemKey(2, 41, 404, 246), "Library: Librarian", ItemProvider.Get(EInventoryRelicType.Tablet), LeftLibrary);
			Add(new ItemKey(2, 44, 680, 368), "Library: Reading nook chest", ItemProvider.Get(EInventoryRelicType.FoeScanner), LeftLibrary);
			Add(new ItemKey(2, 47, 216, 208), "Library: Storage Room chest 1", ItemProvider.Get(EInventoryUseItemType.Ether), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 152, 208), "Library: Storage Room chest 2", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Passive), LeftLibrary & R.CardD);
			Add(new ItemKey(2, 47, 88, 208), "Library: Storage Room chest 3", ItemProvider.Get(EInventoryOrbType.Blade, EOrbSlot.Spell), LeftLibrary & R.CardD);
			areaName = "Library Top";
			Add(new ItemKey(2, 56, 168, 192), "Library: Backer Room chest 5", ItemProvider.Get(EInventoryUseItemType.GoldNecklace), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 392, 192), "Library: Backer Room chest 4", ItemProvider.Get(EInventoryUseItemType.GoldRing), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 616, 192), "Library: Backer Room chest 3", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 840, 192), "Library: Backer Room chest 2", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 1064, 192), "Library: Backer Room chest 1", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLeftLibrary);
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
			Add(new ItemKey(9, 10, 248, 848), "Sealed Caves (Xarion): Skeleton", ItemProvider.Get(EInventoryRelicType.ScienceKeycardB), SealedCavesSkeleton);
			Add(new ItemKey(9, 19, 664, 704), "Sealed Caves (Xarion): Shroom Jump room", ItemProvider.Get(EInventoryUseItemType.Antidote), SealedCaves & R.TimeStop);
			Add(new ItemKey(9, 39, 88, 192), "Sealed Caves (Xarion): Double shroom room", ItemProvider.Get(EInventoryUseItemType.Antidote), SealedCaves);
			Add(new ItemKey(9, 41, 312, 192), "Sealed Caves (Xarion): Mini jackpot room", ItemProvider.Get(EInventoryUseItemType.GalaxyStone), SealedCaves & ForwardDashDoubleJump);
			Add(new ItemKey(9, 42, 328, 192), "Sealed Caves (Xarion): Below mini jackpot room", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), SealedCaves);
			Add(new ItemKey(9, 12, 280, 160), "Sealed Caves (Xarion): Secret room", ItemProvider.Get(EItemType.MaxHP), SealedCaves & OculusRift);
			Add(new ItemKey(9, 48, 104, 160), "Sealed Caves (Xarion): Bottom left room", ItemProvider.Get(EInventoryUseItemType.FutureEther), SealedCaves);
			Add(new ItemKey(9, 15, 248, 192), "Sealed Caves (Xarion): Last chance before Xarion", ItemProvider.Get(EInventoryUseItemType.FutureEther), SealedCaves & R.DoubleJump);
			Add(new RoomItemKey(9, 13), "Sealed Caves (Xarion): Xarion", ItemProvider.Get(EInventoryRelicType.TimespinnerGear3), SealedCaves & (FloodsFlags.Xarion ? R.Swimming : R.Free));
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
			Add(new ItemKey(10, 7, 104, 192), "Military Fortress: B door chest 2", ItemProvider.Get(EInventoryUseItemType.PlasmaIV), LabEntrance & R.CardB & NeedSwimming(FloodsFlags.Lab));
			Add(new ItemKey(10, 7, 152, 192), "Military Fortress: B door chest 1", ItemProvider.Get(EItemType.MaxSand), LabEntrance & R.CardB & NeedSwimming(FloodsFlags.Lab));
			Add(new ItemKey(10, 18, 280, 189), "Military Fortress: Pedestal", ItemProvider.Get(EInventoryOrbType.Gun, EOrbSlot.Melee), LabEntrance & (FloodsFlags.Lab ? R.Free : DoubleJumpOfNpc | ForwardDashDoubleJump));
			areaName = "The Lab";
			Add(new ItemKey(11, 36, 312, 192), "Lab: Coffee break", ItemProvider.Get(EInventoryUseItemType.FoodSynth), MainLab);
			Add(new ItemKey(11, 3, 1528, 192), "Lab: Lower trash right", ItemProvider.Get(EItemType.MaxHP), MainLab & (FloodsFlags.Lab ? R.Free : R.DoubleJump));
			Add(new ItemKey(11, 3, 72, 192), "Lab: Lower trash left", ItemProvider.Get(EInventoryUseItemType.FuturePotion), MainLab & (FloodsFlags.Lab ? R.Free : (SeedOptions.LockKeyAmadeus ? DoubleJumpOfNpc: R.UpwardDash))); // When lab power is on, it only requires DoubleJumpOfNpc, but we cant code for the power state
			Add(new ItemKey(11, 25, 104, 192), "Lab: Below lab entrance", ItemProvider.Get(EItemType.MaxAura), MainLab & (FloodsFlags.Lab ? R.Swimming : R.DoubleJump));
			Add(new ItemKey(11, 18, 824, 128), "Lab: Trash jump room", ItemProvider.Get(EInventoryUseItemType.ChaosHeal), MainLab & (SeedOptions.LockKeyAmadeus ? DoubleJumpOfNpc : R.UpwardDash)); // Only requires DoubleJumpOffNpc when lab power is on
			Add(new RoomItemKey(11, 39), "Lab: Dynamo Works", ItemProvider.Get(EInventoryOrbType.Eye, EOrbSlot.Melee), SeedOptions.LockKeyAmadeus ? LabResearchWing & R.LabDynamo & R.UpwardDash : LabResearchWing); // Blast door is closed in Lock Key Amadeus
			Add(new RoomItemKey(11, 21), "Lab: Genza (Blob Mom)", ItemProvider.Get(EInventoryRelicType.ScienceKeycardA), UpperLab);
			Add(new RoomItemKey(11, 1), "Lab: Experiment #13", ItemProvider.Get(EInventoryRelicType.Dash), SeedOptions.LockKeyAmadeus ? MainLab & R.LabExperiment : LabResearchWing);
			Add(new ItemKey(11, 6, 328, 192), "Lab: Download and chest room chest", ItemProvider.Get(EInventoryEquipmentType.LabCoat), UpperLab);
			Add(new ItemKey(11, 27, 296, 160), "Lab: Lab secret", ItemProvider.Get(EItemType.MaxSand), UpperLab & OculusRift);
			Add(new RoomItemKey(11, 26), "Lab: Spider Hell", ItemProvider.Get(EInventoryRelicType.TimespinnerGear1), LabResearchWing & R.CardA);
			areaName = "Emperor's Tower";
			Add(new ItemKey(12, 5, 344, 192), "Emperor's Tower: Courtyard bottom chest", ItemProvider.Get(EItemType.MaxAura), EmperorsTowerCourtyard);
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
			Add(new RoomItemKey(3, 0), "Refugee Camp: Neliste\'s Bra", ItemProvider.Get(EInventoryOrbType.Flame, EOrbSlot.Melee), RefugeeCamp);
			Add(new ItemKey(3, 30, 296, 176), "Refugee Camp: Storage chest 3", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), RefugeeCamp);
			Add(new ItemKey(3, 30, 232, 176), "Refugee Camp: Storage chest 2", ItemProvider.Get(EInventoryUseItemType.GoldNecklace), RefugeeCamp);
			Add(new ItemKey(3, 30, 168, 176), "Refugee Camp: Storage chest 1", ItemProvider.Get(EInventoryRelicType.JewelryBox), RefugeeCamp);
			areaName = "Forest";
			Add(new ItemKey(3, 3, 648, 272), "Forest: Refugee camp roof", ItemProvider.Get(EInventoryUseItemType.Herb), RefugeeCamp);
			Add(new ItemKey(3, 15, 248, 112), "Forest: Bat jump ledge", ItemProvider.Get(EItemType.MaxAura), RefugeeCamp & (DoubleJumpOfNpc | ForwardDashDoubleJump | (R.TimeStop & R.ForwardDash)));
			Add(new ItemKey(3, 21, 120, 192), "Forest: Green platform secret", ItemProvider.Get(EItemType.MaxSand), RefugeeCamp & OculusRift);
			Add(new ItemKey(3, 12, 776, 560), "Forest: Rats guarded chest", ItemProvider.Get(EInventoryEquipmentType.PointyHat), RefugeeCamp);
			Add(new ItemKey(3, 11, 392, 608), "Forest: Waterfall chest 1", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), RefugeeCamp & R.Swimming);
			Add(new ItemKey(3, 5, 184, 192), "Forest: Waterfall chest 2", ItemProvider.Get(EInventoryEquipmentType.Pendulum), RefugeeCamp & R.Swimming);
			Add(new ItemKey(3, 2, 584, 368), "Forest: Batcave", ItemProvider.Get(EInventoryUseItemType.Potion), RefugeeCamp);
			Add(new ItemKey(4, 20, 264, 160), "Castle Ramparts: In the moat", ItemProvider.Get(EItemType.MaxAura), RefugeeCamp & NeedSwimming(FloodsFlags.CastleMoat));
			Add(new ItemKey(3, 29, 248, 192), "Forest: Before Serene single bat cave", ItemProvider.Get(EItemType.MaxHP), LeftSideForestCaves);
			areaName = "Upper Lake Serene";
			Add(new ItemKey(7, 16, 152, 96), "Lake Serene (Upper): Rat nest", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), UpperLakeSirine);
			Add(new ItemKey(7, 19, 248, 96), "Lake Serene (Upper): Double jump cave platform", ItemProvider.Get(EItemType.MaxAura), UpperLakeSirine & R.DoubleJump);
			Add(new ItemKey(7, 19, 168, 240), "Lake Serene (Upper): Double jump cave floor", ItemProvider.Get(EInventoryEquipmentType.TravelersCloak), UpperLakeSirine);
			Add(new ItemKey(7, 27, 184, 144), "Lake Serene (Upper): Cave secret", ItemProvider.Get(EInventoryFamiliarType.Griffin), UpperLakeSirine & OculusRift);
			Add(new RoomItemKey(7, 28), "Lake Serene: Before Big Bird", ItemProvider.Get(EInventoryUseItemType.AlchemistTools), UpperLakeSirine);
			Add(new ItemKey(7, 13, 56, 176), "Lake Serene: Behind the vines", ItemProvider.Get(EInventoryUseItemType.WarpCard), UpperLakeSirine);
			Add(new RoomItemKey(7, 30), "Lake Serene: Pyramid keys room", ItemProvider.Get(EInventoryRelicType.PyramidsKey), UpperLakeSirine);
			Add(new ItemKey(7, 3, 120, 204), "Lake Serene (Upper): Chicken ledge", null, UpperLakeSirine);
			areaName = "Lower Lake Serene";
			Add(new ItemKey(7, 3, 440, 1232), "Lake Serene (Lower): Deep dive", ItemProvider.Get(EInventoryUseItemType.Potion), LowerLakeSirine);
			Add(new ItemKey(7, 7, 1432, 576), "Lake Serene (Lower): Under the eels", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), LeftSideForestCaves & R.Swimming);
			Add(new ItemKey(7, 6, 520, 496), "Lake Serene (Lower): Water spikes room", ItemProvider.Get(EInventoryUseItemType.Potion), LowerLakeSirine);
			Add(new ItemKey(7, 11, 88, 240), "Lake Serene (Lower): Underwater secret", ItemProvider.Get(EItemType.MaxHP), LowerLakeSirine & OculusRift);
			Add(new ItemKey(7, 2, 1016, 384), "Lake Serene (Lower): T chest", ItemProvider.Get(EInventoryUseItemType.Ether), LowerLakeSirine & (FloodsFlags.DryLakeSerene ? DoubleJumpOfNpc : R.Free));
			Add(new ItemKey(7, 20, 248, 96), "Lake Serene (Lower): Past the eels", ItemProvider.Get(EItemType.MaxSand), LeftSideForestCaves & R.Swimming);
			Add(new ItemKey(7, 9, 584, 189), "Lake Serene (Lower): Underwater pedestal", ItemProvider.Get(EInventoryOrbType.Ice, EOrbSlot.Melee), LowerLakeSirine & (FloodsFlags.DryLakeSerene ? R.DoubleJump : R.Free));
			areaName = "Caves of Banishment (Maw)";
			Add(new ItemKey(8, 19, 664, 704), "Caves of Banishment (Maw): Shroom Jump room", ItemProvider.Get(EInventoryUseItemType.SilverOre), CavesOfBanishment & (FloodsFlags.Maw ? R.Free : R.DoubleJump));
			Add(new ItemKey(8, 12, 280, 160), "Caves of Banishment (Maw): Secret room", ItemProvider.Get(EItemType.MaxHP), CavesOfBanishmentFlooded & OculusRift);
			Add(new ItemKey(8, 48, 104, 160), "Caves of Banishment (Maw): Bottom left room", ItemProvider.Get(EInventoryUseItemType.Spaghetti), CavesOfBanishmentFlooded); //Default item is Herb but got replaced by Spaghetti
			Add(new ItemKey(8, 39, 88, 192), "Caves of Banishment (Maw): Single shroom room", ItemProvider.Get(EInventoryUseItemType.SilverOre), CavesOfBanishment);
			Add(new ItemKey(8, 41, 168, 192), "Caves of Banishment (Maw): Jackpot room chest 1", ItemProvider.Get(EInventoryUseItemType.GoldNecklace), CavesOfBanishment & (FloodsFlags.Maw ? R.Free : ForwardDashDoubleJump));
			Add(new ItemKey(8, 41, 216, 192), "Caves of Banishment (Maw): Jackpot room chest 2", ItemProvider.Get(EInventoryUseItemType.GoldRing), CavesOfBanishment & (FloodsFlags.Maw ? R.Free : ForwardDashDoubleJump));
			Add(new ItemKey(8, 41, 264, 192), "Caves of Banishment (Maw): Jackpot room chest 3", ItemProvider.Get(EInventoryUseItemType.EssenceCrystal), CavesOfBanishment & (FloodsFlags.Maw ? R.Free : ForwardDashDoubleJump));
			Add(new ItemKey(8, 41, 312, 192), "Caves of Banishment (Maw): Jackpot room chest 4", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), CavesOfBanishment & (FloodsFlags.Maw ? R.Free : ForwardDashDoubleJump));
			Add(new ItemKey(8, 42, 216, 189), "Caves of Banishment (Maw): Pedestal", ItemProvider.Get(EInventoryOrbType.Wind, EOrbSlot.Melee), CavesOfBanishmentFlooded);
			Add(new ItemKey(8, 15, 248, 192), "Caves of Banishment (Maw): Last chance before Maw", ItemProvider.Get(EInventoryUseItemType.SilverOre), CavesOfBanishmentFlooded & (FloodsFlags.Maw ? R.Free : R.DoubleJump));
			Add(new RoomItemKey(8, 21), "Caves of Banishment (Maw): Plasma Crystal", ItemProvider.Get(EInventoryUseItemType.RadiationCrystal), CavesOfBanishmentFlooded & (MawGasMask | R.ForwardDash));
			Add(new ItemKey(8, 31, 88, 400), "Caves of Banishment (Maw): Mineshaft", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), CavesOfBanishmentFlooded & (MawGasMask | R.ForwardDash));
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
			Add(new ItemKey(5, 9, 104, 189), "Castle Basement: Secret pedestal", ItemProvider.Get(EInventoryOrbType.Blood, EOrbSlot.Melee), CastleBasement & OculusRift);
			Add(new ItemKey(5, 10, 104, 192), "Castle Basement: Clean the castle basement", ItemProvider.Get(EInventoryFamiliarType.Sprite), CastleBasement);
			Add(new ItemKey(5, 14, 88, 208), "Castle Keep: Yas queen room", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), CastleKeep & R.PinkOrb & R.DoubleJump);
			Add(new ItemKey(5, 44, 216, 192), "Castle Basement: Giantess guarded chest", ItemProvider.Get(EInventoryUseItemType.Potion), CastleBasement);
			Add(new ItemKey(5, 45, 104, 192), "Castle Basement: Omelette chest", ItemProvider.Get(EItemType.MaxHP), CastleBasement);
			Add(new ItemKey(5, 15, 296, 192), "Castle Basement: Just an egg", ItemProvider.Get(EItemType.MaxAura), CastleBasement);
			Add(new ItemKey(5, 41, 72, 160), "Castle Keep: Under the twins", ItemProvider.Get(EInventoryEquipmentType.BuckleHat), CastleKeep);
			Add(new ItemKey(5, 20, 504, 48), "Castle Keep: Advisor jump", null, CastleKeep & R.TimeStop);
			Add(new RoomItemKey(5, 5), "Castle Keep: Twins", ItemProvider.Get(EInventoryRelicType.DoubleJump), CastleKeep & R.TimeStop);
			Add(new ItemKey(5, 22, 312, 176), "Castle Keep: Royal guard tiny room", ItemProvider.Get(EItemType.MaxSand), CastleKeep & ((R.TimeStop & R.ForwardDash) | R.DoubleJump));
			areaName = "Royal Towers";
			Add(new ItemKey(6, 19, 200, 176), "Royal Towers: Floor secret", ItemProvider.Get(EItemType.MaxAura), RoyalTower & R.DoubleJump & OculusRift);
			Add(new ItemKey(6, 27, 472, 384), "Royal Towers: Pre-climb gap", ItemProvider.Get(EInventoryUseItemType.MagicMarbles), MidRoyalTower);
			Add(new ItemKey(6, 1, 1512, 288), "Royal Towers: Long balcony", ItemProvider.Get(EInventoryUseItemType.Potion), MidRoyalTower & NeedSwimming(FloodsFlags.CastleCourtyard));
			Add(new ItemKey(6, 25, 360, 176), "Royal Towers: Past bottom struggle juggle", ItemProvider.Get(EInventoryUseItemType.HiEther), MidRoyalTower & (FloodsFlags.CastleCourtyard ? R.Free : DoubleJumpOfNpc));
			Add(new ItemKey(6, 3, 120, 208), "Royal Towers: Bottom struggle juggle", ItemProvider.Get(EInventoryFamiliarType.Demon), MidRoyalTower & DoubleJumpOfNpc);
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
			Add(new ItemKey(16, 14, 312, 192), "Ancient Pyramid: Why not it's right there", ItemProvider.Get(EItemType.MaxSand), PyramidEntrance);
			Add(new ItemKey(16, 3, 88, 192), "Ancient Pyramid: Conviction guarded room", ItemProvider.Get(EItemType.MaxHP), MidPyramid);
			Add(new ItemKey(16, 22, 200, 192), "Ancient Pyramid: Pit secret room", ItemProvider.Get(EItemType.MaxAura), MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft));
			Add(new ItemKey(16, 16, 1512, 144), "Ancient Pyramid: Regret chest", ItemProvider.Get(EInventoryRelicType.EssenceOfSpace), MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft));
			Add(new ItemKey(16, 5, 136, 192), "Ancient Pyramid: Nightmare Door chest", ItemProvider.Get(EInventoryEquipmentType.SelenBangle), RightPyramid & NeedSwimming(FloodsFlags.BackPyramid));
		}

		void AddPyramidStartLocations()
		{
			areaName = "Dark Forest";
			Add(new ItemKey(15, 2, 200, 562), "Dark Forest: Training Dummy", null, PyramidEntrance);
			areaName = "Temporal Gyre";
			Add(new ItemKey(14, 0, 240, 192), "Temporal Gyre: Forest Entrance", null, OldGyreEntrance);
			areaName = "Ancient Pyramid";
			Add(new ItemKey(16, 2, 2192, 1552), "Ancient Pyramid: Rubble", null, PyramidEntrance);
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
			Add(new ItemKey(2, 44, 456, 368), "Library: Terminal 3 (Emperor Nuvius)", null, LeftLibrary & R.Tablet);
			Add(new ItemKey(2, 58, 152, 208), "Library: V terminal 1 (War of the Sisters)", null, LeftLibrary & R.Tablet & R.CardV);
			Add(new ItemKey(2, 58, 232, 208), "Library: V terminal 2 (Lake Desolation Map)", null, LeftLibrary & R.Tablet & R.CardV);
			Add(new ItemKey(2, 58, 312, 208), "Library: V terminal 3 (Vilete)", null, LeftLibrary & R.Tablet & R.CardV);
			areaName = "Library top";
			Add(new ItemKey(2, 44, 568, 176), "Library: Backer Room terminal (Vandagray Metropolis Map)", null, UpperLeftLibrary & R.Tablet);
			areaName = "Varndagroth Tower right";
			Add(new ItemKey(2, 18, 200, 192), "Varndagroth Towers (Right): Medbay terminal (Bleakness Research)", null, RightSideLibraryElevator & R.CardB & R.Tablet);
			areaName = "The lab";
			Add(new ItemKey(11, 6, 200, 192), "Lab: Download and chest room terminal (Experiment #13)", null, UpperLab & R.Tablet);
			Add(new ItemKey(11, 15, 152, 176), "Lab: Middle terminal (Amadeus Laboratory Map)", null, LabResearchWing & R.Tablet);
			Add(new ItemKey(11, 16, 600, 192), "Lab: Sentry platform terminal (Origins)", null, (SeedOptions.LockKeyAmadeus ? UpperLab | (MainLab & R.LabGenza) : LabResearchWing) & R.Tablet);
			Add(new ItemKey(11, 34, 200, 192), "Lab: Experiment 13 terminal (W.R.E.C Farewell)", null, MainLab & R.Tablet);
			Add(new ItemKey(11, 37, 200, 192), "Lab: Left terminal (Biotechnology)", null, MainLab & R.Tablet);
			Add(new ItemKey(11, 38, 120, 176), "Lab: Right terminal (Experiment #11)", null, LabResearchWing & R.Tablet);
		}

		void AddLoreLocations()
		{
			// Memories
			areaName = "LakeDesolation";
			Add(new ItemKey(1, 10, 312, 81), "Lake Desolation: Memory - Coyote Jump (Time Messenger)", null, LakeDesolationRight);
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
			Add(new ItemKey(11, 7, 248, 129), "Lab: Memory - Genza's Secret Stash 1 (An Old Friend)", null, MainLab & OculusRift);
			Add(new ItemKey(11, 7, 296, 129), "Lab: Memory - Genza's Secret Stash 2 (Twilight Dinner)", null, MainLab & OculusRift);
			areaName = "Emperor's Tower";
			Add(new ItemKey(12, 19, 56, 145), "Emperor's Tower: Memory - Way Up There (Final Circle)", null, EmperorsTower & DoubleJumpOfNpc);
			// Letters
			areaName = "Forest";
			Add(new ItemKey(3, 12, 472, 161), "Forest: Journal - Rats (Lachiem Expedition)", null, RefugeeCamp);
			Add(new ItemKey(3, 15, 328, 97), "Forest: Journal - Bat Jump Ledge (Peace Treaty)", null, RefugeeCamp & (DoubleJumpOfNpc | ForwardDashDoubleJump | (R.TimeStop & R.ForwardDash)));
			Add(new ItemKey(4, 18, 456, 497), "Forest: Journal - Floating in Moat (Prime Edicts)", null, CastleRamparts & NeedSwimming(FloodsFlags.CastleMoat));
			areaName = "Castle Ramparts";
			Add(new ItemKey(4, 11, 360, 161), "Castle Ramparts: Journal - Archer + Knight (Declaration of Independence)", null, CastleRamparts);
			areaName = "Castle Keep";
			Add(new ItemKey(5, 41, 184, 177), "Castle Keep: Journal - Under the Twins (Letter of Reference)", null, CastleKeep);
			Add(new ItemKey(5, 44, 264, 161), "Castle Basement: Journal - Castle Loop Giantess (Political Advice)", null, CastleBasement);
			Add(new ItemKey(5, 14, 568, 177), "Royal Towers: Journal - Aelana\'s Room (Diplomatic Missive)", null, CastleKeep & R.PinkOrb & R.DoubleJump);
			areaName = "Royal Towers";
			Add(new ItemKey(6, 17, 344, 433), "Royal Towers: Journal - Top Struggle Juggle Base (War of the Sisters)", null, UpperRoyalTower);
			Add(new ItemKey(6, 14, 136, 177), "Royal Towers: Journal - Aelana Boss (Stained Letter)", null, UpperRoyalTower);
			Add(new ItemKey(6, 25, 152, 145), "Royal Towers: Journal - Near Bottom Struggle Juggle (Mission Findings)", null, MidRoyalTower & (FloodsFlags.CastleCourtyard ? R.Free : DoubleJumpOfNpc));
			areaName = "Caves of Banishment (Maw)";
			Add(new ItemKey(8, 36, 136, 145), "Caves of Banishment (Maw): Journal - Lower Left Caves (Naivety)", null, CavesOfBanishmentFlooded | (R.GateMaw & NeedSwimming(FloodsFlags.Maw)));
		}

		void AddLanternLocations()
		{
			areaName = "Lower Lake Desolation";
			Add(new ItemKey(1, 11, 106, 221), "Lake Desolation (Lower): Not so secret room Lantern", null, LakeDesolationRight & OculusRift & LanternCube);
			Add(new ItemKey(1, 6, 474, 413), "Lake Desolation (Lower): Middle Room Lantern 1", null, LakeDesolationRight & OculusRift & LanternCube);
			Add(new ItemKey(1, 6, 570, 173), "Lake Desolation (Lower): Middle Room Lantern 2", null, LakeDesolationRight & OculusRift & LanternCube);
			Add(new ItemKey(1, 9, 122, 189), "Lake Desolation (Lower): Timespinner Wheel room Lantern 1", null, LakeDesolationLeft & LanternCube);
			Add(new ItemKey(1, 9, 314, 141), "Lake Desolation (Lower): Timespinner Wheel room Lantern 2", null, LakeDesolationLeft & LanternCube);

			areaName = "Upper Lake Desolation";
			Add(new ItemKey(1, 16, 106, 461), "Lake Desolation (Upper): Upper Left Room Lantern 1", null, UpperLakeDesolation & LanternCube);
			Add(new ItemKey(1, 16, 282, 269), "Lake Desolation (Upper): Upper Left Room Lantern 2", null, UpperLakeDesolation & LanternCube);
			Add(new ItemKey(1, 17, 282, 141), "Lake Desolation (Upper): Oxygen recovery room Lantern", null, UpperLakeDesolation & LanternCube);
			Add(new ItemKey(1, 19, 218, 365), "Lake Desolation (Upper): Upper Right Room Lantern 1", null, UpperLakeDesolation & LanternCube);
			Add(new ItemKey(1, 20, 250, 237), "Lake Desolation (Upper): Double jump cave Lantern", null, UpperLakeDesolation & LanternCube);

			areaName = "Lake Desolation";
			Add(new ItemKey(1, 7, 1432, 58), "Lake Desolation: Metropolis Bridge Lantern 1", null, LakeDesolationRight & R.DoubleJump & LanternCube);
			Add(new ItemKey(1, 7, 472, 58), "Lake Desolation: Metropolis Bridge Lantern 2", null, LakeDesolationRight & R.DoubleJump & LanternCube);
			Add(new ItemKey(1, 7, 1624, 58), "Lake Desolation: Metropolis Bridge Lantern 3", null, LakeDesolationRight & R.DoubleJump & LanternCube);
			Add(new ItemKey(1, 7, 664, 58), "Lake Desolation: Metropolis Bridge Lantern 4", null, LakeDesolationRight & R.DoubleJump & LanternCube);
			Add(new ItemKey(1, 7, 856, 58), "Lake Desolation: Metropolis Bridge Lantern 5", null, LakeDesolationRight & R.DoubleJump & LanternCube);
			Add(new ItemKey(1, 7, 1054, 58), "Lake Desolation: Metropolis Bridge Lantern 6", null, LakeDesolationRight & R.DoubleJump & LanternCube);
			Add(new ItemKey(1, 7, 1240, 58), "Lake Desolation: Metropolis Bridge Lantern 7", null, LakeDesolationRight & R.DoubleJump & LanternCube);

			areaName = "Library";
			Add(new ItemKey(2, 57, 200, 100), "Library: Sewer Entrance Lantern", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 0, 1432, 116), "Library: Left Sewer Lantern 1", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 0, 504, 116), "Library: Left Sewer Lantern 2", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 1, 1208, 116), "Library: Right Sewer Lantern 1", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 1, 1320, 116), "Library: Right Sewer Lantern 2", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 1, 280, 116), "Library: Right Sewer Lantern 3", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 1, 392, 116), "Library: Right Sewer Lantern 4", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 1, 616, 116), "Library: Right Sewer Lantern 5", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 1, 984, 116), "Library: Right Sewer Lantern 6", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 59, 112, 96), "Library: Sewer Exit Lantern 1", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 59, 288, 96), "Library: Sewer Exit Lantern 2", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 60, 184, 100), "Library: Basement Lantern", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 31, 208, 96), "Library: Exit Lantern", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 10, 2536, 116), "Library: Moving Sidewalk Lantern 1", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 10, 664, 116), "Library: Moving Sidewalk Lantern 2", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 41, 176, 208), "Library: Librarian Lantern 1", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 41, 624, 208), "Library: Librarian Lantern 2", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 42, 112, 512), "Library: Left Staircase Lantern 1", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 42, 112, 96), "Library: Left Staircase Lantern 2", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 42, 288, 512), "Library: Left Staircase Lantern 3", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 42, 288, 96), "Library: Left Staircase Lantern 4", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 44, 1072, 496), "Library: Lantern 1", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 44, 112, 496), "Library: Lantern 2", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 44, 400, 496), "Library: Lantern 3", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 44, 400, 96), "Library: Lantern 4", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 44, 624, 496), "Library: Lantern 5", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 44, 736, 96), "Library: Lantern 6", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 44, 848, 496), "Library: Lantern 7", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 47, 128, 96), "Library: Storage Room Lantern 1", null, LeftLibrary & R.CardD & LanternCube);
			Add(new ItemKey(2, 47, 240, 96), "Library: Storage Room Lantern 2", null, LeftLibrary & R.CardD & LanternCube);
			Add(new ItemKey(2, 5, 120, 100), "Library: Waterway Lantern 1", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 5, 280, 100), "Library: Waterway Lantern 2", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 56, 1072, 96), "Library: Backer Room Lantern 1", null, UpperLeftLibrary & LanternCube);
			Add(new ItemKey(2, 56, 176, 96), "Library: Backer Room Lantern 2", null, UpperLeftLibrary & LanternCube);
			Add(new ItemKey(2, 56, 400, 96), "Library: Backer Room Lantern 3", null, UpperLeftLibrary & LanternCube);
			Add(new ItemKey(2, 56, 624, 96), "Library: Backer Room Lantern 4", null, UpperLeftLibrary & LanternCube);
			Add(new ItemKey(2, 56, 848, 96), "Library: Backer Room Lantern 5", null, UpperLeftLibrary & LanternCube);
			Add(new ItemKey(2, 58, 160, 96), "Library: V Room Lantern 1", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 58, 272, 96), "Library: V Room Lantern 2", null, LeftLibrary & LanternCube);
			Add(new ItemKey(2, 4, 112, 512), "Library: Backer Stairs Lantern 1", null, UpperLeftLibrary & LanternCube);
			Add(new ItemKey(2, 4, 112, 96), "Library: Backer Stairs Lantern 2", null, UpperLeftLibrary & LanternCube);
			Add(new ItemKey(2, 4, 288, 512), "Library: Backer Stairs Lantern 3", null, UpperLeftLibrary & LanternCube);
			Add(new ItemKey(2, 4, 288, 96), "Library: Backer Stairs Lantern 4", null, UpperLeftLibrary & LanternCube);
			Add(new ItemKey(2, 51, 128, 96), "Library: Mr. Hat Lantern 1", null, UpperLeftLibrary & LanternCube);
			Add(new ItemKey(2, 51, 240, 96), "Library: Mr. Hat Lantern 2", null, UpperLeftLibrary & LanternCube);

			areaName = "Varndagroth Towers (Left)";
			Add(new ItemKey(2, 16, 200, 192), "Varndagroth Towers (Left): Entrance Lantern", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 32, 120, 192), "Varndagroth Towers (Left): Bottom Floor Lantern", null, MidLibrary & R.CardC & LanternCube);
			Add(new ItemKey(2, 34, 232, 1520), "Varndagroth Towers (Left): Left Elevator Lantern 1", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 34, 232, 2512), "Varndagroth Towers (Left): Left Elevator Lantern 2", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 43, 200, 192), "Varndagroth Towers (Left): Left Stairs Base Lantern 1", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 43, 600, 192), "Varndagroth Towers (Left): Left Stairs Base Lantern 2", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 12, 168, 256), "Varndagroth Towers (Left): Left Stairs Floor 2 Lantern 1", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 12, 632, 256), "Varndagroth Towers (Left): Left Stairs Floor 2 Lantern 2", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 36, 312, 208), "Varndagroth Towers (Left): Left Stairs Middle Lantern", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 37, 152, 192), "Varndagroth Towers (Left): Left Stairs Ladder Lantern 1", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 37, 728, 192), "Varndagroth Towers (Left): Left Stairs Ladder Lantern 2", null, MidLibrary & LanternCube);
			Add(new ItemKey(2, 39, 264, 192), "Varndagroth Towers (Left): Bridge Entrance Lantern 1", null, UpperRightSideLibrary & LanternCube);
			Add(new ItemKey(2, 39, 552, 192), "Varndagroth Towers (Left): Bridge Entrance Lantern 2", null, UpperRightSideLibrary & LanternCube);
			areaName = "Varndagroth Towers (Right)";
			Add(new ItemKey(2, 19, 1048, 192), "Varndagroth Towers (Right): Bridge Exit Lantern 1", null, UpperRightSideLibrary & LanternCube);
			Add(new ItemKey(2, 19, 168, 192), "Varndagroth Towers (Right): Bridge Exit Lantern 2", null, UpperRightSideLibrary & LanternCube);
			Add(new ItemKey(2, 17, 168, 192), "Varndagroth Towers (Right): Right Stairs Lantern 1", null, LowerRightSideLibrary & LanternCube);
			Add(new ItemKey(2, 17, 648, 192), "Varndagroth Towers (Right): Right Stairs Lantern 2", null, LowerRightSideLibrary & LanternCube);
			Add(new ItemKey(2, 9, 200, 192), "Varndagroth Towers (Right): Base Lantern 1", null, LowerRightSideLibrary & LanternCube);
			Add(new ItemKey(2, 9, 600, 192), "Varndagroth Towers (Right): Base Lantern 2", null, LowerRightSideLibrary & LanternCube);
			Add(new ItemKey(2, 20, 168, 1200), "Varndagroth Towers (Right): Right Elevator Lantern 1", null, RightSideLibraryElevator & LanternCube);
			Add(new ItemKey(2, 20, 168, 1840), "Varndagroth Towers (Right): Right Elevator Lantern 2", null, RightSideLibraryElevator & LanternCube);
			Add(new ItemKey(2, 20, 168, 208), "Varndagroth Towers (Right): Right Elevator Lantern 3", null, RightSideLibraryElevator & LanternCube);
			Add(new ItemKey(2, 20, 168, 2160), "Varndagroth Towers (Right): Right Elevator Lantern 4", null, RightSideLibraryElevator & LanternCube);
			Add(new ItemKey(2, 20, 168, 2512), "Varndagroth Towers (Right): Right Elevator Lantern 5", null, RightSideLibraryElevator & LanternCube);
			Add(new ItemKey(2, 20, 168, 880), "Varndagroth Towers (Right): Right Elevator Lantern 6", null, RightSideLibraryElevator & LanternCube);
			Add(new ItemKey(2, 24, 1048, 192), "Varndagroth Towers (Right): Above Vents Lantern 1", null, UpperRightSideLibrary & (R.CardE | R.DoubleJump) & LanternCube); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 24, 88, 192), "Varndagroth Towers (Right): Above Vents Lantern 2", null, UpperRightSideLibrary & (R.CardE | R.DoubleJump) & LanternCube); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 200, 560), "Varndagroth Towers (Right): Vent Lantern 1", null, UpperRightSideLibrary & (R.CardE | R.DoubleJump) & LanternCube); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 648, 560), "Varndagroth Towers (Right): Vent Lantern 2", null, UpperRightSideLibrary & (R.CardE | R.DoubleJump) & LanternCube); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out
			Add(new ItemKey(2, 23, 984, 560), "Varndagroth Towers (Right): Vent Lantern 3", null, UpperRightSideLibrary & (R.CardE | R.DoubleJump) & LanternCube); //needs only UpperRightSideLibrary but requires Elevator Card | Double Jump to get out

			areaName = "Sealed Caves (Xarion)";
			Add(new ItemKey(9, 0, 1032, 140), "Sealed Caves (Xarion) First Hall Lantern 1", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 0, 1176, 140), "Sealed Caves (Xarion) First Hall Lantern 2", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 0, 1416, 140), "Sealed Caves (Xarion) First Hall Lantern 3", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 0, 1576, 140), "Sealed Caves (Xarion) First Hall Lantern 4", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 0, 424, 140), "Sealed Caves (Xarion) First Hall Lantern 5", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 0, 568, 140), "Sealed Caves (Xarion) First Hall Lantern 6", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 37, 1032, 124), "Sealed Caves (Xarion) Second Hall Lantern 1", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 37, 1176, 124), "Sealed Caves (Xarion) Second Hall Lantern 2", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 37, 424, 124), "Sealed Caves (Xarion) Second Hall Lantern 3", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 37, 584, 124), "Sealed Caves (Xarion) Second Hall Lantern 4", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 38, 120, 124), "Sealed Caves (Xarion) Forked Shaft Lantern 1", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 38, 120, 508), "Sealed Caves (Xarion) Forked Shaft Lantern 2", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 38, 168, 828), "Sealed Caves (Xarion) Forked Shaft Lantern 3", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 38, 248, 828), "Sealed Caves (Xarion) Forked Shaft Lantern 4", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 38, 296, 124), "Sealed Caves (Xarion) Forked Shaft Lantern 5", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 19, 1096, 1756), "Sealed Caves (Xarion): Shroom Jump Lantern 1", null, SealedCaves & R.TimeStop & LanternCube);
			Add(new ItemKey(9, 19, 520, 876), "Sealed Caves (Xarion): Shroom Jump Lantern 2", null, SealedCaves & R.TimeStop & LanternCube);
			Add(new ItemKey(9, 19, 536, 140), "Sealed Caves (Xarion): Shroom Jump Lantern 3", null, SealedCaves & R.TimeStop & LanternCube);
			Add(new ItemKey(9, 19, 696, 876), "Sealed Caves (Xarion): Shroom Jump Lantern 4", null, SealedCaves & R.TimeStop & LanternCube);
			Add(new ItemKey(9, 46, 104, 140), "Sealed Caves (Xarion): Lower Fork Start Lantern 1", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 46, 296, 140), "Sealed Caves (Xarion): Lower Fork Start Lantern 2", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 9, 104, 124), "Sealed Caves (Xarion): Lower Fork Vertical Room Lantern 1", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 9, 104, 508), "Sealed Caves (Xarion): Lower Fork Vertical Room Lantern 2", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 9, 280, 124), "Sealed Caves (Xarion): Lower Fork Vertical Room Lantern 3", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 9, 280, 508), "Sealed Caves (Xarion): Lower Fork Vertical Room Lantern 4", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 10, 120, 124), "Sealed Caves (Xarion): Skeleton Lantern 1", null, SealedCavesSkeleton & LanternCube);
			Add(new ItemKey(9, 10, 184, 828), "Sealed Caves (Xarion): Skeleton Lantern 2", null, SealedCavesSkeleton & LanternCube);
			Add(new ItemKey(9, 10, 296, 124), "Sealed Caves (Xarion): Skeleton Lantern 3", null, SealedCavesSkeleton & LanternCube);
			Add(new ItemKey(9, 16, 1096, 140), "Sealed Caves (Xarion): Mini Jackpot Ledge Lantern", null, SealedCaves & ForwardDashDoubleJump & LanternCube);
			Add(new ItemKey(9, 16, 104, 140), "Sealed Caves (Xarion): Pre-Jackpot Waterfall Room Lantern 1", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 16, 104, 1756), "Sealed Caves (Xarion): Pre-Jackpot Waterfall Room Lantern 2", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 16, 1096, 1756), "Sealed Caves (Xarion): Pre-Jackpot Waterfall Room Lantern 3", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 16, 1096, 780), "Sealed Caves (Xarion): Pre-Jackpot Waterfall Room Lantern 4", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 16, 920, 780), "Sealed Caves (Xarion): Pre-Jackpot Waterfall Room Lantern 5", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 47, 122, 157), "Sealed Caves (Xarion): Post-Fork Room Lantern 1", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 47, 266, 157), "Sealed Caves (Xarion): Post-Fork Room Lantern 2", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 43, 1050, 157), "Sealed Caves (Xarion): Rejoined Hallway Lantern 1", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 43, 1258, 157), "Sealed Caves (Xarion): Rejoined Hallway Lantern 2", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 43, 1466, 157), "Sealed Caves (Xarion): Rejoined Hallway Lantern 3", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 43, 218, 157), "Sealed Caves (Xarion): Rejoined Hallway Lantern 4", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 43, 426, 157), "Sealed Caves (Xarion): Rejoined Hallway Lantern 5", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 43, 634, 157), "Sealed Caves (Xarion): Rejoined Hallway Lantern 6", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 14, 1050, 157), "Sealed Caves (Xarion): Penultimate Hall Lantern 1", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 14, 1258, 157), "Sealed Caves (Xarion): Penultimate Hall Lantern 2", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 14, 1466, 157), "Sealed Caves (Xarion): Penultimate Hall Lantern 3", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 14, 218, 157), "Sealed Caves (Xarion): Penultimate Hall Lantern 4", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 14, 426, 157), "Sealed Caves (Xarion): Penultimate Hall Lantern 5", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 14, 634, 157), "Sealed Caves (Xarion): Penultimate Hall Lantern 6", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 8, 282, 157), "Sealed Caves (Xarion): Last Chance Room Lantern 1", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 8, 392, 348), "Sealed Caves (Xarion): Last Chance Room Lantern 2", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 8, 392, 540), "Sealed Caves (Xarion): Last Chance Room Lantern 3", null, SealedCaves & LanternCube);
			Add(new ItemKey(9, 8, 506, 157), "Sealed Caves (Xarion): Last Chance Room Lantern 4", null, SealedCaves & R.DoubleJump & LanternCube);

			areaName = "Sealed Caves (Sirens)";
			Add(new ItemKey(9, 11, 153, 140), "Sealed Caves (Sirens): Middle Hall Lantern 1", null, SealedCavesSirens & LanternCube);
			Add(new ItemKey(9, 11, 409, 140), "Sealed Caves (Sirens): Middle Hall Lantern 2", null, SealedCavesSirens & LanternCube);
			Add(new ItemKey(9, 11, 793, 140), "Sealed Caves (Sirens): Middle Hall Lantern 3", null, SealedCavesSirens & LanternCube);
			Add(new ItemKey(9, 11, 985, 140), "Sealed Caves (Sirens): Middle Hall Lantern 4", null, SealedCavesSirens & LanternCube);
			Add(new ItemKey(9, 20, 1001, 140), "Sealed Caves (Sirens): First Hall Lantern 1", null, SealedCavesSirens & LanternCube);
			Add(new ItemKey(9, 20, 297, 140), "Sealed Caves (Sirens): First Hall Lantern 2", null, SealedCavesSirens & LanternCube);
			Add(new ItemKey(9, 20, 617, 140), "Sealed Caves (Sirens): First Hall Lantern 3", null, SealedCavesSirens & LanternCube);
			Add(new ItemKey(9, 31, 329, 124), "Sealed Caves (Sirens): Condemned Shaft Lantern 1", null, SealedCavesSirens & LanternCube);
			Add(new ItemKey(9, 31, 73, 124), "Sealed Caves (Sirens): Condemned Shaft Lantern 2", null, SealedCavesSirens & LanternCube);

			areaName = "Military Fortress";
			Add(new ItemKey(10, 1, 136, 156), "Military Fortress: Entrance Lantern 1", null, MilitaryFortress & LanternCube);
			Add(new ItemKey(10, 1, 264, 156), "Military Fortress: Entrance Lantern 2", null, MilitaryFortress & LanternCube);
			Add(new ItemKey(10, 3, 232, 508), "Military Fortress: Bombing Room Upper Lantern 1", null, MilitaryFortressHangar & LanternCube);
			Add(new ItemKey(10, 3, 232, 876), "Military Fortress: Bombing Room Upper Lantern 2", null, MilitaryFortressHangar & LanternCube);
			Add(new ItemKey(10, 3, 568, 508), "Military Fortress: Bombing Room Lower Lantern 1", null, MilitaryFortress & LanternCube);
			Add(new ItemKey(10, 3, 568, 876), "Military Fortress: Bombing Room Lower Lantern 2", null, MilitaryFortress & LanternCube);
			Add(new ItemKey(10, 18, 120, 156), "Military Fortress: Pedestal Lantern", null, LabEntrance & (FloodsFlags.Lab ? R.Free : DoubleJumpOfNpc | ForwardDashDoubleJump) & LanternCube);
			Add(new ItemKey(10, 4, 1512, 140), "Military Fortress: Left Bridge Lantern 1", null, MilitaryFortressHangar & LanternCube);
			Add(new ItemKey(10, 4, 264, 140), "Military Fortress: Left Bridge Lantern 2", null, MilitaryFortressHangar & LanternCube);
			Add(new ItemKey(10, 4, 744, 140), "Military Fortress: Left Bridgee Lantern 3", null, MilitaryFortressHangar & LanternCube);
			Add(new ItemKey(10, 5, 120, 556), "Military Fortress: Middle Room Lantern 1", null, MilitaryFortressHangar & LanternCube);
			Add(new ItemKey(10, 5, 280, 556), "Military Fortress: Middle Room Lantern 2", null, MilitaryFortressHangar & LanternCube);
			Add(new ItemKey(10, 8, 1512, 140), "Military Fortress: Right Bridge Lantern 1", null, MilitaryFortressHangar & LanternCube);
			Add(new ItemKey(10, 8, 248, 140), "Military Fortress: Right Bridge Lantern 2", null, MilitaryFortressHangar & LanternCube);
			Add(new ItemKey(10, 8, 632, 140), "Military Fortress: Right Bridge Lantern 3", null, MilitaryFortressHangar & LanternCube);
			Add(new ItemKey(10, 6, 1048, 556), "Military Fortress: Spike Room Lantern 1", null, LabEntrance & NeedSwimming(FloodsFlags.Lab) & LanternCube);
			Add(new ItemKey(10, 6, 152, 556), "Military Fortress: Spike Room Lantern 2", null, LabEntrance & NeedSwimming(FloodsFlags.Lab) & LanternCube);

			areaName = "The Lab";
			Add(new ItemKey(11, 0, 136, 169), "Lab: Lower Trash Entrance Lantern 1", null, MainLab & LanternCube);
			Add(new ItemKey(11, 0, 264, 169), "Lab: Lower Trash Entrance Lantern 2", null, MainLab & LanternCube);
			Add(new ItemKey(11, 36, 126, 176), "Lab: Coffee Lantern 1", null, MainLab & LanternCube);
			Add(new ItemKey(11, 36, 190, 176), "Lab: Coffee Lantern 2", null, MainLab & LanternCube);
			Add(new ItemKey(11, 36, 254, 176), "Lab: Coffee Lantern 3", null, MainLab & LanternCube);
			Add(new ItemKey(11, 16, 504, 169), "Lab: Sentry Lantern 1", null, (SeedOptions.LockKeyAmadeus ? UpperLab | (MainLab & R.LabGenza) : LabResearchWing) & LanternCube);
			Add(new ItemKey(11, 16, 696, 169), "Lab: Sentry Lantern 2", null, (SeedOptions.LockKeyAmadeus ? UpperLab | (MainLab & R.LabGenza) : LabResearchWing) & LanternCube);
			Add(new ItemKey(11, 35, 120, 1209), "Lab: Main Shaft Lantern 1", null, MainLab & LanternCube);
			Add(new ItemKey(11, 35, 120, 169), "Lab: Main Shaft Lantern 2", null, MainLab & LanternCube);
			Add(new ItemKey(11, 35, 120, 537), "Lab: Main Shaft Lantern 3", null, MainLab & LanternCube);
			Add(new ItemKey(11, 35, 120, 905), "Lab: Main Shaft Lantern 4", null, MainLab & LanternCube);
			Add(new ItemKey(11, 35, 264, 169), "Lab: Main Shaft Lantern 5", null, MainLab & LanternCube);
			Add(new ItemKey(11, 35, 280, 1209), "Lab: Main Shaft Lantern 6", null, MainLab & LanternCube);
			Add(new ItemKey(11, 35, 280, 537), "Lab: Main Shaft Lantern 7", null, MainLab & LanternCube);
			Add(new ItemKey(11, 35, 280, 905), "Lab: Main Shaft Lantern 8", null, MainLab & LanternCube);
			Add(new ItemKey(11, 4, 136, 169), "Lab: File Cabinet Lantern 1", null, UpperLab & LanternCube);
			Add(new ItemKey(11, 4, 264, 169), "Lab: File Cabinet Lantern 2", null, UpperLab & LanternCube);
			Add(new ItemKey(11, 22, 104, 169), "Lab: File Cabinet Staircase Lantern 1", null, UpperLab & LanternCube);
			Add(new ItemKey(11, 22, 104, 505), "Lab: File Cabinet Staircase Lantern 2", null, UpperLab & LanternCube);
			Add(new ItemKey(11, 22, 280, 169), "Lab: File Cabinet Staircase Lantern 3", null, UpperLab & LanternCube);
			Add(new ItemKey(11, 22, 280, 505), "Lab: File Cabinet Staircase Lantern 4", null, UpperLab & LanternCube);
			Add(new ItemKey(11, 17, 216, 281), "Lab: Trash Stairs Lantern 1", null, MainLab & LanternCube);
			Add(new ItemKey(11, 17, 120, 505), "Lab: Trash Stairs Lantern 2", null, MainLab & LanternCube);
			Add(new ItemKey(11, 18, 126, 144), "Lab: Trash Jump Lantern 1", null, MainLab & (SeedOptions.LockKeyAmadeus ? DoubleJumpOfNpc : R.UpwardDash) & LanternCube);
			Add(new ItemKey(11, 18, 782, 112), "Lab: Trash Jump Lantern 2", null, MainLab & (SeedOptions.LockKeyAmadeus ? DoubleJumpOfNpc : R.UpwardDash) & LanternCube);
			Add(new ItemKey(11, 19, 120, 169), "Lab: Genza Door Lantern 1", null, UpperLab & LanternCube);
			Add(new ItemKey(11, 19, 264, 169), "Lab: Genza Door Lantern 2", null, UpperLab & LanternCube);
			Add(new ItemKey(11, 23, 104, 169), "Lab: Spider Hell Entrance Lantern 1", null, LabResearchWing & R.CardA & LanternCube);
			Add(new ItemKey(11, 23, 280, 169), "Lab: Spider Hell Entrance Lantern 2", null, LabResearchWing & R.CardA & LanternCube);
			Add(new ItemKey(11, 3, 1448, 489), "Lab: Lower Trash Lantern 1", null, MainLab & (FloodsFlags.Lab ? R.Free : (SeedOptions.LockKeyAmadeus ? DoubleJumpOfNpc : R.UpwardDash)) & LanternCube);
			Add(new ItemKey(11, 3, 152, 489), "Lab: Lower Trash Lantern 2", null, MainLab & (FloodsFlags.Lab ? R.Free : (SeedOptions.LockKeyAmadeus ? DoubleJumpOfNpc : R.UpwardDash)) & LanternCube);
			Add(new ItemKey(11, 30, 152, 169), "Lab: Intro Hallway Lantern", null, LabEntrance & LanternCube);
			Add(new ItemKey(11, 34, 136, 169), "Lab: Exp. 13 Terminal Lantern 1", null, MainLab & LanternCube);
			Add(new ItemKey(11, 34, 264, 169), "Lab: Exp. 13 Terminal Lantern 2", null, MainLab & LanternCube);
			Add(new ItemKey(11, 37, 136, 169), "Lab: Left Terminal Lantern 1", null, MainLab & LanternCube);
			Add(new ItemKey(11, 37, 264, 169), "Lab: Left Terminal Lantern 2", null, MainLab & LanternCube);
			Add(new ItemKey(11, 6, 136, 169), "Lab: Download and Chest Lantern 1", null, UpperLab & LanternCube);
			Add(new ItemKey(11, 6, 264, 169), "Lab: Download and Chest Lantern 2", null, UpperLab & LanternCube);

			areaName = "Emperor's Tower";
			Add(new ItemKey(12, 1, 122, 572), "Emperor's Tower: Courtyard Lantern 1", null, EmperorsTowerCourtyard & LanternCube);
			Add(new ItemKey(12, 1, 234, 572), "Emperor's Tower: Courtyard Lantern 2", null, EmperorsTowerCourtyard & LanternCube);
			Add(new ItemKey(12, 1, 218, 172), "Emperor's Tower: Courtyard Staircase Lantern", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 3, 138, 124), "Emperor's Tower: Courtyard floor secret Lantern 1", null, EmperorsTower & R.UpwardDash & OculusRift & LanternCube);
			Add(new ItemKey(12, 3, 282, 124), "Emperor's Tower: Courtyard floor secret Lantern 2", null, EmperorsTower & R.UpwardDash & OculusRift & LanternCube);
			Add(new ItemKey(12, 12, 1034, 140), "Emperor's Tower: Lower Hallway Lantern 1", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 12, 1466, 140), "Emperor's Tower: Lower Hallway Lantern 2", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 12, 170, 140), "Emperor's Tower: Lower Hallway Lantern 3", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 12, 602, 140), "Emperor's Tower: Lower Hallway Lantern 4", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 16, 122, 140), "Emperor's Tower: Lower Left Room Lantern 1", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 16, 266, 140), "Emperor's Tower: Lower Left Room Lantern 2", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 17, 106, 172), "Emperor's Tower: Outside Way Up There Lantern 1", null, EmperorsTower & DoubleJumpOfNpc & LanternCube);
			Add(new ItemKey(12, 17, 282, 172), "Emperor's Tower: Outside Way Up There Lantern 2", null, EmperorsTower & DoubleJumpOfNpc & LanternCube);
			Add(new ItemKey(12, 19, 106, 156), "Emperor's Tower: Way Up There Lantern 1", null, EmperorsTower & DoubleJumpOfNpc & LanternCube);
			Add(new ItemKey(12, 19, 282, 156), "Emperor's Tower: Way Up There Lantern 2", null, EmperorsTower & DoubleJumpOfNpc & LanternCube);
			Add(new ItemKey(12, 21, 122, 892), "Emperor's Tower: Lower Left Tower Lantern 1", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 21, 298, 892), "Emperor's Tower: Lower Left Tower Lantern 2", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 22, 106, 156), "Emperor's Tower: Galactic Sage Lantern 1", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 22, 282, 156), "Emperor's Tower: Galactic Sage Lantern 2", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 24, 122, 140), "Emperor's Tower: Courtyard Giantess Lantern 1", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 24, 266, 140), "Emperor's Tower: Courtyard Giantess Lantern 2", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 25, 122, 140), "Emperor's Tower: Courtyard Upper Lantern", null, EmperorsTower & R.UpwardDash & LanternCube);
			Add(new ItemKey(12, 26, 106, 156), "Emperor's Tower: Lower Save Room Lantern 1", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 26, 282, 156), "Emperor's Tower: Lower Save Room Lantern 2", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 4, 106, 572), "Emperor's Tower: Climb Past Stairs Lantern 1", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 4, 106, 172), "Emperor's Tower: Climb Past Stairs Lantern 2", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 4, 282, 172), "Emperor's Tower: Climb Past Stairs Lantern 3", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 4, 282, 572), "Emperor's Tower: Climb Past Stairs Lantern 4", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 5, 122, 156), "Emperor's Tower: Courtyard Bottom Lantern 1", null, EmperorsTowerCourtyard & LanternCube);
			Add(new ItemKey(12, 5, 298, 156), "Emperor's Tower: Courtyard Bottom Lantern 2", null, EmperorsTowerCourtyard & LanternCube);
			Add(new ItemKey(12, 6, 1178, 140), "Emperor's Tower: Middle Bridge Lantern 1", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 6, 1466, 140), "Emperor's Tower: Middle Bridge Lantern 2", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 6, 314, 140), "Emperor's Tower: Middle Bridge Lantern 3", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 6, 602, 140), "Emperor's Tower: Middle Bridge Lantern 4", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 6, 890, 140), "Emperor's Tower: Middle Bridge Lantern 5", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 7, 202, 1244), "Emperor's Tower: Upper Left Tower Lantern 1", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 7, 202, 332), "Emperor's Tower: Upper Left Tower Lantern 2", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 9, 122, 892), "Emperor's Tower: Lower Right Tower Lantern 1", null, EmperorsTower & LanternCube);
			Add(new ItemKey(12, 9, 298, 892), "Emperor's Tower: Lower Right Tower Lantern 2", null, EmperorsTower & LanternCube);


			areaName = "Upper Lake Serene";
			Add(new ItemKey(7, 1, 2568, 107), "Lake Serene (Upper): Intro Room Lantern", null, UpperLakeSirine & LanternCube);
			Add(new ItemKey(7, 14, 104, 283), "Lake Serene (Upper): Middle Cave Lantern", null, UpperLakeSirine & LanternCube);
			Add(new ItemKey(7, 15, 200, 187), "Lake Serene (Upper): Uncrashed Site Lantern 1", null, UpperLakeSirine & LanternCube);
			Add(new ItemKey(7, 15, 2104, 187), "Lake Serene (Upper): Uncrashed Site Lantern 2", null, UpperLakeSirine & LanternCube);
			Add(new ItemKey(7, 15, 2200, 187), "Lake Serene (Upper): Uncrashed Site Lantern 3", null, UpperLakeSirine & LanternCube);
			Add(new ItemKey(7, 15, 296, 187), "Lake Serene (Upper): Uncrashed Site Lantern 4", null, UpperLakeSirine & LanternCube);
			Add(new ItemKey(7, 17, 312, 267), "Lake Serene (Upper): Past First Vine Lantern", null, UpperLakeSirine & LanternCube);
			Add(new ItemKey(7, 3, 120, 379), "Lake Serene (Upper): Fork Dry Lantern", null, LeftSideForestCaves & LanternCube);
			Add(new ItemKey(7, 4, 328, 155), "Lake Serene (Upper): Past Cantoran Lantern", null, LeftSideForestCaves & LanternCube);

			areaName = "Lower Lake Serene";
			Add(new ItemKey(7, 3, 328, 736), "Lake Serene (Upper): Fork Wet Lantern 1", null, LeftSideForestCaves & R.Swimming & LanternCube);
			Add(new ItemKey(7, 3, 392, 1104), "Lake Serene (Upper): Fork Wet Lantern 2", null, LeftSideForestCaves & R.Swimming & LanternCube);
			Add(new ItemKey(7, 11, 184, 160), "Lake Serene (Lower): Underwater Secret Lantern 1", null, LowerLakeSirine & OculusRift & LanternCube);
			Add(new ItemKey(7, 11, 248, 160), "Lake Serene (Lower): Underwater Secret Lantern 2", null, LowerLakeSirine & OculusRift & LanternCube);
			Add(new ItemKey(7, 11, 312, 160), "Lake Serene (Lower): Underwater Secret Lantern 3", null, LowerLakeSirine & OculusRift & LanternCube);
			Add(new ItemKey(7, 7, 1112, 592), "Lake Serene (Lower): Under The Eels Lantern 1", null, LeftSideForestCaves & R.Swimming & LanternCube);
			Add(new ItemKey(7, 7, 1544, 592), "Lake Serene (Lower): Under The Eels Lantern 2", null, LeftSideForestCaves & R.Swimming & LanternCube);
			Add(new ItemKey(7, 7, 1704, 155), "Lake Serene (Lower): Under The Eels Lantern 3", null, LeftSideForestCaves & R.Swimming & LanternCube);
			Add(new ItemKey(7, 7, 488, 576), "Lake Serene (Lower): Under The Eels Lantern 4", null, LeftSideForestCaves & R.Swimming & LanternCube);
			Add(new ItemKey(7, 7, 824, 608), "Lake Serene (Lower): Under The Eels Lantern 5", null, LeftSideForestCaves & R.Swimming & LanternCube);
			Add(new ItemKey(7, 6, 1336, 256), "Lake Serene (Lower): Water Spikes Lantern 1", null, LowerLakeSirine & LanternCube);
			Add(new ItemKey(7, 6, 472, 432), "Lake Serene (Lower): Water Spikes Lantern 2", null, LowerLakeSirine & LanternCube);
			Add(new ItemKey(7, 6, 568, 192), "Lake Serene (Lower): Water Spikes Lantern 3", null, LowerLakeSirine & LanternCube);
			Add(new ItemKey(7, 2, 1480, 688), "Lake Serene (Lower): Underwater Lantern 1", null, LowerLakeSirine & LanternCube);
			Add(new ItemKey(7, 2, 152, 688), "Lake Serene (Lower): Underwater Lantern 2", null, LowerLakeSirine & LanternCube);
			Add(new ItemKey(7, 2, 936, 656), "Lake Serene (Lower): Underwater Lantern 3", null, LowerLakeSirine & LanternCube);
			Add(new ItemKey(7, 20, 184, 240), "Lake Serene (Lower): Past the Eels Lantern", null, LeftSideForestCaves & R.Swimming & LanternCube);

			areaName = "Caves of Banishment (Maw)";
			Add(new ItemKey(8, 10, 120, 124), "Caves of Banishment (Maw): Not Dead Yet Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 10, 184, 828), "Caves of Banishment (Maw): Not Dead Yet Lantern 2", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 10, 232, 828), "Caves of Banishment (Maw): Not Dead Yet Lantern 3", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 10, 296, 124), "Caves of Banishment (Maw): Not Dead Yet Lantern 4", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 0, 1032, 140), "Caves of Banishment (Maw): First Hall Lantern 1" , null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 0, 1176, 140), "Caves of Banishment (Maw): First Hall Lantern 2", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 0, 1416, 140), "Caves of Banishment (Maw): First Hall Lantern 3", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 0, 1576, 140), "Caves of Banishment (Maw): First Hall Lantern 4", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 0, 424, 140), "Caves of Banishment (Maw): First Hall Lantern 5", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 0, 568, 140), "Caves of Banishment (Maw): First Hall Lantern 6", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 37, 1032, 124), "Caves of Banishment (Maw): Second Hall Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 37, 1176, 124), "Caves of Banishment (Maw): Second Hall Lantern 2", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 37, 424, 124), "Caves of Banishment (Maw): Second Hall Lantern 3", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 37, 584, 124), "Caves of Banishment (Maw): Second Hall Lantern 4", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 19, 520, 876), "Caves of Banishment (Maw): Shroom Jump Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 19, 536, 140), "Caves of Banishment (Maw): Shroom Jump Lantern 2", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 19, 696, 876), "Caves of Banishment (Maw): Shroom Jump Lantern 3", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 19, 712, 140), "Caves of Banishment (Maw): Shroom Jump Lantern 4", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 38, 120, 124), "Caves of Banishment (Maw): Forked Shaft Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 38, 120, 508), "Caves of Banishment (Maw): Forked Shaft Lantern 2", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 38, 168, 828), "Caves of Banishment (Maw): Forked Shaft Lantern 3", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 38, 248, 828), "Caves of Banishment (Maw): Forked Shaft Lantern 4", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 38, 296, 124), "Caves of Banishment (Maw): Forked Shaft Lantern 5", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 9, 104, 124), "Caves of Banishment (Maw): Lower Fork Vertical Room Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 9, 104, 508), "Caves of Banishment (Maw): Lower Fork Vertical Room Lantern 2", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 9, 280, 124), "Caves of Banishment (Maw): Lower Fork Vertical Room Lantern 3", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 9, 280, 508), "Caves of Banishment (Maw): Lower Fork Vertical Room Lantern 4", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 14, 1050, 157), "Caves of Banishment (Maw): Penultimate Hall Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 14, 1258, 157), "Caves of Banishment (Maw): Penultimate Hall Lantern 2", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 14, 1466, 157), "Caves of Banishment (Maw): Penultimate Hall Lantern 3", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 14, 218, 157), "Caves of Banishment (Maw): Penultimate Hall Lantern 4", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 14, 426, 157), "Caves of Banishment (Maw): Penultimate Hall Lantern 5", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 14, 634, 157), "Caves of Banishment (Maw): Penultimate Hall Lantern 6", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 16, 1096, 140), "Caves of Banishment (Maw): Jackpot Ledge Lantern", null, CavesOfBanishmentFlooded & ForwardDashDoubleJump & LanternCube);
			Add(new ItemKey(8, 16, 104, 140), "Caves of Banishment (Maw): Pre-Jackpot Waterfall Room Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 16, 104, 1756), "Caves of Banishment (Maw): Pre-Jackpot Waterfall Room Lantern 2", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 16, 1096, 1756), "Caves of Banishment (Maw): Pre-Jackpot Waterfall Room Lantern 3", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 16, 1096, 780), "Caves of Banishment (Maw): Pre-Jackpot Waterfall Room Lantern 4", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 16, 920, 780), "Caves of Banishment (Maw): Pre-Jackpot Waterfall Room Lantern 5", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 43, 1050, 157), "Caves of Banishment (Maw): Rejoined Hallway Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 43, 1258, 157), "Caves of Banishment (Maw): Rejoined Hallway Lantern 2", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 43, 1466, 157), "Caves of Banishment (Maw): Rejoined Hallway Lantern 3", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 43, 218, 157), "Caves of Banishment (Maw): Rejoined Hallway Lantern 4", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 43, 426, 157), "Caves of Banishment (Maw): Rejoined Hallway Lantern 5", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 46, 104, 140), "Caves of Banishment (Maw): Lower Fork Start Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 46, 296, 140), "Caves of Banishment (Maw): Lower Fork Start Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 47, 122, 157), "Caves of Banishment (Maw): Post-Fork Room Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 47, 266, 157), "Caves of Banishment (Maw): Post-Fork Room Lantern 2", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 8, 282, 157), "Caves of Banishment (Maw): Last Chance Lantern 1", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 8, 392, 348), "Caves of Banishment (Maw): Last Chance Lantern 2", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 8, 392, 540), "Caves of Banishment (Maw): Last Chance Lantern 3", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 8, 506, 157), "Caves of Banishment (Maw): Last Chance Lantern 4", null, CavesOfBanishmentFlooded & LanternCube);
			Add(new ItemKey(8, 31, 105, 540), "Caves of Banishment (Maw): Mineshaft Lantern 1", null, CavesOfBanishmentFlooded & (MawGasMask | R.ForwardDash) & LanternCube);
			Add(new ItemKey(8, 31, 265, 572), "Caves of Banishment (Maw): Mineshaft Lantern 2", null, CavesOfBanishmentFlooded & (MawGasMask | R.ForwardDash) & LanternCube);
			Add(new ItemKey(8, 31, 281, 1516), "Caves of Banishment (Maw): Mineshaft Lantern 3", null, CavesOfBanishmentFlooded & (MawGasMask | R.ForwardDash) & LanternCube);
			Add(new ItemKey(8, 31, 329, 124), "Caves of Banishment (Maw): Mineshaft Lantern 4", null, CavesOfBanishmentFlooded & (MawGasMask | R.ForwardDash) & LanternCube);
			Add(new ItemKey(8, 31, 73, 124), "Caves of Banishment (Maw): Mineshaft Lantern 5", null, CavesOfBanishmentFlooded & (MawGasMask | R.ForwardDash) & LanternCube);

			areaName = "Caves of Banishment (Sirens)";
			Add(new ItemKey(8, 20, 1001, 140), "Caves of Banishment (Sirens): First Hall Lantern 1", null, UpperCavesOfBanishment & LanternCube);
			Add(new ItemKey(8, 20, 297, 140), "Caves of Banishment (Sirens): First Hall Lantern 2", null, UpperCavesOfBanishment & LanternCube);
			Add(new ItemKey(8, 20, 617, 140), "Caves of Banishment (Sirens): First Hall Lantern 3", null, UpperCavesOfBanishment & LanternCube);
			Add(new ItemKey(8, 5, 121, 156), "Caves of Banishment (Sirens): Second Hall Lantern 1", null, UpperCavesOfBanishment & LanternCube);
			Add(new ItemKey(8, 5, 1241, 156), "Caves of Banishment (Sirens): Second Hall Lantern 2", null, UpperCavesOfBanishment & LanternCube);
			Add(new ItemKey(8, 5, 377, 156), "Caves of Banishment (Sirens): Second Hall Lantern 3", null, UpperCavesOfBanishment & LanternCube);
			Add(new ItemKey(8, 5, 985, 156), "Caves of Banishment (Sirens): Second Hall Lantern 4", null, UpperCavesOfBanishment & LanternCube);
			Add(new ItemKey(8, 11, 153, 140), "Caves of Banishment (Sirens): Middle Hall Lantern 1", null, UpperCavesOfBanishment & LanternCube);
			Add(new ItemKey(8, 11, 409, 140), "Caves of Banishment (Sirens): Middle Hall Lantern 2", null, UpperCavesOfBanishment & LanternCube);
			Add(new ItemKey(8, 11, 793, 140), "Caves of Banishment (Sirens): Middle Hall Lantern 3", null, UpperCavesOfBanishment & LanternCube);
			Add(new ItemKey(8, 11, 985, 140), "Caves of Banishment (Sirens): Middle Hall Lantern 4", null, UpperCavesOfBanishment & LanternCube);

			areaName = "Forest";
			Add(new ItemKey(3, 1, 235, 125), "Forest: Lantern Past Signpost", null, LeftSideForestCaves  & LanternCube);
			Add(new ItemKey(3, 12, 1019, 109), "Forest: Rats Lantern", null, RefugeeCamp & LanternCube);
			Add(new ItemKey(3, 13, 1227, 125), "Forest: Ramparts Bridge Lantern 1", null, RefugeeCamp & LanternCube);
			Add(new ItemKey(3, 13, 251, 125), "Forest: Ramparts Bridge Lantern 2", null, RefugeeCamp & LanternCube);
			Add(new ItemKey(3, 13, 747, 125), "Forest: Ramparts Bridge Lantern 3", null, RefugeeCamp & LanternCube);
			Add(new ItemKey(3, 2, 235, 157), "Forest: Batcave Lantern", null, RefugeeCamp & LanternCube);
			Add(new ItemKey(3, 4, 1691, 77), "Forest: Lantern Before Broken Bridge", null, RefugeeCamp & LanternCube);
			Add(new ItemKey(3, 4, 411, 77), "Forest: Lantern After Broken Bridge", null, LeftSideForestCaves & LanternCube);
			Add(new ItemKey(3, 7, 1307, 125), "Forest: Left Caves Lantern", null, LeftSideForestCaves & LanternCube);

			areaName = "Castle Ramparts";
			Add(new ItemKey(4, 10, 122, 161), "Castle Ramparts: Giantess Lantern 1", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 10, 266, 161), "Castle Ramparts: Giantess Lantern 2", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 11, 138, 161), "Castle Ramparts: Archer + Knight Lantern 1", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 11, 282, 161), "Castle Ramparts: Archer + Knight Lantern 2", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 22, 298, 161), "Castle Ramparts: Pedestal Lantern", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 3, 234, 561), "Castle Ramparts: Big Boulder Room Lantern 1", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 3, 570, 561), "Castle Ramparts: Big Boulder Room Lantern 2", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 4, 1418, 129), "Castle Ramparts: Left Rooftops Lantern 1", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 4, 378, 129), "Castle Ramparts: Left Rooftops Lantern 2", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 4, 922, 129), "Castle Ramparts: Left Rooftops Lantern 3", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 5, 202, 497), "Castle Ramparts: Middle Hammer Lantern 1", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 5, 312, 264), "Castle Ramparts: Middle Hammer Lantern 2", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 5, 88, 264), "Castle Ramparts: Middle Hammer Lantern 3", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 6, 234, 561), "Castle Ramparts: Exit Lantern 1", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 6, 280, 104), "Castle Ramparts: Exit Lantern 2", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 6, 520, 104), "Castle Ramparts: Exit Lantern 3", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 6, 570, 561), "Castle Ramparts: Exit Lantern 4", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 7, 202, 497), "Castle Ramparts: Pedestal Stairs Lantern 1", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 7, 312, 264), "Castle Ramparts: Pedestal Stairs Lantern 2", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 7, 88, 264), "Castle Ramparts: Pedestal Stairs Lantern 3", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 8, 1418, 129), "Castle Ramparts: Right Rooftops Lantern 1", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 8, 362, 129), "Castle Ramparts: Right Rooftops Lantern 2", null, CastleRamparts & LanternCube);
			Add(new ItemKey(4, 8, 858, 129), "Castle Ramparts: Right Rooftops Lantern 3", null, CastleRamparts & LanternCube);

			areaName = "Castle Keep";
			Add(new ItemKey(5, 0, 1032, 173), "Castle Keep: Beginning Lantern 1", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 0, 184, 173), "Castle Keep: Beginning Lantern 2", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 0, 456, 173), "Castle Keep: Beginning Lantern 3", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 0, 744, 173), "Castle Keep: Beginning Lantern 4", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 1, 200, 188), "Castle Basement: Entrance Lantern 1", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 1, 200, 444), "Castle Basement: Entrance Lantern 2", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 1, 200, 700), "Castle Basement: Entrance Lantern 3", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 11, 122, 161), "Castle Basement: Exit Lantern 1", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 11, 298, 161), "Castle Basement: Exit Lantern 2", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 13, 200, 188), "Castle Basement: Exit Climb Lantern 1", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 13, 200, 444), "Castle Basement: Exit Climb Lantern 2", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 16, 298, 129), "Castle Basement: First Bird Hall Lantern 1", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 16, 490, 129), "Castle Basement: First Bird Hall Lantern 2", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 18, 200, 172), "Castle Keep: Far-Left Climb Lantern 1", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 18, 200, 492), "Castle Keep: Far-Left Climb Lantern 2", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 18, 200, 748), "Castle Keep: Far-Left Climb Lantern 3", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 19, 168, 157), "Castle Keep: Under The Twins Hallway Lantern 1", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 19, 424, 157), "Castle Keep: Under The Twins Hallway Lantern 2", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 19, 728, 157), "Castle Keep: Under The Twins Hallway Lantern 3", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 19, 984, 157), "Castle Keep: Under The Twins Hallway Lantern 4", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 21, 184, 157), "Castle Keep: Right-Middle Hallway Lantern 1", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 21, 440, 157), "Castle Keep: Right-Middle Hallway Lantern 2", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 21, 728, 157), "Castle Keep: Right-Middle Hallway Lantern 3", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 21, 984, 157), "Castle Keep: Right-Middle Hallway Lantern 4", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 22, 152, 141), "Castle Keep: Tiny Royal Guard Room Lantern 1", null, CastleKeep & ((R.TimeStop & R.ForwardDash) | R.DoubleJump) & LanternCube);
			Add(new ItemKey(5, 22, 264, 141), "Castle Keep: Tiny Royal Guard Room Lantern 2", null, CastleKeep & ((R.TimeStop & R.ForwardDash) | R.DoubleJump) & LanternCube);
			Add(new ItemKey(5, 29, 200, 204), "Castle Keep: Royal Tower Entrance Lantern 1", null, RoyalTower & LanternCube);
			Add(new ItemKey(5, 29, 200, 460), "Castle Keep: Royal Tower Entrance Lantern 2", null, RoyalTower & LanternCube);
			Add(new ItemKey(5, 3, 1000, 445), "Castle Keep: Foyer Lantern 1", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 3, 1016, 205), "Castle Keep: Foyer Lantern 2", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 3, 184, 205), "Castle Keep: Foyer Lantern 3", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 3, 200, 445), "Castle Keep: Foyer Lantern 4", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 3, 552, 285), "Castle Keep: Foyer Lantern 5", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 3, 648, 285), "Castle Keep: Foyer Lantern 6", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 32, 488, 141), "Castle Keep: Below Royal Room Lantern", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 33, 122, 161), "Castle Basement: Single Bird Lantern 1", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 33, 298, 161), "Castle Basement: Single Bird Lantern 2", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 35, 298, 129), "Castle Basement: Second Bird Hall Lantern 1", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 35, 490, 129), "Castle Basement: Second Bird Hall Lantern 2", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 39, 200, 157), "Castle Keep: Outside Aelana\'s Room Lantern 1", null, CastleKeep & R.DoubleJump & LanternCube);
			Add(new ItemKey(5, 39, 312, 157), "Castle Keep: Outside Aelana\'s Room Lantern 2", null, CastleKeep & R.DoubleJump & LanternCube);
			Add(new ItemKey(5, 39, 488, 157), "Castle Keep: Outside Aelana\'s Room Lantern 3", null, CastleKeep & R.DoubleJump & LanternCube);
			Add(new ItemKey(5, 39, 600, 157), "Castle Keep: Outside Aelana\'s Room Lantern 4", null, CastleKeep & R.DoubleJump & LanternCube);
			Add(new ItemKey(5, 4, 168, 173), "Castle Keep: Twins Door Lantern 1", null, CastleKeep & R.TimeStop & LanternCube);
			Add(new ItemKey(5, 4, 232, 173), "Castle Keep: Twins Door Lantern 2", null, CastleKeep & R.TimeStop & LanternCube);
			Add(new ItemKey(5, 41, 152, 173), "Castle Keep: Under The Twins Lantern 1", null, CastleKeep & LanternCube);
			Add(new ItemKey(5, 43, 200, 172), "Castle Basement: Center Shaft Lantern 1", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 43, 200, 428), "Castle Basement: Center Shaft Lantern 2", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 44, 122, 161), "Castle Basement: Giantess Lantern 1", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 44, 298, 161), "Castle Basement: Giantess Lantern 2", null, CastleBasement & LanternCube);
			Add(new ItemKey(5, 6, 264, 141), "Castle Keep: Twins Approach Lantern 1", null, CastleKeep & R.TimeStop & LanternCube);
			Add(new ItemKey(5, 6, 552, 141), "Castle Keep: Twins Approach Lantern 2", null, CastleKeep & R.TimeStop & LanternCube);
			Add(new ItemKey(5, 6, 808, 141), "Castle Keep: Twins Approach Lantern 3", null, CastleKeep & R.TimeStop & LanternCube);
			Add(new ItemKey(5, 7, 200, 188), "Castle Keep: Twins Stairwell Lantern 1", null, CastleKeep & R.TimeStop & LanternCube);
			Add(new ItemKey(5, 7, 200, 444), "Castle Keep: Twins Stairwell Lantern 2", null, CastleKeep & R.TimeStop & LanternCube);

			areaName = "Royal Towers";
			Add(new ItemKey(6, 1, 1114, 257), "Royal Towers: Long Balcony Lantern 1", null, MidRoyalTower & NeedSwimming(FloodsFlags.CastleCourtyard) & LanternCube);
			Add(new ItemKey(6, 1, 1402, 257), "Royal Towers: Long Balcony Lantern 2", null, MidRoyalTower & NeedSwimming(FloodsFlags.CastleCourtyard) & LanternCube);
			Add(new ItemKey(6, 1, 250, 257), "Royal Towers: Long Balcony Lantern 3", null, MidRoyalTower & NeedSwimming(FloodsFlags.CastleCourtyard) & LanternCube);
			Add(new ItemKey(6, 1, 538, 257), "Royal Towers: Long Balcony Lantern 4", null, MidRoyalTower & NeedSwimming(FloodsFlags.CastleCourtyard) & LanternCube);
			Add(new ItemKey(6, 1, 826, 257), "Royal Towers: Long Balcony Lantern 5", null, MidRoyalTower & NeedSwimming(FloodsFlags.CastleCourtyard) & LanternCube);
			Add(new ItemKey(6, 10, 138, 253), "Royal Towers: Bottom Struggle Base Lantern 1", null, MidRoyalTower & (FloodsFlags.CastleCourtyard ? R.Free : DoubleJumpOfNpc) & LanternCube);
			Add(new ItemKey(6, 10, 266, 253), "Royal Towers: Bottom Struggle Base Lantern 1", null, MidRoyalTower & (FloodsFlags.CastleCourtyard ? R.Free : DoubleJumpOfNpc) & LanternCube);
			Add(new ItemKey(6, 11, 298, 173), "Royal Towers: Before Aelana Lantern 1", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 16, 202, 157), "Royal Towers: Tower Base Entrance Lantern 1", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 16, 346, 157), "Royal Towers: Tower Base Entrance Lantern 2", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 16, 58, 157), "Royal Towers: Tower Base Entrance Lantern 3", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 2, 74, 349), "Royal Towers: Lantern Above Time-Stop Demon", null, MidRoyalTower & LanternCube);
			Add(new ItemKey(6, 2, 74, 541), "Royal Towers:  Lantern Below Time-Stop Demon", null, RoyalTower & LanternCube);
			Add(new ItemKey(6, 21, 154, 893), "Royal Towers: Left Tower Base Lantern 1", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 21, 250, 893), "Royal Towers: Left Tower Base Lantern 2", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 22, 154, 173), "Royal Towers: Left Royal Guard Lantern 1", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 22, 250, 173), "Royal Towers: Left Royal Guard Lantern 2", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 24, 122, 141), "Royal Towers: Before Bottom Struggle Lantern 1", null, MidRoyalTower & (FloodsFlags.CastleCourtyard ? R.Free : DoubleJumpOfNpc) & LanternCube);
			Add(new ItemKey(6, 24, 266, 141), "Royal Towers: Before Bottom Struggle Lantern 2", null, MidRoyalTower & (FloodsFlags.CastleCourtyard ? R.Free : DoubleJumpOfNpc) & LanternCube);
			Add(new ItemKey(6, 25, 135, 73), "Royal Towers: Past Bottom Struggle Lantern 1", null, MidRoyalTower & (FloodsFlags.CastleCourtyard ? R.Free : DoubleJumpOfNpc) & LanternCube);
			Add(new ItemKey(6, 25, 87, 73), "Royal Towers: Past Bottom Struggle Lantern 2", null, MidRoyalTower & (FloodsFlags.CastleCourtyard ? R.Free : DoubleJumpOfNpc) & LanternCube);
			Add(new ItemKey(6, 27, 394, 157), "Royal Towers: Pre-Climb Lantern 1", null, MidRoyalTower & LanternCube);
			Add(new ItemKey(6, 27, 394, 541), "Royal Towers: Pre-Climb Lantern 2", null, MidRoyalTower & LanternCube);
			Add(new ItemKey(6, 3, 330, 173), "Royal Towers: Bottom Struggle Lantern", null, MidRoyalTower & DoubleJumpOfNpc & LanternCube);
			Add(new ItemKey(6, 7, 202, 1245), "Royal Towers: Final Climb Lantern 1", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 7, 202, 333), "Royal Towers: Final Climb Lantern 2", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 9, 199, 217), "Royal Towers: Right Tower Base Lantern 1", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 9, 199, 537), "Royal Towers: Right Tower Base Lantern 2", null, UpperRoyalTower & LanternCube);
			Add(new ItemKey(6, 9, 199, 841), "Royal Towers: Right Tower Base Lantern 3", null, UpperRoyalTower & LanternCube);

			areaName = "Ancient Pyramid";
			Add(new ItemKey(16, 0, 152, 128), "Ancient Pyramid: Entrance Lantern 1", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(16, 0, 247, 128), "Ancient Pyramid: Entrance Lantern 2", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(16, 2, 1370, 822), "Ancient Pyramid: Rubble Lantern 1", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(16, 2, 202, 166), "Ancient Pyramid: Rubble Lantern 2", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(16, 2, 2202, 1526), "Ancient Pyramid: Rubble Lantern 3", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(16, 17, 154, 566), "Ancient Pyramid: Entrance Climb Lantern 1", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(16, 17, 250, 566), "Ancient Pyramid: Entrance Climb Lantern 1", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(16, 15, 154, 886), "Ancient Pyramid: First Enemy Lantern 1", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 15, 250, 886), "Ancient Pyramid: First Enemy Lantern 2", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 1, 202, 1526), "Ancient Pyramid: Nightmare Stairway Lantern 1", null, RightPyramid & NeedSwimming(FloodsFlags.BackPyramid) & LanternCube);
			Add(new ItemKey(16, 1, 2202, 166), "Ancient Pyramid: Nightmare Stairway Lantern 2", null, RightPyramid & NeedSwimming(FloodsFlags.BackPyramid) & LanternCube);
			Add(new ItemKey(16, 10, 202, 1526), "Ancient Pyramid: Upper-Left Stairway Lantern 1", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 10, 2202, 166), "Ancient Pyramid: Upper-Left Stairway Lantern 2", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 10, 634, 1494), "Ancient Pyramid: Upper-Left Stairway Lantern 3", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 11, 202, 166), "Ancient Pyramid: Upper-Right Stairway Lantern 1", null, RightPyramid & LanternCube);
			Add(new ItemKey(16, 11, 2202, 1526), "Ancient Pyramid: Upper-Right Stairway Lantern 2", null, RightPyramid & LanternCube);
			Add(new ItemKey(16, 14, 202, 166), "Ancient Pyramid: Why Not It's Right There Lantern", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(16, 20, 154, 886), "Ancient Pyramid: Regret Shaft Lantern 1", null, MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 20, 250, 866), "Ancient Pyramid: Regret Shaft Lantern 2", null, MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 16, 1002, 166), "Ancient Pyramid: Regret Lantern 1", null, MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 16, 1402, 166), "Ancient Pyramid: Regret Lantern 2", null, MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 16, 202, 166), "Ancient Pyramid: Regret Lantern 3", null, MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 16, 602, 166), "Ancient Pyramid: Regret Lantern 4", null, MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 18, 1002, 166), "Ancient Pyramid: Left Hallway Lantern 1", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 18, 1402, 166), "Ancient Pyramid: Left Hallway Lantern 2", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 18, 202, 166), "Ancient Pyramid: Left Hallway Lantern 3", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 18, 602, 166), "Ancient Pyramid: Left Hallway Lantern 4", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 22, 138, 166), "Ancient Pyramid: Pit Secret Lantern 1", null, MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 22, 266, 166), "Ancient Pyramid: Pit Secret Lantern 2", null, MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 23, 154, 566), "Ancient Pyramid: Pit Secret's Secret Lantern 1", null, MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 23, 250, 566), "Ancient Pyramid: Pit Secret's Secret Lantern 2", null, MidPyramid & OculusRift & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 24, 154, 566), "Ancient Pyramid: Outside Inner Warp Lantern 1", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 24, 250, 566), "Ancient Pyramid: Outside Inner Warp Lantern 2", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 25, 202, 166), "Ancient Pyramid: Nightmare Stairway Entrance Lantern", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 3, 152, 128), "Ancient Pyramid: Conviction Lantern 1", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 3, 247, 128), "Ancient Pyramid: Conviction Lantern 2", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 5, 152, 128), "Ancient Pyramid: Nightmare Door Lantern 1", null, RightPyramid & NeedSwimming(FloodsFlags.BackPyramid) & LanternCube);
			Add(new ItemKey(16, 5, 247, 128), "Ancient Pyramid: Nightmare Door Lantern 2", null, RightPyramid & NeedSwimming(FloodsFlags.BackPyramid) & LanternCube);
			Add(new ItemKey(16, 6, 154, 2486), "Ancient Pyramid: A Long Fall Lantern 1", null, MidPyramid & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 6, 250, 2486), "Ancient Pyramid: A Long Fall Lantern 2", null, MidPyramid & NeedSwimming(FloodsFlags.PyramidShaft) & LanternCube);
			Add(new ItemKey(16, 7, 202, 166), "Ancient Pyramid: Last Chance Before Shaft Lantern 1", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 7, 602, 166), "Ancient Pyramid: Last Chance Before Shaft Lantern 2", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 8, 202, 166), "Ancient Pyramid: Pit Secret Wall Lantern", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 9, 202, 166), "Ancient Pyramid: Post-Shaft Hallway Lantern 1", null, MidPyramid & LanternCube);
			Add(new ItemKey(16, 9, 602, 166), "Ancient Pyramid: Post-Shaft Hallway Lantern 2", null, MidPyramid & LanternCube);

			areaName = "Dark Forest";
			Add(new ItemKey(15, 0, 1144, 169), "Dark Forest: Pyramid Entrance Lantern 1", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(15, 0, 1448, 169), "Dark Forest: Pyramid Entrance Lantern 2", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(15, 0, 344, 169), "Dark Forest: Pyramid Entrance Lantern 3", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(15, 0, 616, 169), "Dark Forest: Pyramid Entrance Lantern 4", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(15, 0, 824, 169), "Dark Forest: Pyramid Entrance Lantern 5", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(15, 0, 88, 169), "Dark Forest: Pyramid Entrance Lantern 6", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(15, 2, 104, 537), "Dark Forest: Training Dummy Lantern 1", null, PyramidEntrance & LanternCube);
			Add(new ItemKey(15, 2, 344, 537), "Dark Forest: Training Dummy Lantern 2", null, PyramidEntrance & LanternCube);


			areaName = "Temporal Gyre";
			Add(new ItemKey(14, 12, 280, 128), "Temporal Gyre: Room 1 Lantern 1", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 12, 520, 128), "Temporal Gyre: Room 1 Lantern 2", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 13, 280, 128), "Temporal Gyre: Room 2 Lantern 1", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 13, 520, 128), "Temporal Gyre: Room 2 Lantern 2", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 15, 280, 128), "Temporal Gyre: Room 3 Lantern 1", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 15, 520, 128), "Temporal Gyre: Room 3 Lantern 2", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 16, 280, 128), "Temporal Gyre: Room 4 Lantern 1", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 16, 520, 128), "Temporal Gyre: Room 4 Lantern 2", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 18, 280, 128), "Temporal Gyre: Room 5 Lantern 1", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 18, 520, 128), "Temporal Gyre: Room 5 Lantern 2", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 19, 280, 128), "Temporal Gyre: Room 6 Lantern 1", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 19, 520, 128), "Temporal Gyre: Room 6 Lantern 2", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 21, 280, 128), "Temporal Gyre: Room 7 Lantern 1", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 21, 520, 128), "Temporal Gyre: Room 7 Lantern 2", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 22, 280, 128), "Temporal Gyre: Room 8 Lantern 1", null, TemporalGyre & LanternCube);
			Add(new ItemKey(14, 22, 520, 128), "Temporal Gyre: Room 8 Lantern 2", null, TemporalGyre & LanternCube);
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
				.Where(l => l.ItemInfo != null && l.ItemInfo.IsProgression)
				.ToArray();
		}

		public R GetAvailableRequirementsBasedOnObtainedItems()
		{
			var pickedUpProgressionItemLocations = this
				.Where(l => l.IsPickedUp && l.ItemInfo.IsProgression)
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
			//Gasmask may never be placed in a Gas affected place
			//the very basics to reach maw should also allow you to get Gasmask
			//unless we run inverted, then we can guarantee the user has the pyramid keys before entering lake desolation
			var gasmaskLocation = this.First(l => l.ItemInfo?.Identifier == new ItemIdentifier(EInventoryRelicType.AirMask));

			var levelIdsToAvoid = new List<int>(3) { 1 }; //lake desolation
			var mawRequirements = R.None;
	
			if (StartingEra == Era.Present)
			{
				mawRequirements |= R.GateAccessToPast;

				levelIdsToAvoid.Add(2); //library
				levelIdsToAvoid.Add(9); //xarion skeleton
			}
			else
			{
				if (!FloodsFlags.DryLakeSerene)
					mawRequirements |= R.Swimming;
				
				var pastUnlock = SeedOptions.UnchainedKeys
					? UnlockingMap.GetAllUnlock(CustomItem.GetIdentifier(CustomItemType.ModernWarpBeacon))
					: UnlockingMap.GetAllUnlock(new ItemIdentifier(EInventoryRelicType.PyramidsKey));
				
				mawRequirements |= pastUnlock;
			}

			return !levelIdsToAvoid.Contains(gasmaskLocation.Key.LevelId) && gasmaskLocation.Gate.CanBeOpenedWith(mawRequirements);
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

		bool CanCompleteGame(R obtainedRequirements) =>
			SeedOptions.DadPercent 
				? EmperorsTower.CanBeOpenedWith(obtainedRequirements)
				: Nightmare.CanBeOpenedWith(obtainedRequirements);

		public virtual void Update(Level level, GameplayScreen gameplayScreen)
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

		protected void Add(ItemKey itemKey, string name, ItemInfo defaultItem) => 
			Add(new ItemLocation(itemKey, areaName, name, defaultItem));

		protected void Add(ItemKey itemKey, string name, ItemInfo defaultItem, Gate gate) => 
			Add(new ItemLocation(itemKey, areaName, name, defaultItem, gate));

		public void AddCollected(ItemIdentifier item) => 
			Add(new ExternalItemLocation(ItemProvider.Get(item)));
	}

	class ProgressionChain
	{
		public IEnumerable<ItemLocation> Locations { get; set; }
		public ProgressionChain Sub { get; set; }
	}
}
