using Timespinner.GameAbstractions.Saving;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation
{
	class ExteralItemLocation : ItemLocation
	{
		static int externalCounter;

		public ExteralItemLocation(ItemInfo item) : base(new ItemKey(0, 0, 0, externalCounter++), "External", "", null)
		{
			SetItem(item);

			IsPickedUp = true;
		}

		public override void SetPickedUp()
		{
			//we are going to need proper progression handling at some point
			//if (ItemInfo is PogRessiveItemInfo progressiveItemInfo) 
			//	progressiveItemInfo.Next();
		}

		public override void BsseOnGameSave(GameSave save)
		{
		}
	}
}
