﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TsRandomizer.Settings
{
	public static class GameSettingsLoader
	{
		const string SaveFileSettingKey = "TSRandomizerGameSettings";

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

					settings = FromJson(settingsString);
				}
				
				Console.WriteLine("Settings file not found: " + file);
			}
			catch
			{
				Console.WriteLine("Error loading settings from " + SettingSubFolderName);

				settings = new SettingCollection();
			}

			WriteSettingsToFile(settings); // write to file to ensure any missing settings are added with defaults

			return settings;
		}

		public static void WriteSettingsToFile(SettingCollection settings)
		{
			try
			{
				var jsonSettings = ToJson(settings, true);

				var filename = GetSettingsFilePath();

				File.WriteAllText(filename, jsonSettings);

				Console.WriteLine($"Writing settings as: {jsonSettings}");
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error writing settings: {e}");
			}
		}

		public static SettingCollection LoadSettingsFromSlotData(Dictionary<string, object> slotData)
		{
			var settings = LoadSettingsFromFile();

			if (slotData.TryGetValue("DamageRando", out var value) && IsTrue(value))
				settings.DamageRando.SetValue(true);

			return settings;
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


		internal static SettingCollection FromJson(string json)
		{
			try
			{
				var settings = new SettingCollection();
				JsonConvert.PopulateObject(json, settings, new JsonSerializerSettings
				{
					ContractResolver = new JsonSettingsContractResolver()
				});

				return settings;
			}
			catch
			{
				Console.WriteLine("Error loading settings from " + json);

				return new SettingCollection();
			}
		}

		internal static string ToJson(SettingCollection settings, bool intended) =>
			JsonConvert.SerializeObject(settings, intended ? Formatting.Indented : Formatting.None);

		static bool IsTrue(object o)
		{
			if (o is bool b) return b;
			if (o is string s) return bool.Parse(s);
			if (o is int i) return i > 0;
			if (o is long l) return l > 0;

			return false;
		}
	}
}