namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class ArchipelagoRemoteItem : CustomItemBase
	{
		public ArchipelagoRemoteItem() : 
			base(CustomItem.Archipelago, "Archipelago Item", 
				"Item that belongs to a distant timeline somewhere in the Archipelago (cannot be sold)",
				32)
		{
		}

		/*public override void OnPickup(Level level)
		{
			//yes send item checks?
		}*/
	}
}
