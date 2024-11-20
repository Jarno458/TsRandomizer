using Microsoft.Xna.Framework;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(10, 0)]
	class MilitaryHangarGyreWarp : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Level.GameSave.GetSaveBool("IsPastCleared"))
			{
				if (roomState.Seed.Options.GyreArchives)
					// Military Hangar crash site to Gyre
					RoomTriggerHelper.SpawnGyreWarp(roomState.Level, 340, 180);
			}
			else if (roomState.Seed.Options.RiskyWarps)
				RoomTriggerHelper.SpawnGlowingFloor(roomState.Level, new Point(360, 120));
			
		}
	}
}
