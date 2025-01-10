using Microsoft.Xna.Framework;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects.CustomItems;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 16)]
	class LabSoftLockExits : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Seed.Options.RiskyWarps)
				return;
			// Only spawns if you are past the laser but it's still on (i.e. coming from Dad's Tower warp)
			if (roomState.Level.RoomID == 16 && (
				// 11_LabPower true = power off
				!roomState.Level.GameSave.GetSaveBool("11_LabPower")
				|| (roomState.Seed.Options.LockKeyAmadeus && !roomState.Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.LabAccessGenza)))
			))
				RoomTriggerHelper.SpawnGlowingFloor(roomState.Level, new Point(900, 300));
		}

	}
}
