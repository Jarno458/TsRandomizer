using Timespinner.Core.Specifications;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(9, 8)]
	class XarionSoftlockPrevention : RoomTrigger
	{
		public override void OnRoomLoad(RoomState state)
		{
			var tile1 = new TileSpecification {
				ID = 48,
				Layer = ETileLayerType.Middle,
				IsFlippedHorizontally = false,
				X = 36,
				Y = 32
			};

			var tile2 = new TileSpecification
			{
				ID = 48,
				Layer = ETileLayerType.Middle,
				IsFlippedHorizontally = true,
				X = 37,
				Y = 32
			};

			state.Level.PlaceTile(tile1, false);
			state.Level.PlaceTile(tile2, false);
		}
	}
}
