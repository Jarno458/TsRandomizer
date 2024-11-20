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
				SpawnExperiment(roomState, new Point(600, 176), true);
			else if (roomState.RoomKey.RoomId == 15)
			{
				SpawnExperiment(roomState, new Point(984, 176), false);
				SpawnExperiment(roomState, new Point(552, 176), false);
			}
			else if (roomState.RoomKey.RoomId == 28)
				SpawnExperiment(roomState, new Point(392, 192), false);
			else if (roomState.RoomKey.RoomId == 29)
			{
				SpawnExperiment(roomState, new Point(536, 176), false);
				SpawnExperiment(roomState, new Point(760, 176), false);
			}
			else if (roomState.RoomKey.RoomId == 37)
				SpawnExperiment(roomState, new Point(216, 192), false);
		}

		protected static void SpawnExperiment(RoomState state, Point point, bool isAdult)
		{
			var level = state.Level;
			var enemyTile = new ObjectTileSpecification
			{
				Category = EObjectTileCategory.Enemy,
				Layer = ETileLayerType.Objects,
				ObjectID = isAdult ? (int)EEnemyTileType.LabAdult : (int)EEnemyTileType.LabChild,
				Argument = 0,
				IsFlippedHorizontally = false
			};
			var enemyType = isAdult 
				? TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.LabAdult") 
				: TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.LabChild");
			var sprite = isAdult ? level.GCM.SpLabAdult : level.GCM.SpLabChild;
			var enemy = enemyType.CreateInstance(false, point, level, sprite, -1, enemyTile);

			enemy.AsDynamic().InitializeMob();

			level.AsDynamic().RequestAddObject(enemy);
		}
	}
}
