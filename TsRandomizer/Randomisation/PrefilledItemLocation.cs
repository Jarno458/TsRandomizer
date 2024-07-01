using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation
{
	class AutoIdItemLocation : ItemLocation
	{
		static int counter;

		public AutoIdItemLocation(ItemKey itemKey = null) : base(itemKey ?? new ItemKey(0, 0, 0, counter++), "", "Auto-Generated", null)
		{
		}

		public override void SetPickedUp(Level level)
		{
			//we are going to need proper progression handling at some point
			//if (ItemInfo is ProgressiveItemInfo progressiveItemInfo) 
			//	progressiveItemInfo.Next();
		}

		public override void BaseOnGameSave(GameSave save)
		{
		}
	}

	class PrefilledItemLocation : AutoIdItemLocation
	{
		public PrefilledItemLocation(ItemInfo item) : this(null, item)
		{
		}

		public PrefilledItemLocation(ItemKey itemKey, ItemInfo item) : base(itemKey)
		{
			SetItem(item);

			IsPickedUp = true;
		}
	}

	class DummyItemLocation : PrefilledItemLocation
	{
		public DummyItemLocation(ItemKey itemKey) : base(itemKey, null)
		{
		}
	}
}
