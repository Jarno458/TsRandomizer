using System;
using Microsoft.Xna.Framework.Graphics;

namespace TsRandomizer.Extensions
{
    static class SpriteBatchExtensions
    {
	    internal static DisposableSpriteBatch BeginUsing(this SpriteBatch spriteBatch,
		    SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState)
	    {
			spriteBatch.Begin(sortMode, blendState, samplerState, null, null);

			return new DisposableSpriteBatch(spriteBatch);
	    }

	    internal class DisposableSpriteBatch : IDisposable
	    {
		    readonly SpriteBatch spriteBatch;

		    public DisposableSpriteBatch(SpriteBatch spriteBatch)
		    {
			    this.spriteBatch = spriteBatch;
		    }

			public void Dispose()
		    {
				spriteBatch.End();
		    }
	    }
    }
}
