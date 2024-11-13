using System;
using System.Linq;
using System.Collections.Generic;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(7, 30)]
	class PyramidKeysRoom : RoomTrigger
	{
		static readonly Type TimeGateEventType = TimeSpinnerType.Get("Timespinner.GameAbstractions.GameObjects.TimeGateEvent");
		public override void OnRoomLoad(RoomState roomState)
		{
			// If entering the past via this warp room, this prevents a duplicate item spawn.
			var timeGateEvent = ((Dictionary<int, GameEvent>)roomState.Level.AsDynamic()._levelEvents).Values
				.Where(obj => obj.GetType() ==  TimeGateEventType);
			if (timeGateEvent.Count() == 0 && !roomState.RoomItemLocation.IsPickedUp)
				RoomTriggerHelper.SpawnTreasureChest(roomState.Level, false, 296, 176);
		}
	}
}
