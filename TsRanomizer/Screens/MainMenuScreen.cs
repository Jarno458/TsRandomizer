using System.Reflection;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
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
			var newVersionString = $"Randomizer: v{randomizerVersion}, Timespinner: {Reflected._versionNumber}";

			ExceptionLogger.SetVersionContext(newVersionString);

			Reflected._versionNumber = newVersionString;
			Reflected.RefreshSizes();
		}
	}
}
 