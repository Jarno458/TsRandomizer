using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.InGame.BaseToastPopup")]
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.InGame.Toasts.RelicOrbGetToast")]
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.InGame.Toasts.CharacterLevelUpToast")]
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.InGame.Toasts.OrbLevelUpToast")]
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.InGame.Toasts.StatMaxUpToast")]
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.InGame.Toasts.QuestCompleteToast")]
	// ReSharper disable once UnusedMember.Global
	class ToastPopupScreen : Screen
	{
		static readonly string BaseToastTypeName =
			"Timespinner.GameStateManagement.Screens.InGame.BaseToastPopup";
		static readonly string LevelUpToastTypeName =
			"Timespinner.GameStateManagement.Screens.InGame.Toasts.CharacterLevelUpToast";

		// Cached via reflection once, reused across all toast instances
		static System.Reflection.FieldInfo _freezeField;
		static System.Reflection.FieldInfo _timeToWaitField;
		static System.Reflection.FieldInfo _totalDisplayField;
		static System.Reflection.FieldInfo _timeBeforeFlashingField;
		static System.Reflection.FieldInfo _timeToFlashField;
		static System.Reflection.FieldInfo _timeToFadeField;
		static System.Reflection.PropertyInfo _isOverlayProp;
		static System.Reflection.PropertyInfo _waitForInputProp;
		static System.Reflection.PropertyInfo _hasReceivedInputProp;
		static System.Reflection.FieldInfo _controlTimerField;

		bool _isLevelUpToast;

		public ToastPopupScreen(ScreenManager screenManager, GameScreen gameScreen)
			: base(screenManager, gameScreen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			_isLevelUpToast = GameScreen.GetType().FullName == LevelUpToastTypeName;

			var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
			var publicFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

			if (_freezeField == null)
			{
				var baseType = TimeSpinnerType.Get(BaseToastTypeName);
				var levelUpType = TimeSpinnerType.Get(LevelUpToastTypeName);

				_freezeField = baseType.GetField("_doesFreezeGameplay", flags);
				_timeToWaitField = baseType.GetField("_timeToWait", flags);
				_totalDisplayField = baseType.GetField("_totalDisplayTime", flags);
				_timeBeforeFlashingField = baseType.GetField("_timeBeforeFlashing", flags);
				_timeToFlashField = baseType.GetField("_timeToFlash", flags);
				_timeToFadeField = baseType.GetField("_timeToFade", flags);
				_isOverlayProp = GameScreen.GetType().BaseType?.GetProperty("IsOverlayScreen", publicFlags);
				_waitForInputProp = baseType.GetProperty("DoesWaitForInputToFinish", publicFlags);
				_hasReceivedInputProp = baseType.GetProperty("HasReceivedInputToClose", publicFlags);
				_controlTimerField = levelUpType.GetField("_controlTimer", flags);
			}

			// FastToastPopups: collapse the wait phase to zero so the toast flies through
			if (QoLSettings.Current.FastToastPopups)
			{
				_timeToWaitField.SetValue(GameScreen, 0f);

				float before = (float)_timeBeforeFlashingField.GetValue(GameScreen);
				float flash = (float)_timeToFlashField.GetValue(GameScreen);
				float fade = (float)_timeToFadeField.GetValue(GameScreen);
				_totalDisplayField.SetValue(GameScreen, before + flash + fade);
			}

			// ToastsBlockMovement: control whether the toast freezes gameplay
			if (!QoLSettings.Current.ToastsBlockMovement)
			{
				_freezeField.SetValue(GameScreen, false);
				_isOverlayProp?.SetValue(GameScreen, true);

				// CharacterLevelUpToast has its own _controlTimer that stalls input
				if (_isLevelUpToast)
					_controlTimerField?.SetValue(GameScreen, 0.75f);
			}
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			// FastToastPopups suppresses the button-press requirement.
			// The original Harmony patch blocked set_DoesWaitForInputToFinish globally
			// on all toasts. We replicate that by overriding every frame, since
			// RelicOrbGetToast sets it back to true during its own update/script handling.
			if (QoLSettings.Current.FastToastPopups)
			{
				_waitForInputProp?.SetValue(GameScreen, false);
				_hasReceivedInputProp?.SetValue(GameScreen, true);
			}
		}
	}
}