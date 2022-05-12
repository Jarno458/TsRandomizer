using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TsRandomizer.Randomisation;

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
					Console.WriteLine("Settings file not found: " + file);
				}
				else
				{
					var settingsString = File.ReadAllText(file);

					settings = FromJson(settingsString);
				}

			}
			catch
			{
				Console.WriteLine("Error loading settings from " + SettingSubFolderName);
				BackupMalformedSettingsFile();
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

		public static void BackupMalformedSettingsFile()
		{
			try
			{
				File.Copy(GetSettingsFilePath(), $"{SettingSubFolderName}/settings_ERROR.json");
			}
			catch
			{
				// tough luck man idk
			}

		}

		public static SettingCollection LoadSettingsFromSlotData(Dictionary<string, object> slotData)
		{
			var settings = LoadSettingsFromFile();

			if (slotData.TryGetValue("ShopWarpShards", out var shopWarpShards))
				settings.ShopWarpShards.Value = IsTrue(shopWarpShards);
			if (slotData.TryGetValue("ShopMultiplier", out var shopMultiplier))
				settings.ShopMultiplier.Value = ToInt(shopMultiplier, 1);
			if (slotData.TryGetValue("ShopFill", out var shopFill))
			{
				var value = ToInt(shopFill);
				string enumValue;

				switch (value)
				{
					case 1:
						enumValue = "Random";
						break;

					case 2:
						enumValue = "Vanilla";
						break;

					case 3:
						enumValue = "Empty";
						break;

					default:
						enumValue = "Default";
						break;
				}

				settings.ShopFill.Value = enumValue;
			}
			if (slotData.TryGetValue("DamageRando", out var damageRando))
			{
				var value = ToInt(damageRando);
				string enumValue;

				switch (value)
				{
					case 1:
						enumValue = "All Nerfs";
						break;

					case 2:
						enumValue = "Mostly Nerfs";
						break;

					case 3:
						enumValue = "Balanced";
						break;

					case 4:
						enumValue = "Mostly Buffs";
						break;

					case 5:
						enumValue = "All Buffs";
						break;

					case 6:
						enumValue = "Manual";
						break;

					default:
						enumValue = "Off";
						break;
				}
				settings.DamageRando.Value = enumValue;
			}

			if (settings.DamageRando.Value != "Off"
				&& slotData.TryGetValue("DamageRandoOverrides", out var damageRandoOverrides))
			{
				Dictionary<string, OrbDamageOdds> overrides = new Dictionary<string, OrbDamageOdds>();
				JsonConvert.PopulateObject(damageRandoOverrides.ToString(), overrides);
				settings.DamageRandoOverrides.Value = FixOrbNames(overrides);
			}
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
				BackupMalformedSettingsFile();
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

		static int ToInt(object o, int fallback = 0)
		{
			if (o is int i) return i;
			if (o is long l) return (int)l;

			return fallback;
		}

		static Dictionary<string, OrbDamageOdds> FixOrbNames(Dictionary<string, OrbDamageOdds> orbs)
		{
			Dictionary<string, OrbDamageOdds> namingFixes = new Dictionary<string, OrbDamageOdds>();
			foreach (var orb in orbs)
			{
				switch (orb.Key)
				{
					case "Plasma":
						namingFixes.Add("Pink", orb.Value);
						break;
					case "Fire":
						namingFixes.Add("Flame", orb.Value);
						break;
					case "ForbiddenTome":
					case "Forbidden Tome":
					case "Forbidden":
						namingFixes.Add("Book", orb.Value);
						break;
					case "Shattered":
						namingFixes.Add("Moon", orb.Value);
						break;
					case "Radiant":
						namingFixes.Add("Barrier", orb.Value);
						break;
				}
			}
			orbs.Remove("Plasma");
			orbs.Remove("Fire");
			orbs.Remove("ForbiddenTome");
			orbs.Remove("Forbidden Tome");
			orbs.Remove("Forbidden");
			orbs.Remove("Shattered");
			orbs.Remove("Radiant");
			orbs = orbs.Keys
				.Union(namingFixes.Keys)
				.ToDictionary(
					orb => orb,
					orb => namingFixes.ContainsKey(orb) ? namingFixes[orb] : orbs[orb]
				);
			return orbs;
		}
	}
}
