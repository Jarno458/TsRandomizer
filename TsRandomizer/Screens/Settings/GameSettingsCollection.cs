using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TsRandomizer.Screens.Settings.GameSettingObjects;

namespace TsRandomizer.Screens.Settings
{
	class GameSettingsCollection
	{
		private const string path = "/settings/";
		[JsonIgnore()]
		public string PresetName { get; set; }
		public OnOffGameSetting StartWithMeyef = new OnOffGameSetting("Start with Meyef", "Start with Meyef, ideal for when you want to play multiplayer", false);
		public OnOffGameSetting DamageRando = new OnOffGameSetting("Damage Randomizer", "Adds a high chance to make orb damage very low, and a low chance to make orb damage very, very high", false);
		public StringGameSetting PlayerName = new StringGameSetting("Player Name", "Changes all references to Lunais into the given name", "Lunais", 15);
		public OnOffGameSetting StartWithJewelryBox = new OnOffGameSetting("Start with Jewelry Box", "Start with Jewelry Box unlocked", false);
		public NumberGameSetting OrbXpMultiplier = new NumberGameSetting("Orb XP Multiplier", "Multiplies orb XP gained by the given number", 1, 1, 100, false);

		public GameSettingsCollection()
		{
			var settingsFiles = Directory.GetFiles(path).ToList().Where(f => f.EndsWith(".json"));
			if (settingsFiles.Count() > 0)
			{
				var defaultFile = settingsFiles.First();
				GameSettingsCollection settings = LoadSettingsFromFile(defaultFile);
				PresetName = defaultFile.Substring(0, defaultFile.Length - 6);
				StartWithMeyef = settings.StartWithMeyef;
				DamageRando = settings.DamageRando;
				PlayerName = settings.PlayerName;
				StartWithJewelryBox = settings.StartWithJewelryBox;
				OrbXpMultiplier = settings.OrbXpMultiplier;
			}
		}

		private GameSettingsCollection LoadSettingsFromFile(string fileName)
		{
			try
			{
				if (File.Exists(fileName))
				{
					string settingsString = File.ReadAllText(fileName);
					GameSettingsCollection settings = JsonConvert.DeserializeObject<GameSettingsCollection>(settingsString);
					return settings;
				}
				Console.WriteLine("Settings file not found: " + fileName);
				return new GameSettingsCollection();
			}
			catch
			{
				Console.WriteLine("Error loading settings from " + fileName);
				return new GameSettingsCollection();
			}
		}

		public GameSettingsCollection LoadSettings(string presetName)
		{
			try
			{
				var settingsFromFile = LoadSettingsFromFile($"{path}{presetName}.json");
				settingsFromFile.PresetName = presetName;
				return settingsFromFile;
			}
			catch
			{
				Console.WriteLine("Error loading settings for " + presetName);
				return new GameSettingsCollection();
			}
		}

		public bool WriteSettings(string presetName, GameSettingsCollection settings)
		{
			try
			{
				string jsonSettings = JsonConvert.SerializeObject(settings);
				File.WriteAllText($"{path}{presetName}.json", jsonSettings);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
