using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;


namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 1)] // Experiment 13
	// [RoomTriggerTrigger(11, 35)] // Genza
	// [RoomTriggerTrigger(11, 37)] // Research Wing
	// 11, 39 also has a laser but is covered as Dynamo Works
	class LabExperimentRooms : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			SpawnExperiment(roomState, new Point(600, 176), true);
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
			var enemyType = isAdult ? TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.LabAdult") : TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.LabChild");
			var sprite = isAdult ? level.GCM.SpLabAdult : level.GCM.SpLabChild;
			var enemy = enemyType.CreateInstance(false, point, level, sprite, -1, enemyTile);
			level.AsDynamic().RequestAddObject(enemy);
		}
	}
}
