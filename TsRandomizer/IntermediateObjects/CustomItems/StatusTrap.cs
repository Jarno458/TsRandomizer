using System;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Extensions;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class StatusTrap : CustomItem
	{
		static Type StatusEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.StatusEffects.EStatusEffectType");

		readonly object statusEnumValue;

		public override int AnimationIndex => 208; // 'starry void' item

		public StatusTrap(CustomItemType itemType, string statusToApply) : base(itemType)
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
		public NeurotoxinTrap() : base(CustomItemType.NeurotoxinTrap, "NeuroToxin") { }
	}

	class ChaosTrap : StatusTrap
	{
		public ChaosTrap() : base(CustomItemType.ChaosTrap, "Chaos") { }
	}

	class PoisonTrap : StatusTrap
	{
		public PoisonTrap() : base(CustomItemType.PoisonTrap, "Poison") { }
	}
}