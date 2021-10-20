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
		const double FadeStartDelayInSeconds = 4;
		const double FadeEndDelayInSeconds = 5;

		static readonly TimeSpan FadeStart = TimeSpan.FromSeconds(FadeStartDelayInSeconds);
		static readonly TimeSpan FadeEnd = TimeSpan.FromSeconds(FadeEndDelayInSeconds);
		static readonly TimeSpan FadeTime = FadeEnd - FadeStart;

		readonly GCM gcm;

		readonly ConcurrentQueue<Message> lines = new ConcurrentQueue<Message>();
		readonly ConcurrentQueue<Message> pendingImportantLines = new ConcurrentQueue<Message>();
		readonly ConcurrentQueue<Message> pendingLines = new ConcurrentQueue<Message>();

		public Log(GCM gcm)
		{
			this.gcm = gcm;

			Add(this);
		}

		public void Add(bool important, params Part[] parts)
		{
			if (important)
				pendingImportantLines.Enqueue(new Message(parts));
			else
				pendingLines.Enqueue(new Message(parts));
		}

		public override void Update(GameTime gameTime)
		{
			while (lines.TryPeek(out var message) && message.OnScreenTime > FadeEnd)
				lines.TryDequeue(out _);

			CopyMessagesBetweenQueues(lines, pendingImportantLines);
			CopyMessagesBetweenQueues(lines, pendingLines);
		}

		static void CopyMessagesBetweenQueues(
			ConcurrentQueue<Message> destination, ConcurrentQueue<Message> source, int maxDestinationCount = 6)
		{
			while (destination.Count < maxDestinationCount && source.Count >= 1)
			{
				if (!source.TryDequeue(out var message))
					return;

				message.TimeAdded = DateTime.UtcNow;

				destination.Enqueue(message);
			}
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

					double fade = (message.OnScreenTime > FadeStart)
						? 1 - ((message.OnScreenTime - FadeStart).Ticks / (double)FadeTime.Ticks)
						: 1;
					
					DrawBackdrop(spriteBatch, point, screenSize, (int)lineHeight, fade);
					DrawMessage(spriteBatch, point, message, fade);

					i++;
				}
			}
		}

		void DrawBackdrop(SpriteBatch spriteBatch, Point drawPoint, Rectangle screenSize, int lineHeight, double alpha)
		{
			var backdropColor = Color.Black;
			backdropColor.A = (byte)(200 * alpha);

			var origin = new Vector2(0,0);

			var backdropArea = new Rectangle(drawPoint.X, drawPoint.Y, screenSize.Width, lineHeight);

			spriteBatch.Draw(gcm.TxBlankSquare, backdropArea, null, backdropColor, 0, origin, SpriteEffects.None, 1);
		}

		void DrawMessage(SpriteBatch spriteBatch, Point drawPoint, Message message, double alpha)
		{
			var font = gcm.LatinFont;
			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;
			float partOffset = 0;

			foreach (var part in message.Parts)
			{
				var position = new Vector2(drawPoint.X + partOffset, drawPoint.Y);
				var color = part.Color;
				color.A = (byte)(255 * alpha);

				spriteBatch.DrawString(font, part.Text, position, color, inGameZoom * 0.5f);

				partOffset += font.MeasureString(part.Text).X * inGameZoom * 0.5f;
			}
		}
	}

	class Message
	{
		public TimeSpan OnScreenTime => DateTime.UtcNow - TimeAdded;

		public DateTime TimeAdded { get; set; }
		public Part[] Parts { get; }

		public Message(Part[] parts)
		{
			TimeAdded = DateTime.MaxValue;
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
