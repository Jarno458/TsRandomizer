using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TsRandomizer.Screens.Settings.GameSettingObjects
{
	public class SpecificValuesGameSetting : GameSetting
	{
		public List<string> AllowedValues { get; private set; }

		public override void SetValue(object input)
		{
			try
			{
				string value = (string)input;
				if (AllowedValues.Contains(value))
				{
					base.SetValue(input);
				}
				else
				{
					base.SetValue(DefaultValue);
				}
			}
			catch
			{
				Console.WriteLine("SpecificValuesGameSetting: Can't set value. Input was not a string.");
			}

		}

		public SpecificValuesGameSetting(string name, string description, string defaultValue, List<string> allowedValues, bool canBeChangedInGame) : base(name, description, defaultValue, canBeChangedInGame)
		{
			AllowedValues = allowedValues;
			SetValue(defaultValue);
		}

		[JsonConstructor]
		public SpecificValuesGameSetting() { }

		public SpecificValuesGameSetting(SpecificValueSettingsConstants constants, SpecificValuesGameSetting settings) : base(constants, settings)
		{
			AllowedValues = constants.AllowedValues;
			SetValue(settings.CurrentValue);
		}
	}
}
