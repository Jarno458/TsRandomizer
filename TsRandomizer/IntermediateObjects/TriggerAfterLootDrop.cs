using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.Events;
using TsRandomizer.Extensions;

namespace TsRandomizer.IntermediateObjects
{
	class TriggerAfterLootDrop : TriggerBase
	{
		readonly dynamic treasureChest;

		public TriggerAfterLootDrop(Level inLevel, TreasureChestEvent treasureChest, Action action) : base(inLevel, action)
		{
			this.treasureChest = treasureChest.AsDynamic();
		}

		protected override bool ShouldTrigger()
		{
			return treasureChest._hasDroppedLoot;
		}
	}
}