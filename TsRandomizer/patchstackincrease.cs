using System;
using HarmonyLib;
using Timespinner.GameAbstractions.Inventory;
using System.Reflection;
using System.Collections.Generic;
namespace TsRandomizer
{
	[HarmonyPatch(typeof(InventoryUseItem))]
	[HarmonyPatch(MethodType.Constructor)]
	[HarmonyPatch(new[] { typeof(EInventoryUseItemType) })]


	//increase stack limit in inventory
	class StackCapPatch
	{
		static void Postfix(InventoryUseItem __instance, EInventoryUseItemType useItemType)
		{
			switch (useItemType)
			{
				case EInventoryUseItemType.MagicMarbles:
				case EInventoryUseItemType.EssenceCrystal:
				case EInventoryUseItemType.GoldRing:
				case EInventoryUseItemType.GoldNecklace:
					break; // leave at 99
				default:
					__instance.StackCap = 99;
					break;
			}
		}
	}

	//Inscrease buying limit to match inventory size
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
			int replaced = 0;

			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].LoadsConstant(9))
				{
					codes[i].operand = 99;
					replaced++;
				}
			}

			return codes;
		}
	}
}