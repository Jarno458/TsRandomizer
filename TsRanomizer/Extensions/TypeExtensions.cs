using System;
using System.Reflection;

namespace TsRanodmizer.Extensions
{
	static class TypeExtensions
	{
		public static MethodBase GetPrivateMethod(this Type type, string methodName)
		{
			return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public static MethodBase GetPrivateMethod(this Type type, string methodName, params Type[] argTypes)
		{
			return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, argTypes, null);
		}
	}
}
