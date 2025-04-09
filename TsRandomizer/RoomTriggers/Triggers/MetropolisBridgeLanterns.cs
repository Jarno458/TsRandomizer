using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(1, 7)]
	// ReSharper disable once UnusedMember.Global
	class MetropolisBridgeLanterns : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Seed.Options.PureTorcher)
				return;
			SpawnLantern(roomState, new Point(472, 95));
			SpawnLantern(roomState, new Point(664, 95));
			SpawnLantern(roomState, new Point(856, 95));
			SpawnLantern(roomState, new Point(1054, 95));
			SpawnLantern(roomState, new Point(1240, 95));
			SpawnLantern(roomState, new Point(1432, 95));
			SpawnLantern(roomState, new Point(1624, 95));
		}

        protected static void SpawnLantern(RoomState state, Point point)
        {
                var level = state.Level;
                var lanternTile = new ObjectTileSpecification
                {
                        Category = EObjectTileCategory.Event,
                        Layer = ETileLayerType.Objects,
                        ObjectID = (int)EEventTileType.Lantern,
                        Argument = 0,
                        IsFlippedHorizontally = false
                };
                var lanternType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Lanterns.GyreLanternEvent");
                var lantern = lanternType.CreateInstance(false, level, point, -1, lanternTile);
                level.AsDynamic().RequestAddObject(lantern);
        }
	}
}
