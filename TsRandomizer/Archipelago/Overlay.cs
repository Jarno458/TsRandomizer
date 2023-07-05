using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;

namespace TsRandomizer.Archipelago
{
	abstract class Overlay
	{
		static readonly ConcurrentDictionary<Overlay, bool> Overlays = new ConcurrentDictionary<Overlay, bool>();

		static readonly object LockObj = new object();

		protected static void Add(Overlay overlay) => Overlays.TryAdd(overlay, false);

		public static void UpdateAll(GameTime gameTime, InputState input, Jukebox jukebox)
		{
			lock (LockObj)
			{
				foreach (var overlay in Overlays.Keys)
					overlay.Update(gameTime, input, jukebox);
			}
		}

		public virtual void Update(GameTime gameTime, InputState input, Jukebox jukebox)
		{
		}

		public static void DrawAll(SpriteBatch spriteBatch, Rectangle screenSize, GCM gcm)
		{
			foreach (var overlay in Overlays.Keys)
				overlay.Draw(spriteBatch, screenSize, gcm);
		}

		public virtual void Draw(SpriteBatch spriteBatch, Rectangle screenSize, GCM gcm)
		{
		}

		protected void Delete() => Overlays.TryRemove(this, out _);
	}
}