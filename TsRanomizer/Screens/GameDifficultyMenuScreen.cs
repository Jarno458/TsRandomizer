using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Drawables;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.MainMenu.GameDifficultyMenu")]
	// ReSharper disable once UnusedMember.Global
	class GameDifficultyMenuScreen : Screen
	{
		readonly MenuEntry seedMenuEntry;
		readonly SeedRepresentation seedRepresentation;

		public GameDifficultyMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			seedMenuEntry = AddSelectSeedMenu();
			seedRepresentation = new SeedRepresentation(Point.Zero, 10, ScreenManager.Reflected.GCM);
		}

		MenuEntry AddSelectSeedMenu()
		{
			var entry = MenuEntry.Create("Seed:", SelectSeed);
			entry.Description = "Select the seed used to generate the randomness";
			var menuEntries = (IList)ScreenReflected.MenuEntries;
			menuEntries.Insert(0, entry.AsTimeSpinnerMenuEntry());
			((object)ScreenReflected._primaryMenuCollection).Reflect().SelectedIndex = 2;

			return entry;
		}

		void SelectSeed(PlayerIndex pi)
		{
			var selectSeedMenu = SeedSelectionMenuScreen.Create(ScreenManager);

			ScreenManager.AddScreen(selectSeedMenu, PlayerIndex.One);
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			base.Draw(spriteBatch, menuFont);
			DrawSeedRepresentation(spriteBatch, menuFont);
		}

		void DrawSeedRepresentation(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			var zoom = (int) TimeSpinnerGame.Constants.InGameZoom;
			var menuEntryOrigin = CalculateFirstMenuEntryOrigin(menuFont, zoom);

			seedRepresentation.IconSize = menuFont.LineSpacing * zoom;

			var seedStringSize = menuFont.MeasureString(seedMenuEntry.Text + " ") * zoom;
			var seedRepresentationDrawArea = new Point(
				(int) menuEntryOrigin.X + ((int) seedStringSize.X / 2),
				(int) menuEntryOrigin.Y - ((int) seedStringSize.Y / 2));

			seedRepresentation.SetDrawPoint(seedRepresentationDrawArea);

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
			seedRepresentation.Draw(spriteBatch);
			spriteBatch.End();
		}

		Vector2 CalculateFirstMenuEntryOrigin(SpriteFont menuFont, int zoom)
		{
			var menuEntries = (IList)ScreenReflected.MenuEntries;

			var vector2 = menuFont.MeasureString("N") * zoom;
			var menuOffset = new Point(0, (int)(vector2.Y * 2.0));

			var titleSafeArea = ScreenManager.TitleSafeArea;
			var rowHeight = ((object)ScreenReflected._primaryMenuCollection).Reflect().EntryHeight;

			var x = titleSafeArea.Center.X + menuOffset.X;
			var y = titleSafeArea.Center.Y - (menuEntries.Count - 1) / 2 * rowHeight + menuOffset.Y;

			return new Vector2(x, y);
		}
	}
}