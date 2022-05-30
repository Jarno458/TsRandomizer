using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation
{
	class ExternalItemLocation : ItemLocation
	{
		static int externalCounter;

		public ExternalItemLocation() : base(new ItemKey(0, 0, 0, externalCounter++), "External", "", null)
		{
		}

		public ExternalItemLocation(ItemInfo item) : this()
		{
			SetItem(item);

			IsPickedUp = true;
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
}
