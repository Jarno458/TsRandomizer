using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsRandomizer.Screens.Settings
{
	public abstract class GameSetting
	{
		public GameSetting(string name, string description, dynamic defaultValue, bool canBeChangedInGame)
		{
			Name = name;
			Description = description;
			DefaultValue = defaultValue;
			CanBeChangedInGame = canBeChangedInGame;
		}
		public string Name { get; set; }
		public string Description { get; set; }
		public dynamic DefaultValue { get; set; }
		public bool CanBeChangedInGame { get; set; }
	}
}
