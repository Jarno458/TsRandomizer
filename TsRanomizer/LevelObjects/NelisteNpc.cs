using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.AstrologerNPC")]
	// ReSharper disable once UnusedMember.Global
	class NelisteNpc : LevelObject
	{
		readonly GameSave gameSave;
		readonly dynamic reflected;
		int lastSubProgress;

		public NelisteNpc(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
		{
			gameSave = ((Level)Reflected._level).GameSave;
			reflected = typedObject.Reflect();
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null)
				return;

			var itemInfo = new ItemInfo(EInventoryFamiliarType.MerchantCrow);

			var currentSubProgress = reflected.SubProgress;

			if (reflected.PrimaryProgress != 0 || (currentSubProgress != 0 && currentSubProgress != 1))
				return;

			if (reflected.IsTalking && lastSubProgress == 0 && currentSubProgress == 1)
			{
				var scripts = (Queue<ScriptAction>)((Level)Reflected._level).Reflect()._waitingScripts;
				scripts.UpdateRelicOrbGetToastToItem(itemInfo);

				gameSave.AddItem(itemInfo);
				
				//((Animate)reflected._fireOrb).Reflect()._unhiddenAnimationIndex = ; uses differnt sprite sheet
			}

			lastSubProgress = reflected.SubProgress;
		}
	}
}
