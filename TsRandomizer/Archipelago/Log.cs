using System;
using System.Collections.Concurrent;
using Archipelago.MultiClient.Net.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.Settings;
using ScreenManager = TsRandomizer.Screens.ScreenManager;

namespace TsRandomizer.Archipelago
{
	class Log : Overlay
	{
		static SettingCollection settings;

		TimeSpan fadeStart;
		TimeSpan fadeEnd;
		TimeSpan fadeTime;

		readonly GCM gcm;

		readonly ConcurrentQueue<Message> lines = new ConcurrentQueue<Message>();
		readonly ConcurrentQueue<Message> pendingImportantLines = new ConcurrentQueue<Message>();
		readonly ConcurrentQueue<Message> pendingLines = new ConcurrentQueue<Message>();

		public Log(GCM gcm)
		{
			this.gcm = gcm;

			SetSettings(GameSettingsLoader.LoadSettingsFromFile());

			Add(this);
		}

		public void SetSettings(SettingCollection settingsCollection)
		{
			settings = settingsCollection;

			fadeEnd = TimeSpan.FromSeconds(settingsCollection.OnScreenLogLineScreenTime.Value);
			fadeStart = TimeSpan.FromSeconds(settingsCollection.OnScreenLogLineScreenTime.Value * 0.8);
			fadeTime = fadeEnd - fadeStart;
		}

		public void AddSystemMessage(params Part[] parts)
		{
			if (!settings.ShowSystemMessages.Value)
				return;

			pendingImportantLines.Enqueue(new Message(parts));
		}
		
		public void Add(bool isSendByMe, bool isReceivedByMe, ItemFlags itemFlags, params Part[] parts)
		{
			if (settings.NumberOfOnScreenLogLines.Value == 0)
				return;

			if (isSendByMe || isReceivedByMe)
			{
				if (isReceivedByMe && !settings.ShowReceivedItemsFromMe.Value)
					return;
				if (isSendByMe && !settings.ShowSendItemsFromMe.Value)
					return;

				pendingImportantLines.Enqueue(new Message(parts));
			}
			else
			{
				if (itemFlags.HasFlag(ItemFlags.Advancement) && !settings.ShowSendProgressionItems.Value)
					return;
				if (itemFlags.HasFlag(ItemFlags.NeverExclude) && !settings.ShowSendImportantItems.Value)
					return;
				if (itemFlags.HasFlag(ItemFlags.Trap) && !settings.ShowSendTrapItems.Value)
					return;
				if (itemFlags == ItemFlags.None && !settings.ShowSendGenericItems.Value)
					return;

				pendingLines.Enqueue(new Message(parts));
			}
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			while (lines.TryPeek(out var message) && message.OnScreenTime > fadeEnd)
				lines.TryDequeue(out _);

			var maxLines = (int)settings.NumberOfOnScreenLogLines.Value;
			if (maxLines == 0)
			{
				while (!pendingImportantLines.IsEmpty)
					pendingImportantLines.TryDequeue(out _);
				while (!pendingLines.IsEmpty)
					pendingLines.TryDequeue(out _);
			}
			else
			{
				CopyMessagesBetweenQueues(pendingImportantLines, lines, maxLines);
				CopyMessagesBetweenQueues(pendingLines, lines, maxLines);
			}
		}

		static void CopyMessagesBetweenQueues(
			ConcurrentQueue<Message> source, ConcurrentQueue<Message> destination, int maxDestinationCount)
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
			if(ScreenManager.IsConsoleOpen)
				return;

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
			{
				var i = 1;
				foreach (var message in lines)
				{
					var lineHeight = gcm.LatinFont.LineSpacing * (int)TimeSpinnerGame.Constants.InGameZoom * 0.5f;

					var point = new Point(screenSize.X, (int)(screenSize.Height - lineHeight/2 - lineHeight*i));

					var fade = (message.OnScreenTime > fadeStart)
						? 1 - ((message.OnScreenTime - fadeStart).Ticks / (double)fadeTime.Ticks)
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
