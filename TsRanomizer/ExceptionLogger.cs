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
		static uint Seed;
		static string Versions = "";
		// ReSharper restore InconsistentNaming

		public static void SetVersionContext(string versions)
		{
			Versions = versions;
		}

		public static void SetSeedContext(uint seed)
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
				file.WriteLine($"Version: {Versions}");
				file.WriteLine($"Level: {LevelId}, Room: {RoomId}, Seed: {Seed:X8}");
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