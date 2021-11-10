using System;
using System.IO;
using System.Reflection;

namespace TsRandomizer
{
	static class ExceptionLogger
	{
		// ReSharper disable InconsistentNaming
		static int LevelId;
		static int RoomId;
		static string Seed;
		// ReSharper restore InconsistentNaming

		public static void SetSeedContext(string seed)
		{
			Seed = seed;
		}

		public static void SetLevelContext(int levelId, int roomId)
		{
			LevelId = levelId;
			RoomId = roomId;
		}

		public static void LogException(Exception exception)
		{
			using (var file = new StreamWriter(GetFileName()))
			{
				file.WriteLine("Context:");
				file.WriteLine($"Timespinner Version: {TimeSpinnerGame.Constants.GameVersion}, TsRandomizer Version: {Assembly.GetExecutingAssembly().GetName().Version}");
				file.WriteLine($"Level: {LevelId}, Room: {RoomId}, Seed: {Seed}");
				file.WriteLine();
				file.WriteLine("Exceptions:");

				while (exception != null)
				{
					file.WriteLine($"Exception: {exception.Message}");
					file.WriteLine(exception.StackTrace);
					file.WriteLine();

					exception = exception.InnerException;
				}
			}
		}

		static string GetFileName()
		{
			var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var fileDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH.mm");

			// ReSharper disable once AssignNullToNotNullAttribute
			return Path.Combine(directory, $"TsRandomizer {fileDateTime}.txt");
		}
	}
}