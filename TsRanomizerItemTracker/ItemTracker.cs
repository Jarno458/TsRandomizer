using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TsRandomizerItemTracker.Extensions;

namespace TsRandomizerItemTracker
{
	class ItemTracker : Game
	{
		readonly GraphicsDeviceManager graphics;
		readonly SpriteBatch spriteBatch;

		Texture2D icons;

		public ItemTracker()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 300,
				PreferredBackBufferHeight = 300
			};

			spriteBatch = new SpriteBatch(GraphicsDevice);

			Content.RootDirectory = "Content";

			var framesPerSecond = 10d;
			TargetElapsedTime = TimeSpan.FromSeconds(1/framesPerSecond);
		}

		protected override void LoadContent()
		{
			icons = Content.Load<Texture2D>("Sprites/Items/MenuIcons");
		}

		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.WhiteSmoke);

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
			{
				spriteBatch.Draw(icons, new Rectangle(0, 0, 300, 300), null, Color.White);
			}

			base.Draw(gameTime);
		}
	}
}
