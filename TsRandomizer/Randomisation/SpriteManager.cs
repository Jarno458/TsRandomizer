using Timespinner.Core;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.Heroes.Familiars;
using TsRandomizer.Extensions;
using TsRandomizer.Settings;
using TsRandomizer.Settings.GameSettingObjects;

namespace TsRandomizer.Randomisation
{
	static class SpriteManager
	{
		public static void ReloadCustomSprites(Level level, GCM gameContentManager, SettingCollection settings)
		{
			var gcm = gameContentManager.AsDynamic();

			gcm.SpLunais = LoadCustomSprite(gcm, "Sprites/Heroes/LunaisSprite", settings.LunaisSprite);
			gcm.SpAltLunais = LoadCustomSprite(gcm, "Sprites/Heroes/LunaisAltSprite", settings.LunaisEternalSprite);
			gcm.SpAltLunais2 = LoadCustomSprite(gcm, "Sprites/Heroes/LunaisAltSprite2", settings.LunaisGoddessSprite);

			gcm.SpFamiliarMeyef = LoadCustomSprite(gcm, "Sprites/Heroes/FamiliarMeyef", settings.MeyefSprite);
			gcm.SpFamiliarAltMeyef = LoadCustomSprite(gcm, "Sprites/Heroes/FamiliarAltMeyef", settings.MeyefWyrmSprite);

			gcm.SpFamiliarCrow = LoadCustomSprite(gcm, "Sprites/Heroes/FamiliarCrow", settings.MerchantCrowSprite);
			gcm.SpFamiliarAltCrow = LoadCustomSprite(gcm, "Sprites/Heroes/FamiliarAltCrow", settings.MerchantCrowGreedSprite);

			gcm.SpFamiliarKobo = LoadCustomSprite(gcm, "Sprites/Heroes/FamiliarKobo", settings.KoboSprite);
			gcm.SpFamiliarDemon = LoadCustomSprite(gcm, "Sprites/Heroes/FamiliarDemon", settings.DemonSprite);
			gcm.SpFamiliarGriffin = LoadCustomSprite(gcm, "Sprites/Heroes/FamiliarGriffin", settings.GriffinSprite);
			gcm.SpFamiliarSprite = LoadCustomSprite(gcm, "Sprites/Heroes/FamiliarSprite", settings.SpriteFamiliarSprite);

			if (level != null)
			{
				ReloadLunaisSprite(level, gameContentManager);
				ReloadFamiliarSprite(level, gameContentManager);
			}
		}

		static SpriteSheet LoadCustomSprite(dynamic gcm, string spriteKey, SpriteGameSetting spriteSetting)
		{
			TextureAtlasDatabase atlas = gcm._textureAtlasDatabase;

			atlas.TextureAtlasSpecifications[spriteKey].ContentPath = $"..//{spriteSetting.Value}";
			
			return gcm.Get(spriteKey);
		}

		static void ReloadLunaisSprite(Level level, GCM gameContentManager)
		{
			var lunaisSprite = gameContentManager.SpLunais;

			if (level.GameSave.HasRelicEnabled(EInventoryRelicType.EternalBrooch))
				lunaisSprite = gameContentManager.SpAltLunais2;

			else if (level.GameSave.HasRelicEnabled(EInventoryRelicType.EmpireBrooch))
				lunaisSprite = gameContentManager.SpAltLunais;
			
			level.MainHero.AsDynamic()._sprite = lunaisSprite;
		}

		static void ReloadFamiliarSprite(Level level, GCM gameContentManager)
		{
			var luniasObject = level.MainHero.AsDynamic();
			var familiar = (FamiliarBase)luniasObject.EquippedFamiliar;

			if (familiar == null) 
				return;

			var dynamicFamiliar = familiar.AsDynamic();

			SpriteSheet sprite;

			switch (dynamicFamiliar.FamiliarType)
			{
				case EInventoryFamiliarType.Meyef:
					sprite = level.GameSave.HasRelicEnabled(EInventoryRelicType.FamiliarAltMeyef)
						? gameContentManager.SpFamiliarAltMeyef
						: gameContentManager.SpFamiliarMeyef;
					break;
				case EInventoryFamiliarType.Griffin:
					sprite = gameContentManager.SpFamiliarGriffin;
					break;
				case EInventoryFamiliarType.MerchantCrow:
					sprite = level.GameSave.HasRelicEnabled(EInventoryRelicType.FamiliarAltCrow)
						? gameContentManager.SpFamiliarAltCrow
						: gameContentManager.SpFamiliarCrow;
					break;
				case EInventoryFamiliarType.Kobo:
					sprite = gameContentManager.SpFamiliarKobo;
					break;
				case EInventoryFamiliarType.Sprite:
					sprite = gameContentManager.SpFamiliarSprite;
					break;
				case EInventoryFamiliarType.Demon:
					sprite = gameContentManager.SpFamiliarDemon;
					break;
				default:
					return;
			}

			dynamicFamiliar._sprite = sprite;
		}

	}
}
