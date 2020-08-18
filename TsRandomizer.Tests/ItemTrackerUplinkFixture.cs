using NUnit.Framework;
using TsRandomizer.ItemTracker;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class ItemTrackerUplinkFixture
	{
		[Test]
		public void Should_save_and_read_item_tracker_state()
		{
			var originalState = new ItemTrackerState
			{
				CardA = true,
				Dash = true,
				FireRing = true,
				DoubleJump = true,
				Timestop = false,
				CardB = false,
				PyramidKeys = false,
				DinsFire = false
			};

			ItemTrackerUplink.UpdateState(originalState);

			var retreivedState = ItemTrackerUplink.LoadState();

			Assert.IsTrue(retreivedState.CardA);
			Assert.IsTrue(retreivedState.Dash);
			Assert.IsTrue(retreivedState.FireRing);
			Assert.IsTrue(retreivedState.DoubleJump);
			Assert.IsFalse(retreivedState.Timestop);
			Assert.IsFalse(retreivedState.CardB);
			Assert.IsFalse(retreivedState.PyramidKeys);
			Assert.IsFalse(retreivedState.DinsFire);
		}
	}
}
