using System;
using System.Diagnostics;
using System.Threading;
using Timespinner;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer
{
	static class DummyPlatformHelper
	{
		public static PlatformHelper CreateStreamInstance()
		{
			var platformHelper = (PlatformHelper)Activator.CreateInstance(TimeSpinnerType.Get("Timespinner.PlatformHelper"), true);

			while ((Process.GetProcessesByName("Timespinner").Length == 0) && (Process.GetProcessesByName("Timespinner.bin.x86_64").Length == 0) && (Process.GetProcessesByName("Timespinner.bin.osx").Length == 0))
				Thread.Sleep(100);

			foreach (var process in Process.GetProcessesByName("Timespinner"))
				process.Kill();

			foreach (var process in Process.GetProcessesByName("Timespinner.bin.x86_64"))
				process.Kill();

			foreach (var process in Process.GetProcessesByName("Timespinner.bin.osx"))
				process.Kill();

			return platformHelper;
		}

		public static PlatformHelper CreateDrmFreeInstance()
		{
			return (PlatformHelper)Activator.CreateInstance(TimeSpinnerType.Get("Timespinner.PlatformHelper"), true);
		}
	}
}
