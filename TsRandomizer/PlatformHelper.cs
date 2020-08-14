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

			while (Process.GetProcessesByName("Timespinner").Length == 0)
				Thread.Sleep(100);

			foreach (var process in Process.GetProcessesByName("Timespinner"))
				process.Kill();

			return platformHelper;
		}

		public static PlatformHelper CreateDrmFreeInstance()
		{
			return (PlatformHelper)Activator.CreateInstance(TimeSpinnerType.Get("Timespinner.PlatformHelper"), true);
		}
	}
}
