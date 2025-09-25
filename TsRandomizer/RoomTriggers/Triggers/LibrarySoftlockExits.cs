using Timespinner.Core.Specifications;


namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(2, 23)]
	class LibrarySoftLockLower: RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			TileSpecification[] tiles = new[] {
				new TileSpecification { ID = 261, X = 66, Y = 34, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 260, X = 67, Y = 34, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 259, X = 66, Y = 33, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 258, X = 67, Y = 33, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 257, X = 66, Y = 32, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 256, X = 67, Y = 32, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 513, X = 66, Y = 32, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
				new TileSpecification { ID = 513, X = 67, Y = 32, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
				new TileSpecification { ID = 262, X = 66, Y = 31, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 513, X = 66, Y = 31, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
			};
			foreach (TileSpecification tile in tiles)
			{
				roomState.Level.ReplaceTile(tile);
			}
		}
	}

	[RoomTriggerTrigger(2, 20)]
	class LibrarySoftLockUpper: RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			TileSpecification[] tiles = new[] {
				new TileSpecification { ID = 261, X = 11, Y = 55, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 260, X = 12, Y = 55, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 259, X = 11, Y = 54, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 258, X = 12, Y = 53, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 259, X = 11, Y = 53, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 258, X = 12, Y = 54, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 257, X = 11, Y = 52, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 256, X = 12, Y = 52, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 513, X = 11, Y = 52, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
				new TileSpecification { ID = 513, X = 12, Y = 52, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
				new TileSpecification { ID = 262, X = 11, Y = 51, Layer = ETileLayerType.Bottom, IsFlippedHorizontally = true },
				new TileSpecification { ID = 513, X = 11, Y = 51, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
			};
			foreach (TileSpecification tile in tiles)
			{
				roomState.Level.ReplaceTile(tile);
			}
		}
	}
}
