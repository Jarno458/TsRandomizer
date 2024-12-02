using Timespinner.GameAbstractions.Gameplay;
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

			var level = roomState.Level;
			// When Risky Warps is on and Boss Rando is off, Lunais should enter the boss to the right
			if (level.GameSave.GetSeed().Value.Options.RiskyWarps &&
				level.GameSave.GetSettings().BossRando.Value == "Off"
				&& !level.GameSave.GetSaveBool("TSRando_IsBossDead_Shapeshift"))
				if (level.RoomEntrance == EDirection.West)
					level.RequestChangeLevel(new LevelChangeRequest
					{
						LevelID = 11,
						RoomID = 21,
						EnterDirection = EDirection.East
					});

			if (level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			if (!roomState.RoomItemLocation.IsPickedUp
					&& level.GameSave.GetSaveBool("TSRando_IsBossDead_Shapeshift")
					&& level.GameSave.HasRelic(EInventoryRelicType.ScienceKeycardA))
				RoomTriggerHelper.SpawnItemDropPickup(level, roomState.RoomItemLocation.ItemInfo, 200, 200);

			if (!roomState.Seed.Options.Inverted 
					&& !roomState.Seed.Options.PyramidStart 
					&& level.GameSave.HasCutsceneBeenTriggered("Alt3_Teleport"))
				RoomTriggerHelper.CreateSimpleOneWayWarp(roomState.Level, 16, 12);
		}
	}
}
