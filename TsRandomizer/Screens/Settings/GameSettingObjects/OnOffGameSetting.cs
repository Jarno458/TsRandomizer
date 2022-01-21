using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TsRandomizer.Screens.Settings.GameSettingObjects
{
	public class OnOffGameSetting : GameSetting
	{
		[JsonConstructor]
		public OnOffGameSetting(string name, string description, bool defaultValue, bool canBeToggledInGame) : base(name, description, defaultValue, canBeToggledInGame)
		{

		}
		public OnOffGameSetting(SettingsConstants constants, OnOffGameSetting settings) : base(constants, settings)
		{
			SetValue(settings.CurrentValue);
		}
	}
}
