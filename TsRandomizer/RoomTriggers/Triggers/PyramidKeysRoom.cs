using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(7, 30)]
	class PyramidKeysRoom : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Level.GameSave.HasRelic(EInventoryRelicType.PyramidsKey)) 
				return;

			RoomTriggerHelper.SpawnTreasureChest(roomState.Level, false, 296, 176);
		}
	}
}
