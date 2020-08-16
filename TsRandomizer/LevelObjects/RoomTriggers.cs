using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.LevelObjects
{
	class RoomTrigger
	{
		static readonly LookupDictionairy<RoomItemKey, RoomTrigger> RoomTriggers = new LookupDictionairy<RoomItemKey, RoomTrigger>(rt => rt.key);

		static readonly Type TransitionWarpEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.TransitionWarpEvent");
		static readonly Type NelisteNpcType = TimeSpinnerType.Get("Timespinner.GameObjects.NPCs.AstrologerNPC");

		static RoomTrigger()
		{
			RoomTriggers.Add(new RoomTrigger(1, 5, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp || !level.GameSave.GetSaveBool("IsBossDead_RoboKitty")) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 208);
			}));
			RoomTriggers.Add(new RoomTrigger(5, 5, (level, itemLocation) =>
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
				if (!itemLocation.IsPickedUp
				    && level.GameSave.HasRelic(EInventoryRelicType.ScienceKeycardA)
				    && level.GameSave.GetSaveBool("IsBossDead_Shapeshift")) 
						SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 208);

				if(level.GameSave.HasCutsceneBeenTriggered("Alt3_Teleport"))
					CreateSimpelOneWayWarp(level, 16, 12);
			}));
			RoomTriggers.Add(new RoomTrigger(11, 26, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp
				    || !level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear1)) return;

				SpawnTreasureChest(level, true, 136, 192);
			}));
			RoomTriggers.Add(new RoomTrigger(2, 52, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp
				    || !level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear2)) return;

				SpawnTreasureChest(level, true, 104, 192);
			}));
			RoomTriggers.Add(new RoomTrigger(9, 13, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp
				    || !level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear3)) return;

				SpawnTreasureChest(level, false, 296, 176);
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
			RoomTriggers.Add(new RoomTrigger(7, 30, (level, itemLocation) =>
			{
				if (!level.GameSave.HasRelic(EInventoryRelicType.PyramidsKey)) return;

				SpawnTreasureChest(level, false, 296, 176);
			}));
			RoomTriggers.Add(new RoomTrigger(3, 0, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp
					|| level.GameSave.DataKeyBools.ContainsKey("HasUsedCityTS")
					|| !level.GameSave.HasCutsceneBeenTriggered("Forest3_Haristel")
				    || ((Dictionary<int, NPCBase>)level.AsDynamic()._npcs).Values.Any(npc => npc.GetType() == NelisteNpcType)) return;

				SpawnNeliste(level);
			}));
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

		static void SpawnTreasureChest(Level level, bool flipHorizontally, int x, int y)
		{
			var itemPosition = new Point(x, y);
			var specification = new ObjectTileSpecification {IsFlippedHorizontally = flipHorizontally, Layer = ETileLayerType.Objects};
			var treasureChest = new TreasureChestEvent(level, itemPosition, -1, specification);

			var chest = treasureChest.AsDynamic();
			chest.Initialize();

			var levelReflected = level.AsDynamic();
			levelReflected.RequestAddObject(treasureChest);
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
			var warpTrigger = events.Values.FirstOrDefault(e => e.GetType() == TransitionWarpEventType);
			if (warpTrigger == null)
			{
				var specification = new ObjectTileSpecification
				{
					Category = EObjectTileCategory.Event,
					ID = 468,
					Layer = ETileLayerType.Objects,
					ObjectID = 13,
					X = 12,
					Y = 12
				};
				var point = new Point(specification.X * 16 + 8, specification.Y * 16 + 16);
				warpTrigger = (GameEvent)TransitionWarpEventType.CreateInstance(false, level, point, -1, specification);

				dynamicLevel.RequestAddObject(warpTrigger);
			}

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

		static void SpawnNeliste(Level level)
		{
			var position = new Point(720, 368);
			var neliste = (NPCBase)NelisteNpcType.CreateInstance(false, level, position, -1, new ObjectTileSpecification());

			level.AsDynamic().RequestAddObject(neliste);
		}
	}
}
