using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Saving;
using TsRanodmizer.ReplacementObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocationMap : LookupDictionairy<ItemKey, ItemLocation>
	{
		const int MaxBeatableCheckItterations = 25;

		static readonly Gate DoubleJumpOfNpc = Requirement.DoubleJump & Requirement.TimeStop;

		static readonly Requirement LowerLakeDesolationBridge = Requirement.TimeStop | Requirement.ForwardDash | Requirement.GateKittyBoss | Requirement.GateLeftLibrary;
		static readonly Gate AccessToPast = 
			(
               Requirement.TimeStop & Requirement.TimespinnerSpindle //activateLibraryTimespinner
               & (LowerLakeDesolationBridge & Requirement.CardD) //midLibrary
			) //libraryTimespinner
			| (Requirement.GateLakeSirineLeft & (Requirement.TimeStop | Requirement.ForwardDash)) //gateLakeSirine
			| Requirement.GateAccessToPast; //gateAccessToPast

		//past
		static readonly Gate AccessLeftSideForestCaves = (AccessToPast & (Requirement.TimeStop | Requirement.ForwardDash)) | Requirement.GateLakeSirineRight;
		static readonly Gate LeftSideForestCaves = AccessLeftSideForestCaves | Requirement.GateLakeSirineLeft;
		static readonly Gate CastleRamparts = LeftSideForestCaves;
		static readonly Gate CastleKeep = CastleRamparts;
		static readonly Gate RoyalTower = CastleKeep & Requirement.DoubleJump;
		static readonly Gate UpperRoyalTower = RoyalTower & DoubleJumpOfNpc;
		static readonly Gate UpperLakeSirine = (AccessLeftSideForestCaves & Requirement.TimeStop) | Requirement.GateLakeSirineLeft;
		static readonly Gate LowerlakeSirine = (LeftSideForestCaves | Requirement.GateLakeSirineLeft) & Requirement.Swimming;
		static readonly Gate LowerCavesOfBanishment = LowerlakeSirine;
		static readonly Gate UpperCavesOfBanishment = AccessToPast;

		//future
		static readonly Gate UpperLakeDesolation = AccessToPast & Requirement.AntiWeed;
		static readonly Gate LeftLibrary = UpperLakeDesolation | LowerLakeDesolationBridge;
		static readonly Gate UpperLeftLibrary = LeftLibrary & (Requirement.DoubleJump | Requirement.ForwardDash);
		static readonly Gate MidLibrary = LeftLibrary & Requirement.CardD;
		static readonly Gate RightSideLibrary = MidLibrary & (Requirement.CardC | (Requirement.CardB & Requirement.CardE));
		static readonly Gate RightSizeLibraryElevator = MidLibrary & Requirement.CardE & (Requirement.CardC | Requirement.CardB);
		static readonly Requirement SealedCavesLeft = Requirement.DoubleJump;
		static readonly Gate SealedCavesLower = SealedCavesLeft & Requirement.CardA;
		static readonly Gate SealedCavesSirens = (MidLibrary & Requirement.CardB) | Requirement.GateSealedSirensCave;
		static readonly Gate KillAll3MajorBosses = RightSideLibrary & CastleKeep & UpperRoyalTower & AccessToPast & Requirement.Swimming;
		static readonly Gate MilitairyFortress = KillAll3MajorBosses;
		static readonly Gate MilitairyFortressHangar = MilitairyFortress & Requirement.TimeStop;
		static readonly Gate RightSidemilitairyFortressHangar = MilitairyFortressHangar & Requirement.DoubleJump;
		static readonly Gate TheLab = MilitairyFortressHangar & Requirement.CardB;
		static readonly Gate TheLabPoweredOff = TheLab & DoubleJumpOfNpc;
		static readonly Gate EmperorsTower = TheLab;

		static readonly Gate LeftPyramid = TheLabPoweredOff
			& (Requirement.TimespinnerPiece1 & Requirement.TimespinnerPiece2 & Requirement.TimespinnerPiece3);
		static readonly Gate Nightmare = LeftPyramid & Requirement.UpwardDash;

		public new ItemLocation this[ItemKey key] => GetItemLocationBasedOnKeyOrRoomKey(key);

		public ItemLocationMap() : base(200, l => l.Key)
		{
			AddPresentItemLocations();
			AddPastItemLocations();
			AddPyramidItemLocations();
		}

		void AddPresentItemLocations()
		{
			//tutorial
			Add(ItemKey.TutorialMeleeOrb);
			Add(ItemKey.TutorialSpellOrb);
			//starter lake desolation
			Add(new ItemKey(1, 1, 1528, 144));
			Add(new ItemKey(1, 15, 264, 144));
			Add(new ItemKey(1, 25, 296, 176));
			Add(new ItemKey(1, 9, 600, 144 + TimespinnerWheel.YOffset));
			Add(new ItemKey(1, 14, 40, 176), UpperLakeDesolation);
			//lower lake desolation
			Add(new ItemKey(1, 2, 1016, 384), Requirement.TimeStop);
			Add(new ItemKey(1, 11, 72, 240), LowerLakeDesolationBridge);
			Add(new ItemKey(1, 3, 56, 176), Requirement.TimeStop);
			//upper lake desolation
			Add(new ItemKey(1, 17, 152, 96), UpperLakeDesolation);
			Add(new ItemKey(1, 21, 200, 144), UpperLakeDesolation);
			Add(new ItemKey(1, 20, 232, 96), UpperLakeDesolation & Requirement.TimeStop);
			Add(new ItemKey(1, 20, 168, 240), UpperLakeDesolation);
			Add(new ItemKey(1, 22, 344, 160), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1320, 189), UpperLakeDesolation);
			Add(new ItemKey(1, 18, 1272, 192), UpperLakeDesolation & Requirement.GassMask & KillAll3MajorBosses);
			Add(new ItemKey(1, 18, 1368, 192), UpperLakeDesolation & Requirement.GassMask & KillAll3MajorBosses);
			Add(new RoomItemKey(1, 5), UpperLakeDesolation | LowerLakeDesolationBridge); //kitty boss
			//libary left
			Add(new ItemKey(2, 60, 328, 160), LeftLibrary);
			Add(new ItemKey(2, 54, 296, 176), LeftLibrary);
			Add(new ItemKey(2, 44, 600, 368), LeftLibrary);
			Add(new ItemKey(2, 47, 216, 208), LeftLibrary & Requirement.CardD);
			Add(new ItemKey(2, 47, 152, 208), LeftLibrary & Requirement.CardD);
			Add(new ItemKey(2, 47, 88, 208), LeftLibrary & Requirement.CardD);
			//libary top
			Add(new ItemKey(2, 56, 168, 192), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 392, 192), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 616, 192), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 840, 192), UpperLeftLibrary);
			Add(new ItemKey(2, 56, 1064, 192), UpperLeftLibrary);
			//libary mid
			Add(new ItemKey(2, 34, 232, 1200), MidLibrary);
			Add(new ItemKey(2, 40, 344, 176), MidLibrary);
			Add(new ItemKey(2, 32, 328, 160), MidLibrary & Requirement.CardC);
			Add(new ItemKey(2, 7, 232, 144), MidLibrary);
			Add(new ItemKey(2, 25, 328, 192), MidLibrary & Requirement.CardE);
			//libary right, 
			Add(new ItemKey(2, 15, 760, 192), RightSideLibrary);
			Add(new ItemKey(2, 20, 72, 1200), RightSizeLibraryElevator);
			Add(new ItemKey(2, 23, 72, 560), RightSideLibrary);
			Add(new ItemKey(2, 23, 1112, 112), RightSideLibrary);
			Add(new ItemKey(2, 23, 136, 304), RightSideLibrary);
			Add(new ItemKey(2, 11, 104, 192), RightSideLibrary);
			Add(new ItemKey(2, 29, 280, 222 + TimespinnerSpindle.YOffset), RightSizeLibraryElevator);
			//Sealed Caves left
			Add(new ItemKey(9, 10, 248, 848), SealedCavesLeft);
			Add(new ItemKey(9, 19, 664, 704), SealedCavesLower & Requirement.TimeStop);
			Add(new ItemKey(9, 39, 88, 192), SealedCavesLower);
			Add(new ItemKey(9, 41, 312, 192), SealedCavesLower & (Requirement.UpwardDash | (Requirement.ForwardDash & Requirement.DoubleJump)));
			Add(new ItemKey(9, 42, 328, 192), SealedCavesLower);
			Add(new ItemKey(9, 12, 280, 160), SealedCavesLower);
			Add(new ItemKey(9, 48, 104, 160), SealedCavesLower);
			Add(new ItemKey(9, 15, 248, 192), SealedCavesLower);
			//Add(new ItemKey(9, 13, ???, ???), sealedCavesLeft); //TODO Timespinner Gear 3
			//Sealed Caves (sirens)
			Add(new ItemKey(9, 5, 88, 496), SealedCavesSirens & Requirement.Swimming);
			Add(new ItemKey(9, 3, 1848, 576), SealedCavesSirens & Requirement.Swimming);
			Add(new ItemKey(9, 3, 744, 560), SealedCavesSirens & Requirement.Swimming);
			Add(new ItemKey(9, 2, 184, 176), SealedCavesSirens);
			Add(new ItemKey(9, 2, 104, 160), SealedCavesSirens);
			//Militairy Fortress
			Add(new ItemKey(10, 3, 264, 128), MilitairyFortress & DoubleJumpOfNpc);
			Add(new ItemKey(10, 11, 296, 192), MilitairyFortress);
			Add(new ItemKey(10, 4, 1064, 176), MilitairyFortressHangar);
			Add(new ItemKey(10, 10, 104, 192), MilitairyFortressHangar);
			Add(new ItemKey(10, 8, 1080, 176), MilitairyFortressHangar);
			Add(new ItemKey(10, 7, 104, 192), RightSidemilitairyFortressHangar & Requirement.CardB);
			Add(new ItemKey(10, 7, 152, 192), RightSidemilitairyFortressHangar & Requirement.CardB);
			Add(new ItemKey(10, 18, 280, 189), RightSidemilitairyFortressHangar & (DoubleJumpOfNpc | Requirement.ForwardDash & Requirement.DoubleJump));
			// The lab
			Add(new ItemKey(11, 36, 312, 192), TheLab);
			Add(new ItemKey(11, 3, 1528, 192), TheLab & Requirement.DoubleJump);
			Add(new ItemKey(11, 3, 72, 192), TheLab & DoubleJumpOfNpc);
			Add(new ItemKey(11, 25, 104, 192), TheLab & Requirement.DoubleJump);
			Add(new ItemKey(11, 18, 824, 128), TheLabPoweredOff);
			Add(new ItemKey(11, 39, 200, 156), TheLabPoweredOff);
			Add(new ItemKey(11, 6, 328, 192), TheLabPoweredOff);
			Add(new ItemKey(11, 27, 296, 160), TheLabPoweredOff);
			Add(new RoomItemKey(11, 21), TheLabPoweredOff);
			//Emperors tower
			Add(new ItemKey(12, 5, 344, 192), EmperorsTower);
			Add(new ItemKey(12, 3, 200, 160), EmperorsTower & Requirement.UpwardDash);
			Add(new ItemKey(12, 25, 360, 176), EmperorsTower & Requirement.UpwardDash);
			Add(new ItemKey(12, 22, 56, 192), EmperorsTower);
			Add(new ItemKey(12, 9, 344, 928), EmperorsTower);
			Add(new ItemKey(12, 19, 72, 192), EmperorsTower & DoubleJumpOfNpc);
			Add(new ItemKey(12, 13, 120, 176), EmperorsTower);
			Add(new ItemKey(12, 11, 264, 208), EmperorsTower);
		}

		void AddPastItemLocations()
		{
			//Refugee Camp
			Add(new ItemKey(3, 0, 104, 160), Requirement.TimeStop & Requirement.TimespinnerSpindle
			                                 & (LowerLakeDesolationBridge & Requirement.CardD)); //neliste , if you join past through a gate you dont get it
			Add(new ItemKey(3, 30, 296, 176), AccessToPast);
			Add(new ItemKey(3, 30, 232, 176), AccessToPast);
			Add(new ItemKey(3, 30, 168, 176), AccessToPast);
			//Forest
			Add(new ItemKey(3, 3, 648, 272), AccessToPast);
			Add(new ItemKey(3, 15, 248, 112), AccessToPast & DoubleJumpOfNpc);
			Add(new ItemKey(3, 21, 120, 192), AccessToPast);
			Add(new ItemKey(3, 12, 776, 560), AccessToPast);
			Add(new ItemKey(3, 11, 392, 608), AccessToPast & Requirement.Swimming);
			Add(new ItemKey(3, 5, 184, 192), AccessToPast & Requirement.Swimming);
			Add(new ItemKey(3, 2, 584, 368), AccessToPast);
			Add(new ItemKey(4, 20, 264, 160), AccessToPast);
			Add(new ItemKey(3, 29, 248, 192), LeftSideForestCaves);
			//Upper Lake Sirine
			Add(new ItemKey(7, 16, 152, 96), UpperLakeSirine);
			Add(new ItemKey(7, 19, 248, 96), UpperLakeSirine & Requirement.DoubleJump);
			Add(new ItemKey(7, 19, 168, 240), UpperLakeSirine);
			Add(new ItemKey(7, 27, 184, 144), UpperLakeSirine);
			Add(new ItemKey(7, 13, 56, 176), UpperLakeSirine);
			//Lower Lake Sirine
			Add(new ItemKey(7, 3, 440, 1232), LowerlakeSirine);
			Add(new ItemKey(7, 7, 1432, 576), LowerlakeSirine);
			Add(new ItemKey(7, 6, 520, 496), LowerlakeSirine);
			Add(new ItemKey(7, 11, 88, 240), LowerlakeSirine);
			Add(new ItemKey(7, 2, 1016, 384), LowerlakeSirine);
			Add(new ItemKey(7, 20, 248, 96), LowerlakeSirine);
			Add(new ItemKey(7, 9, 584, 189), LowerlakeSirine);
			//Caves of Banishment
			Add(new ItemKey(8, 19, 664, 704), LowerCavesOfBanishment & Requirement.TimeStop);
			Add(new ItemKey(8, 12, 280, 160), LowerCavesOfBanishment);
			Add(new ItemKey(8, 48, 104, 160), LowerCavesOfBanishment);
			Add(new ItemKey(8, 39, 88, 192), LowerCavesOfBanishment);
			Add(new ItemKey(8, 41, 168, 192), LowerCavesOfBanishment & (Requirement.UpwardDash | Requirement.ForwardDash & Requirement.DoubleJump));
			Add(new ItemKey(8, 41, 216, 192), LowerCavesOfBanishment & (Requirement.UpwardDash | Requirement.ForwardDash & Requirement.DoubleJump));
			Add(new ItemKey(8, 41, 264, 192), LowerCavesOfBanishment & (Requirement.UpwardDash | Requirement.ForwardDash & Requirement.DoubleJump));
			Add(new ItemKey(8, 41, 312, 192), LowerCavesOfBanishment & (Requirement.UpwardDash | Requirement.ForwardDash & Requirement.DoubleJump));
			Add(new ItemKey(8, 42, 216, 189), LowerCavesOfBanishment & (Requirement.UpwardDash | Requirement.ForwardDash & Requirement.DoubleJump));
			Add(new ItemKey(8, 15, 248, 192), LowerCavesOfBanishment);
			Add(new ItemKey(8, 31, 88, 400), LowerCavesOfBanishment & Requirement.DoubleJump);
			//Caves of banishment (sirens)
			Add(new ItemKey(8, 4, 664, 144), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 808, 144), UpperCavesOfBanishment);
			Add(new ItemKey(8, 3, 744, 560), UpperCavesOfBanishment & Requirement.Swimming);
			Add(new ItemKey(8, 3, 1848, 576), UpperCavesOfBanishment & Requirement.Swimming);
			Add(new ItemKey(8, 5, 88, 496), UpperCavesOfBanishment & Requirement.Swimming);
			//Caste Ramparts
			Add(new ItemKey(4, 1, 456, 160), CastleRamparts & (Requirement.UpwardDash | Requirement.TimeStop));
			Add(new ItemKey(4, 3, 136, 144), CastleRamparts);
			Add(new ItemKey(4, 10, 56, 192), CastleRamparts);
			Add(new ItemKey(4, 11, 344, 192), CastleRamparts);
			Add(new ItemKey(4, 22, 104, 189), CastleRamparts);
			//Caste Keep
			Add(new ItemKey(5, 9, 104, 189), CastleKeep);
			Add(new ItemKey(5, 10, 104, 192), CastleKeep);
			Add(new ItemKey(5, 14, 88, 208), CastleKeep & Requirement.PinkOrb);
			Add(new ItemKey(5, 44, 216, 192), CastleKeep);
			Add(new ItemKey(5, 45, 104, 192), CastleKeep);
			Add(new ItemKey(5, 15, 296, 192), CastleKeep);
			Add(new ItemKey(5, 41, 72, 160), CastleKeep);
			Add(new RoomItemKey(5, 5), CastleKeep); //sucabus //was & Requirement.TimeStop but why?
			Add(new ItemKey(5, 22, 312, 176), CastleKeep & (Requirement.DoubleJump | Requirement.ForwardDash));
			//Royal towers
			Add(new ItemKey(6, 19, 200, 176), RoyalTower);
			Add(new ItemKey(6, 27, 472, 384), UpperRoyalTower);
			Add(new ItemKey(6, 1, 1512, 288), UpperRoyalTower);
			Add(new ItemKey(6, 25, 360, 176), UpperRoyalTower);
			Add(new ItemKey(6, 3, 120, 208), UpperRoyalTower);
			Add(new ItemKey(6, 17, 200, 112), UpperRoyalTower & Requirement.UpwardDash);
			Add(new ItemKey(6, 17, 360, 1840), UpperRoyalTower);
			Add(new ItemKey(6, 17, 56, 448), UpperRoyalTower);
			Add(new ItemKey(6, 13, 120, 176), UpperRoyalTower);
			Add(new ItemKey(6, 22, 88, 208), UpperRoyalTower);
			Add(new ItemKey(6, 11, 360, 544), UpperRoyalTower);
			Add(new ItemKey(6, 23, 856, 208), UpperRoyalTower);
			Add(new ItemKey(6, 14, 136, 208), UpperRoyalTower);
			Add(new ItemKey(6, 14, 184, 205), UpperRoyalTower);
		}

		void AddPyramidItemLocations()
		{
			Add(new ItemKey(16, 14, 312, 192), LeftPyramid);
			Add(new ItemKey(16, 3, 88, 192), LeftPyramid);
			Add(new ItemKey(16, 22, 200, 192), LeftPyramid);
			Add(new ItemKey(16, 16, 1512, 144), LeftPyramid);
			//Add(new ItemKey(16, 5, 136, 192), LeftPyramid); //Post nightmare

			/*var challengeDungion = Requirement.UpwardDash;
			Add(new ItemKey(14, 14, 200, 832), challengeDungion); //transition chest 1
			Add(new ItemKey(14, 17, 200, 832), challengeDungion); //transition chest 2
			Add(new ItemKey(14, 20, 200, 832), challengeDungion); //transition chest 3
			Add(new ItemKey(14, 8, 120, 176), challengeDungion); //Ravenlord pre fight
			Add(new ItemKey(14, 9, 280, 176), challengeDungion); //Ravenlord post fight
			Add(new ItemKey(14, 6, 40, 208), challengeDungion); //ifrid pre fight
			Add(new ItemKey(14, 7, 280, 208), challengeDungion); //ifrid post fight*/
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
			var obtainedRequirements = Requirement.None;
			var itteration = 0;

			do
			{
				itteration++;
				var previusObtainedRequirements = obtainedRequirements;

				obtainedRequirements = GetReachableLocations(obtainedRequirements)
					.Select(l => l.Unlocks)
					.Aggregate(Requirement.None, (current, unlock) => current | unlock);

				if (obtainedRequirements == previusObtainedRequirements)
					return false;

			} while (!CanCompleteGame(obtainedRequirements) && itteration < MaxBeatableCheckItterations);

			return true;
		}

		public IEnumerable<ItemLocation> GetReachableLocations(Requirement obtainedRequirements)
		{
			return this.Where(l => l.Gate.CanBeOpenedWith(obtainedRequirements));
		}

		static bool CanCompleteGame(Requirement obtainedRequirements)
		{
			return Nightmare.CanBeOpenedWith(obtainedRequirements);
		}

		public void BaseOnSave(GameSave gameSave)
		{
			foreach (var itemLocation in this)
				itemLocation.BsseOnGameSave(gameSave);
		}

		void Add(ItemKey itemKey)
		{
			Add(new ItemLocation(itemKey));
		}

		void Add(ItemKey itemKey, Requirement requirement)
		{
			Add(new ItemLocation(itemKey, requirement));
		}

		void Add(ItemKey itemKey, Gate gate)
		{
			Add(new ItemLocation(itemKey, gate));
		}
	}
}
