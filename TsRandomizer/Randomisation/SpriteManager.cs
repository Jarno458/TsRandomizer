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

			level.GCM.SpFamiliarCrow = LoadCustomSprite(level, "Sprites/Heroes/FamiliarCrow", level.GameSave.GetSettings().MerchantCrowSprite.Value);
			level.GCM.SpFamiliarAltCrow = LoadCustomSprite(level, "Sprites/Heroes/FamiliarAltCrow", level.GameSave.GetSettings().MerchantCrowGreedSprite.Value);

			level.GCM.SpFamiliarKobo = LoadCustomSprite(level, "Sprites/Heroes/FamiliarKobo", level.GameSave.GetSettings().KoboSprite.Value);
			level.GCM.SpFamiliarDemon = LoadCustomSprite(level, "Sprites/Heroes/FamiliarDemon", level.GameSave.GetSettings().DemonSprite.Value);
			level.GCM.SpFamiliarGriffin = LoadCustomSprite(level, "Sprites/Heroes/FamiliarGriffin", level.GameSave.GetSettings().GriffinSprite.Value);
			level.GCM.SpFamiliarSprite = LoadCustomSprite(level, "Sprites/Heroes/FamiliarSprite", level.GameSave.GetSettings().SpriteFamiliarSprite.Value);

			ReloadLunaisSprite(level);
		}

		static SpriteSheet LoadCustomSprite(Level level, string spriteKey, string spritePath)
		{
			var atlas = level.GCM.AsDynamic()._textureAtlasDatabase;
			atlas.TextureAtlasSpecifications[spriteKey].ContentPath = $"..//{spritePath}";
			return level.GCM.AsDynamic().Get(spriteKey);
		}

		public static void ReloadLunaisSprite(Level level)
		{
			// Lunais is loaded before the GameplayScreen hook, and as such her `_sprite` needs to be reset or it will be the default
			// Familiars are loaded after the hook and apply automatically
			SpriteSheet lunaisSprite = level.GCM.SpLunais;
			if (level.GameSave.HasRelic(Timespinner.GameAbstractions.Inventory.EInventoryRelicType.EternalBrooch))
				lunaisSprite = level.GCM.SpAltLunais2;
			else if (level.GameSave.HasRelic(Timespinner.GameAbstractions.Inventory.EInventoryRelicType.EmpireBrooch))
				lunaisSprite = level.GCM.SpAltLunais;
			
			level.MainHero.AsDynamic()._sprite = lunaisSprite;
		}
	}
}
