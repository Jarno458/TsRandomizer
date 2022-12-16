using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;

namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(7, 5)]
	class Cantoran : BossRoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			SpawnBoss(roomState.Level, roomState.Seed.Options, TargetBossId);

			if (roomState.Level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			// Set Cantoran quest active when fighting Pink Bird
			if (!roomState.Level.GameSave.GetSaveBool("TSRando_IsPinkBirdDead"))
			{
				roomState.Level.GameSave.SetValue("TSRando_IsPinkBirdDead", true);

				BestiaryManager.RefreshBossSaveFlags(roomState.Level);

				return;
			}

			if (!roomState.Seed.Options.Cantoran)
				return;

			CreateBossWarp(roomState.Level, (int)EBossID.Cantoran);

			if (!roomState.RoomItemLocation.IsPickedUp
					&& roomState.Level.GameSave.GetSaveBool("TSRando_IsBossDead_Cantoran")
					&& roomState.Level.AsDynamic()._newObjects.Count == 0) // Orb Pedestal event
				RoomTriggerHelper.SpawnItemDropPickup(roomState.Level, roomState.RoomItemLocation.ItemInfo, 170, 194);
		}
	}
}
