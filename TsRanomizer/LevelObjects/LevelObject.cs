using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

		protected LevelObject(T typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
		{
			TypedObject = typedObject;
		}
	}

	abstract class LevelObject
	{
		protected static readonly Dictionary<Type, Type> RegisteredTypes = new Dictionary<Type, Type>(); //ObjectType, EventHandler
		protected static readonly Dictionary<EEventTileType, Type> AlwaysSpawningTypes = new Dictionary<EEventTileType, Type>(); //EEventTileType, ObjectType
		protected static readonly List<LevelObject> Objects = new List<LevelObject>();

		public readonly ItemInfo ItemInfo;
		public readonly dynamic Reflected;
		public readonly Mobile Object;

		static LevelObject()
		{
			var levelObjectType = MethodBase.GetCurrentMethod().DeclaringType;

			// ReSharper disable once PossibleNullReferenceException
			var dirievedTypes = levelObjectType.Assembly.GetTypes()
				.Where(t => levelObjectType.IsAssignableFrom(t)
						 && !t.IsGenericType
						 && t != levelObjectType);

			foreach (var dirivedType in dirievedTypes)
			{
				var gameEventTypes = dirivedType
					.GetCustomAttributes(typeof(TimeSpinnerType), true)
					.Cast<TimeSpinnerType>()
					.Select(a => a.Type)
					.ToList();

				if (!gameEventTypes.Any())
					// ReSharper disable once PossibleNullReferenceException
					gameEventTypes.Add(dirivedType.BaseType.GetGenericArguments()[0]);

				foreach (var supportedGameEventType in gameEventTypes)
				{
					RegisteredTypes.Add(supportedGameEventType, dirivedType);

					var alwaysSpawnAttribute = (AlwaysSpawnAttribute)dirivedType
						.GetCustomAttributes(typeof(AlwaysSpawnAttribute), true)
						.FirstOrDefault();

					if(alwaysSpawnAttribute == null)
						continue;

					AlwaysSpawningTypes.Add(alwaysSpawnAttribute.EventType, supportedGameEventType);
				}
			}
		}

		protected LevelObject(Mobile typedObject, ItemInfo itemInfo)
		{
			ItemInfo = itemInfo;
			Reflected = typedObject.Reflect();
			Object = typedObject;

			GameSave gameSave = ((Level)Reflected._level).GameSave; //TODO Remove lolz
			gameSave.AddItem(ItemInfo.Get(EInventoryRelicType.Dash));
			gameSave.AddItem(ItemInfo.Get(EInventoryRelicType.DoubleJump));
		}

		public static void UpdateAll()
		{
			foreach (var obj in Objects)
				obj.OnUpdate();
		}

		public static void DrawAll(
			SpriteBatch spriteBatch, SpriteFont menuFont, Vector2 levelRenderCenter, 
			ItemLocationMap itemLocations
		)
		{
			//TODO Remove lolz
			for (var i = 0; i < Objects.Count; i++)
			{
				var obj = Objects[i];
				var drawKeyPos = new Vector2(30, 160 + 60 * i);
				var drawRequirementPos = new Vector2(30, 160 + (60 * i) + 24);
				var key = GetKey(obj.Object);
				var requirement = itemLocations.GetItemGate(key);
				var color = obj.ItemInfo != null
					? obj.ItemInfo != ItemInfo.Dummy
						? Color.Green
						: Color.DarkGreen
					: Color.Red;

				spriteBatch.DrawString(menuFont, $"{key}", drawKeyPos, color, 2);
				spriteBatch.DrawString(menuFont, $"Requirement: {requirement}", drawRequirementPos, color, 1.5f);
			}
		}

		public static void OnChangeRoom(ItemLocationMap itemLocations, Level level)
		{
			Console.Out.WriteLine("OnChangeRoom"); //TODO Remove lolz

			Objects.Clear();

			var levelPrivate = level.Reflect();

			IEnumerable<Animate> eventObjects = levelPrivate._levelEvents.Values;
			IEnumerable<Animate> itemObjects = levelPrivate._items.Values;
			IEnumerable<Animate> npcs = levelPrivate.NPCs.Values;
			IEnumerable<Alive> monsters = levelPrivate._enemies.Values;

			foreach (Alive monster in monsters) //TODO Remove lolz
				monster.MaxHP = 1;

			var objects = eventObjects
				.Concat(itemObjects)
				.Concat(npcs)
				.ToList();

			Replaces.ReplaceObjects(level, objects);
			GenerateShadowObjects(itemLocations, objects);
			SpawnMissingObjects(level, levelPrivate);
		}

		public static void GenerateShadowObjects(ItemLocationMap itemLocations, IEnumerable<Mobile> objects)
		{
			var objectsPerTypes = objects.GroupBy(o => o.GetType());

			foreach (var objectsPerType in objectsPerTypes)
			{
				if (!RegisteredTypes.TryGetValue(objectsPerType.Key, out Type levelObjectType)) continue;

				foreach (var obj in objectsPerType)
				{
					var itemKey = GetKey(obj);
					var itemInfo = itemLocations.GetItemInfo(itemKey);
					var levelObject = (LevelObject)Activator.CreateInstance(levelObjectType, obj, itemInfo);

					Objects.Add(levelObject);

					levelObject.Initialize();

					if (itemInfo == null)
						Console.Out.WriteLine($"UnmappedItem: {itemKey}");
				}
			}
		}

		static void SpawnMissingObjects(Level level, dynamic levelPrivate)
		{
			var newObjects = new List<GameEvent>();

			foreach (var alwaysSpawn in AlwaysSpawningTypes)
			{
				var eventTiles = ((RoomSpecification)levelPrivate.CurrentRoom).ObjectTiles
					.Values.SelectMany(list => list.ToArray())
					.Where(t => t.Category == EObjectTileCategory.Event
					            && (EEventTileType)t.ObjectID == alwaysSpawn.Key)
					.ToArray();

				if (!eventTiles.Any() || Objects.Any(o => o.GetType() == alwaysSpawn.Value))
					continue;

				foreach (var specification in eventTiles)
				{
					var point = new Point(specification.X * 16 + 8, specification.Y * 16 + 16);
					var gameEvent = (GameEvent)Activator.CreateInstance(alwaysSpawn.Value, level, point, -1, specification);
					newObjects.Add(gameEvent);
				}
			}

			foreach (var gameEvent in newObjects)
				levelPrivate.RequestAddObject(gameEvent);
		}

		static ItemKey GetKey(Mobile obj)
		{
			var objectPrivate = obj.Reflect();
			var level = (Level)objectPrivate._level;
			var levelPrivate = level.Reflect();
			var position = (Point)objectPrivate._position;
			var currentRoom = (RoomSpecification)levelPrivate.CurrentRoom;
			return new ItemKey(levelPrivate._id, currentRoom.ID, position.X, position.Y);
		}

		protected virtual void Initialize()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected void AwardContainedItem(Level level = null)
		{
			level = level ?? (Level)Reflected._level;

			level.GameSave.AddItem(ItemInfo);
			ItemInfo.OnPickup(level);
		}
	}
}