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
using TsRandomizer.Screens.Console;

namespace TsRandomizer.Screens
{
	class ScreenManager : Timespinner.GameStateManagement.ScreenManager.ScreenManager
	{
		static readonly Type GamePlayScreenType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.InGame.GameplayScreen");
		static readonly Type BaseToastPopupType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.InGame.BaseToastPopup");

		static readonly System.Reflection.FieldInfo FreezeField =
			BaseToastPopupType.GetField("_doesFreezeGameplay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		static readonly System.Reflection.FieldInfo TimeToWaitField =
			BaseToastPopupType.GetField("_timeToWait", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		static readonly System.Reflection.FieldInfo TotalDisplayField =
			BaseToastPopupType.GetField("_totalDisplayTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		static readonly System.Reflection.FieldInfo TimeBeforeFlashingField =
			BaseToastPopupType.GetField("_timeBeforeFlashing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		static readonly System.Reflection.FieldInfo TimeToFlashField =
			BaseToastPopupType.GetField("_timeToFlash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		static readonly System.Reflection.FieldInfo TimeToFadeField =
			BaseToastPopupType.GetField("_timeToFade", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		static readonly System.Reflection.PropertyInfo IsOverlayProp =
			BaseToastPopupType.BaseType?.GetProperty("IsOverlayScreen", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
		static readonly System.Reflection.PropertyInfo WaitForInputProp =
			BaseToastPopupType.GetProperty("DoesWaitForInputToFinish", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		static readonly System.Reflection.PropertyInfo HasReceivedInputProp =
			BaseToastPopupType.GetProperty("HasReceivedInputToClose", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

		static readonly string LevelUpToastTypeName =
			"Timespinner.GameStateManagement.Screens.InGame.Toasts.CharacterLevelUpToast";
		static readonly System.Reflection.FieldInfo ControlTimerField =
			TimeSpinnerType.Get(LevelUpToastTypeName)
				.GetField("_controlTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

		// Only patch toasts that relate to item/level pickups.
		// AreaTitleToast (room name on screen transition) must be left alone.
		static readonly HashSet<string> PatchableToastTypeNames = new HashSet<string>
		{
			"Timespinner.GameStateManagement.Screens.InGame.Toasts.RelicOrbGetToast",
			"Timespinner.GameStateManagement.Screens.InGame.Toasts.CharacterLevelUpToast",
			"Timespinner.GameStateManagement.Screens.InGame.Toasts.OrbLevelUpToast",
			"Timespinner.GameStateManagement.Screens.InGame.Toasts.StatMaxUpToast",
			"Timespinner.GameStateManagement.Screens.InGame.Toasts.QuestCompleteToast",
		};

		readonly LookupDictionary<GameScreen, Screen> hookedScreens
			= new LookupDictionary<GameScreen, Screen>(s => s.GameScreen);
		readonly List<GameScreen> foundScreens = new List<GameScreen>(20);
		readonly HashSet<GameScreen> toastScreensPatched = new HashSet<GameScreen>();

		ItemLocationMap itemLocationMap;

		public readonly dynamic Dynamic;
		public GCM GameContentManager => Dynamic.GCM;

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
			GameContentManager.LatinFont.DefaultCharacter = '?';
			Log = new Log();
			Console = new GameConsole(this, GameContentManager);
			Console.AddCommand(new ConnectCommand(this));
		}

		public override void Update(GameTime gameTime)
		{
			var input = (InputState)Dynamic._input;
			DetectNewScreens();
			UpdateScreens(gameTime, input);
			Overlay.UpdateAll(gameTime, input, Jukebox);
			if (input.IsNewKeyPress(Keys.OemTilde))
				ToggleConsole();
			base.Update(gameTime);
			// Apply AFTER base.Update so we overwrite whatever the toast's own
			// Update just set — otherwise we lose the race every frame.
			ForceToastSettings();
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
			Overlay.DrawAll(SpriteBatch, new Rectangle(0, 0, ScreenSize.X, ScreenSize.Y), GameContentManager);
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
				if (!Screen.RegisteredTypes.TryGetValue(screen.GetType(), out var handlerType))
					continue;
				var screenHandler = (Screen)Activator.CreateInstance(handlerType, this, screen);
				hookedScreens.Add(screenHandler);
				foundScreens.Add(screen);
				screenHandler.Initialize(itemLocationMap, GameContentManager);
			}
			if (foundScreens.Count != hookedScreens.Count)
				hookedScreens.Filter(foundScreens, s => s.Unload());
		}

		void ForceToastSettings()
		{
			foreach (var screen in GetScreens())
				if (BaseToastPopupType.IsInstanceOfType(screen)
					&& PatchableToastTypeNames.Contains(screen.GetType().FullName))
					ApplyToastSettings(screen);
		}

		static void ApplyToastSettings(GameScreen screen)
		{
			if (QoLSettings.Current.FastToastPopups)
			{
				TimeToWaitField.SetValue(screen, 0f);
				WaitForInputProp?.SetValue(screen, false);
				HasReceivedInputProp?.SetValue(screen, true);

				float before = (float)TimeBeforeFlashingField.GetValue(screen);
				float flash = (float)TimeToFlashField.GetValue(screen);
				float fade = (float)TimeToFadeField.GetValue(screen);
				TotalDisplayField.SetValue(screen, before + flash + fade);
			}

			if (!QoLSettings.Current.ToastsBlockMovement)
			{
				FreezeField.SetValue(screen, false);
				IsOverlayProp?.SetValue(screen, true);

				if (screen.GetType().FullName == LevelUpToastTypeName)
					ControlTimerField?.SetValue(screen, 0.75f);
			}
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