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
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.SelenNPC")]
	// ReSharper disable once UnusedMember.Global
	class SelenNpc : LevelObject
	{
		readonly GameSave gameSave;
		bool hasReplacedSpellPopup;
		bool hasAwardedSpellOrb;

		public SelenNpc(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
		{
			gameSave = ((Level)Reflected._level).GameSave;
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null)
				return;

			if (!hasReplacedSpellPopup && Reflected._tutorialSection == 2)
			{
				var scripts = ((Queue<ScriptAction>)((Level)Reflected._level).Reflect()._waitingScripts).ToArray();
				var giveOrbScript = scripts.Single(s => s.Reflect().ScriptType == EScriptType.RelicOrbGetToast);

				giveOrbScript.Reflect().ItemToGive = (int)ItemInfo.OrbType;
				giveOrbScript.Reflect().OrbSlot = ItemInfo.OrbSlot;
				hasReplacedSpellPopup = true;
			}
			
			if (hasAwardedSpellOrb)
				return;

			var orbCollection = gameSave.Inventory.OrbInventory.Inventory;
			if (!orbCollection.TryGetValue((int)EInventoryOrbType.Blue, out InventoryOrb orb) || !orb.IsSpellUnlocked)
				return;

			orbCollection[(int)EInventoryOrbType.Blue].IsSpellUnlocked = false;
			gameSave.AddOrb(ItemInfo.OrbType, EOrbSlot.Spell);
			hasAwardedSpellOrb = true;
		}
	}
}
