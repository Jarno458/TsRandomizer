using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TsRandomizer.Screens.Settings.GameSettingObjects
{
	public class SpecificValuesGameSetting : GameSetting
	{
		public string[] AllowedValues { get; set; }

		public override void SetValue(object input)
		{
			try
			{
				string value = (string)input;
				if (Array.Find(AllowedValues, v => v == value) != "")
				{
					base.SetValue(input);
				}
			}
			catch
			{
				Console.WriteLine("SpecificValuesGameSetting: Can't set value. Input was not a string.");
			}

		}

		public SpecificValuesGameSetting(string name, string description, string defaultValue, string[] allowedValues, bool canBeChangedInGame) : base(name, description, defaultValue, canBeChangedInGame)
		{
			AllowedValues = allowedValues;
			SetValue(defaultValue);
		}

		public SpecificValuesGameSetting() { }
	}
}
