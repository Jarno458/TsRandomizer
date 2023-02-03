using System;
using System.Linq;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.HUD;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.L09_CursedCaves.EnvPrefabCursedCavesCorpse")]
	// ReSharper disable once UnusedMember.Global
	class EnvPrefabCursedCavesCorpse : ItemManipulator
	{
		bool hasReplacedItemScript;
		int animationIndex;

		public EnvPrefabCursedCavesCorpse(Mobile typedObject, GameplayScreen gameplayScreen, ItemLocation itemLocation) 
			: base(typedObject, gameplayScreen, itemLocation)
		{
		}

		protected override void Initialize(Seed seed)
		{
			if (ItemInfo == null)
				return;

			Dynamic._hasKeycard = !IsPickedUp; 
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || hasReplacedItemScript)
				return;

			var dialogScript = Scripts.FirstOrDefault(s => s.AsDynamic().ScriptType == EScriptType.Dialogue);
			if (dialogScript == null || ((DialogueBox)dialogScript.AsDynamic().Dialogue).AsDynamic()._speakerEnglish != "Lunais")
				return;

			var rewardItemScript = Scripts.FirstOrDefault(s => s.AsDynamic().ScriptType == EScriptType.RelicOrbGetToast);
			if (rewardItemScript == null || rewardItemScript.AsDynamic().ItemToGive != (int)EInventoryRelicType.ScienceKeycardB)
				return;

			animationIndex = ItemInfo.AnimationIndex;

			UpdateRelicOrbGetToastToItem();

			var rewardItemDelegate = Scripts.Single(s =>
				{
					var script = s.AsDynamic();
					return script.ScriptType == EScriptType.Delegate 
					       && script.Arguments != ScriptActionQueueExtensions.ReplacedArguments;
				});

			rewardItemDelegate.AsDynamic().Delegate = new Action(() =>
			{
				AwardContainedItem();
				var itemPopupAppendage = (Appendage)Dynamic._itemPopupAppendage;
				itemPopupAppendage.ChangeAnimation(animationIndex);
				itemPopupAppendage.AsDynamic().IsPopppingUp = true;
				Dynamic._appendages.Add(itemPopupAppendage);
			});

			hasReplacedItemScript = true;
		}
	}
}
