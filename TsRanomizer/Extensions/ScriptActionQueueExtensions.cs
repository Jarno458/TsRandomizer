using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Extensions
{
	public static class ScriptActionQueueExtensions
	{
		internal static void UpdateRelicOrbGetToastToItem(this Queue<ScriptAction> scripts, ItemInfo itemInfo)
		{
			var giveOrbScript = scripts.Single(s => s.AsDynamic().ScriptType == EScriptType.RelicOrbGetToast);
			var reflectedScript = giveOrbScript.AsDynamic();

			if (itemInfo.LootType == LootType.ConstStat || itemInfo.LootType == LootType.ConstUseItem)
				reflectedScript.ScriptType = EScriptType.GiveItem;

			reflectedScript.ItemToGiveType = itemInfo.LootType.ToEInventoryCategoryType();
			reflectedScript.ItemToGive = itemInfo.ItemId;
			reflectedScript.OrbSlot = itemInfo.OrbSlot;
		}
	}
}
