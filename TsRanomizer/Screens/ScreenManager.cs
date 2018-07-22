using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;

namespace TsRanodmizer.Screens
{
	class ScreenManager : Timespinner.GameStateManagement.ScreenManager.ScreenManager
	{
		readonly LookupDictionairy<GameScreen, Screen> hookedScreens
			= new LookupDictionairy<GameScreen, Screen>(s => s.GameScreen);
		readonly List<GameScreen> foundScreens = new List<GameScreen>(20);

		public readonly dynamic Reflected;

		public ScreenManager(TimeSpinnerGame game) : base(game)
		{
			Reflected = this.Reflect();
		}

		public override void Update(GameTime gameTime)
		{
			DetectNewScreens();
			UpdateScreens();

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			DrawGameplayScreens();
		}

		void DetectNewScreens()
		{
			foundScreens.Clear();

			foreach (var screen in GetScreens())
			{
				if (hookedScreens.Contains(screen))
				{
					foundScreens.Add(screen);
					continue;
				}

				if(!Screen.RegisteredTypes.TryGetValue(screen.GetType(), out Type handlerType))
					continue;

				var screenHandler = (Screen)Activator.CreateInstance(handlerType, this, screen);
				hookedScreens.Add(screenHandler);
				foundScreens.Add(screen);
			}

			if(foundScreens.Count != hookedScreens.Count)
				hookedScreens.Filter(foundScreens);
		}

		void UpdateScreens()
		{
			var input = (InputState)Reflected._input;

			foreach (var screen in hookedScreens)
				screen.Update(input);
		}

		void DrawGameplayScreens()
		{
			foreach (var screen in hookedScreens)
				screen.Draw(SpriteBatch, MenuFont);
		}

		public void CopyScreensFrom(Timespinner.GameStateManagement.ScreenManager.ScreenManager screenManager)
		{
			foreach (var screen in screenManager.GetScreens())
				AddScreen(screen, null);
		}
	}
}
