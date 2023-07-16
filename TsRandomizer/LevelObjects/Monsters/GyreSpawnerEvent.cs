using System.Collections.Generic;
using System.Linq;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Misc.GyreSpawnerEvent")]
	class GyreSpawnerEvent : LevelObject
	{
		public GyreSpawnerEvent(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void OnUpdate()
		{
			if (Dynamic._isRoomCleared)
				return;

			//Update to fix enemizer
			Dynamic._spawnedEnemies = ((IEnumerable<Monster>)LevelReflected._enemies.Values).ToList();
		}
	}
}
