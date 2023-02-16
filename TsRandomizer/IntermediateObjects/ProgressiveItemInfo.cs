using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects
{
	class ProgressiveItemInfo : ItemInfo
	{
		public ItemInfo[] Items { get; }

		int index;

		public ItemInfo PreviousItem => index > 0 ? Items[index - 1] : Items.First();
		public ItemInfo CurrentItem => index < Items.Length ? Items[index] : Items.Last();

		public override ItemIdentifier Identifier => CurrentItem.Identifier;
		public override Enum TreasureLootType => CurrentItem.TreasureLootType;
		public override int AnimationIndex => CurrentItem.AnimationIndex;
		public override BestiaryItemDropSpecification BestiaryItemDropSpecification => CurrentItem.BestiaryItemDropSpecification;
		internal override Requirement Unlocks => CurrentItem.Unlocks;
		internal override void OnPickup(Level level, GameplayScreen gameplayScreen) => CurrentItem.OnPickup(level, gameplayScreen);

		public ProgressiveItemInfo(params ItemInfo[] items) : this(items, 0)
		{
		}

		ProgressiveItemInfo(ItemInfo[] items, int i)
		{
			Items = items;
			index = i;
		}

		public ProgressiveItemInfo Clone() => new ProgressiveItemInfo(Items, index);

		public void Reset() => index = 0;

		public void Next()
		{
			if (index < Items.Length)
				index++;
		}

		public IEnumerable<ItemInfo> GetAllUnlockedItems()
		{
			for (var i = 0; i < index; i++)
				yield return Items[i];
		}

		public override string ToString() => string.Join(@" -> ", Items.Select(i => i.ToString()));
	}
}