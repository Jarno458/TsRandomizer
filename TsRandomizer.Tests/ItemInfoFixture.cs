using NUnit.Framework;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class ItemInfoFixture
	{
		[Test]
		public void should_equal()
		{
			var a = new SingleItemInfo(EInventoryRelicType.Dash);
			var b = new SingleItemInfo(EInventoryRelicType.Dash);

			Assert.That(a, Is.EqualTo(b));
		}

		[Test]
		public void should_equal_across_provider()
		{
			var providerA = new ItemInfoProvider();
			var a = providerA.Get(EInventoryRelicType.Dash);

			var providerB = new ItemInfoProvider();
			var b = providerB.Get(EInventoryRelicType.Dash);

			Assert.That(a, Is.EqualTo(b));
		}
	}
}
