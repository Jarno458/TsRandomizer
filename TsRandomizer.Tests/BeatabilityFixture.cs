using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TsRanodmizer.Randomisation;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class BeatabilityFixture
	{
		[Test]
		public void With_no_items_only_6_item_locatios_should_be_accessable()
		{
			var itemLocations = new ItemLocationMap();

			var accessableLocations = itemLocations.GetReachableLocations(Requirement.None).ToArray();

			Assert.That(Contains(accessableLocations, ItemKey.TutorialMeleeOrb));
			Assert.That(Contains(accessableLocations, ItemKey.TutorialSpellOrb));
			Assert.That(Contains(accessableLocations, new ItemKey(1, 1, 1528, 144)));
			Assert.That(Contains(accessableLocations, new ItemKey(1, 15, 264, 144)));
			Assert.That(Contains(accessableLocations, new ItemKey(1, 25, 296, 176)));
			Assert.That(Contains(accessableLocations, new ItemKey(1, 9, 600, 192)));
		}

		static bool Contains(IEnumerable<ItemLocation> itemLocations, ItemKey itemKey)
		{
			return itemLocations.Any(loc => loc.Key == itemKey);
		}
	}
}
