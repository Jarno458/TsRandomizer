using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class UnchainedKey : CustomItem
	{
		public override int AnimationIndex => 5;
		protected override bool RemoveFromInventory => false;

		public UnchainedKey(CustomItemType itemType) : base(itemType)
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
		public override string Name => "Timeworn Warp Beacon";

		public TimewornWarpBeacon() : base(CustomItemType.TimewornWarpBeacon)
		{
			SetDescription("Attunes warps to a gate in the past", "Twin Pyramid Key");
		}
	}

	class ModernWarpBeacon : UnchainedKey
	{
		public override string Name => "Modern Warp Beacon";

		public ModernWarpBeacon() : base(CustomItemType.ModernWarpBeacon)
		{
			SetDescription("Attunes warps gate within the present", "Twin Pyramid Key");
		}
	}

	class MysteriousWarpBeacon : UnchainedKey
	{
		public override string Name => "Mysterious Warp Beacon";

		public MysteriousWarpBeacon() : base(CustomItemType.MysteriousWarpBeacon)
		{
			SetDescription("Attunes warps to a gate beyond time", "Twin Pyramid Key");
		}
	}
}