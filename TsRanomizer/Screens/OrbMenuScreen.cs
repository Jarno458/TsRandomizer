using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.OrbMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class OrbMenuScreen : Screen
	{
		public OrbMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			GameSave saveFile = Reflected._saveFile;

			RemoveMeleeOrbs(saveFile, Reflected._meleeOrbAInventory);
			RemoveMeleeOrbs(saveFile, Reflected._meleeOrbBInventory);
		}

		public void RemoveMeleeOrbs(GameSave saveFile, object inventory)
		{
			var reflected = inventory.AsDynamic();

			var orbs = ((IEnumerable<InventoryItem>)reflected._items).Cast<InventoryOrb>();
			var orbsToRemove = orbs
				.Where(o => !saveFile.HasMeleeOrb(o.OrbType))
				.Select(o => o.Name)
				.ToList();

			var entries = (IList)reflected.Entries;
			var entryMapping = (IList<int>)reflected.KeyToItemLookup;
			for (var i = entries.Count - 1; i >= 0; i--)
			{
				var entry = entries[i];
				if (orbsToRemove.Contains(entry.AsDynamic().Text))
				{
					entries.RemoveAt(i);
					entryMapping.RemoveAt(i);
				}
			}
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			/*var reflected = ((object)Reflected._meleeOrbAInventory).Reflect();
			var entries = (IList)reflected.Entries;

			var x = entries;*/
		}
	}
}