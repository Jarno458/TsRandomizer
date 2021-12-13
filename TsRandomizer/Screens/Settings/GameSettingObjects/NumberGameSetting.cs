using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsRandomizer.Screens.Settings.GameSettingObjects
{
	class NumberGameSetting : GameSetting
	{
		double _currentValue;
		public double MinimumValue { get; }
		public double MaximumValue { get; }
		public bool AllowDecimals { get; }
		public double CurrentValue { 
			get {
				return AllowDecimals ? _currentValue : Math.Round(_currentValue);
			} 
			set {
				_currentValue = AllowDecimals ? value : Math.Round(value);
			}
		}

		public NumberGameSetting(string name, string description, double defaultValue, double minValue, double maxValue, bool allowDecimals) : base(name, description, defaultValue)
		{
			MinimumValue = minValue;
			MaximumValue = maxValue;
			AllowDecimals = allowDecimals;
			CurrentValue = defaultValue;
		}
	}
}
