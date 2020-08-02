using System;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.InGame.GameOverScreen")]
	// ReSharper disable once UnusedMember.Global
	class GameOverScreen : Screen
	{
		public GameOverScreen(ScreenManager screenManager, GameScreen gameScreen) : base(screenManager, gameScreen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			Action<GameSave> originalReloadSaveAction = Reflected._reloadSaveAction;

			void ReloadSave(GameSave gameSave)
			{
				itemLocationMap.BaseOnSave(gameSave);
				originalReloadSaveAction(gameSave);
			}

			Reflected._reloadSaveAction = (Action<GameSave>)ReloadSave;
		}
	}
}
