using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TsRandomizer.Screens.Settings.GameSettingObjects;

namespace TsRandomizer.Screens.Settings
{
	public class SettingsConstants
	{
		public string Name;
		public string Description;
		public object DefaultValue;
	}

	public class NumberSettingsConstants : SettingsConstants
	{
		public double MinimumValue;
		public double MaximumValue;
		public double StepValue;
		public bool AllowDecimals;
	}

	public class SpecificValueSettingsConstants : SettingsConstants
	{
		public List<string> AllowedValues;
	}

	public class GameSettingsCollection
	{
		static readonly SettingsConstants DamageRandoConstants = new SettingsConstants
		{
			Name = "Damage Randomizer",
			Description = "Adds a high chance to make orb damage very low, and a low chance to make orb damage very, very high",
			DefaultValue = false
		};

		static readonly SpecificValueSettingsConstants ShopFillConstants = new SpecificValueSettingsConstants
		{
			Name = "Shop Inventory",
			Description = "Sets the items for sale in Merchant Crow's shops. Options: [Default,Random,Vanilla,Empty]",
			DefaultValue = "Default",
			AllowedValues = new List<string> { "Default", "Random", "Vanilla", "Empty" }
		};

		static readonly NumberSettingsConstants ShopMultiplierConstants = new NumberSettingsConstants
		{
			Name = "Shop Price Multiplier",
			Description = "Multiplier for the cost of items in the shop. Set to 0 for free shops",
			DefaultValue = 1,
			StepValue = 0.5,
			MinimumValue = 0,
			MaximumValue = 10,
			AllowDecimals = true
		};

		static readonly SettingsConstants ShopWarpShardsConstants = new SettingsConstants
		{
			Name = "Always Sell Warp Shards",
			Description = "Shops always sell warp shards (when keys possessed), ignoring inventory setting.",
			DefaultValue = true
		};

		[JsonIgnore]
		public string Path { get; }
		public OnOffGameSetting DamageRando { get; set; }
		public SpecificValuesGameSetting ShopFill { get; set; }
		public NumberGameSetting ShopMultiplier { get; set; }
		public OnOffGameSetting ShopWarpShards { get; set; }

		public GameSettingsCollection()
		{
			string dir = "settings/";
			if (CreateSettingsDirectory(dir))
			{
				var settingsFiles = GetSettingsFiles(dir);
				if (settingsFiles == null)
				{
					Console.WriteLine("Error reading from settings directory.");
					return;
				}
				if (settingsFiles.Count() > 0)
				{
					var defaultFile = settingsFiles.First();
					Path = defaultFile;
				}
				else
				{
					Console.WriteLine("No settings file found. Creating...");
					Path = dir + "settings.json";

					DamageRando = new OnOffGameSetting(DamageRandoConstants.Name, DamageRandoConstants.Description, false, false);
					ShopFill = new SpecificValuesGameSetting(ShopFillConstants.Name, ShopFillConstants.Description, "Default", ShopFillConstants.AllowedValues, false);
					ShopMultiplier = new NumberGameSetting(ShopMultiplierConstants.Name, ShopMultiplierConstants.Description, 1, 0, 10, 1, true, true);
					ShopWarpShards = new OnOffGameSetting(ShopWarpShardsConstants.Name, ShopWarpShardsConstants.Description, true, true);

					WriteSettings(); //write settings file with default values
				}
			}
			else
			{
				Console.WriteLine("Error finding or creating settings directory.");
			}
		}

		public void LoadSettingsFromFile()
		{
			try
			{
				if (File.Exists(Path))
				{
					var settingsString = File.ReadAllText(Path);
					var settings = JsonConvert.DeserializeObject<GameSettingsCollection>(settingsString);

					DamageRando = new OnOffGameSetting(DamageRandoConstants, settings.DamageRando);
					ShopFill = new SpecificValuesGameSetting(ShopFillConstants, settings.ShopFill);
					ShopMultiplier = new NumberGameSetting(ShopMultiplierConstants, settings.ShopMultiplier);
					ShopWarpShards = new OnOffGameSetting(ShopWarpShardsConstants, settings.ShopWarpShards);

					WriteSettings(); // write to file to ensure any missing settings are added with defaults
				}
				else
				{
					Console.WriteLine("Settings file not found: " + Path);
				}
			}
			catch
			{
				Console.WriteLine("Error loading settings from " + Path);
			}
		}

		public bool WriteSettings()
		{
			try
			{
				string jsonSettings = JsonConvert.SerializeObject(this, Formatting.Indented);

				File.WriteAllText(Path, jsonSettings);

				Console.WriteLine($"Writing settings for: {this}");
				Console.WriteLine($"Writing settings as: {jsonSettings}");

				return true;
			}
			catch (Exception exc)
			{
				Console.WriteLine($"Error writing settings: {exc}");

				return false;
			}
		}

		public bool CreateSettingsDirectory(string dir)
		{
			try
			{
				if (!Directory.Exists(dir)) 
					Directory.CreateDirectory(dir);

				return true;
			}
			catch
			{
				return false;
			}
		}

		public IEnumerable<string> GetSettingsFiles(string dir)
		{
			try
			{
				return Directory.GetFiles(dir).ToList().Where(f => f.EndsWith(".json"));
			}
			catch
			{
				return null;
			}
		}
	}
}
