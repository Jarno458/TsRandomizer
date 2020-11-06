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
		readonly TrackerSettings settings;

		ItemTrackerState trackerState;

		TrackerRenderer trackerRenderer;
		BackgroundRenderer backgroundRenderer;

		SpriteFont font;

		double trackerUpdateTimer = 1000;

		MouseInputHandler mouseInputHandler;

		bool oneTimeSetup = true;

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

			settings = TrackerSettings.LoadSettings();
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

			settings.WindowSize = trackerRenderer.GetSize();

			UpdateWindowSize();

			Window.ClientSizeChanged += Window_ClientSizeChanged;
		}

		void UpdateWindowSize()
		{
			graphics.PreferredBackBufferWidth = settings.WindowSize.X;
			graphics.PreferredBackBufferHeight = settings.WindowSize.Y;
			graphics.ApplyChanges();
		}

		void OnDoubleClick()
		{
			settings.Borderless = !settings.Borderless;
			Window.IsBorderlessEXT = settings.Borderless;
		}

		void OnRightClick()
		{
			settings.BackgrondIndex++;

			if (settings.BackgrondIndex >= backgroundRenderer.NumberOfBackgrounds)
				settings.BackgrondIndex = 0;

			backgroundRenderer.SetBackground(settings.BackgrondIndex);
		}

		void OnScroll(int scrolledAmount)
		{
			if (scrolledAmount > 0)
			{
				settings.IconSize += 2;
			}
			else
			{
				if (settings.IconSize > 8)
					settings.IconSize -= 2;
			}

			trackerRenderer.IconSize = settings.IconSize;
			settings.WindowSize = trackerRenderer.GetSize();

			UpdateWindowSize();
		}

		protected override void LoadContent()
		{
			font = Content.Load<SpriteFont>("Fonts/LatinFont");
			font.LineSpacing = 16;

			var gcm = new GCM();

			trackerRenderer = new TrackerRenderer(gcm, Content) { IconSize = settings.IconSize };
			trackerRenderer.SetWidth(settings.WindowSize.X);

			UpdateWindowSize();

			backgroundRenderer = new BackgroundRenderer(gcm, Content);
			backgroundRenderer.SetBackground(settings.BackgrondIndex);

			Window.IsBorderlessEXT = settings.Borderless;

			settings.EnableSaving();
		}

		protected override void Update(GameTime gameTime)
		{
			if (oneTimeSetup)
			{
				Window.AllowUserResizing = true;
				Window.ClientSizeChanged += Window_ClientSizeChanged;
				oneTimeSetup = false;
			}

			if (IsActive)
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

			backgroundRenderer.Draw(spriteBatch, graphics.GraphicsDevice.Viewport.Bounds, trackerRenderer.IconSize);

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
