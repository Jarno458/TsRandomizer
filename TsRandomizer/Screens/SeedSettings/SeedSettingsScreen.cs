using System;
using System.Linq;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;
using TsRandomizer.Screens.SeedSelection;

using System.Collections;


namespace TsRandomizer.Screens.SeedSettings
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

		bool IsUsedAsSeedSettingsMenu => seedSelectionScreen != null;

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			if (!IsUsedAsSeedSettingsMenu)
				return;

			// Default order is Memories, Letters, Files, Quests, Bestiary, Feats
			Dynamic._menuTitle = "Seed Settings";
			ClearAllSubmenus();

			var menuEntryList = new object[0].ToList(MenuEntryType);

			var scalingMenu = MenuEntry.Create("Stats", OnScalingSelected);
			scalingMenu.AsDynamic().Description = "Settings related to player stat scaling.";
			scalingMenu.AsDynamic().IsCenterAligned = false;
			menuEntryList.Add(scalingMenu.AsTimeSpinnerMenuEntry());

			var scalingSubmenu = Dynamic._memoriesInventoryCollection;

			var enemyMenu = MenuEntry.Create("Enemies", OnEnemiesSelected);
			enemyMenu.Description = "Settings related to enemy placement and stats.";
			enemyMenu.IsCenterAligned = false;
			menuEntryList.Add(enemyMenu.AsTimeSpinnerMenuEntry());

			var lootMenu = MenuEntry.Create("Loot", OnLootSelected);
			lootMenu.Description = "Settings related to shop inventory and loot.";
			lootMenu.IsCenterAligned = false;
			menuEntryList.Add(lootMenu.AsTimeSpinnerMenuEntry());

			var spriteMenu = MenuEntry.Create("Sprites", OnSpritesSelected, false);
			spriteMenu.Description = "Settings related to sprite replacement.";
			spriteMenu.IsCenterAligned = false;
			menuEntryList.Add(spriteMenu.AsTimeSpinnerMenuEntry());

			var otherMenu = MenuEntry.Create("Other", OnOtherSelected);
			otherMenu.Description = "Various other settings.";
			otherMenu.IsCenterAligned = false;
			menuEntryList.Add(otherMenu.AsTimeSpinnerMenuEntry());

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

		void OnScalingSelected()
		{
			var collection = FetchCollection("Stats");
			var submenu = (IList)collection.AsDynamic()._entries;
			var menuEntry = MenuEntry.Create("Placeholder Entry", () => { }).AsTimeSpinnerMenuEntry();
			menuEntry.AsDynamic().IsCenterAligned = false;
			menuEntry.AsDynamic().Description = "A setting that does something";
			submenu.Add(menuEntry);
			Dynamic.ChangeMenuCollection(Dynamic._bestiaryInventory, true);
		}
		void OnEnemiesSelected()
		{
			var collection = FetchCollection("Enemies");
			var submenu = (IList)collection.AsDynamic()._entries;
			var menuEntry = MenuEntry.Create("Placeholder Entry", () => { }).AsTimeSpinnerMenuEntry();
			menuEntry.AsDynamic().IsCenterAligned = false;
			menuEntry.AsDynamic().Description = "A setting that does something";
			submenu.Add(menuEntry);
			Dynamic.ChangeMenuCollection(collection, true);
		}
		void OnLootSelected()
		{
			var collection = FetchCollection("Loot");
			var submenu = (IList)collection.AsDynamic()._entries;
			var menuEntry = MenuEntry.Create("Placeholder Entry", () => { }).AsTimeSpinnerMenuEntry();
			menuEntry.AsDynamic().IsCenterAligned = false;
			menuEntry.AsDynamic().Description = "A setting that does something";
			submenu.Add(menuEntry);
			Dynamic.ChangeMenuCollection(collection, true);
		}
		void OnOtherSelected()
		{
			var collection = FetchCollection("Other");
			var submenu = (IList)collection.AsDynamic()._entries;
			var menuEntry = MenuEntry.Create("Placeholder Entry", () => { }).AsTimeSpinnerMenuEntry();
			menuEntry.AsDynamic().IsCenterAligned = false;
			menuEntry.AsDynamic().Description = "A setting that does something";
			submenu.Add(menuEntry);
			Dynamic.ChangeMenuCollection(collection, true);
		}
		void OnSpritesSelected()
		{
			var collection = FetchCollection("Sprite");
			var submenu = (IList)collection.AsDynamic()._entries;
			var menuEntry = MenuEntry.Create("Placeholder Entry", () => { }).AsTimeSpinnerMenuEntry();
			menuEntry.AsDynamic().IsCenterAligned = false;
			menuEntry.AsDynamic().Description = "A setting that does something";
			submenu.Add(menuEntry);
			Dynamic.ChangeMenuCollection(collection, true);
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
