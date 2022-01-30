using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.OptionsMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class OptionsMenuScreen : Screen
	{
		static readonly Type MainMenuEntryType = TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntry");

		public OptionsMenuScreen(ScreenManager screenManager, GameScreen gameScreen) : base(screenManager, gameScreen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			AddSettingButton(MenuEntry.Create("Settings", OpenSettingsMenu));
		}

		void AddSettingButton(MenuEntry settingButton)
		{
			settingButton.IsCenterAligned = false;
			settingButton.DoesDrawLargeShadow = false;

			var buttons = (IList)((object)Dynamic._primaryMenuCollection).AsDynamic()._entries;
			buttons.Add(settingButton.AsTimeSpinnerMenuEntry());

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = buttons.ToList(MainMenuEntryType);
		}

		void OpenSettingsMenu(PlayerIndex playerIndex)
		{
			var gameSettingsMenu = GameSettingsScreen.Create(ScreenManager);

			ScreenManager.AddScreen(gameSettingsMenu, playerIndex);
		}
	}
}
