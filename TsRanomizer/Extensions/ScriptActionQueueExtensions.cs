using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Extensions
{
	public static class ScriptActionQueueExtensions
	{
		public static readonly Vector4 ReplacedArguments = new Vector4(1, 3, 3, 7);

		static readonly Type ToasterType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.InGame.EToastType");

		internal static void UpdateRelicOrbGetToastToItem(this Queue<ScriptAction> scripts, Level level, ItemInfo itemInfo)
		{
			var giveOrbScript = scripts.Single(s => s.AsDynamic().ScriptType == EScriptType.RelicOrbGetToast);
			var reflectedScript = giveOrbScript.AsDynamic();

			switch (itemInfo.LootType)
			{
				case LootType.ConstUseItem:
					reflectedScript.ScriptType = EScriptType.GiveItem;
					break;

				case LootType.ConstStat:
					reflectedScript.ScriptType = EScriptType.Delegate;
					reflectedScript.Delegate = (Action)(() => RewardStat(level, itemInfo));
					break;
			}

			reflectedScript.ItemToGiveType = itemInfo.LootType.ToEInventoryCategoryType();
			reflectedScript.ItemToGive = itemInfo.ItemId;
			reflectedScript.OrbSlot = itemInfo.OrbSlot;
			reflectedScript.Arguments = ReplacedArguments;
		}

		internal static void RewardStat(Level level, ItemInfo itemInfo)
		{
			switch (itemInfo.Stat)
			{
				case EItemType.MaxHP:
					level.AsDynamic().RequestToastPopup(ToasterType.GetEnumValue("Health"), 0);
					break;
				case EItemType.MaxAura:
					level.AsDynamic().RequestToastPopup(ToasterType.GetEnumValue("Aura"), 0);
					break;
				case EItemType.MaxSand:
					level.AsDynamic().RequestToastPopup(ToasterType.GetEnumValue("Sand"), 0);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

		}
	}
}
