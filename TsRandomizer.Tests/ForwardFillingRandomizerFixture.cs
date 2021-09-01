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
			var seed = new Seed(1U, SeedOptions.None);
			var unlockingMap = new ItemUnlockingMap(seed);
			var itemProvder = new ItemInfoProvider(SeedOptions.None, unlockingMap);

			var randimizer = new ForwardFillingItemLocationRandomizer(seed, itemProvder, unlockingMap);

			var itemLocations = randimizer.GenerateItemLocationMap(true);

			Assert.That(itemLocations.IsBeatable(), Is.True);
		}

		[TestCase(1U)]
		[TestCase(2U)]
		[TestCase(3U)]
		[TestCase(4U)]
		[TestCase(5U)]
		public void Should_fill_tuturial_with_melee_and_spellorb(uint seedIndex)
		{
			var seed = new Seed(seedIndex, SeedOptions.None);
			var unlockingMap = new ItemUnlockingMap(seed);
			var itemProvder = new ItemInfoProvider(SeedOptions.None, unlockingMap);

			var randimizer = new ForwardFillingItemLocationRandomizer(seed, itemProvder, unlockingMap);

			var itemLocations = randimizer.GenerateItemLocationMap(true);

			Assert.That(itemLocations[ItemKey.TutorialMeleeOrb].ItemInfo.Identifier.LootType, Is.EqualTo(LootType.Orb));
			Assert.That(itemLocations[ItemKey.TutorialMeleeOrb].ItemInfo.Identifier.OrbSlot, Is.EqualTo(EOrbSlot.Melee));

			Assert.That(itemLocations[ItemKey.TutorialSpellOrb].ItemInfo.Identifier.LootType, Is.EqualTo(LootType.Orb));
			Assert.That(itemLocations[ItemKey.TutorialSpellOrb].ItemInfo.Identifier.OrbSlot, Is.EqualTo(EOrbSlot.Spell));
		}
	}
}
