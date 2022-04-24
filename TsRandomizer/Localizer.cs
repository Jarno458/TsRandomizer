using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using Timespinner.Core.Localization;
using TsRandomizer.Extensions;
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

		public void OverrideKey(string key, string value)
		{
			try
			{
				var stringLibrary = (StringLibrary) Type
					.GetField("_currentLibrary", BindingFlags.Static | BindingFlags.NonPublic)
					.GetValue(null);

				var stringInstances = (Dictionary<string, StringInstance>) stringLibrary.AsDynamic()._stringInstances;
				if (!stringInstances.ContainsKey(key))
				{
					stringInstances.Add(key, new StringInstance() { Key = key, Text = value });
				}

				stringInstances[key].Text = value;
			}
			catch(Exception ex)
			{
				Console.WriteLine("Error replacing text: " + ex.Message);
			}
		}

		public void ResetStrings()
		{
			Type ELanguageLocale = 
				TimeSpinnerType.Get("Timespinner.Core.Localization.ELanguageLocale");
			var currentLocale = 
				Type.GetField("_currentLocale", BindingFlags.Static | BindingFlags.NonPublic)
					.GetValue(null);
			var currentLibrary = Type.GetField("_currentLibrary", BindingFlags.Static | BindingFlags.NonPublic);
			var constructor = 
				typeof(StringLibrary).GetConstructor(
					BindingFlags.NonPublic | BindingFlags.Instance, 
					null, 
					new Type[] { ELanguageLocale }, 
					null);
			var newLibrary = constructor.Invoke(new object[] { currentLocale });
			currentLibrary.SetValue(null, newLibrary);
		}
	}
}
