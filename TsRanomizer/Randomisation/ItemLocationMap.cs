using System.Collections;
using System.Collections.Generic;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.ReplacementObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocationMap : IEnumerable<ItemLocation>
	{
		readonly ItemLocations itemLocations = new ItemLocations(200);

		public ItemLocation this[ItemKey key] => itemLocations[key];

		public static ItemLocationMap FromSeed(Seed seed)
		{
			return new ItemLocationRandomizer().RandonmiseItemLocations(seed, new ItemLocationMap());
		}

		ItemLocationMap()
		{
			var upwardDash = ProgressionItem.UpwardDash | ProgressionItem.Lightwall;
			var doubleJump = ProgressionItem.DoubleJump | upwardDash;
			var jumpOfNpc = ProgressionItem.TimeStop | doubleJump;
			var doubleJumpOfNpc = (ProgressionItem.DoubleJump & ProgressionItem.TimeStop) | upwardDash;
			
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
			var lowerLakeDesolationBridge = doubleJump | ProgressionItem.ForwardDash | ProgressionItem.TimeStop;
			itemLocations.Add(new ItemKey(1, 2, 1016, 384), jumpOfNpc);
			itemLocations.Add(new ItemKey(1, 11, 72, 240), lowerLakeDesolationBridge);
			itemLocations.Add(new ItemKey(1, 3, 56, 176), jumpOfNpc);
			//upper lake desolation
			var topLakeDesolation = ProgressionItem.AntiWeed;
			itemLocations.Add(new ItemKey(1, 17, 152, 96), topLakeDesolation);
			itemLocations.Add(new ItemKey(1, 21, 200, 144), topLakeDesolation);
			itemLocations.Add(new ItemKey(1, 20, 232, 96), topLakeDesolation & doubleJump);
			itemLocations.Add(new ItemKey(1, 20, 168, 240), topLakeDesolation);
			itemLocations.Add(new ItemKey(1, 22, 344, 160), topLakeDesolation);
			//kitty boss
			itemLocations.Add(new RoomItemKey(1, 5), topLakeDesolation | lowerLakeDesolationBridge);
			//libary left
			var leftLibrary = topLakeDesolation | lowerLakeDesolationBridge;
			itemLocations.Add(new ItemKey(2, 60, 328, 160), leftLibrary);
			itemLocations.Add(new ItemKey(2, 54, 296, 176), leftLibrary);
			itemLocations.Add(new ItemKey(2, 44, 680, 368), leftLibrary);
			itemLocations.Add(new ItemKey(2, 34, 232, 1200), leftLibrary);
			itemLocations.Add(new ItemKey(2, 47, 216, 208), leftLibrary & ProgressionItem.CardD);
			itemLocations.Add(new ItemKey(2, 47, 152, 208), leftLibrary & ProgressionItem.CardD);
			itemLocations.Add(new ItemKey(2, 47, 88, 208), leftLibrary & ProgressionItem.CardD);
			//libary top
			var upperLeftLibrary = leftLibrary & (doubleJump | ProgressionItem.ForwardDash);
			itemLocations.Add(new ItemKey(2, 56, 168, 192), upperLeftLibrary);
			itemLocations.Add(new ItemKey(2, 56, 392, 192), upperLeftLibrary);
			itemLocations.Add(new ItemKey(2, 56, 616, 192), upperLeftLibrary);
			itemLocations.Add(new ItemKey(2, 56, 840, 192), upperLeftLibrary);
			itemLocations.Add(new ItemKey(2, 56, 1064, 192), upperLeftLibrary);
			//libary mid
			var midLibrary = leftLibrary & ProgressionItem.CardD;
			itemLocations.Add(new ItemKey(2, 32, 328, 160), midLibrary & ProgressionItem.CardC);
			itemLocations.Add(new ItemKey(2, 7, 232, 144), midLibrary);
			itemLocations.Add(new ItemKey(2, 25, 328, 192), midLibrary & ProgressionItem.CardE);
			//libary right, 
			var rightSideLibrary = midLibrary & (ProgressionItem.CardC | (ProgressionItem.CardB & ProgressionItem.CardE));
			var rightSizeLibraryElevator = midLibrary & ProgressionItem.CardE & (ProgressionItem.CardC | ProgressionItem.CardB);
			itemLocations.Add(new ItemKey(2, 15, 760, 192), rightSideLibrary);
			itemLocations.Add(new ItemKey(2, 20, 72, 1200), rightSizeLibraryElevator);
			itemLocations.Add(new ItemKey(2, 23, 72, 560), rightSideLibrary);
			itemLocations.Add(new ItemKey(2, 23, 1112, 112), rightSideLibrary);
			itemLocations.Add(new ItemKey(2, 23, 136, 304), rightSideLibrary);
			itemLocations.Add(new ItemKey(2, 11, 104, 192), rightSideLibrary);
			itemLocations.Add(new ItemKey(2, 29, 280, 222 + TimespinnerSpindle.YOffset), rightSizeLibraryElevator);
			//Sealed Caves left
			var sealedCavesLeft = doubleJump | ProgressionItem.CardA;
			itemLocations.Add(new ItemKey(9, 19, 664, 704), sealedCavesLeft & jumpOfNpc);
			itemLocations.Add(new ItemKey(9, 39, 88, 192), sealedCavesLeft);
			itemLocations.Add(new ItemKey(9, 41, 312, 192), sealedCavesLeft & (upwardDash | ProgressionItem.ForwardDash & ProgressionItem.DoubleJump));
			itemLocations.Add(new ItemKey(9, 42, 328, 192), sealedCavesLeft);
			itemLocations.Add(new ItemKey(9, 12, 280, 160), sealedCavesLeft);
			itemLocations.Add(new ItemKey(9, 48, 104, 160), sealedCavesLeft);
			itemLocations.Add(new ItemKey(9, 15, 248, 192), sealedCavesLeft);
			//itemLocations.Add(new ItemKey(9, 13, ???, ???), sealedCavesLeft); //Timespinner Gear 3

			//Sealed Caves (sirens)
			var sealedCavesSirens = midLibrary & ProgressionItem.CardB & ProgressionItem.CardE;
			itemLocations.Add(new ItemKey(9, 5, 88, 496), sealedCavesSirens & ProgressionItem.Swimming);
			itemLocations.Add(new ItemKey(9, 3, 1848, 576), sealedCavesSirens & ProgressionItem.Swimming);
			itemLocations.Add(new ItemKey(9, 3, 744, 560), sealedCavesSirens & ProgressionItem.Swimming);
			itemLocations.Add(new ItemKey(9, 2, 184, 176), sealedCavesSirens);
			itemLocations.Add(new ItemKey(9, 2, 104, 160), sealedCavesSirens);
			//Militairy Fortress
			var militairyFortress = rightSideLibrary; //TODO add lazer gates...
			itemLocations.Add(new ItemKey(10, 3, 264, 128), militairyFortress & doubleJumpOfNpc);
			itemLocations.Add(new ItemKey(10, 11, 296, 192), militairyFortress);
			var militairyFortressHangar = militairyFortress & jumpOfNpc;
			itemLocations.Add(new ItemKey(10, 4, 1064, 176), militairyFortressHangar);
			itemLocations.Add(new ItemKey(10, 10, 104, 192), militairyFortressHangar);
			itemLocations.Add(new ItemKey(10, 8, 1080, 176), militairyFortressHangar);
			var rightSidemilitairyFortressHangar = militairyFortressHangar & doubleJump;
			itemLocations.Add(new ItemKey(10, 7, 104, 192), rightSidemilitairyFortressHangar);
			itemLocations.Add(new ItemKey(10, 7, 152, 192), rightSidemilitairyFortressHangar);
			//itemLocations.Add(new ItemKey(10, 7, 152, 192), rightSidemilitairyFortressHangar & doubleJumpOfNpc); TODO Gun Orb
			// The lab
			var theLab = militairyFortressHangar & ProgressionItem.CardB;
			//TODO thelab
			//Emperors tower
			var emperorsTower = theLab;
			itemLocations.Add(new ItemKey(12, 5, 344, 192), emperorsTower);
			itemLocations.Add(new ItemKey(12, 3, 200, 160), emperorsTower & upwardDash);
			itemLocations.Add(new ItemKey(12, 25, 360, 176), emperorsTower & upwardDash);
			itemLocations.Add(new ItemKey(12, 22, 56, 192), emperorsTower);
			itemLocations.Add(new ItemKey(12, 9, 344, 928), emperorsTower);
			itemLocations.Add(new ItemKey(12, 19, 72, 192), emperorsTower & doubleJumpOfNpc);
			itemLocations.Add(new ItemKey(12, 13, 120, 176), emperorsTower);
			//itemLocations.Add(new ItemKey(12, 11, 264, 208), emperorsTower); //TODO decide to use, Emperor completion chest...


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
