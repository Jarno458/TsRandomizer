using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Drawables;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;
using TsRandomizer.Screens.SeedSelection;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.MainMenu.GameDifficultyMenu")]
	// ReSharper disable once UnusedMember.Global
	class GameDifficultyMenuScreen : Screen
	{
		static readonly Type LoadingScreenType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.LoadingScreen");
		static readonly Type MainMenuEntryType = 
			TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntry");

		readonly MenuEntry seedMenuEntry;
		readonly SeedRepresentation seedRepresentation;

		Seed? seed;
		FillingMethod fillingmethod;

		Action<GameSave> onDifficultSelected;

		bool isArchipelago;

		Action<GameSave.EGameDifficultyType> originalOnDifficultyChosenMethod;

		public GameDifficultyMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			UpdateHardModeDifficulties();

			DisableDefaultDifficultOptions();

			seedMenuEntry = GetSelectSeedMenu();
			AddMenuEntryAtIndex(0, seedMenuEntry); 

			seedRepresentation = new SeedRepresentation(ScreenManager.Reflected.GCM);

			HookOnDifficultySelectedMethod();
		}

		void UpdateHardModeDifficulties()
		{
			if (!Dynamic._isHardModeAvailable)
			{
				((object)Dynamic._hardMenuEntry).AsDynamic().BaseDrawColor = MenuEntry.UnSelectedColor;
				Dynamic._isHardModeAvailable = true;

				string title = TimeSpinnerGame.Localizer.Get("DifficultyMenuHardCap1");
				var menuEntry = MenuEntry.Create(title, p => Dynamic.OnHardCap1EntrySelected(null, null));
				menuEntry.Description = TimeSpinnerGame.Localizer.Get("DifficultyMenuHardLevelCap1Description");

				AddMenuEntry(menuEntry);
			}

			if (Dynamic._isLevelCap255Available)
				RemoveLastMenuEntry();
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			SetSelectedMenuItemByIndex(0);
		}

		void AddMenuEntry(MenuEntry menuEntry)
		{
			var entries = ((IList)Dynamic.MenuEntries)
				.Cast<object>()
				.Concat(menuEntry.AsTimeSpinnerMenuEntry())
				.ToList(MainMenuEntryType);

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = entries;
		}

		void RemoveLastMenuEntry()
		{
			var entries = ((IList)Dynamic.MenuEntries)
				.Cast<object>()
				.Reverse().Skip(1).Reverse()
				.ToList(MainMenuEntryType);

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = entries;
		}

		void DisableDefaultDifficultOptions()
		{
			var menuEntries = (IList)Dynamic.MenuEntries;

			foreach (var menuEntry in menuEntries)
			{
				var entry = menuEntry.AsDynamic();
				entry.BaseDrawColor = MenuEntry.UnAvailableColor;
			}
		}

		MenuEntry GetSelectSeedMenu()
		{
			var entry = MenuEntry.Create("Choose seed", OpenSelectSeedMenu);
			entry.Description = "Select the seed used to generate the randomness";

			return entry;
		}

		void OpenSelectSeedMenu(PlayerIndex pi)
		{
			GameScreen screen;
			if (isArchipelago)
				screen = ArchipelagoSelectionScreen.Create(ScreenManager);
			else
				screen = SeedSelectionMenuScreen.Create(ScreenManager);

			ScreenManager.AddScreen(screen, pi);
		}

		void HookOnDifficultySelectedMethod()
		{
			originalOnDifficultyChosenMethod = Dynamic._onDifficultyChosen;

			void NewOnDifficultySelectedMethod(GameSave.EGameDifficultyType difficulty)
			{
				if (!seed.HasValue)
					return;

				originalOnDifficultyChosenMethod(difficulty);

				var loadingScreen = ScreenManager
					.GetScreens()
					.First(s => s.GetType() == LoadingScreenType)
					.AsDynamic();

				var gameplayScreen = ((GameScreen[])loadingScreen._screensToLoad)[0];
				var saveGame = (GameSave)gameplayScreen.AsDynamic().SaveFile;

				AddSeedAndFillingMethodToSelectedSave(saveGame);

				onDifficultSelected?.Invoke(saveGame);
			}

			Dynamic._onDifficultyChosen = (Action<GameSave.EGameDifficultyType>)NewOnDifficultySelectedMethod;
		}

		void AddSeedAndFillingMethodToSelectedSave(GameSave saveGame)
		{
			saveGame.SetSeed(seed.Value);
			saveGame.SetFillingMethod(fillingmethod);

			saveGame.DataKeyStrings["TsRandomizerVersion"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}

		public void SetSeedAndFillingMethod(Seed selectedSeed, FillingMethod choosenFillingMethod)
		{
			seed = selectedSeed;
			fillingmethod = choosenFillingMethod;

			seedRepresentation.SetSeed(selectedSeed);

			SetSelectedMenuItemByIndex(2);

			EnableAllMenuItems();
		}

		public void HookOnDifficultySelected(Action<GameSave> onDifficultSelectedHook)
		{
			onDifficultSelected = onDifficultSelectedHook;
		}

		void EnableAllMenuItems()
		{
			var menuEntries = (IList)Dynamic.MenuEntries;

			foreach (var menuEntry in menuEntries)
			{
				var entry = menuEntry.AsDynamic();
				entry.BaseDrawColor = MenuEntry.UnSelectedColor;
			}
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			base.Update(gameTime, input);

			if (seed == null)
			{
				SetSelectedMenuItemByIndex(0);

				if (input.IsNewButtonPress(Buttons.LeftThumbstickLeft)
				    || input.IsNewButtonPress(Buttons.LeftThumbstickRight)
				    || input.IsNewButtonPress(Buttons.RightThumbstickLeft)
				    || input.IsNewButtonPress(Buttons.RightThumbstickRight)
				    || input.IsNewButtonPress(Buttons.DPadLeft)
				    || input.IsNewButtonPress(Buttons.DPadRight)
				    || input.IsNewButtonPress(Buttons.LeftTrigger)
				    || input.IsNewButtonPress(Buttons.RightTrigger)
				    || input.IsNewKeyPress(Keys.Left)
				    || input.IsNewKeyPress(Keys.Right))
				{
					isArchipelago = !isArchipelago;
				}
			}

			if (isArchipelago)
			{
				seedMenuEntry.Text = "<< Archipelago >>";
				seedMenuEntry.Description = "Connect to an Archipelago Multiworld server";
			}
			else if (seed == null)
			{
				seedMenuEntry.Text = "<< Choose seed >>";
				seedMenuEntry.Description = "Select the seed used to generate the randomness";
			}
			else
			{
				seedMenuEntry.Text = "Seed: ";
				seedMenuEntry.Description = "Select the seed used to generate the randomness";
			}
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			if(GameScreen.IsActive && seed != null)
				DrawSeedRepresentation(spriteBatch, menuFont);
		}

		void DrawSeedRepresentation(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			var zoom = (int) TimeSpinnerGame.Constants.InGameZoom;
			var menuDrawPosition = ((object)Dynamic._primaryMenuCollection).AsDynamic().DrawPosition;

			seedRepresentation.IconSize = menuFont.LineSpacing * zoom;

			var seedStringSize = menuFont.MeasureString(seedMenuEntry.Text + " ") * zoom;
			var seedRepresentationDrawArea = new Point(
				(int) menuDrawPosition.X + ((int) seedStringSize.X / 2),
				(int) menuDrawPosition.Y - ((int) seedStringSize.Y / 2));

			seedRepresentation.SetDrawPoint(seedRepresentationDrawArea);

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)) 
				seedRepresentation.Draw(spriteBatch);
		}

		protected void AddMenuEntryAtIndex(int index, MenuEntry menuEntry)
		{
			var menuEntries = (IList)Dynamic.MenuEntries;
			menuEntries.Insert(index, menuEntry.AsTimeSpinnerMenuEntry());
		}

		protected void SetSelectedMenuItemByIndex(int index)
		{
			((object)Dynamic._primaryMenuCollection).AsDynamic().SelectedIndex = index;
			Dynamic.OnSelectedEntryChanged(index);
		}
	}
}