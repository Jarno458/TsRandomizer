using Microsoft.Xna.Framework;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects.CustomItems;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 16)]
	[RoomTriggerTrigger(11, 2)]
	class LabSoftLockExits : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Seed.Options.RiskyWarps)
				return;
			// Only spawns if you are past the laser but it's still on (i.e. coming from Dad's Tower warp)
			if (roomState.Level.RoomID == 16 && !roomState.Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessGenza)))
				RoomTriggerHelper.SpawnGlowingFloor(roomState.Level, new Point(900, 300));
			// Entrance to Spider-Hell, only spawns if entered from Dynamo Works BUT lower laser is still blocked
			if (roomState.Level.RoomID == 2 &&
				// roomState.Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessDynamo)) &&
				!roomState.Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessResearch)))
				RoomTriggerHelper.SpawnGlowingFloor(roomState.Level, new Point(200, 1850));
		}

	}
}
