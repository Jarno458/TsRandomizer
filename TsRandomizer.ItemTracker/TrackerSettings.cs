using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace TsRandomizerItemTracker
{
	public class TrackerSettings
	{
		const string SettingFileName = "TsRandomizerItemTrackerSettings.xml";

		int iconSize;
		public int IconSize
		{
			get => iconSize;
			set
			{
				iconSize = value;
				SaveSettings();
			}
		}

		int backgrondIndex;
		public int BackgrondIndex
		{
			get => backgrondIndex;
			set
			{
				backgrondIndex = value;
				SaveSettings();
			}
		}

		bool borderless;
		public bool Borderless
		{
			get => borderless;
			set
			{
				borderless = value;
				SaveSettings();
			}
		}

		Point windowSize;
		public Point WindowSize
		{
			get => windowSize;
			set
			{
				windowSize = value;
				SaveSettings();
			}
		}

		bool savingEnabled = false;

		public static TrackerSettings LoadSettings()
		{
			var defaultSettings = new TrackerSettings
			{
				iconSize = 32,
				backgrondIndex = 0,
				borderless = false,
				windowSize = new Point(160, 128),
			};

			if (!File.Exists(SettingFileName))
				return defaultSettings;

			try
			{
				var serializer = new XmlSerializer(typeof(TrackerSettings));

				using (var reader = new StreamReader(SettingFileName))
					return (TrackerSettings)serializer.Deserialize(reader);
			}
			catch (Exception)
			{
				return defaultSettings;
			}
		}

		public void EnableSaving()
		{
			savingEnabled = true;
		}

		void SaveSettings()
		{
			if(!savingEnabled)
				return;

			var serializer = new XmlSerializer(typeof(TrackerSettings));

			using (var writer = new StreamWriter(SettingFileName, false))
				serializer.Serialize(writer, this);
		}
	}
}
