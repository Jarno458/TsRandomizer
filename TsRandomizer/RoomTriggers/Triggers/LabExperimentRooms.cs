using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 1)] // Experiment 13
	[RoomTriggerTrigger(11, 15)]
	[RoomTriggerTrigger(11, 28)]
	[RoomTriggerTrigger(11, 29)]
	[RoomTriggerTrigger(11, 37)]
	// ReSharper disable once UnusedMember.Global
	class LabExperimentRooms : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Seed.Options.LockKeyAmadeus)
				return;

			if (roomState.RoomKey.RoomId == 1)
				SpawnExperimentAdult(roomState, new Point(600, 176));
			else if (roomState.RoomKey.RoomId == 15)
			{
				SpawnExperimentChild(roomState, new Point(61, 10));
				SpawnExperimentChild(roomState, new Point(34, 10));
			}
			else if (roomState.RoomKey.RoomId == 28)
				SpawnExperimentChild(roomState, new Point(24, 11));
			else if (roomState.RoomKey.RoomId == 29)
			{
				SpawnExperimentChild(roomState, new Point(33, 10));
				SpawnExperimentChild(roomState, new Point(47, 10));
			}
			else if (roomState.RoomKey.RoomId == 37)
				SpawnExperimentChild(roomState, new Point(13, 11));
		}

                protected static void SpawnExperimentAdult(RoomState state, Point point)
                {
                        var level = state.Level;
                        var enemyTile = new ObjectTileSpecification
                        {
                                Category = EObjectTileCategory.Enemy,
                                Layer = ETileLayerType.Objects,
                                ObjectID = (int)EEnemyTileType.LabAdult,
                                Argument = 0,
                                IsFlippedHorizontally = false
                        };
                        var enemyType = TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.LabAdult");
                        var sprite = level.GCM.SpLabAdult;
                        var enemy = enemyType.CreateInstance(false, point, level, sprite, -1, enemyTile);

                        enemy.AsDynamic().InitializeMob();

                        level.AsDynamic().RequestAddObject(enemy);
                }

		protected static void SpawnExperimentChild(RoomState state, Point point)
		{
			var level = state.Level;
			var enemyTile = new ObjectTileSpecification
			{
				Category = EObjectTileCategory.Enemy,
				Layer = ETileLayerType.Objects,
				ObjectID = (int)EEnemyTileType.LabChild,
				Argument = 0,
				IsFlippedHorizontally = false,
				X = point.X,
				Y = point.Y
			};

			level.PlaceEvent(enemyTile, false);
		}
	}
}
