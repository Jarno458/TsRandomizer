using Timespinner.Core.Specifications;
using TsRandomizer.IntermediateObjects;
using System;
using Microsoft.Xna.Framework;
using TsRandomizer.Extensions;


namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 3)]
	class LabLowerTrash : RoomTrigger
	{
		static readonly Type LaserType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.EnvironmentPrefabs.L11_Lab.EnvPrefabLabForceField");
		public override void OnRoomLoad(RoomState roomState)
		{

			if (roomState.Seed.Options.LockKeyAmadeus)
				return;

			Point[] lasers = new[] { new Point(492, 112), new Point(516, 112) };
			foreach (Point point in lasers)
			{
				var laser = LaserType.CreateInstance(false, roomState.Level, point, -1, new ObjectTileSpecification(), Timespinner.GameObjects.Events.EnvironmentPrefabs.EEnvironmentPrefabType.L11_ForceField);
				roomState.Level.AsDynamic().RequestAddObject(laser);
			}

			// Remove barrier wall tiles if powered down
			if (roomState.Level.GameSave.GetSaveBool("11_LabPower"))
            {
				TileSpecification[] tiles = new[] {
					new TileSpecification { ID = 510, X = 31, Y = 6, Layer = ETileLayerType.Top },
					new TileSpecification { ID = 510, X = 31, Y = 5, Layer = ETileLayerType.Top },
					new TileSpecification { ID = 510, X = 31, Y = 4, Layer = ETileLayerType.Top },
					new TileSpecification { ID = 510, X = 31, Y = 3, Layer = ETileLayerType.Top }
					};
				foreach (TileSpecification tile in tiles)
				{
					roomState.Level.ReplaceTile(tile);
				}
			}
		}
	}
	[RoomTriggerTrigger(11, 18)]
	class LabUpperTrash : RoomTrigger
	{
		struct Engineer
		{
			public Point position;
			public bool isFlipped;
		}
		public override void OnRoomLoad(RoomState roomState)
		{
			// Only run room trigger when power is off
			if (roomState.Seed.Options.LockKeyAmadeus || !roomState.Level.GameSave.GetSaveBool("11_LabPower"))
			  return;

			TileSpecification[] replaceTiles = new[] {
				new TileSpecification { ID = 22, X = 15, Y = 2, Layer = ETileLayerType.Middle},
				new TileSpecification { ID = 22, X = 16, Y = 2, Layer = ETileLayerType.Middle},
				new TileSpecification { ID = 22, X = 21, Y = 2, Layer = ETileLayerType.Middle},
				new TileSpecification { ID = 22, X = 22, Y = 2, Layer = ETileLayerType.Middle},
				new TileSpecification { ID = 22, X = 29, Y = 2, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 22, X = 30, Y = 2, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 22, X = 35, Y = 2, Layer = ETileLayerType.Middle},
				new TileSpecification { ID = 22, X = 36, Y = 2, Layer = ETileLayerType.Middle},
			};
			foreach (TileSpecification tile in replaceTiles)
			{
				roomState.Level.ReplaceTile(tile);
			}

			Engineer[] engineers = new[] {
				 new Engineer
				 {
					position = new Point(256, 16),
					isFlipped = true
				 },
				 new Engineer
				 {
					position = new Point(352, 16),
					isFlipped = false
				 },
				 new Engineer
				 {
					position = new Point(480, 16),
					isFlipped = true
				 },
				 new Engineer
				 {
					position = new Point(576, 16),
					isFlipped = false
				 },
			};
			var enemyTile = new ObjectTileSpecification
			{
				Category = EObjectTileCategory.Enemy,
				Layer = ETileLayerType.Objects,
				ObjectID = (int)EEnemyTileType.FortressEngineer,
				Argument = 1,
				IsFlippedHorizontally = false
			};
			var enemyType = TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.FortressEngineer");
			var sprite = roomState.Level.GCM.SpFortressEngineer;
			foreach (Engineer engineer in engineers)
			{
				enemyTile.IsFlippedHorizontally = engineer.isFlipped;
				var enemy = enemyType.CreateInstance(false, engineer.position, roomState.Level, sprite, -1, enemyTile);
				enemy.AsDynamic().InitializeMob();
				roomState.Level.AsDynamic().RequestAddObject(enemy);
			}
		}
	}

	[RoomTriggerTrigger(11, 2)]
	class DynamoShaft : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			// Power is always off here except during Lock Key Amadeus
			if (roomState.Seed.Options.LockKeyAmadeus)
				return;
			TileSpecification[] deleteTiles = new[] {
				new TileSpecification { ID = 513, X = 9, Y = 20, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 15, Y = 27, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 9, Y = 34, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 15, Y = 41, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 9, Y = 48, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 15, Y = 55, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 9, Y = 62, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 15, Y = 69, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 9, Y = 76, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 15, Y = 83, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 9, Y = 90, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 15, Y = 97, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 9, Y = 104, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 513, X = 15, Y = 106, Layer = ETileLayerType.Middle },
			};
			foreach (TileSpecification tile in deleteTiles)
			{
				roomState.Level.DeleteTile(tile);
			}
			// Set Background
			foreach (TileSpecification tile in deleteTiles)
			{
				tile.ID = 258;
				tile.Layer = ETileLayerType.Bottom;
				roomState.Level.PlaceTile(tile, true);
			}
			TileSpecification[] replaceTiles = new[] {
				new TileSpecification { ID = 21, X = 9, Y = 19, Layer = ETileLayerType.Middle, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 9, Y = 21, Layer = ETileLayerType.Middle},
				new TileSpecification { ID = 21, X = 15, Y = 26, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 15, Y = 28, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true},
				new TileSpecification { ID = 21, X = 9, Y = 33, Layer = ETileLayerType.Middle, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 9, Y = 35, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 21, X = 15, Y = 40, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 15, Y = 42, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
				new TileSpecification { ID = 21, X = 9, Y = 47, Layer = ETileLayerType.Middle, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 9, Y = 49, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 21, X = 15, Y = 54, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 15, Y = 56, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
				new TileSpecification { ID = 21, X = 9, Y = 61, Layer = ETileLayerType.Middle, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 9, Y = 63, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 21, X = 15, Y = 68, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 15, Y = 70, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
				new TileSpecification { ID = 21, X = 9, Y = 75, Layer = ETileLayerType.Middle, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 9, Y = 77, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 21, X = 15, Y = 82, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 15, Y = 84, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
				new TileSpecification { ID = 21, X = 9, Y = 89, Layer = ETileLayerType.Middle, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 9, Y = 91, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 21, X = 15, Y = 96, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 15, Y = 98, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
				new TileSpecification { ID = 21, X = 9, Y = 103, Layer = ETileLayerType.Middle, IsFlippedVertically = true },
				new TileSpecification { ID = 21, X = 9, Y = 105, Layer = ETileLayerType.Middle },
				new TileSpecification { ID = 21, X = 15, Y = 105, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true, IsFlippedVertically = true },
				new TileSpecification { ID = 22, X = 15, Y = 107, Layer = ETileLayerType.Middle, IsFlippedHorizontally = true },
				new TileSpecification { ID = 22, X = 16, Y = 107, Layer = ETileLayerType.Middle },
			};
			foreach (TileSpecification tile in replaceTiles)
			{
				roomState.Level.ReplaceTile(tile);
			}
		}
	}
}
