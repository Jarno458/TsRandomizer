using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TsRandomizer.Screens.Settings;

namespace TsRandomizer.Extensions
{
	public static class SettingsExtensions
	{
		public static bool WriteSettings(this GameSettingsCollection settings)
		{
			try
			{
				string jsonSettings = JsonConvert.SerializeObject(settings, Formatting.Indented);
				File.WriteAllText($"{settings.Path}settings.json", jsonSettings);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
