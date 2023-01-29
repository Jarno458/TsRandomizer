using System;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	abstract class StatusTrap : Trap
	{
		static Type StatusEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.StatusEffects.EStatusEffectType");

		readonly object statusEnumValue;

		public StatusTrap(ItemUnlockingMap unlockingMap, CustomItemType itemType, string statusToApply) 
			: base(unlockingMap, itemType)
		{
			statusEnumValue = StatusEnumType.GetEnumValue(statusToApply);
		}

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);

			level.MainHero.AsDynamic().GiveStatusEffect(statusEnumValue, 100);
		}
	}

	class NeurotoxinTrap : StatusTrap
	{
		public NeurotoxinTrap(ItemUnlockingMap unlockingMap) 
			: base(unlockingMap, CustomItemType.NeurotoxinTrap, "NeuroToxin") { }
	}

	class ChaosTrap : StatusTrap
	{
		public ChaosTrap(ItemUnlockingMap unlockingMap) 
			: base(unlockingMap, CustomItemType.ChaosTrap, "Chaos") { }
	}

	class PoisonTrap : StatusTrap
	{
		public PoisonTrap(ItemUnlockingMap unlockingMap) 
			: base(unlockingMap, CustomItemType.PoisonTrap, "Poison") { }
	}
}