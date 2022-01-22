using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TsRandomizer.Settings.GameSettingObjects;

namespace TsRandomizer.Settings
{
	public class GameSettingCategoryInfo
	{
		[JsonIgnore]
		public string Name { get; set; }

		[JsonIgnore]
		public string Description { get; set; }

		[JsonIgnore]
		public List<Func<SettingCollection, GameSetting>> SettingsPerCategory;
	}
}
