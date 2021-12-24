using Newtonsoft.Json;

namespace TsRandomizer.Screens.Settings
{
	public abstract class GameSetting
	{

		[JsonProperty]
		public object CurrentValue { get; protected set; }
		[JsonIgnore()]
		public string Name { get; }
		[JsonIgnore()]
		public string Description { get; }
		public object DefaultValue { get; }
		[JsonIgnore()]
		public bool CanBeChangedInGame { get; set; }

		public GameSetting(string name, string description, object defaultValue, bool canBeChangedInGame)
		{
			Name = name;
			Description = description;
			DefaultValue = defaultValue;
			CanBeChangedInGame = canBeChangedInGame;
			CurrentValue = DefaultValue;
		}

		public GameSetting() { }

		public virtual void SetValue(object input)
		{
			CurrentValue = input;
		}

		public bool IsDefault()
		{
			return CurrentValue == DefaultValue;
		}
	}
}
