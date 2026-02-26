using HarmonyLib;
using System.Reflection;

namespace TsRandomizer
{
	[HarmonyPatch]
	class DialogueAutoSkipPatch
	{
		static MethodBase TargetMethod()
		{
			return AccessTools.Method(
				AccessTools.TypeByName("Timespinner.GameAbstractions.HUD.DialogueBox"),
				"Update");
		}

		static void Prefix(object __instance)
		{
			if (!QoLSettings.Current.AutoSkipDialogue) return;

			var type = AccessTools.TypeByName("Timespinner.GameAbstractions.HUD.DialogueBox");
			AccessTools.Method(type, "FinishDialogue").Invoke(__instance, null);
		}
	}
}