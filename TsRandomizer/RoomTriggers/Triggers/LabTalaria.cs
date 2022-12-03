using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 1)]
	class LabTalaria : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.RoomItemLocation.IsPickedUp || !roomState.Level.GameSave.HasRelic(EInventoryRelicType.Dash)) 
				return;

			RoomTriggerHelper.SpawnItemDropPickup(roomState.Level, roomState.RoomItemLocation.ItemInfo, 280, 191);
		}
	}
}
