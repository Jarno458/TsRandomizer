using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TsRandomizer.ItemTracker;

namespace TsRandomizerItemTracker.TrackerStyles
{
	interface ITrakcerStyle
	{
		Point GetSize();

		void Draw(SpriteBatch spriteBatch, ItemTrackerState state);
	}
}
