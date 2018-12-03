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
		protected override IEnumerable<Animate> Replace(Level level, Animate objectToReplace)
		{
			return Replace(level, (T)objectToReplace);
		}

		protected abstract IEnumerable<T> Replace(Level level, T objectToReplace);
	}

	abstract class Replaces
	{
		protected static readonly Dictionary<Type, Type> RegisteredTypes = new Dictionary<Type, Type>(); //TypeToReplace, ReplacerType

		static Replaces()
		{
			var replaces = MethodBase.GetCurrentMethod().DeclaringType
			                      ?? throw new TypeLoadException("Cannot load Type LevelObject");

			var dirievedTypes = replaces.Assembly.GetTypes()
				.Where(t => replaces.IsAssignableFrom(t)
				            && !t.IsGenericType
				            && t != replaces);

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
			var levelReflected = level.Reflect();
			var objectsPerTypes = objects.GroupBy(o => o.GetType());

			foreach (var objectsPerType in objectsPerTypes)
			{
				if (!RegisteredTypes.TryGetValue(objectsPerType.Key, out Type replacerType)) continue;

				var replacer = (Replaces)Activator.CreateInstance(replacerType);

				foreach (var obj in objectsPerType)
				{
					var newObjects = replacer.Replace(level, obj);

					obj.SilentKill();

					foreach (var newObject in newObjects)
						levelReflected.RequestAddObject(newObject);
				}
			}
		}
		
		protected abstract IEnumerable<Animate> Replace(Level level, Animate objectToReplace);
	}
}