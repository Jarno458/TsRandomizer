using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using TsRandomizer.Extensions;

namespace TsRandomizer.Archipelago
{
	class Log : Overlay
	{
		readonly GCM gcm;

		List<string> Lines = new List<string>
		{
			"This is line 1",
			"This is a totally different line ok",
			"And an other line"
		};

		public Log(GCM gcm)
		{
			this.gcm = gcm;

			Add(this);
		}

		public override void Update(GameTime gameTime)
		{
		}

		public override void Draw(SpriteBatch spriteBatch, Rectangle screenSize)
		{
			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
			{
				for (var i = 1; i <= Lines.Count; i++)
				{
					var lineHeight = gcm.LatinFont.LineSpacing * (int)TimeSpinnerGame.Constants.InGameZoom * 0.5f;

					var point = new Point(screenSize.X, (int)(screenSize.Height - lineHeight/2 - lineHeight*i));

					DrawBackdrop(spriteBatch, point, screenSize, (int)lineHeight);
					DrawMessage(spriteBatch, point, Lines[i-1]);
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

		void DrawMessage(SpriteBatch spriteBatch, Point drawPoint, string message)
		{
			var font = gcm.LatinFont;
			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;
			var position = new Vector2(drawPoint.X, drawPoint.Y);

			var message1 = "This is";
			var message2 = " a different section";
			var message3 = " of a message";

			//spriteBatch.DrawString(font, message, position, Color.White, inGameZoom * 0.5f);

			var message1Length = font.MeasureString(message1).X * inGameZoom * 0.5f;
			var message2Length = font.MeasureString(message2).X * inGameZoom * 0.5f;

			var message2Position = new Vector2(position.X + message1Length, position.Y);
			var message3Position = new Vector2(message2Position.X + message2Length, message2Position.Y);

			spriteBatch.DrawString(font, message1, position, Color.White, inGameZoom * 0.5f);
			spriteBatch.DrawString(font, message2, message2Position, Color.Purple, inGameZoom * 0.5f);
			spriteBatch.DrawString(font, message3, message3Position, Color.Green, inGameZoom * 0.5f);

		}
	}
}
