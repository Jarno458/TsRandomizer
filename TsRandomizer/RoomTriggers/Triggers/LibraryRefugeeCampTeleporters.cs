using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(2, 54)]
	class LibraryTeleporter : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.PyramidsKey)
					&& roomState.Level.GameSave.DataKeyBools.ContainsKey("HasAccessedToRefugeCampTeleporter"))
				RoomTriggerHelper.CreateSimpleOneWayWarp(roomState.Level, 3, 6);
		}
	}

	[RoomTriggerTrigger(3, 6)]
	class RefugeeCampTeleporter : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			roomState.Level.GameSave.SetValue("HasAccessedToRefugeCampTeleporter", true);

			if (roomState.Seed.StartingEra == Era.Present
			    || (roomState.Seed.Options.BackToTheFuture
			        && roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.TimespinnerWheel)
			        && roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.TimespinnerSpindle)))
			{
				if (roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.PyramidsKey))
					roomState.Level.MarkRoomAsVisited(2, 54);
				else
					RoomTriggerHelper.CreateSimpleOneWayWarp(roomState.Level, 2, 54);
			}
		}
	}
}
