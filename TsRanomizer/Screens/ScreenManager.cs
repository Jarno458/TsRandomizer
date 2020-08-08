using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
{
	class ScreenManager : Timespinner.GameStateManagement.ScreenManager.ScreenManager
	{
		static readonly Type GamePlayScreenType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.InGame.GameplayScreen");

		readonly LookupDictionairy<GameScreen, Screen> hookedScreens
			= new LookupDictionairy<GameScreen, Screen>(s => s.GameScreen);
		readonly List<GameScreen> foundScreens = new List<GameScreen>(20);
		
		ItemLocationMap itemLocationMap;

		public readonly dynamic Reflected;

		public ScreenManager(TimeSpinnerGame game, PlatformHelper platformHelper) : base(game, platformHelper)
		{
			Reflected = this.AsDynamic();
		}

		public override void Update(GameTime gameTime)
		{
			DetectNewScreens();
			UpdateScreens(gameTime);

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

					if (screen.GetType() == GamePlayScreenType)
						itemLocationMap = ((GameplayScreen)hookedScreens[screen]).ItemLocations;

					continue;
				}

				if(!Screen.RegisteredTypes.TryGetValue(screen.GetType(), out Type handlerType))
					continue;

				var screenHandler = (Screen)Activator.CreateInstance(handlerType, this, screen);
				hookedScreens.Add(screenHandler);
				foundScreens.Add(screen);

				screenHandler.Initialize(itemLocationMap, (GCM)Reflected.GCM);
			}

			if (foundScreens.Count != hookedScreens.Count)
				hookedScreens.Filter(foundScreens, s => s.Unload());
		}

		void UpdateScreens(GameTime gameTime)
		{
			var input = (InputState)Reflected._input;

			foreach (var screen in hookedScreens)
				screen.Update(gameTime, input);
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
