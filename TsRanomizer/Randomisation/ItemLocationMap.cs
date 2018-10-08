using System.Collections;
using System.Collections.Generic;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.ReplacementObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocationMap : IEnumerable<ItemLocation>
	{
		readonly ItemLocations itemLocations = new ItemLocations(200);

		static readonly ProgressionItem SecurityAccessA = ProgressionItem.CardA;
		static readonly ProgressionItem SecurityAccessB = ProgressionItem.CardB | SecurityAccessA;
		static readonly ProgressionItem SecurityAccessC = ProgressionItem.CardC | SecurityAccessB;
		static readonly ProgressionItem SecurityAccessD = ProgressionItem.CardD | SecurityAccessC;
		static readonly ProgressionItem UpwardDash = ProgressionItem.UpwardDash | ProgressionItem.Lightwall;
		static readonly ProgressionItem DoubleJump = ProgressionItem.DoubleJump | UpwardDash;
		static readonly ProgressionItem JumpOfNpc = ProgressionItem.TimeStop | DoubleJump;
		static readonly ProgressionItem ForwardJumpOfNpc = JumpOfNpc | ProgressionItem.ForwardDash;
		static readonly Gate DoubleJumpOfNpc = DoubleJump & ProgressionItem.TimeStop;

		public ItemLocation this[ItemKey key] => itemLocations[key];

		public static ItemLocationMap FromSeed(Seed seed)
		{
			return new ItemLocationRandomizer().RandonmiseItemLocations(seed, new ItemLocationMap());
		}

		ItemLocationMap()
		{
			AddPresentItemLocations();
			AddPastItemLocations();
			AddPyramidItemLocations();
		}

		void AddPresentItemLocations()
		{
			//tutorial
			itemLocations.Add(ItemKey.TutorialMeleeOrb);
			itemLocations.Add(ItemKey.TutorialSpellOrb);
			//starter lake desolation
			itemLocations.Add(new ItemKey(1, 1, 1528, 144));
			itemLocations.Add(new ItemKey(1, 15, 264, 144));
			itemLocations.Add(new ItemKey(1, 25, 296, 176));
			itemLocations.Add(new ItemKey(1, 9, 600, 144 + TimespinnerWheel.YOffset));
			itemLocations.Add(new ItemKey(1, 14, 40, 176), ProgressionItem.AntiWeed);
			//lower lake desolation
			var lowerLakeDesolationBridge = ForwardJumpOfNpc;
			itemLocations.Add(new ItemKey(1, 2, 1016, 384), JumpOfNpc);
			itemLocations.Add(new ItemKey(1, 11, 72, 240), lowerLakeDesolationBridge);
			itemLocations.Add(new ItemKey(1, 3, 56, 176), JumpOfNpc);
			//upper lake desolation
			var topLakeDesolation = ProgressionItem.AntiWeed;
			itemLocations.Add(new ItemKey(1, 17, 152, 96), topLakeDesolation);
			itemLocations.Add(new ItemKey(1, 21, 200, 144), topLakeDesolation);
			itemLocations.Add(new ItemKey(1, 20, 232, 96), topLakeDesolation & DoubleJump);
			itemLocations.Add(new ItemKey(1, 20, 168, 240), topLakeDesolation);
			itemLocations.Add(new ItemKey(1, 22, 344, 160), topLakeDesolation);
			//itemLocations.Add(new ItemKey(1, 18, 1272, 192), topLakeDesolation & ProgressionItem.GassMask); //TODO & KILL all 3 bosses
			//itemLocations.Add(new ItemKey(1, 18, 1368, 192), topLakeDesolation & ProgressionItem.GassMask); //TODO & KILL all 3 bosses
			//TODO shattered orb
			//kitty boss
			itemLocations.Add(new RoomItemKey(1, 5), topLakeDesolation | lowerLakeDesolationBridge);
			//libary left
			var leftLibrary = topLakeDesolation | lowerLakeDesolationBridge;
			itemLocations.Add(new ItemKey(2, 60, 328, 160), leftLibrary);
			itemLocations.Add(new ItemKey(2, 54, 296, 176), leftLibrary);
			itemLocations.Add(new ItemKey(2, 44, 680, 368), leftLibrary);
			itemLocations.Add(new ItemKey(2, 34, 232, 1200), leftLibrary);
			itemLocations.Add(new ItemKey(2, 47, 216, 208), leftLibrary & SecurityAccessD);
			itemLocations.Add(new ItemKey(2, 47, 152, 208), leftLibrary & SecurityAccessD);
			itemLocations.Add(new ItemKey(2, 47, 88, 208), leftLibrary & SecurityAccessD);
			//libary top
			var upperLeftLibrary = leftLibrary & (DoubleJump | ProgressionItem.ForwardDash);
			itemLocations.Add(new ItemKey(2, 56, 168, 192), upperLeftLibrary);
			itemLocations.Add(new ItemKey(2, 56, 392, 192), upperLeftLibrary);
			itemLocations.Add(new ItemKey(2, 56, 616, 192), upperLeftLibrary);
			itemLocations.Add(new ItemKey(2, 56, 840, 192), upperLeftLibrary);
			itemLocations.Add(new ItemKey(2, 56, 1064, 192), upperLeftLibrary);
			//libary mid
			var midLibrary = leftLibrary & SecurityAccessD;
			itemLocations.Add(new ItemKey(2, 32, 328, 160), midLibrary & SecurityAccessC);
			itemLocations.Add(new ItemKey(2, 7, 232, 144), midLibrary);
			itemLocations.Add(new ItemKey(2, 25, 328, 192), midLibrary & ProgressionItem.CardE);
			//libary right, 
			var rightSideLibrary = midLibrary & (SecurityAccessC | (SecurityAccessB & ProgressionItem.CardE));
			var rightSizeLibraryElevator = midLibrary & ProgressionItem.CardE & (SecurityAccessC | SecurityAccessB);
			itemLocations.Add(new ItemKey(2, 15, 760, 192), rightSideLibrary);
			itemLocations.Add(new ItemKey(2, 20, 72, 1200), rightSizeLibraryElevator);
			itemLocations.Add(new ItemKey(2, 23, 72, 560), rightSideLibrary);
			itemLocations.Add(new ItemKey(2, 23, 1112, 112), rightSideLibrary);
			itemLocations.Add(new ItemKey(2, 23, 136, 304), rightSideLibrary);
			itemLocations.Add(new ItemKey(2, 11, 104, 192), rightSideLibrary);
			itemLocations.Add(new ItemKey(2, 29, 280, 222 + TimespinnerSpindle.YOffset), rightSizeLibraryElevator);
			//Sealed Caves left
			var sealedCavesLeft = ProgressionItem.DoubleJump | SecurityAccessA;
			itemLocations.Add(new ItemKey(9, 19, 664, 704), sealedCavesLeft & JumpOfNpc);
			itemLocations.Add(new ItemKey(9, 39, 88, 192), sealedCavesLeft);
			itemLocations.Add(new ItemKey(9, 41, 312, 192),
				sealedCavesLeft & (UpwardDash | (ProgressionItem.ForwardDash & ProgressionItem.DoubleJump)));
			itemLocations.Add(new ItemKey(9, 42, 328, 192), sealedCavesLeft);
			itemLocations.Add(new ItemKey(9, 12, 280, 160), sealedCavesLeft);
			itemLocations.Add(new ItemKey(9, 48, 104, 160), sealedCavesLeft);
			itemLocations.Add(new ItemKey(9, 15, 248, 192), sealedCavesLeft);
			//itemLocations.Add(new ItemKey(9, 13, ???, ???), sealedCavesLeft); //TODO Timespinner Gear 3
			//Sealed Caves (sirens)
			var sealedCavesSirens = midLibrary & SecurityAccessB & ProgressionItem.CardE;
			itemLocations.Add(new ItemKey(9, 5, 88, 496), sealedCavesSirens & ProgressionItem.Swimming);
			itemLocations.Add(new ItemKey(9, 3, 1848, 576), sealedCavesSirens & ProgressionItem.Swimming);
			itemLocations.Add(new ItemKey(9, 3, 744, 560), sealedCavesSirens & ProgressionItem.Swimming);
			itemLocations.Add(new ItemKey(9, 2, 184, 176), sealedCavesSirens);
			itemLocations.Add(new ItemKey(9, 2, 104, 160), sealedCavesSirens);
			//Militairy Fortress
			var militairyFortress = rightSideLibrary; //TODO add lazer gates...
			itemLocations.Add(new ItemKey(10, 3, 264, 128), militairyFortress & DoubleJumpOfNpc);
			itemLocations.Add(new ItemKey(10, 11, 296, 192), militairyFortress);
			var militairyFortressHangar = militairyFortress & JumpOfNpc;
			itemLocations.Add(new ItemKey(10, 4, 1064, 176), militairyFortressHangar);
			itemLocations.Add(new ItemKey(10, 10, 104, 192), militairyFortressHangar);
			itemLocations.Add(new ItemKey(10, 8, 1080, 176), militairyFortressHangar);
			var rightSidemilitairyFortressHangar = militairyFortressHangar & DoubleJump;
			itemLocations.Add(new ItemKey(10, 7, 104, 192), rightSidemilitairyFortressHangar);
			itemLocations.Add(new ItemKey(10, 7, 152, 192), rightSidemilitairyFortressHangar);
			//itemLocations.Add(new ItemKey(10, 7, 152, 192), rightSidemilitairyFortressHangar & DoubleJumpOfNpc); TODO Gun Orb
			// The lab
			var theLab = militairyFortressHangar & SecurityAccessB;
			//TODO thelab
			//Emperors tower
			var emperorsTower = theLab;
			itemLocations.Add(new ItemKey(12, 5, 344, 192), emperorsTower);
			itemLocations.Add(new ItemKey(12, 3, 200, 160), emperorsTower & UpwardDash);
			itemLocations.Add(new ItemKey(12, 25, 360, 176), emperorsTower & UpwardDash);
			itemLocations.Add(new ItemKey(12, 22, 56, 192), emperorsTower);
			itemLocations.Add(new ItemKey(12, 9, 344, 928), emperorsTower);
			itemLocations.Add(new ItemKey(12, 19, 72, 192), emperorsTower & DoubleJumpOfNpc);
			itemLocations.Add(new ItemKey(12, 13, 120, 176), emperorsTower);
			//itemLocations.Add(new ItemKey(12, 11, 264, 208), emperorsTower); //TODO decide to use, Emperor completion chest...
		}

		void AddPastItemLocations()
		{
			var accessToPast = ProgressionItem.TimeStop & ProgressionItem.TimespinnerSpindle;
			//Refugee Camp
			itemLocations.Add(new ItemKey(3, 30, 296, 176), accessToPast);
			itemLocations.Add(new ItemKey(3, 30, 232, 176), accessToPast);
			itemLocations.Add(new ItemKey(3, 30, 168, 176), accessToPast);
			//Forest
			itemLocations.Add(new ItemKey(3, 3, 648, 272), accessToPast);
			itemLocations.Add(new ItemKey(3, 15, 248, 112), accessToPast & DoubleJumpOfNpc);
			itemLocations.Add(new ItemKey(3, 21, 120, 192), accessToPast);
			itemLocations.Add(new ItemKey(3, 12, 776, 560), accessToPast);
			itemLocations.Add(new ItemKey(3, 11, 392, 608), accessToPast & ProgressionItem.Swimming);
			itemLocations.Add(new ItemKey(3, 5, 184, 192), accessToPast & ProgressionItem.Swimming);
			itemLocations.Add(new ItemKey(3, 2, 584, 368), accessToPast);
			itemLocations.Add(new ItemKey(4, 20, 264, 160), accessToPast);
			var leftSideForestCaves = accessToPast & (JumpOfNpc | ProgressionItem.ForwardDash);
			itemLocations.Add(new ItemKey(3, 29, 248, 192), leftSideForestCaves);
			//Upper Lake Sirine
			var upperLakeSirine = leftSideForestCaves;
			itemLocations.Add(new ItemKey(7, 16, 152, 96), upperLakeSirine);
			itemLocations.Add(new ItemKey(7, 19, 248, 96), upperLakeSirine & ProgressionItem.DoubleJump);
			itemLocations.Add(new ItemKey(7, 19, 168, 240), upperLakeSirine);
			itemLocations.Add(new ItemKey(7, 27, 184, 144), upperLakeSirine);
			itemLocations.Add(new ItemKey(7, 13, 56, 176), upperLakeSirine);
			//Lower Lake Sirine
			var lowerlakeSirine = leftSideForestCaves & ProgressionItem.Swimming;
			itemLocations.Add(new ItemKey(7, 7, 1432, 576), lowerlakeSirine);
			itemLocations.Add(new ItemKey(7, 3, 440, 1232), lowerlakeSirine);
			itemLocations.Add(new ItemKey(7, 6, 520, 496), lowerlakeSirine);
			itemLocations.Add(new ItemKey(7, 11, 88, 240), lowerlakeSirine);
			itemLocations.Add(new ItemKey(7, 2, 1016, 384), lowerlakeSirine);
			//Caves of Banishment
			var lowerCavesOfBanishment = lowerlakeSirine;
			itemLocations.Add(new ItemKey(8, 19, 664, 704), lowerCavesOfBanishment & JumpOfNpc);
			itemLocations.Add(new ItemKey(8, 12, 280, 160), lowerCavesOfBanishment);
			itemLocations.Add(new ItemKey(8, 48, 104, 160), lowerCavesOfBanishment);
			itemLocations.Add(new ItemKey(8, 39, 88, 192), lowerCavesOfBanishment);
			itemLocations.Add(new ItemKey(8, 41, 168, 192), lowerCavesOfBanishment & (ProgressionItem.UpwardDash | ProgressionItem.ForwardDash & ProgressionItem.DoubleJump));
			itemLocations.Add(new ItemKey(8, 41, 216, 192), lowerCavesOfBanishment & (ProgressionItem.UpwardDash | ProgressionItem.ForwardDash & ProgressionItem.DoubleJump));
			itemLocations.Add(new ItemKey(8, 41, 264, 192), lowerCavesOfBanishment & (ProgressionItem.UpwardDash | ProgressionItem.ForwardDash & ProgressionItem.DoubleJump));
			itemLocations.Add(new ItemKey(8, 41, 312, 192), lowerCavesOfBanishment & (ProgressionItem.UpwardDash | ProgressionItem.ForwardDash & ProgressionItem.DoubleJump));
			itemLocations.Add(new ItemKey(8, 15, 248, 192), lowerCavesOfBanishment & ProgressionItem.DoubleJump);
			itemLocations.Add(new ItemKey(8, 31, 88, 400), lowerCavesOfBanishment & ProgressionItem.DoubleJump);
			//Caves of banishment (sirens)
			var upperCavesOfBanishment = accessToPast;
			itemLocations.Add(new ItemKey(8, 4, 664, 144), upperCavesOfBanishment);
			itemLocations.Add(new ItemKey(8, 3, 808, 144), upperCavesOfBanishment);
			itemLocations.Add(new ItemKey(8, 3, 744, 560), upperCavesOfBanishment & ProgressionItem.Swimming);
			itemLocations.Add(new ItemKey(8, 3, 1848, 576), upperCavesOfBanishment & ProgressionItem.Swimming);
			itemLocations.Add(new ItemKey(8, 5, 88, 496), upperCavesOfBanishment & ProgressionItem.Swimming);
			//Caste Ramparts
			var castleRamparts = leftSideForestCaves;
			itemLocations.Add(new ItemKey(4, 1, 456, 160), castleRamparts & (ProgressionItem.UpwardDash | ProgressionItem.TimeStop));
			itemLocations.Add(new ItemKey(4, 3, 136, 144), castleRamparts);
			itemLocations.Add(new ItemKey(4, 10, 56, 192), castleRamparts);
			itemLocations.Add(new ItemKey(4, 11, 344, 192), castleRamparts);
			//Caste Keep
			var castleKeep = castleRamparts;
			itemLocations.Add(new ItemKey(5, 10, 104, 192), castleKeep);
			itemLocations.Add(new ItemKey(5, 44, 216, 192), castleKeep);
			itemLocations.Add(new ItemKey(5, 45, 104, 192), castleKeep);
			itemLocations.Add(new ItemKey(5, 15, 296, 192), castleKeep);
			itemLocations.Add(new ItemKey(5, 41, 72, 160), castleKeep);
			//itemLocations.Add(new ItemKey(???), castleKeep & JumpOfNpc); Sucobus drop
			itemLocations.Add(new ItemKey(5, 22, 312, 176), castleKeep & (DoubleJump | ProgressionItem.ForwardDash));
			//Royal towers
			var royalTower = castleKeep & DoubleJump;
			itemLocations.Add(new ItemKey(6, 19, 200, 176), royalTower);
			var upperRoyalTower = royalTower & DoubleJumpOfNpc;
			itemLocations.Add(new ItemKey(6, 27, 472, 384), upperRoyalTower);
			itemLocations.Add(new ItemKey(6, 1, 1512, 288), upperRoyalTower);
			itemLocations.Add(new ItemKey(6, 25, 360, 176), upperRoyalTower);
			itemLocations.Add(new ItemKey(6, 3, 120, 208), upperRoyalTower);
			itemLocations.Add(new ItemKey(6, 17, 200, 132), upperRoyalTower);
			itemLocations.Add(new ItemKey(6, 17, 360, 1840), upperRoyalTower);
			itemLocations.Add(new ItemKey(6, 17, 56, 448), upperRoyalTower & UpwardDash);
			itemLocations.Add(new ItemKey(6, 13, 120, 176), upperRoyalTower);
			itemLocations.Add(new ItemKey(6, 22, 88, 208), upperRoyalTower);
			itemLocations.Add(new ItemKey(6, 11, 360, 544), upperRoyalTower);
			itemLocations.Add(new ItemKey(6, 23, 856, 208), upperRoyalTower);
		}

		void AddPyramidItemLocations()
		{
			var pyramid = ProgressionItem.DoubleJump; //TODO & access to world 3
			itemLocations.Add(new ItemKey(16, 14, 312, 192), pyramid);
			itemLocations.Add(new ItemKey(16, 3, 88, 192), pyramid);
			itemLocations.Add(new ItemKey(16, 22, 200, 192), pyramid);
			itemLocations.Add(new ItemKey(16, 16, 1512, 144), pyramid);
			
			//var challengeDungion = ProgressionItem.UpwardDash; //TODO & access to world 3
			//itemLocations.Add(new ItemKey(14, 14, 200, 832), challengeDungion); //transition chest 1
			//itemLocations.Add(new ItemKey(14, 17, 200, 832), challengeDungion); //transition chest 2
			//itemLocations.Add(new ItemKey(14, 20, 200, 832), challengeDungion); //transition chest 3
			//itemLocations.Add(new ItemKey(14, 8, 120, 176), challengeDungion); //Ravenlord pre fight
			//itemLocations.Add(new ItemKey(14, 9, 280, 176), challengeDungion); //Ravenlord post fight
			//itemLocations.Add(new ItemKey(14, 6, 40, 208), challengeDungion); //ifrid pre fight
			//itemLocations.Add(new ItemKey(14, 7, 280, 208), challengeDungion); //ifrid post fight
		}

		public ItemInfo GetItemInfo(ItemKey key)
		{
			return GetItemLocation(key)?.ItemInfo;
		}

		public Gate GetItemRequirements(ItemKey key)
		{
			return GetItemLocation(key)?.Gate;
		}

		ItemLocation GetItemLocation(ItemKey key)
		{
			return itemLocations.TryGetValue(key, out var itemLocation)
				? itemLocation
				: itemLocations.TryGetValue(key.ToRoomItemKey(), out var roomItemLocation)
					? roomItemLocation
					: null;
		}

		public IEnumerator<ItemLocation> GetEnumerator()
		{
			return itemLocations.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		class ItemLocations : LookupDictionairy<ItemKey, ItemLocation>
		{
			public ItemLocations(int capacity) : base(capacity, loc => loc.Key) { }

			public void Add(ItemKey itemKey)
			{
				Add(new ItemLocation(itemKey));
			}

			public void Add(ItemKey itemKey, ProgressionItem progressionItem)
			{
				Add(new ItemLocation(itemKey, progressionItem));
			}

			public void Add(ItemKey itemKey, Gate gate)
			{
				Add(new ItemLocation(itemKey, gate));
			}
		}
	}
}
