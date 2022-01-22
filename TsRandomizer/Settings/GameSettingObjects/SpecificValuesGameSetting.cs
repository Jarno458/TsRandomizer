using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class SpecificValuesGameSetting : GameSetting<string>
	{
		[JsonIgnoreDeserialize]
		public List<string> AllowedValues { get; }

		public override void SetValue(object input)
		{
			try
			{
				var value = input.ToString();
				if (AllowedValues.Contains(value))
					base.SetValue(input);
				else
					base.SetValue(DefaultValue);
			}
			catch
			{
				Console.WriteLine("SpecificValuesGameSetting: Can't set value. Input was not a string.");
			}

		}

		public SpecificValuesGameSetting(string category, string name, string description, List<string> allowedValues,
			string defaultValue = "Default", bool canBeChangedInGame = false) 
				: base(category, name, description, defaultValue, canBeChangedInGame)
		{
			AllowedValues = allowedValues;

			SetValue(defaultValue);
		}

		[JsonConstructor]
		public SpecificValuesGameSetting()
		{
		}
	}
}
