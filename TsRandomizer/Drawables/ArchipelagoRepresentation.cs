using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation.ItemPlacers;

namespace TsRandomizer.Drawables
{
	class ArchipelagoRepresentation : IDrawable
	{
		public bool ShowArchipelagoInfo { get; set; }

		GameSave save;

		Point drawPoint = Point.Zero;
		Vector2 origin = Vector2.Zero;

		readonly GCM gcm;

		public ArchipelagoRepresentation(GCM gcm)
			: this(null, gcm)
		{
		}

		public ArchipelagoRepresentation(GameSave save, GCM gcm)
		{
			this.save = save;
			this.gcm = gcm;
		}

		public void SetDrawPoint(Point newDrawPoint, Vector2 newOrigin = new Vector2())
		{
			drawPoint = newDrawPoint;
			if (newOrigin != Vector2.Zero)
				origin = newOrigin;
		}

		public void SetSave(GameSave selectedSave) => save = selectedSave;

		public void Draw(SpriteBatch spriteBatch)
		{

			if (save == null)
			{
				DrawArchipelagoInfo(spriteBatch, "Invalid", "", Color.Red);
				return;
			}

			if (ShowArchipelagoInfo)
			{
				var hasServerInfo = save.DataKeyStrings.TryGetValue(ArchipelagoItemLocationRandomizer.GameSaveServerKey, out var server);
				var hasUserInfo = save.DataKeyStrings.TryGetValue(ArchipelagoItemLocationRandomizer.GameSaveUserKey, out var user);
				if (!hasServerInfo && !hasUserInfo)
				{
					//Non-Archipelago Seed
				}
				else
				{
					DrawArchipelagoInfo(spriteBatch, server, user);
				}
			}
		}

		void DrawArchipelagoInfo(SpriteBatch spriteBatch, string server, string user) =>
			DrawArchipelagoInfo(spriteBatch, server, user, Color.WhiteSmoke);

		void DrawArchipelagoInfo(SpriteBatch spriteBatch, string server, string user,  Color color)
		{
			DrawBackdrop(spriteBatch, server, user);

			var font = gcm.LatinFont;
			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;
			var position = new Vector2(drawPoint.X, drawPoint.Y - ((font.LineSpacing / 2) * inGameZoom));

			spriteBatch.DrawString(font, server, position, color, inGameZoom);
			position.Y += font.LineSpacing * inGameZoom;
			spriteBatch.DrawString(font, user, position, color, inGameZoom);
		}

		void DrawBackdrop(SpriteBatch spriteBatch, string firstLine, string secondLine)
		{
			var backdropColor = Color.Black;
			backdropColor.A = 200;
			var font = gcm.LatinFont;
			var inGameZoom = TimeSpinnerGame.Constants.InGameZoom;

			var width = Math.Max((int)(font.MeasureString(firstLine).X * (int)inGameZoom), (int)(font.MeasureString(secondLine).X * (int)inGameZoom));
			var height = (int)(font.MeasureString(firstLine).Y * (int)inGameZoom);
			var y = drawPoint.Y;
			if (!string.IsNullOrEmpty(secondLine))
			{
				height *= 2;
				y += ((font.LineSpacing /2) * inGameZoom);
			}

			var backdropArea = new Rectangle(drawPoint.X, y, width, height);

			spriteBatch.Draw(gcm.TxBlankSquare, backdropArea, null, backdropColor, 0, origin, SpriteEffects.None, 1);
		}
	}
}
