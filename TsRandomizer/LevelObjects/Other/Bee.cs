using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core;
using Timespinner.Core.Specifications;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.LakeFly")]
	class Bee : LevelObject<Monster>
	{
		public const int Argument = 2;

		static SpriteSheet beeSpriteSheet;

		public Bee(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
			beeSpriteSheet = beeSpriteSheet ?? GenerateBeeTextureAtlas((TextureAtlas)gameplayScreen.GameContentManager.SpLakeFly);
		}

		protected override void Initialize(Seed seed)
		{
			//LakeFly's dont use Argument therefor they just load the normal LakeFly and its normal LakeFly BestiaryEntrySpecification
			if (Dynamic._argument != Argument)
				return;

			Dynamic._sprite = beeSpriteSheet;

			BestiaryEntrySpecification spec = Dynamic._bestiaryEntry.Duplicate();

			spec.VisibleName = "Bee";

			Dynamic._bestiaryEntry = spec;
		}

		static TextureAtlas GenerateBeeTextureAtlas(TextureAtlas flySheet)
		{
			var source = flySheet.Texture;
			var beeTexture = new Texture2D(source.GraphicsDevice, source.Width, source.Height);

			var pixels = new Color[source.Width * source.Height];

			source.GetData(pixels);

			for (var i = 0; i < pixels.Length; i++)
			{
				if (pixels[i] == Color.Transparent)
					continue;

				var yellow = pixels[i].R * 1.5;
				if (yellow > 255)
					yellow = 255;
				else if (yellow < 150)
					yellow *= 0.6;

				pixels[i].R = (byte)yellow;
				pixels[i].G = (byte)yellow;

				if (pixels[i].B > pixels[i].R)
					pixels[i].B = pixels[i].R;
			}

			beeTexture.SetData(pixels);

			return new TextureAtlas(flySheet.Name, beeTexture, flySheet.AtlasFrames);
		}
	}
}
