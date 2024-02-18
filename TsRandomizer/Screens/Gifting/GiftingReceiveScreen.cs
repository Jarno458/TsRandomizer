using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago;
using TsRandomizer.Archipelago.Gifting;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer.Screens.Gifting
{
	class GiftingReceiveScreen : GiftingScreen
	{
		const int NumberOfTraitsToDisplay = 3;

		static readonly Type StatEntryType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.StatEntry");

		static readonly Type StatEntryDisplayEnumType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.StatEntry+EStatDisplayType");

		static readonly Type MenuUseItemInventoryType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.MenuUseItemInventory");

		static readonly Type MenuRelicInventoryType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.MenuRelicInventory");

		static readonly Dictionary<Trait, string> Descriptions = new Dictionary<Trait, string> {
			{ Trait.Armor, "A piece of armor (body or hat)" },
			{ Trait.Egg, "Gilded Egg's" },
			{ Trait.Flower, "Chaos status's" },
			{ Trait.Drink, "Potions" },
			{ Trait.Food, "Solid food that restores health" },
			{ Trait.Fruit, "Healthy fruit that restores health or mana" },
			{ Trait.Vegetable, "Solid food that restores health containing vegetables" },
			{ Trait.Meat, "Solid food that restores health containing meat" },
			{ Trait.Fish, "Solid food that restores health containing fish" },
			{ Trait.Heal, "Healing items" },
			{ Trait.Mana, "Mana items" },
			{ Trait.Cure, "Items that cure status ailments" },
			{ Trait.Speed, "Warp Shards" },
			{ Trait.Consumable, "Use items" },
			{ Trait.Resource, "Sand bottles" },
			{ Trait.Fiber, "Herbs" },
		};

		static readonly Dictionary<Trait, object> Icons = new Dictionary<Trait, object> {
			{ Trait.Armor, EInventoryItemIconType.GetEnumValue("AdvisorRobe") },
			{ Trait.Egg, EInventoryItemIconType.GetEnumValue("FamiliarEgg") },
			{ Trait.Flower, EInventoryItemIconType.GetEnumValue("ChaosHeal") },
			{ Trait.Drink, EInventoryItemIconType.GetEnumValue("FiligreeTea") },
			{ Trait.Food, EInventoryItemIconType.GetEnumValue("Spaghetti") },
			{ Trait.Fruit, EInventoryItemIconType.GetEnumValue("LachiemiSun") },
			{ Trait.Vegetable, EInventoryItemIconType.GetEnumValue("Casserole") },
			{ Trait.Meat, EInventoryItemIconType.GetEnumValue("FriedCheveux") },
			{ Trait.Fish, EInventoryItemIconType.GetEnumValue("UnagiRoll") },
			{ Trait.Heal, EInventoryItemIconType.GetEnumValue("Potion") },
			{ Trait.Mana, EInventoryItemIconType.GetEnumValue("Ether") },
			{ Trait.Cure, EInventoryItemIconType.GetEnumValue("Antidote") },
			{ Trait.Speed, EInventoryItemIconType.GetEnumValue("WarpCard") },
			{ Trait.Consumable, EInventoryItemIconType.GetEnumValue("FuturePotion") },
			{ Trait.Resource, EInventoryItemIconType.GetEnumValue("SandBottle") },
			{ Trait.Fiber, EInventoryItemIconType.GetEnumValue("Herb") },
		};

		object traitSelectionMenu;
		TraitsInventoryCollection traitsCollection;

		bool shouldUpdateMotherbox;

		object giftsSelectionMenu;
		Gift selectedGift;
		int selectedHash;
		InventoryItem selectedItem;

		public static readonly Dictionary<int, Gift> GiftMapping = new Dictionary<int, Gift>();
		public static readonly Dictionary<int, InventoryItem> ItemMapping = new Dictionary<int, InventoryItem>();

		static string GetTraitNameKey(Trait trait) => $"TSR_trait_{trait}";
		static string GetTraitDescriptionKey(Trait trait) => GetTraitNameKey(trait) + "_desc";

		static GiftingReceiveScreen()
		{
			Initialize();
		}

		public static void Initialize()
		{
			foreach (var trait in (Trait[])Enum.GetValues(typeof(Trait)))
			{
				TimeSpinnerGame.Localizer.OverrideKey(GetTraitNameKey(trait), trait.ToString());
				TimeSpinnerGame.Localizer.OverrideKey(GetTraitDescriptionKey(trait), Descriptions[trait]);
			}
		}

		public GiftingReceiveScreen(ScreenManager screenManager, GameScreen gameScreen) : base(screenManager,
			gameScreen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gcm)
		{
			base.Initialize(itemLocationMap, gcm);

			Dynamic._menuTitle = "Gifting - Receive";
		}

		protected override void PopulateMainMenu(IList menuEntriesList, IList subMenuCollections)
		{
			traitSelectionMenu = CreateMenuForTraits();
			if (traitSelectionMenu != null)
			{
				subMenuCollections.Add(traitSelectionMenu);

				var selectTraitsMenu = MenuEntry.Create("Choose wanted types",
					() => {
						traitSelectionMenu.AsDynamic().IsVisible = true;
						Dynamic.ChangeMenuCollection(traitSelectionMenu, true);
					});
				selectTraitsMenu.IsCenterAligned = false;
				selectTraitsMenu.DoesDrawLargeShadow = false;
				selectTraitsMenu.ColumnWidth = 144;
				menuEntriesList.Add(selectTraitsMenu.AsTimeSpinnerMenuEntry());

				traitSelectionMenu.AsDynamic().IsVisible = false;
			}

			giftsSelectionMenu = CreateMenuForGifts();
			if (giftsSelectionMenu != null)
			{
				subMenuCollections.Add(giftsSelectionMenu);

				var giftReceiveMenu = MenuEntry.Create("Open Gifts",
					() => {
						traitSelectionMenu.AsDynamic().IsVisible = false;
						Dynamic.ChangeMenuCollection(giftsSelectionMenu, true);
					});

				giftReceiveMenu.IsCenterAligned = false;
				giftReceiveMenu.DoesDrawLargeShadow = false;
				giftReceiveMenu.ColumnWidth = 144;
				menuEntriesList.Add(giftReceiveMenu.AsTimeSpinnerMenuEntry());
			}
		}

		object CreateMenuForTraits()
		{
			void OnTraitSelected(Trait trait, InventoryRelic relicMenuItem)
			{
				relicMenuItem.IsActive = !relicMenuItem.IsActive;

				shouldUpdateMotherbox = true;
			}

			var enabledTraits = GiftingService.EnabledTraits();
			traitsCollection = new TraitsInventoryCollection(OnTraitSelected);

			foreach (var trait in (Trait[])Enum.GetValues(typeof(Trait)))
			{
				traitsCollection.AddItem(trait);

				var inventoryRelic = traitsCollection.Inventory[traitsCollection.Inventory.Count - 1];

				inventoryRelic.IsActive = enabledTraits.Contains(trait);
			}

			traitsCollection.RefreshItemNameAndDescriptions();

			var inventoryMenu = MenuRelicInventoryType.CreateInstance(false, traitsCollection,
				(Action<InventoryRelic>)traitsCollection.OnRelicSelected, GameContentManager.SpPauseMenu).AsDynamic();
			inventoryMenu.Font = GameContentManager.ActiveFont;

			return ~inventoryMenu;
		}

		object CreateMenuForGifts()
		{
			void OnGiftSelected(int hash, Gift gift, InventoryItem item)
			{
				var sendingPlayer = Client.GetPlayerInfo(gift.SenderTeam, gift.SenderSlot);
				var sendingPlayerAlias = sendingPlayer?.Alias ?? $"Unknown Player {gift.SenderSlot}";

				selectedHash = hash;
				selectedItem = item;
				selectedGift = gift;

				ConfirmMenuCollection.SetDescription($"Accept gift of {gift.ItemName} from {sendingPlayerAlias}");

				Dynamic.ChangeMenuCollection(~ConfirmMenuCollection, true);
			}

			var gifts = GiftingService.GetGifts();
			var giftsCollection = new GiftingsInventoryCollection(OnGiftSelected);

			foreach (var gift in gifts)
				giftsCollection.AddItem(gift);

			giftsCollection.RefreshItemNameAndDescriptions();

			var inventoryMenu = MenuUseItemInventoryType
				.CreateInstance(false, giftsCollection, (Func<InventoryUseItem, bool>)giftsCollection.OnUseItemSelected)
				.AsDynamic();
			inventoryMenu.Font = GameContentManager.ActiveFont;

			return ~inventoryMenu;
		}

		protected override void OnGiftItemAccept(object obj, EventArgs args)
		{
			ConfirmMenuCollection.IsVisible = false;
			Dynamic.GoToPreviousMenuCollection();

			var acceptedAmount = 0;
			switch (selectedItem)
			{
				case InventoryUseItem giftedUseItem:
				{
					acceptedAmount = CalculateAmountToAccept(giftedUseItem);
					if (acceptedAmount == 0)
					{
						ScreenManager.Jukebox.PlayCue(ESFX.MenuError);
						Dynamic.ChangeDescription("Inventory full, cant accept gift", EInventoryItemIconType.GetEnumValue("None"));
						return;
					}

					Save.Inventory.UseItemInventory.AddItem(giftedUseItem.Key, acceptedAmount);
					break;
				}
				case InventoryEquipment equipment:
				{
					acceptedAmount = CalculateAmountToAccept(equipment);
					if (acceptedAmount == 0)
					{
						ScreenManager.Jukebox.PlayCue(ESFX.MenuError);
						Dynamic.ChangeDescription("Inventory full, cant accept gift");
						return;
					}

					Save.Inventory.EquipmentInventory.AddItem(equipment.Key, acceptedAmount);
					break;
				}
			}

			GiftingService.AcceptGift(selectedGift, acceptedAmount);

			RemoveFromMenu(selectedHash, acceptedAmount);

			ScreenManager.Jukebox.PlayCue(ESFX.MenuSell);
		}
		
		int CalculateAmountToAccept(InventoryUseItem giftedUseItem)
		{
			if (!Save.Inventory.UseItemInventory.Inventory.TryGetValue(giftedUseItem.Key, out var inventoryItem))
				return giftedUseItem.Count <= giftedUseItem.StackCap
					? giftedUseItem.Count
					: giftedUseItem.StackCap;

			if (inventoryItem.Count == inventoryItem.StackCap)
				return 0;

			var amountToAccept = inventoryItem.StackCap - inventoryItem.Count;
			return giftedUseItem.Count <= amountToAccept
				? giftedUseItem.Count
				: amountToAccept;
		}

		int CalculateAmountToAccept(InventoryEquipment giftEquipment)
		{
			if (!Save.Inventory.EquipmentInventory.Inventory.TryGetValue(giftEquipment.Key, out var inventoryItem))
				return giftEquipment.Count <= giftEquipment.StackCap
					? giftEquipment.Count
					: giftEquipment.StackCap;

			if (inventoryItem.Count == inventoryItem.StackCap)
				return 0;

			var amountToAccept = inventoryItem.StackCap - inventoryItem.Count;
			return giftEquipment.Count <= amountToAccept
				? giftEquipment.Count
				: amountToAccept;
		}

		protected override void OnGiftItemCancel(object obj, EventArgs args)
		{
			Dynamic.OnCancel(obj, args);

			GiftingService.RejectGift(selectedGift);

			RemoveFromMenu(selectedHash, selectedGift.Amount);

			ScreenManager.Jukebox.PlayCue(ESFX.MenuSelect);
		}

		void RemoveFromMenu(int hash, int amount)
		{
			var menuItem = new InventoryUseItem((EInventoryUseItemType)hash);
			foreach (var collection in Dynamic._subMenuCollections)
				if (collection.GetType() == MenuUseItemInventoryType)
					for (var i = 0; i < amount; i++)
						((object)collection).AsDynamic().RemoveItem(menuItem);
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			base.Update(gameTime, input);

			var selectedMenuCollection = Dynamic._selectedMenuCollection;
			if (selectedMenuCollection == traitSelectionMenu)
			{
				var menuCollection = traitSelectionMenu.AsDynamic();
				var entries = (IList)menuCollection.Entries;
				var entryIndex = (int)menuCollection.SelectedIndex;

				if (entryIndex < entries.Count)
				{
					var traitName = (string)entries[entryIndex].AsDynamic().Text;

					var highlightedTrait = (Trait)typeof(Trait).GetEnumValue(traitName);
					Dynamic.ChangeDescription(Descriptions[highlightedTrait], Icons[highlightedTrait]);
				}

				ClearPlayerGiftInfo();
			}
			else if (selectedMenuCollection == giftsSelectionMenu)
			{
				HideRelicsMenu();
				RefreshGiftInfo(GameContentManager.ActiveFont);
			} 
			else if (selectedMenuCollection == Dynamic._primaryMenuCollection)
			{
				ClearPlayerGiftInfo();

				if ((int)((object)Dynamic._primaryMenuCollection).AsDynamic().SelectedIndex != 0)
					HideRelicsMenu();

				if (shouldUpdateMotherbox)
					UpdateMotherbox();
			}
			else if (selectedMenuCollection == ~ConfirmMenuCollection)
			{
				HideRelicsMenu();
			}
		}

		void HideRelicsMenu() => traitSelectionMenu.AsDynamic().DrawPosition = new Vector2(-1000, 1000);

		void ClearPlayerGiftInfo()
		{
			var entries = (IList)PlayerInfoCollection.Entries;
			entries.Clear();
		}
 
		void RefreshGiftInfo(SpriteFont menuFont)
		{
			// Player: 
			// Game:
			// Item: 
			// Traits A 
			// B      C

			InventoryUseItem selectedMenuItem = null;
			try
			{
				//can throw
				selectedMenuItem = (InventoryUseItem)giftsSelectionMenu.AsDynamic().GetSelected();
			}
			catch
			{
			}
			if (selectedMenuItem == null)
				return;

			var gift = GiftMapping[selectedMenuItem.Key];
			var sendingPlayer = Client.GetPlayerInfo(gift.SenderTeam, gift.SenderSlot);
			if (sendingPlayer == null)
				return;

			var entries = (IList)PlayerInfoCollection.Entries;
			entries.Clear();

			var playerEntry = StatEntryType.CreateInstance().AsDynamic();
			playerEntry.Type = StatEntryDisplayEnumType.GetEnumValue("ColoredText");
			playerEntry.Title = "Player:";
			playerEntry.Text = sendingPlayer.Alias;
			playerEntry.TextColor = StatEntryColor;
			playerEntry.Initialize(menuFont);
			playerEntry._drawStringWidth = (int)(menuFont.MeasureString(playerEntry._drawString).X - 24);
			playerEntry._titleTextWidthReduction = playerEntry._drawStringWidth + 2;

			entries.Add(~playerEntry);

			var gameEntry = StatEntryType.CreateInstance().AsDynamic();
			gameEntry.Type = StatEntryDisplayEnumType.GetEnumValue("ColoredText");
			gameEntry.Title = "Game:";
			gameEntry.Text = sendingPlayer.Game;
			gameEntry.TextColor = StatEntryColor;
			gameEntry.Initialize(menuFont);
			gameEntry._drawStringWidth = (int)(menuFont.MeasureString(gameEntry._drawString).X - 24);
			gameEntry._titleTextWidthReduction = gameEntry._drawStringWidth + 2;

			entries.Add(~gameEntry);

			var originalItemEntry = StatEntryType.CreateInstance().AsDynamic();
			originalItemEntry.Type = StatEntryDisplayEnumType.GetEnumValue("ColoredText");
			originalItemEntry.Title = (gift.IsRefund) ? "Refund:" : "Gift";
			originalItemEntry.Text = gift.ItemName;
			originalItemEntry.TextColor = StatEntryColor;
			originalItemEntry.Initialize(menuFont);
			originalItemEntry._drawStringWidth = (int)(menuFont.MeasureString(originalItemEntry._drawString).X - 24);
			originalItemEntry._titleTextWidthReduction = originalItemEntry._drawStringWidth + 2;

			entries.Add(~originalItemEntry);

			var traitNames = gift.Traits
				.Where(kvp => Enum.IsDefined(typeof(Trait), kvp.Trait))
				.Select(kvp => (Trait)typeof(Trait).GetEnumValue(kvp.Trait))
				.ToArray();

			var traits = new List<string>(NumberOfTraitsToDisplay + 1) {
				"Types:"
			};

			for (var i = 0; i < NumberOfTraitsToDisplay; i++)
			{
				if (i == NumberOfTraitsToDisplay - 1)
				{
					if (traitNames.Length == NumberOfTraitsToDisplay)
						traits.Add(traitNames[i].ToString());
					else if (traitNames.Length < NumberOfTraitsToDisplay)
						traits.Add("");
					else
						traits.Add("More...");

				}
				else
				{
					if (i < traitNames.Length)
						traits.Add(traitNames[i].ToString());
					else
						traits.Add("");
				}
			}

			for (var i = 0; i < NumberOfTraitsToDisplay; i += 2)
			{
				var statEntry = StatEntryType.CreateInstance().AsDynamic();
				statEntry.Type = StatEntryDisplayEnumType.GetEnumValue("ColoredText");
				statEntry.Title = traits[i];
				statEntry.Text = traits[i + 1];
				statEntry.TextColor = StatEntryColor;
				statEntry.Initialize(menuFont);
				statEntry._drawStringWidth = (int)(menuFont.MeasureString(statEntry._drawString).X - 24);
				statEntry._titleTextWidthReduction = statEntry._drawStringWidth + 2;

				entries.Add(~statEntry);
			}
			
		}

		public override void Unload() => UpdateMotherbox();

		void UpdateMotherbox()
		{
			if (!shouldUpdateMotherbox || traitsCollection == null)
				return;

			shouldUpdateMotherbox = false;

			var enabledTraits = traitsCollection.Inventory.Values
				.Where(r => r.IsActive)
				.Select(r => (Trait)r.Key)
				.ToArray();

			GiftingService.SetAcceptedGifts(enabledTraits);
		}

		class TraitsInventoryCollection : InventoryRelicCollection
		{
			readonly Action<Trait, InventoryRelic> onTraitSelected;

			public TraitsInventoryCollection(Action<Trait, InventoryRelic> onTraitSelected)
			{
				this.onTraitSelected = onTraitSelected;
			}

			public void AddItem(Trait item) => AddItem((int)item);

			public override void RefreshItemNameAndDescriptions()
			{
				// ReSharper disable once SuggestVarOrType_SimpleTypes
				foreach (InventoryRelic relic in Inventory.Values)
				{
					var trait = (Trait)relic.Key;

					var dynamicInventoryItem = relic.AsDynamic();
					dynamicInventoryItem.NameKey = GetTraitNameKey(trait);
					dynamicInventoryItem.DescriptionKey = GetTraitDescriptionKey(trait);
				}

				base.RefreshItemNameAndDescriptions();
			}

			public void OnRelicSelected(InventoryRelic relic) =>
				onTraitSelected((Trait)relic.Key, relic);
		}

		class GiftingsInventoryCollection : InventoryUseItemCollection
		{
			readonly Action<int, Gift, InventoryItem> onGiftSelected;

			public GiftingsInventoryCollection(Action<int, Gift, InventoryItem> onGiftSelected)
			{
				this.onGiftSelected = onGiftSelected;
			}

			public void AddItem(Gift gift) => AddItem(gift, gift.Amount);

			public void AddItem(Gift gift, int count)
			{
				int hash;
				foreach (var giftMap in GiftMapping)
				{
					if (giftMap.Value.ID != gift.ID)
						continue;

					AddItem(giftMap.Key, count);

					return;
				}

				var random = new Random(gift.GetHashCode());
				while (true)
				{
					hash = random.Next();
					if (hash > 1000 && !GiftMapping.ContainsKey(hash))
					{
						var simplifiedTraits = gift.Traits
							.Where(t => Enum.IsDefined(typeof(Trait), t.Trait))
							.ToDictionary(t => (Trait)typeof(Trait).GetEnumValue(t.Trait), t => (float)t.Quality);

						GiftMapping.Add(hash, gift);
						ItemMapping.Add(hash, TraitMapping.ParseItem(gift.ItemName, simplifiedTraits, gift.Amount));

						AddItem(hash, count);

						return;
					}
				}
			}

			public override void RefreshItemNameAndDescriptions()
			{
				// ReSharper disable once SuggestVarOrType_SimpleTypes
				foreach (InventoryUseItem useItem in Inventory.Values)
				{
					var item = ItemMapping[useItem.Key];

					var dynamicInventoryItem = useItem.AsDynamic();
					dynamicInventoryItem.NameKey = item.NameKey;
					dynamicInventoryItem.DescriptionKey = item.DescriptionKey;
				}

				base.RefreshItemNameAndDescriptions();
			}

			public bool OnUseItemSelected(InventoryUseItem useItem)
			{
				onGiftSelected(useItem.Key, GiftMapping[useItem.Key], ItemMapping[useItem.Key]);
				return true;
			}
		}
	}
}
