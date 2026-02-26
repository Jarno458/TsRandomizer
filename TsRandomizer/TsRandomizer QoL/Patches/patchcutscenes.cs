using System.Collections;
using HarmonyLib;
using System.Reflection;
using Microsoft.Xna.Framework;

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

			if (isInputBlocked && !isUnskippable && !isDoingDeathCutscene && IsRealCutsceneActive(level, levelType))
			{
				AccessTools.Method(levelType, "SkipCutscene").Invoke(level, null);
				AccessTools.Field(type, "_isShowingSkipCutscenePrompt").SetValue(__instance, false);
			}
		}

		static bool IsRealCutsceneActive(object level, System.Type levelType)
		{
			try
			{
				var activeScripts = (IList)AccessTools.Field(levelType, "_activeScripts").GetValue(level);
				var scriptActionType = AccessTools.TypeByName("Timespinner.GameAbstractions.Gameplay.ScriptAction");
				var scriptTypeProp = AccessTools.Property(scriptActionType, "ScriptType");
				var argumentsProp = AccessTools.Property(scriptActionType, "Arguments");
				var cutsceneStartValue = System.Enum.Parse(
					AccessTools.TypeByName("Timespinner.GameAbstractions.Gameplay.EScriptType"), "CutsceneStart");

				foreach (var script in activeScripts)
				{
					if (script == null) continue;
					var scriptType = scriptTypeProp.GetValue(script, null);
					if (!scriptType.Equals(cutsceneStartValue)) continue;
					var arguments = (Vector4)argumentsProp.GetValue(script, null);
					if (arguments.X >= 1f) return true;
				}

				return false;
			}
			catch
			{
				return false;
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