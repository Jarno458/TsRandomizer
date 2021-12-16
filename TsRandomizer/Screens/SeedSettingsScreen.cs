using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens.Menu;

using System;


namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.JournalMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class SeedSettingsScreen : Screen
	{
		public SeedSettingsScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			Dynamic._menuTitle = "Seed Settings";

			var emptyMenuEntryList = new object[0]
				.ToList(TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntry"));
			var emptyMenuCollectionList = new object[0]
				.ToList(TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntryCollection"));

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = emptyMenuEntryList;
			Dynamic._subMenuCollections = emptyMenuCollectionList;
		}
	}
}
