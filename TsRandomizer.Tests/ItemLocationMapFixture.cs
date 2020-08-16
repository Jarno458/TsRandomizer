using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class ItemLocationMapFixture
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

		[Test]
		public void With_doubejump_timestop_spindle_and_cardD_should_get_access_to_past()
		{
			var itemLocations = new ItemLocationMap();

			var accessableLocations = itemLocations.GetReachableLocations(
					Requirement.DoubleJump | Requirement.GateAccessToPast | Requirement.Swimming)
				.ToArray();

			Assert.That(Contains(accessableLocations, new ItemKey(3, 3, 648, 272)));
		}

		[Test]
		public void With_shoud_mark_captians_chests_as_available()
		{
			var itemLocations = new ItemLocationMap();

			var accessableLocations = itemLocations.GetReachableLocations(
					Requirement.GassMask | Requirement.AntiWeed | Requirement.Swimming | Requirement.GateLakeSirineRight | Requirement.UpwardDash)
				.ToArray();

			Assert.That(Contains(accessableLocations, new ItemKey(1, 18, 1320, 189))); 
			Assert.That(Contains(accessableLocations, new ItemKey(1, 18, 1272, 192)));
			Assert.That(Contains(accessableLocations, new ItemKey(1, 18, 1368, 192)));
		}

		static bool Contains(IEnumerable<ItemLocation> itemLocations, ItemKey itemKey)
		{
			return itemLocations.Any(loc => loc.Key == itemKey);
		}
	}
}
