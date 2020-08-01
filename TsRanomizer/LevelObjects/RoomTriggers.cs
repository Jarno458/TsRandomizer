using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.LevelObjects
{
	class RoomTrigger
	{
		static readonly LookupDictionairy<RoomItemKey, RoomTrigger> RoomTriggers = new LookupDictionairy<RoomItemKey, RoomTrigger>(rt => rt.key);

		static readonly Type TransitionWarpEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.TransitionWarpEvent");

		static RoomTrigger()
		{
			RoomTriggers.Add(new RoomTrigger(1, 5, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp || !level.GameSave.GetSaveBool("IsBossDead_RoboKitty")) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 208);
			}));
			RoomTriggers.Add(new RoomTrigger(5,5, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp || !level.GameSave.HasCutsceneBeenTriggered("Keep0_Demons0")) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 208);
			}));
			RoomTriggers.Add(new RoomTrigger(11, 1, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp || !level.GameSave.HasRelic(EInventoryRelicType.Dash)) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 280, 191);
			}));
			RoomTriggers.Add(new RoomTrigger(11, 39, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp 
					|| !level.GameSave.HasOrb(EInventoryOrbType.Eye)
				    || !level.GameSave.GetSaveBool("11_LabPower")) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 176);
			}));
			RoomTriggers.Add(new RoomTrigger(11, 21, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp
				    || !level.GameSave.HasRelic(EInventoryRelicType.ScienceKeycardA)
				    || !level.GameSave.GetSaveBool("IsBossDead_Shapeshift")) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 208);
			}));
			RoomTriggers.Add(new RoomTrigger(3, 6, (level, itemLocation) =>
			{
				if (level.GameSave.HasRelic(EInventoryRelicType.PyramidsKey)) return;

				CreateSimpelOneWayWarp(level, 2, 54);
			}));
			RoomTriggers.Add(new RoomTrigger(2, 54, (level, itemLocation) =>
			{
				if (level.GameSave.HasRelic(EInventoryRelicType.PyramidsKey)
					|| !level.GameSave.DataKeyBools.ContainsKey("HasUsedCityTS")) return;

				CreateSimpelOneWayWarp(level, 3, 6);
			}));

#if DEBUG
			RoomTriggers.Add(new RoomTrigger(1, 13, (level, itemLocation) =>
			{
				SpawnItemDropPickup(level, ItemInfo.Get(EItemType.MaxHP), 350, 150);
				SpawnOrbPredestal(level, 300, 150);
			}));
#endif
		}

		readonly RoomItemKey key;
		readonly Action<Level, ItemLocation> trigger;

		public RoomTrigger(int levelId, int roomId, Action<Level, ItemLocation> triggerMethod)
		{
			key = new RoomItemKey(levelId, roomId);
			trigger = triggerMethod;
		}

		public static void OnChangeRoom(Level level, ItemLocationMap itemLocations, int levelId, int roomId)
		{
			var roomKey = new RoomItemKey(levelId, roomId);

			if(RoomTriggers.TryGetValue(roomKey, out var trigger))
				trigger.trigger(level, itemLocations[roomKey]);
		}

		static void SpawnItemDropPickup(Level level, ItemInfo itemInfo, int x, int y)
		{
			var itemDropPickupType = TimeSpinnerType.Get("Timespinner.GameObjects.Items.ItemDropPickup");
			var itemPosition = new Point(x, y);
			var itemDropPickup = Activator.CreateInstance(itemDropPickupType, itemInfo.BestiaryItemDropSpecification, level, itemPosition, -1);

			var item = itemDropPickup.AsDynamic();
			item.Initialize();

			var levelReflected = level.AsDynamic();
			levelReflected.RequestAddObject((Item)itemDropPickup);
		}

		static void SpawnOrbPredestal(Level level, int x, int y)
		{
			var orbPedestalEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Treasure.OrbPedestalEvent");
			var itemPosition = new Point(x, y);
			var pedistalSpecification = new TileSpecification
			{
				Argument = (int)EInventoryOrbType.Monske,
				ID = 480, //orb pedistal
				Layer = ETileLayerType.Objects,
			};
			var orbPedestalEvent = Activator.CreateInstance(orbPedestalEventType, level, itemPosition, -1, ObjectTileSpecification.FromTileSpecification(pedistalSpecification));

			var pedestal = orbPedestalEvent.AsDynamic();
			pedestal.DoesSpawnDespiteBeingOwned = true;
			pedestal.Initialize();

			var levelReflected = level.AsDynamic();
			levelReflected.RequestAddObject((GameEvent)orbPedestalEvent);
		}

		static void CreateSimpelOneWayWarp(Level level, int destinationLevelId, int destinationRoomId)
		{
			var dynamicLevel = level.AsDynamic();

			Dictionary<int, GameEvent> events = dynamicLevel._levelEvents;
			var warpTrigger = events.Values.First(e => e.GetType() == TransitionWarpEventType);
			var dynamicWarpTrigger = warpTrigger.AsDynamic();

			var backToTheFutureWarp =
				new RequestButtonPressTrigger(level, warpTrigger.Position, dynamicWarpTrigger._objectSpec, (Action)delegate
				{
					dynamicWarpTrigger.StartWarpSequence(new LevelChangeRequest
					{
						LevelID = destinationLevelId,
						PreviousLevelID = level.ID,
						RoomID = destinationRoomId,
						IsUsingWarp = true,
						IsUsingWhiteFadeOut = true,
						AdditionalBlackScreenTime = 0.25f,
						FadeOutTime = 0.25f,
						FadeInTime = 1f
					});
				});

			dynamicLevel.RequestAddObject(backToTheFutureWarp);
		}
	}
}
