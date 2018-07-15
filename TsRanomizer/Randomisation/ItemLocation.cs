using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.LevelObjects;

namespace TsRanodmizer.Randomisation
{
    class ItemLocation
    {
        public readonly ItemKey Key;
        public readonly Gate Gate;

        public ItemInfo ItemInfo { get; private set; }
        public bool IsUsed { get; private set; }

        public ItemLocation(int levelId, int roomId, int x, int y)
        {
            Key = new ItemKey(levelId, roomId, x, y);
            Gate = new Gate(ProgressionItem.None);
        }

        public ItemLocation(int levelId, int roomId, int x, int y, Gate gate)
        {
            Key = new ItemKey(levelId, roomId, x, y);
            Gate = gate;
        }

        public ItemLocation(int levelId, int roomId, int x, int y, ProgressionItem requiredProgressionItems)
        {
            Key = new ItemKey(levelId, roomId, x, y);
            Gate = new Gate(requiredProgressionItems);
        }

        public void SetItem(ItemInfo item)
        {
            ItemInfo = item;
            IsUsed = true;
        }
    }
}