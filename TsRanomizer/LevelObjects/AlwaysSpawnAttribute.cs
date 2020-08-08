using System;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;

namespace TsRandomizer.LevelObjects
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
		Mobile Spawn(Level level, ObjectTileSpecification specification);
	}
}