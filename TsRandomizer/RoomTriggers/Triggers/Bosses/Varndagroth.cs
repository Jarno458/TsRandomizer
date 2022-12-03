namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(2 ,29)]
	class Varndagroth : BossRoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			base.OnRoomLoad(roomState);

			if (roomState.Level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			if (!roomState.RoomItemLocation.IsPickedUp
					&& roomState.Level.GameSave.GetSaveBool("TSRando_IsBossDead_Varndagroth"))
				RoomTriggerHelper.SpawnItemDropPickup(roomState.Level, roomState.RoomItemLocation.ItemInfo, 280, 222);
		}
	}
}
