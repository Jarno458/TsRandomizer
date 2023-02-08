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
		static readonly Type Type = TimeSpinnerType.Get("Timespinner.Core.Localization.Loc");
		static readonly Type ELanguageLocale = TimeSpinnerType.Get("Timespinner.Core.Localization.ELanguageLocale");
		static readonly MethodInfo GetMethod = Type.GetMethod("Get", BindingFlags.Static | BindingFlags.NonPublic);

		public string Get(string key) => (string)GetMethod.Invoke(null, new object[]{ key });

		public void OverrideKey(string key, string value, string speaker = null)
		{
			try
			{
				var stringLibrary = (StringLibrary) Type
					.GetField("_currentLibrary", BindingFlags.Static | BindingFlags.NonPublic)
					.GetValue(null);

				var stringInstances = (Dictionary<string, StringInstance>)stringLibrary.AsDynamic()._stringInstances;

				if (stringInstances.ContainsKey(key))
				{
					stringInstances[key].Text = value;

					if(speaker != null)
						stringInstances[key].Speaker = speaker;
				}
				else
				{
					stringInstances[key] = new StringInstance { Key = key, Text = value, Speaker = speaker };
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine("Error replacing text: " + ex.Message);
			}
		}

		public void ResetStrings()
		{
			var currentLocale = Type.GetField("_currentLocale", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			var currentLibrary = Type.GetField("_currentLibrary", BindingFlags.Static | BindingFlags.NonPublic);

			var constructor = 
				typeof(StringLibrary).GetConstructor(
					BindingFlags.NonPublic | BindingFlags.Instance, 
					null, 
					new [] { ELanguageLocale }, 
					null);

			var newLibrary = constructor.Invoke(new [] { currentLocale });

			currentLibrary.SetValue(null, newLibrary);
		}
	}
}
