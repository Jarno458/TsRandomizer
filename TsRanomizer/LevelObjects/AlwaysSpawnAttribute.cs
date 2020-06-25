using System;
using System.Reflection;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.LevelObjects
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	class AlwaysSpawnAttribute : Attribute
	{
		public readonly EEventTileType EventType;
		public readonly int Argument;
		public readonly bool IgnoreArgument;

		public Type ObjectType;
		public Type TimeSpinnerObjectType;

		public AlwaysSpawnAttribute(EEventTileType eventType, int argument = 0, bool ignoreArgument = false)
		{
			EventType = eventType;
			Argument = argument;
			IgnoreArgument = ignoreArgument;
		}
	}

	interface ICustomSpwanMethod
	{
		GameEvent Spawn(Level level, ObjectTileSpecification specification);
	}
}