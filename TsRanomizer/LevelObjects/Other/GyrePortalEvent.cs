using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Doors.GyrePortalEvent")]
	// ReSharper disable once UnusedMember.Global
	class GyrePortalEvent : LevelObject
	{
		public GyrePortalEvent(Mobile typedObject) : base(typedObject)
		{
			Object._isUsable = false;
		}
	}
}
