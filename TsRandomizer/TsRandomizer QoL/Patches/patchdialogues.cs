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
			AccessTools.Field(type, "_isAutoplay").SetValue(__instance, true);
			AccessTools.Field(type, "_autoplayNextLineTimer").SetValue(__instance, 999f);
			AccessTools.Field(type, "_isReadyForNextLine").SetValue(__instance, true);
		}
	}
}
