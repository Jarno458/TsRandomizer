using System;
using System.Linq;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.L09_CursedCaves.EnvPrefabCursedCavesCorpse")]
	// ReSharper disable once UnusedMember.Global
	class EnvPrefabCursedCavesCorpse : ItemManipulator
	{
		bool hasReplacedItemScript;

		public EnvPrefabCursedCavesCorpse(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
		}

		protected override void Initialize()
		{
			if (ItemInfo == null)
				return;

			Object._hasKeycard = !IsPickedUp; 
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || hasReplacedItemScript)
				return;
			
			// ReSharper disable once SimplifyLinqExpression
			if(!Scripts.Any(s => s.AsDynamic().ScriptType == EScriptType.RelicOrbGetToast))
				return;

			Scripts.UpdateRelicOrbGetToastToItem(ItemInfo);

			var rewardItemDelegate = Scripts.Single(s => s.AsDynamic().ScriptType == EScriptType.Delegate);
			rewardItemDelegate.AsDynamic().Delegate = new Action(() =>
			{
				AwardContainedItem();
				var itemPopupAppendage = (Appendage)Object._itemPopupAppendage;
				itemPopupAppendage.ChangeAnimation(ItemInfo.AnimationIndex);
				itemPopupAppendage.AsDynamic().IsPopppingUp = true;
				Object._appendages.Add(itemPopupAppendage);
			});

			hasReplacedItemScript = true;
		}
	}
}
