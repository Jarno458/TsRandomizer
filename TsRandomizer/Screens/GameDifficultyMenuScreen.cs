using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

		readonly MenuEntry seedMenuEntry;
		readonly SeedRepresentation seedRepresentation;

		Seed? seed;

		Action<GameSave.EGameDifficultyType> originalOnDifficultyChosenMethod;

		public GameDifficultyMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			DisableDefaultDifficultOptions();

			seedMenuEntry = GetSelectSeedMenu();
			AddMenuEntryAtIndex(0, seedMenuEntry); 

			seedRepresentation = new SeedRepresentation(ScreenManager.Reflected.GCM);

			HookOnDifficultySelectedMethod();
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			SetSelectedMenuItemByIndex(0);
		}

		void DisableDefaultDifficultOptions()
		{
			var menuEntries = (IList)Dynamic.MenuEntries;

			foreach (var menuEntry in menuEntries)
			{
				var entry = menuEntry.AsDynamic();
				entry.BaseDrawColor = MenuEntry.UnavailableColor;
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
			var selectSeedMenu = SeedSelectionMenuScreen.Create(ScreenManager);

			ScreenManager.AddScreen(selectSeedMenu, pi);
		}

		void HookOnDifficultySelectedMethod()
		{
			originalOnDifficultyChosenMethod = Dynamic._onDifficultyChosen;

			void NewOnDifficultySelectedMethod(GameSave.EGameDifficultyType difficulty)
			{
				if (!seed.HasValue)
					return;

				originalOnDifficultyChosenMethod(difficulty);

				AddSeedToSelectedSave();
			}

			Dynamic._onDifficultyChosen = (Action<GameSave.EGameDifficultyType>)NewOnDifficultySelectedMethod;
		}

		void AddSeedToSelectedSave()
		{
			var loadingScreen = ScreenManager
				.GetScreens()
				.First(s => s.GetType() == LoadingScreenType)
				.AsDynamic();

			var gameplayScreen = ((GameScreen[])loadingScreen._screensToLoad)[0];
			var saveGame = (GameSave)gameplayScreen.AsDynamic().SaveFile;

			saveGame.SetSeed(seed.Value);
			saveGame.SetFillingMethod(FillingMethod.Random);
			saveGame.DataKeyStrings["TsRandomizerVersion"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}

		public void SetSeed(Seed selectedSeed)
		{
			seed = selectedSeed;
			seedRepresentation.SetSeed(selectedSeed);

			seedMenuEntry.Text = "Seed: ";

			SetSelectedMenuItemByIndex(2);

			EnableAllMenuItems();
		}

		void EnableAllMenuItems()
		{
			var menuEntries = (IList)Dynamic.MenuEntries;

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