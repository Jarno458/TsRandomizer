using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Threading;
using Timespinner;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer
{
	static class DummyPlatformHelper
	{
		public static PlatformHelper CreateInstance()
		{
			var platformHelper = (PlatformHelper)Activator.CreateInstance(TimeSpinnerType.Get("Timespinner.PlatformHelper"), true);

			while (Process.GetProcessesByName("Timespinner").Length == 0)
				Thread.Sleep(100);

			foreach (var process in Process.GetProcessesByName("Timespinner"))
				process.Kill();

			return platformHelper;
		}
	}
}
