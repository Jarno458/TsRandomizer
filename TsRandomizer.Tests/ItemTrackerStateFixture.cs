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
				GetPickedUpItemLocation(new SingleItemInfo(EInventoryOrbType.Flame, EOrbSlot.Spell), Requirement.AntiWeed),
				GetPickedUpItemLocation(new SingleItemInfo(EInventoryRelicType.Dash), Requirement.ForwardDash),
				GetPickedUpItemLocation(new SingleItemInfo(EInventoryRelicType.TimespinnerWheel), Requirement.TimespinnerWheel),
				GetPickedUpItemLocation(new SingleItemInfo(EInventoryOrbType.Barrier, EOrbSlot.Spell), Requirement.UpwardDash)
			};

			var state = ItemTrackerState.FromItemLocationMap(itemlocations);

			Assert.IsTrue(state.FireSpell);
			Assert.IsTrue(state.Dash);
			Assert.IsTrue(state.Timestop);
			Assert.IsTrue(state.Lightwall);
			Assert.IsFalse(state.CelestialSash);
		}

		ItemLocation GetPickedUpItemLocation(ItemInfo item, Requirement requirement)
		{
			return new ItemLocation(null, null, null)
			{
				ItemInfo = item,
				Unlocks = requirement,
				IsPickedUp = true
			};
		}
	}
}
