using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Timespinner;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer
{
	static class DummyPlatformHelper
	{
		static IEnumerable<Process> GetTimespinnerProcesses()
		{
			string[] processesToKill = {
				"Timespinner",
				"Timespinner.bin.x86",
				"Timespinner.bin.x86_64",
				"Timespinner.bin.osx"
			};

			foreach (var processName in processesToKill)
				foreach (var process in Process.GetProcessesByName(processName))
					yield return process;
		}

		public static PlatformHelper CreateInstance()
		{
			var type = TimeSpinnerType.Get("Timespinner.PlatformHelper");

			var isStream = type.GetField("SteamAppID", BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Static) != null;
			//var isGoG = type.GetField("GogClientID", BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Static) != null;

			var helper = (PlatformHelper)Activator.CreateInstance(type, true);

			if (isStream)
				KillSteamVersion();

			return helper;
		}

		public static void KillSteamVersion()
		{
			var startTime = DateTime.UtcNow;

			try
			{
				do
				{
					Thread.Sleep(100);

					var processes = GetTimespinnerProcesses().ToArray();

					if (!processes.Any())
						continue;

					foreach (var process in processes)
						process.Kill();

					break;
				} while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(10));

			}
			catch
			{
			}
		}
	}
}
