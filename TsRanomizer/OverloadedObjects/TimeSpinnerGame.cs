using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Timespinner;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using TsRanodmizer.Screens;

namespace TsRanodmizer.OverloadedObjects
{
    class TimeSpinnerGame : TimespinnerGame
    {
	    public static dynamic Constants => new Constants();

	    public static Assembly TimeSpinnerAssembly = typeof(TimespinnerGame).Assembly;

	    public TimeSpinnerGame()
        {
            var screenManager = HookScreenManager();

            InjectMenu(screenManager);
        }

        static void InjectMenu(ScreenManager2 screenManager)
        {
            var spashScreen = screenManager.GetScreens().First();
            screenManager.RemoveScreen(spashScreen);
            screenManager.AddScreen(new Menu(spashScreen), new PlayerIndex?());
        }

        ScreenManager2 HookScreenManager()
        {
            var screenManager = Components.FirstOfType<ScreenManager>();
            var newScreenManager = new ScreenManager2(this);

            newScreenManager.CopyScreensFrom(screenManager);

            Components.ReplaceComponent(screenManager, newScreenManager);

            return newScreenManager;
        }
    }
}
