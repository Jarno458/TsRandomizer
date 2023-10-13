using System;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens.Gifting
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.EquipmentMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class EquipmentMenuScreen : Screen
	{
		static readonly Type EquipmentMenuScreenType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.PauseMenu.EquipmentMenuScreen");

		readonly Screen subScreen;

		public EquipmentMenuScreen(ScreenManager screenManager, GameScreen gameScreen) : base(screenManager, gameScreen)
		{
			var pauseMenuScreen = screenManager.FirstOrDefault<PauseMenuScreen>();

			switch (pauseMenuScreen.GiftingMenuType)
			{
				case GiftingMenuType.None:
					return;
				case GiftingMenuType.Send:
					subScreen = new GiftingSendScreen(screenManager, gameScreen);
					break;
				case GiftingMenuType.Receive:
					subScreen = new GiftingReceiveScreen(screenManager, gameScreen);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static GameScreen Create(ScreenManager screenManager, GameSave save)
		{
			GCM gcm = screenManager.AsDynamic().GCM;

			void ResetPauseMenuOpenOverride()
			{
				var pauseMenuScreen = screenManager.FirstOrDefault<PauseMenuScreen>();
				pauseMenuScreen.GiftingMenuType = GiftingMenuType.None;
			}

			return (GameScreen)Activator.CreateInstance(EquipmentMenuScreenType, save, gcm, (Action)ResetPauseMenuOpenOverride, (Action)ResetPauseMenuOpenOverride);
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gcm) => subScreen?.Initialize(itemLocationMap, gcm);

		public override void Update(GameTime gameTime, InputState input) => subScreen?.Update(gameTime, input);
	}
}
