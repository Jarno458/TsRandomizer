using Microsoft.Xna.Framework;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using TsRanodmizer.Screens;
using GameplayScreen = TsRanodmizer.Screens.GameplayScreen;

namespace TsRanodmizer.OverloadedObjects
{
    class ScreenManager2 : ScreenManager
    {
		readonly LookupDictionairy<GameScreen, Screen> screens 
			= new LookupDictionairy<GameScreen, Screen>(s => s.GameScreen);
		
		public ScreenManager2(TimeSpinnerGame game) : base(game)
        {
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
				if(screens.Contains(screen)) continue;

				switch (screen.GetType().Name)
	            {
					case "MainMenuScreen":
						var mainMenuScreen = new MainMenuScreen(screen);
						screens.Add(mainMenuScreen);
						break;

					case "GameplayScreen":
						var gameplayScreen = new GameplayScreen(screen);
						screens.Add(gameplayScreen);
						break;
				}
            }
        }

        void UpdateGameplayScreens()
        {
			var input = (InputState)this.Reflect()._input;

            foreach (var screen in screens)
                screen.Update(input);
        }

        void DrawGameplayScreens()
        {
            foreach (var screen in screens)
                screen.Draw(SpriteBatch, MenuFont);
        }

        public void CopyScreensFrom(ScreenManager screenManager)
        {
            foreach (var screen in screenManager.GetScreens())
                AddScreen(screen, null);
        }
    }
}
