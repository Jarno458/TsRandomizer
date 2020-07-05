using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Timespinner.Core;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation;
using TsRanodmizer.ReplacementObjects;

namespace TsRanodmizer.LevelObjects
{
	abstract class LevelObject<T> : LevelObject where T : Mobile
	{
		public readonly T TypedObject;

		protected LevelObject(T typedObject, ItemInfo itemInfo) : base(typedObject)
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

		public readonly dynamic Object;

		public Level Level => (Level)Object?._level;
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
				// ReSharper disable once PossibleNullReferenceException
				var correspondingTimeSpinnerType = typeDirivedFromLevelObject
					.GetCustomAttributes(typeof(TimeSpinnerType), true)
					.Cast<TimeSpinnerType>()
					.Select(a => a.Type)
					.FirstOrDefault() ?? typeDirivedFromLevelObject.BaseType.GetGenericArguments()[0];

				RegisteredTypes.Add(correspondingTimeSpinnerType, typeDirivedFromLevelObject);

				var alwaysSpawnAttribute = (AlwaysSpawnAttribute)typeDirivedFromLevelObject
					.GetCustomAttributes(typeof(AlwaysSpawnAttribute), true)
					.FirstOrDefault();

				if (alwaysSpawnAttribute == null)
					continue;

				alwaysSpawnAttribute.ObjectType = typeDirivedFromLevelObject;
				alwaysSpawnAttribute.TimeSpinnerObjectType = correspondingTimeSpinnerType;

				AlwaysSpawningEventTypes.Add(alwaysSpawnAttribute.EventType, alwaysSpawnAttribute);
			}
		}

		protected LevelObject(Mobile typedObject)
		{
			if (typedObject == null)
				return;

			Object = typedObject.AsDynamic();
		}

		public static void Update(Level level, ItemLocationMap itemLocations, bool roomChanged)
		{
			if (roomChanged)
				OnChangeRoom(level, itemLocations);

			var levelReflected = level.AsDynamic();
			var newObjects = (List<Mobile>)levelReflected._newObjects;
			if (newObjects.Any())
			{
				GenerateShadowObjects(level.GameSave, itemLocations, newObjects);

				SetMonsterHpTo1(newObjects.OfType<Alive>());
			}

			var itemsDictionary = (Dictionary<int, Item>)levelReflected._items;
			var currentItemIds = itemsDictionary.Keys;
			var newItems = currentItemIds
				.Except(KnownItemIds)
				.Select(i => itemsDictionary[i])
				.ToArray();

			if (newItems.Any())
				GenerateShadowObjects(level.GameSave, itemLocations, newItems);

			KnownItemIds.Clear();
			KnownItemIds.AddRange(currentItemIds);

			foreach (var obj in Objects)
				obj.OnUpdate();
		}

		static void OnChangeRoom(Level level, ItemLocationMap itemLocations)
		{
#if DEBUG
			Console.Out.WriteLine("OnChangeRoom");

			//TODO Remove LOLZ
			level.GameSave.AddItem(ItemInfo.Get(EInventoryRelicType.Dash));
			level.GameSave.AddItem(ItemInfo.Get(EInventoryRelicType.EssenceOfSpace));
			level.GameSave.AddItem(ItemInfo.Get(EInventoryOrbType.Blade, EOrbSlot.Melee));
			level.GameSave.AddItem(ItemInfo.Get(EInventoryOrbType.Blood, EOrbSlot.Melee));
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

			RoomTrigger.OnChangeRoom(level, itemLocations, levelReflected._id, ((RoomSpecification)levelReflected.CurrentRoom).ID);
			Replaces.ReplaceObjects(level, objects);
			GenerateShadowObjects(level.GameSave, itemLocations, objects);
			SpawnMissingObjects(level, levelReflected);
		}

		public static void GenerateShadowObjects(GameSave gameSave, ItemLocationMap itemLocations, IEnumerable<Mobile> objects)
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
					levelObject.Initialize();
				}
			}
		}

		static void SpawnMissingObjects(Level level, dynamic levelPrivate)
		{
			var newObjects = new List<GameEvent>();

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
					GameEvent gameEvent;

					if (typeof(ICustomSpwanMethod).IsAssignableFrom(objectType))
					{
						var instance = (ICustomSpwanMethod)Activator.CreateInstance(objectType, new object[] { null, null });
						gameEvent = instance.Spawn(level, specification);
					}
					else
					{
						var point = new Point(specification.X * 16 + 8, specification.Y * 16 + 16);
						gameEvent = (GameEvent)Activator.CreateInstance(timeSpinnerType, level, point, -1, specification);
					}

					gameEvent.Initialize();

					newObjects.Add(gameEvent);
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

		protected virtual void Initialize()
		{
		}

		protected virtual void OnUpdate()
		{
		}
	}
}