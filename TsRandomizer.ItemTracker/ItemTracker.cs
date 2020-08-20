using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core;
using Timespinner.GameAbstractions;
using TsRandomizer.ItemTracker;
using TsRandomizer.Extensions;
using TsRandomizerItemTracker.TrackerStyles;

namespace TsRandomizerItemTracker
{
	class ItemTracker : Game
	{
		readonly GraphicsDeviceManager graphics;
		readonly SpriteBatch spriteBatch;
		
		ItemTrackerState trackerState;
		ITrakcerStyle trakcerStyle;

		SpriteFont font;

		public ItemTracker()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 0,
				PreferredBackBufferHeight = 0
			};

			spriteBatch = new SpriteBatch(GraphicsDevice);

			Content.RootDirectory = "Content";

			var framesPerSecond = 2d;
			TargetElapsedTime = TimeSpan.FromSeconds(1/framesPerSecond);
		}

		protected override void LoadContent()
		{
			var gcm = new GCM();
			var spriteSheet = (SpriteSheet)gcm.AsDynamic().Get("Sprites/Items/MenuIcons", Content);

			font = Content.Load<SpriteFont>("Fonts/LatinFont");
			font.LineSpacing = 16;

			trakcerStyle = new HorizontalTracker(spriteSheet, font);

			var size = trakcerStyle.GetSize();

			graphics.PreferredBackBufferWidth = size.X;
			graphics.PreferredBackBufferHeight = size.Y;
			graphics.ApplyChanges();
		}

		protected override void Update(GameTime gameTime)
		{
			//trackerState = ItemTrackerUplink.LoadState();
			trackerState = new ItemTrackerState();

#if DEBUG
			Console.Clear();
			XmlSerializer xsSubmit = new XmlSerializer(typeof(ItemTrackerState));

			using (var sww = new StringWriter())
			{
				using (XmlWriter writer = XmlWriter.Create(sww))
				{
					xsSubmit.Serialize(writer, trackerState);
					Console.WriteLine(sww.ToString());
				}
			}
#endif
		}

		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.WhiteSmoke);

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
			{
				if(trackerState != null)
					trakcerStyle.Draw(spriteBatch, trackerState);
				else
					spriteBatch.DrawString(font, "Awaiting data", new Vector2(8, 8), Color.Black);
			}

			base.Draw(gameTime);
		}
	}
}
