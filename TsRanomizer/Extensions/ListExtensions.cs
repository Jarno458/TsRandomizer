using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.Core.Specifications;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.Extensions
{
	static class ListExtensions
	{
		internal static MinimapRoom GetRoom(this List<MinimapArea> areas, RoomItemKey roomKey)
		{
			return areas[roomKey.LevelId].Rooms.Single(r => r.RoomID == roomKey.RoomId); //Room Array Index isnt equal to RoomId
		}
	}
}
