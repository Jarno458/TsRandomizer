using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(11, 21)]
	class Genza : BossRoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			base.OnRoomLoad(roomState);

			if (roomState.Level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			if (!roomState.RoomItemLocation.IsPickedUp
					&& roomState.Level.GameSave.GetSaveBool("TSRando_IsBossDead_Shapeshift")
					&& roomState.Level.GameSave.HasRelic(EInventoryRelicType.ScienceKeycardA))
				RoomTriggerHelper.SpawnItemDropPickup(roomState.Level, roomState.RoomItemLocation.ItemInfo, 200, 200);

			if (!roomState.SeedOptions.Inverted && roomState.Level.GameSave.HasCutsceneBeenTriggered("Alt3_Teleport"))
				RoomTriggerHelper.CreateSimpleOneWayWarp(roomState.Level, 16, 12);
		}
	}
}
