using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsRandomizer.Screens.Settings.GameSettingObjects
{
	public class StringGameSetting : GameSetting
	{
		string _currentValue;
		public int MaxLength { get; }
		public string CurrentValue {
			get {
				return _currentValue == null || _currentValue.Length == 0 ? DefaultValue : _currentValue;
			}
			set {
				_currentValue = value;
			}
		}

		public StringGameSetting(string name, string description, string defaultValue, int maxLength) : base(name, description, defaultValue)
		{
			MaxLength = maxLength;
			_currentValue = defaultValue;
		}
	}
}
