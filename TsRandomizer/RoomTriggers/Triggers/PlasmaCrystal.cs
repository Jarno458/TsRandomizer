using System.Collections.Generic;
using System.Linq;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(8, 21)]
	class PlasmaCrystal : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			var levelReflected = roomState.Level.AsDynamic();
			IEnumerable<Animate> eventObjects = levelReflected._levelEvents.Values;

			if (!roomState.RoomItemLocation.IsPickedUp && !eventObjects.Any(o => o.GetType()
				    .ToString() == "Timespinner.GameObjects.Events.EnvironmentPrefabs.EnvPrefabCavesRadiationCrystal"))
				RoomTriggerHelper.SpawnItemDropPickup(roomState.Level, roomState.RoomItemLocation.ItemInfo, 312, 912);
		}
	}
}
