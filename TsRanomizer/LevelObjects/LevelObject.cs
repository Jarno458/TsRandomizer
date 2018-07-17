using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
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
		protected static readonly List<LevelObject> Objects = new List<LevelObject>();

		public readonly ItemInfo ItemInfo;
		public readonly dynamic ObjectPrivate;
		public readonly Mobile Object;

		static LevelObject()
		{
			var levelObjectType = MethodBase.GetCurrentMethod().DeclaringType
								  ?? throw new TypeLoadException("Cannot load Type LevelObject");

			var dirievedTypes = levelObjectType.Assembly.GetTypes()
				.Where(t => levelObjectType.IsAssignableFrom(t)
						 && !t.IsGenericType
						 && t != levelObjectType);

			foreach (var dirivedType in dirievedTypes)
			{
				var supportedGameEventTypes = dirivedType
					.GetCustomAttributes(typeof(TimeSpinnerType), true)
					.Cast<TimeSpinnerType>()
					.Select(a => a.Type)
					.ToArray();

				if (!supportedGameEventTypes.Any())
					// ReSharper disable once PossibleNullReferenceException
					RegisteredTypes.Add(dirivedType.BaseType.GetGenericArguments()[0], dirivedType);
				else
					foreach (var supportedGameEventType in supportedGameEventTypes)
						RegisteredTypes.Add(supportedGameEventType, dirivedType);
			}
		}

		protected LevelObject(Mobile typedObject, ItemInfo itemInfo)
		{
			ItemInfo = itemInfo;
			ObjectPrivate = typedObject.Reflect();
			Object = typedObject;
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
			for (var i = 0; i < Objects.Count; i++)
			{
				var obj = Objects[i];
				var drawPos = new Vector2(30, 160 + 30 * i);
				var key = GetKey(obj.Object);
				var requirement = itemLocations.GetItemRequirements(key);
				var color = obj.ItemInfo != null
					? obj.ItemInfo != ItemInfo.Dummy
						? Color.DarkGreen
						: Color.Orange
					: Color.Red;

				var text = $"{key}, Requirement: {requirement}";
				spriteBatch.DrawString(menuFont, text, drawPos, color, 2);
			}
		}

		public static void OnChangeRoom(ItemLocationMap itemLocations, Level level)
		{
			Objects.Clear();

			var levelPrivate = level.Reflect();
			IEnumerable<Animate> eventObjects = levelPrivate._levelEvents.Values;
			IEnumerable<Animate> itemObjects = levelPrivate._items.Values;

			var objects = eventObjects.Concat(itemObjects).ToList();

			Replaces.ReplaceObjects(level, objects);

			RandomiseObjects(itemLocations, objects);
		}

		public static void RandomiseObjects(ItemLocationMap itemLocations, IEnumerable<Mobile> objects)
		{
			var objectsPerTypes = objects.GroupBy(o => o.GetType());

			foreach (var objectsPerType in objectsPerTypes)
			{
				if (!RegisteredTypes.TryGetValue(objectsPerType.Key, out Type levelObjectType)) continue;

				foreach (var obj in objectsPerType)
				{
					var itemKey = GetKey(obj);
					var itemInfo = itemLocations.GetItemInfo(itemKey);
					var levelObject =
						(LevelObject)Activator.CreateInstance(levelObjectType, obj, itemInfo);

					Objects.Add(levelObject);

					levelObject.OnChangeRoom();
				}
			}
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

		protected virtual void OnChangeRoom()
		{
		}

		protected virtual void OnUpdate()
		{
		}
	}
}