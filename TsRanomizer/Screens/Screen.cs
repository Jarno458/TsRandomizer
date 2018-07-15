using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;

namespace TsRanodmizer.Screens
{
	class Screen
	{
		public readonly GameScreen GameScreen;

		protected readonly dynamic ScreenReflected;

		public Screen(GameScreen gameScreen)
		{
			GameScreen = gameScreen;
			ScreenReflected = gameScreen.Reflect();
		}

		public virtual void Update(InputState input)
		{
		}

		public virtual void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
		}
	}
}
