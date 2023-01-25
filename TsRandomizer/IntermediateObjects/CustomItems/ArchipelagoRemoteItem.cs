namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class ArchipelagoRemoteItem : CustomItem
	{
		public override string Name => "Archipelago Item";
		public override int AnimationIndex => 32;

		public ArchipelagoRemoteItem() : base(CustomItemType.Archipelago)
		{
			SetDescription("Item that belongs to a distant timeline somewhere in the Archipelago (cannot be sold)", "Archipelago");
		}

		/*public override void OnPickup(Level level)
		{
			//yes send item checks?
		}*/
	}
}
