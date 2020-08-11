using Timespinner.GameAbstractions.Saving;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation
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

		public ItemInfo DefaultItem { get; internal set;  }

		public bool IsUsed => ItemInfo != null;
		
		public ItemLocation(ItemKey key, ItemInfo defaultItem) 
			: this(key, defaultItem, Requirement.None)
		{
		}

		public ItemLocation(ItemKey key, ItemInfo defaultItem, Requirement requiredRequirements)
			: this(key, defaultItem, (Gate)requiredRequirements)
		{
		}

		public ItemLocation(ItemKey key, ItemInfo defaultItem, Gate gate)
		{
			Key = key;
			Gate = gate;
			DefaultItem = defaultItem;
		}

		public void SetItem(ItemInfo item, Requirement unlocks)
		{
			ItemInfo = item;
			Unlocks = unlocks;
		}

		public void SetPickedUp()
		{
#if DEBUG
			if (Key == ItemKey.DebugRoom) return;
#endif

			IsPickedUp = true;
			gameSave.DataKeyBools[LootedItemDataString] = true;
		}

		public override string ToString()
		{
			return $"{Key} [{ItemInfo ?? null}]";
		}

		public void BsseOnGameSave(GameSave save)
		{
			gameSave = save;

			IsPickedUp = gameSave.DataKeyBools.ContainsKey(LootedItemDataString);
		}
	}
}