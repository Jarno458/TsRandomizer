using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	abstract class LaserAccessKey : CustomItem
	{
		public override int AnimationIndex => 46;

		protected override bool RemoveFromInventory => false;

		public LaserAccessKey(ItemUnlockingMap unlockingMap, CustomItemType itemType) : base(unlockingMap, itemType)
		{
		}

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);

		} 
	}
	
	class LaserAccessA : LaserAccessKey
	{
		public LaserAccessA(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.LaserAccessA)
		{
			SetDescription("ALEANNA-class laser operating manual. Includes deactivaton codes.", null);
		}
	}

	class LaserAccessI : LaserAccessKey
	{
		public LaserAccessI(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.LaserAccessI)
		{
			SetDescription("IDOL-class laser operating manual. Includes deactivaton codes.", null);
		}
	}

	class LaserAccessM : LaserAccessKey
	{
		public LaserAccessM(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.LaserAccessM)
		{
			SetDescription("MAW-class laser operating manual. Includes deactivaton codes.", null);
		}
	}
}
