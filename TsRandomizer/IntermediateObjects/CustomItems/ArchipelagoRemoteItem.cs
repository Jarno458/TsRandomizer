using TsRandomizer.Randomisation;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class ArchipelagoRemoteItem : CustomItem
	{
		public override string Name => "Archipelago Item";
		public override int AnimationIndex => 198;

		public ArchipelagoRemoteItem(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.Archipelago)
		{
			SetDescription("Item that belongs to a distant timeline somewhere in the Archipelago (cannot be sold)", "Archipelago");
		}
	}
}
