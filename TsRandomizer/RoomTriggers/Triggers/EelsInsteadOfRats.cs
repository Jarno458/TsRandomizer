
using System;
using System.Collections.Generic;
using Timespinner.Core.Specifications;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(8, 14)]
	[RoomTriggerTrigger(8, 45)]
	[RoomTriggerTrigger(9, 14)]
	class EelsInsteadOfRats : RoomTrigger
	{
		static readonly Type EelType = TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.LakeEel");

		public override void OnRoomLoad(RoomState state)
		{
			if (state.RoomKey.LevelId == 8 && !state.Seed.FloodFlags.Maw)
				return;

			if (state.RoomKey.LevelId == 9 && !state.Seed.FloodFlags.Xarion)
				return;

			var dynamicLevel = state.Level.AsDynamic();

			foreach (var monster in ((Dictionary<int, Monster>)dynamicLevel._enemies).Values)
			{
				if (monster.EnemyType != EEnemyTileType.CavesCopperWyvern)
					return;

				var pos = monster.Position;
				monster.SilentKill();

				var enemyTile = new ObjectTileSpecification
				{
					Category = EObjectTileCategory.Enemy,
					Layer = ETileLayerType.Objects,
					ObjectID = (int)EEnemyTileType.LakeEel
				};

				var sprite = state.Level.GCM.SpLakeEel;
				var enemy = EelType.CreateInstance(false, pos, state.Level, sprite, -1, enemyTile);

				dynamicLevel.RequestAddObject(enemy);
			}
		}
	}
}
