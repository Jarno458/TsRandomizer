using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;


namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(1, 5)]
	class RoboKitty : BossRoomTrigger
	{
		static readonly Type OrbPedestalEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Treasure.OrbPedestalEvent");
		public override void OnRoomLoad(RoomState roomState)
		{
			base.OnRoomLoad(roomState);

			if (roomState.Level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			var eventTypes = ((Dictionary<int, GameEvent>)roomState.Level.AsDynamic()._levelEvents).Values.Select(e => e.GetType());

			// Spawn item if not picked up and not spawned naturally
			if (!roomState.RoomItemLocation.IsPickedUp
					&& roomState.Level.GameSave.GetSaveBool("TSRando_IsBossDead_RoboKitty")
					// On first load, the event will not be in the events list, but will be in the new objects list
					&& !(eventTypes.Contains(OrbPedestalEventType) || roomState.Level.AsDynamic()._newObjects.Count > 0))
				RoomTriggerHelper.SpawnItemDropPickup(roomState.Level, roomState.RoomItemLocation.ItemInfo, 200, 208);
		}
	}
}
