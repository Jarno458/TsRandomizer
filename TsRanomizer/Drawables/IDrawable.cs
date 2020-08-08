using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TsRandomizer.Drawables
{
	interface IDrawable
	{
		void SetDrawPoint(Point newDrawPoint, Vector2 origin = new Vector2());
		void Draw(SpriteBatch spriteBatch);
	}
}