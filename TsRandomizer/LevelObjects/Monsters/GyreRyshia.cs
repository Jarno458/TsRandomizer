﻿using System;
using System.Collections.Generic;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.GyreRyshia")]
	class GyreRyshia : LevelObject<Monster>
	{
		dynamic summonZone;

		bool shouldScale;

		readonly int damage;
		readonly int maxHP;
		readonly int hp;
		readonly int experience;

		Dictionary<int, Monster> enemies;
		readonly HashSet<Monster> scaledEnemyIds = new HashSet<Monster>();

		public GyreRyshia(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
			summonZone = ((GameEvent)Dynamic._summoningZone).AsDynamic();
			enemies = (Dictionary<int, Monster>)LevelReflected._enemies;

			damage = TypedObject.Damage;
			maxHP = typedObject.MaxHP;
			hp = typedObject.HP;
			experience = typedObject.ExperienceGiven;
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			summonZone._randomizer = new Random(
				(int)seed.Id 
				+ (Level.ID * LevelReflected.CurrentRoom.ID * 100)
				+ TypedObject.ID);

			shouldScale = settings.EnemyRando.Value == "Scaled" || settings.EnemyRando.Value == "Ryshia";
		}

		//scale her summons
		protected override void OnUpdate()
		{
			if (!shouldScale)
				return;

			if (scaledEnemyIds.Count == 0)
			{
				scaledEnemyIds.UnionWith(enemies.Values);
			}
			else
			{
				foreach (var enemy in enemies.Values)
				{
					if (scaledEnemyIds.Contains(enemy))
						continue;

					enemy.ScaleTo(damage, maxHP, hp, experience);

					scaledEnemyIds.Add(enemy);
				}
			}
		}
	}
}
