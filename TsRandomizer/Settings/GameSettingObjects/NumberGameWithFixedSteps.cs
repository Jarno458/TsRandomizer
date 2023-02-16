using System;
using Newtonsoft.Json;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class NumberGameWithFixedSteps : GameSetting<double>
	{
		[JsonIgnore]
		public double[] Steps { get; set; }

		public NumberGameWithFixedSteps(string name, string description, double[] steps,
			double defaultValue, bool canBeChangedInGame = false)
			: base(name, description, defaultValue, canBeChangedInGame)
		{
			Steps = steps;
		}

		[JsonConstructor]
		public NumberGameWithFixedSteps() { }

		public override void ToggleValue()
		{
			try
			{
				var currentIndex = Array.IndexOf(Steps, Value);

				if (currentIndex == -1)
				{
					SetValueToClosestStep();
				}
				else
				{
					var newIndex = currentIndex + 1 >= Steps.Length ? 0 : currentIndex + 1;

					Value = Steps[newIndex];

				}
			}
			catch
			{
				Value = Default;
			}
		}

		void SetValueToClosestStep()
		{
			if (Value < Steps[0])
			{
				Value = Steps[0];
				return;
			}

			if (Value > Steps[Steps.Length - 1])
			{
				Value = Steps[Steps.Length - 1];
				return;
			}

			var currentIndex = 0;
			while (currentIndex < Steps.Length)
			{
				if (Value < Steps[currentIndex])
					continue;

				if (currentIndex + 1 < Steps.Length)
				{
					Value = Steps[currentIndex + 1];
					return;
				}

				Value = Steps[currentIndex];
			}
		}
	}
}
