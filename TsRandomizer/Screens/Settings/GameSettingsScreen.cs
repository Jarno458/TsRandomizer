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
using TsRandomizer.Screens.Settings.GameSettingObjects;


namespace TsRandomizer.Screens.Settings
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.JournalMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class GameSettingsScreen : Screen
	{
		static readonly Type MenuEntryType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntry");
		static readonly Type MenuEntryCollectionType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntryCollection");
		static readonly Type JournalMenuType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.PauseMenu.JournalMenuScreen");

		readonly SeedSelectionMenuScreen seedSelectionScreen;
		GameSettingsCollection gameSettings;
		GameSettingCategoryInfo[] settingCategories;

		bool IsUsedAsGameSettingsMenu => seedSelectionScreen != null;
		GCM gcm;

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			if (!IsUsedAsGameSettingsMenu)
				return;

			gcm = gameContentManager;

			Dynamic._menuTitle = "Game Settings";

			ResetMenu();
		}

		public GameSettingsScreen(ScreenManager screenManager, GameScreen passwordMenuScreen) : base(screenManager, passwordMenuScreen)
		{
			seedSelectionScreen = screenManager.FirstOrDefault<SeedSelectionMenuScreen>();
		}

		public static GameScreen Create(ScreenManager screenManager, SeedOptionsCollection options)
		{
			void Noop() { }

			GCM gcm = screenManager.AsDynamic().GCM;

			gcm.LoadAllResources(screenManager.AsDynamic().GeneralContentManager, screenManager.GraphicsDevice);

			return (GameScreen)Activator.CreateInstance(JournalMenuType, GetSave(options), gcm, (Action)Noop);
		}

		static GameSave GetSave(SeedOptionsCollection options)
		{
			var save = GameSave.DemoSave;

			save.Inventory.AsDynamic()._relicInventory = options;

			return save;
		}

		object CreateDefaultsMenu(GameSetting[] settings)
		{
			var defaultsMenu = MenuEntry.Create("Defaults", () => OnDefaultsSelected(settings)).AsTimeSpinnerMenuEntry();

			defaultsMenu.AsDynamic().Description = "Restore all values to their defaults";
			defaultsMenu.AsDynamic().IsCenterAligned = false;

			return defaultsMenu;
		}

		void OnDefaultsSelected(GameSetting[] settings)
		{
			foreach (var setting in settings) 
				setting.SetValue(setting.DefaultValue);

			gameSettings.WriteSettings();

			ResetMenu();
		}

		void ResetMenu()
		{
			gameSettings = new GameSettingsCollection();
			gameSettings.LoadSettingsFromFile();

			settingCategories = new []
			{
				new GameSettingCategoryInfo {
					Name = "Stats",
					Description = "Settings related to player stat scaling.",
					Settings = new GameSetting[] { gameSettings.DamageRando }
				},
				new GameSettingCategoryInfo {
					Name = "Loot",
					Description = "Settings related to shop inventory and loot.",
					Settings = new GameSetting[] { gameSettings.ShopMultiplier, gameSettings.ShopFill, gameSettings.ShopWarpShards }
				}
			};

			ClearAllSubmenus();

			var menuEntryList = new object[0].ToList(MenuEntryType);

			foreach (var category in settingCategories)
			{
				var submenu = MenuEntry.Create(category.Name, () => CreateMenuForCategory(category));

				submenu.AsDynamic().Description = category.Description;
				submenu.AsDynamic().IsCenterAligned = false;

				menuEntryList.Add(submenu.AsTimeSpinnerMenuEntry());
			}

			var allSettings = new GameSetting[] { 
				gameSettings.DamageRando,
				gameSettings.ShopFill, 
				gameSettings.ShopMultiplier, 
				gameSettings.ShopWarpShards
			};

			menuEntryList.Add(CreateDefaultsMenu(allSettings));

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

			foreach (var setting in category.Settings) 
				submenu.Add(CreateMenuForSetting(setting));

			// Exclude submenu defaults for now, enable when they don't wipe the current view
			// submenu.Add(CreateDefaultsMenu(category.Settings));
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

		object CreateMenuForSetting(GameSetting setting)
		{
			var menuEntry = MenuEntry.Create(setting.Name, () => CreateToggleForSetting(setting)).AsTimeSpinnerMenuEntry();

			SetCurrentSettingText(menuEntry, setting);

			return menuEntry;
		}

		void CreateToggleForSetting(GameSetting setting)
		{
			switch (setting)
			{
				case OnOffGameSetting _:
					setting.SetValue(!(bool)setting.CurrentValue);
					break;
				case StringGameSetting _:
					// Future phase this should allow SDL input
					setting.SetValue(setting.CurrentValue);
					break;
				case NumberGameSetting gameSetting:
				{
					var value = (double)gameSetting.CurrentValue;
					var numberSetting = gameSetting;
					var stepValue = numberSetting.StepValue;

					value = value + stepValue <= numberSetting.MaximumValue ? value + stepValue : numberSetting.MinimumValue;

					setting.SetValue(value);

					break;
				}
				case SpecificValuesGameSetting gameSetting:
				{
					var enumSetting = gameSetting;
					var currentValue = enumSetting.AllowedValues.IndexOf((string)enumSetting.CurrentValue);
					var value = currentValue + 1 >= enumSetting.AllowedValues.Count ? 0 : currentValue + 1;

					gameSetting.SetValue(enumSetting.AllowedValues[value]);
					break;
				}
				default:
					return;
			}

			gameSettings.WriteSettings();

			var selectedMenu = ((object)Dynamic._selectedMenuCollection).AsDynamic();

			object menuEntry = ((IList)selectedMenu.Entries)[selectedMenu.SelectedIndex];

			SetCurrentSettingText(menuEntry, setting);
		}

		void SetCurrentSettingText(object menuEntry, GameSetting setting)
		{
			menuEntry.AsDynamic().IsCenterAligned = false;

			var currentValue = !(setting is OnOffGameSetting) 
				? setting.CurrentValue 
				: (bool)setting.CurrentValue 
					? "On" 
					: "Off";

			menuEntry.AsDynamic().Text = $"{setting.Name} - {currentValue}";
			menuEntry.AsDynamic().Description = setting.Description;
		}
	}
}
