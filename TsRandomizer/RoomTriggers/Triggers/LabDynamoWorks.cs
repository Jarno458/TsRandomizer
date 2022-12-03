using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 39)]
	class LabDynamoWorks : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.RoomItemLocation.IsPickedUp
			    || !roomState.Level.GameSave.HasOrb(EInventoryOrbType.Eye)
			    || !roomState.Level.GameSave.GetSaveBool("11_LabPower")) 
				return;

			RoomTriggerHelper.SpawnItemDropPickup(roomState.Level, roomState.RoomItemLocation.ItemInfo, 200, 176);
		}
	}
}
