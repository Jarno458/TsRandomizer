using System;
using System.Collections.Generic;
using TsRandomizer.Settings.GameSettingObjects;

namespace TsRandomizer.Settings
{
	public class SettingCollection
	{
		public static readonly GameSettingCategoryInfo[] Categories = {
			new GameSettingCategoryInfo { Name = "Stats", Description = "Settings related to player and enemy stat scaling.",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.BossRando, s => s.BossScaling, s => s.DamageRando, s => s.HpCap, s => s.BossHealing
				}},
			new GameSettingCategoryInfo { Name = "Loot", Description = "Settings related to shop inventory and loot.",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.ShopFill, s => s.ShopMultiplier, s => s.ShopWarpShards, s => s.LootPool, s => s.DropRateCategory, s => s.DropRate, s => s.LootTierDistro
				}},
			new GameSettingCategoryInfo { Name = "Minimap", Description = "Settings related to minimap colors.",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.PastMinimapColor, s => s.PresentMinimapColor, s => s.PyramidMinimapColor,
					s => s.LootMinimapColor, s => s.SpecailLootMinimapColor,
					s => s.SaveStatueMinimapColor, s => s.PresentTransitionMinimapColor, s => s.PastTransitionMinimapColor
				}},
			new GameSettingCategoryInfo { Name = "Archipelago", Description = "Settings related to games with the Archipelago multiworld.",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.NumberOfOnScreenLogLines, s => s.OnScreenLogLineScreenTime, s => s.ShowSendItemsFromMe, s => s.ShowReceivedItemsFromMe,
					s => s.ShowSendGenericItems, s => s.ShowSendImportantItems, s => s.ShowSendProgressionItems, s => s.ShowSendTrapItems,
					s => s.ShowSystemMessages, s => s.DeathLink
				}},
			new GameSettingCategoryInfo { Name = "Other", Description = "Miscellaneous settings",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.ShowBestiary, s => s.ShowDrops
				}}
		};
		public OnOffGameSetting BossRando = new OnOffGameSetting("Boss Randomization",
			"All boss locations are shuffled.", false, false);

		public OnOffGameSetting BossScaling = new OnOffGameSetting("Random Boss Scaling",
			"Random bosses inherit the HP, ATK, and XP of their location (Recommended).", true, false);

		public SpecificValuesGameSetting DamageRando = new SpecificValuesGameSetting("Damage Randomizer",
			"Randomly nerfs and buffs orbs, spells, and some rings. \"Manual\" requires editing the randomizer settings file.",
			new List<string> { "Off", "All Nerfs", "Mostly Nerfs", "Balanced", "Mostly Buffs", "All Buffs", "Manual" }, "Off");

		public DamageRandoOverridesSetting DamageRandoOverrides = new DamageRandoOverridesSetting("Damage Randomizer Overrides",
			"Overrides the odds for each orb to be nerfed or buffed. Only editable from the file, so you shouldn't even be seeing this text.");

		public NumberGameSetting HpCap = new NumberGameSetting("HP Cap",
			"Sets the maximum HP Lunais is allowed to have", 1, 999, 64, 999);

		public OnOffGameSetting BossHealing = new OnOffGameSetting("Heal After Bosses",
			"When disabled, bosses will not provide healing. NOTE: currently only applicable for Boss Rando", true, false);

		public OnOffGameSetting ShowBestiary = new OnOffGameSetting("Show Bestiary",
			"All bestiary entries in the journal are visible by default.", false, false);

		public OnOffGameSetting ShowDrops = new OnOffGameSetting("Show Enemy Drops",
			"All item drops in the bestiary are visible by default.", false, false);

		public SpecificValuesGameSetting LootPool = new SpecificValuesGameSetting("Loot Pool",
			"Sets which items enemies will drop: [Vanilla, Random, Empty]",
			new List<string> { "Vanilla", "Random", "Empty" }, "Vanilla", true);

		public SpecificValuesGameSetting DropRateCategory = new SpecificValuesGameSetting("Drop Rate Category",
			"Sets the drop rate of enemy items [Tiered (based on item), Vanilla (based on enemy), Random (2-12%), Fixed",
			new List<string> { "Tiered", "Vanilla", "Random", "Fixed" }, "Tiered", true);

		public NumberGameSetting DropRate = new NumberGameSetting("Fixed Drop Rate",
			"Drop rate to be used when Drop Rate Category set to 'Fixed'", 0, 100, 2.5, 5);

		public SpecificValuesGameSetting LootTierDistro = new SpecificValuesGameSetting("Loot Tier Distribution",
			"Sets how frequently items of each loot rarity tier will be assigned to a drop slot.",
			new List<string> { "Default Weight", "Full Random", "Inverted Weight" }, "Default Weight", true);

		public SpecificValuesGameSetting ShopFill = new SpecificValuesGameSetting("Shop Inventory",
			"Sets the items for sale in Merchant Crow's shops. Options: [Default,Random,Vanilla,Empty]",
			new List<string> { "Default", "Random", "Vanilla", "Empty" }, "Default", true);

		public NumberGameSetting ShopMultiplier = new NumberGameSetting("Shop Price Multiplier",
			"Multiplier for the cost of items in the shop. Set to 0 for free shops", 0, 10, 1, 1);

		public OnOffGameSetting ShopWarpShards = new OnOffGameSetting("Always Sell Warp Shards",
			"Shops always sell warp shards (when keys possessed), ignoring inventory setting.", false);

		public NumberGameSetting NumberOfOnScreenLogLines = new NumberGameSetting("Log Number of Lines",
			"Max number of messages to show at the bottom left of the screen, 0 to turn onscreen log off", 0, 25, 1, 3, true);

		public NumberGameSetting OnScreenLogLineScreenTime = new NumberGameSetting("Log Line ScreenTime",
			"How long does a single line shown at the bottom left of the screen stay visible", 1, 10, 0.5, 8, true);

		public OnOffGameSetting ShowSendItemsFromMe = new OnOffGameSetting("Log Items sent by you",
			"Logs items sent from the you to other players", true, true);

		public OnOffGameSetting ShowReceivedItemsFromMe = new OnOffGameSetting("Log Items received by you",
			"Logs items you receive from other players", false, true);

		public OnOffGameSetting ShowSendGenericItems = new OnOffGameSetting("Log Generic Items",
			"Logs Generic items sent between other players", false, true);

		public OnOffGameSetting ShowSendImportantItems = new OnOffGameSetting("Log Important Items",
			"Logs Important items sent between other players", false, true);

		public OnOffGameSetting ShowSendProgressionItems = new OnOffGameSetting("Log Progression Items",
			"Logs Progression items sent between other players", true, true);

		public OnOffGameSetting ShowSendTrapItems = new OnOffGameSetting("Log Trap Items",
			"Logs Traps sent between other players", true, true);

		public OnOffGameSetting ShowSystemMessages = new OnOffGameSetting("Log System Message",
			"Logs System messages, like who connected/left and who changed tags", true, true);

		public ColorGameSetting PastMinimapColor = new ColorGameSetting("Past color",
			"Sets the default color for past minimap rooms", "#486090", true);

		public ColorGameSetting PresentMinimapColor = new ColorGameSetting("Present color",
			"Sets the default color for present minimap rooms", "#802880", true);

		public ColorGameSetting PyramidMinimapColor = new ColorGameSetting("Pyramid color",
			"Sets the default color for ancient pyramid minimap rooms", "#609030", true);

		public ColorGameSetting LootMinimapColor = new ColorGameSetting("Loot color",
			"Sets the color for rooms that have items in them for you", "#C5782A", true);

		public ColorGameSetting SpecailLootMinimapColor = new ColorGameSetting("Special Loot color",
			"Sets the color for rooms that have items in them for you and that are also special rooms like a boss room or transition room", "#F0D840", true);

		public ColorGameSetting SaveStatueMinimapColor = new ColorGameSetting("Save statue color",
			"Sets the color for rooms that have items in them for you", "#D04040", true);

		public ColorGameSetting PresentTransitionMinimapColor = new ColorGameSetting("Present transition color",
			"Sets the color for rooms that have items in them for you", "#1A52FB", true);

		public ColorGameSetting PastTransitionMinimapColor = new ColorGameSetting("Past transition color",
			"Sets the color for rooms that have items in them for you", "#9712C2", true);

		public OnOffGameSetting DeathLink = new OnOffGameSetting("DeathLink",
			"Sets whether DeathLink is on or off", false, true);
	}
}