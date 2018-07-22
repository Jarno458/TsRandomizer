using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.MainMenu.GameDifficultyMenu")]
	// ReSharper disable once UnusedMember.Global
	class GameDifficultyMenuScreen : Screen
	{
		readonly MenuEntry selectSeedMenuEntry;

		public GameDifficultyMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			selectSeedMenuEntry = AddSelectSeedMenu(screen);
		}

		MenuEntry AddSelectSeedMenu(GameScreen screen)
		{
			var entry = MenuEntry.Create("", SelectSeed);
			entry.Description = "Select the seed used to generate the randomness";
			var menuEntries = (IList)screen.Reflect().MenuEntries;
			menuEntries.Insert(0, entry.AsTimeSpinnerMenuEntry());
			((object)ScreenReflected._primaryMenuCollection).Reflect().SelectedIndex = 2;

			return entry;
		}

		void SelectSeed(PlayerIndex pi)
		{
			var selectSeedMenu = SeedSelectionMenuScreen.Create(ScreenManager);

			ScreenManager.AddScreen(selectSeedMenu, PlayerIndex.One);
		}

		public override void Update(InputState input)
		{
			selectSeedMenuEntry.Text = $"Seed: {Seed.Current}";
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			base.Draw(spriteBatch, menuFont);

			/*GCM gcm = screenManager.Reflected.GCM;

			spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
			spriteBatch.Draw(gcm.TxBlankSquare, new Rectangle(30,30,30,30), Color.Aqua);
			spriteBatch.End();*/
		}
	}
}