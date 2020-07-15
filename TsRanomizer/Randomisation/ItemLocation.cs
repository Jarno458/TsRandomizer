using Timespinner.GameAbstractions.Saving;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocation
	{
		string LootedItemDataString => $"LEWTED{Key}";

		public readonly ItemKey Key;
		public readonly Gate Gate;

		GameSave gameSave;

		public bool IsPickedUp { get; private set; }

		public Requirement Unlocks { get; private set; }

		public ItemInfo ItemInfo { get; private set; }

		public bool IsUsed => ItemInfo != null;
		
		public ItemLocation(ItemKey key) 
			: this(key, Requirement.None)
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

		public void SetItem(ItemInfo item, Requirement unlocks)
		{
			ItemInfo = item;
			Unlocks = unlocks;
		}

		public void SetPickedUp()
		{
#if DEBUG
			if (Key == ItemKey.DebugRoom)return;
#endif

			IsPickedUp = true;
			gameSave.DataKeyBools[LootedItemDataString] = true;
		}

		public override string ToString()
		{
			return Key.ToString();
		}

		public void BsseOnGameSave(GameSave save)
		{
			gameSave = save;

			IsPickedUp = gameSave.DataKeyBools.ContainsKey(LootedItemDataString);
		}
	}
}