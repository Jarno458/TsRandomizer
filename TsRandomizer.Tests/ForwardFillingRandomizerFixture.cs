using NUnit.Framework;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Randomisation.ItemPlacers;

namespace TsRandomizer.Tests
{
	[TestFixture]
	[Ignore("Its broken AF and lost its value")]
	class ForwardFillingRandomizerFixture
	{
		[Test]
		public void Should_generate_beatable_seed_in_1_pass()
		{
			var itemProvder = new ItemInfoProvider();
			var itemLocations = new ItemLocationMap(itemProvder);

			var seed = new Seed(1);
			var unlockingMap = new ItemUnlockingMap(itemProvder, seed);

			ForwardFillingItemLocationRandomizer.AddRandomItemsToLocationMap(seed, itemProvder, unlockingMap, itemLocations, true);

			Assert.That(itemLocations.IsBeatable(), Is.True);
		}

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		public void Should_fill_tuturial_with_melee_and_spellorb(int seedIndex)
		{
			var itemProvder = new ItemInfoProvider();
			var itemLocations = new ItemLocationMap(itemProvder);

			var seed = new Seed(seedIndex);
			var unlockingMap = new ItemUnlockingMap(itemProvder, seed);

			ForwardFillingItemLocationRandomizer.AddRandomItemsToLocationMap(seed, itemProvder, unlockingMap, itemLocations, true);

			Assert.That(itemLocations[ItemKey.TutorialMeleeOrb].ItemInfo.LootType, Is.EqualTo(LootType.Orb));
			Assert.That(itemLocations[ItemKey.TutorialMeleeOrb].ItemInfo.OrbSlot, Is.EqualTo(EOrbSlot.Melee));

			Assert.That(itemLocations[ItemKey.TutorialSpellOrb].ItemInfo.LootType, Is.EqualTo(LootType.Orb));
			Assert.That(itemLocations[ItemKey.TutorialSpellOrb].ItemInfo.OrbSlot, Is.EqualTo(EOrbSlot.Spell));
		}
	}
}
