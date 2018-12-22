using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TsRanodmizer
{
	public static class Program
	{
		/*
		TODO
		Remove TODO's
		Fix path calculation
		Fix spawnning of orb pedistals
		Add item location of orb pedistals
		Add ItemList
		Fix Save game menu seeeeeeeds display
		Improve item location map with lab
		*/

		public static int Main(string[] args)
		{
			if (!CheckArguments(args))
				return -1;

			//TODO: re-enable
			//if (!Md5CheckPassed("A2F880953099610FACF4E3CC153085E1"))
			//	return -1;

			StartTimeSpinner();

			return 0;
		}

		static bool CheckArguments(string[] args)
		{
			if (args.Length > 1)
			{
				Console.Out.WriteLine("Invalid arguments, Usage TsRanodmizer.exe [hex seed]");
				Console.ReadKey(true);
				return false;
			}

			if (args.Length == 1)
			{
				if (Seed.TrySetFromText(args[0]))
					return true;

				Console.Out.WriteLine("Invalid arguments, Usage TsRanodmizer.exe [hex seed]");
				Console.ReadKey(true);
				return false;
			}

			return true;
		}

		static bool Md5CheckPassed(string knowhash)
		{
			if (knowhash == GetTimespinnerMd5Hash())
				return true;

			Console.Out.WriteLine("TsRanodmizer version missmatch!, pleaze update TsRanodmizer");
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

			/*try
			{*/
				new TimeSpinnerGame(DummyPlatformHelper.CreateInstance()).Run();
			/*}
			catch (Exception e)
			{
				Console.Error.WriteLine($"Exeception of type {e.GetType()} occured:");
				Console.Error.WriteLine(e.Message);
				Console.Error.WriteLine(e.StackTrace);

				Console.ReadKey(true);
			}*/
		}
	}
}
