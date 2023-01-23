using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Settings;

namespace TsRandomizer.Randomisation
{
	enum ETrapType
	{
		None,
		MeteorSparrow,
		Neurotoxin,
		Chaos,
		Poison
	}



	static class TrapManager
	{
		public static void SparrowTrap(Level level)
		{
			var enemyTile = new ObjectTileSpecification
			{
				Category = EObjectTileCategory.Enemy,
				Layer = ETileLayerType.Objects,
				ObjectID = (int)EEnemyTileType.CastleArcher
			};
			var sprite = level.GCM.SpGyreMeteorSparrow;
			var enemyType = TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.Monster.GyreMeteorSparrow");
			var lunaisPos = level.MainHero.Position;
			var position = new Point(lunaisPos.X + 200, lunaisPos.Y + 200);
			var sparrow = enemyType.CreateInstance(false, position, level, sprite, enemyTile);
			level.AsDynamic().RequestAddObject(sparrow);
		}
	}
}
