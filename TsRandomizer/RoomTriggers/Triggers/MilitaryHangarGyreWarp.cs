using Microsoft.Xna.Framework;
using TsRandomizer.Extensions;

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
			else if (roomState.Seed.Options.RiskyWarps && !roomState.Level.IsRoomVisited(10, 12))
				// Spawns only if lasers are still up, coming from right
				// Soft-lock exit back to lab
				RoomTriggerHelper.SpawnGlowingFloor(roomState.Level, new Point(360, 120));
		}
	}
}
