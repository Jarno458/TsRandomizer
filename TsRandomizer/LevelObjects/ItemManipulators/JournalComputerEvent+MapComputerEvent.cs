using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Treasure.JournalComputerEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Treasure.MapComputerEvent")]
	// ReSharper disable once UnusedMember.Global
	class DownloadEvent : ItemManipulator
	{
		bool hasAwardedItem;

		public DownloadEvent(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (ItemInfo == null || hasAwardedItem || !Dynamic._isTriggered || !Dynamic._wasActivating)
				return;

			ShowItemAwardPopup();
			AwardContainedItem();

			hasAwardedItem = true;
		}
	}
}
