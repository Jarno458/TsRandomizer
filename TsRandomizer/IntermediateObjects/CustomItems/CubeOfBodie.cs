using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class CubeOfBodie : CustomItem
	{
		public override int AnimationIndex => 240;

		protected override bool RemoveFromInventory => false;

		public CubeOfBodie(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.CubeOfBodie)
		{
			SetDescription("A strange box that creates items from lanterns.", null);
		}

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);
			level.JukeBox.PlayCue(Timespinner.GameAbstractions.ESFX.MeyefMeow);
		}
	}
}
