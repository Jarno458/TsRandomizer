using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameStateManagement.ScreenManager;

namespace TsRandomizer.Archipelago
{
	abstract class Overlay
	{
		static readonly List<Overlay> Overlays = new List<Overlay>();	

		protected static void Add(Overlay overlay) => Overlays.Add(overlay);

		public static void UpdateAll(GameTime gameTime, InputState input)
		{
			foreach (var overlay in Overlays)
				overlay.Update(gameTime, input);
		}

		public virtual void Update(GameTime gameTime, InputState input)
		{
		}

		public static void DrawAll(SpriteBatch spriteBatch, Rectangle screenSize)
		{
			foreach (var overlay in Overlays)
				overlay.Draw(spriteBatch, screenSize);
		}

		public virtual void Draw(SpriteBatch spriteBatch, Rectangle screenSize)
		{
		}
	}
}