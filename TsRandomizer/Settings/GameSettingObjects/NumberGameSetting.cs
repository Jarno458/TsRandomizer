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

		public NumberGameSetting(string name, string description, double minValue, double maxValue, double stepValue,
			double defaultValue, bool canBeChangedInGame = false)
			: base(name, description, defaultValue, canBeChangedInGame)
		{
			MinimumValue = minValue;
			MaximumValue = maxValue;
			StepValue = stepValue;
		}

		[JsonConstructor]
		public NumberGameSetting() { }

		public override void ToggleValue()
		{
			double newValue = Value + StepValue;
			if (newValue <= MaximumValue)
			{
				if (Value == MinimumValue && MinimumValue < StepValue)
					Value = StepValue;
				else Value = newValue;
			}
			else
			{
				Value = Value == MaximumValue ? MinimumValue : MaximumValue;
			}
		}

	}
}
