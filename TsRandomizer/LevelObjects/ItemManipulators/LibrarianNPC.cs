using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.LibrarianNPC")]
	// ReSharper disable once UnusedMember.Global
	class LibrarianNpc : ItemManipulator
	{
		bool hasReplacedItem;
		int initialProgress;

		public LibrarianNpc(Mobile typedObject, GameplayScreen gameplayScreen, ItemLocation itemLocation) 
			: base(typedObject, gameplayScreen, itemLocation)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			initialProgress = Dynamic.PrimaryProgress;
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || hasReplacedItem || initialProgress != 0 || Dynamic.PrimaryProgress != 1)
				return;

			if (Dynamic.IsTalking && Dynamic._isStandingUp)
			{
				Scripts.RemoveGiveItem();
				UpdateRelicOrbGetToastToItem();

				AwardContainedItem();

				hasReplacedItem = true;
			}
		}
	}
}
