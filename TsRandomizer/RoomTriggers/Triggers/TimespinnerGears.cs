using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 26)]
	class TimespinnerGear1 : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.RoomItemLocation.IsPickedUp
			    || !roomState.Level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear1))
				return;

			RoomTriggerHelper.SpawnTreasureChest(roomState.Level, true, 136, 192);
		}
	}

	[RoomTriggerTrigger(2, 52)]
	class TimespinnerGear2 : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.RoomItemLocation.IsPickedUp
			    || !roomState.Level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear2))
				return;

			RoomTriggerHelper.SpawnTreasureChest(roomState.Level, true, 104, 192);
		}
	}

	[RoomTriggerTrigger(9, 13)]
	class TimespinnerGear3 : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.RoomItemLocation.IsPickedUp
			    || !roomState.Level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear3)) 
				return;

			RoomTriggerHelper.SpawnTreasureChest(roomState.Level, false, 296, 176);
		}
	}
}
