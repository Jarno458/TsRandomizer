using System;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Archipelago;

namespace TsRandomizer.Screens.SeedSelection
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.Options.PasswordMenuScreen")]
	class PasswordMenuScreen : Screen
	{
		static readonly Type PasswordMenuScreenType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.PauseMenu.Options.PasswordMenuScreen");

		readonly GameDifficultyMenuScreen difficultyMenu;

		public readonly PasswordMenuOverride menuOverride;

		public static GameScreen Create(ScreenManager screenManager)
		{
			void Noop() { }

			return (GameScreen)Activator.CreateInstance(PasswordMenuScreenType, null, screenManager.Reflected.GCM, (Action)Noop);
		}

		public PasswordMenuScreen(ScreenManager screenManager, GameScreen passwordMenuScreen) : base(screenManager, passwordMenuScreen)
		{
			difficultyMenu = screenManager.FirstOrDefault<GameDifficultyMenuScreen>();

			if (difficultyMenu == null)
				return;

			switch (difficultyMenu.PasswordMode)
			{
				case PasswordMode.SelectSeed:
					menuOverride = new SeedSelectionMenuScreen(screenManager, passwordMenuScreen);
					break;
				case PasswordMode.SelectArchipelagoServer:
					//menuOverride = new ServerAdressSelectionScreen(screenManager, passwordMenuScreen);
					menuOverride = new SeedSelectionMenuScreen(screenManager, passwordMenuScreen);
					break;
				case PasswordMode.SelectUserName:
					break;
				case PasswordMode.SelectPassword:
					break;
				default:
					return;
			}
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			menuOverride?.Initialize(itemLocationMap, gameContentManager);
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			menuOverride?.Update(gameTime, input);
		}
	}

	abstract class PasswordMenuOverride
	{
		protected readonly ScreenManager ScreenManager;
		public readonly GameScreen GameScreen;
		protected readonly dynamic Dynamic;

		protected PasswordMenuOverride(ScreenManager screenManager, GameScreen gameScreen)
		{
			ScreenManager = screenManager;
			GameScreen = gameScreen;
			Dynamic = gameScreen.AsDynamic();
		}

		public abstract void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager);
		public abstract void Update(GameTime gameTime, InputState input);
	}
}
