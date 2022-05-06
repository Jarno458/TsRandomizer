using Newtonsoft.Json;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public abstract class GameSetting
	{
		public object CurrentValue { get; set; }

		[JsonIgnore]
		public string Name { get; }
		[JsonIgnore]
		public string Description { get; }
		[JsonIgnore]
		public object DefaultValue { get; }
		[JsonIgnore]
		public bool CanBeChangedInGame { get; }

		protected GameSetting(string name, string description, object defaultValue, bool canBeChangedInGame)
		{
			Name = name;
			Description = description;
			DefaultValue = defaultValue;
			CanBeChangedInGame = canBeChangedInGame;
			CurrentValue = DefaultValue;
		}

		// ReSharper disable once PublicConstructorInAbstractClass
		public GameSetting()
		{
		}

		public abstract void ToggleValue();

		public void SetDefault() => CurrentValue = DefaultValue;

		internal virtual void UpdateMenuEntry(MenuEntry menuEntry)
		{
			menuEntry.IsCenterAligned = false;

			menuEntry.Text = $"{Name} - {CurrentValue}";
			menuEntry.Description = Description;
		}
	}

	public abstract class GameSetting<T> : GameSetting
	{
		[JsonIgnore]
		public T Value {
			get { return (T)CurrentValue; }
			set { CurrentValue = value; }
		}

		protected GameSetting(string name, string description, T defaultValue, bool canBeChangedInGame)
			: base(name, description, defaultValue, canBeChangedInGame)
		{
		}

		[JsonConstructor]
		public GameSetting()
		{
		}
	}
}
