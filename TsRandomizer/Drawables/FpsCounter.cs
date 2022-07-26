using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using TsRandomizer.Extensions;

namespace TsRandomizer.Drawables
{
	public class FrameCounter : IDrawable
	{
		float elsapsedTime;
		uint frames;

		readonly GCM gcm;

		Point drawPoint = new Point(50, 50);

		public float AverageFramesPerSecond { get; private set; }

		public FrameCounter(GCM gcm)
		{
			this.gcm = gcm;
		}

		public void Update(float deltaTime)
		{
			elsapsedTime += deltaTime;
			frames++;

			if (elsapsedTime > 1)
			{
				AverageFramesPerSecond = frames;

				elsapsedTime -= 1;
				frames = 0;
			}
		}

		public void SetDrawPoint(Point newDrawPoint, Vector2 newOrigin = new Vector2())
		{
			drawPoint = newDrawPoint;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			var font = gcm.LatinFont;
			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;
			var position = new Vector2(drawPoint.X, drawPoint.Y - ((font.LineSpacing / 2) * inGameZoom));

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
				spriteBatch.DrawString(font, $"FPS: {AverageFramesPerSecond:###}", position, Color.DarkGreen, inGameZoom * 0.5f);
		}
	}
}
