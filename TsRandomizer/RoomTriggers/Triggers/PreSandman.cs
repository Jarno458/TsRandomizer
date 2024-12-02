using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(16 , 5)]
	class PreSandman : RoomTrigger
	{
		static readonly Type BossDoorEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.BossDoorEvent");
		static readonly Type TeleportEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.TeleportEvent");

		public override void OnRoomLoad(RoomState roomState)
		{
			// Clear final boss saves
			roomState.Level.GameSave.SetValue("TSRando_IsBossDead_Sandman", false);
			roomState.Level.GameSave.SetValue("TSRando_IsBossDead_Nightmare", false);

			if (!roomState.Seed.Options.EnterSandman && !roomState.Seed.Options.PyramidStart)
				return;

			var bossDoor = ((Dictionary<int, GameEvent>)roomState.Level.AsDynamic()._levelEvents).Values
				.FirstOrDefault(obj => obj.GetType() == BossDoorEventType);
			var bossTeleportEvent = ((Dictionary<int, GameEvent>)roomState.Level.AsDynamic()._levelEvents).Values
				.Where(obj => obj.GetType() == TeleportEventType)
				.Single(te => te.AsDynamic().Direction == EDirection.West);

			if (!(roomState.Level.GameSave.HasRelic(EInventoryRelicType.TimespinnerWheel)
			      && roomState.Level.GameSave.HasRelic(EInventoryRelicType.TimespinnerSpindle)
			      && roomState.Level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear1)
			      && roomState.Level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear2)
			      && roomState.Level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear3)))
			{
				bossDoor.AsDynamic()._isLocked = true;
				bossDoor.AsDynamic()._isDemonLocked = true;

				bossTeleportEvent.SilentKill();
			}
		}
	}
}
