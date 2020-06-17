using System;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Gameplay;
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
			RoomTriggers.Add(new RoomTrigger(5,5, (level, itemLocation) =>
			{
				if (itemLocation.IsPickedUp || !level.GameSave.HasCutsceneBeenTriggered("Keep0_Demons0")) return;

				var itemInfo = itemLocation.ItemInfo;

				var itemDropPickupType = TimeSpinnerType.Get("Timespinner.GameObjects.Items.ItemDropPickup");
				var itemPosition = new Point(266, 208); //based on CutsceneKeep1 itemPosition
				var itemDropPickup = Activator.CreateInstance(itemDropPickupType, itemInfo.BestiaryItemDropSpecification, level, itemPosition, -1);

				var levelReflected = level.AsDynamic();
				levelReflected.RequestAddObject((Item)itemDropPickup);
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
	}
}
