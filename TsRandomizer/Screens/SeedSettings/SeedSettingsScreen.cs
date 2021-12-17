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

			Dynamic._menuTitle = "Seed Settings";

			var menuEntryList = new object[0].ToList(MenuEntryType);

			var scalingMenu = MenuEntry.Create("Stats", OnScalingSelected);
			scalingMenu.AsDynamic().Description = "Settings related to player stat scaling.";
			scalingMenu.AsDynamic().IsCenterAligned = false;
			menuEntryList.Add(scalingMenu.AsTimeSpinnerMenuEntry());

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

			/*
			var menuCollectionList = new object[0].ToList(MenuEntryCollectionType);
			Dynamic._subMenuCollections = menuCollectionList;
			*/
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
			Dynamic.ChangeMenuCollection(Dynamic._memoriesInventoryCollection, true);
		}
		void OnEnemiesSelected()
		{
			Dynamic.ChangeMenuCollection(Dynamic._lettersInventoryCollection, true);
		}
		void OnLootSelected()
		{
			Dynamic.ChangeMenuCollection(Dynamic._filesInventoryCollection, true);
		} 
		void OnSpritesSelected()
		{
			Dynamic.ChangeMenuCollection(Dynamic._bestiaryInventory, true);
		}
		void OnOtherSelected()
		{
			Dynamic.ChangeMenuCollection(Dynamic._questInventory, true);
		}
	}
}
