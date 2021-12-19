using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core;
using Timespinner.GameAbstractions;
using TsRandomizer.Extensions;

namespace TsRandomizer.Drawables
{
	class SeedRepresentation : IDrawable
	{
		const int NumberOfItemsToDraw = 5;

		public int IconSize { get; set; }

		public bool ShowSeedId { get; set; }

		Seed? seed;

		Point drawPoint = Point.Zero;
		Vector2 origin = Vector2.Zero;

		readonly GCM gcm;
		readonly bool drawBackdrop;
		readonly SpriteSheet menuIcons;

		public float Width => NumberOfItemsToDraw * IconSize;

		public SeedRepresentation(GCM gcm, bool drawBackdrop = true)
			: this(null, gcm, drawBackdrop)
		{
		}

		public SeedRepresentation(Seed? seed, GCM gcm, bool drawBackdrop = true) 
		{
			this.seed = seed;
			this.gcm = gcm;
			this.drawBackdrop = drawBackdrop;
			menuIcons = (SpriteSheet)gcm.AsDynamic().Get("Sprites/Items/MenuIcons");
		}

		public void SetDrawPoint(Point newDrawPoint, Vector2 newOrigin = new Vector2())
		{
			drawPoint = newDrawPoint;
			if(newOrigin != Vector2.Zero)
				origin = newOrigin;
		}

		public void SetSeed(Seed selectedSeed) => seed = selectedSeed;

		public void Draw(SpriteBatch spriteBatch)
		{
			if(drawBackdrop)
				DrawBackdrop(spriteBatch, NumberOfItemsToDraw);

			if (!seed.HasValue)
			{
				DrawSeedString(spriteBatch, "Invalid", Color.Red);
				return;
			}

			if (ShowSeedId)
			{
				if (seed.Value.Options.Archipelago)
					DrawSeedString(spriteBatch, "Archipelago", Color.Yellow);
				else
				{
					DrawBackdrop(spriteBatch, seed.Value.ToString());
					DrawSeedString(spriteBatch, seed.Value.ToString());
				}
			}
			else
			{
				var random = new Random(~(int)seed.Value.Id);

				for (uint i = 0; i < seed.Value.Options.Flags; i++)
					random.Next();

				for (var i = 0; i < NumberOfItemsToDraw; i++)
					DrawItemIcon(spriteBatch, i, random);
			}
		}

		void DrawSeedString(SpriteBatch spriteBatch, string seedId) => 
			DrawSeedString(spriteBatch, seedId, Color.WhiteSmoke);

		void DrawSeedString(SpriteBatch spriteBatch, string seedId, Color color)
		{
			var font = gcm.LatinFont;
			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;
			var position = new Vector2(drawPoint.X, drawPoint.Y - ((font.LineSpacing / 2) * inGameZoom));

			spriteBatch.DrawString(font, seedId, position, color, inGameZoom);	
		}

		void DrawItemIcon(SpriteBatch spriteBatch, int i, Random random)
		{
			var position = new Rectangle(drawPoint.X + (i * IconSize), drawPoint.Y, IconSize, IconSize);
			var spritePosition = menuIcons.FrameStarts.SelectRandom(random);
			var sprite = new Rectangle(spritePosition.X, spritePosition.Y, menuIcons.FrameSize.X, menuIcons.FrameSize.Y);

			spriteBatch.Draw(menuIcons.Texture, position, sprite, Color.White, 0, origin, SpriteEffects.None, 1);
		}

		void DrawBackdrop(SpriteBatch spriteBatch, int numberOfItemsToDraw)
		{
			var backdropColor = Color.Black;
			backdropColor.A = 200;

			var backdropArea = new Rectangle(drawPoint.X, drawPoint.Y, numberOfItemsToDraw * IconSize, IconSize);

			spriteBatch.Draw(gcm.TxBlankSquare, backdropArea, null, backdropColor, 0, origin, SpriteEffects.None, 1);
		}

		void DrawBackdrop(SpriteBatch spriteBatch, string message)
		{
			var backdropColor = Color.Black;
			backdropColor.A = 200;

			var backdropArea = new Rectangle(drawPoint.X, drawPoint.Y, (int)(gcm.LatinFont.MeasureString(message).X * (int)TimeSpinnerGame.Constants.InGameZoom), IconSize);

			spriteBatch.Draw(gcm.TxBlankSquare, backdropArea, null, backdropColor, 0, origin, SpriteEffects.None, 1);
		}
	}
}
