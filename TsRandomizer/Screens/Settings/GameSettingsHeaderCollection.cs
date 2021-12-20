namespace TsRandomizer.Screens.Settings
{
	class GameSettingsCategoryCollection
	{
		public GameSettingCategoryInfo[] SettingCategories;
		public GameSettingsCategoryCollection()
		{
			GameSettingsCollection gameSettings = new GameSettingsCollection();
			gameSettings.LoadSettingsFromFile();
			SettingCategories = new GameSettingCategoryInfo[]
			{
				new GameSettingCategoryInfo {Name = "Stats",
					Description = "Settings related to player stat scaling.",
					Settings = new GameSetting[] { gameSettings.DamageRando, gameSettings.OrbXPMultiplier } },
				/* new GameSettingCategoryInfo {Name = "Enemies",
					Description = "Settings related to enemy placement and stats.",
					Settings = new GameSetting[] { } }, */ 
				new GameSettingCategoryInfo {Name = "Loot",
					Description = "Settings related to shop inventory and loot.",
					Settings = new GameSetting[] { gameSettings.ShopMultiplier, gameSettings.ShopFill, gameSettings.ShopWarpShards } },
				/* new GameSettingCategoryInfo {Name = "Sprite",
					Description =  "Settings related to sprite replacement.",
					Settings = new GameSetting[] { } }, */
				new GameSettingCategoryInfo {Name = "Other",
					Description = "Various other settings.",
					Settings = new GameSetting[] { gameSettings.PlayerName, gameSettings.StartWithMeyef, gameSettings.StartWithJewelryBox } }
			};
		}
	}
}
