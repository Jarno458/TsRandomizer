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

		public ItemInfo ItemInfo { get; private set; }

		public bool IsUsed => ItemInfo != null;
		
		public ItemLocation(GameSave gameSave, ItemKey key) : this(gameSave, key, Requirement.None)
		{
		}

		public ItemLocation(GameSave gameSave, ItemKey key, Requirement requiredRequirements)
			: this(gameSave, key, (Gate)requiredRequirements)
		{
		}

		public ItemLocation(GameSave gameSave, ItemKey key, Gate gate)
		{
			Key = key;
			Gate = gate;

			this.gameSave = gameSave;

			if (gameSave.DataKeyBools.ContainsKey(LootedItemDataString))
				IsPickedUp = true;
		}

		public void SetItem(ItemInfo item)
		{
			ItemInfo = item;
		}

		public void SetPickedUp()
		{
			IsPickedUp = true;
			gameSave.DataKeyBools[LootedItemDataString] = true;
		}

		public override string ToString()
		{
			return Key.ToString();
		}
	}
}