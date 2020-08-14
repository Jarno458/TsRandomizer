using System;
using System.Runtime.Remoting.Activation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TsRandomizer
{
	class SimpelTestGame : Game
	{
		readonly GraphicsDeviceManager graphics;

		public SimpelTestGame()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 100,
				PreferredBackBufferHeight = 100,
				GraphicsProfile = GraphicsProfile.Reach
			};

			Content.RootDirectory = "Content";
			TargetElapsedTime = TimeSpan.FromTicks(166666L);

			Components.Add(new SimpelComponent(this));
		}

		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.Black);
			base.Draw(gameTime);
		}
	}

	class SimpelComponent : DrawableGameComponent
	{
		ContentManager content;
		Texture2D Icons;

		public SimpelComponent(Game game) : base(game)
		{

		}

		protected override void LoadContent()
		{
			content = Game.Content;
			Icons = content.Load<Texture2D>("Sprites/Items/MenuIcons");
		}


		public override void Draw(GameTime gameTime)
		{

		}


	}
}
