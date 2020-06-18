
using System.Linq;
using NUnit.Framework;
using TsRanodmizer.Randomisation;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class BeatabilityFixture
	{
		[Test]
		public void With_no_items_only_4_and_2_tutorial_item_locatios_should_be_accessable()
		{
			var itemLocations = new ItemLocationMap();

			var accessableLocations = itemLocations.GetReachableLocations(Requirement.None);

			Assert.That(accessableLocations.Count(), Is.EqualTo(6));
		}
	}
}
