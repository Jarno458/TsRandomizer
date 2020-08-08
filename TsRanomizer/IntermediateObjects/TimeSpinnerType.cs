using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Timespinner;

namespace TsRandomizer.IntermediateObjects
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TimeSpinnerType : Attribute
	{
		public static Dictionary<string, Type> TypeCache = new Dictionary<string, Type>(40);
		public static Assembly TimeSpinnerAssembly = typeof(TimespinnerGame).Assembly;

		public Type Type { get; }

		public TimeSpinnerType(string typeName)
		{
			Type = Get(typeName);
		}

		public static Type Get(string typeName)
		{
			if (TypeCache.TryGetValue(typeName, out Type knownType))
				return knownType;

			var type = TimeSpinnerAssembly
				.GetTypes()
				.Single(t => t.FullName == typeName);

			TypeCache.Add(typeName, type);
			return type;
		}
	}
}