using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.AstrologerNPC")]
	// ReSharper disable once UnusedMember.Global
	class NelisteNpc : ItemManipulator
	{
		int lastSubProgress;

		public NelisteNpc(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (ItemInfo == null)
				return;

			var currentSubProgress = Dynamic.SubProgress;

			if (Dynamic.PrimaryProgress != 0 || (currentSubProgress != 0 && currentSubProgress != 1))
				return;

			if (Dynamic.IsTalking && lastSubProgress == 0 && currentSubProgress == 1)
			{
				Scripts.UpdateRelicOrbGetToastToItem(Level, ItemInfo);

				AwardContainedItem();

				var fireOrbAppendage = ((Animate)Dynamic._fireOrb).AsDynamic();

				fireOrbAppendage._sprite = gameplayScreen.GameContentManager.SpMenuIcons;
				fireOrbAppendage._unhiddenAnimationIndex = ItemInfo.AnimationIndex; //uses differnt sprite sheet
			}

			lastSubProgress = Dynamic.SubProgress;
		}
	}
}
