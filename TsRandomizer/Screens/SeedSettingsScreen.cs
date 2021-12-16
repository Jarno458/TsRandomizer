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


namespace TsRandomizer.Screens
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

			var scalingMenu = MenuEntry.Create("Stats", () => { }).AsTimeSpinnerMenuEntry();
			var enemyMenu = MenuEntry.Create("Enemies", () => { }).AsTimeSpinnerMenuEntry();
			var spriteMenu = MenuEntry.Create("Sprites", () => { }).AsTimeSpinnerMenuEntry();
			var otherMenu = MenuEntry.Create("Other", () => { }).AsTimeSpinnerMenuEntry();

			scalingMenu.AsDynamic().Description = "Settings related to stat scaling.";
			scalingMenu.AsDynamic().IsCenterAligned = false;

			enemyMenu.AsDynamic().Description = "Settings related to enemy placement and drops.";
			enemyMenu.AsDynamic().IsCenterAligned = false;

			spriteMenu.AsDynamic().Description = "Settings related to sprite replacement.";
			spriteMenu.AsDynamic().IsCenterAligned = false;

			otherMenu.AsDynamic().Description = "Various other settings.";
			otherMenu.AsDynamic().IsCenterAligned = false;

			menuEntryList.Add(scalingMenu);
			menuEntryList.Add(enemyMenu);
			menuEntryList.Add(spriteMenu);
			menuEntryList.Add(otherMenu);

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = menuEntryList;
			Dynamic._subMenuCollections = menuCollectionList;

		}

		public SeedSettingsScreen(ScreenManager screenManager, GameScreen passwordMenuScreen) : base(screenManager, passwordMenuScreen)
		{
			seedSelectionScreen = screenManager.FirstOrDefault<SeedSelectionMenuScreen>();
		}

		public static GameScreen Create(ScreenManager screenManager, SeedOptionsCollection options)
		{
			void Noop() { }
			return (GameScreen)Activator.CreateInstance(JournalMenuType, GameSave.DemoSave, screenManager.Dynamic.GCM, (Action)Noop);
		}
	}
}
