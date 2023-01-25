using System;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Randomisation;

namespace TsRandomizer.IntermediateObjects
{
	public class SingleItemInfo : ItemInfo
	{
		public override ItemIdentifier Identifier { get; }
		internal override Requirement Unlocks { get; }

		public override Enum TreasureLootType => Identifier.LootType.ToETreasureLootType();
		public override int AnimationIndex => Identifier.GetAnimationIndex();
		public override BestiaryItemDropSpecification BestiaryItemDropSpecification => Identifier.GetBestiaryItemDropSpecification();

		public Action<Level> PickupAction { get; protected set; }

		public SingleItemInfo(ItemIdentifier identifier)
		{
			Identifier = identifier;
		}

		internal SingleItemInfo(ItemUnlockingMap unlockingMap, ItemIdentifier identifier)
		{
			Identifier = identifier;
			Unlocks = unlockingMap.GetAllUnlock(identifier);
			PickupAction = unlockingMap.GetPickupAction(identifier);
		}

		public override void OnPickup(Level level) => PickupAction?.Invoke(level);
	}
}