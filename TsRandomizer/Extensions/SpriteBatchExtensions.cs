using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TsRandomizer.Extensions
{
	public static class SpriteBatchExtensions
	{
		public static void DrawString(
			this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, float zoom) =>
				spriteBatch.DrawString(font, text, position, color, 0, Vector2.Zero, zoom, SpriteEffects.None, 0);

		public static DisposableSpriteBatch BeginUsing(this SpriteBatch spriteBatch) => 
			spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

		public static DisposableSpriteBatch BeginUsing(this SpriteBatch spriteBatch,
			SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState)
		{
			spriteBatch.Begin(sortMode, blendState, samplerState, null, null);

			return new DisposableSpriteBatch(spriteBatch);
		}

		public class DisposableSpriteBatch : IDisposable
		{
			readonly SpriteBatch spriteBatch;

			public DisposableSpriteBatch(SpriteBatch spriteBatch)
			{
				this.spriteBatch = spriteBatch;
			}

			public void Dispose() => spriteBatch.End();
		}
	}
}
