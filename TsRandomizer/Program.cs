using System;

namespace TsRandomizer
{
	public static class Program
	{
		[STAThread]
		public static int Main()
		{
			WithExceptionLogging(() => {
				var platformHelper = DummyPlatformHelper.CreateInstance();

				new TimeSpinnerGame(platformHelper).Run();
			});

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
