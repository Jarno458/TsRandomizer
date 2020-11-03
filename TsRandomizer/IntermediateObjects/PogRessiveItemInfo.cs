using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Randomisation;

namespace TsRandomizer.IntermediateObjects
{
	class PogRessiveItemInfo : ItemInfo
	{
		ItemInfo[] Items { get; }

		int index;
		ItemInfo CurrentItem => index < Items.Length ? Items[index] : Items.Last();

		public override ItemIdentifier Identifier => CurrentItem.Identifier;
		public override Enum TreasureLootType => CurrentItem.TreasureLootType;
		public override int AnimationIndex => CurrentItem.AnimationIndex;
		public override BestiaryItemDropSpecification BestiaryItemDropSpecification => CurrentItem.BestiaryItemDropSpecification;
		internal override Requirement Unlocks => CurrentItem.Unlocks;
		public override void OnPickup(Level level) => CurrentItem.OnPickup(level);

		public PogRessiveItemInfo(params ItemInfo[] items) : this(items, 0)
		{
		}

		PogRessiveItemInfo(ItemInfo[] items, int i)
		{
			Items = items;
			index = i;
		}

		public PogRessiveItemInfo Clone()
		{
			return new PogRessiveItemInfo(Items, index);
		}

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

		public override string ToString()
		{
			return string.Join(@" -> ", Items.Select(i => i.ToString()));
		}
	}
}