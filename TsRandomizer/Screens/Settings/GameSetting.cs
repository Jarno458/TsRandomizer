using Newtonsoft.Json;

namespace TsRandomizer.Screens.Settings
{
	public abstract class GameSetting
	{
		[JsonProperty]
		public object CurrentValue { get; protected set; }
		[JsonIgnore]
		public string Name { get; set; }
		[JsonIgnore]
		public string Description { get; set; }
		[JsonIgnore]
		public object DefaultValue { get; }
		[JsonIgnore]
		public bool CanBeChangedInGame { get; set; }

		public GameSetting(string name, string description, object defaultValue, bool canBeChangedInGame)
		{
			Name = name;
			Description = description;
			DefaultValue = defaultValue;
			CanBeChangedInGame = canBeChangedInGame;
			CurrentValue = DefaultValue;
		}

		public GameSetting(SettingsConstants constants, GameSetting setting)
		{
			DefaultValue = constants.DefaultValue;
			Description = constants.Description;
			Name = constants.Name;
		}

		public GameSetting() { }

		public virtual void SetValue(object input) => CurrentValue = input;

		public bool IsDefault() => CurrentValue == DefaultValue;
	}
}
