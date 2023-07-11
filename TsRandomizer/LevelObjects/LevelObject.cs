using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Heroes;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.ReplacementObjects;
using TsRandomizer.RoomTriggers;
using TsRandomizer.Screens;
using TsRandomizer.Settings;
#if DEBUG
using Timespinner.GameAbstractions.Inventory;
#endif

namespace TsRandomizer.LevelObjects
{
	abstract class LevelObject<T> : LevelObject where T : Mobile
	{
		public readonly new T TypedObject;

		protected LevelObject(T typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
			TypedObject = typedObject;
		}
	}

	abstract class LevelObject
	{
		static readonly List<LevelObject> Objects = new List<LevelObject>();

		static readonly Dictionary<Type, Type> RegisteredTypes = new Dictionary<Type, Type>(); //ObjectType, EventHandler
		static readonly Dictionary<EEventTileType, AlwaysSpawnAttribute> AlwaysSpawningEventTypes = new Dictionary<EEventTileType, AlwaysSpawnAttribute>(); //EEventTileType, SpawnerMethod

		static readonly Dictionary<Type, List<int>> KnownIds = new Dictionary<Type, List<int>>();

		public readonly dynamic Dynamic;
		public readonly Mobile TypedObject;
		public readonly GameplayScreen GameplayScreen;

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

		protected LevelObject(Mobile typedObject, GameplayScreen gameplayScreen)
		{
			if (typedObject == null)
				return;

			TypedObject = typedObject;
			Dynamic = typedObject.AsDynamic();
			GameplayScreen = gameplayScreen;
		}

		static void OnCollisionDetection()
		{
		}

		public static void Update(
			Level level, GameplayScreen gameplayScreen, ItemLocationMap itemLocations,
			Seed seed, SettingCollection gameSettings,
			ScreenManager screenManager)
		{
			var levelReflected = level.AsDynamic();
			var roomChanged = !IsRoomRandomized(levelReflected);

			if (roomChanged)
				OnChangeRoom(level, levelReflected, itemLocations, seed, gameSettings, screenManager, gameplayScreen);
			else
				itemLocations.Update(level, gameplayScreen);
			
			var newNonItemObjects = ((List<Mobile>)levelReflected._newObjects)
				.Where(o => o.BaseType != EGameObjectBaseType.Item)
				.ToArray();

			if (newNonItemObjects.Any())
				GenerateShadowObjects(itemLocations, newNonItemObjects, seed, gameplayScreen);

			GenerateShadowObjectsForNewObjects<Monster>(levelReflected._enemies, itemLocations, seed, gameplayScreen);
			var hasNewItems = 
				GenerateShadowObjectsForNewObjects<Item>(levelReflected._items, itemLocations, seed, gameplayScreen);

			foreach (var obj in Objects)
				obj.OnUpdate();

			if (roomChanged || hasNewItems) 
				AwardFirstFrameItem(levelReflected._items.Values, level.MainHero);
		}

		static bool IsRoomRandomized(dynamic levelReflected) =>
			((Dictionary<int, GameEvent>)levelReflected._levelEvents).ContainsKey(RandomizerEvent.Id);
		
		public static void AwardFirstFrameItem(IEnumerable<Item> itemDictionary, Protagonist lunais)
		{
			//sometimes lunais picks up an item because she's intersecting with it as the screen loads, or right as it drops.
			//that doesn't give the replacer enough time to replace the item. But she deserves it. You deserve it.
			foreach (var item in itemDictionary)
				if (item.Bbox.Intersects(lunais.Bbox))
					item.GetItem(lunais);
		}

		static void OnChangeRoom(Level level, dynamic levelReflected, ItemLocationMap itemLocations, Seed seed,
			SettingCollection gameSettings, ScreenManager screenManager, GameplayScreen gameplayScreen)
		{
			Dictionary<int, GameEvent> events = levelReflected._levelEvents;

#if DEBUG
			level.GameSave.AddItem(level, new ItemIdentifier(EInventoryRelicType.Dash));
			level.GameSave.AddItem(level, new ItemIdentifier(EInventoryRelicType.EssenceOfSpace));
			level.GameSave.AddItem(level, new ItemIdentifier(EInventoryRelicType.DoubleJump));

			level.GameSave.AddItem(level, new ItemIdentifier(EInventoryEquipmentType.NelisteEarring));
#endif

			Objects.Clear();
			foreach (var knownIds in KnownIds.Values)
				knownIds.Clear();

			IEnumerable<GameEvent> eventObjects = events.Values;
			IEnumerable<Animate> npcs = levelReflected.NPCs.Values;
			IEnumerable<Monster> enemies = levelReflected._enemies.Values;

			SetMonsterHpTo1(levelReflected._enemies.Values);

			var objects = eventObjects
				.Concat(npcs)
				.Concat(enemies)
				.ToArray();

			int levelId = levelReflected._id;
			int roomId = ((RoomSpecification)levelReflected.CurrentRoom).ID;
			var roomKey = new Roomkey(levelId, roomId);

			RoomTrigger.OnChangeRoom(level, seed, gameSettings, itemLocations, screenManager, roomKey);
			TextReplacer.OnChangeRoom(level, seed.Options, itemLocations, levelId, roomId);
			Replaces.ReplaceObjects(level, objects);

			if (gameSettings.EnemyRando.Value != "Off")
				Enemizer.RandomizeEnemies(level, roomKey, gameSettings, enemies, seed);

			GenerateShadowObjects(itemLocations, objects, seed, gameplayScreen);
			SpawnMissingObjects(level, levelReflected, itemLocations, gameplayScreen);

			if (gameSettings.ExtraEarringsXP.Value > 0)
				OrbExperienceManager.ResetHitRegistry();

			_ = new RandomizerEvent(level, events, OnCollisionDetection);
		}

		static bool GenerateShadowObjectsForNewObjects<T>(IDictionary<int, T> dictionary,
			ItemLocationMap itemLocations, Seed seed, GameplayScreen gameplayScreen) where T : Mobile
		{
			if (!KnownIds.TryGetValue(typeof(T), out var knownIds))
			{
				knownIds = new List<int>();

				KnownIds[typeof(T)] = knownIds;
			}

			var ids = dictionary.Keys;
			var newObjects = ids
				.Except(knownIds)
				.Select(i => dictionary[i])
				.ToArray();
			if (newObjects.Any())
				GenerateShadowObjects(itemLocations, newObjects, seed, gameplayScreen);

			knownIds.Clear();
			knownIds.AddRange(ids);

			return newObjects.Any();
		}

		public static void GenerateShadowObjects(
			ItemLocationMap itemLocations, Mobile[] objects, Seed seed, GameplayScreen gameplayScreen)
		{
			var objectsPerTypes = objects.GroupBy(o => o.GetType());

			foreach (var objectsPerType in objectsPerTypes)
			{
				if (!RegisteredTypes.TryGetValue(objectsPerType.Key, out var levelObjectType)) continue;

				foreach (var obj in objectsPerType)
				{
					LevelObject levelObject;

					if (typeof(ItemManipulator).IsAssignableFrom(levelObjectType))
						levelObject = ItemManipulator.GenerateShadowObject(levelObjectType, obj, gameplayScreen, itemLocations);
					else
						levelObject = (LevelObject)Activator.CreateInstance(levelObjectType, obj, gameplayScreen);

					if (levelObject == null)
						continue;

					Objects.Add(levelObject);
					levelObject.Initialize(seed);
				}
			}

			SetMonsterHpTo1(objects.OfType<Alive>());
		}

		static void SpawnMissingObjects(Level level, dynamic levelPrivate, ItemLocationMap itemLocations, GameplayScreen gameplayScreen)
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
						var instance = (ICustomSpwanMethod)Activator.CreateInstance(objectType, null, gameplayScreen, itemInfo);
						mobile = instance.Spawn(level, specification);
					}
					else
					{
						var point = new Point(specification.X * 16 + 8, specification.Y * 16 + 16);
						mobile = (GameEvent)Activator.CreateInstance(timeSpinnerType, level, point, -1, specification);
					}

					if (mobile is GameEvent gameEvent)
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
			foreach (var monster in monsters)
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

		static void SetLevelTileSheet(dynamic levelReflected, ETilesetType tileSet)
		{
			var spriteSheet = ((GCM)levelReflected.GCM).GetTileset(tileSet);
			levelReflected.CurrentTileset = spriteSheet;

			foreach (Tile tile in levelReflected._solidTiles.Values)
			{
				var tileReflected = tile.AsDynamic();
				tileReflected._sprite = spriteSheet;

				if (!tileReflected._isInvisibleSolidTile)
					tileReflected._drawSource = tileReflected._sprite.GetFrameSource(tileReflected._tileIndex);
			}
		}

		protected virtual void Initialize(Seed seed)
		{
		}

		protected virtual void OnUpdate()
		{
		}
	}
}