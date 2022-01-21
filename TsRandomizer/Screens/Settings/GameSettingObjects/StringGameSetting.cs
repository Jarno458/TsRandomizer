
namespace TsRandomizer.Screens.Settings.GameSettingObjects
{
	public class StringGameSetting : GameSetting
	{
		public int MaxLength { get; }
		public override void SetValue(dynamic input)
		{
			if (input is string)
			{
				string value = input;
				if (string.IsNullOrWhiteSpace(value))
				{
					base.SetValue((string)DefaultValue);
				}
				else
				{
					if (value.Length > MaxLength) value = value.Substring(0, MaxLength - 1);
					base.SetValue(value);
				}
			}
		}

		public StringGameSetting(string name, string description, string defaultValue, int maxLength, bool canBeChangedInGame) : base(name, description, defaultValue, canBeChangedInGame)
		{
			MaxLength = maxLength;

			SetValue(defaultValue);
		}

		public StringGameSetting() { }
	}
}
