using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.MainMenu.MainMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class MainMenuScreen : Screen
	{
		HashSet<string> knownMd5Hashes = new HashSet<string>() {
			"B7A81613D3B3933FB1CBF5D96E1198CD", //Win_Steam_V1_033
			"F207565C0E364F749CCE02E4F46CF027", //Win_DrmFree_V1_033
			"F7CDB06A80180A2E494B26438AD9362C" //Win_DrmFree_GOG_Galaxy_V1_033
		};

		public MainMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			Client.Disconnect();

			var randomizerVersion = Assembly.GetExecutingAssembly().GetName().Version;
			var newVersionString = $"TsRandomizer: {GetRandoVersion(randomizerVersion)}, Timespinner: {GetVanillaVersion()}";

			new Localizer().ResetStrings();
			
			Dynamic._versionNumber = newVersionString;
			Dynamic.RefreshSizes();
		}

		static string GetRandoVersion(Version v) =>
			$"v{v.Major}.{v.Minor}.{v.Build}";

		string GetVanillaVersion()
		{
			string version = Dynamic._versionNumber;

			var md5 = GetTimespinnerMd5Hash();

			if (!knownMd5Hashes.Contains(md5))
				version += " [MODIFIED]";

			return version;
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
				return "";
			}
		}
		static string ByteArrayToString(byte[] bytes)
		{
			var hex = new StringBuilder(bytes.Length * 2);
			foreach (var b in bytes)
				hex.Append(b.ToString("X2"));
			return hex.ToString();
		}
	}
}
 