using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.ReplacementObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Relics.TimespinnerSpindleItem")]
	class TimespinnerSpindle : Replaces
	{
		public const int YOffset = 18;

		protected override IEnumerable<Animate> Replace(Level level, Animate obj)
		{
			var reflected = obj.AsDynamic();
			var levelReflected = level.AsDynamic();

			var treasureChest = 
				new TreasureChestEvent(level, new Point(obj.Position.X, obj.Position.Y + YOffset), -1, reflected._objectSpec);

			var trigger = new TriggerAfterLootDrop(level, treasureChest, () =>
			{
				bool hasSpindle = level.GameSave.HasRelic(EInventoryRelicType.TimespinnerSpindle);
				if (hasSpindle)
					level.GameSave.Inventory.RelicInventory.Inventory.Remove((int)EInventoryRelicType.TimespinnerSpindle);

				((Queue<ScriptAction>)levelReflected._waitingScripts).Clear();

				reflected._onPickedUpAction();

				if (!hasSpindle)
					level.GameSave.Inventory.RelicInventory.Inventory.Remove((int) EInventoryRelicType.TimespinnerSpindle);

				var scripts = ((Queue<ScriptAction>) levelReflected._waitingScripts).ToArray().ToList();
				var giveOrbScript = scripts.Single(s => s.AsDynamic().ScriptType == EScriptType.RelicOrbGetToast);

				giveOrbScript.AsDynamic().ScriptType = EScriptType.Wait;
				giveOrbScript.AsDynamic().ActionTimer = 0f;
			});

			return new Animate[] {treasureChest, trigger};
		}
	}
}
