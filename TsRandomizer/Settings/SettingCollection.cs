using System;
using System.Collections.Generic;
using TsRandomizer.Extensions;
using TsRandomizer.Settings.GameSettingObjects;

namespace TsRandomizer.Settings
{
	public class SettingCollection
	{
		public static readonly GameSettingCategoryInfo[] Categories = {
			new GameSettingCategoryInfo { Name = "Stats", Description = "Settings related to player and enemy stat scaling.",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.BossRando, s => s.BossRandoType, s => s.EnemyRando, s => s.DamageRando,
					s => s.HpCap, s => s.AuraCap, s => s.LevelCap, s => s.ExtraEarringsXP, s => s.BossHealing
				}},
			new GameSettingCategoryInfo { Name = "Loot", Description = "Settings related to shop inventory and loot.",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.ShopFill, s => s.ShopMultiplier, s => s.ShopWarpShards, s => s.LootPool,
					s => s.DropRateCategory, s => s.DropRate, s => s.LootTierDistro
				}},
			new GameSettingCategoryInfo { Name = "Traps", Description = "Toggles traps available via the Trapped Chests flag.",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.SparrowTrap, s => s.NeurotoxinTrap, s => s.ChaosTrap, s => s.PoisonTrap, s => s.BeeTrap, s => s.ThrowStunTrap
				}},
			new GameSettingCategoryInfo { Name = "Minimap", Description = "Settings related to minimap colors.",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.PastMinimapColor, s => s.PresentMinimapColor, s => s.PyramidMinimapColor,
					s => s.LootMinimapColor, s => s.SpecailLootMinimapColor,
					s => s.SaveStatueMinimapColor, s => s.PresentTransitionMinimapColor, s => s.PastTransitionMinimapColor,
					s => s.HintedMinimapColor, s => s.FinalBossMinimapColor
				}},
			new GameSettingCategoryInfo { Name = "Sprites", Description = "Changes custom sprites",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.LunaisSprite, s => s.LunaisEternalSprite, s => s.LunaisGoddessSprite,
					s => s.MeyefSprite, s => s.MeyefWyrmSprite,
					s => s.MerchantCrowSprite, s => s.MerchantCrowGreedSprite,
					s => s.KoboSprite, s => s.GriffinSprite, s => s.DemonSprite, s => s.SpriteFamiliarSprite
				}},
			new GameSettingCategoryInfo { Name = "Archipelago", Description = "Settings related to games with the Archipelago multiworld.",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.NumberOfOnScreenLogLines, s => s.OnScreenLogLineScreenTime, s => s.ShowSendItemsFromMe, s => s.ShowReceivedItemsFromMe,
					s => s.ShowSendGenericItems, s => s.ShowSendImportantItems, s => s.ShowSendProgressionItems, s => s.ShowSendTrapItems,
					s => s.ShowSystemMessages, s => s.DeathLink, s => s.GiftingReminderInterval
				}},
			new GameSettingCategoryInfo { Name = "Other", Description = "Miscellaneous settings",
				SettingsPerCategory = new List<Func<SettingCollection, GameSetting>> {
					s => s.ShowBestiary, s => s.ShowDrops, s => s.MeleeAutofire, s => s.NoSaveStatues, s => s.EnableMapFromStart
				}}
		};

		public SpecificValuesGameSetting BossRando = new SpecificValuesGameSetting("Boss Randomization",
			"Sets wheter all boss locations are shuffled, and if their damage/hp should be scaled.",
			new List<string> { "Off", "Scaled", "UnScaled" }, "Off", true);

		public SpecificValuesGameSetting BossRandoType = new SpecificValuesGameSetting("Boss Randomization Type",
			"Sets what type of boss shuffling occurs.",
			new List<string> { "Shuffle", "Chaos", "Singularity", "Manual" }, "Shuffle", true);

		public BossRandoOverridesSetting BossRandoOverrides = new BossRandoOverridesSetting("Boss Randomizer Overrides",
			"Overrides the shuffling for each boss. Only editable from the file");

		public SpecificValuesGameSetting EnemyRando = new SpecificValuesGameSetting("Enemy Randomization",
			"Sets wheter enemies will be randomized, and if their damage/hp should be scaled.",
			new List<string> { "Off", "Scaled", "UnScaled", "Ryshia" }, "Off", true);

		public SpecificValuesGameSetting DamageRando = new SpecificValuesGameSetting("Damage Randomizer",
			"Randomly nerfs and buffs orbs, spells, and some rings. \"Manual\" requires editing the randomizer settings file.",
			new List<string> { "Off", "All Nerfs", "Mostly Nerfs", "Balanced", "Mostly Buffs", "All Buffs", "Manual" }, "Off");

		public DamageRandoOverridesSetting DamageRandoOverrides = new DamageRandoOverridesSetting("Damage Randomizer Overrides",
			"Overrides the odds for each orb to be nerfed or buffed. Only editable from the file");

		public NumberGameSetting HpCap = new NumberGameSetting("HP Cap",
			"Sets the maximum HP Lunais is allowed to have", 1, 999, 64, 999);

		public NumberGameSettingWithFixedSteps AuraCap = new NumberGameSettingWithFixedSteps("Aura Cap",
			"Sets the maximum Aura Lunais is allowed to have. Level 1 is 80. Djinn Inferno costs 45.",
			new double[] { 45, 69, 80, 89, 90, 120, 150, 180, 210, 240, 270, 300, 420, 999 }, 999);

		public OnOffGameSetting BossHealing = new OnOffGameSetting("Heal After Bosses",
			"When disabled, bosses will not provide healing. NOTE: currently only applicable for Boss Rando", true);

		public OnOffGameSetting ShowBestiary = new OnOffGameSetting("Show Bestiary",
			"All bestiary entries in the journal are visible by default.", false);

		public OnOffGameSetting ShowDrops = new OnOffGameSetting("Show Enemy Drops",
			"All item drops in the bestiary are visible by default.", false);

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

		public OnOffGameSetting SparrowTrap = new OnOffGameSetting("Meteor Sparrows",
			"Traps can spawn meteor sparrows.", true);

		public OnOffGameSetting NeurotoxinTrap = new OnOffGameSetting("Neurotoxin",
			"Traps can inflict aura-draining neurotoxin.", true);

		public OnOffGameSetting ChaosTrap = new OnOffGameSetting("Chaos",
			"Traps can cause sand-draining chaos.", true);

		public OnOffGameSetting PoisonTrap = new OnOffGameSetting("Poison",
			"Traps can inflict poison.", true);

		public OnOffGameSetting BeeTrap = new OnOffGameSetting("Bees",
			"Beeeeeeeeeeeeeeeeeeeeeeeeees!", true);

		public OnOffGameSetting ThrowStunTrap = new OnOffGameSetting("Throw",
			"Traps can cause Lunais to go flying.", true);

		[DoesNotAffectCompetitiveBalance] 
		public SpriteGameSetting LunaisSprite = new SpriteGameSetting("Lunais",
			"Sets the default Lunais sprite.", "Lunais", "Content\\Sprites\\Heroes\\LunaisSprite.xnb");
		[DoesNotAffectCompetitiveBalance] 
		public SpriteGameSetting LunaisEternalSprite = new SpriteGameSetting("Eternal",
			"Sets the Lunais sprite used by the eternal brooch.", "Lunais", "Content\\Sprites\\Heroes\\LunaisAltSprite.xnb");
		[DoesNotAffectCompetitiveBalance]
		public SpriteGameSetting LunaisGoddessSprite = new SpriteGameSetting("Goddess",
			"Sets the Lunais sprite used by the goddess brooch.", "Lunais", "Content\\Sprites\\Heroes\\LunaisAltSprite2.xnb");
		[DoesNotAffectCompetitiveBalance]
		public SpriteGameSetting MeyefSprite = new SpriteGameSetting("Meyef",
			"Sets the default Meyef sprite.", "Meyef", "Content\\Sprites\\Heroes\\FamiliarMeyef.xnb");
		[DoesNotAffectCompetitiveBalance]
		public SpriteGameSetting MeyefWyrmSprite = new SpriteGameSetting("Wyrm",
			"Sets the Meyef Sprite used by the wyrm brooch.", "Meyef", "Content\\Sprites\\Heroes\\FamiliarAltMeyef.xnb");
		[DoesNotAffectCompetitiveBalance]
		public SpriteGameSetting MerchantCrowSprite = new SpriteGameSetting("Merchant Crow",
			"Sets the default Merchant Crow sprite.", "Crow", "Content\\Sprites\\Heroes\\FamiliarCrow.xnb");
		[DoesNotAffectCompetitiveBalance]
		public SpriteGameSetting MerchantCrowGreedSprite = new SpriteGameSetting("Greed",
			"Sets the Merchant Crow sprite used by the greed brooch.", "Crow", "Content\\Sprites\\Heroes\\FamiliarAltCrow.xnb");
		[DoesNotAffectCompetitiveBalance]
		public SpriteGameSetting GriffinSprite = new SpriteGameSetting("Griffin",
			"Sets the Griffin sprite.", "Griffin", "Content\\Sprites\\Heroes\\FamiliarGriffin.xnb");
		[DoesNotAffectCompetitiveBalance]
		public SpriteGameSetting DemonSprite = new SpriteGameSetting("Demon",
			"Sets the Demon familiar sprite.", "Demon", "Content\\Sprites\\Heroes\\FamiliarDemon.xnb");
		[DoesNotAffectCompetitiveBalance]
		public SpriteGameSetting KoboSprite = new SpriteGameSetting("Kobo",
			"Sets the Kobo sprite.", "Kobo", "Content\\Sprites\\Heroes\\FamiliarKobo.xnb");
		[DoesNotAffectCompetitiveBalance]
		public SpriteGameSetting SpriteFamiliarSprite = new SpriteGameSetting("Sprite",
			"Sets the sprite for the Sprite familiar", "Sprite", "Content\\Sprites\\Heroes\\FamiliarSprite.xnb", contentExcludeRegex: "Lunais.*");

		[DoesNotAffectCompetitiveBalance]
		public NumberGameSetting NumberOfOnScreenLogLines = new NumberGameSetting("Log Number of Lines",
			"Max number of messages to show at the bottom left of the screen, 0 to turn onscreen log off", 0, 25, 1, 3, true);

		[DoesNotAffectCompetitiveBalance]
		public NumberGameSetting OnScreenLogLineScreenTime = new NumberGameSetting("Log Line ScreenTime",
			"How long does a single line shown at the bottom left of the screen stay visible", 1, 10, 0.5, 8, true);

		[DoesNotAffectCompetitiveBalance]
		public OnOffGameSetting ShowSendItemsFromMe = new OnOffGameSetting("Log Items sent by you",
			"Logs items sent from the you to other players", true, true);

		[DoesNotAffectCompetitiveBalance]
		public OnOffGameSetting ShowReceivedItemsFromMe = new OnOffGameSetting("Log Items received by you",
			"Logs items you receive from other players", false, true);

		[DoesNotAffectCompetitiveBalance]
		public OnOffGameSetting ShowSendGenericItems = new OnOffGameSetting("Log Generic Items",
			"Logs Generic items sent between other players", false, true);

		[DoesNotAffectCompetitiveBalance]
		public OnOffGameSetting ShowSendImportantItems = new OnOffGameSetting("Log Important Items",
			"Logs Important items sent between other players", false, true);

		[DoesNotAffectCompetitiveBalance]
		public OnOffGameSetting ShowSendProgressionItems = new OnOffGameSetting("Log Progression Items",
			"Logs Progression items sent between other players", true, true);

		[DoesNotAffectCompetitiveBalance]
		public OnOffGameSetting ShowSendTrapItems = new OnOffGameSetting("Log Trap Items",
			"Logs Traps sent between other players", true, true);

		[DoesNotAffectCompetitiveBalance]
		public OnOffGameSetting ShowSystemMessages = new OnOffGameSetting("Log System Message",
			"Logs System messages, like who connected/left and who changed tags", true, true);

		[DoesNotAffectCompetitiveBalance]
		public ColorGameSetting PastMinimapColor = new ColorGameSetting("Past color",
			"Sets the default color for past minimap rooms", "#486090", true);

		[DoesNotAffectCompetitiveBalance]
		public ColorGameSetting PresentMinimapColor = new ColorGameSetting("Present color",
			"Sets the default color for present minimap rooms", "#802880", true);

		[DoesNotAffectCompetitiveBalance]
		public ColorGameSetting PyramidMinimapColor = new ColorGameSetting("Pyramid color",
			"Sets the default color for ancient pyramid minimap rooms", "#609030", true);

		[DoesNotAffectCompetitiveBalance]
		public ColorGameSetting LootMinimapColor = new ColorGameSetting("Loot color",
			"Sets the color for rooms that have items in them for you", "#C5782A", true);

		[DoesNotAffectCompetitiveBalance]
		public ColorGameSetting SpecailLootMinimapColor = new ColorGameSetting("Special Loot color",
			"Sets the color for rooms that have items in them for you and that are also special rooms like a boss room or transition room", "#F0D840", true);

		[DoesNotAffectCompetitiveBalance]
		public ColorGameSetting SaveStatueMinimapColor = new ColorGameSetting("Save statue color",
			"Sets the color for rooms that have items in them for you", "#D04040", true);

		[DoesNotAffectCompetitiveBalance]
		public ColorGameSetting PresentTransitionMinimapColor = new ColorGameSetting("Present transition color",
			"Sets the color for rooms that have items in them for you", "#1A52FB", true);

		[DoesNotAffectCompetitiveBalance]
		public ColorGameSetting PastTransitionMinimapColor = new ColorGameSetting("Past transition color",
			"Sets the color for rooms that have items in them for you", "#9712C2", true);

		[DoesNotAffectCompetitiveBalance]
		public ColorGameSetting HintedMinimapColor = new ColorGameSetting("Hinted loot color",
			"Sets the overlay color for rooms that have items in them that have been hinted for", "#00FF85", true);

		[DoesNotAffectCompetitiveBalance]
		public ColorGameSetting FinalBossMinimapColor = new ColorGameSetting("Final Boss color",
			"Sets the color for room of the final boss", "#EEEEEE", true);

		public NumberGameSettingWithFixedSteps LevelCap = new NumberGameSettingWithFixedSteps("Level Cap",
			"Sets the max level Lunais can achieve.",
			new double[] { 1, 5, 10, 15, 20, 25, 30, 69, 99 }, 99);

		public OnOffGameSetting DeathLink = new OnOffGameSetting("DeathLink",
			"Sets whether DeathLink is on or off", false, true);

		public NumberGameSetting ExtraEarringsXP = new NumberGameSetting("Extra Earrings XP",
			"Adds additional XP granted by Galaxy Earrings", 0, 24, 1, 0, true);

		[DoesNotAffectCompetitiveBalance]
		public OnOffGameSetting MeleeAutofire = new OnOffGameSetting("Autofire (Melee)",
			"Holding the melee attack button will attack repeatedly.", false, true);

		public OnOffGameSetting NoSaveStatues = new OnOffGameSetting("No Save Statues",
			"Breaks all the save statues", false, true);

		[DoesNotAffectCompetitiveBalance]
		public OnOffGameSetting EnableMapFromStart = new OnOffGameSetting("Enable map from start",
			"Marks all the rooms on minimap as known", false);

		[DoesNotAffectCompetitiveBalance]
		public NumberGameSettingWithFixedSteps GiftingReminderInterval = new NumberGameSettingWithFixedSteps("Received gifts reminder",
			"The interval in seconds to remind the user about pending received gifts.", new double[] { 0, 1, 5, 10, 30, 60, 300 }, 1, true);
	}
}
