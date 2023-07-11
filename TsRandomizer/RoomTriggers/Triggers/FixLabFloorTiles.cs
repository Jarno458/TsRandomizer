using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 3)]
	class FixLabFloorTiles : RoomTrigger
	{
		public override void OnRoomLoad(RoomState state)
		{
			var foregroundTiles = (Dictionary<Point, List<Tile>>)state.Level.AsDynamic()._foregroundTiles;
			for (var x = 19; x <= 81; x++)
				for (var y = 38; y <= 39; y++)
					foregroundTiles.Remove(new Point(x, y));
		}
	}
}
