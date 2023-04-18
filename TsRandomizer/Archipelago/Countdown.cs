using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using ScreenManager = TsRandomizer.Screens.ScreenManager;

namespace TsRandomizer.Archipelago
{
	class Countdown : Overlay
	{
		static readonly TimeSpan FadeStart = TimeSpan.FromSeconds(1);
		static readonly TimeSpan FadeEnd = TimeSpan.FromSeconds(1.2);
		static readonly TimeSpan FadeTime = FadeEnd - FadeStart;

		readonly int countdown;
		readonly DateTime created;

		readonly float zoom;

		TimeSpan OnScreenTime => DateTime.UtcNow - created;

		bool triggeredSound;

		public Countdown(int currentCountdown)
		{
			countdown = currentCountdown;
			created = DateTime.UtcNow;

			zoom = TimeSpinnerGame.Constants.InGameZoom * 2;

			Add(this);
		}

		public override void Update(GameTime gameTime, InputState input, Jukebox jukebox)
		{
			if (!triggeredSound)
			{
				triggeredSound = true;

				if (countdown > 3)
					jukebox.PlayCue(ESFX.LunaisOrbGunMelee);
				else if (countdown > 0)
					jukebox.PlayCue(ESFX.EnemyMeteorSparrowDashImpact);
				else
					jukebox.PlayCue(ESFX.BossDeathFinalHit);
			}

			if (OnScreenTime > FadeEnd)
				Delete();
		}

		string GetMessage() => countdown > 0 ? $"Countdown: {countdown}" : "Go!";

		public override void Draw(SpriteBatch spriteBatch, Rectangle screenSize, GCM gcm)
		{
			if(ScreenManager.IsConsoleOpen)
				return;

			using (spriteBatch.BeginUsing())
			{
				var message = GetMessage();

				var messageSize = gcm.LatinFont.MeasureString(message) * zoom;
				
				var point = new Point(
					(int)((screenSize.Width / 2f) - (messageSize.X / 2)), 
					(int)((screenSize.Height / 2f) - (messageSize.Y / 2)));

				var fade = (OnScreenTime > FadeStart)
					? 1 - ((OnScreenTime - FadeStart).Ticks / (double)FadeTime.Ticks)
					: 1;
				
				DrawBackdrop(spriteBatch, gcm, point, messageSize, fade);
				DrawMessage(spriteBatch, gcm, point, message, fade);
			}
		}

		void DrawBackdrop(SpriteBatch spriteBatch, GCM gcm, Point drawPoint, Vector2 messageSize, double alpha)
		{
			var backdropColor = Color.Black;
			backdropColor.A = (byte)(200 * alpha);

			var backdropArea = new Rectangle(
				drawPoint.X - (int)(1 * zoom), 
				drawPoint.Y, 
				(int)(messageSize.X + (int)(2 * zoom)),
				(int)messageSize.Y);

			spriteBatch.Draw(gcm.TxBlankSquare, backdropArea, null, backdropColor, 0, Vector2.Zero, SpriteEffects.None, 1);
		}

		void DrawMessage(SpriteBatch spriteBatch, GCM gcm, Point drawPoint, string message, double alpha)
		{
			var color = Color.White;
			color.A = (byte)(255 * alpha);

			var font = gcm.LatinFont;
			var position = new Vector2(drawPoint.X, drawPoint.Y);

			spriteBatch.DrawString(font, message, position, color, zoom);
		}
	}
}
