using NUnit.Framework;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.ItemTracker;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class ItemTrackerStateFixture
	{
		[Test]
		public void Should_load_state_from_itemLocationMap()
		{
			var itemlocations = new []
			{
				GetPickedUpItemLocation(new SingleItemInfo(new ItemUnlockingMap(Seed.Zero), new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Spell))),
				GetPickedUpItemLocation(new SingleItemInfo(new ItemUnlockingMap(Seed.Zero), new ItemIdentifier(EInventoryRelicType.Dash))),
				GetPickedUpItemLocation(new SingleItemInfo(new ItemUnlockingMap(Seed.Zero), new ItemIdentifier(EInventoryRelicType.TimespinnerWheel))),
				GetPickedUpItemLocation(new SingleItemInfo(new ItemUnlockingMap(Seed.Zero), new ItemIdentifier(EInventoryOrbType.Barrier, EOrbSlot.Spell)))
			};

			var state = ItemTrackerState.FromItemLocationMap(itemlocations);

			Assert.IsTrue(state.FireSpell);
			Assert.IsTrue(state.Dash);
			Assert.IsTrue(state.Timestop);
			Assert.IsTrue(state.Lightwall);
			Assert.IsFalse(state.CelestialSash);
		}

		ItemLocation GetPickedUpItemLocation(ItemInfo item)
		{
			return new ItemLocation(null, null, null)
			{
				ItemInfo = item,
				IsPickedUp = true
			};
		}
	}
}
