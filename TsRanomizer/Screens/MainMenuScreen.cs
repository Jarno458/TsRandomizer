using System.Reflection;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.MainMenu.MainMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class MainMenuScreen : Screen
	{
		public MainMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			var randomizerVersion = Assembly.GetExecutingAssembly().GetName().Version;
			Reflected._versionNumber = $"Randomizer: v{randomizerVersion}, Timespinner: {Reflected._versionNumber}";

			Reflected.RefreshSizes();
		}
	}
}
 