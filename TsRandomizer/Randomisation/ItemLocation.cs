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

		public string Name { get; internal set; }
		public string AreaName { get; internal set; }
		public bool IsPickedUp { get; internal set; }
		public ItemInfo ItemInfo { get; internal set; }
		public ItemInfo DefaultItem { get; internal set;  }

		public bool IsUsed => ItemInfo != null;
		
		public ItemLocation(ItemKey key, string areaName, string name, ItemInfo defaultItem) 
			: this(key, areaName, name, defaultItem, Requirement.None)
		{
		}

		public ItemLocation(ItemKey key, string areaName, string name, ItemInfo defaultItem, Requirement requiredRequirements)
			: this(key, areaName, name, defaultItem, (Gate)requiredRequirements)
		{
		}

		public ItemLocation(ItemKey key, string areaName, string name, ItemInfo defaultItem, Gate gate)
		{
			Key = key;
			AreaName = areaName;
			Name = name;
			Gate = gate;
			DefaultItem = defaultItem;
		}

		public void SetItem(ItemInfo item) => ItemInfo = item;

		public void SetPickedUp()
		{
			IsPickedUp = true;

			gameSave.DataKeyBools[LootedItemDataString] = true;

			if (ItemInfo is PogRessiveItemInfo progressiveItemInfo)
				progressiveItemInfo.Next();
		}

		public override string ToString() =>
			$"{AreaName} {Name ?? Key.ToString()} [{ItemInfo}]";

		public void BsseOnGameSave(GameSave save)
		{
			gameSave = save;

			IsPickedUp = gameSave.DataKeyBools.ContainsKey(LootedItemDataString);

			if(IsPickedUp && ItemInfo is PogRessiveItemInfo progressiveItemInfo)
				progressiveItemInfo.Next();
		}
	}
}