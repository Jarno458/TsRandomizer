using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocation
	{
		string LootedItemDataString => $"LEWTED{Key}";

		public readonly ItemKey Key;
		public readonly Gate Gate;

		readonly IGameSaveDataAccess gameSave;

		public bool IsPickedUp { get; private set; }

		public Requirement Unlocks { get; private set; }

		public ItemInfo ItemInfo { get; private set; }

		public bool IsUsed => ItemInfo != null;
		
		public ItemLocation(IGameSaveDataAccess gameSave, ItemKey key) 
			: this(gameSave, key, Requirement.None)
		{
		}

		public ItemLocation(IGameSaveDataAccess gameSave, ItemKey key, Requirement requiredRequirements)
			: this(gameSave, key, (Gate)requiredRequirements)
		{
		}

		public ItemLocation(IGameSaveDataAccess gameSave, ItemKey key, Gate gate)
		{
			Key = key;
			Gate = gate;

			this.gameSave = gameSave;

			if (gameSave.HasKey(LootedItemDataString))
				IsPickedUp = true;
		}

		public void SetItem(ItemInfo item, Requirement unlocks)
		{
			ItemInfo = item;
			Unlocks = unlocks;
		}

		public void SetPickedUp()
		{
			IsPickedUp = true;
			gameSave.SetKey(LootedItemDataString);
		}

		public override string ToString()
		{
			return Key.ToString();
		}
	}
}