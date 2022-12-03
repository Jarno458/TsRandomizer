using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(13, 1)]
	class VolTerrilis : BossRoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Level.GameSave.GetSettings().BossRando.Value)
				RoomTriggerHelper.CreateAndCallCutScene(roomState, "Alt1_Vol");

			base.OnRoomLoad(roomState);
		}
	}
}
