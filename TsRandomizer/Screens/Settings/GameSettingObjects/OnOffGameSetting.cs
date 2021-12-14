using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsRandomizer.Screens.Settings.GameSettingObjects
{
	public class OnOffGameSetting : GameSetting
	{
		public bool CurrentValue { get; set; }
		public OnOffGameSetting(string name, string description, bool defaultValue) : base(name, description, defaultValue)
		{
			CurrentValue = defaultValue;
		}
	}
}
