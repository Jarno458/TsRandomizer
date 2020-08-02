using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation;
using TsRanodmizer.Screens;

namespace TsRanodmizer.LevelObjects.ItemManipulators
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

			var currentSubProgress = Object.SubProgress;

			if (Object.PrimaryProgress != 0 || (currentSubProgress != 0 && currentSubProgress != 1))
				return;

			if (Object.IsTalking && lastSubProgress == 0 && currentSubProgress == 1)
			{
				Scripts.UpdateRelicOrbGetToastToItem(Level, ItemInfo);

				AwardContainedItem();

				var fireOrbAppendage = ((Animate) Object._fireOrb).AsDynamic();

				fireOrbAppendage._sprite = gameplayScreen.GameContentManager.SpMenuIcons;
				fireOrbAppendage._unhiddenAnimationIndex = ItemInfo.AnimationIndex; //uses differnt sprite sheet
			}

			lastSubProgress = Object.SubProgress;
		}
	}
}
