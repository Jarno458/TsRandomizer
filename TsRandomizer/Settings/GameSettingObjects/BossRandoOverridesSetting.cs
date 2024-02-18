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

		public BossRandoOverridesSetting(string name, string description, bool canBeChangedInGame = false)
				: base(name, description, GetDefaultBossMapping(), canBeChangedInGame)
		{

		}

		[JsonConstructor]
		public BossRandoOverridesSetting()
		{
		}

		public override void ToggleValue()
		{

		}
	}
}
