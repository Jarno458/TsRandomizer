using Timespinner.GameAbstractions.Saving;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation
{
	class ExteralItemLocation : ItemLocation
	{
		static int externalCounter;

		public ExteralItemLocation() : base(new ItemKey(0, 0, 0, externalCounter++), "External", "", null)
		{
		}

		public ExteralItemLocation(ItemInfo item) : this()
		{
			SetItem(item);

			IsPickedUp = true;
		}

		public override void SetPickedUp()
		{
			//we are going to need proper progression handling at some point
			//if (ItemInfo is ProgressiveItemInfo progressiveItemInfo) 
			//	progressiveItemInfo.Next();
		}

		public override void BaseOnGameSave(GameSave save)
		{
		}
	}
}
