using HarmonyLib;
using System.Reflection;

namespace TsRandomizer
{
	[HarmonyPatch]
	class AutoSkipCutscenePatch
	{
		static MethodBase TargetMethod()
		{
			var type = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.InGame.GameplayScreen");
			return AccessTools.Method(type, "UpdateLevelRequests");
		}

		static void Postfix(object __instance)
		{
			if (!QoLSettings.Current.AutoSkipCutscenes) return;

			var type = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.InGame.GameplayScreen");
			var level = AccessTools.Field(type, "_level").GetValue(__instance);
			if (level == null) return;

			var levelType = AccessTools.TypeByName("Timespinner.GameAbstractions.Gameplay.Level");
			bool isUnskippable = (bool)AccessTools.Property(levelType, "IsActiveScriptUnskippable").GetValue(level);
			bool isDoingDeathCutscene = (bool)AccessTools.Property(levelType, "IsDoingPlayerDeathCutscene").GetValue(level);
			bool isInputBlocked = (bool)AccessTools.Property(levelType, "IsPlayerInputBlocked").GetValue(level);

			if (isInputBlocked && !isUnskippable && !isDoingDeathCutscene)
			{
				AccessTools.Method(levelType, "SkipCutscene").Invoke(level, null);
				AccessTools.Field(type, "_isShowingSkipCutscenePrompt").SetValue(__instance, false);
			}
		}
	}

	[HarmonyPatch]
	class CutsceneUnskippableWaitPatch
	{
		static MethodBase TargetMethod()
		{
			var type = AccessTools.TypeByName("Timespinner.GameObjects.BaseClasses.GameEvent");
			return AccessTools.Method(type, "AddUnskippableWaitScript");
		}

		static void Prefix(ref float waitTime)
		{
			if (!QoLSettings.Current.AutoSkipCutscenes) return;
			waitTime *= 0.25f;
		}
	}
}
