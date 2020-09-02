using NUnit.Framework;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Randomisation.ItemPlacers;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class RandomFillingRandomizerFixture
	{
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		public void Should_fill_tuturial_with_melee_and_spellorb(int seedIndex)
		{
			var seed = new Seed(seedIndex);
			var unlockingMap = new ItemUnlockingMap(seed);
			var itemProvider = new ItemInfoProvider(unlockingMap);
			var itemLocations = new ItemLocationMap(itemProvider);

			FullRandomItemLocationRandomizer.AddRandomItemsToLocationMap(seed, itemProvider, unlockingMap, itemLocations, true);

			Assert.That(itemLocations[ItemKey.TutorialMeleeOrb].ItemInfo.Identifier.LootType, Is.EqualTo(LootType.Orb));
			Assert.That(itemLocations[ItemKey.TutorialMeleeOrb].ItemInfo.Identifier.OrbSlot, Is.EqualTo(EOrbSlot.Melee));

			Assert.That(itemLocations[ItemKey.TutorialSpellOrb].ItemInfo.Identifier.LootType, Is.EqualTo(LootType.Orb));
			Assert.That(itemLocations[ItemKey.TutorialSpellOrb].ItemInfo.Identifier.OrbSlot, Is.EqualTo(EOrbSlot.Spell));
		}
	}
}
