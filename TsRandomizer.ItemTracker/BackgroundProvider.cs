using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core;
using Timespinner.GameAbstractions;
using TsRandomizer.Extensions;

namespace TsRandomizerItemTracker
{
	class BackgroundRenderer
	{
		int currentBackground;

		readonly Background[] backgrounds;
		public int NumberOfBackgrounds => backgrounds.Length;

		public BackgroundRenderer(GCM gcm, ContentManager contentManager)
		{
			var blank = contentManager.Load<Texture2D>("Overlays/BlankSquare");
			var pauseMenu = (SpriteSheet)gcm.AsDynamic().Get("Overlays/Menu/PauseMenu", contentManager);

			backgrounds = new[]
			{
				new Background { Texture = blank, Color = Color.WhiteSmoke },
				new Background { Texture = blank, Color = Color.BlueViolet },
				new Background { Texture = blank, Color = Color.Pink },
				new Background { Texture = blank, Color = Color.IndianRed },
				new Background { Texture = blank, Color = Color.DarkGray },
				new Background { Texture = blank, Color = Color.Goldenrod },
				new Background { Texture = blank, Color = Color.GreenYellow },
				new Background { Texture = blank, Color = Color.DarkKhaki },

				new Background { Texture = pauseMenu.Texture, Souce = new Rectangle(48, 112, 16, 16), Color = Color.White },
				new Background { Texture = pauseMenu.Texture, Souce = new Rectangle(48, 96, 16, 16), Color = Color.White },
				new Background { Texture = pauseMenu.Texture, Souce = new Rectangle(112, 96, 16, 16), Color = Color.White },
			};
		}

		public void SetBackground(int index)
		{
			currentBackground = index;

			if (currentBackground >= backgrounds.Length)
				currentBackground = 0;
		}

		public void Draw(SpriteBatch spriteBatch, Rectangle backdropArea)
		{
			var background = backgrounds[currentBackground];

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap))
				spriteBatch.Draw(background.Texture, backdropArea, background.Souce, background.Color);
		}

		class Background
		{
			public Texture2D Texture;
			public Rectangle Souce;
			public Color Color;
		}
	}
}
