using System.Collections;
using Microsoft.Xna.Framework;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using ScreenManager = TsRanodmizer.OverloadedObjects.ScreenManager;

namespace TsRanodmizer.Screens
{
	class MainMenuScreen : Screen
	{
		readonly ScreenManager screenManager;

		public MainMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screen)
		{
			this.screenManager = screenManager;
			var entry = MenuEntry.Create("Select Seed", SelectSeed);
			var menuEntries = (IList)screen.Reflect().MenuEntries;
			menuEntries.Insert(0, entry.Entry);
		}

		void SelectSeed(PlayerIndex pi)
		{
			var selectSeedMenu = SeedSelectionMenuScreen.Create(screenManager);

			screenManager.AddScreen(selectSeedMenu.Screen, PlayerIndex.One);
		}
	}
}