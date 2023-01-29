using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	abstract class UnchainedKey : CustomItem
	{
		public override int AnimationIndex => 5;

		protected override bool RemoveFromInventory => false;

		public UnchainedKey(ItemUnlockingMap unlockingMap, CustomItemType itemType) : base(unlockingMap, itemType)
		{
		}

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);

			level.ShowGhostDialogueMessage(GetDescriptionKey(ItemType));
		} 
	}
	
	class TimewornWarpBeacon : UnchainedKey
	{
		public TimewornWarpBeacon(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.TimewornWarpBeacon)
		{
		}
	}

	class ModernWarpBeacon : UnchainedKey
	{
		public ModernWarpBeacon(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.ModernWarpBeacon)
		{
		}
	}

	class MysteriousWarpBeacon : UnchainedKey
	{
		public MysteriousWarpBeacon(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.MysteriousWarpBeacon)
		{
		}
	}
}