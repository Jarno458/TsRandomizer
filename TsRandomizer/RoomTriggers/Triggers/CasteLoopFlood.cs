using Microsoft.Xna.Framework;

namespace TsRandomizer.RoomTriggers.Triggers
{
	/*[RoomTriggerTrigger(5, 1)]
	[RoomTriggerTrigger(5, 3)]
	[RoomTriggerTrigger(5, 9)]
	[RoomTriggerTrigger(5, 10)]
	[RoomTriggerTrigger(5, 11)]
	[RoomTriggerTrigger(5, 13)]
	[RoomTriggerTrigger(5, 15)]
	[RoomTriggerTrigger(5, 16)]
	[RoomTriggerTrigger(5, 33)]
	[RoomTriggerTrigger(5, 35)]
	[RoomTriggerTrigger(5, 37)]
	[RoomTriggerTrigger(5, 38)]
	[RoomTriggerTrigger(5, 43)]
	[RoomTriggerTrigger(5, 44)]
	[RoomTriggerTrigger(5, 45)]
	class CasteLoopFlood : RoomTrigger
	{
		public override void OnRoomLoad(RoomState state)
		{
			if (!state.SeedOptions.FloodBasement)
				return;

			switch (state.RoomKey.RoomId)
			{
				case 3:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 24), state.Level.RoomSize16);
					break;
				case 1:
				case 13:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 8), state.Level.RoomSize16);
					break;
				case 9:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 5), state.Level.RoomSize16);
					break;
				case 11:
				case 38:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 7), state.Level.RoomSize16);
					break;
				case 37:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 9), state.Level.RoomSize16);
					break;
				case 10:
					var waterLeftOffset = state.Level.GameSave.GetSaveBool("BW_5_10_0") ? 0 : 4;
					RoomTriggerHelper.PlaceWater(state.Level, new Point(waterLeftOffset, 7), state.Level.RoomSize16);
					break;
				default:
					RoomTriggerHelper.FillRoomWithWater(state.Level);
					break;
			}
		}
	}*/
}
