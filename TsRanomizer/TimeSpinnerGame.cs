using Timespinner;
using TsRandomizer.Extensions;
using TsRandomizer.Screens;

namespace TsRandomizer
{
	class TimeSpinnerGame : TimespinnerGame
	{
		readonly PlatformHelper platformHelper;
		public static dynamic Constants => new Constants();

		public TimeSpinnerGame(PlatformHelper platformHelper) : base(platformHelper)
		{
			this.platformHelper = platformHelper;
			HookScreenManager();
		}

		void HookScreenManager()
		{
			var screenManager = Components.FirstOfType<Timespinner.GameStateManagement.ScreenManager.ScreenManager>();
			var newScreenManager = new ScreenManager(this, platformHelper);

			newScreenManager.CopyScreensFrom(screenManager);

			Components.ReplaceComponent(screenManager, newScreenManager);
		}
	}
}
