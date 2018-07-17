using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.ReplacementObjects
{
	abstract class Replaces<T> : Replaces where T : Animate
	{
		protected override Animate Replace(Level level, Animate objectToReplace)
		{
			return Replace(level, (T)objectToReplace);
		}

		protected abstract T Replace(Level level, T objectToReplace);
	}

	abstract class Replaces
	{
		protected static readonly Dictionary<Type, Type> RegisteredTypes = new Dictionary<Type, Type>(); //TypeToReplace, ReplacerType

		static Replaces()
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

		public static void ReplaceObjects(Level level, List<Animate> objects)
		{
			var newObjects = new List<Animate>();
			var objectsPerTypes = objects.GroupBy(o => o.GetType());

			foreach (var objectsPerType in objectsPerTypes)
			{
				if (!RegisteredTypes.TryGetValue(objectsPerType.Key, out Type replacerType)) continue;

				var replacer = (Replaces)Activator.CreateInstance(replacerType);

				foreach (var obj in objectsPerType)
				{
					var newObject = replacer.Replace(level, obj);

					obj.SilentKill();
					newObjects.Add(newObject);
				}
			}

			((List<Mobile>)level.Reflect()._newObjects).AddRange(newObjects);
			objects.AddRange(newObjects);
		}


		protected abstract Animate Replace(Level level, Animate objectToReplace);
	}
}