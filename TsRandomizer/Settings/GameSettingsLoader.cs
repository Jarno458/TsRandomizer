using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
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

	public static class GameSettingsLoader
	{
		const string SettingSubFolderName = "settings";

		public static SettingCollection LoadSettingsFromFile()
		{
			SettingCollection settings;

			var file = GetSettingsFilePath();

			try
			{
				if (!File.Exists(file))
				{
					settings = new SettingCollection();
				} 
				else 
				{
					var settingsString = File.ReadAllText(file);
					settings = JsonConvert.DeserializeObject<SettingCollection>(settingsString);
				}
				
				Console.WriteLine("Settings file not found: " + file);
			}
			catch
			{
				Console.WriteLine("Error loading settings from " + SettingSubFolderName);

				settings = new SettingCollection();
			}

			WriteSettings(settings); // write to file to ensure any missing settings are added with defaults

			return settings;
		}

		public static void WriteSettings(SettingCollection settings)
		{
			try
			{
				var jsonSettings = JsonConvert.SerializeObject(settings, Formatting.Indented);

				var filename = GetSettingsFilePath();

				File.WriteAllText(filename, jsonSettings);

				Console.WriteLine($"Writing settings as: {jsonSettings}");
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error writing settings: {e}");
			}
		}

		static void CreateSettingsDirectoryIfNotExists()
		{
			try
			{
				if (!Directory.Exists(SettingSubFolderName)) 
					Directory.CreateDirectory(SettingSubFolderName);
			}
			catch
			{
			}
		}

		static string GetSettingsFilePath()
		{
			CreateSettingsDirectoryIfNotExists();

			return Directory
				.EnumerateFiles(SettingSubFolderName, "*.json")
				.FirstOrDefault() ?? "settings.json";
		}
	}
}
