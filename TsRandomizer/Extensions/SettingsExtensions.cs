using TsRandomizer.Settings;

namespace TsRandomizer.Extensions
{
	static class SettingsExtensions
	{
		public static void EnforceTournamentSettings(this SettingCollection settings)
		{
			settings.BossRando.Value = "Scaled";
			settings.EnemyRando.Value = "Off";

			settings.DamageRando.Value = "Balanced";
			settings.DamageRandoOverrides.SetDefault();

			settings.HpCap.Value = 999;
			settings.LevelCap.Value = 99;
			settings.BossHealing.Value = true;

			settings.ShopFill.Value = "Random";
			settings.ShopMultiplier.Value = 1;
			settings.ShopWarpShards.Value = true;

			settings.LootPool.Value = "Random";

			settings.DropRateCategory.Value = "Random";
			settings.DropRate.SetDefault();
			settings.LootTierDistro.Value = "Default Weight";

			settings.ShowBestiary.Value = false;

			settings.ShowDrops.Value = true;

			settings.SparrowTrap.Value = true;
			settings.BeeTrap.Value = true;
			settings.PoisonTrap.Value = true;
			settings.NeurotoxinTrap.Value = true;
			settings.ChaosTrap.Value = true;

			settings.ExtraEarringsXP.Value = 0;

			settings.NoSaveStatues.Value = false;
		}
	}
}
