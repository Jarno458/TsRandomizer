using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.RoomTriggers
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	class RoomTriggerTrigger : Attribute
	{
		public int LevelId { get; }
		public int RoomId { get; }

		public RoomTriggerTrigger(int levelId, int roomId)
		{
			LevelId = levelId;
			RoomId = roomId;
		}
	}

	class RoomState
	{
		internal Level Level { get; set; }
		internal ItemLocation RoomItemLocation { get; set; }
		internal Seed Seed { get; set; }
		internal SettingCollection Settings { get; set; }
		internal ScreenManager ScreenManager { get; set; }
		internal Roomkey RoomKey { get; set; }
	}

	abstract class RoomTrigger
	{
		static readonly Dictionary<Roomkey, List<RoomTrigger>> RoomTriggers = new Dictionary<Roomkey, List<RoomTrigger>>();

		static RoomTrigger()
		{
			// ReSharper disable once PossibleNullReferenceException
			var roomTriggerType = MethodBase.GetCurrentMethod().DeclaringType;

			// ReSharper disable once PossibleNullReferenceException
			var typesDirivedFromRoomTrigger = roomTriggerType.Assembly.GetTypes()
				.Where(t => roomTriggerType.IsAssignableFrom(t)
				            && !t.IsGenericType
				            && t != roomTriggerType);

			foreach (var typeDirivedFromRoomTrigger in typesDirivedFromRoomTrigger)
			{
				var roomTriggerTriggers = typeDirivedFromRoomTrigger
					.GetCustomAttributes(typeof(RoomTriggerTrigger), true)
					.Cast<RoomTriggerTrigger>()
					.ToList();

				if (!roomTriggerTriggers.Any())
					continue;

				var instance = (RoomTrigger)typeDirivedFromRoomTrigger.CreateInstance();

				foreach (var roomTriggerTrigger in roomTriggerTriggers)
				{
					var key = new Roomkey(roomTriggerTrigger.LevelId, roomTriggerTrigger.RoomId);

					if (RoomTriggers.TryGetValue(key, out var triggersForRoom))
						triggersForRoom.Add(instance);
					else
						RoomTriggers.Add(key, new List<RoomTrigger>{ instance });
				}
			}
		}
		
		public static void OnChangeRoom(
			Level level, Seed seed, SettingCollection gameSettings, ItemLocationMap itemLocations, ScreenManager screenManager,
			int levelId, int roomId)
		{
			var roomKey = new Roomkey(levelId, roomId);

			SpriteManager.ReloadCustomSprites(level);

			if (RoomTriggers.TryGetValue(roomKey, out var triggersForRoom))
			{
				var roomState = new RoomState {
					Level = level,
					RoomItemLocation = itemLocations[new RoomItemKey(roomKey.LevelId, roomKey.RoomId)],
					Seed = seed,
					Settings = gameSettings,
					ScreenManager = screenManager,
					RoomKey = roomKey
				};

				foreach (var trigger in triggersForRoom)
					trigger.OnRoomLoad(roomState);
			}
		}

		public abstract void OnRoomLoad(RoomState roomState);
	}
}
