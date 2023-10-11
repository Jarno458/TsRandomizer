using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago;
using TsRandomizer.Archipelago.Gifting;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer.Screens.Gifting
{
	class GiftingReceiveScreen : Screen
	{
		const int DummyTeam = -999;
		const int NumberOfTraitsToDisplay = 7;

		static readonly Type EquipmentMenuScreenType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.PauseMenu.EquipmentMenuScreen");
		static readonly Type StatCollectionType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.StatCollection");
		static readonly Type StatEntryType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.StatEntry");
		static readonly Type StatEntryDisplayEnumType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.StatEntry+EStatDisplayType");
		static readonly Type MenuUseItemInventoryType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.MenuUseItemInventory");
		static readonly Type ConfirmationMenuEntryCollectionType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.ConfirmationMenuEntryCollection");

		static readonly Color StatEntryColor = new Color(240, 240, 208);

		readonly bool isUsedAsGiftingMenu;
		GCM gameContentManager;
		GameSave save;

		GiftingService giftingService;
		List<AcceptedTraits> acceptedTraitsPerSlot = new List<AcceptedTraits>();

		InventoryItem selectedItem;
		AcceptedTraits selectedPlayer;
		dynamic confirmMenuCollection;

		dynamic playerInfoCollection;

		public GiftingReceiveScreen(ScreenManager screenManager, GameScreen gameScreen) : base(screenManager, gameScreen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gcm)
		{
			gameContentManager = gcm;
			save = Dynamic._saveFile;

			Dynamic._menuTitle = "Gifting - Sending";
			Dynamic.DoesDrawScrollbarWidget = true;

			giftingService = Client.GetGiftingService();
			acceptedTraitsPerSlot = giftingService.GetAcceptedTraits();

			var menuCollection = ((object)Dynamic._primaryMenuCollection).AsDynamic();

			menuCollection.DoesMenuAllowScrolling = true;
			menuCollection.ScrollRowHeight = 4;
			menuCollection.SetIsCenterAligned(false);

			confirmMenuCollection = ConfirmationMenuEntryCollectionType
				.CreateInstance(false, TimeSpinnerGame.Localizer.Get("use_item_yes"),
					TimeSpinnerGame.Localizer.Get("use_item_no"), "Gift item to player?",
					new Action<object, EventArgs>(OnGiftItemAccept), new Action<object, EventArgs>(OnGiftItemCancel)).AsDynamic();
			confirmMenuCollection.Font = gameContentManager.ActiveFont;

			playerInfoCollection = StatCollectionType.CreateInstance().AsDynamic();
			((IList)Dynamic.StatCollections).Add(~playerInfoCollection);

			PopulatePlayerMenus();
		};


		void PopulatePlayerMenus()
		{
			var menuEntries = (IList)Dynamic.MenuEntries;
			menuEntries.Clear();

			var subMenuCollections = (IList)Dynamic._subMenuCollections;
			subMenuCollections.Clear();

			if (!acceptedTraitsPerSlot.Any())
			{
				var mainMenuEntry = MenuEntry.Create("No Available Players", () => { });
				mainMenuEntry.IsCenterAligned = false;
				mainMenuEntry.DoesDrawLargeShadow = false;
				mainMenuEntry.ColumnWidth = 144;
				menuEntries.Add(mainMenuEntry.AsTimeSpinnerMenuEntry());
			}
			else
			{
				foreach (var acceptedTraits in acceptedTraitsPerSlot)
				{
					var inventoryMenu = CreateMenuUseItemInventory(acceptedTraits);
					if (inventoryMenu == null)
						continue;

					subMenuCollections.Add(inventoryMenu);

					var mainMenuEntry = MenuEntry.Create(acceptedTraits.Name, () => { Dynamic.ChangeMenuCollection(inventoryMenu, true); });
					mainMenuEntry.IsCenterAligned = false;
					mainMenuEntry.DoesDrawLargeShadow = false;
					mainMenuEntry.ColumnWidth = 144;
					menuEntries.Add(mainMenuEntry.AsTimeSpinnerMenuEntry());
				}
			}

			subMenuCollections.Add(~confirmMenuCollection);
		}



#if DEBUG
		void LoadAcceptedTraitsDummyData()
		{
			acceptedTraitsPerSlot.Clear();

			acceptedTraitsPerSlot.Add(new AcceptedTraits
			{
				Team = DummyTeam,
				Slot = 1,
				Game = "Timespinner",
				Name = "HealMe",
				AcceptsAnyTrait = false,
				DesiredTraits = new[] { Trait.Heal }
			});

			acceptedTraitsPerSlot.Add(new AcceptedTraits
			{
				Team = DummyTeam,
				Slot = 2,
				Game = "Satisfactory",
				Name = "JarnoSF",
				AcceptsAnyTrait = true,
				DesiredTraits = new Trait[0]
			});

			acceptedTraitsPerSlot.Add(new AcceptedTraits
			{
				Team = DummyTeam,
				Slot = 3,
				Game = "SomeGame",
				Name = "I Need Mana",
				AcceptsAnyTrait = false,
				DesiredTraits = new[] { Trait.Mana }
			});

			acceptedTraitsPerSlot.Add(new AcceptedTraits
			{
				Team = DummyTeam,
				Slot = 4,
				Game = "Yolo",
				Name = "Fishy",
				AcceptsAnyTrait = false,
				DesiredTraits = new[] { Trait.Fish }
			});

			acceptedTraitsPerSlot.Add(new AcceptedTraits
			{
				Team = DummyTeam,
				Slot = 5,
				Game = "Yolo2",
				Name = "Some really rather long name",
				AcceptsAnyTrait = false,
				DesiredTraits = new[] { Trait.Consumable, Trait.Flower, Trait.Heal, Trait.Food, Trait.Cure, Trait.Drink, Trait.Vegetable, Trait.Fruit }
			});

			PopulatePlayerMenus();

			giftingService.NumberOfGifts += 1;
		}
#endif

		void OnGiftItemAccept(object obj, EventArgs args)
		{
			if (selectedPlayer.Team == DummyTeam || giftingService.Send(selectedItem, selectedPlayer))
			{
				confirmMenuCollection.IsVisible = false;
				Dynamic.GoToPreviousMenuCollection();

				switch (selectedItem)
				{
					case InventoryUseItem useItem:
						save.Inventory.UseItemInventory.RemoveItem((int)useItem.UseItemType);
						((object)Dynamic._selectedMenuCollection).AsDynamic().RemoveItem(useItem);
						break;
					case InventoryEquipment equipment:
						save.Inventory.EquipmentInventory.RemoveItem((int)equipment.EquipmentType);
						((object)Dynamic._selectedMenuCollection).AsDynamic().RemoveItem(equipment.ToInventoryUseItem());
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(selectedItem), "paramter should be either UseItem or Equipment");
				}

				ScreenManager.Jukebox.PlayCue(ESFX.MenuSell);
			}
			else
			{
				ScreenManager.Jukebox.PlayCue(ESFX.MenuError);
			}
		}

		void OnGiftItemCancel(object obj, EventArgs args) => Dynamic.OnCancel(obj, args);

		public override void Update(GameTime gameTime, InputState input)
		{
#if DEBUG
			if (input.IsNewPressTertiary(null))
				LoadAcceptedTraitsDummyData();
#endif

			var subMenuCollection = (IList)Dynamic._subMenuCollections;
			foreach (var subMenu in subMenuCollection)
			{
				var dynamicSubMenu = subMenu.AsDynamic();
				dynamicSubMenu.DrawPosition = Dynamic.ListTextDrawPosition;
				dynamicSubMenu.SetColumnWidth(Dynamic.ListColumnWidth, Dynamic.Zoom);
			}

			var menuCollection = ((object)Dynamic._primaryMenuCollection).AsDynamic();
			menuCollection.DrawPosition = new Vector2(0.05f * (float)Dynamic._screenWidth + (float)Dynamic._screenLeft, menuCollection.DrawPosition.Y);

			playerInfoCollection.Location = new Vector2(0.55f * (float)Dynamic._screenWidth + (float)Dynamic._screenLeft, (float)Dynamic._screenTop + 5f / 32f * (float)Dynamic._topSectionHeight + (float)Dynamic.Zoom);
			playerInfoCollection.Width = (int)(0.3f * (float)Dynamic._screenWidth);

			((object)Dynamic._selectedItemStats).AsDynamic().Location = new Vector2(-10000, 10000); //yeet lunais stats display
			Dynamic._iconDisplayFramePosition = new Vector2(-10000, 10000); //yeet enquipment icons

			confirmMenuCollection.DrawPosition = new Vector2(
				(float)Dynamic.DescriptionDrawPosition.X + (float)Dynamic._screenWidth * 0.125f,
				(float)Dynamic.DescriptionDrawPosition.Y + (float)Dynamic._bottomSectionHeight * 0.5f);
			confirmMenuCollection.SetColumnWidth(Dynamic.ListColumnWidth, Dynamic.Zoom);

			RefreshPlayerGiftboxInfo(gameContentManager.ActiveFont);
		};
	}
}
