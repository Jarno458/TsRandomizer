using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TsRanodmizer.Extensions
{
    static class SpriteBatchExtensions
    {
        public static void DrawString(
            this SpriteBatch spriteBatch,
            SpriteFont font, string text, Vector2 position, Color color, float zoom)
        {
            spriteBatch.DrawString(font, text, position, color, 0, Vector2.Zero, zoom, SpriteEffects.None, 0);
        }
    }
}
