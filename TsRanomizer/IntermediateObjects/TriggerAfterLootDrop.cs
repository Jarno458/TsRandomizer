using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.Events;
using TsRanodmizer.Extensions;

namespace TsRanodmizer.IntermediateObjects
{
	class TriggerAfterLootDrop : TriggerBase
	{
		readonly dynamic treasureChest;

		public TriggerAfterLootDrop(Level inLevel, TreasureChestEvent treasureChest, Action action) : base(inLevel, action)
		{
			this.treasureChest = treasureChest.Reflect();
		}

		protected override void OnUpdate()
		{
			if (treasureChest._hasDroppedLoot)
				Trigger();
		}
	}
}