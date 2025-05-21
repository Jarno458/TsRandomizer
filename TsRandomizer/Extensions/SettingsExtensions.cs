using System;
using System.Linq;
using System.Reflection;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Randomisation;
using TsRandomizer.Settings;
using TsRandomizer.Settings.GameSettingObjects;

namespace TsRandomizer.Extensions
{
	static class SettingsExtensions
	{
		public static void EnforceTournamentSettings(this SettingCollection settings)
		{
			settings.BossRando.Value = "Scaled"; //Not clear if should be scaled or unscaled
			settings.BossRandoType.Value = "Shuffle";
			settings.BossRandoOverrides.SetDefault();

			settings.DamageRando.Value = "Balanced";
			settings.DamageRandoOverrides.SetDefault();

			settings.ShopFill.Value = "Random";
			settings.ShopMultiplier.Value = 1; //not voted
			settings.ShopWarpShards.Value = true;

			settings.LootPool.Value = "Random";
			settings.DropRateCategory.Value = "Random"; //Not voted
			settings.DropRate.SetDefault();
			settings.LootTierDistro.Value = "Default Weight"; //not voted

			settings.HpCap.Value = 999;
			settings.LevelCap.Value = 99;
			settings.AuraCap.Value = 999;

			settings.ExtraEarringsXP.Value = 15;

			settings.EnemyRando.Value = "Off"; //not voted
			settings.BossHealing.Value = true; //not voted

			settings.ShowBestiary.Value = false; //not voted
			settings.ShowDrops.Value = true; //not voted

			settings.SparrowTrap.Value = true;
			settings.BeeTrap.Value = true;
			settings.PoisonTrap.Value = true;
			settings.NeurotoxinTrap.Value = true;
			settings.ChaosTrap.Value = true;
			settings.ThrowStunTrap.Value = true;
			settings.SpiderTrap.Value = true;
			settings.LightsOutTrap.Value = true;
			settings.PalmPunchTrap.Value = true;

			settings.NoSaveStatues.Value = false; //not voted

			settings.EnableMapFromStart.Value = false; //not voted
		}

		static readonly FieldInfo[] OrderedSettingsFieldsThatAffectCompetitiveBalance =
			typeof(SettingCollection)
				.GetFields()
				.Where(f => f.FieldType.IsSubclassOf(typeof(GameSetting))
				            && !f.GetCustomAttributes(typeof(DoesNotAffectCompetitiveBalance), false).Any())
				.OrderBy(f => f.Name)
				.ToArray();

		public static int GetHash(this SettingCollection settings)
		{
			uint hash = 0;

			for (var i = 0; i < OrderedSettingsFieldsThatAffectCompetitiveBalance.Length; i++)
			{
				var field = OrderedSettingsFieldsThatAffectCompetitiveBalance[i];
				hash ^= RotateLeft(GetFieldValueHash(settings, field), i);
			}

			return (int)hash;
		}

		static uint GetFieldValueHash(SettingCollection settings, FieldInfo field)
		{
			var setting = (GameSetting)field.GetValue(settings);

			switch (setting)
			{
				case OnOffGameSetting boolSetting:
					return boolSetting.Value ? 1U : 0U;
				case NumberGameSetting doubleSetting:
					return GetHashForDouble(doubleSetting.Value);
				case NumberGameSettingWithFixedSteps doubleSetting:
					return GetHashForDouble(doubleSetting.Value);
				case SpecificValuesGameSetting specificValuesSetting:
					return (uint)specificValuesSetting.AllowedValues.IndexOf(specificValuesSetting.Value);
				case BossRandoOverridesSetting _:
					return GetHashForBossRandoOverrides(settings);
				case DamageRandoOverridesSetting _:
					return GetHashForDamageRandoOverrides(settings);
				default:
					throw new NotImplementedException(
						$"GameSettings type {setting.GetType()} does not have an hash function implemented");
			}
		}
		
		static uint GetHashForDouble(double value)
		{
			if (value == 0)
				return 0U;

			var data = BitConverter.GetBytes(value);
			var a = BitConverter.ToUInt32(data, 0);
			var b = BitConverter.ToUInt32(data, 4);
			return a ^ b;
		}

		static uint GetHashForBossRandoOverrides(SettingCollection settings)
		{
			if (settings.BossRando.Value == "Off" || settings.BossRandoType.Value != "Manual" || settings.BossRandoOverrides.Value == null)
				return 0U;

			var orderedKvp = settings.BossRandoOverrides.Value.OrderBy(kvp => kvp.Key).ToArray();

			uint hash = 0;

			for (var i = 0; i < orderedKvp.Length; i++)
			{
				var originalBoss = orderedKvp[i].Key;
				var replacementBoss = orderedKvp[i].Value;

				uint kvpHash;

				if (originalBoss == null || replacementBoss == null
					|| !Enum.TryParse(originalBoss, out EBossID originalBossEnumValue)
				    || !Enum.TryParse(replacementBoss, out EBossID replacementBossEnumValue))
						kvpHash = 1337U;
				else
					kvpHash= ((uint)originalBossEnumValue << 8) | (uint)replacementBossEnumValue;

				hash ^= RotateLeft(kvpHash, i);
			}

			return hash;
		}

		static uint GetHashForDamageRandoOverrides(SettingCollection settings)
		{
			if (settings.DamageRando.Value == null || settings.DamageRando.Value != "Manual" || settings.DamageRandoOverrides.Value == null)
				return 0U;

			var orderedKvp = settings.DamageRandoOverrides.Value.OrderBy(kvp => kvp.Key).ToArray();

			uint hash = 0;

			for (var i = 0; i < orderedKvp.Length; i++)
			{
				var orb = orderedKvp[i].Key;
				var damageRandoOdds = orderedKvp[i].Value;

				uint kvpHash;

				if (orb == null || damageRandoOdds == null || !Enum.TryParse(orb, out EInventoryOrbType orbType))
					kvpHash = 1337U;
				else
					kvpHash = ((uint)orbType << 24) 
					          | ((uint)damageRandoOdds.MinusOdds << 16) 
					          | ((uint)damageRandoOdds.NormalOdds << 8) 
					          | (uint)damageRandoOdds.PlusOdds;

				hash ^= RotateLeft(kvpHash, i);
			}

			return hash;
		}

		static uint RotateLeft(this uint x, int nbitsShift) => (x << nbitsShift) | (x >> (32 - nbitsShift));
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class DoesNotAffectCompetitiveBalance : Attribute { }
}
