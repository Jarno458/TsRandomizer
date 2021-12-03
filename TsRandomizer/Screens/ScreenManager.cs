using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Timespinner;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Commands;

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

		public readonly dynamic Dynamic;

		public static Log Log;
		public static GameConsole Console;

		bool isConsoleOpen;

		public ScreenManager(TimespinnerGame game, PlatformHelper platformHelper) : base(game, platformHelper)
		{
			Dynamic = this.AsDynamic();
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			Dynamic.GCM.LatinFont.DefaultCharacter = '?';

			Log = new Log(Dynamic.GCM);
			Console = new GameConsole(Dynamic.GCM);

			Console.AddCommand(new ConnectCommand(this));
		}

		public override void Update(GameTime gameTime)
		{
			var input = (InputState)Dynamic._input;

			DetectNewScreens();
			UpdateScreens(gameTime, input);

			Overlay.UpdateAll(gameTime, input);

			ToggleConsole(input);

			base.Update(gameTime);
		}

		void ToggleConsole(InputState input)
		{
			if (input.IsNewKeyPress(Keys.OemTilde))
			{
				isConsoleOpen = !isConsoleOpen;

				if (isConsoleOpen)
					AddScreen(Console, null);
				else
					RemoveScreen(Console);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			DrawGameplayScreens();

			Overlay.DrawAll(SpriteBatch, new Rectangle(0, 0, ScreenSize.X, ScreenSize.Y));
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

				screenHandler.Initialize(itemLocationMap, (GCM)Dynamic.GCM);
			}

			if (foundScreens.Count != hookedScreens.Count)
				hookedScreens.Filter(foundScreens, s => s.Unload());
		}

		void UpdateScreens(GameTime gameTime, InputState input)
		{
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

		public T FirstOrDefault<T>() where T : Screen
		{
			return (T)hookedScreens.FirstOrDefault(s => s.GetType() == typeof(T));
		}
	}
}
