using System;
using Microsoft.Xna.Framework;
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
			RoomTriggers.Add(new RoomTrigger(11, 39, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp 
					|| !level.GameSave.HasOrb(EInventoryOrbType.Eye)
				    || !level.GameSave.GetSaveBool("11_LabPower")) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 176);
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

			var levelReflected = level.AsDynamic();
			levelReflected.RequestAddObject((Item)itemDropPickup);
		}
	}
}
