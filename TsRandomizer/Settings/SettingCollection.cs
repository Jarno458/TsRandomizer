using System;
using System.Collections.Generic;
using TsRandomizer.Settings.GameSettingObjects;

namespace TsRandomizer.Settings
{
	public class SettingCollection
	{
		public static readonly GameSettingCategoryInfo[] Categories = {
			new GameSettingCategoryInfo { Name = "Stats", Description = "Settings related to player stat scaling.", 
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.DamageRando
				}},
			new GameSettingCategoryInfo { Name = "Loot", Description = "Settings related to shop inventory and loot.", 
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.ShopFill, s => s.ShopMultiplier, s => s.ShopWarpShards
				}}
		};

		public OnOffGameSetting DamageRando = new OnOffGameSetting("Stat", "Damage Randomizer",
			"Adds a high chance to make orb damage very low, and a low chance to make orb damage very, very high");

		public SpecificValuesGameSetting ShopFill = new SpecificValuesGameSetting("Loot", "Shop Inventory",
			"Sets the items for sale in Merchant Crow's shops. Options: [Default,Random,Vanilla,Empty]",
			new List<string> { "Default", "Random", "Vanilla", "Empty" });

		public NumberGameSetting ShopMultiplier = new NumberGameSetting("Loot", "Shop Price Multiplier",
			"Multiplier for the cost of items in the shop. Set to 0 for free shops", 0, 10, 1);

		public OnOffGameSetting ShopWarpShards = new OnOffGameSetting("Loot", "Always Sell Warp Shards",
			"Shops always sell warp shards (when keys possessed), ignoring inventory setting.");
	}
}