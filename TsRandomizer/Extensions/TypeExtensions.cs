using System;
using System.Reflection;

namespace TsRandomizer.Extensions
{
	static class TypeExtensions
	{
		internal static MethodInfo GetPrivateMethod(this Type type, string methodName)
		{
			return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		}

		internal static MethodInfo GetPublicStaticMethod(this Type type, string methodName)
		{
			return type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
		}

		internal static MethodInfo GetPrivateMethod(this Type type, string methodName, params Type[] argTypes)
		{
			return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, argTypes, null);
		}

		internal static MethodInfo GetPrivateStaticMethod(this Type type, string methodName, params Type[] argTypes)
		{
			return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static, null, argTypes, null);
		}

		internal static object GetPrivateStaticField(this Type type, string fieldName)
		{
			return type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic)
				?.GetValue(null);
		}

		internal static void SetPrivateStaticField(this Type type, string fieldName, object value)
		{
			type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic)
				?.SetValue(null, value);
		}

		internal static object GetEnumValue(this Type type, string enumMemberName)
		{
			var underlyingType = Enum.GetUnderlyingType(type);
			var members = Enum.GetNames(type);
			var values = Enum.GetValues(type);

			for (var i = 0; i < members.Length; i++)
			{
				if (members[i] != enumMemberName) continue;

				return Convert.ChangeType(values.GetValue(i), underlyingType);
			}

			throw new MissingMemberException(type.FullName, enumMemberName);
		}

		internal static object CreateInstance(this Type type, bool nonPublic = false, params object[] args)
		{
			var bindingFlags = nonPublic
				? BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance
				: BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;

			return Activator.CreateInstance(type, bindingFlags, null, args, null);
		}
	}
}
