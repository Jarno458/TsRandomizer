using Timespinner.Core;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Extensions;

namespace TsRandomizer.Randomisation
{
	static class SpriteManager
	{
		public static void ReloadCustomSprites(Level level)
		{
			level.GCM.SpLunais = LoadCustomSprite(level, "Sprites/Heroes/LunaisSprite", level.GameSave.GetSettings().LunaisSprite.Value);
			level.GCM.SpAltLunais = LoadCustomSprite(level, "Sprites/Heroes/LunaisAltSprite", level.GameSave.GetSettings().LunaisEternalSprite.Value);
			level.GCM.SpAltLunais2 = LoadCustomSprite(level, "Sprites/Heroes/LunaisAltSprite2", level.GameSave.GetSettings().LunaisGoddessSprite.Value);

			level.GCM.SpFamiliarMeyef = LoadCustomSprite(level, "Sprites/Heroes/FamiliarMeyef", level.GameSave.GetSettings().MeyefSprite.Value);
			level.GCM.SpFamiliarAltMeyef = LoadCustomSprite(level, "Sprites/Heroes/FamiliarAltMeyef", level.GameSave.GetSettings().MeyefWyrmSprite.Value);

			level.GCM.SpFamiliarCrow = LoadCustomSprite(level, "Sprites/Heroes/FamiliarMeyef", level.GameSave.GetSettings().MerchantCrowSprite.Value);
			level.GCM.SpFamiliarAltCrow = LoadCustomSprite(level, "Sprites/Heroes/FamiliarAltCrow", level.GameSave.GetSettings().MerchantCrowGreedSprite.Value);
		}

		static SpriteSheet LoadCustomSprite(Level level, string spriteKey, string spritePath)
		{
			var atlas = level.GCM.AsDynamic()._textureAtlasDatabase;
			atlas.TextureAtlasSpecifications[spriteKey].ContentPath = $"..//{spritePath}";
			return level.GCM.AsDynamic().Get(spriteKey);
		}
	}
}
