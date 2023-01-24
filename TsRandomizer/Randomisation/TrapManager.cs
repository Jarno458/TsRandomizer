using System;
using System.Collections.Generic;
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
		public static void TriggerRandomTrap(Level level, Random random)
		{
			ETrapType[] validTraps = GetValidTraps();
			// TODO: make random seed varied on chest room/location
			ETrapType selectedTrap = validTraps[random.Next(validTraps.Length)];
			TriggerTrap(level, selectedTrap);
		}
		public static void TriggerTrap(Level level, ETrapType trapType)
		{
			switch (trapType)
			{
				case ETrapType.MeteorSparrow:
					SparrowTrap(level);
					break;
				case ETrapType.Neurotoxin:
					NeurotoxinTrap(level);
					break;
				case ETrapType.Chaos:
					ChaosTrap(level);
					break;
				case ETrapType.Poison:
					PoisonTrap(level);
					break;
				default:
					return;
			}
			level.JukeBox.PlayCue(Timespinner.GameAbstractions.ESFX.DoorKeycardError);
		}
		static ETrapType[] GetValidTraps()
		{
			List<ETrapType> validTraps = new List<ETrapType>();
			// TODO: assemble list based on settings
			validTraps.Add(ETrapType.MeteorSparrow);

			if (validTraps.Count == 0)
				validTraps.Add(ETrapType.None);
			return validTraps.ToArray();
		}
		static void SparrowTrap(Level level)
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
		}

		static void NeurotoxinTrap(Level level)
		{

		}

		static void ChaosTrap(Level level)
		{

		}

		static void PoisonTrap(Level level)
		{

		}
	}
}
