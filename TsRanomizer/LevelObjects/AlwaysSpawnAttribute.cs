using System;
using Timespinner.Core.Specifications;

namespace TsRanodmizer.LevelObjects
{
	[AttributeUsage(AttributeTargets.Class)]
	class AlwaysSpawnAttribute : Attribute
	{
		public readonly EEventTileType EventType;

		public AlwaysSpawnAttribute(EEventTileType eventType)
		{
			EventType = eventType;
		}
	}
}