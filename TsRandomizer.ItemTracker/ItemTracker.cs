using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using TsRandomizer.ItemTracker;
using TsRandomizer.Extensions;

namespace TsRandomizerItemTracker
{
	class ItemTracker : Game
	{
		readonly GraphicsDeviceManager graphics;
		readonly SpriteBatch spriteBatch;
		
		ItemTrackerState trackerState;

		TrackerRenderer trackerRenderer;
		BackgroundRenderer backgroundRenderer;

		SpriteFont font;

		double trackerUpdateTimer = 1000;

		MouseInputHandler mouseInputHandler;

		public ItemTracker()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 0,
				PreferredBackBufferHeight = 0
			};

			spriteBatch = new SpriteBatch(GraphicsDevice);

			Content.RootDirectory = "Content";

			TargetElapsedTime = TimeSpan.FromSeconds(1/60d); //60 fps

			Window.AllowUserResizing = true;
			Window.ClientSizeChanged += Window_ClientSizeChanged;
		}

		protected override void Initialize()
		{
			base.Initialize();

			mouseInputHandler = new MouseInputHandler(OnDoubleClick, OnRightClick, OnScroll);
		}

		void Window_ClientSizeChanged(object sender, EventArgs e)
		{
			Window.ClientSizeChanged -= Window_ClientSizeChanged;

			trackerRenderer.SetWidth(Window.ClientBounds.Width);

			UpdateWindowSize();

			Window.ClientSizeChanged += Window_ClientSizeChanged;
		}

		void UpdateWindowSize()
		{
			var size = trackerRenderer.GetSize();
			graphics.PreferredBackBufferWidth = size.X;
			graphics.PreferredBackBufferHeight = size.Y;
			graphics.ApplyChanges();
		}

		void OnDoubleClick()
		{
			Window.IsBorderlessEXT = !Window.IsBorderlessEXT;
		}

		void OnRightClick()
		{
			backgroundRenderer.NextBackground();
		}

		void OnScroll(int scrolledAmount)
		{
			if (scrolledAmount > 0)
			{
				trackerRenderer.IconSize += 2;
			}
			else
			{
				if (trackerRenderer.IconSize > 8)
					trackerRenderer.IconSize -= 2;
			}

			UpdateWindowSize();
		}

		protected override void LoadContent()
		{
			font = Content.Load<SpriteFont>("Fonts/LatinFont");
			font.LineSpacing = 16;

			var gcm = new GCM();

			trackerRenderer = new TrackerRenderer(gcm, Content);
			backgroundRenderer = new BackgroundRenderer(gcm, Content);

			UpdateWindowSize();
		}

		protected override void Update(GameTime gameTime)
		{
			if(IsActive)
				mouseInputHandler.Update(gameTime);

			trackerUpdateTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
			if (trackerUpdateTimer > 1000)
			{
				trackerState = ItemTrackerUplink.LoadState();

				trackerUpdateTimer = 0;
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.Black);

			backgroundRenderer.Draw(spriteBatch, graphics.GraphicsDevice.Viewport.Bounds);

			if (trackerState != null)
				trackerRenderer.Draw(spriteBatch, trackerState);
			else
			{
				using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
					spriteBatch.DrawString(font, "Awaiting data", new Vector2(8, 8), Color.Black);
			}

			base.Draw(gameTime);
		}
	}
}
