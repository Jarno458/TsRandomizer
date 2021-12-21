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
					Settings = new GameSetting[] { gameSettings.DamageRando } },
				new GameSettingCategoryInfo {Name = "Loot",
					Description = "Settings related to shop inventory and loot.",
					Settings = new GameSetting[] { gameSettings.ShopMultiplier, gameSettings.ShopFill, gameSettings.ShopWarpShards } },
			};
		}
	}
}
