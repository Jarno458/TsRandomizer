using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TsRandomizer.Screens.Settings
{
	public abstract class GameSetting
	{

		[JsonProperty]
		public dynamic CurrentValue { get; protected set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public dynamic DefaultValue { get; set; }
		[JsonIgnore()]
		public bool CanBeChangedInGame { get; set; }

		public GameSetting(string name, string description, dynamic defaultValue, bool canBeChangedInGame)
		{
			Name = name;
			Description = description;
			DefaultValue = defaultValue;
			CanBeChangedInGame = canBeChangedInGame;
			CurrentValue = DefaultValue;
		}

		public GameSetting() { }

		public virtual void SetValue(dynamic input)
		{
			CurrentValue = input;
		}

		public bool IsDefault()
		{
			return CurrentValue == DefaultValue;
		}
	}
}
