using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class BossRandoOverridesSetting : GameSetting<Dictionary<string, string>>
	{
		internal static Dictionary<string, string> GetDefaultBossMapping()
		{
			Dictionary<string, string> defaultBossMapping =
				new Dictionary<string, string>();
			foreach (string bossName in Enum.GetNames(typeof(EBossID)))
			{
				defaultBossMapping.Add(bossName, bossName);
			}
			return defaultBossMapping;
		}

		public BossRandoOverridesSetting(string name, string description)
				: base(name, description, GetDefaultBossMapping(), false)
		{

		}

		[JsonConstructor]
		public BossRandoOverridesSetting()
		{
		}

		public override void ToggleValue() => throw new NotImplementedException("This value cannot be edited ingame");
	}
}
