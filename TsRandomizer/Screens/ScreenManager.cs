using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Timespinner;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago;
using TsRandomizer.Commands;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
{
	class ScreenManager : Timespinner.GameStateManagement.ScreenManager.ScreenManager
	{
		static readonly Type GamePlayScreenType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.InGame.GameplayScreen");

		readonly LookupDictionary<GameScreen, Screen> hookedScreens
			= new LookupDictionary<GameScreen, Screen>(s => s.GameScreen);
		readonly List<GameScreen> foundScreens = new List<GameScreen>(20);

		ItemLocationMap itemLocationMap;

		public readonly dynamic Dynamic;

		public static Log Log;
		public static GameConsole Console;

		public static bool IsConsoleOpen;

		public ScreenManager(TimespinnerGame game, PlatformHelper platformHelper) : base(game, platformHelper)
		{
			Dynamic = this.AsDynamic();
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			Dynamic.GCM.LatinFont.DefaultCharacter = '?';

			Log = new Log(Dynamic.GCM);
			Console = new GameConsole(this, Dynamic.GCM);

			Console.AddCommand(new ConnectCommand(this));
		}

		public override void Update(GameTime gameTime)
		{
			var input = (InputState)Dynamic._input;

			DetectNewScreens();
			UpdateScreens(gameTime, input);

			Overlay.UpdateAll(gameTime, input);

			if (input.IsNewKeyPress(Keys.OemTilde))
				ToggleConsole();

			base.Update(gameTime);
		}

		public void ToggleConsole()
		{
			IsConsoleOpen = !IsConsoleOpen;

			if (IsConsoleOpen)
				AddScreen(Console, null);
			else
				RemoveScreen(Console);
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

				if(!Screen.RegisteredTypes.TryGetValue(screen.GetType(), out var handlerType))
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

		public T FirstOrDefault<T>() where T : Screen => (T)hookedScreens.FirstOrDefault(s => s.GetType() == typeof(T));

		public GameScreen FirstOrDefaultTimespinnerOfType(Type type) => ((List<GameScreen>)Dynamic._screens).FirstOrDefault(s => s.GetType() == type);
	}
}
