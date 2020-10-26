using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Screens.SeedSelection
{
	class SeedOptionsMenuScreen
	{
		static readonly Type RelicMenuScreenType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.PauseMenu.RelicsMenuScreen");

		readonly GameScreen screen;
		readonly dynamic reflected;

		SeedOptionsMenuScreen(GameScreen screen)
		{
			this.screen = screen;

			reflected = screen.AsDynamic();
		}

		public static SeedOptionsMenuScreen Create(ScreenManager screenManager, SeedOptionsCollection options, Action<SeedOptionsCollection> onUpdate)
		{
			GCM gcm = screenManager.Reflected.GCM;

			void Noop(){}

			var screen = (GameScreen)Activator.CreateInstance(RelicMenuScreenType, GetSave(options), gcm, (Action)Noop);
			var seedOptionsMenu = new SeedOptionsMenuScreen(screen);

			seedOptionsMenu.reflected._menuTitle = "Select Seed Options";

			var relicInventory = ((object)seedOptionsMenu.reflected._relicInventory).AsDynamic();
			relicInventory.ColumnCount = 1;
			relicInventory.SetColumnWidth(226 * seedOptionsMenu.reflected.Zoom, seedOptionsMenu.reflected.Zoom);

			HookOnSelectedAction(relicInventory, options, onUpdate);

			UpdateMenuItems(seedOptionsMenu.reflected._relicInventory);

			return seedOptionsMenu;
		}

		static GameSave GetSave(SeedOptionsCollection options)
		{
			var save = GameSave.DemoSave;

			save.Inventory.AsDynamic()._relicInventory = options;

			return save;
		}

		static void HookOnSelectedAction(dynamic relicInventory, SeedOptionsCollection options, Action<SeedOptionsCollection> onUpdate)
		{
			var originalOnSelectedAction = (Action<InventoryRelic>)relicInventory._onSelectedAction;

			void OnSelected(InventoryRelic relic)
			{
				originalOnSelectedAction(relic);

				onUpdate(options);
			}

			relicInventory._onSelectedAction = (Action<InventoryRelic>)OnSelected;
		}

		static void UpdateMenuItems(object menuRelicInventory)
		{
			var relicInventory = menuRelicInventory.AsDynamic();

			var options = ((IEnumerable<InventoryItem>) relicInventory._items)
				.Cast<InventoryRelic>()
				.Select(r => r.Key)
				.ToArray();

			var entries = (IList)relicInventory.Entries;
			var entryMapping = (IList<int>)relicInventory.KeyToItemLookup;

			foreach (var option in options)
			{
				var entry = entries[entryMapping.IndexOf(option)].AsDynamic();
				var optionInfo = SeedOptionsCollection.GetSeedOptionInfo(option);

				entry.SetText(optionInfo.Name);
				entry.Description = optionInfo.Description;
			}
		}

		public static implicit operator GameScreen(SeedOptionsMenuScreen value)
		{
			return value.screen;
		}
	}
}
