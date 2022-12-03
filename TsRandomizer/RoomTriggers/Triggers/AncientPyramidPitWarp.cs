using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(16, 21)]
	class AncientPyramidPitWarp : RoomTrigger
	{
		static readonly Type GlowingFloorEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.EnvironmentPrefabs.L11_Lab.EnvPrefabLabVilete");

		public override void OnRoomLoad(RoomState roomState)
		{
			// Spawn glowing floor event to give a soft-lock exit warp
			if (((Dictionary<int, NPCBase>)roomState.Level.AsDynamic()._npcs).Values.Any(npc => npc.GetType() == GlowingFloorEventType)) 
				return;

			var position = new Point(100, 195);

			RoomTriggerHelper.SpawnGlowingFloor(roomState.Level, position);
		}
	}
}
