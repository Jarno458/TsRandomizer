using Newtonsoft.Json;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public abstract class GameSetting
	{
		public dynamic CurrentValue { get; protected set; }

		[JsonIgnore]
		public string Category { get; }
		[JsonIgnore]
		public string Name { get; }
		[JsonIgnore]
		public string Description { get; }
		[JsonIgnore]
		public object DefaultValue { get; }
		[JsonIgnore]
		public bool CanBeChangedInGame { get; }

		protected GameSetting(string category, string name, string description, object defaultValue, bool canBeChangedInGame)
		{
			Category = category;
			Name = name;
			Description = description;
			DefaultValue = defaultValue;
			CanBeChangedInGame = canBeChangedInGame;
			CurrentValue = DefaultValue;
		}

		// ReSharper disable once PublicConstructorInAbstractClass
		public GameSetting() { }

		public virtual void SetValue(object input) => CurrentValue = input;
	}

	public abstract class GameSetting<T> : GameSetting
	{
		public T Value => (T)CurrentValue;

		protected GameSetting(string category, string name, string description, T defaultValue, bool canBeChangedInGame)
			: base(category, name, description, defaultValue, canBeChangedInGame)
		{
		}

		// ReSharper disable once PublicConstructorInAbstractClass
		public GameSetting() { }
	}
}
