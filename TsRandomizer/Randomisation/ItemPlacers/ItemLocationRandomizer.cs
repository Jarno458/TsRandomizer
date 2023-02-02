using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	abstract class ItemLocationRandomizer
	{
		protected readonly Seed Seed;
		protected readonly SeedOptions SeedOptions;
		protected readonly ItemInfoProvider ItemInfoProvider;
		protected readonly ItemUnlockingMap UnlockingMap;

		protected ItemLocationRandomizer(
			Seed seed, ItemInfoProvider itemInfoProvider, ItemUnlockingMap unlockingMap)
		{
			Seed = seed;
			SeedOptions = seed.Options;
			ItemInfoProvider = itemInfoProvider;
			UnlockingMap = unlockingMap;
		}

		public abstract ItemLocationMap GenerateItemLocationMap(bool isProgressionOnly);

		protected abstract void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation);
	}
}
