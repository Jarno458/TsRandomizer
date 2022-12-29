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

		protected override void Initialize(Seed seed)
		{
			initialProgress = Dynamic.PrimaryProgress;
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (ItemInfo == null || hasReplacedItem || initialProgress != 0 || Dynamic.PrimaryProgress != 1)
				return;

			if (Dynamic.IsTalking && Dynamic._isStandingUp)
			{
				Scripts.RemoveGiveItem();
				Scripts.UpdateRelicOrbGetToastToItem(Level, ItemInfo);

				AwardContainedItem();

				hasReplacedItem = true;
			}
		}
	}
}
