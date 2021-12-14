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
		public string Path { get; private set; }
		public OnOffGameSetting StartWithMeyef = new OnOffGameSetting("Start with Meyef", "Start with Meyef, ideal for when you want to play multiplayer", false, false);
		public OnOffGameSetting DamageRando = new OnOffGameSetting("Damage Randomizer", "Adds a high chance to make orb damage very low, and a low chance to make orb damage very, very high", false, true);
		public OnOffGameSetting StartWithJewelryBox = new OnOffGameSetting("Start with Jewelry Box", "Start with Jewelry Box unlocked", false, false);
		public StringGameSetting PlayerName = new StringGameSetting("Player Name", "Changes all references to Lunais into the given name", "Lunais", 15, false);
		public NumberGameSetting OrbXpMultiplier = new NumberGameSetting("Orb XP Multiplier", "Multiplies orb XP gained by the given number", 1, 1, 100, true, true);

		public GameSettingsCollection(string path)
		{
			Path = path;
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
			var settingsFiles = Directory.GetFiles(path).ToList().Where(f => f.EndsWith(".json"));
			if (settingsFiles.Count() > 0)
			{
				var defaultFile = settingsFiles.First();
				GameSettingsCollection settings = LoadSettingsFromFile(defaultFile);
				StartWithMeyef = settings.StartWithMeyef;
				DamageRando = settings.DamageRando;
				PlayerName = settings.PlayerName;
				StartWithJewelryBox = settings.StartWithJewelryBox;
				OrbXpMultiplier = settings.OrbXpMultiplier;
			}
			else
			{
				GameSettingsCollection settings = new GameSettingsCollection();
				settings.WriteSettings();
			}
		}

		public GameSettingsCollection() { }

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

		public bool WriteSettings()
		{
			try
			{
				string jsonSettings = JsonConvert.SerializeObject(this, Formatting.Indented);
				File.WriteAllText($"{Path}settings.json", jsonSettings);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
