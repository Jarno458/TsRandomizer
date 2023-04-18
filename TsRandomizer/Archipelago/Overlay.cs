using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;

namespace TsRandomizer.Archipelago
{
	abstract class Overlay
	{
		static readonly List<Overlay> Overlays = new List<Overlay>();
		static readonly List<Overlay> OverlaysToDelete = new List<Overlay>();

		static readonly object LockObj = new object();

		protected static void Add(Overlay overlay) => Overlays.Add(overlay);

		public static void UpdateAll(GameTime gameTime, InputState input, Jukebox jukebox)
		{
			lock (LockObj)
			{
				foreach (var overlay in Overlays)
					overlay.Update(gameTime, input, jukebox);

				foreach (var overlay in OverlaysToDelete)
					Overlays.Remove(overlay);

				OverlaysToDelete.Clear();
			}
		}

		public virtual void Update(GameTime gameTime, InputState input, Jukebox jukebox)
		{
		}

		public static void DrawAll(SpriteBatch spriteBatch, Rectangle screenSize, GCM gcm)
		{
			foreach (var overlay in Overlays)
				overlay.Draw(spriteBatch, screenSize, gcm);
		}

		public virtual void Draw(SpriteBatch spriteBatch, Rectangle screenSize, GCM gcm)
		{
		}

		protected void Delete() => OverlaysToDelete.Add(this);
	}
}