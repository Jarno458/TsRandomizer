using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class DamageRandoOverridesSetting : GameSetting<Dictionary<string, OrbDamageOdds>>
	{
		internal static Dictionary<string, OrbDamageOdds> GetDefaultOrbOdds()
		{
			Dictionary<string, OrbDamageOdds> defaultOdds =
				new Dictionary<string, OrbDamageOdds>();
			foreach (EInventoryOrbType orbType in Enum.GetValues(typeof(EInventoryOrbType)))
			{
				if (orbType != EInventoryOrbType.Monske && orbType != EInventoryOrbType.None)
					defaultOdds.Add(orbType.ToString(), new OrbDamageOdds
					{
						MinusOdds = 1,
						NormalOdds = 1,
						PlusOdds = 1,
					});
			}
			return defaultOdds;
		}

		public DamageRandoOverridesSetting(string name, string description)
				: base(name, description, GetDefaultOrbOdds(), false)
		{

		}

		[JsonConstructor]
		public DamageRandoOverridesSetting()
		{
		}

		public override void ToggleValue() => throw new NotImplementedException("This value cannot be edited ingame");
	}
}
