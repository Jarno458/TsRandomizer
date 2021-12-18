using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TsRandomizer.Screens.SeedSelection;
using TsRandomizer.Screens.Settings.GameSettingObjects;

namespace TsRandomizer.Screens.Settings
{
	class SeedSettingCategoryInfo
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public GameSetting[] Settings { get; set; }
	}
}
