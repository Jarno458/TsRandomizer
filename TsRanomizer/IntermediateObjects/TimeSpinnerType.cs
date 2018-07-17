using System;
using System.Linq;
using System.Reflection;
using Timespinner;

namespace TsRanodmizer.IntermediateObjects
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class TimeSpinnerType : Attribute
	{
		public static Assembly TimeSpinnerAssembly = typeof(TimespinnerGame).Assembly;

		public Type Type { get; }

		public TimeSpinnerType(Type type)
		{
			Type = type;
		}

		public TimeSpinnerType(string typeName)
		{
			Type = Get(typeName);
		}

		public static Type Get(string typeName)
		{
			return TimeSpinnerAssembly
				.GetTypes()
				.Single(t => t.FullName == typeName);
		}
	}
}