using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocation
	{
		public readonly ItemKey Key;
		public readonly Gate Gate;

		public ItemInfo ItemInfo { get; private set; }
		public bool IsUsed { get; private set; }


		public ItemLocation(int levelId, int roomId, int x, int y) : this(new ItemKey(levelId, roomId, x, y))
		{
		}

		public ItemLocation(int levelId, int roomId, int x, int y, Gate gate) : this(new ItemKey(levelId, roomId, x, y), gate)
		{
		}

		public ItemLocation(int levelId, int roomId, int x, int y, ProgressionItem requiredProgressionItems)
			: this(new ItemKey(levelId, roomId, x, y), new Gate(requiredProgressionItems))
		{
		}

		public ItemLocation(ItemKey key, ProgressionItem requiredProgressionItems) 
			: this(key, new Gate(requiredProgressionItems))
		{
		}

		public ItemLocation(ItemKey key) : this(key, new Gate(ProgressionItem.None))
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