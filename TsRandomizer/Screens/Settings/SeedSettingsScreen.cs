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
		GCM gcm;

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			if (!IsUsedAsSeedSettingsMenu)
				return;
			gcm = gameContentManager;

			// Default order is Memories, Letters, Files, Quests, Bestiary, Feats
			Dynamic._menuTitle = "Seed Settings";
			ResetMenu();
		}

		void ResetMenu()
		{
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
			GameSetting[] allSettings = new GameSetting[] { gameSettings.StartWithMeyef, gameSettings.DamageRando,
				gameSettings.StartWithJewelryBox, gameSettings.PlayerName, gameSettings. OrbXPMultiplier,
				gameSettings.ShopFill, gameSettings.ShopMultiplier };
			menuEntryList.Add(CreateDefaultsMenu(allSettings));

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = menuEntryList;
		}

		public SeedSettingsScreen(ScreenManager screenManager, GameScreen passwordMenuScreen) : base(screenManager, passwordMenuScreen)
		{
			seedSelectionScreen = screenManager.FirstOrDefault<SeedSelectionMenuScreen>();
		}

		public static GameScreen Create(ScreenManager screenManager)
		{
			void Noop() { }
			GCM gcm = screenManager.AsDynamic().GCM;
			gcm.LoadAllResources(screenManager.AsDynamic().GeneralContentManager, screenManager.GraphicsDevice);
			return (GameScreen)Activator.CreateInstance(JournalMenuType, GameSave.EditorSave, gcm, (Action)Noop);
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
				submenu.Add(CreateDefaultsMenu(category.Settings));
				Dynamic.ChangeMenuCollection(collection, true);
			}
			return CreateMenu;
		}

		object CreateDefaultsMenu(GameSetting[] settings)
		{
			var defaultsMenu = MenuEntry.Create("Defaults", OnDefaultsSelected(settings)).AsTimeSpinnerMenuEntry();
			defaultsMenu.AsDynamic().Description = "Restore all values to their defaults";
			defaultsMenu.AsDynamic().IsCenterAligned = false;
			return defaultsMenu;
		}

		object CreateMenuForSetting(GameSetting setting)
		{
			var menuEntry = MenuEntry.Create(setting.Name, CreateToggleForSetting(setting)).AsTimeSpinnerMenuEntry();
			menuEntry.AsDynamic().IsCenterAligned = false;
			var currentValue = !(setting is OnOffGameSetting) ? setting.CurrentValue : setting.CurrentValue ? "On" : "Off";
			menuEntry.AsDynamic().Text = $"{setting.Name} - {currentValue}";
			menuEntry.AsDynamic().Description = setting.Description;

			return menuEntry;
		}

		Action CreateToggleForSetting(GameSetting setting)
		{
			void ToggleSetting()
			{
				Console.Out.WriteLine($"Can't toggle {setting.Name} to {setting.CurrentValue}");
			}
			return ToggleSetting;
		}

		Action OnDefaultsSelected(GameSetting[] settings)
		{
			void ResetDefaults()
			{
				foreach (GameSetting setting in settings)
				{
					setting.SetValue(setting.DefaultValue);
				}
				gameSettings.WriteSettings();
				ResetMenu();
			}
			return ResetDefaults;
		}

		object FetchCollection(string submenu)
		{
			var collection = Dynamic._filesInventoryCollection;
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
	}
}
