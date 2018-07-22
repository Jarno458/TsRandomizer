using System.Collections;
using System.Collections.Generic;
using Timespinner.GameAbstractions.Saving;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.ReplacementObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocationMap : IEnumerable<ItemLocation>
	{
		readonly LookupDictionairy<ItemKey, ItemLocation> itemLocations;

		public ItemLocation this[ItemKey key] => itemLocations[key];

		public static ItemLocationMap FromSaveFile(GameSave saveFile)
		{
			var seed = saveFile.FindSeed() ?? Seed.Current;
			saveFile.SetSeed(seed);
			return new ItemLocationRandomizer().RandonmiseItemLocations(seed, new ItemLocationMap());
		}

		ItemLocationMap()
		{
			var longJump = new Gate(ProgressionItem.DoubleJump | ProgressionItem.UpwardDash | ProgressionItem.Lightwall | ProgressionItem.ForwardDash);
			var highJump = new Gate(ProgressionItem.DoubleJump | ProgressionItem.UpwardDash | ProgressionItem.Lightwall);
			var highJumpOfNpc = new Gate(ProgressionItem.DoubleJump | ProgressionItem.UpwardDash | ProgressionItem.Lightwall | ProgressionItem.TimeStop);
			var vineWall = new Gate(ProgressionItem.AntiWeed);
			var lowerLakeDesolationBridge = longJump | ProgressionItem.TimeStop;
			var leftLibrary = new Gate(ProgressionItem.KittyBoss);
			var upperLeftLibrary = leftLibrary & longJump;
			var midLibrary = leftLibrary & ProgressionItem.CardD;
			var rightSideLibrary = midLibrary & (ProgressionItem.CardC | (ProgressionItem.CardB & ProgressionItem.CardE));
			var rightSizeLibraryElevator = midLibrary & ProgressionItem.CardE & (ProgressionItem.CardC | ProgressionItem.CardB);

			itemLocations = new LookupDictionairy<ItemKey, ItemLocation>(loc => loc.Key)
			{
				//tutorial
				new ItemLocation(ItemKey.TutorialMeleeOrb),
				new ItemLocation(ItemKey.TutorialSpellOrb),
				//starter
				new ItemLocation(1,1,1528,144),
				new ItemLocation(1,15,264,144),
				new ItemLocation(1,25,296,176),
				new ItemLocation(1,9,600,144 + TimeSpinnerWheeel.YOffset),
				new ItemLocation(1,14,40,176, vineWall),
				//lower lake desolation
				new ItemLocation(1,2,1016,384, highJumpOfNpc),
				new ItemLocation(1,11,72,240, lowerLakeDesolationBridge),
				new ItemLocation(1,3,56,176, highJumpOfNpc),
				//upper lake desolation
				new ItemLocation(1,17,152,96, vineWall),
				new ItemLocation(1,21,200,144, vineWall),
				new ItemLocation(1,20,232,96, vineWall & highJump),
				new ItemLocation(1,20,168,240, vineWall),
				new ItemLocation(1,22,344,160, vineWall),
				//libary left
				new ItemLocation(2,60,328,160, leftLibrary),
				new ItemLocation(2,54,296,176, leftLibrary),
				new ItemLocation(2,44,680,368, leftLibrary),
				new ItemLocation(2,34,232,1200, leftLibrary),
				new ItemLocation(2,47,216,208, leftLibrary & ProgressionItem.CardD),
				new ItemLocation(2,47,152,208, leftLibrary & ProgressionItem.CardD),
				new ItemLocation(2,47,88,208, leftLibrary & ProgressionItem.CardD),
				//libary top
				new ItemLocation(2,56,168,192, upperLeftLibrary),
				new ItemLocation(2,56,392,192, upperLeftLibrary),
				new ItemLocation(2,56,616,192, upperLeftLibrary),
				new ItemLocation(2,56,840,192, upperLeftLibrary),
				new ItemLocation(2,56,1064,192, upperLeftLibrary),
				//libary mid
				new ItemLocation(2,32,328,160, midLibrary & ProgressionItem.CardC),
				new ItemLocation(2,7,232,144, midLibrary),
				new ItemLocation(2,25,328,192, midLibrary & ProgressionItem.CardE),
				//libary right, 
				new ItemLocation(2,15,760,192, rightSideLibrary),
				new ItemLocation(2,20,72,1200, rightSizeLibraryElevator),
				new ItemLocation(2,23,72,560, rightSideLibrary),
				new ItemLocation(2,23,1112,112, rightSideLibrary),
				new ItemLocation(2,23,136,304, rightSideLibrary),
				new ItemLocation(2,11,104,192, rightSideLibrary),
			};
		}

		public ItemInfo GetItemInfo(ItemKey key)
		{
			return itemLocations.TryGetValue(key, out var itemInfo)
				? itemInfo.ItemInfo
				: null;
		}

		public Gate GetItemRequirements(ItemKey key)
		{
			return itemLocations.TryGetValue(key, out var itemInfo)
				? itemInfo.Gate
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
	}
}
