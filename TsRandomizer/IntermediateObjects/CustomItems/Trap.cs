using TsRandomizer.Randomisation;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	abstract class Trap : CustomItem
	{
		public override int AnimationIndex => 208; // 'starry void' item

		public Trap(ItemUnlockingMap unlockingMap, CustomItemType itemType) : base(unlockingMap, itemType)
		{
		}
	}
}
