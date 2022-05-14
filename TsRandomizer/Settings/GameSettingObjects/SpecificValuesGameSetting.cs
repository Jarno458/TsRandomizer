using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class SpecificValuesGameSetting : GameSetting<string>
	{
		[JsonIgnoreDeserialize]
		public List<string> AllowedValues { get; }

		public SpecificValuesGameSetting(string name, string description, List<string> allowedValues,
			string defaultValue = "Default", bool canBeChangedInGame = false)
				: base(name, description, defaultValue, canBeChangedInGame)
		{
			AllowedValues = allowedValues;
		}

		[JsonConstructor]
		public SpecificValuesGameSetting()
		{
		}

		public override void ToggleValue()
		{
			try
			{
				var currentIndex = AllowedValues.IndexOf(Value);
				var newIndex = currentIndex + 1 >= AllowedValues.Count ? 0 : currentIndex + 1;

				Value = AllowedValues[newIndex];
			}
			catch
			{
				Value = (string)DefaultValue;
			}

		}
	}
}
