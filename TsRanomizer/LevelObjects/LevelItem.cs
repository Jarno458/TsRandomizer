using TsRanodmizer.IntermediateObjects;
using TimespinnerItem = Timespinner.GameObjects.BaseClasses.Item;

namespace TsRanodmizer.LevelObjects
{
    class LevelItem : LevelObject<TimespinnerItem>
    {
        public LevelItem(TimespinnerItem item, ItemInfo itemInfo) : base(item, itemInfo)
        {
        
        }

        protected override void OnChangeRoom()
        {
            //TODO:
        }

        protected override void OnUpdate()
        {
            //TODO:
        }
    }
}
