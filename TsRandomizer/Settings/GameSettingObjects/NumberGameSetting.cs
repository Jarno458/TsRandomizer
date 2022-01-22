using Newtonsoft.Json;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class NumberGameSetting : GameSetting<float>
	{
		public double MinimumValue { get; }
		public double MaximumValue { get; }

		[JsonIgnore]
		public double StepValue { get; set; }

		public override void SetValue(dynamic input)
		{
			if (!double.IsNaN(input))
			{
				double value = input;

				base.SetValue(value); 
			}
			else
			{
				base.SetValue(DefaultValue);
			}
		}

		public NumberGameSetting(string category, string name, string description, float minValue, float maxValue, float stepValue,
			float? defaultValue = null, bool canBeChangedInGame = false) 
			: base(category, name, description, defaultValue ?? minValue, canBeChangedInGame)
		{
			MinimumValue = minValue;
			MaximumValue = maxValue;
			StepValue = stepValue;

			SetValue(defaultValue ?? minValue);
		}

		[JsonConstructor]
		public NumberGameSetting() { }
	}
}
