
using System;
using Timespinner.GameStateManagement.ScreenManager;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class StringGameSetting : GameSetting<string>
	{
		public int MaxLength { get; }

		public StringGameSetting(string name, string description, string defaultValue, int maxLength, bool canBeChangedInGame) 
			: base(name, description, defaultValue, canBeChangedInGame)
		{
			MaxLength = maxLength;
		}

		public StringGameSetting() { }

		//cannot be toggle instead required input
		public override void ToggleValue()
		{
		}

		public void HandleInput(InputState input)
		{
			throw new NotImplementedException("Not yet implemented");

			/*string value = input;
			if (string.IsNullOrWhiteSpace(value))
			{
				base.SetValue((string)DefaultValue);
			}
			else
			{
				if (value.Length > MaxLength) value = value.Substring(0, MaxLength - 1);

				base.SetValue(value);
			}*/
		}
	}
}
