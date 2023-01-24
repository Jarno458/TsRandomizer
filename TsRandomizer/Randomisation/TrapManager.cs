using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
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
			ETrapType[] validTraps = GetValidTraps(level);
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
		static ETrapType[] GetValidTraps(Level level)
		{
			SettingCollection gameSettings = level.GameSave.GetSettings();
			List<ETrapType> validTraps = new List<ETrapType>();
			if (gameSettings.SparrowTrap.Value)
				validTraps.Add(ETrapType.MeteorSparrow);
			if (gameSettings.NeurotoxinTrap.Value)
				validTraps.Add(ETrapType.Neurotoxin);
			if (gameSettings.ChaosTrap.Value)
				validTraps.Add(ETrapType.Chaos);
			if (gameSettings.PoisonTrap.Value)
				validTraps.Add(ETrapType.Poison);

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
			ApplyStatus(level, "NeuroToxin");
		}

		static void ChaosTrap(Level level)
		{
			ApplyStatus(level, "Chaos");
		}

		static void PoisonTrap(Level level)
		{
			ApplyStatus(level, "Poison");
		}

		static void ApplyStatus(Level level, string status)
		{
			var statusEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.StatusEffects+EStatuseffectType");
			level.MainHero.AsDynamic().GiveStatusEffect(statusEnumType.GetEnumValue(status), 100);
		}
		
	}
}
