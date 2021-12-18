using System;
using System.Collections;
using System.Linq;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;
using TsRandomizer.Screens.SeedSelection;
using TsRandomizer.Screens.Settings.GameSettingObjects;


namespace TsRandomizer.Screens.Settings
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.JournalMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class SeedSettingsScreen : Screen
	{
		static readonly Type MenuEntryType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntry");
		static readonly Type MenuEntryCollectionType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntryCollection");
		static readonly Type JournalMenuType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.PauseMenu.JournalMenuScreen");

		readonly SeedSelectionMenuScreen seedSelectionScreen;
		GameSettingsCollection gameSettings = new GameSettingsCollection();

		bool IsUsedAsSeedSettingsMenu => seedSelectionScreen != null;

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			if (!IsUsedAsSeedSettingsMenu)
				return;

			// Default order is Memories, Letters, Files, Quests, Bestiary, Feats
			Dynamic._menuTitle = "Seed Settings";
			ClearAllSubmenus();
			gameSettings.LoadSettingsFromFile();

			var menuEntryList = new object[0].ToList(MenuEntryType);
	
			SeedSettingsCategoryCollection categories = new SeedSettingsCategoryCollection();
			foreach (SeedSettingCategoryInfo category in categories.SettingCategories)
			{
				var submenu = MenuEntry.Create(category.Name, CreateMenuForCategory(category));
				submenu.AsDynamic().Description = category.Description;
				submenu.AsDynamic().IsCenterAligned = false;
				menuEntryList.Add(submenu.AsTimeSpinnerMenuEntry());
			}

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = menuEntryList;
		}

		public SeedSettingsScreen(ScreenManager screenManager, GameScreen passwordMenuScreen) : base(screenManager, passwordMenuScreen)
		{
			seedSelectionScreen = screenManager.FirstOrDefault<SeedSelectionMenuScreen>();
		}

		public static GameScreen Create(ScreenManager screenManager, SeedOptionsCollection options)
		{
			void Noop() { }
			GameSave save = GameSave.EditorSave;
			return (GameScreen)Activator.CreateInstance(JournalMenuType, save, screenManager.Dynamic.GCM, (Action)Noop);
		}

		Action CreateMenuForCategory(SeedSettingCategoryInfo category)
		{
			void CreateMenu()
			{
				var collection = FetchCollection(category.Name);
				var submenu = (IList)collection.AsDynamic()._entries;
				foreach (GameSetting setting in category.Settings)
				{
					submenu.Add(CreateMenuForSetting(setting));
				}
				Dynamic.ChangeMenuCollection(Dynamic._bestiaryInventory, true);
			}
			return CreateMenu;
		}

		object CreateMenuForSetting(GameSetting setting)
		{
			var menuEntry = MenuEntry.Create(setting.Name, () => { }).AsTimeSpinnerMenuEntry();
			menuEntry.AsDynamic().IsCenterAligned = false;
			var currentValue = !(setting is OnOffGameSetting) ? setting.CurrentValue : setting.CurrentValue ? "On" : "Off";
			menuEntry.AsDynamic().Text = $"{setting.Name} - {currentValue}";
			menuEntry.AsDynamic().Description = setting.Description;

			return menuEntry;
		}

		object FetchCollection(string submenu)
		{
			var collection = Dynamic._filesInventoryCollection;
			HideAll();
			// Multiple submenus can share the same inventory collection
			// as the in-use collection is cleared before use.
			switch (submenu)
			{
				// Currently using bestiary layout for most, other layouts may be useful for other menus
				/*
				collection = Dynamic._memoriesInventoryCollection;
				collection = Dynamic._lettersInventoryCollection;
				collection = Dynamic._filesInventoryCollection;
				*/
				case "Stats":
				case "Enemies":
				case "Loot":
				case "Other":
					collection = Dynamic._bestiaryInventory;
					break;
				case "Sprite":
					collection = Dynamic._featsInventory;
					break;
				default:
					ClearAllSubmenus();
					break;
			}

			var menuEntryList = new object[0].ToList(MenuEntryType);
			((object)collection).AsDynamic()._entries = menuEntryList;
			return (object)collection;
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
		}

		void HideAll()
		{
			var menuEntryList = new object[0].ToList(MenuEntryType);
			((object)Dynamic._memoriesInventoryCollection).AsDynamic().IsVisible = true;
			((object)Dynamic._lettersInventoryCollection).AsDynamic().IsVisible = true;
			((object)Dynamic._filesInventoryCollection).AsDynamic().IsVisible = true;
			((object)Dynamic._questInventory).AsDynamic().IsVisible = true;
			((object)Dynamic._bestiaryInventory).AsDynamic().IsVisible = true;
			((object)Dynamic._featsInventory).AsDynamic().IsVisible = true;
			((object)Dynamic._primaryMenuCollection).AsDynamic().IsVisible = true;
		}
	}
}
