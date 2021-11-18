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
		}
    }
}
