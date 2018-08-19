using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core;
using Timespinner.GameAbstractions;
using TsRanodmizer.Extensions;

namespace TsRanodmizer.Drawables
{
	interface IDrawable
	{
		void SetDrawPoint(Point newDrawPoint);

		void Draw(SpriteBatch spriteBatch);
	}

	class SeedRepresentation : IDrawable
	{
		public int IconSize { get; set; }
		Point drawPoint;
		readonly SpriteSheet menuIcons;

		public SeedRepresentation(Point drawPoint, int iconSize, GCM gcm)
		{
			this.drawPoint = drawPoint;
			this.IconSize = iconSize;
			menuIcons = (SpriteSheet) gcm.Reflect().Get("Sprites/Items/MenuIcons");
		}

		public void SetDrawPoint(Point newDrawPoint)
		{
			drawPoint = newDrawPoint;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			var random = new Random(~Seed.Current);
			var numberOfItemsToDraw = 5;

			for (int i = 0; i < numberOfItemsToDraw; i++)
			{
				var position = new Rectangle(drawPoint.X + (i * IconSize), drawPoint.Y, IconSize, IconSize);
				var spritePosition = menuIcons.FrameStarts[random.Next(menuIcons.FrameStarts.Count)];
				var sprite = new Rectangle(spritePosition.X, spritePosition.Y, menuIcons.FrameSize.X, menuIcons.FrameSize.Y);

				spriteBatch.Draw(menuIcons.Texture, position, sprite, Color.White);
			}
		}
	}
}
