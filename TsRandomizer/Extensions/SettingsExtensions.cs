using TsRandomizer.Settings;

namespace TsRandomizer.Extensions
{
	static class SettingsExtensions
	{
		public static void EnforceTournamentSettings(this SettingCollection settings)
		{
			settings.BossRando.Value = true;
			settings.BossScaling.Value = true;
			settings.DamageRando.Value = "Balanced";
			settings.DamageRandoOverrides.SetDefault();
			settings.HpCap.Value = 999;
			settings.BossHealing.Value = true;
			settings.ShopFill.Value = "Random";
			settings.ShopMultiplier.Value = 1;
			settings.ShopWarpShards.Value = true;
			settings.LootPool.Value = "Random";
			settings.DropRateCategory.Value = "Vanilla";
			settings.DropRate.SetDefault();
			settings.LootTierDistro.Value = "Default Weight";
			settings.ShowBestiary.Value = false;
			settings.ShowDrops.Value = true;
		}
	}
}
