using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.ReplacementObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocationMap : LookupDictionairy<ItemKey, ItemLocation>
	{
		static readonly Requirement SecurityAccessA = Requirement.CardA;
		static readonly Requirement SecurityAccessB = Requirement.CardB | SecurityAccessA;
		static readonly Requirement SecurityAccessC = Requirement.CardC | SecurityAccessB;
		static readonly Requirement SecurityAccessD = Requirement.CardD | SecurityAccessC;
		static readonly Requirement DoubleJump = Requirement.DoubleJump | Requirement.UpwardDash;
		static readonly Requirement JumpOfNpc = Requirement.TimeStop | DoubleJump;
		static readonly Requirement ForwardJumpOfNpc = JumpOfNpc | Requirement.ForwardDash;
		static readonly Gate DoubleJumpOfNpc = DoubleJump & Requirement.TimeStop;

		public new ItemLocation this[ItemKey key] => GetItemLocationBasedOnKeyOrRoomKey(key);

		ItemLocationMap() : base(200, loc => loc.Key)
		{
			AddPresentItemLocations();
			AddPastItemLocations();
			AddPyramidItemLocations();
		}

		public static ItemLocationMap FromSeed(Seed seed)
		{
			var itemLocationMap = new ItemLocationMap();
			ItemLocationRandomizer.AddRandomItemsToLocationMap(itemLocationMap, seed);
			return itemLocationMap;
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
			Add(new ItemKey(1, 14, 40, 176), Requirement.AntiWeed);
			//lower lake desolation
			var lowerLakeDesolationBridge = ForwardJumpOfNpc | Requirement.GateKittyBoss | Requirement.GateLeftLibrary;
			Add(new ItemKey(1, 2, 1016, 384), JumpOfNpc);
			Add(new ItemKey(1, 11, 72, 240), lowerLakeDesolationBridge);
			Add(new ItemKey(1, 3, 56, 176), JumpOfNpc);
			//upper lake desolation
			var topLakeDesolation = Requirement.AntiWeed;
			Add(new ItemKey(1, 17, 152, 96), topLakeDesolation);
			Add(new ItemKey(1, 21, 200, 144), topLakeDesolation);
			Add(new ItemKey(1, 20, 232, 96), topLakeDesolation & DoubleJump);
			Add(new ItemKey(1, 20, 168, 240), topLakeDesolation);
			Add(new ItemKey(1, 22, 344, 160), topLakeDesolation);
			//Add(new ItemKey(1, 18, 1272, 192), topLakeDesolation & Requirement.GassMask); //TODO & KILL all 3 bosses
			//Add(new ItemKey(1, 18, 1368, 192), topLakeDesolation & Requirement.GassMask); //TODO & KILL all 3 bosses
			//TODO shattered orb
			//kitty boss
			Add(new RoomItemKey(1, 5), topLakeDesolation | lowerLakeDesolationBridge);
			//libary left
			var leftLibrary = topLakeDesolation | lowerLakeDesolationBridge;
			Add(new ItemKey(2, 60, 328, 160), leftLibrary);
			Add(new ItemKey(2, 54, 296, 176), leftLibrary);
			Add(new ItemKey(2, 44, 680, 368), leftLibrary);
			Add(new ItemKey(2, 34, 232, 1200), leftLibrary);
			Add(new ItemKey(2, 47, 216, 208), leftLibrary & SecurityAccessD);
			Add(new ItemKey(2, 47, 152, 208), leftLibrary & SecurityAccessD);
			Add(new ItemKey(2, 47, 88, 208), leftLibrary & SecurityAccessD);
			//libary top
			var upperLeftLibrary = leftLibrary & (DoubleJump | Requirement.ForwardDash);
			Add(new ItemKey(2, 56, 168, 192), upperLeftLibrary);
			Add(new ItemKey(2, 56, 392, 192), upperLeftLibrary);
			Add(new ItemKey(2, 56, 616, 192), upperLeftLibrary);
			Add(new ItemKey(2, 56, 840, 192), upperLeftLibrary);
			Add(new ItemKey(2, 56, 1064, 192), upperLeftLibrary);
			//libary mid
			var midLibrary = leftLibrary & SecurityAccessD;
			Add(new ItemKey(2, 32, 328, 160), midLibrary & SecurityAccessC);
			Add(new ItemKey(2, 7, 232, 144), midLibrary);
			Add(new ItemKey(2, 25, 328, 192), midLibrary & Requirement.CardE);
			//libary right, 
			var rightSideLibrary = midLibrary & (SecurityAccessC | (SecurityAccessB & Requirement.CardE));
			var rightSizeLibraryElevator = midLibrary & Requirement.CardE & (SecurityAccessC | SecurityAccessB);
			Add(new ItemKey(2, 15, 760, 192), rightSideLibrary);
			Add(new ItemKey(2, 20, 72, 1200), rightSizeLibraryElevator);
			Add(new ItemKey(2, 23, 72, 560), rightSideLibrary);
			Add(new ItemKey(2, 23, 1112, 112), rightSideLibrary);
			Add(new ItemKey(2, 23, 136, 304), rightSideLibrary);
			Add(new ItemKey(2, 11, 104, 192), rightSideLibrary);
			Add(new ItemKey(2, 29, 280, 222 + TimespinnerSpindle.YOffset), rightSizeLibraryElevator);
			//Sealed Caves left
			var sealedCavesLeft = Requirement.DoubleJump | SecurityAccessA;
			Add(new ItemKey(9, 19, 664, 704), sealedCavesLeft & JumpOfNpc);
			Add(new ItemKey(9, 39, 88, 192), sealedCavesLeft);
			Add(new ItemKey(9, 41, 312, 192),
				sealedCavesLeft & (Requirement.UpwardDash | (Requirement.ForwardDash & Requirement.DoubleJump)));
			Add(new ItemKey(9, 42, 328, 192), sealedCavesLeft);
			Add(new ItemKey(9, 12, 280, 160), sealedCavesLeft);
			Add(new ItemKey(9, 48, 104, 160), sealedCavesLeft);
			Add(new ItemKey(9, 15, 248, 192), sealedCavesLeft);
			//Add(new ItemKey(9, 13, ???, ???), sealedCavesLeft); //TODO Timespinner Gear 3
			//Sealed Caves (sirens)
			var sealedCavesSirens = (midLibrary & SecurityAccessB) | Requirement.GateSealedSirensCave;
			Add(new ItemKey(9, 5, 88, 496), sealedCavesSirens & Requirement.Swimming);
			Add(new ItemKey(9, 3, 1848, 576), sealedCavesSirens & Requirement.Swimming);
			Add(new ItemKey(9, 3, 744, 560), sealedCavesSirens & Requirement.Swimming);
			Add(new ItemKey(9, 2, 184, 176), sealedCavesSirens);
			Add(new ItemKey(9, 2, 104, 160), sealedCavesSirens);
			//Militairy Fortress
			var militairyFortress = rightSideLibrary; //TODO add lazer gates...
			Add(new ItemKey(10, 3, 264, 128), militairyFortress & DoubleJumpOfNpc);
			Add(new ItemKey(10, 11, 296, 192), militairyFortress);
			var militairyFortressHangar = militairyFortress & JumpOfNpc;
			Add(new ItemKey(10, 4, 1064, 176), militairyFortressHangar);
			Add(new ItemKey(10, 10, 104, 192), militairyFortressHangar);
			Add(new ItemKey(10, 8, 1080, 176), militairyFortressHangar);
			var rightSidemilitairyFortressHangar = militairyFortressHangar & DoubleJump;
			Add(new ItemKey(10, 7, 104, 192), rightSidemilitairyFortressHangar);
			Add(new ItemKey(10, 7, 152, 192), rightSidemilitairyFortressHangar);
			//Add(new ItemKey(10, 7, 152, 192), rightSidemilitairyFortressHangar & DoubleJumpOfNpc); TODO Gun Orb
			// The lab
			var theLab = militairyFortressHangar & SecurityAccessB;
			//TODO thelab
			//Emperors tower
			var emperorsTower = theLab;
			Add(new ItemKey(12, 5, 344, 192), emperorsTower);
			Add(new ItemKey(12, 3, 200, 160), emperorsTower & Requirement.UpwardDash);
			Add(new ItemKey(12, 25, 360, 176), emperorsTower & Requirement.UpwardDash);
			Add(new ItemKey(12, 22, 56, 192), emperorsTower);
			Add(new ItemKey(12, 9, 344, 928), emperorsTower);
			Add(new ItemKey(12, 19, 72, 192), emperorsTower & DoubleJumpOfNpc);
			Add(new ItemKey(12, 13, 120, 176), emperorsTower);
			//Add(new ItemKey(12, 11, 264, 208), emperorsTower); //TODO decide to use, Emperor completion chest...
		}

		void AddPastItemLocations()
		{
			var accessToPast = (Requirement.TimeStop & Requirement.TimespinnerSpindle) | Requirement.GateAccessToPast | (Requirement.GateLakeSirine & (JumpOfNpc | Requirement.ForwardDash));
			//Refugee Camp
			Add(new ItemKey(3, 0, 104, 160), accessToPast); //neliste
			Add(new ItemKey(3, 30, 296, 176), accessToPast);
			Add(new ItemKey(3, 30, 232, 176), accessToPast);
			Add(new ItemKey(3, 30, 168, 176), accessToPast);
			//Forest
			Add(new ItemKey(3, 3, 648, 272), accessToPast);
			Add(new ItemKey(3, 15, 248, 112), accessToPast & DoubleJumpOfNpc);
			Add(new ItemKey(3, 21, 120, 192), accessToPast);
			Add(new ItemKey(3, 12, 776, 560), accessToPast);
			Add(new ItemKey(3, 11, 392, 608), accessToPast & Requirement.Swimming);
			Add(new ItemKey(3, 5, 184, 192), accessToPast & Requirement.Swimming);
			Add(new ItemKey(3, 2, 584, 368), accessToPast);
			Add(new ItemKey(4, 20, 264, 160), accessToPast);
			var leftSideForestCaves = (accessToPast & (JumpOfNpc | Requirement.ForwardDash)) | Requirement.GateLakeSirine;
			Add(new ItemKey(3, 29, 248, 192), leftSideForestCaves);
			//Upper Lake Sirine
			var upperLakeSirine = leftSideForestCaves;
			Add(new ItemKey(7, 16, 152, 96), upperLakeSirine);
			Add(new ItemKey(7, 19, 248, 96), upperLakeSirine & Requirement.DoubleJump);
			Add(new ItemKey(7, 19, 168, 240), upperLakeSirine);
			Add(new ItemKey(7, 27, 184, 144), upperLakeSirine);
			Add(new ItemKey(7, 13, 56, 176), upperLakeSirine);
			//Lower Lake Sirine
			var lowerlakeSirine = leftSideForestCaves & Requirement.Swimming;
			Add(new ItemKey(7, 7, 1432, 576), lowerlakeSirine);
			Add(new ItemKey(7, 3, 440, 1232), lowerlakeSirine);
			Add(new ItemKey(7, 6, 520, 496), lowerlakeSirine);
			Add(new ItemKey(7, 11, 88, 240), lowerlakeSirine);
			Add(new ItemKey(7, 2, 1016, 384), lowerlakeSirine);
			//Caves of Banishment
			var lowerCavesOfBanishment = lowerlakeSirine;
			Add(new ItemKey(8, 19, 664, 704), lowerCavesOfBanishment & JumpOfNpc);
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
			var upperCavesOfBanishment = accessToPast;
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
			//Add(new ItemKey(???), castleKeep & JumpOfNpc); Sucobus drop
			Add(new ItemKey(5, 22, 312, 176), castleKeep & (DoubleJump | Requirement.ForwardDash));
			//Royal towers
			var royalTower = castleKeep & DoubleJump;
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
		}

		void AddPyramidItemLocations()
		{
			//TODO & access to world 3 accessToTimePortal2
			var accessToPyramid = Requirement.TimespinnerPiece1 & Requirement.TimespinnerPiece2 & Requirement.TimespinnerPiece3;
			var pyramid = Requirement.DoubleJump & accessToPyramid;
			Add(new ItemKey(16, 14, 312, 192), pyramid);
			Add(new ItemKey(16, 3, 88, 192), pyramid);
			Add(new ItemKey(16, 22, 200, 192), pyramid);
			Add(new ItemKey(16, 16, 1512, 144), pyramid);
			
			//var challengeDungion = Requirement.UpwardDash; //TODO & access to world 3
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

		public Gate GetItemGate(ItemKey key)
		{
			return GetItemLocationBasedOnKeyOrRoomKey(key)?.Gate;
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
			Add(new ItemLocation(itemKey));
		}

		public void Add(ItemKey itemKey, Requirement requirement)
		{
			Add(new ItemLocation(itemKey, requirement));
		}

		public void Add(ItemKey itemKey, Gate gate)
		{
			Add(new ItemLocation(itemKey, gate));
		}
	}
}
