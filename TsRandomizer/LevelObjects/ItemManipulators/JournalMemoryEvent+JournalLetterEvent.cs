using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Treasure.JournalLetterEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Treasure.JournalMemoryEvent")]
	// ReSharper disable once UnusedMember.Global
	class MemoryEvent : ItemManipulator
	{
		bool hasAwardedItem;

		public MemoryEvent(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			Dynamic._oscillationDelta = 0; // Fix the item's position so the replacement doesn't miss
			if (ItemInfo == null || hasAwardedItem || !Dynamic._isFading)
				return;
			ShowItemAwardPopup();
			AwardContainedItem();

			hasAwardedItem = true;
		}
	}
}
