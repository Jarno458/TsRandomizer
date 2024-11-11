using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(2, 54)]
	class LibraryTeleporter : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.PyramidsKey))
				return;

			if (roomState.Level.GameSave.DataKeyBools.ContainsKey("HasUsedCityTS") 
			    && (
					!roomState.Seed.Options.Inverted
					|| (
						roomState.Seed.Options.BackToTheFuture
					    && roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.TimespinnerWheel)
						&& roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.TimespinnerSpindle)
					))
				)
				RoomTriggerHelper.CreateSimpleOneWayWarp(roomState.Level, 3, 6);

				
		}
	}

	[RoomTriggerTrigger(3, 6)]
	class RefugeeCampTeleporter : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.PyramidsKey))
			{
				if (roomState.Seed.Options.Inverted
				    && roomState.Seed.Options.BackToTheFuture
				    && roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.TimespinnerWheel)
				    && roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.TimespinnerSpindle))
						roomState.Level.MarkRoomAsVisited(2, 54);
				
				return;
			}

			if (!roomState.Seed.Options.Inverted
			    || (roomState.Seed.Options.BackToTheFuture
			        && roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.TimespinnerWheel)
			        && roomState.Level.GameSave.HasRelicEnabled(EInventoryRelicType.TimespinnerSpindle)))


				RoomTriggerHelper.CreateSimpleOneWayWarp(roomState.Level, 2, 54);
		}
	}
}
