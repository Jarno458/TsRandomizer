using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Settings;

namespace TsRandomizer.Randomisation
{
	static class SpriteManager
	{
		public static void ReplaceLunaisSprite(Level level)
		{
			var lunais = level.MainHero.AsDynamic();
			var atlas = level.GCM.AsDynamic()._textureAtlasDatabase;
			var spritePath = level.GameSave.GetSettings().LunaisSprite.Value; //.AllowedValues[2];
			atlas.TextureAtlasSpecifications["Sprites/Heroes/LunaisSprite"].ContentPath = $"..//{spritePath}";
			lunais._sprite = level.GCM.AsDynamic().Get("Sprites/Heroes/LunaisSprite");
		}
	}
}
