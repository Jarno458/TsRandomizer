using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Timespinner.GameAbstractions.Inventory;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class OrbDamageOdds
	{
		public enum OrbDamageModifier
		{
			Minus,
			Normal,
			Plus
		}

		public EInventoryOrbType orbType;
		public int MinusOdds { get; set; }
		public int NormalOdds { get; set; }
		public int PlusOdds { get; set; }
		public OrbDamageModifier GetModifier(Random random)
		{
			try
			{
				int sumOfWeights = MinusOdds + PlusOdds + NormalOdds;
				int choice = random.Next(1, sumOfWeights);
				switch (choice)
				{
					case int o when (o <= MinusOdds):
						return OrbDamageModifier.Minus;
					case int o when (o > MinusOdds + NormalOdds):
						return OrbDamageModifier.Plus;
					default:
						return OrbDamageModifier.Normal;
				}
			}
			catch
			{
				return OrbDamageModifier.Normal;
			}
		}
	}

	public class DamageRandoOverridesSetting : GameSetting<List<OrbDamageOdds>>
	{
		private static List<OrbDamageOdds> GetDefaultOrbOdds()
		{
			List<OrbDamageOdds> defaultOdds = new List<OrbDamageOdds>();
			foreach (EInventoryOrbType orbType in Enum.GetValues(typeof(EInventoryOrbType)))
			{
				defaultOdds.Add(new OrbDamageOdds { MinusOdds = 1, NormalOdds = 1, PlusOdds = 1, orbType = orbType });
			}
			return defaultOdds;
		}

		public DamageRandoOverridesSetting(string name, string description, bool canBeChangedInGame = false)
				: base(name, description, GetDefaultOrbOdds(), canBeChangedInGame)
		{

		}

		[JsonConstructor]
		public DamageRandoOverridesSetting()
		{
		}

		public override void ToggleValue()
		{

		}
	}
}
