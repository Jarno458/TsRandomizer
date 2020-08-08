using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace TsRandomizer
{
	public static class Program
	{
		public static bool IsSteam;

		public static int Main(string[] args)
		{
			var md5 = GetTimespinnerMd5Hash();

			switch (md5)
			{
				case "93DC447605E4DF8F349B1FA66342E79A": //win steam v1.032
					IsSteam = true;
					break;

				case "2CC3F5AD830F32D9F6294E5205E61FBE": //win DRM free v1.031
					IsSteam = false;
					break;

				default:
					if (!ContinueWithoutMd5Check())
						return -1;
					else
						IsSteam = Assembly.GetExecutingAssembly().Location.Contains("steamapps");
					break;
			}

			StartTimeSpinner();

			return 0;
		}

		static bool ContinueWithoutMd5Check()
		{
			Console.Out.WriteLine("TsRandomizer version missmatch!, pleaze update TsRandomizer");
			Console.Out.WriteLine("The installed version of TsRanodmizer is not made to work with the installed version of TimeSpinner");
			Console.Out.WriteLine("If you continue the game might crash at any given point");

			do
			{
				Console.Out.Write("Do you want to continue? Y/N");

				var key = Console.ReadKey().Key;
				Console.Out.WriteLine();

				switch (key)
				{
					case ConsoleKey.Y:
						return true;
					case ConsoleKey.N:
						return false;
				}
			} while (true);
		}

		static string GetTimespinnerMd5Hash()
		{
			var md5 = MD5.Create();
			using (var fileStream = new FileStream(@"Timespinner.exe", FileMode.Open))
				return ByteArrayToString(md5.ComputeHash(fileStream));
		}

		static string ByteArrayToString(byte[] bytes)
		{
			StringBuilder hex = new StringBuilder(bytes.Length * 2);
			foreach (var b in bytes)
				hex.Append(b.ToString("X2"));
			return hex.ToString();
		}

		static void StartTimeSpinner()
		{
			Console.Out.WriteLine("Starting TimeSpinner...");

			WithExceptionLogging(() =>
			{
				var platformHelper = IsSteam
					? DummyPlatformHelper.CreateStreamInstance()
					: DummyPlatformHelper.CreateDrmFreeInstance();

				new TimeSpinnerGame(platformHelper).Run();
			});
		}

		static void WithExceptionLogging(Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				ExceptionLogger.LogException(e);
			}
		}
	}
}
