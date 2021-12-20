using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TsRandomizer.Extensions;
using TsRandomizer.Screens.Settings.GameSettingObjects;

namespace TsRandomizer.Screens.Settings
{
	public class GameSettingsCollection
	{
		[JsonIgnore()]
		public string Path { get; private set; }
		public OnOffGameSetting StartWithMeyef { get; set; }
		public OnOffGameSetting DamageRando { get; set; }
		public OnOffGameSetting StartWithJewelryBox { get; set; }
		public StringGameSetting PlayerName { get; set; }
		public NumberGameSetting OrbXPMultiplier { get; set; }
		public StringGameSetting ShopFill { get; set; }
		public NumberGameSetting ShopMultiplier { get; set; }
		public OnOffGameSetting ShopWarpShards { get; set; }

		public GameSettingsCollection()
		{
			string dir = "settings/";
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
			var settingsFiles = Directory.GetFiles(dir).ToList().Where(f => f.EndsWith(".json"));
			if (settingsFiles.Count() > 0)
			{
				var defaultFile = settingsFiles.First();
				Path = defaultFile;
			}
			else
			{
				Path = dir + "settings.json";
				StartWithMeyef = new OnOffGameSetting("Start with Meyef", "Start with Meyef, ideal for when you want to play multiplayer", false, false);
				DamageRando = new OnOffGameSetting("Damage Randomizer", "Adds a high chance to make orb damage very low, and a low chance to make orb damage very, very high", false, false);
				StartWithJewelryBox = new OnOffGameSetting("Start with Jewelry Box", "Start with Jewelry Box unlocked", false, false);
				PlayerName = new StringGameSetting("Player Name", "Changes all references to Lunais into the given name", "Lunais", 15, false);
				OrbXPMultiplier = new NumberGameSetting("Orb XP Multiplier", "Sets the amount of experience orbs gain per kill. Pairs well with Nightmare Lvl 1.", 1, 1, 100, 11, true, true);
				ShopFill = new StringGameSetting("Shop Inventory", "Sets the items for sale in Merchant Crow's shops. Options: [Default,Random,Vanilla,Empty]", "Default", 9, true);
				ShopMultiplier = new NumberGameSetting("Shop Price Multiplier", "Multiplier for the cost of items in the shop. Set to 0 for free shops", 1, 0, 10, 1, true, true);
				ShopWarpShards = new OnOffGameSetting("Always Sell Warp Shards", "Shops always sell warp shards (when keys possessed), ignoring fill setting.", true, true);
				WriteSettings(); //write settings file with default values
			}
		}

		public void LoadSettingsFromFile()
		{
			try
			{
				if (File.Exists(Path))
				{
					string settingsString = File.ReadAllText(Path);
					var settings = JsonConvert.DeserializeObject<GameSettingsCollection>(settingsString);
					StartWithMeyef = settings.StartWithMeyef;
					DamageRando = settings.DamageRando;
					PlayerName = settings.PlayerName;
					StartWithJewelryBox = settings.StartWithJewelryBox;
					OrbXPMultiplier = settings.OrbXPMultiplier;
					ShopFill = settings.ShopFill;
					ShopMultiplier = settings.ShopMultiplier;
					ShopWarpShards = settings.ShopWarpShards;
				}
				Console.WriteLine("Settings file not found: " + Path);
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
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
