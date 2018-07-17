using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
    [TimeSpinnerType("Timespinner.GameObjects.Events.Treasure.OrbPedestalEvent")]
    class OrbPedestal : LevelObject<Mobile>
    {
        public OrbPedestal(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
        {
        }

        protected override void OnChangeRoom()
        {
	        if (ItemInfo == null || ItemInfo == ItemInfo.Dummy)
		        return;

			//TODO: change sprite

			((ObjectTileSpecification)ObjectPrivate._objectSpec).Argument = 3;
            ObjectPrivate._orbType = EInventoryOrbType.Flame;
        }

        protected override void OnUpdate()
        {
            //TODO: incase of not an melee orb??
        }
    }
}
