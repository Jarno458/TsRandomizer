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

		public NelisteNpc(Mobile typedObject, GameplayScreen gameplayScreen, ItemLocation itemLocation) 
			: base(typedObject, gameplayScreen, itemLocation)
		{
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null)
				return;

			var currentSubProgress = Dynamic.SubProgress;

			if (Dynamic.PrimaryProgress != 0 || (currentSubProgress != 0 && currentSubProgress != 1))
				return;

			if (Dynamic.IsTalking && lastSubProgress == 0 && currentSubProgress == 1)
			{
				UpdateRelicOrbGetToastToItem();

				AwardContainedItem();

				var fireOrbAppendage = ((Animate)Dynamic._fireOrb).AsDynamic();

				fireOrbAppendage._sprite = Level.GCM.SpMenuIcons;
				fireOrbAppendage._unhiddenAnimationIndex = ItemInfo.AnimationIndex; //uses different sprite sheet
			}

			lastSubProgress = Dynamic.SubProgress;
		}
	}
}
