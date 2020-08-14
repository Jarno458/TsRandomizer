using System;
using System.Dynamic;
using System.Reflection;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer
{
	class Constants : DynamicObject
	{
		static readonly Type Type = TimeSpinnerType
			.Get("Timespinner.Core.Constants.Constants");

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			try
			{
				result = Type
					 .GetField(binder.Name, BindingFlags.Static | BindingFlags.NonPublic)
					 .GetValue(null);

				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			try
			{
				result = Type
					 .GetMethod(binder.Name, BindingFlags.Static | BindingFlags.NonPublic)
					 .Invoke(null, args);

				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}
	}
}
