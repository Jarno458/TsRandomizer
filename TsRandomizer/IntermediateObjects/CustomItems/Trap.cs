using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;


namespace TsRandomizer.IntermediateObjects.CustomItems
{
	abstract class Trap : CustomItem
	{
		public override int AnimationIndex => 208; // 'starry void' item

		public Trap(ItemUnlockingMap unlockingMap, CustomItemType itemType) : base(unlockingMap, itemType)
		{
		}

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);
			level.JukeBox.PlayCue(Timespinner.GameAbstractions.ESFX.DoorKeycardError);
		}
	}
}
