using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.PauseMenuScreen")]
	class PauseMenuScreen : Screen
	{
		static readonly Type DialogueLineType = TimeSpinnerType.Get("Timespinner.Core.DialogueLine");

		bool isArchipelagoSeed;

		public bool IsOpeningGiftingSendMenu;
		public bool IsOpeningGiftingReceiveMenu;

		dynamic giftingInfoLine;

		public PauseMenuScreen(ScreenManager screenManager, GameScreen gameScreen) : base(screenManager, gameScreen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			GameSave save = Dynamic._saveFile;
			if (save == null)
				return;

			var seed = save.GetSeed();
			isArchipelagoSeed = seed.HasValue && seed.Value.Options.Archipelago;

			if (!isArchipelagoSeed)
				return;

			giftingInfoLine = DialogueLineType.CreateInstance(true,
				"Press $X to send gifts, Press $Y to receive gifts",
				gameContentManager.ActiveFont,
				gameContentManager.SpUIButtons,
				(int)Dynamic.Zoom,
				ScreenManager.MenuControllerMapping).AsDynamic();
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			if (!isArchipelagoSeed || !GameScreen.IsActive)
				return;

			if (input.IsNewPressSecondary(null))
			{
				IsOpeningGiftingReceiveMenu = true;

				var giftingMenuScreen = EquipmentMenuScreen.Create(ScreenManager, Dynamic._saveFile);

				ScreenManager.AddScreen(giftingMenuScreen, null);
			} else if (input.IsNewPressTertiary(null))
			{
				IsOpeningGiftingSendMenu = true;

				var giftingMenuScreen = EquipmentMenuScreen.Create(ScreenManager, Dynamic._saveFile);

				ScreenManager.AddScreen(giftingMenuScreen, null);
			}
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			if (!isArchipelagoSeed || !GameScreen.IsActive)
				return;

			var descriptionPosition = Dynamic.DescriptionDrawPosition;
			var position = new Vector2(descriptionPosition.X, descriptionPosition.Y - (18 * (float)Dynamic.Zoom));
			var titleBaseColor = new Color(240, 240, 208);
			var titleShadowColor = new Color(60, 60, 24);

			using (spriteBatch.BeginUsing())
				giftingInfoLine.Draw(spriteBatch, position, titleBaseColor, titleShadowColor, Math.Max((int)(Dynamic.Zoom * 0.8f), 1), 1f);
		}
	}
}
