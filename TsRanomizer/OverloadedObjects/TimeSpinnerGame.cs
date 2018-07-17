using Timespinner;
using TsRanodmizer.Extensions;

namespace TsRanodmizer.OverloadedObjects
{
    class TimeSpinnerGame : TimespinnerGame
    {
	    public static dynamic Constants => new Constants();

	    public TimeSpinnerGame()
        {
            HookScreenManager();
        }

        void HookScreenManager()
        {
            var screenManager = Components.FirstOfType<Timespinner.GameStateManagement.ScreenManager.ScreenManager>();
            var newScreenManager = new ScreenManager(this);

            newScreenManager.CopyScreensFrom(screenManager);

            Components.ReplaceComponent(screenManager, newScreenManager);
        }
    }
}
