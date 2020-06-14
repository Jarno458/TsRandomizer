using Timespinner.GameAbstractions.Saving;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.ReplacementObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocationMap : LookupDictionairy<ItemKey, ItemLocation>
	{
		static readonly Gate DoubleJumpOfNpc;
		static readonly Requirement LowerLakeDesolationBridge;
		static readonly Gate AccessToPast;

		readonly GameSave gameSave;

		public new ItemLocation this[ItemKey key] => GetItemLocationBasedOnKeyOrRoomKey(key);

		static ItemLocationMap()
		{
			var midLibrary = LowerLakeDesolationBridge & Requirement.CardD;
			var libraryTimespinner = Requirement.TimespinnerWheel & Requirement.TimespinnerSpindle & midLibrary;
			var gateLakeSirine = Requirement.GateLakeSirineLeft & (Requirement.TimeStop | Requirement.ForwardDash);

			DoubleJumpOfNpc = Requirement.DoubleJump & Requirement.TimeStop;
			LowerLakeDesolationBridge = Requirement.TimeStop | Requirement.ForwardDash | Requirement.GateKittyBoss | Requirement.GateLeftLibrary;
			AccessToPast = libraryTimespinner | gateLakeSirine | Requirement.GateAccessToPast;
		}

		public ItemLocationMap(GameSave gameSave) : base(200, l => l.Key)
		{
			this.gameSave = gameSave;

			AddPresentItemLocations();
			AddPastItemLocations();
			AddPyramidItemLocations();
		}

		void AddPresentItemLocations()
		{
			var upperLakeDesolation = AccessToPast & Requirement.AntiWeed;

			//tutorial
			Add(ItemKey.TutorialMeleeOrb);
			Add(ItemKey.TutorialSpellOrb);
			//starter lake desolation
			Add(new ItemKey(1, 1, 1528, 144));
			Add(new ItemKey(1, 15, 264, 144));
			Add(new ItemKey(1, 25, 296, 176));
			Add(new ItemKey(1, 9, 600, 144 + TimespinnerWheel.YOffset));
			Add(new ItemKey(1, 14, 40, 176), upperLakeDesolation);
			//lower lake desolation
			Add(new ItemKey(1, 2, 1016, 384), Requirement.TimeStop);
			Add(new ItemKey(1, 11, 72, 240), LowerLakeDesolationBridge);
			Add(new ItemKey(1, 3, 56, 176), Requirement.TimeStop);
			//upper lake desolation
			Add(new ItemKey(1, 17, 152, 96), upperLakeDesolation);
			Add(new ItemKey(1, 21, 200, 144), upperLakeDesolation);
			Add(new ItemKey(1, 20, 232, 96), upperLakeDesolation & Requirement.TimeStop);
			Add(new ItemKey(1, 20, 168, 240), upperLakeDesolation);
			Add(new ItemKey(1, 22, 344, 160), upperLakeDesolation);
			//Add(new ItemKey(1, 18, 1272, 192), topLakeDesolation & Requirement.GassMask); //TODO & KILL all 3 bosses
			//Add(new ItemKey(1, 18, 1368, 192), topLakeDesolation & Requirement.GassMask); //TODO & KILL all 3 bosses
			//TODO shattered orb
			Add(new RoomItemKey(1, 5), upperLakeDesolation | LowerLakeDesolationBridge); //kitty boss
			//libary left
			var leftLibrary = upperLakeDesolation | LowerLakeDesolationBridge;
			Add(new ItemKey(2, 60, 328, 160), leftLibrary);
			Add(new ItemKey(2, 54, 296, 176), leftLibrary);
			Add(new ItemKey(2, 44, 680, 368), leftLibrary);
			Add(new ItemKey(2, 47, 216, 208), leftLibrary & Requirement.CardD);
			Add(new ItemKey(2, 47, 152, 208), leftLibrary & Requirement.CardD);
			Add(new ItemKey(2, 47, 88, 208), leftLibrary & Requirement.CardD);
			//libary top
			var upperLeftLibrary = leftLibrary & (Requirement.DoubleJump | Requirement.ForwardDash);
			Add(new ItemKey(2, 56, 168, 192), upperLeftLibrary);
			Add(new ItemKey(2, 56, 392, 192), upperLeftLibrary);
			Add(new ItemKey(2, 56, 616, 192), upperLeftLibrary);
			Add(new ItemKey(2, 56, 840, 192), upperLeftLibrary);
			Add(new ItemKey(2, 56, 1064, 192), upperLeftLibrary);
			//libary mid
			var midLibrary = leftLibrary & Requirement.CardD;
			Add(new ItemKey(2, 34, 232, 1200), midLibrary);
			Add(new ItemKey(2, 40, 344, 176), midLibrary);
			Add(new ItemKey(2, 32, 328, 160), midLibrary & Requirement.CardC);
			Add(new ItemKey(2, 7, 232, 144), midLibrary);
			Add(new ItemKey(2, 25, 328, 192), midLibrary & Requirement.CardE);
			//libary right, 
			var rightSideLibrary = midLibrary & (Requirement.CardC | (Requirement.CardB & Requirement.CardE));
			var rightSizeLibraryElevator = midLibrary & Requirement.CardE & (Requirement.CardC | Requirement.CardB);
			Add(new ItemKey(2, 15, 760, 192), rightSideLibrary);
			Add(new ItemKey(2, 20, 72, 1200), rightSizeLibraryElevator);
			Add(new ItemKey(2, 23, 72, 560), rightSideLibrary);
			Add(new ItemKey(2, 23, 1112, 112), rightSideLibrary);
			Add(new ItemKey(2, 23, 136, 304), rightSideLibrary);
			Add(new ItemKey(2, 11, 104, 192), rightSideLibrary);
			Add(new ItemKey(2, 29, 280, 222 + TimespinnerSpindle.YOffset), rightSizeLibraryElevator);
			//Sealed Caves left
			var SealedCavesLeft = Requirement.DoubleJump;
			Add(new ItemKey(9, 10, 248, 848), SealedCavesLeft);
			var SealedCavesLower = SealedCavesLeft & Requirement.CardA;
			Add(new ItemKey(9, 19, 664, 704), SealedCavesLower & Requirement.TimeStop);
			Add(new ItemKey(9, 39, 88, 192), SealedCavesLower);
			Add(new ItemKey(9, 41, 312, 192), SealedCavesLower & (Requirement.UpwardDash | (Requirement.ForwardDash & Requirement.DoubleJump)));
			Add(new ItemKey(9, 42, 328, 192), SealedCavesLower);
			Add(new ItemKey(9, 12, 280, 160), SealedCavesLower);
			Add(new ItemKey(9, 48, 104, 160), SealedCavesLower);
			Add(new ItemKey(9, 15, 248, 192), SealedCavesLower);
			//Add(new ItemKey(9, 13, ???, ???), sealedCavesLeft); //TODO Timespinner Gear 3
			//Sealed Caves (sirens)
			var sealedCavesSirens = (midLibrary & Requirement.CardB) | Requirement.GateSealedSirensCave;
			Add(new ItemKey(9, 5, 88, 496), sealedCavesSirens & Requirement.Swimming);
			Add(new ItemKey(9, 3, 1848, 576), sealedCavesSirens & Requirement.Swimming);
			Add(new ItemKey(9, 3, 744, 560), sealedCavesSirens & Requirement.Swimming);
			Add(new ItemKey(9, 2, 184, 176), sealedCavesSirens);
			Add(new ItemKey(9, 2, 104, 160), sealedCavesSirens);
			//Militairy Fortress
			var militairyFortress = rightSideLibrary; //TODO add lazer gates... Sucabus \ Alena \ Wraith Gate
			Add(new ItemKey(10, 3, 264, 128), militairyFortress & DoubleJumpOfNpc);
			Add(new ItemKey(10, 11, 296, 192), militairyFortress);
			var militairyFortressHangar = militairyFortress & Requirement.TimeStop;
			Add(new ItemKey(10, 4, 1064, 176), militairyFortressHangar);
			Add(new ItemKey(10, 10, 104, 192), militairyFortressHangar);
			Add(new ItemKey(10, 8, 1080, 176), militairyFortressHangar);
			var rightSidemilitairyFortressHangar = militairyFortressHangar & Requirement.DoubleJump;
			Add(new ItemKey(10, 7, 104, 192), rightSidemilitairyFortressHangar & Requirement.CardB);
			Add(new ItemKey(10, 7, 152, 192), rightSidemilitairyFortressHangar & Requirement.CardB);
			Add(new ItemKey(10, 18, 280, 189), rightSidemilitairyFortressHangar & (DoubleJumpOfNpc | Requirement.ForwardDash & Requirement.DoubleJump));
			// The lab
			var theLab = militairyFortressHangar & Requirement.CardB;
			Add(new ItemKey(11, 36, 312, 192), theLab);
			Add(new ItemKey(11, 25, 104, 192), theLab & Requirement.DoubleJump);
			var theLabPoweredOff = theLab & DoubleJumpOfNpc;
			Add(new ItemKey(11, 18, 824, 128), theLabPoweredOff);
			Add(new ItemKey(11, 39, 200, 156), theLabPoweredOff);
			Add(new ItemKey(11, 6, 328, 192), theLabPoweredOff);
			Add(new ItemKey(11, 27, 296, 160), theLabPoweredOff);
			Add(new RoomItemKey(11, 21), theLabPoweredOff);
			//Emperors tower
			var emperorsTower = theLab;
			Add(new ItemKey(12, 5, 344, 192), emperorsTower);
			Add(new ItemKey(12, 3, 200, 160), emperorsTower & Requirement.UpwardDash);
			Add(new ItemKey(12, 25, 360, 176), emperorsTower & Requirement.UpwardDash);
			Add(new ItemKey(12, 22, 56, 192), emperorsTower);
			Add(new ItemKey(12, 9, 344, 928), emperorsTower);
			Add(new ItemKey(12, 19, 72, 192), emperorsTower & DoubleJumpOfNpc);
			Add(new ItemKey(12, 13, 120, 176), emperorsTower);
			Add(new ItemKey(12, 11, 264, 208), emperorsTower);
		}

		void AddPastItemLocations()
		{
			//Refugee Camp
			Add(new ItemKey(3, 0, 104, 160), AccessToPast); //neliste
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
			var accessLeftSideForestCaves = (AccessToPast & (Requirement.TimeStop | Requirement.ForwardDash)) | Requirement.GateLakeSirineRight;
			var leftSideForestCaves = accessLeftSideForestCaves | Requirement.GateLakeSirineLeft;
			Add(new ItemKey(3, 29, 248, 192), leftSideForestCaves);
			//Upper Lake Sirine
			var upperLakeSirine = (accessLeftSideForestCaves & Requirement.TimeStop) | Requirement.GateLakeSirineLeft;
			Add(new ItemKey(7, 16, 152, 96), upperLakeSirine);
			Add(new ItemKey(7, 19, 248, 96), upperLakeSirine & Requirement.DoubleJump);
			Add(new ItemKey(7, 19, 168, 240), upperLakeSirine);
			Add(new ItemKey(7, 27, 184, 144), upperLakeSirine);
			Add(new ItemKey(7, 13, 56, 176), upperLakeSirine);
			//Lower Lake Sirine
			var lowerlakeSirine = (leftSideForestCaves | Requirement.GateLakeSirineLeft) & Requirement.Swimming;
			Add(new ItemKey(7, 3, 440, 1232), lowerlakeSirine);
			Add(new ItemKey(7, 7, 1432, 576), lowerlakeSirine);
			Add(new ItemKey(7, 6, 520, 496), lowerlakeSirine);
			Add(new ItemKey(7, 11, 88, 240), lowerlakeSirine);
			Add(new ItemKey(7, 2, 1016, 384), lowerlakeSirine);
			//Caves of Banishment
			var lowerCavesOfBanishment = lowerlakeSirine;
			Add(new ItemKey(8, 19, 664, 704), lowerCavesOfBanishment & Requirement.TimeStop);
			Add(new ItemKey(8, 12, 280, 160), lowerCavesOfBanishment);
			Add(new ItemKey(8, 48, 104, 160), lowerCavesOfBanishment);
			Add(new ItemKey(8, 39, 88, 192), lowerCavesOfBanishment);
			Add(new ItemKey(8, 41, 168, 192), lowerCavesOfBanishment & (Requirement.UpwardDash | Requirement.ForwardDash & Requirement.DoubleJump));
			Add(new ItemKey(8, 41, 216, 192), lowerCavesOfBanishment & (Requirement.UpwardDash | Requirement.ForwardDash & Requirement.DoubleJump));
			Add(new ItemKey(8, 41, 264, 192), lowerCavesOfBanishment & (Requirement.UpwardDash | Requirement.ForwardDash & Requirement.DoubleJump));
			Add(new ItemKey(8, 41, 312, 192), lowerCavesOfBanishment & (Requirement.UpwardDash | Requirement.ForwardDash & Requirement.DoubleJump));
			Add(new ItemKey(8, 15, 248, 192), lowerCavesOfBanishment & Requirement.DoubleJump);
			Add(new ItemKey(8, 31, 88, 400), lowerCavesOfBanishment & Requirement.DoubleJump);
			//Caves of banishment (sirens)
			var upperCavesOfBanishment = AccessToPast;
			Add(new ItemKey(8, 4, 664, 144), upperCavesOfBanishment);
			Add(new ItemKey(8, 3, 808, 144), upperCavesOfBanishment);
			Add(new ItemKey(8, 3, 744, 560), upperCavesOfBanishment & Requirement.Swimming);
			Add(new ItemKey(8, 3, 1848, 576), upperCavesOfBanishment & Requirement.Swimming);
			Add(new ItemKey(8, 5, 88, 496), upperCavesOfBanishment & Requirement.Swimming);
			//Caste Ramparts
			var castleRamparts = leftSideForestCaves;
			Add(new ItemKey(4, 1, 456, 160), castleRamparts & (Requirement.UpwardDash | Requirement.TimeStop));
			Add(new ItemKey(4, 3, 136, 144), castleRamparts);
			Add(new ItemKey(4, 10, 56, 192), castleRamparts);
			Add(new ItemKey(4, 11, 344, 192), castleRamparts);
			//Caste Keep
			var castleKeep = castleRamparts;
			Add(new ItemKey(5, 10, 104, 192), castleKeep);
			Add(new ItemKey(5, 44, 216, 192), castleKeep);
			Add(new ItemKey(5, 45, 104, 192), castleKeep);
			Add(new ItemKey(5, 15, 296, 192), castleKeep);
			Add(new ItemKey(5, 41, 72, 160), castleKeep);
			Add(new RoomItemKey(5, 5), castleKeep & Requirement.TimeStop); //sucabus
			Add(new ItemKey(5, 22, 312, 176), castleKeep & (Requirement.DoubleJump | Requirement.ForwardDash));
			//Royal towers
			var royalTower = castleKeep & Requirement.DoubleJump;
			Add(new ItemKey(6, 19, 200, 176), royalTower);
			var upperRoyalTower = royalTower & DoubleJumpOfNpc;
			Add(new ItemKey(6, 27, 472, 384), upperRoyalTower);
			Add(new ItemKey(6, 1, 1512, 288), upperRoyalTower);
			Add(new ItemKey(6, 25, 360, 176), upperRoyalTower);
			Add(new ItemKey(6, 3, 120, 208), upperRoyalTower);
			Add(new ItemKey(6, 17, 200, 132), upperRoyalTower);
			Add(new ItemKey(6, 17, 360, 1840), upperRoyalTower);
			Add(new ItemKey(6, 17, 56, 448), upperRoyalTower & Requirement.UpwardDash);
			Add(new ItemKey(6, 13, 120, 176), upperRoyalTower);
			Add(new ItemKey(6, 22, 88, 208), upperRoyalTower);
			Add(new ItemKey(6, 11, 360, 544), upperRoyalTower);
			Add(new ItemKey(6, 23, 856, 208), upperRoyalTower);
			Add(new ItemKey(6, 14, 136, 208), upperRoyalTower);
			Add(new ItemKey(6, 14, 184, 205), upperRoyalTower);
		}

		void AddPyramidItemLocations()
		{
			//TODO & access to world 3 accessToTimePortal2
			//var accessToTimePortal2 = Requirement.None;
			var accessToPyramid = Requirement.TimespinnerPiece1 & Requirement.TimespinnerPiece2 & Requirement.TimespinnerPiece3;
			var pyramid = Requirement.DoubleJump & accessToPyramid;
			//disabled until requirement is fixed
			//Add(new ItemKey(16, 14, 312, 192), pyramid);
			//Add(new ItemKey(16, 3, 88, 192), pyramid);
			//Add(new ItemKey(16, 22, 200, 192), pyramid);
			//Add(new ItemKey(16, 16, 1512, 144), pyramid);
			
			//var challengeDungion = Requirement.UpwardDash;
			//Add(new ItemKey(14, 14, 200, 832), challengeDungion); //transition chest 1
			//Add(new ItemKey(14, 17, 200, 832), challengeDungion); //transition chest 2
			//Add(new ItemKey(14, 20, 200, 832), challengeDungion); //transition chest 3
			//Add(new ItemKey(14, 8, 120, 176), challengeDungion); //Ravenlord pre fight
			//Add(new ItemKey(14, 9, 280, 176), challengeDungion); //Ravenlord post fight
			//Add(new ItemKey(14, 6, 40, 208), challengeDungion); //ifrid pre fight
			//Add(new ItemKey(14, 7, 280, 208), challengeDungion); //ifrid post fight
		}

		public ItemInfo GetItemInfo(ItemKey key)
		{
			return GetItemLocationBasedOnKeyOrRoomKey(key)?.ItemInfo;
		}

		public ItemLocation GetItemLocation(ItemKey key)
		{
			return GetItemLocationBasedOnKeyOrRoomKey(key);
		}

		ItemLocation GetItemLocationBasedOnKeyOrRoomKey(ItemKey key)
		{
			return TryGetValue(key, out var itemLocation)
				? itemLocation
				: TryGetValue(key.ToRoomItemKey(), out var roomItemLocation)
					? roomItemLocation
					: null;
		}

		public void Add(ItemKey itemKey)
		{
			Add(new ItemLocation(gameSave, itemKey));
		}

		public void Add(ItemKey itemKey, Requirement requirement)
		{
			Add(new ItemLocation(gameSave, itemKey, requirement));
		}

		public void Add(ItemKey itemKey, Gate gate)
		{
			Add(new ItemLocation(gameSave, itemKey, gate));
		}
	}
}
