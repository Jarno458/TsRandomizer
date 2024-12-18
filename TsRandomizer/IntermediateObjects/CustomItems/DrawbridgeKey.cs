using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class DrawbridgeKey : CustomItem
	{
		public override int AnimationIndex => 236;

		protected override bool RemoveFromInventory => false;

		public DrawbridgeKey(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.DrawbridgeKey)
		{
			SetDescription("Holders of this key may approach the castle to find the drawbridge lowered.", null);
		}

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);
			level.JukeBox.PlayCue(Timespinner.GameAbstractions.ESFX.FoleyDrawbridgeLockProper);
		}
	}
}
