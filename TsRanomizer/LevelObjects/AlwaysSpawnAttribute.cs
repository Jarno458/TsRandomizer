using System;
using System.Reflection;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;

namespace TsRanodmizer.LevelObjects
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	class AlwaysSpawnAttribute : Attribute
	{
		public readonly EEventTileType EventType;
		public readonly int Argument;

		public Type ObjectType;
		public Type TimeSpinnerObjectType;

		public AlwaysSpawnAttribute(EEventTileType eventType, int argument = 0)
		{
			EventType = eventType;
			Argument = argument;
		}
	}

	interface ICustomSpwanMethod
	{
		GameEvent Spawn(Level level, ObjectTileSpecification specification);
	}
}