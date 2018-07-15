using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameStateManagement.ScreenManager;

namespace TsRanodmizer.Screens
{
    class Menu : GameScreen
    {
        readonly GameScreen nextScreen;
        ContentManager content;
        float screenShowTimer;

        public Menu(GameScreen nextScreen)
        {
            this.nextScreen = nextScreen;

            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
        }

        public override void UnloadContent()
        {
            content?.Unload();
        }

        void RemoveSelf()
        {
            ScreenManager.RemoveScreen(this);
            nextScreen.LoadContent();
            ScreenManager.AddScreen(nextScreen, new PlayerIndex?());
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (!IsActive)
                return;

            screenShowTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (screenShowTimer >= 0.4)
                RemoveSelf();
        }

        public override void HandleInput(InputState input)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.DarkGoldenrod, 0.0f, 0);
        }
    }
}

