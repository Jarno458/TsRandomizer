using System;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects
{
	class SingleItemInfo : ItemInfo
	{
		public override ItemIdentifier Identifier { get; }
		internal override Requirement Unlocks { get; }

		public override Enum TreasureLootType => Identifier.LootType.ToETreasureLootType();
		public override int AnimationIndex => Identifier.GetAnimationIndex();
		public override BestiaryItemDropSpecification BestiaryItemDropSpecification => Identifier.GetBestiaryItemDropSpecification();

		Action<Level> PickupAction { get; }

		internal SingleItemInfo(ItemUnlockingMap unlockingMap, ItemIdentifier identifier)
		{
			Identifier = identifier;
			Unlocks = unlockingMap.GetAllUnlock(identifier);
			PickupAction = unlockingMap?.GetPickupAction(identifier);
		}

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			PickupAction?.Invoke(level);
		}
	}
}