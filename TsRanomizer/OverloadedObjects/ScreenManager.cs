using Microsoft.Xna.Framework;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using TsRanodmizer.Screens;
using GameplayScreen = TsRanodmizer.Screens.GameplayScreen;

namespace TsRanodmizer.OverloadedObjects
{
	class ScreenManager : Timespinner.GameStateManagement.ScreenManager.ScreenManager
	{
		readonly LookupDictionairy<GameScreen, Screen> hookedScreens
			= new LookupDictionairy<GameScreen, Screen>(s => s.GameScreen);

		public readonly dynamic Reflected;

		public ScreenManager(TimeSpinnerGame game) : base(game)
		{
			Reflected = this.Reflect();
		}

		public override void Update(GameTime gameTime)
		{
			DetectNewGameplayScreens();
			UpdateGameplayScreens();

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			DrawGameplayScreens();
		}

		void DetectNewGameplayScreens()
		{
			foreach (var screen in GetScreens())
			{
				if (hookedScreens.Contains(screen)) continue;

				switch (screen.GetType().Name)
				{
					case "MainMenuScreen":
						var mainMenuScreen = new MainMenuScreen(this, screen);
						hookedScreens.Add(mainMenuScreen);
						break;

					case "GameplayScreen":
						var gameplayScreen = new GameplayScreen(screen);
						hookedScreens.Add(gameplayScreen);
						break;
				}
			}
		}

		void UpdateGameplayScreens()
		{
			var input = (InputState)Reflected._input;

			foreach (var screen in hookedScreens)
				screen.Update(input);
		}

		void DrawGameplayScreens()
		{
			foreach (var screen in hookedScreens)
				screen.Draw(SpriteBatch, MenuFont);
		}

		public void CopyScreensFrom(Timespinner.GameStateManagement.ScreenManager.ScreenManager screenManager)
		{
			foreach (var screen in screenManager.GetScreens())
				AddScreen(screen, null);
		}
	}
}
