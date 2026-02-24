using Timespinner.GameAbstractions;
using Timespinner.Core;
using HarmonyLib;
using System.Reflection;

namespace TsRandomizer
{
	[HarmonyPatch]
	class ToastTimerPatch
	{
		static MethodBase TargetMethod()
		{
			var type = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.InGame.BaseToastPopup");
			return AccessTools.Constructor(type, new[] {
				typeof(SpriteSheet),
				typeof(bool),
				typeof(GCM),
				typeof(float),
				typeof(float),
				typeof(float),
				typeof(float)
			});
		}

		static void Postfix(object __instance)
		{
			var baseType = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.InGame.BaseToastPopup");

			// zero out wait timer for all toasts
			AccessTools.Field(baseType, "_timeToWait").SetValue(__instance, 0f);
			AccessTools.Field(baseType, "_totalDisplayTime").SetValue(__instance,
				(float)AccessTools.Field(baseType, "_timeBeforeFlashing").GetValue(__instance) +
				(float)AccessTools.Field(baseType, "_timeToFlash").GetValue(__instance) +
				(float)AccessTools.Field(baseType, "_timeToFade").GetValue(__instance));
		}
	}

	[HarmonyPatch]
	class DoesWaitForInputPatch
	{
		static MethodBase TargetMethod()
		{
			var type = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.InGame.BaseToastPopup");
			return AccessTools.Method(type, "set_DoesWaitForInputToFinish");
		}

		static bool Prefix()
		{
			return false; // fuckoff mr popup
		}
	}
	//desperate struggle for movement
	[HarmonyPatch]
	class RelicOrbToastPatch
	{
		static MethodBase TargetMethod()
		{
			var type = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.InGame.Toasts.RelicOrbGetToast");
			return AccessTools.Constructor(type, new[] {
		typeof(SpriteSheet),
		typeof(GCM),
		AccessTools.TypeByName("Timespinner.GameAbstractions.Gameplay.ScriptAction"),
		AccessTools.TypeByName("Timespinner.GameAbstractions.Gameplay.ControllerMapping")
	});
		}

		static void Postfix(object __instance)
		{
			var baseType = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.InGame.BaseToastPopup");
			AccessTools.Field(baseType, "_doesFreezeGameplay").SetValue(__instance, false);
			AccessTools.Field(baseType, "_timeToWait").SetValue(__instance, 0f);
			AccessTools.Property(baseType.BaseType, "IsOverlayScreen").SetValue(__instance, true);
		}
	}
	//just let me play already
	[HarmonyPatch]
	class CharacterLevelUpToastPatch
	{
		static MethodBase TargetMethod()
		{
			var type = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.InGame.Toasts.CharacterLevelUpToast");
			return AccessTools.Constructor(type, new[] {
			typeof(SpriteSheet),
			typeof(GCM)
		});
		}

		static void Postfix(object __instance)
		{
			var type = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.InGame.Toasts.CharacterLevelUpToast");
			AccessTools.Field(type, "_controlTimer").SetValue(__instance, 0.75f);
		}
	}
}