using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Extensions
{
	public static class LevelExtensions
	{
		static readonly Type ToasterType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.InGame.EToastType");

		public static void ShowItemAwardPopup(this Level level, ItemIdentifier itemIdentifier)
		{
			switch (itemIdentifier.LootType)
			{
				case LootType.ConstOrb:
					level.AddScript((ScriptAction)typeof(ScriptAction).CreateInstance(true, itemIdentifier.OrbType, itemIdentifier.OrbSlot));
					break;
				case LootType.ConstFamiliar:
					level.AddScript((ScriptAction)typeof(ScriptAction).CreateInstance(true, itemIdentifier.Familiar));
					break;
				case LootType.ConstRelic:
					level.AddScript((ScriptAction)typeof(ScriptAction).CreateInstance(true, itemIdentifier.Relic));
					break;
				case LootType.ConstEquipment:
					level.AddScript((ScriptAction)typeof(ScriptAction).CreateInstance(true, itemIdentifier.Equipment));
					break;
				case LootType.ConstUseItem:
					level.AddScript((ScriptAction)typeof(ScriptAction).CreateInstance(true, itemIdentifier.UseItem, 1));
					break;
				case LootType.ConstStat:
					RequestToastPopupForStats(level, itemIdentifier);
					break;
				default:
					throw new NotImplementedException($"RelicOrOrbGetPopup is not implemented for LootType {itemIdentifier.LootType}");
			}
		}

		internal static void RequestToastPopupForStats(this Level level, ItemIdentifier itemIdentifier)
		{
			switch (itemIdentifier.Stat)
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
