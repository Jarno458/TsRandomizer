using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Treasure.JournalComputerEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Treasure.MapComputerEvent")]
	// ReSharper disable once UnusedMember.Global
	class JournalComputerEvent : ItemManipulator
	{
		bool hasAwardedItem;

		public JournalComputerEvent(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (ItemInfo == null || hasAwardedItem || !Object._isTriggered || !Object._wasActivating)
				return;

			AwardContainedItem();
			ShowItemAwardPopup();

			hasAwardedItem = true;
		}
	}
}
