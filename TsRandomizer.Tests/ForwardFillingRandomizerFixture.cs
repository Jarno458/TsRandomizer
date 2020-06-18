using NUnit.Framework;
using TsRanodmizer;
using TsRanodmizer.Randomisation;
using TsRanodmizer.Randomisation.ItemPlacers;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class ForwardFillingRandomizerFixture
	{
		[Test]
		public void Should_generate_beatable_seed_in_1_pass()
		{
			var itemLocations = new ItemLocationMap();

			var seed = new Seed(1);
			var unlockingMap = new ItemUnlockingMap(seed);

			ForwardFillingItemLocationRandomizer.AddRandomItemsToLocationMap(seed, unlockingMap, itemLocations);

			Assert.That(itemLocations.IsBeatable(), Is.True);
		}
	}
}
