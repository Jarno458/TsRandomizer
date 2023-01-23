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
			ObjectTileSpecification enemyTile = new ObjectTileSpecification();
			enemyTile.Category = EObjectTileCategory.Enemy;
			enemyTile.Layer = ETileLayerType.Objects;
			enemyTile.ObjectID = (int)EEnemyTileType.FlyingCheveux;

			var lunaisPos = level.MainHero.LastPosition;
			var sprite = level.GCM.SpGyreMeteorSparrow;
			var enemyType = TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.GyreMeteorSparrow");
			var enemy = enemyType.CreateInstance(false, new Point(lunaisPos.X + 100, lunaisPos.Y - 50), level, sprite, -1, enemyTile);
			enemy.AsDynamic()._isAggroed = true;
			level.AsDynamic().RequestAddObject(enemy);


			enemy = enemyType.CreateInstance(false, new Point(lunaisPos.X - 100, lunaisPos.Y - 50), level, sprite, -1, enemyTile);
			enemy.AsDynamic()._isAggroed = true;
			level.AsDynamic().RequestAddObject(enemy);
			level.JukeBox.PlayCue(Timespinner.GameAbstractions.ESFX.CsPrologueTableFlip);
		}
	}
}
