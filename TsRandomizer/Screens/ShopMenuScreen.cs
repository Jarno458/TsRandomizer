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
using System;

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

		static readonly Type MenuDescriptionType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.MenuDescription");

		static readonly Type EInventoryItemIconType = TimeSpinnerType
			.Get("Timespinner.GameAbstractions.Inventory.EInventoryItemIcon");

		// Timers for held-button repeat on quantity adjustment
		float _rightHeldTime;
		float _leftHeldTime;
		const float HoldDelay = 0.4f;
		const float HoldRepeat = 0.1f;
		float _rightLastRepeat;
		float _leftLastRepeat;

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

			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

			// Detect raw held state for repeat logic
			bool rightHeld = false;
			bool leftHeld = false;
			for (int i = 0; i < input.CurrentGamePadStates.Length; i++)
			{
				rightHeld |= input.CurrentGamePadStates[i].IsButtonDown(Buttons.DPadRight)
					|| input.CurrentGamePadStates[i].IsButtonDown(Buttons.LeftThumbstickRight)
					|| input.CurrentGamePadStates[i].IsButtonDown(Buttons.RightThumbstickRight);
				leftHeld |= input.CurrentGamePadStates[i].IsButtonDown(Buttons.DPadLeft)
					|| input.CurrentGamePadStates[i].IsButtonDown(Buttons.LeftThumbstickLeft)
					|| input.CurrentGamePadStates[i].IsButtonDown(Buttons.RightThumbstickLeft);
			}
			for (int i = 0; i < input.CurrentKeyboardStates.Length; i++)
			{
				rightHeld |= input.CurrentKeyboardStates[i].IsKeyDown(Keys.Right);
				leftHeld |= input.CurrentKeyboardStates[i].IsKeyDown(Keys.Left);
			}

			bool rightNew = input.IsNewButtonPress(Buttons.DPadRight)
				|| input.IsNewButtonPress(Buttons.LeftThumbstickRight)
				|| input.IsNewButtonPress(Buttons.RightThumbstickRight)
				|| input.IsNewKeyPress(Keys.Right);

			bool leftNew = input.IsNewButtonPress(Buttons.DPadLeft)
				|| input.IsNewButtonPress(Buttons.LeftThumbstickLeft)
				|| input.IsNewButtonPress(Buttons.RightThumbstickLeft)
				|| input.IsNewKeyPress(Keys.Left);

			// Update hold timers
			if (rightHeld)
			{
				_rightHeldTime += dt;
				if (rightNew) _rightLastRepeat = 0f;
			}
			else
			{
				_rightHeldTime = 0f;
				_rightLastRepeat = 0f;
			}

			if (leftHeld)
			{
				_leftHeldTime += dt;
				if (leftNew) _leftLastRepeat = 0f;
			}
			else
			{
				_leftHeldTime = 0f;
				_leftLastRepeat = 0f;
			}

			// Fire on initial press or after hold delay with repeat interval
			bool rightPressed = rightNew
				|| (rightHeld && _rightHeldTime >= HoldDelay && _rightHeldTime - _rightLastRepeat >= HoldRepeat);
			bool leftPressed = leftNew
				|| (leftHeld && _leftHeldTime >= HoldDelay && _leftHeldTime - _leftLastRepeat >= HoldRepeat);

			if (rightPressed && _rightHeldTime >= HoldDelay) _rightLastRepeat = _rightHeldTime;
			if (leftPressed && _leftHeldTime >= HoldDelay) _leftLastRepeat = _leftHeldTime;

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

			if (leftPressed && currentQty > vanillaCap && currentQty != 1)
				entryDynamic.QuanityToBuy = currentQty - 1;
		}

		public override void HandleInput(InputState input)
		{
			int cap = QoLSettings.Current.StackCap;
			if (cap <= 9) return;

			bool isBuying = (bool)Dynamic._isBuying;
			if (!isBuying) return;

			if (!input.IsNewPressConfirm(null)) return;

			var selectedCategory = GetSelectedCategoryMethod?.Invoke(GameScreen, null);
			if (selectedCategory == null) return;

			var selectedEntry = GetSelectedShopEntryMethod?.Invoke(selectedCategory, null);
			if (selectedEntry == null) return;

			var entryDynamic = ((object)selectedEntry).AsDynamic();
			var item = (InventoryItem)entryDynamic.Item;
			if (!(item is InventoryUseItem)) return;

			var save = (GameSave)Dynamic._saveFile;
			int currentCount = save.Inventory.UseItemInventory.Inventory.ContainsKey(item.Key)
				? save.Inventory.UseItemInventory.Inventory[item.Key].Count
				: 0;

			if (currentCount < 9) return;

			// Player holds 9+ and tries to confirm - show message immediately in HandleInput phase
			Dynamic.PlayErrorSound();
			var noneIcon = Enum.ToObject(EInventoryItemIconType, 0);
			var ctor = MenuDescriptionType.GetConstructors(
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if (ctor.Length > 0)
			{
				var menuDesc = ctor[0].Invoke(new object[]
				{
					"Vanilla cap reached - hold fewer than 9 to buy more. (We cannot fix this, sorry!)",
					(object)Dynamic.DescriptionFont,
					noneIcon,
					(object)Dynamic.GCM.SpMenuIcons,
					(object)Dynamic.GCM.SpUIButtons,
					(bool)Dynamic.IsDescriptionCentered,
					(object)Dynamic.DescriptionControllerMapping
				});
				Dynamic.CurrentDescription = menuDesc;
			}
		}
	}
}