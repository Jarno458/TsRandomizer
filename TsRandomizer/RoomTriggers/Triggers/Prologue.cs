namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(0, 0)]
	class Prologue : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Seed.Options.PyramidStart) 
				return;

			RoomTriggerHelper.CreateAndCallCutScene(roomState, "Alt2_Win");

			roomState.Level.MainHero.IsBlocked = true;
		}
	}
}