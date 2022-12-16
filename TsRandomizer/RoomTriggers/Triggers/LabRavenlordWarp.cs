using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(11, 4)]
	class LabRavenlordWarp : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Seed.Options.GyreArchives || !roomState.Level.GameSave.HasFamiliar(EInventoryFamiliarType.MerchantCrow))
				return;

			// Historical Documents room to Ravenlord
			RoomTriggerHelper.SpawnGyreWarp(roomState.Level, 200, 200); 
		}
	}
}
