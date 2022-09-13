using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Doors.GyrePortalEvent")]
	// ReSharper disable once UnusedMember.Global
	class GyrePortalEvent : LevelObject
	{
		public GyrePortalEvent(Mobile typedObject) : base(typedObject)
		{
			Dynamic._isUsable = true;
			// Crash Site
			if (typedObject.Level.ID == 10 && typedObject.Level.RoomID == 0)
				Dynamic._portalType = 0; // start
			// Closed loop
			else if (typedObject.Level.ID == 14 && typedObject.Level.RoomID == 23)
				Dynamic._portalType = 3; // post-boss
			// Lab file cabinet room
			else if (typedObject.Level.ID == 11 && typedObject.Level.RoomID == 4)
			{
				Dynamic._portalType = 2; // post-dungeon
				LevelReflected.SetLevelSaveInt("GyreDungeonSeed", 0); // Warp to Ravenlord
			}
			// Backer room
			else if (typedObject.Level.ID == 2 && typedObject.Level.RoomID == 51)
			{
				Dynamic._portalType = 2; // post-dungeon
				LevelReflected.SetLevelSaveInt("GyreDungeonSeed", 1); // Warp to Ifrit
			}
			else
			{
				Dynamic._isUsable = false;
				Dynamic.SilentKill();
			}
		}
	}
}
