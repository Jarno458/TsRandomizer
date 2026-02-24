using System;
using HarmonyLib;

namespace TsRandomizer
{
	public static class Program
	{
		[STAThread]
		public static int Main()
		{
			WithExceptionLogging(() => {

				var harmony = new Harmony("com.tsrandomizer.mods");
				harmony.PatchAll();
				var platformHelper = DummyPlatformHelper.CreateInstance();

				new TimeSpinnerGame(platformHelper).Run();
			});

			Environment.Exit(0);
			return 0;
		}

		static void WithExceptionLogging(Action action)
		{
#if DEBUG
			action();
#else
			try
			{
				action();
			}
			catch (Exception e)
			{
				ExceptionLogger.LogException(e);
			}
#endif
		}
	}
}
