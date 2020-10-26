using NUnit.Framework;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class ItemInfoFixture
	{
		[Test]
		public void Should_equal_on_identifier()
		{
			var a = new ItemIdentifier(EInventoryRelicType.Dash);
			var b = new ItemIdentifier(EInventoryRelicType.Dash);

			Assert.That(a, Is.EqualTo(b));
		}

		[Test]
		public void Should_equal()
		{
			var a = new SingleItemInfo(new ItemUnlockingMap(Seed.Zero), new ItemIdentifier(EInventoryRelicType.Dash));
			var b = new SingleItemInfo(new ItemUnlockingMap(Seed.Zero), new ItemIdentifier(EInventoryRelicType.Dash));

			Assert.That(a, Is.EqualTo(b));
		}

		[Test]
		public void Should_equal_across_provider()
		{
			var providerA = new ItemInfoProvider(SeedOptions.None, new ItemUnlockingMap(Seed.Zero));
			var a = providerA.Get(EInventoryRelicType.Dash);

			var providerB = new ItemInfoProvider(SeedOptions.None, new ItemUnlockingMap(Seed.Zero));
			var b = providerB.Get(EInventoryRelicType.Dash);

			Assert.That(a, Is.EqualTo(b));
		}
	}
}
