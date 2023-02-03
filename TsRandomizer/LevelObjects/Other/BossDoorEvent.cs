using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Doors.BossDoorEvent")]
	// ReSharper disable once UnusedMember.Global
	class BossDoorEvent : LevelObject
	{
		public BossDoorEvent(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed)
		{
			if (Dynamic._isDemonDoor)
			{
				Dynamic._isDemonDoor = false;
				Dynamic.IsLocked = false;
			}
		}
	}
}
