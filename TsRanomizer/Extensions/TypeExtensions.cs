using System;
using System.Reflection;

namespace TsRanodmizer.Extensions
{
	static class TypeExtensions
	{
		internal static MethodInfo GetPrivateMethod(this Type type, string methodName)
		{
			return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		}

		internal static MethodInfo GetPrivateMethod(this Type type, string methodName, params Type[] argTypes)
		{
			return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, argTypes, null);
		}

		internal static MethodInfo GetPrivateStaticMethod(this Type type, string methodName, params Type[] argTypes)
		{
			return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static, null, argTypes, null);
		}
	}
}
