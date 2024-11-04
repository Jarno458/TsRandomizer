using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 39)]
	class LabDynamoWorks : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			// Power stays on in Lock Key Amadeus
			// 11_LabPower true = power off
			if (roomState.Level.GameSave.GetSeed().Value.Options.LockKeyAmadeus)
				roomState.Level.GameSave.SetValue("11_LabPower", false);

			if (roomState.RoomItemLocation.IsPickedUp
			    || !roomState.Level.GameSave.HasOrb(EInventoryOrbType.Eye)
			    || !roomState.Level.GameSave.GetSaveBool("11_LabPower")) 
				return;

			RoomTriggerHelper.SpawnItemDropPickup(roomState.Level, roomState.RoomItemLocation.ItemInfo, 200, 176);
		}
	}
}
