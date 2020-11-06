using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace TsRandomizer
{
	public static class Program
	{
		public static bool IsSteam;

		[STAThread]
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
					MessageBox.Show("TsRandomizer version missmatch!, pleaze update TsRandomizer\r\nThe installed version of TsRanodmizer is not made to work with the installed version of TimeSpinner", "VersionMissmatch");
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
				MessageBox.Show("Timespinner.exe not found in current directory\r\nPleaze place TsRandomizer.exe in the same folder as the original game", "FileNotFound");
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
