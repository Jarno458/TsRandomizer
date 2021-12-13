using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer.Screens.Settings
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.OptionsMenuScreen")]
	class GameSettingsMenuScreen : Screen
	{
		List<GameSetting> settings;

		readonly GameDifficultyMenuScreen difficultyMenu;

		bool IsUsedAsSettingsMenu => difficultyMenu != null;
		public GameSettingsMenuScreen(ScreenManager screenManager, GameScreen videoScreen) : base(screenManager, videoScreen)
		{
			difficultyMenu = screenManager.FirstOrDefault<GameDifficultyMenuScreen>();
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			if (!IsUsedAsSettingsMenu) return;

			Dynamic._menuTitle = "Settings";
			MenuEntry[] menuEntries = new MenuEntry[settings.Count];
			for(int s = 0; s < settings.Count; s++)
			{
				var menuEntry = MenuEntry.Create($"{settings[s].Name}: ", _ => { });
				menuEntries[s] = menuEntry;
			}
		}
	}
}
