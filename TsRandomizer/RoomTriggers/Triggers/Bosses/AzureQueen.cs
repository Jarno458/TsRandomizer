using TsRandomizer.Extensions;
using Timespinner.GameAbstractions.Gameplay;


namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(7, 0)]
	class AzureQueen : BossRoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			base.OnRoomLoad(roomState);

			var level = roomState.Level;
			// When Risky Warps is on and Boss Rando is off, Lunais should enter the boss to the right
			if (level.GameSave.GetSeed().Value.Options.RiskyWarps &&
				level.GameSave.GetSettings().BossRando.Value == "Off"
				&& !level.GameSave.GetSaveBool("TSRando_IsBossDead_Bird"))
				if (level.RoomEntrance == EDirection.West)
					level.RequestChangeLevel(new LevelChangeRequest
					{
						LevelID = 7,
						RoomID = 0,
						EnterDirection = EDirection.East
					});

		}
	}
}
