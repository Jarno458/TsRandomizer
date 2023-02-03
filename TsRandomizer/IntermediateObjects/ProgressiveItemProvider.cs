using System.Collections.Generic;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Randomisation;

namespace TsRandomizer.IntermediateObjects
{
	class ProgressiveItemProvider : ItemInfoProvider
	{
		readonly Dictionary<ItemInfo, ProgressiveItemInfo> progressiveItems;

		public ProgressiveItemProvider(SeedOptions options, ItemUnlockingMap unlockingMap) : base(options, unlockingMap)
		{
			progressiveItems = new Dictionary<ItemInfo, ProgressiveItemInfo>();

			MakeGearsProgressive();
			MakeBroochProgressive();

			if (options.ProgressiveKeycard)
				MakeKeycardsProgressive();

			if (options.ProgressiveVerticalMovement)
				MakeVerticalMovementProgressive();
		}

		void MakeGearsProgressive()
		{
			var gear1 = base.Get(EInventoryRelicType.TimespinnerGear1);
			var gear2 = base.Get(EInventoryRelicType.TimespinnerGear2);
			var gear3 = base.Get(EInventoryRelicType.TimespinnerGear3);

			var progressiveItem = new ProgressiveItemInfo(gear1, gear2, gear3);

			progressiveItems.Add(gear1, progressiveItem);
			progressiveItems.Add(gear2, progressiveItem);
			progressiveItems.Add(gear3, progressiveItem);
		}

		void MakeBroochProgressive()
		{
			var empireBrooch = base.Get(EInventoryRelicType.EmpireBrooch);
			var godestBrooch = base.Get(EInventoryRelicType.EternalBrooch);

			var progressiveItem = new ProgressiveItemInfo(empireBrooch, godestBrooch);

			progressiveItems.Add(empireBrooch, progressiveItem);
			progressiveItems.Add(godestBrooch, progressiveItem);
		}

		void MakeKeycardsProgressive()
		{
			var cardA = base.Get(EInventoryRelicType.ScienceKeycardA);
			var cardB = base.Get(EInventoryRelicType.ScienceKeycardB);
			var cardC = base.Get(EInventoryRelicType.ScienceKeycardC);
			var cardD = base.Get(EInventoryRelicType.ScienceKeycardD);

			var progressiveItem = new ProgressiveItemInfo(cardD, cardC, cardB, cardA);

			progressiveItems.Add(cardA, progressiveItem);
			progressiveItems.Add(cardB, progressiveItem);
			progressiveItems.Add(cardC, progressiveItem);
			progressiveItems.Add(cardD, progressiveItem);
		}

		void MakeVerticalMovementProgressive()
		{
			var doubleJump = base.Get(EInventoryRelicType.DoubleJump);
			var lightwall = base.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell);
			var celestialSash = base.Get(EInventoryRelicType.EssenceOfSpace);

			var progressiveItem = new ProgressiveItemInfo(doubleJump, lightwall, celestialSash);

			progressiveItems.Add(doubleJump, progressiveItem);
			progressiveItems.Add(lightwall, progressiveItem);
			progressiveItems.Add(celestialSash, progressiveItem);
		}

		public override ItemInfo Get(EInventoryOrbType orbType, EOrbSlot orbSlot) 
			=> HandleProgression(base.Get(orbType, orbSlot));

		public override ItemInfo Get(EInventoryRelicType relicItem) => 
			HandleProgression(base.Get(relicItem));

		public ItemInfo HandleProgression(ItemInfo item) =>
			progressiveItems.TryGetValue(item, out var progressiveItem)
				? progressiveItem
				: item;
	}
}