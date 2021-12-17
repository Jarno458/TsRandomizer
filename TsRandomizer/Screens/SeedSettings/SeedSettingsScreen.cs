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
			var menuCollectionList = new object[0].ToList(MenuEntryCollectionType);

			/*var menus = ((IList)((object)Dynamic._primaryMenuCollection).AsDynamic()._entries)
				.Cast<object>()
				.ToList(MenuEntryType);*/

			var scalingMenu = MenuEntry.Create("Stats", OnScalingSelected);
			scalingMenu.AsDynamic().Description = "Settings related to player stat scaling.";
			scalingMenu.AsDynamic().IsCenterAligned = false;
			// scalingMenu.AsDynamic().Selected = menus[0].AsDynamic().Selected;

			var enemyMenu = MenuEntry.Create("Enemies", OnEnemiesSelected);
			enemyMenu.Description = "Settings related to enemy placement and stats.";
			enemyMenu.IsCenterAligned = false;
			// scalingMenu.AsDynamic().Selected = menus[1].AsDynamic().Selected;

			var lootMenu = MenuEntry.Create("Loot", OnLootSelected);
			lootMenu.Description = "Settings related to shop inventory and loot.";
			lootMenu.IsCenterAligned = false;
			// scalingMenu.AsDynamic().Selected = menus[2].AsDynamic().Selected;

			var spriteMenu = MenuEntry.Create("Sprites", OnSpritesSelected, false);
			spriteMenu.Description = "Settings related to sprite replacement.";
			spriteMenu.IsCenterAligned = false;
			//scalingMenu.AsDynamic().Selected = menus[3].AsDynamic().Selected;

			var otherMenu = MenuEntry.Create("Other", OnOtherSelected);
			otherMenu.Description = "Various other settings.";
			otherMenu.IsCenterAligned = false;
			// scalingMenu.AsDynamic().Selected = menus[4].AsDynamic().Selected;

			menuEntryList.Add(scalingMenu.AsTimeSpinnerMenuEntry());
			menuEntryList.Add(enemyMenu.AsTimeSpinnerMenuEntry());
			menuEntryList.Add(lootMenu.AsTimeSpinnerMenuEntry());
			menuEntryList.Add(spriteMenu.AsTimeSpinnerMenuEntry());
			menuEntryList.Add(otherMenu.AsTimeSpinnerMenuEntry());

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = menuEntryList;

			var collections = ((IList)Dynamic._subMenuCollections)
				.Cast<object>()
				.ToList(MenuEntryCollectionType);
			var bestiary = collections[4];

			menuCollectionList.Add(bestiary);
			menuCollectionList.Add(bestiary);
			menuCollectionList.Add(bestiary);
			menuCollectionList.Add(bestiary);
			menuCollectionList.Add(bestiary);
			Dynamic._subMenuCollections = menuCollectionList;
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
			// Dynamic._selectedMenucollection = Dynamic._subMenuCollections[0];
		}
		void OnEnemiesSelected()
		{
			// Dynamic._selectedMenucollection = Dynamic._subMenuCollections[1];
		}
		void OnLootSelected()
		{
			// Dynamic._selectedMenucollection = Dynamic._subMenuCollections[2];
		} 
		void OnSpritesSelected()
		{
			// Dynamic._selectedMenucollection = Dynamic._subMenuCollections[3];
		}
		void OnOtherSelected()
		{
			// Dynamic._selectedMenucollection = Dynamic._subMenuCollections[4];
		}
	}
}
