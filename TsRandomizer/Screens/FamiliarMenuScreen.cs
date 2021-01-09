using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.FamiliarMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class FamiliarMenuScreen : Screen
	{
		public FamiliarMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			GameSave saveFile = Reflected._saveFile;

			RemoveUnobtainedFamiliars(saveFile, Reflected._familiarMenuInventory);
		}

		public void RemoveUnobtainedFamiliars(GameSave saveFile, object inventory)
		{
			var reflected = inventory.AsDynamic();

			var familiars = ((IEnumerable<InventoryItem>)reflected._items).Cast<InventoryFamiliar>();
			var familiarsToRemove = familiars
				.Where(f => !saveFile.HasFamiliar(f.FamiliarType))
				.Select(f => f.Name)
				.ToList();

			var entries = (IList)reflected.Entries;
			var entryMapping = (IList<int>)reflected.KeyToItemLookup;
			for (var i = entries.Count - 1; i >= 0; i--)
			{
				var entry = entries[i];
				if (familiarsToRemove.Contains(entry.AsDynamic().Text))
				{
					entries.RemoveAt(i);
					entryMapping.RemoveAt(i);
				}
			}
		}
	}
}