using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TsRandomizer.Screens.Settings
{
	class GameSettingsCollection
	{
		private const string path = "/settings/";
		[JsonIgnore()]
		public string PresetName { get; set; }
		public bool StartWithMeyef { get; set; }
		public bool DamageRando { get; set; }
		public string PlayerName { get; set; }

		public GameSettingsCollection()
		{
			var settingsFiles = Directory.GetFiles(path).ToList().Where(f => f.EndsWith(".json"));
			if(settingsFiles.Count() > 0)
			{
				var defaultFile = settingsFiles.First();
				GameSettingsCollection settings = LoadSettingsFromFile(defaultFile);
				PresetName = defaultFile.Substring(0, defaultFile.Length - 6);
				StartWithMeyef = settings.StartWithMeyef;
				DamageRando = settings.DamageRando;
				PlayerName = settings.PlayerName;
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
