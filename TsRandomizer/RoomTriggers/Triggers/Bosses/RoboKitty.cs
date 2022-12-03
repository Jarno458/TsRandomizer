using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(1, 5)]
	class RonboKitty : BossRoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			base.OnRoomLoad(roomState);

			if (roomState.Level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			if (!roomState.RoomItemLocation.IsPickedUp
					&& roomState.Level.GameSave.GetSaveBool("TSRando_IsBossDead_RoboKitty")
					&& roomState.Level.AsDynamic()._newObjects.Count == 0) // Orb Pedestal event
				RoomTriggerHelper.SpawnItemDropPickup(roomState.Level, roomState.RoomItemLocation.ItemInfo, 200, 208);
		}
	}
}
