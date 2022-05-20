using System.Collections;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.Shop.ShopMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class ShopMenuScreen : Screen
	{
		public ShopMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			var gameSettings = ((GameSave)Dynamic._saveFile).GetSettings();

			// Menu count varies on relics/items/equipment etc. being in inventory
			// Last menu is always helper functions that don't have an _items
			// but aren't otherwise distinguishable
			foreach (var i in Enumerable.Range(0, ((IList)Dynamic._subMenuCollections).Count - 1))
			{
				var shopMenu = ((IList)Dynamic._subMenuCollections)[i].AsDynamic();
				foreach (var shopMenuEntry in shopMenu._items)
				{
					var dynamicShopMenuEntry = ((object)shopMenuEntry).AsDynamic();

					var item = (InventoryItem)dynamicShopMenuEntry.Item;
					if (item.NameKey == "inv_use_MagicMarbles")
					{
						item.IsSellable = false;
						dynamicShopMenuEntry.ShopPrice = -1;
					}
					
					int currentPrice = dynamicShopMenuEntry.ShopPrice;
					if (currentPrice == 0)
					{
						// Set a price for "priceless" items
						dynamicShopMenuEntry.ShopPrice = 2000;
						currentPrice = dynamicShopMenuEntry.ShopPrice;
					}
					dynamicShopMenuEntry.ShopPrice = (int)(currentPrice * gameSettings.ShopMultiplier.Value);
				}
			}
		}
	}
}
