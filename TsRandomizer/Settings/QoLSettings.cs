using System;
using System.IO;
using Newtonsoft.Json;

namespace TsRandomizer
{
	public class QoLSettingsData
	{
		public bool AutoSkipCutscenes { get; set; } = true;
		public bool AutoSkipDialogue { get; set; } = true;
		public int StackCap { get; set; } = 99;
		public bool FastToastPopups { get; set; } = true;
		public bool ToastsBlockMovement { get; set; } = false;
	}

	public static class QoLSettings
	{
		static readonly string SettingsPath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory, "qol-settings.json");

		public static QoLSettingsData Current { get; private set; } = Load();

		public static QoLSettingsData Load()
		{
			try
			{
				if (File.Exists(SettingsPath))
				{
					var json = File.ReadAllText(SettingsPath);
					Current = JsonConvert.DeserializeObject<QoLSettingsData>(json) ?? new QoLSettingsData();
				}
				else
				{
					Current = new QoLSettingsData();
					Save();
				}
			}
			catch
			{
				Current = new QoLSettingsData();
			}

			return Current;
		}

		public static void Save()
		{
			try
			{
				var json = JsonConvert.SerializeObject(Current, Formatting.Indented);
				File.WriteAllText(SettingsPath, json);
			}
			catch
			{
				// ignored
			}
		}
	}
}
