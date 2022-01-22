using System.Collections;
using System.Linq;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Settings;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.Shop.ShopMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class ShopMenuScreen : Screen
	{
		public ShopMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			var gameSettings = GameSettingsLoader.LoadSettingsSave((GameSave)Dynamic._saveFile);

			// Menu count varies on relics/items/equipment etc. being in inventory
			// Last menu is always helper functions that don't have an _items
			// but aren't otherwise distinguishable
			foreach (var i in Enumerable.Range(0, ((IList)Dynamic._subMenuCollections).Count - 1))
			{
				var shopMenu = ((IList)Dynamic._subMenuCollections)[i].AsDynamic();
				foreach (var shopMenuEntry in shopMenu._items)
				{
					int currentPrice = shopMenuEntry.AsDynamic().ShopPrice;
					if (currentPrice == 0)
					{
						// Set a price for "priceless" items
						shopMenuEntry.AsDynamic().ShopPrice = 2000;
						currentPrice = shopMenuEntry.AsDynamic().ShopPrice;
					}
					shopMenuEntry.AsDynamic().ShopPrice = (int)(currentPrice * gameSettings.ShopMultiplier.Value);
				}
			}
		}
	}
}
