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
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens.SeedSelection
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.RelicsMenuScreen")]
	class SeedOptionsMenuScreen : Screen
	{
		static readonly Type RelicMenuScreenType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.PauseMenu.RelicsMenuScreen");

		readonly SeedSelectionMenuScreen seedSelectionScreen;

		bool IsUsedAsSeedOptionsMenu => seedSelectionScreen != null;

		public static GameScreen Create(ScreenManager screenManager, SeedOptionsCollection options)
		{
			void Noop() { }

			return (GameScreen)Activator.CreateInstance(RelicMenuScreenType, GetSave(options), screenManager.Dynamic.GCM, (Action)Noop);
		}

		public SeedOptionsMenuScreen(ScreenManager screenManager, GameScreen passwordMenuScreen) : base(screenManager, passwordMenuScreen)
		{
			seedSelectionScreen = screenManager.FirstOrDefault<SeedSelectionMenuScreen>();
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			if(!IsUsedAsSeedOptionsMenu)
				return;

			Dynamic._menuTitle = "Select Seed Options";

			var relicInventory = ((object)Dynamic._relicInventory).AsDynamic();
			relicInventory.ColumnCount = 1;
			relicInventory.SetColumnWidth(226 * Dynamic.Zoom, Dynamic.Zoom);

			HookOnSelectedAction(relicInventory);

			UpdateMenuItems(Dynamic._relicInventory);
		}

		static GameSave GetSave(SeedOptionsCollection options)
		{
			var save = GameSave.DemoSave;

			save.Inventory.AsDynamic()._relicInventory = options;

			return save;
		}

		void HookOnSelectedAction(dynamic relicInventory)
		{
			var originalOnSelectedAction = (Action<InventoryRelic>)relicInventory._onSelectedAction;

			void OnSelected(InventoryRelic relic)
			{
				originalOnSelectedAction(relic);

				seedSelectionScreen.OnSeedOptionsUpdated(GetOptions());
			}

			relicInventory._onSelectedAction = (Action<InventoryRelic>)OnSelected;
		}

		SeedOptionsCollection GetOptions()
		{
			return (SeedOptionsCollection)((object)Dynamic._relicInventory).AsDynamic()._collection;
		}
		static void UpdateMenuItems(object menuRelicInventory)
		{
			var relicInventory = menuRelicInventory.AsDynamic();

			var options = ((IEnumerable<InventoryItem>)relicInventory._items)
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
	}
}
