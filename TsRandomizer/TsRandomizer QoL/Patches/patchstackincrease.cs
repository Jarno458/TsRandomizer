using HarmonyLib;
using Timespinner.GameAbstractions.Inventory;
using System.Reflection;
using System.Collections.Generic;

namespace TsRandomizer
{
	// Increase stack limit in inventory
	[HarmonyPatch(typeof(InventoryUseItem))]
	[HarmonyPatch(MethodType.Constructor)]
	[HarmonyPatch(new[] { typeof(EInventoryUseItemType) })]
	class StackCapPatch
	{
		static void Postfix(InventoryUseItem __instance, EInventoryUseItemType useItemType)
		{
			int cap = QoLSettings.Current.StackCap;

			switch (useItemType)
			{
				case EInventoryUseItemType.MagicMarbles:
				case EInventoryUseItemType.EssenceCrystal:
				case EInventoryUseItemType.GoldRing:
				case EInventoryUseItemType.GoldNecklace:
					break; // leave vanilla
				default:
					__instance.StackCap = cap;
					break;
			}
		}
	}

	// Increase buying limit to match stack cap
	[HarmonyPatch]
	class ShopCapPatch
	{
		static MethodBase TargetMethod()
		{
			var type = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.Shop.ShopMenuScreen");
			return AccessTools.Method(type, "GetAvailableQuantityByItem");
		}

		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);

			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].LoadsConstant(9))
					codes[i].operand = QoLSettings.Current.StackCap;
			}

			return codes;
		}
	}
}
