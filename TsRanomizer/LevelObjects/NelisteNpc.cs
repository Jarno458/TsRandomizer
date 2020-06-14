using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.AstrologerNPC")]
	// ReSharper disable once UnusedMember.Global
	class NelisteNpc : LevelObject
	{
		int lastSubProgress;

		public NelisteNpc(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
		{
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null)
				return;

			var currentSubProgress = Object.SubProgress;

			if (Object.PrimaryProgress != 0 || (currentSubProgress != 0 && currentSubProgress != 1))
				return;

			if (Object.IsTalking && lastSubProgress == 0 && currentSubProgress == 1)
			{
				Scripts.UpdateRelicOrbGetToastToItem(ItemInfo);

				AwardContainedItem();

				//((Animate)Reflected._fireOrb).Reflect()._unhiddenAnimationIndex = ; //uses differnt sprite sheet
			}

			lastSubProgress = Object.SubProgress;
		}
	}
}
