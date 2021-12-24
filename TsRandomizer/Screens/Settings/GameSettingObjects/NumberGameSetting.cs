using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TsRandomizer.Screens.Settings.GameSettingObjects
{
	public class NumberGameSetting : GameSetting
	{
		public double MinimumValue { get; }
		public double MaximumValue { get; }
		[JsonIgnore()]
		public double StepValue { get; set; }
		[JsonIgnore()]
		public bool AllowDecimals { get; set; }
		public override void SetValue(dynamic input)
		{
			if (!double.IsNaN(input))
			{
				double value = input;
				if (AllowDecimals) base.SetValue(value); else base.SetValue(Math.Round(value));
			}
		}

		public NumberGameSetting(string name, string description, double defaultValue, double minValue, double maxValue, double stepValue, bool allowDecimals, bool canBeChangedInGame) : base(name, description, defaultValue, canBeChangedInGame)
		{
			MinimumValue = minValue;
			MaximumValue = maxValue;
			StepValue = stepValue;
			AllowDecimals = allowDecimals;
			SetValue(defaultValue);
		}

		public NumberGameSetting() { }
	}
}
