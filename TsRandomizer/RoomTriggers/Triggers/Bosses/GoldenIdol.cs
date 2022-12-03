using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(5, 5)]
	class GoldenIdol : BossRoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			base.OnRoomLoad(roomState);

			if (roomState.Level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			if (!roomState.RoomItemLocation.IsPickedUp
					&& roomState.Level.GameSave.GetSaveBool("TSRando_IsBossDead_Demon")
					&& roomState.Level.GameSave.HasRelic(EInventoryRelicType.DoubleJump))
				RoomTriggerHelper.SpawnItemDropPickup(roomState.Level, roomState.RoomItemLocation.ItemInfo, 200, 200);
		}
	}
}
