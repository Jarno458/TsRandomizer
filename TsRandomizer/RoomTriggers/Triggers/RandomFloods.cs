using System;
using Microsoft.Xna.Framework;

namespace TsRandomizer.RoomTriggers.Triggers
{
	//castleloop
	[RoomTriggerTrigger(5, 1)]
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


	//xarion
	[RoomTriggerTrigger(9, 7)]
	[RoomTriggerTrigger(9, 8)]
	[RoomTriggerTrigger(9, 13)]
	[RoomTriggerTrigger(9, 14)]
	[RoomTriggerTrigger(9, 21)]
	[RoomTriggerTrigger(9, 33)]
	[RoomTriggerTrigger(9, 36)]
	[RoomTriggerTrigger(9, 43)]
	[RoomTriggerTrigger(9, 49)]
	class RandomFloods : RoomTrigger
	{
		public override void OnRoomLoad(RoomState state)
		{
			var intenral = new Random((int)~state.Seed.Id);

			var floodBasementHigh = state.Seed.Options.FloodBasement && intenral.Next() % 2 == 0;
			var floodBasement = state.Seed.Options.FloodBasement && intenral.Next() % 4 == 0;
			var floodMaw = state.Seed.Options.FloodBasement && intenral.Next() % 4 == 0;
			//var floodXarion = state.Seed.Options.FloodBasement && intenral.Next() % 4 == 0;
			var floodPyramid = state.Seed.Options.FloodBasement && intenral.Next() % 4 == 0;

			var floodXarion = true;

			switch (state.RoomKey.LevelId)
			{
				case 5 when floodBasement:
					HandleCastleLoopFlood(state, floodBasementHigh);
					break;
				case 9 when floodXarion:
					HandleXarionFlood(state);
					break;
			}
		}

		static void HandleCastleLoopFlood(RoomState state, bool high)
		{
			if (high)
			{
				switch (state.RoomKey.RoomId)
				{
					case 3:
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 20), state.Level.RoomSize16);
						break;
					case 1:
					case 13:
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 4), state.Level.RoomSize16);
						break;
					case 9:
					case 11:
					case 38:
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 3), state.Level.RoomSize16);
						break;
					case 10:
						var waterLeftOffset = state.Level.GameSave.GetSaveBool("BW_5_10_0") ? 0 : 4;
						RoomTriggerHelper.PlaceWater(state.Level, new Point(waterLeftOffset, 3), state.Level.RoomSize16);
						break;
					default:
						RoomTriggerHelper.FillRoomWithWater(state.Level);
						break;
				}
			}
			else
			{
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
		}

		static void HandleXarionFlood(RoomState state)
		{
			switch (state.RoomKey.RoomId)
			{
				case 8:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 23), state.Level.RoomSize16);
					break;
				case 21:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 42), state.Level.RoomSize16);
					break;
				default:
					RoomTriggerHelper.FillRoomWithWater(state.Level);
					break;
			}
		}
	}
}
