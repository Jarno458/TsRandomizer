using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
		static readonly MethodInfo GetSelectedCategoryMethod = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.Shop.ShopMenuScreen")
			.GetMethod("GetSelectedCategory", BindingFlags.NonPublic | BindingFlags.Instance);

		static readonly MethodInfo GetSelectedShopEntryMethod = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.Shop.ShopMenuEntryCollection")
			.GetMethod("GetSelectedShopEntry", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

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

					if (item is InventoryUseItem useItem)
					{
						var type = useItem.UseItemType;
						if (type != EInventoryUseItemType.MagicMarbles
							&& type != EInventoryUseItemType.EssenceCrystal
							&& type != EInventoryUseItemType.GoldRing
							&& type != EInventoryUseItemType.GoldNecklace)
							useItem.StackCap = QoLSettings.Current.StackCap;
					}
				}
			}
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			int cap = QoLSettings.Current.StackCap;
			if (cap <= 9) return;

			bool isBuying = (bool)Dynamic._isBuying;
			if (!isBuying) return;

			// Use only IsNew checks — held state would increment every frame
			bool rightPressed = input.IsNewButtonPress(Buttons.DPadRight)
				|| input.IsNewButtonPress(Buttons.LeftThumbstickRight)
				|| input.IsNewButtonPress(Buttons.RightThumbstickRight)
				|| input.IsNewKeyPress(Keys.Right);

			bool leftPressed = input.IsNewButtonPress(Buttons.DPadLeft)
				|| input.IsNewButtonPress(Buttons.LeftThumbstickLeft)
				|| input.IsNewButtonPress(Buttons.RightThumbstickLeft)
				|| input.IsNewKeyPress(Keys.Left);

			if (!rightPressed && !leftPressed) return;

			var selectedCategory = GetSelectedCategoryMethod?.Invoke(GameScreen, null);
			if (selectedCategory == null) return;

			var selectedEntry = GetSelectedShopEntryMethod?.Invoke(selectedCategory, null);
			if (selectedEntry == null) return;

			var entryDynamic = ((object)selectedEntry).AsDynamic();
			var item = (InventoryItem)entryDynamic.Item;
			if (!(item is InventoryUseItem useItem2)) return;

			var type2 = useItem2.UseItemType;
			if (type2 == EInventoryUseItemType.MagicMarbles
				|| type2 == EInventoryUseItemType.EssenceCrystal
				|| type2 == EInventoryUseItemType.GoldRing
				|| type2 == EInventoryUseItemType.GoldNecklace)
				return;

			var save = (GameSave)Dynamic._saveFile;
			int currentCount = save.Inventory.UseItemInventory.Inventory.ContainsKey(item.Key)
				? save.Inventory.UseItemInventory.Inventory[item.Key].Count
				: 0;

			int maxCanBuy = cap - currentCount;
			if (maxCanBuy <= 0) return;

			int currentQty = (int)entryDynamic.QuanityToBuy;
			// Vanilla cap is 9 total, so vanilla allows buying up to (9 - currentCount)
			int vanillaCap = 9 - currentCount;

			// Only act when we are already at or above the vanilla cap,
			// so below that the vanilla input handling works as normal.
			if (rightPressed && currentQty >= vanillaCap && currentQty < maxCanBuy)
				entryDynamic.QuanityToBuy = currentQty + 1;

			if (leftPressed && currentQty > vanillaCap)
				entryDynamic.QuanityToBuy = currentQty - 1;
		}
	}
}