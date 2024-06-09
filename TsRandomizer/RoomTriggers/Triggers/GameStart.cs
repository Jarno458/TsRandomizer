using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(0, 3)]
	class GameStart : RoomTrigger
	{
		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Seed.Options.StartWithJewelryBox)
				roomState.Level.AsDynamic().UnlockRelic(EInventoryRelicType.JewelryBox);

			if (roomState.Seed.Options.StartWithMeyef)
			{
				roomState.Level.GameSave.AddItem(roomState.Level, new ItemIdentifier(EInventoryFamiliarType.Meyef));
				roomState.Level.GameSave.Inventory.EquippedFamiliar = EInventoryFamiliarType.Meyef;

				var luniasObject = roomState.Level.MainHero.AsDynamic();
				var familiarManager = ((object)luniasObject._familiarManager).AsDynamic();

				familiarManager.ChangeFamiliar(EInventoryFamiliarType.Meyef);
				familiarManager.AddFamiliarPoofAnimation();
			}

			if (roomState.Seed.Options.StartWithTalaria)
				roomState.Level.AsDynamic().UnlockRelic(EInventoryRelicType.Dash);

			if (roomState.Seed.Options.UnchainedKeys)
			{
				roomState.Level.AsDynamic().UnlockRelic(EInventoryRelicType.PyramidsKey);
			}
		}
	}
}
