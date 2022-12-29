using System;
using Microsoft.Xna.Framework;

namespace TsRandomizer.RoomTriggers.Triggers
{
	class RandomFloodsFlags
	{
		public bool FloodBasementHigh;
		public bool FloodBasement;
		public bool FloodXarion;
		public bool FloodMaw;
		public bool FloodPyramid;
		public bool FloodBackPyramid;
		
		public RandomFloodsFlags(Seed seed)
		{
			if (!seed.Options.FloodBasement)
				return;

			var random = new Random((int)~seed.Id);

			FloodBasementHigh = random.Next() % 2 == 0;
			FloodBasement = random.Next() % 4 == 0;
			FloodXarion = random.Next() % 4 == 0;
			FloodMaw = random.Next() % 4 == 0;
			FloodPyramid = random.Next() % 4 == 0;
			FloodBackPyramid = random.Next() % 4 == 0;

			FloodBasementHigh = true;
			FloodBasement = true;
			FloodXarion = true;
			FloodMaw = true;
			FloodPyramid = true;
			FloodBackPyramid = true;
		}
	}
	
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

	//maw
	[RoomTriggerTrigger(8, 6)]
	[RoomTriggerTrigger(8, 7)]
	[RoomTriggerTrigger(8, 8)]
	[RoomTriggerTrigger(8, 9)]
	[RoomTriggerTrigger(8, 12)]
	[RoomTriggerTrigger(8, 13)]
	[RoomTriggerTrigger(8, 14)]
	[RoomTriggerTrigger(8, 15)]
	[RoomTriggerTrigger(8, 16)]
	[RoomTriggerTrigger(8, 19)]
	[RoomTriggerTrigger(8, 21)]
	[RoomTriggerTrigger(8, 31)]
	[RoomTriggerTrigger(8, 33)]
	[RoomTriggerTrigger(8, 34)]
	[RoomTriggerTrigger(8, 36)]
	[RoomTriggerTrigger(8, 38)]
	[RoomTriggerTrigger(8, 40)]
	[RoomTriggerTrigger(8, 44)]
	[RoomTriggerTrigger(8, 43)]
	[RoomTriggerTrigger(8, 45)]
	[RoomTriggerTrigger(8, 47)]
	[RoomTriggerTrigger(8, 48)]
	[RoomTriggerTrigger(8, 49)]

	//pyramid back
	[RoomTriggerTrigger(16, 1)]
	[RoomTriggerTrigger(16, 4)]
	[RoomTriggerTrigger(16, 5)]
	//pyramid shaft
	[RoomTriggerTrigger(16, 6)]
	[RoomTriggerTrigger(16, 8)]
	[RoomTriggerTrigger(16, 16)]
	[RoomTriggerTrigger(16, 20)]
	[RoomTriggerTrigger(16, 21)]
	[RoomTriggerTrigger(16, 22)]
	[RoomTriggerTrigger(16, 23)]
	class RandomFloods : RoomTrigger
	{
		public override void OnRoomLoad(RoomState state)
		{
			var flags = new RandomFloodsFlags(state.Seed);

			switch (state.RoomKey.LevelId)
			{
				case 5 when flags.FloodBasement:
					HandleCastleLoopFlood(state, flags.FloodBasementHigh);
					break;
				case 9 when flags.FloodXarion:
					HandleXarionFlood(state);
					break;
				case 8 when flags.FloodMaw:
					HandleMawFlood(state);
					break;
				case 16:
					if (flags.FloodPyramid)
						HandleFloodPyramidShaft(state);
					if(flags.FloodBackPyramid)
						HandleFloodPyramidBack(state);
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

		static void HandleMawFlood(RoomState state)
		{
			switch (state.RoomKey.RoomId)
			{
				case 9:
					var isBreakableWallBroken = state.Level.GameSave.GetSaveBool("BW_8_9_0");

					if (isBreakableWallBroken)
					{
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 0), state.Level.RoomSize16);
					}
					else
					{
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 0), new Point(state.Level.RoomSize16.X - 4, state.Level.RoomSize16.Y));
						RoomTriggerHelper.PlaceWater(state.Level, new Point(state.Level.RoomSize16.X - 4, 40), state.Level.RoomSize16);
					}
					break;
				case 16:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 17), state.Level.RoomSize16);
					break;
				case 19:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 53), state.Level.RoomSize16);
					break;
				case 31:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 73), state.Level.RoomSize16);
					break;
				case 38:
				case 40:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 6), state.Level.RoomSize16);
					break;
				case 44:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 30), state.Level.RoomSize16);
					break;
				default:
					RoomTriggerHelper.FillRoomWithWater(state.Level);
					break;
			}
		}

		void HandleFloodPyramidShaft(RoomState state)
		{
			switch (state.RoomKey.RoomId)
			{
				case 6:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 10), state.Level.RoomSize16);
					break;
				case 8:
					var waterRightOffset8 = state.Level.GameSave.GetSaveBool("BW_16_8_0") 
						? state.Level.RoomSize16.X
						: state.Level.RoomSize16.X - 3;
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 0), new Point(waterRightOffset8, state.Level.RoomSize16.Y));
					break;
				case 22:
					var waterRightOffset22 = state.Level.GameSave.GetSaveBool("BW_16_22_0") 
						? state.Level.RoomSize16.X 
						: state.Level.RoomSize16.X - 3;
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 0), new Point(waterRightOffset22, state.Level.RoomSize16.Y));
					break;
				case 16:
				case 20:
				case 21:
				case 23:
					RoomTriggerHelper.FillRoomWithWater(state.Level);
					break;
			}
		}

		void HandleFloodPyramidBack(RoomState state)
		{
			switch (state.RoomKey.RoomId)
			{
				case 1:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 50), state.Level.RoomSize16);
					break;
				case 4:
				case 5:
					RoomTriggerHelper.FillRoomWithWater(state.Level);
					break;
			}
		}
	}
}
