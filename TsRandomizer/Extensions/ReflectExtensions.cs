using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace TsRandomizer.Extensions
{
	public static class ReflectExtensions
	{
		public static dynamic AsDynamic(this object instance) => new Dynamic(instance);
	}

	public class Dynamic : DynamicObject
	{
		const BindingFlags InstanceLevel = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;
		readonly object instance;

		public Dynamic(object instance)
		{
			this.instance = instance;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			Type type = instance.GetType();
			while (type != null)
			{
				if (TryGetProperty(type, binder.Name, out PropertyInfo propertyInfo))
				{
					result = propertyInfo.GetValue(instance, null);
					return true;
				}

				if (TryGetField(type, binder.Name, out FieldInfo fieldInfo))
				{
					result = fieldInfo.GetValue(instance);
					return true;
				}

				type = type.BaseType;
			}

			result = null;
			return false;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			Type type = instance.GetType();
			while (type != null)
			{
				if (TryGetProperty(type, binder.Name, out PropertyInfo propertyInfo))
				{
					propertyInfo.SetValue(instance, value, null);
					return true;
				}

				if (TryGetField(type, binder.Name, out FieldInfo fieldInfo))
				{
					fieldInfo.SetValue(instance, value);
					return true;
				}

				type = type.BaseType;
			}

			return false;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var argTypes = args
				 .Select(a => a.GetType())
				 .ToArray();

			Type type = instance.GetType();
			while (type != null)
			{
				if (TryGetMethod(type, binder.Name, argTypes, out MethodInfo methoidInfo))
				{
					result = methoidInfo.Invoke(instance, args);
					return true;
				}

				type = type.BaseType;
			}

			result = null;
			return false;
		}

		static bool TryGetProperty(Type type, string propertyName, out PropertyInfo propertyInfo)
		{
			try
			{
				propertyInfo = type.GetProperty(propertyName, InstanceLevel);
				return propertyInfo != null;
			}
			catch
			{
				propertyInfo = null;
				return false;
			}
		}

		static bool TryGetField(Type type, string fieldName, out FieldInfo fieldInfo)
		{
			try
			{
				fieldInfo = type.GetField(fieldName, InstanceLevel);
				return fieldInfo != null;
			}
			catch
			{
				fieldInfo = null;
				return false;
			}
		}

		static bool TryGetMethod(Type type, string methodName, Type[] argTypes, out MethodInfo methodInfo)
		{
			try
			{
				methodInfo = type.GetMethod(methodName, InstanceLevel);
				return methodInfo != null;
			}
			catch (AmbiguousMatchException)
			{
				try
				{
					methodInfo = type.GetMethod(methodName, InstanceLevel, null, argTypes, null);
					return methodInfo != null;
				}
				catch
				{
					methodInfo = null;
					return false;
				}
			}
			catch
			{
				methodInfo = null;
				return false;
			}
		}
	}
}
