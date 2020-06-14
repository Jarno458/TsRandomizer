using System;
using System.Linq;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.L09_CursedCaves.EnvPrefabCursedCavesCorpse")]
	// ReSharper disable once UnusedMember.Global
	class EnvPrefabCursedCavesCorpse : LevelObject
	{
		bool hasReplacedItemScript;

		public EnvPrefabCursedCavesCorpse(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
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
