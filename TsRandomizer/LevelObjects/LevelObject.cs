﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Timespinner.Core;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Heroes;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.ReplacementObjects;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects
{
	abstract class LevelObject<T> : LevelObject where T : Mobile
	{
		public readonly T TypedObject;

		protected LevelObject(T typedObject) : base(typedObject)
		{
			TypedObject = typedObject;
		}
	}

	abstract class LevelObject
	{
		protected static readonly List<LevelObject> Objects = new List<LevelObject>();

		static readonly Dictionary<Type, Type> RegisteredTypes = new Dictionary<Type, Type>(); //ObjectType, EventHandler
		static readonly Dictionary<EEventTileType, AlwaysSpawnAttribute> AlwaysSpawningEventTypes = new Dictionary<EEventTileType, AlwaysSpawnAttribute>(); //EEventTileType, SpawnerMethod
		static readonly List<int> KnownItemIds = new List<int>();
		
		public readonly dynamic Dynamic;

		public Level Level => (Level)Dynamic?._level;
		public dynamic LevelReflected => Level.AsDynamic();
		public Queue<ScriptAction> Scripts => (Queue<ScriptAction>)LevelReflected._waitingScripts;

		static LevelObject()
		{
			var levelObjectType = MethodBase.GetCurrentMethod().DeclaringType;

			// ReSharper disable once PossibleNullReferenceException
			var typesDirivedFromLevelObject = levelObjectType.Assembly.GetTypes()
				.Where(t => levelObjectType.IsAssignableFrom(t)
						 && !t.IsGenericType
						 && t != levelObjectType
						 && t != typeof(ItemManipulator));

			foreach (var typeDirivedFromLevelObject in typesDirivedFromLevelObject)
			{
				var timeSpinnerTypes = typeDirivedFromLevelObject
					.GetCustomAttributes(typeof(TimeSpinnerType), true)
					.Cast<TimeSpinnerType>()
					.Select(a => a.Type)
					.ToList();

				if (timeSpinnerTypes.Count > 0)
					foreach (var timeSpinnerType in timeSpinnerTypes)
						RegisteredTypes.Add(timeSpinnerType, typeDirivedFromLevelObject);
				else
					// ReSharper disable once PossibleNullReferenceException
					RegisteredTypes.Add(typeDirivedFromLevelObject.BaseType.GetGenericArguments()[0], typeDirivedFromLevelObject);

				var alwaysSpawnAttribute = (AlwaysSpawnAttribute)typeDirivedFromLevelObject
					.GetCustomAttributes(typeof(AlwaysSpawnAttribute), true)
					.FirstOrDefault();

				if (alwaysSpawnAttribute == null)
					continue;

				alwaysSpawnAttribute.ObjectType = typeDirivedFromLevelObject;
				alwaysSpawnAttribute.TimeSpinnerObjectType = timeSpinnerTypes.First();

				AlwaysSpawningEventTypes.Add(alwaysSpawnAttribute.EventType, alwaysSpawnAttribute);
			}
		}

		protected LevelObject(Mobile typedObject)
		{
			if (typedObject == null)
				return;

			Dynamic = typedObject.AsDynamic();
		}

		public static void AwardFirstFrameItem(Dictionary<int, Item> itemDictionary, Protagonist lunais)
        {
            //sometimes lunais picks up an item because she's intersecting with it as the screen loads, or right as it drops.
            //that doesn't give the replacer enough time to replace the item. But she deserves it. You deserve it.
            foreach (var item in itemDictionary)
            {
				if (item.Value.Bbox.Intersects(lunais.Bbox)) item.Value.GetItem(lunais);				
            }
        }

		public static void Update(
			Level level, GameplayScreen gameplayScreen, ItemLocationMap itemLocations,
			bool roomChanged, SeedOptions seedOptions, ScreenManager screenManager)
		{
			if (roomChanged)
				OnChangeRoom(level, itemLocations, seedOptions, screenManager);
			else
				itemLocations.Update(level);

			var levelReflected = level.AsDynamic();
			var newNonItemObjects = ((List<Mobile>)levelReflected._newObjects)
				.Where(o => o.BaseType != EGameObjectBaseType.Item)
				.ToArray();

			if (newNonItemObjects.Any())
			{
				GenerateShadowObjects(itemLocations, newNonItemObjects, seedOptions);

				SetMonsterHpTo1(newNonItemObjects.OfType<Alive>());
			}

			var itemsDictionary = (Dictionary<int, Item>)levelReflected._items;
				
			var currentItemIds = itemsDictionary.Keys;
			var newItems = currentItemIds
				.Except(KnownItemIds)
				.Select(i => itemsDictionary[i])
				.ToArray();

			if (newItems.Any())
				GenerateShadowObjects(itemLocations, newItems, seedOptions);

			if (roomChanged || newItems.Any()) AwardFirstFrameItem(itemsDictionary, level.MainHero);

			KnownItemIds.Clear();
			KnownItemIds.AddRange(currentItemIds);

			foreach (var obj in Objects)
				obj.OnUpdate(gameplayScreen);
		}

		static void OnChangeRoom(Level level, ItemLocationMap itemLocations, SeedOptions seedOptions, ScreenManager screenManager)
		{
#if DEBUG
			level.GameSave.AddItem(level, new ItemIdentifier(EInventoryRelicType.Dash));
			level.GameSave.AddItem(level, new ItemIdentifier(EInventoryRelicType.EssenceOfSpace));
#endif

			var levelReflected = level.AsDynamic();

			Objects.Clear();
			KnownItemIds.Clear();

			IEnumerable<Animate> eventObjects = levelReflected._levelEvents.Values;
			IEnumerable<Animate> npcs = levelReflected.NPCs.Values;
			IEnumerable<Animate> enemies = levelReflected._enemies.Values;

			SetMonsterHpTo1(levelReflected._enemies.Values);
			
			var objects = eventObjects
				.Concat(npcs)
				.Concat(enemies)
				.ToList();

			RoomTrigger.OnChangeRoom(
				level, seedOptions, itemLocations, screenManager,
				levelReflected._id, ((RoomSpecification)levelReflected.CurrentRoom).ID);
			Replaces.ReplaceObjects(level, objects);
 			GenerateShadowObjects(itemLocations, objects, seedOptions);
			SpawnMissingObjects(level, levelReflected, itemLocations);
		}

		public static void GenerateShadowObjects(ItemLocationMap itemLocations, IEnumerable<Mobile> objects, SeedOptions options)
		{
			var objectsPerTypes = objects.GroupBy(o => o.GetType());

			foreach (var objectsPerType in objectsPerTypes)
			{
				if (!RegisteredTypes.TryGetValue(objectsPerType.Key, out Type levelObjectType)) continue;

				foreach (var obj in objectsPerType)
				{
					LevelObject levelObject;

					if (typeof(ItemManipulator).IsAssignableFrom(levelObjectType))
						levelObject = ItemManipulator.GenerateShadowObject(levelObjectType, obj, itemLocations);
					else
						levelObject = (LevelObject)Activator.CreateInstance(levelObjectType, obj);

					if (levelObject == null)
						continue;

					Objects.Add(levelObject);
					levelObject.Initialize(options);
				}
			}
		}

		static void SpawnMissingObjects(Level level, dynamic levelPrivate, ItemLocationMap itemLocations)
		{
			var newObjects = new List<Mobile>();

			foreach (var alwaysSpawningEventType in AlwaysSpawningEventTypes)
			{
				var eventTileType = alwaysSpawningEventType.Key;
				var argument = alwaysSpawningEventType.Value.Argument;
				var ignoreArgument = alwaysSpawningEventType.Value.IgnoreArgument;
				var objectType = alwaysSpawningEventType.Value.ObjectType;
				var timeSpinnerType = alwaysSpawningEventType.Value.TimeSpinnerObjectType;

				var eventTilesOfEventType = ((RoomSpecification)levelPrivate.CurrentRoom).ObjectTiles
					.Values.SelectMany(list => list.ToArray())
					.Where(t => t.Category == EObjectTileCategory.Event && (EEventTileType)t.ObjectID == eventTileType)
					.Where(t => ignoreArgument || (!t.DoesHaveArgument && argument == 0) || t.Argument == argument)
					.ToArray();

				if (!eventTilesOfEventType.Any() || Objects.Any(o => o.GetType() == objectType))
					continue;

				foreach (var specification in eventTilesOfEventType)
				{
					Mobile mobile;
					var itemPosition = new Point(specification.X * 16 + 8, specification.Y * 16 + 16);
					var currentRoom = (RoomSpecification)levelPrivate.CurrentRoom;
					var itemInfo = itemLocations[new ItemKey(levelPrivate._id, currentRoom.ID, itemPosition.X, itemPosition.Y)];

					if (typeof(ICustomSpwanMethod).IsAssignableFrom(objectType))
					{
						var instance = (ICustomSpwanMethod)Activator.CreateInstance(objectType, null, itemInfo);
						mobile = instance.Spawn(level, specification);
					}
					else
					{
						var point = new Point(specification.X * 16 + 8, specification.Y * 16 + 16);
						mobile = (GameEvent)Activator.CreateInstance(timeSpinnerType, level, point, -1, specification);
					}

					if(mobile is GameEvent gameEvent)
						gameEvent.Initialize();

					newObjects.Add(mobile);
				}
			}

			foreach (var gameEvent in newObjects)
				levelPrivate.RequestAddObject(gameEvent);
		}

		static void SetMonsterHpTo1(IEnumerable<Alive> monsters)
		{
#if DEBUG
			foreach (Alive monster in monsters)
				monster.MaxHP = 1;
#endif
		}

		protected static ItemKey GetKey(Mobile obj)
		{
			var objectPrivate = obj.AsDynamic();
			var level = (Level)objectPrivate._level;
			var levelPrivate = level.AsDynamic();
			var position = (Point)objectPrivate._position;
			var currentRoom = (RoomSpecification)levelPrivate.CurrentRoom;
			return new ItemKey(levelPrivate._id, currentRoom.ID, position.X, position.Y);
		}

		static void SetLevelTileSheet(dynamic levelReflected, SpriteSheet spriteSheet)
		{
			levelReflected.CurrentTileset = spriteSheet;

			foreach (Tile tile in levelReflected._solidTiles.Values)
			{
				var tileReflected = tile.AsDynamic();
				tileReflected._sprite = spriteSheet;

				if (!tileReflected._isInvisibleSolidTile)
					tileReflected._drawSource = tileReflected._sprite.GetFrameSource(tileReflected._tileIndex);
			}
		}

		protected virtual void Initialize(SeedOptions options)
		{
		}

		protected virtual void OnUpdate(GameplayScreen gameplayScreen)
		{
		}
	}
}