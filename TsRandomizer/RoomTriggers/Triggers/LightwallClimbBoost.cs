using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 2)] // Post-dynamo room
	[RoomTriggerTrigger(16, 6)] // Pyramid Shaft
	class LightwallClimbBoost : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			// Mark the room as boosted aura to help with the climb
			roomState.Level.AsDynamic().IsOnVilete = true;
			roomState.Level.MainHero.AsDynamic().IsOnVilete = true;
		}

	}
}
