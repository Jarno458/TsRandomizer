using System;
using System.Collections;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Drawables;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.MainMenu.GameDifficultyMenu")]
	// ReSharper disable once UnusedMember.Global
	class GameDifficultyMenuScreen : Screen
	{
		readonly MenuEntry seedMenuEntry;
		readonly SeedRepresentation seedRepresentation;

		Seed? seed;

		Action<GameSave.EGameDifficultyType> originalOnDifficultyChosenMethod;

		public GameDifficultyMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			DisableDefaultDifficultOptions();

			seedMenuEntry = GetSelectSeedMenu();
			AddMenuEntryAtIndex(0, seedMenuEntry);
			SetSelectedMenuItemByIndex(0);
			
			seedRepresentation = new SeedRepresentation(ScreenManager.Reflected.GCM);

			HookOnDifficultySelectedMethod();
		}

		void DisableDefaultDifficultOptions()
		{
			var menuEntries = (IList)Reflected.MenuEntries;

			foreach (var menuEntry in menuEntries)
			{
				var entry = menuEntry.AsDynamic();
				entry.BaseDrawColor = MenuEntry.UnavailableColor;
			}
		}

		void AddMenuEntryAtIndex(int index, MenuEntry menuEntry)
		{
			var menuEntries = (IList)Reflected.MenuEntries;
			menuEntries.Insert(index, menuEntry.AsTimeSpinnerMenuEntry());

		}
		void SetSelectedMenuItemByIndex(int index)
		{
			((object)Reflected._primaryMenuCollection).AsDynamic().SelectedIndex = index;
		}

		MenuEntry GetSelectSeedMenu()
		{
			var entry = MenuEntry.Create("Choose seed", OpenSelectSeedMenu);
			entry.Description = "Select the seed used to generate the randomness";

			return entry;
		}

		void OpenSelectSeedMenu(PlayerIndex pi)
		{
			var selectSeedMenu = SeedSelectionMenuScreen.Create(ScreenManager, this);

			ScreenManager.AddScreen(selectSeedMenu, pi);
		}

		void HookOnDifficultySelectedMethod()
		{
			originalOnDifficultyChosenMethod = Reflected._onDifficultyChosen;


			Reflected._onDifficultyChosen = (Action<GameSave.EGameDifficultyType>)NewOnDifficultySelectedMethod;
		}

		void NewOnDifficultySelectedMethod(GameSave.EGameDifficultyType difficulty)
		{
			if(!seed.HasValue)
				return;

			originalOnDifficultyChosenMethod(difficulty);
			AddSeedToSelectedSave();
		}

		void AddSeedToSelectedSave()
		{
			foreach (var gameScreen in ScreenManager.GetScreens())
			{
				if (gameScreen.GetType() == TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.LoadingScreen"))
				{
					var loadingScreen = gameScreen.AsDynamic();
					var gameplayScreen = ((GameScreen[])loadingScreen._screensToLoad)[0];
					var saveGame = (GameSave)gameplayScreen.AsDynamic().SaveFile;

					saveGame.SetSeed(seed.Value);
					saveGame.SetFillingMethod(FillingMethod.Random);
					saveGame.DataKeyStrings["TsRandomizerVersion"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
				}
			}
		}

		public void SetSeed(Seed selectedSeed)
		{
			seed = selectedSeed;
			seedRepresentation.SetSeed(selectedSeed);

			seedMenuEntry.Text = "Seed: ";

			((object)Reflected._primaryMenuCollection).AsDynamic().SelectedIndex = 2;

			EnableAllMenuItems();
		}

		void EnableAllMenuItems()
		{
			var menuEntries = (IList)Reflected.MenuEntries;

			foreach (var menuEntry in menuEntries)
			{
				var entry = menuEntry.AsDynamic();
				entry.BaseDrawColor = MenuEntry.UnselectedColor;
			}
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			base.Update(gameTime, input);

			if (seed == null)
				SetSelectedMenuItemByIndex(0);
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			if(GameScreen.IsActive && seed != null)
				DrawSeedRepresentation(spriteBatch, menuFont);
		}

		void DrawSeedRepresentation(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			var zoom = (int) TimeSpinnerGame.Constants.InGameZoom;
			var menuDrawPosition = ((object)Reflected._primaryMenuCollection).AsDynamic().DrawPosition;

			seedRepresentation.IconSize = menuFont.LineSpacing * zoom;

			var seedStringSize = menuFont.MeasureString(seedMenuEntry.Text + " ") * zoom;
			var seedRepresentationDrawArea = new Point(
				(int) menuDrawPosition.X + ((int) seedStringSize.X / 2),
				(int) menuDrawPosition.Y - ((int) seedStringSize.Y / 2));

			seedRepresentation.SetDrawPoint(seedRepresentationDrawArea);

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)) 
				seedRepresentation.Draw(spriteBatch);
		}
	}
}