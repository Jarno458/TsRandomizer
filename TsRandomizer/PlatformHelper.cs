using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Timespinner;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer
{
	static class DummyPlatformHelper
	{
		public static PlatformHelper CreateSteamInstance()
		{
			var platformHelper = (PlatformHelper)Activator.CreateInstance(TimeSpinnerType.Get("Timespinner.PlatformHelper"), true);

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

			return platformHelper;
		}

		public static PlatformHelper CreateDrmFreeInstance() => 
			(PlatformHelper)Activator.CreateInstance(TimeSpinnerType.Get("Timespinner.PlatformHelper"), true);

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
	}
}
