using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Cutscene.CutscenePrologue4")]
	// ReSharper disable once UnusedMember.Global
	class CutscenePrologue4 : LevelObject
	{
		readonly Level level;
		bool hasAwardedMeleeOrb;

		public CutscenePrologue4(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
		{
			level = (Level)Reflected._level;
		}

		protected override void Initialize()
		{
			if (ItemInfo == null)
				return;
			
			var scripts = ((Queue<ScriptAction>)((Level)Reflected._level).Reflect()._waitingScripts).ToArray();
			var giveOrbScript = scripts.Single(s => s.Reflect().ScriptType == EScriptType.RelicOrbGetToast);

			giveOrbScript.Reflect().ItemToGive = (int)ItemInfo.OrbType;
			giveOrbScript.Reflect().OrbSlot = ItemInfo.OrbSlot;
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null)
				return;

			var orbCollection = level.GameSave.Inventory.OrbInventory.Inventory;

			if (hasAwardedMeleeOrb || !orbCollection.ContainsKey((int)EInventoryOrbType.Blue))
				return;

			AwardContainedItem();
			hasAwardedMeleeOrb = true;
		}
	}
}
