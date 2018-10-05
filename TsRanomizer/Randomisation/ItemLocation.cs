using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocation
	{
		public readonly ItemKey Key;
		public readonly Gate Gate;

		public ItemInfo ItemInfo { get; private set; }
		public bool IsUsed { get; private set; }

		public ItemLocation(ItemKey key) : this(key, ProgressionItem.None)
		{
		}

		public ItemLocation(ItemKey key, ProgressionItem requiredProgressionItems)
			: this(key, (Gate)requiredProgressionItems)
		{
		}

		public ItemLocation(ItemKey key, Gate gate)
		{
			Key = key;
			Gate = gate;
		}

		public void SetItem(ItemInfo item)
		{
			ItemInfo = item;
			IsUsed = true;
		}
	}
}