using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Timespinner.Core;
using Timespinner.Core.Specifications;
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
		static readonly List<int> KnownItemIds = new List<int>();

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

		public static void AwardFirstFrameItem(Dictionary<int, Item> itemDictionary, Protagonist lunais)
		{
			//sometimes lunais picks up an item because she's intersecting with it as the screen loads, or right as it drops.
			//that doesn't give the replacer enough time to replace the item. But she deserves it. You deserve it.
			foreach (var item in itemDictionary)
			{
				if (item.Value.Bbox.Intersects(lunais.Bbox)) item.Value.GetItem(lunais);
			}
		}

		static void OnCollisionDetection()
		{
		}

		public static void Update(
			Level level, GameplayScreen gameplayScreen, ItemLocationMap itemLocations,
			bool roomChanged, Seed seed, SettingCollection gameSettings,
			ScreenManager screenManager)
		{
			if (roomChanged)
				OnChangeRoom(level, itemLocations, seed, gameSettings, screenManager, gameplayScreen);
			else
				itemLocations.Update(level, gameplayScreen);

			var levelReflected = level.AsDynamic();
			var newNonItemObjects = ((List<Mobile>)levelReflected._newObjects)
				.Where(o => o.BaseType != EGameObjectBaseType.Item)
				.ToArray();

			if (newNonItemObjects.Any())
			{
				GenerateShadowObjects(itemLocations, newNonItemObjects, seed, gameplayScreen);

				SetMonsterHpTo1(newNonItemObjects.OfType<Alive>());
			}

			var itemsDictionary = (Dictionary<int, Item>)levelReflected._items;
			var currentItemIds = itemsDictionary.Keys;
			var newItems = currentItemIds
				.Except(KnownItemIds)
				.Select(i => itemsDictionary[i])
				.ToArray();
			if (newItems.Any())
				GenerateShadowObjects(itemLocations, newItems, seed, gameplayScreen);

			KnownItemIds.Clear();
			KnownItemIds.AddRange(currentItemIds);

			foreach (var obj in Objects)
				obj.OnUpdate();

			var lunais = level.MainHero;
			if (roomChanged || newItems.Any()) AwardFirstFrameItem(itemsDictionary, lunais);

			int hpCap = Convert.ToInt32(gameSettings.HpCap.Value);
			lunais.MaxHP = hpCap > lunais.MaxHP ? lunais.MaxHP : hpCap;

			if (gameSettings.ExtraEarringsXP.Value > 0)
			{
				OrbExperienceManager.UpdateHitRegistry(lunais);
				OrbExperienceManager.UpdateOrbXp(level, lunais, gameSettings.ExtraEarringsXP.Value);
			}

			if (gameSettings.DamageRando.Value != "Off")
				OrbDamageManager.UpdateOrbDamage(level.GameSave, level.MainHero);

			if (lunais.CurrentState == EAFSM.Skydashing && lunais.Velocity.Y == 0 && lunais.AsDynamic()._isHittingHeadOnCeiling)
				level.GameSave.AddConcussion();
		}

		static void OnChangeRoom(Level level, ItemLocationMap itemLocations, Seed seed,
			SettingCollection gameSettings, ScreenManager screenManager, GameplayScreen gameplayScreen)
		{
#if DEBUG
			level.GameSave.AddItem(level, new ItemIdentifier(EInventoryRelicType.Dash));
			level.GameSave.AddItem(level, new ItemIdentifier(EInventoryRelicType.EssenceOfSpace));
			level.GameSave.AddItem(level, new ItemIdentifier(EInventoryRelicType.DoubleJump));
#endif

			var levelReflected = level.AsDynamic();

			Objects.Clear();
			KnownItemIds.Clear();

			IEnumerable<GameEvent> eventObjects = levelReflected._levelEvents.Values;
			IEnumerable<Animate> npcs = levelReflected.NPCs.Values;
			IEnumerable<Animate> enemies = levelReflected._enemies.Values;

			SetMonsterHpTo1(levelReflected._enemies.Values);

			var objects = eventObjects
				.Concat(npcs)
				.Concat(enemies)
				.ToList();

			RoomTrigger.OnChangeRoom(
				level, seed, gameSettings, itemLocations, screenManager,
				levelReflected._id, ((RoomSpecification)levelReflected.CurrentRoom).ID);
			TextReplacer.OnChangeRoom(level, seed.Options, itemLocations,
				levelReflected._id, ((RoomSpecification)levelReflected.CurrentRoom).ID);
			Replaces.ReplaceObjects(level, objects);
			GenerateShadowObjects(itemLocations, objects, seed, gameplayScreen);
			SpawnMissingObjects(level, levelReflected, itemLocations, gameplayScreen);

			if (gameSettings.ExtraEarringsXP.Value > 0)
			{
				OrbExperienceManager.ResetHitRegistry();
			}
			level.AddEvent(new CollisionDetectionEvent(level, OnCollisionDetection));
		}

		public static void GenerateShadowObjects(
			ItemLocationMap itemLocations, IEnumerable<Mobile> objects, Seed seed, GameplayScreen gameplayScreen)
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

		protected virtual void Initialize(Seed seed)
		{
		}

		protected virtual void OnUpdate()
		{
		}
	}
}