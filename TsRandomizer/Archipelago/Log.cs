using System;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using TsRandomizer.Extensions;

namespace TsRandomizer.Archipelago
{
	class Log : Overlay
	{
		const int FadeDelayInSeconds = 5;

		readonly GCM gcm;

		readonly ConcurrentQueue<Message> lines = new ConcurrentQueue<Message>();

		public Log(GCM gcm)
		{
			this.gcm = gcm;

			Add(this);
		}

		public void Add(string message)
		{
			lines.Enqueue(new Message(message));
		}

		public void Add(params Part[] parts)
		{
			lines.Enqueue(new Message(parts));
		}

		public override void Update(GameTime gameTime)
		{
			while (lines.TryPeek(out var message) && DateTime.UtcNow - message.TimeAdded > TimeSpan.FromSeconds(FadeDelayInSeconds))
				lines.TryDequeue(out _);
		}

		public override void Draw(SpriteBatch spriteBatch, Rectangle screenSize)
		{
			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
			{
				var i = 1;
				foreach (var message in lines)
				{
					var lineHeight = gcm.LatinFont.LineSpacing * (int)TimeSpinnerGame.Constants.InGameZoom * 0.5f;

					var point = new Point(screenSize.X, (int)(screenSize.Height - lineHeight/2 - lineHeight*i));

					DrawBackdrop(spriteBatch, point, screenSize, (int)lineHeight);
					DrawMessage(spriteBatch, point, message);

					i++;
				}
			}
		}

		void DrawBackdrop(SpriteBatch spriteBatch, Point drawPoint, Rectangle screenSize, int lineHeight)
		{
			var backdropColor = Color.Black;
			backdropColor.A = 200;

			var origin = new Vector2(0,0);

			var backdropArea = new Rectangle(drawPoint.X, drawPoint.Y, screenSize.Width, lineHeight);

			spriteBatch.Draw(gcm.TxBlankSquare, backdropArea, null, backdropColor, 0, origin, SpriteEffects.None, 1);
		}

		void DrawMessage(SpriteBatch spriteBatch, Point drawPoint, Message message)
		{
			var font = gcm.LatinFont;
			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;
			float partOffset = 0;

			foreach (var part in message.Parts)
			{
				var position = new Vector2(drawPoint.X + partOffset, drawPoint.Y);

				spriteBatch.DrawString(font, part.Text, position, part.Color, inGameZoom * 0.5f);

				partOffset += font.MeasureString(part.Text).X * inGameZoom * 0.5f;
			}
		}
	}

	class Message
	{
		public DateTime TimeAdded { get; }
		public Part[] Parts { get; }

		public Message(string message) : this(new []{ new Part(message)})
		{
		}

		public Message(Part[] parts)
		{
			TimeAdded = DateTime.UtcNow;
			Parts = parts;
		}
	}

	class Part
	{
		public string Text { get; }
		public Color Color { get; }

		public Part(string text) : this(text, Color.White)
		{
		}

		public Part(string text, Color color)
		{
			Text = text;
			Color = color;
		}
	}
}
