using Newtonsoft.Json;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class NumberGameSetting : GameSetting<double>
	{
		[JsonIgnoreDeserialize]
		public double MinimumValue { get; }

		[JsonIgnoreDeserialize]
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

		public NumberGameSetting(string category, string name, string description, double minValue, double maxValue, double stepValue,
			double? defaultValue = null, bool canBeChangedInGame = false) 
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
