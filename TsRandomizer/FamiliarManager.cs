using System;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.Heroes.Familiars;
using TsRandomizer.Extensions;

namespace TsRandomizer
{
	static class FamiliarManager
	{
		public static void Update(Level level)
		{
			OverwriteSwitchFamiliarFunction(level);
		}

		static void OverwriteSwitchFamiliarFunction(Level level)
		{
			var luniasObject = level.MainHero.AsDynamic();
			var familiar = (FamiliarBase)luniasObject.EquippedFamiliar;

			if (familiar == null) return;

			familiar.AsDynamic()._switchFamiliarAction =
				(Action<bool>)(isGoingRight => SwitchFamiliar(level, luniasObject, isGoingRight));
		}

		static void SwitchFamiliar(Level level, dynamic luniasObject, bool isGoingRight)
		{
			var familiarTypes = level.GameSave.Inventory.FamiliarInventory.Inventory.Keys
				.Where(k => level.GameSave.HasFamiliar((EInventoryFamiliarType)k))
				.ToArray();

			if (familiarTypes.Length <= 1)
				return;

			var equippedFamiliarType = level.GameSave.Inventory.EquippedFamiliar;

			int key = (int)equippedFamiliarType;
			var maxIndex = familiarTypes.OrderBy(k => k).Last();

			for (int index = 0; index < maxIndex; ++index)
			{
				key += isGoingRight ? 1 : -1;

				if (key > maxIndex)
					key = 1;
				else if (key < 1)
					key = maxIndex;

				if (!familiarTypes.Contains(key)) continue;

				level.GameSave.Inventory.EquippedFamiliar = (EInventoryFamiliarType)key;

				var familiarManager = ((object)luniasObject._familiarManager).AsDynamic();

				familiarManager.ChangeFamiliar((EInventoryFamiliarType)key);
				familiarManager.AddFamiliarPoofAnimation();

				return;
			}
		}
	}
}