using System;
using System.Linq;
using TsRanodmizer.OverloadedObjects;

namespace TsRanodmizer.IntermediateObjects
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class TargetTimeSpinnerObjectTypeAttribute : Attribute
	{
		public Type ObjectType { get; }

		public TargetTimeSpinnerObjectTypeAttribute(Type objectType)
		{
			ObjectType = objectType;
		}

		public TargetTimeSpinnerObjectTypeAttribute(string typeName)
		{
			ObjectType = TimeSpinnerGame.TimeSpinnerAssembly
				.GetTypes()
				.Single(t => t.Name == typeName);
		}
	}
}