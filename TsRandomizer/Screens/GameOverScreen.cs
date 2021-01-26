using System;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.ItemTracker;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
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
			Action<GameSave> originalReloadSaveAction = Dynamic._reloadSaveAction;

			void ReloadSave(GameSave gameSave)
			{
				itemLocationMap.BaseOnSave(gameSave);

				ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(itemLocationMap));

				originalReloadSaveAction(gameSave);
			}

			Dynamic._reloadSaveAction = (Action<GameSave>)ReloadSave;
		}
	}
}
