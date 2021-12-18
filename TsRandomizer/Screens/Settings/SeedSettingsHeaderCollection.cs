namespace TsRandomizer.Screens.Settings
{
	class SeedSettingsCategoryCollection
	{
		public SeedSettingCategoryInfo[] SettingCategories;
		public SeedSettingsCategoryCollection()
		{
			GameSettingsCollection gameSettings = new GameSettingsCollection();
			gameSettings.LoadSettingsFromFile();
			SettingCategories = new SeedSettingCategoryInfo[]
			{
				new SeedSettingCategoryInfo {Name = "Stats",
					Description = "Settings related to player stat scaling.",
					Settings = new GameSetting[] { gameSettings.DamageRando, gameSettings.OrbXPMultiplier } },
				/* new SeedSettingCategoryInfo {Name = "Enemies",
					Description = "Settings related to enemy placement and stats.",
					Settings = new GameSetting[] { } }, */ 
				new SeedSettingCategoryInfo {Name = "Loot",
					Description = "Settings related to shop inventory and loot.",
					Settings = new GameSetting[] { gameSettings.ShopMultiplier, gameSettings.ShopFill } },
				/* new SeedSettingCategoryInfo {Name = "Sprite",
					Description =  "Settings related to sprite replacement.",
					Settings = new GameSetting[] { } }, */
				new SeedSettingCategoryInfo {Name = "Other",
					Description = "Various other settings.",
					Settings = new GameSetting[] { gameSettings.PlayerName, gameSettings.StartWithMeyef, gameSettings.StartWithJewelryBox } }
			};
		}
	}
}
