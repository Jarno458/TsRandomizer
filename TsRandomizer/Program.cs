using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SDL2;

namespace TsRandomizer
{
	public static class Program
	{
		// ReSharper disable InconsistentNaming
		const string Win_Steam_V1_033 = "B7A81613D3B3933FB1CBF5D96E1198CD";
		const string Win_DrmFree_V1_033 = "F207565C0E364F749CCE02E4F46CF027";
		const string Win_DrmFree_GOG_Galaxy_V1_033 = "F7CDB06A80180A2E494B26438AD9362C";
		// ReSharper restore InconsistentNaming

		public static bool IsSteam;

		[STAThread]
		public static int Main()
		{
			var md5 = GetTimespinnerMd5Hash();

			switch (md5)
			{
				case Win_Steam_V1_033:
					IsSteam = true;
					break;

				case Win_DrmFree_V1_033:
				case Win_DrmFree_GOG_Galaxy_V1_033:
					IsSteam = false;
					break;

				case null:
					return -1;

				default:
					SDL.SDL_ShowSimpleMessageBox(
						SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
						"VersionMismatch",
						"TsRandomizer version missmatch!, pleaze update TsRandomizer\r\nThe installed version of TsRanodmizer is not made to work with the installed version of TimeSpinner",
						IntPtr.Zero
					);
					return -1;
			}

			StartTimeSpinner();

			return 0;
		}

		static string GetTimespinnerMd5Hash()
		{
			var md5 = MD5.Create();

			try
			{
				return ByteArrayToString(md5.ComputeHash(File.ReadAllBytes("Timespinner.exe")));
			}
			catch (FileNotFoundException)
			{
				SDL.SDL_ShowSimpleMessageBox(
					SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
					"FileNotFound",
					"Timespinner.exe not found in current directory\r\nPleaze place TsRandomizer.exe in the same folder as the original game",
					IntPtr.Zero
				);
				return null;
			}
		}

		static string ByteArrayToString(byte[] bytes)
		{
			var hex = new StringBuilder(bytes.Length * 2);
			foreach (var b in bytes)
				hex.Append(b.ToString("X2"));
			return hex.ToString();
		}

		static void StartTimeSpinner()
		{
			WithExceptionLogging(() => {
				var platformHelper = IsSteam
					? DummyPlatformHelper.CreateSteamInstance()
					: DummyPlatformHelper.CreateDrmFreeInstance();

				new TimeSpinnerGame(platformHelper).Run();
			});
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
