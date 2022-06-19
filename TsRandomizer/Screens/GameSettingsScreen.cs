using System;
using System.Collections;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;
using TsRandomizer.Screens.SeedSelection;
using TsRandomizer.Settings;
using TsRandomizer.Settings.GameSettingObjects;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.JournalMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class GameSettingsScreen : Screen
	{
		static readonly Type MenuEntryType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntry");
		static readonly Type JournalMenuType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.PauseMenu.JournalMenuScreen");

		readonly SeedSelectionMenuScreen seedSelectionScreen;
		readonly OptionsMenuScreen optionsMenuScreen;

		GameSave save;
		SettingCollection settings;
		GCM gcm;

		bool IsUsedAsGameSettingsMenu => seedSelectionScreen != null || optionsMenuScreen != null;
		bool IsInGame => save != null;

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			gcm = gameContentManager;

			if (!IsUsedAsGameSettingsMenu)
				return;

			Dynamic._menuTitle = "Game Settings";

			var gameplayScreen = ScreenManager.FirstOrDefault<GameplayScreen>();
			save = gameplayScreen?.Save;

			settings = IsInGame
				? gameplayScreen.Settings
				: GameSettingsLoader.LoadSettingsFromFile();

			ResetMenu();
		}

		public override void Unload()
		{
			try
			{
				if (settings != null)
					gcm.UpdateMinimapColors(settings);
			}
			catch
			{
				// ignored
			}
		}

		public GameSettingsScreen(ScreenManager screenManager, GameScreen passwordMenuScreen) : base(screenManager, passwordMenuScreen)
		{
			seedSelectionScreen = screenManager.FirstOrDefault<SeedSelectionMenuScreen>();
			optionsMenuScreen = screenManager.FirstOrDefault<OptionsMenuScreen>();
		}

		public static GameScreen Create(ScreenManager screenManager)
		{
			GCM gcm = screenManager.AsDynamic().GCM;

			void Noop()
			{
				var gameplayScreen = screenManager.FirstOrDefault<GameplayScreen>();

				if(gameplayScreen != null && gameplayScreen.Settings != null)
					gcm.UpdateMinimapColors(gameplayScreen.Settings);
			}

			gcm.LoadAllResources(screenManager.AsDynamic().GeneralContentManager, screenManager.GraphicsDevice);

			return (GameScreen)Activator.CreateInstance(JournalMenuType, GameSave.DemoSave, gcm, (Action)Noop);
		}

		object CreateDefaultsMenu(GameSettingCategoryInfo[] menusToClear, bool isSubmenu)
		{
			var defaultsMenu = MenuEntry.Create("Defaults", () => OnDefaultsSelected(menusToClear, isSubmenu)).AsTimeSpinnerMenuEntry();

			string menuDescription = isSubmenu ? menusToClear[0].Name : "all";
			defaultsMenu.AsDynamic().Description = $"Restore {menuDescription} settings to their defaults";
			defaultsMenu.AsDynamic().IsCenterAligned = false;

			return defaultsMenu;
		}

		void OnDefaultsSelected(GameSettingCategoryInfo[] menusToClear, bool isSubmenu)
		{
			// Clear the root menu
			if (!IsInGame && !isSubmenu)
			{
				settings = new SettingCollection();
			}
			else
			{
				foreach (var category in menusToClear)
				{
					foreach (var settingsFunc in category.SettingsPerCategory)
					{
						var setting = settingsFunc(settings);

						if(IsInGame && !setting.CanBeChangedInGame)
							continue;

						setting.SetDefault();
					}
				}
			}

			if (IsInGame)
				save.SetSettings(settings);
			else
				GameSettingsLoader.WriteSettingsToFile(settings);

			ResetMenu();
			if (isSubmenu)
				Dynamic.GoToPreviousMenuCollection();
		}

		void ResetMenu()
		{
			ClearAllSubmenus();

			var menuEntryList = new object[0].ToList(MenuEntryType);

			foreach (var category in SettingCollection.Categories)
			{
				var submenu = MenuEntry.Create(category.Name, () => CreateMenuForCategory(category));

				submenu.AsDynamic().Description = category.Description;
				submenu.AsDynamic().IsCenterAligned = false;

				menuEntryList.Add(submenu.AsTimeSpinnerMenuEntry());
			}

			menuEntryList.Add(CreateDefaultsMenu(SettingCollection.Categories, false));

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = menuEntryList;
			((object)Dynamic._selectedMenuCollection).AsDynamic().SetSelectedIndex(0);
		}

		void ClearAllSubmenus()
		{
			var menuEntryList = new object[0].ToList(MenuEntryType);

			((object)Dynamic._memoriesInventoryCollection).AsDynamic()._entries = menuEntryList;
			((object)Dynamic._lettersInventoryCollection).AsDynamic()._entries = menuEntryList;
			((object)Dynamic._filesInventoryCollection).AsDynamic()._entries = menuEntryList;
			((object)Dynamic._questInventory).AsDynamic()._entries = menuEntryList;
			((object)Dynamic._bestiaryInventory).AsDynamic()._entries = menuEntryList;
			((object)Dynamic._featsInventory).AsDynamic()._entries = menuEntryList;

			((object)Dynamic._primaryMenuCollection).AsDynamic().SetSelectedIndex(0);
			((object)Dynamic._selectedMenuCollection).AsDynamic().SetSelectedIndex(0);
		}

		void CreateMenuForCategory(GameSettingCategoryInfo category)
		{
			var collection = FetchCollection(category.Name);
			var submenu = (IList)collection.AsDynamic()._entries;

			foreach (var settingFunc in category.SettingsPerCategory)
				submenu.Add(CreateMenuForSetting(settingFunc(settings)).AsTimeSpinnerMenuEntry());

			submenu.Add(CreateDefaultsMenu(new GameSettingCategoryInfo[] { category }, true));
			Dynamic.ChangeMenuCollection(collection, true);

			((object)Dynamic._selectedMenuCollection).AsDynamic().SetSelectedIndex(0);
		}

		object FetchCollection(string submenu)
		{
			dynamic collection;
			// Multiple submenus can share the same inventory collection
			// as the in-use collection is cleared before use.
			switch (submenu)
			{
				// Currently using quest layout for most, other layouts may be useful for other menus
				// Leaving as switch to easily add new menus as Memories, Letters, Files, Quests, Bestiary, Feats
				case "Sprite":
					collection = Dynamic._featsInventory;
					break;
				default:
					collection = Dynamic._questInventory;
					break;
			}

			var menuEntryList = new object[0].ToList(MenuEntryType);

			((object)collection).AsDynamic()._entries = menuEntryList;

			return (object)collection;
		}

		MenuEntry CreateMenuForSetting(GameSetting setting)
		{
			var menuEntry = MenuEntry.Create(setting.Name, () => ToggleSetting(setting));

			setting.UpdateMenuEntry(menuEntry);

			return menuEntry;
		}

		void ToggleSetting(GameSetting setting)
		{
			if (!setting.CanBeChangedInGame && IsInGame)
			{
				Dynamic.PlayErrorSound();
				return;
			}

			setting.ToggleValue();

			if (IsInGame)
				save.SetSettings(settings);
			else
				GameSettingsLoader.WriteSettingsToFile(settings);

			var selectedMenu = ((object)Dynamic._selectedMenuCollection).AsDynamic();
			var menuEntry = new MenuEntry(((IList)selectedMenu.Entries)[selectedMenu.SelectedIndex]);

			if (!setting.CanBeChangedInGame && IsInGame)
				menuEntry.BaseDrawColor = MenuEntry.UnAvailableColor;

			setting.UpdateMenuEntry(menuEntry);
		}
	}
}
