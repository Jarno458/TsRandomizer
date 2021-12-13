using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsRandomizer.Screens.Settings
{
	abstract class GameSetting
	{
		public GameSetting(string name, string description, dynamic defaultValue)
		{
			Name = name;
			Description = description;
			DefaultValue = defaultValue;
		}
		public string Name { get; set; }
		public string Description { get; set; }
		public dynamic DefaultValue { get; set; }
	}
}
