using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Archipelago
{
	class ArchipelagoRemoteItem : SingleItemInfo
	{
		public ArchipelagoRemoteItem() : base(new ItemIdentifier(EInventoryUseItemType.MagicMarbles))
		{
		}

		public override void OnPickup(Level level)
		{
		}
	}
}
