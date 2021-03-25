using System;
using System.Dynamic;
using System.Reflection;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer
{
	class Localizer : DynamicObject
	{
		static readonly Type Type = TimeSpinnerType
			.Get("Timespinner.Core.Localization.Loc");

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
