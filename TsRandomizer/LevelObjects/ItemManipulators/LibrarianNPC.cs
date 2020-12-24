using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.LibrarianNPC")]
	// ReSharper disable once UnusedMember.Global
	class LibrarianNpc : ItemManipulator
	{
		bool hasReplacedItem;
		int initialProgress;

		public LibrarianNpc(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
		}

		protected override void Initialize(SeedOptions options)
		{
			initialProgress = Object.PrimaryProgress;
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (ItemInfo == null || hasReplacedItem || initialProgress != 0 || Object.PrimaryProgress != 1)
				return;

			if (Object.IsTalking && Object._isStandingUp)
			{
				Scripts.RemoveGiveItem();
				Scripts.UpdateRelicOrbGetToastToItem(Level, ItemInfo);

				AwardContainedItem();

				hasReplacedItem = true;
			}
		}
	}
}
