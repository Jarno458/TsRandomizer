using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocation
	{
		public readonly ItemKey Key;
		public readonly Gate Gate;

		public ItemInfo ItemInfo { get; private set; }
		public bool IsUsed { get; private set; }

		public ItemLocation(ItemKey key) : this(key, Requirement.None)
		{
		}

		public ItemLocation(ItemKey key, Requirement requiredRequirements)
			: this(key, (Gate)requiredRequirements)
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

		public override string ToString()
		{
			return Key.ToString();
		}
	}
}