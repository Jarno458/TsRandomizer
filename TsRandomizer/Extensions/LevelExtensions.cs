using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Extensions
{
	public static class LevelExtensions
	{
		static readonly Type ToasterType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.InGame.EToastType");

		internal static void RequestToastPopupForStats(this Level level, ItemInfo itemInfo)
		{
			switch (itemInfo.Identifier.Stat)
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
