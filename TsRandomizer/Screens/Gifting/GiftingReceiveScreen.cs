using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago.Gifting;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer.Screens.Gifting
{
	class GiftingReceiveScreen : GiftingScreen
	{
		const int DummyTeam = -999;
		const int NumberOfTraitsToDisplay = 7;

		static readonly Type StatEntryType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.StatEntry");
		static readonly Type StatEntryDisplayEnumType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.StatEntry+EStatDisplayType");
		static readonly Type MenuUseItemInventoryType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.MenuUseItemInventory");

		List<AcceptedTraits> acceptedTraitsPerSlot = new List<AcceptedTraits>();

		InventoryItem selectedItem;
		AcceptedTraits selectedPlayer;

		public GiftingReceiveScreen(ScreenManager screenManager, GameScreen gameScreen) : base(screenManager, gameScreen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gcm)
		{
			base.Initialize(itemLocationMap, gcm);

			Dynamic._menuTitle = "Gifting - Receive";

			acceptedTraitsPerSlot = GiftingService.GetAcceptedTraits();
		}

		protected override void PopulateMainMenu(IList menuEntriesList, IList subMenuCollections)
		{
			var selectTraitsMenu = MenuEntry.Create("Choose wanted types", () => { });
			selectTraitsMenu.IsCenterAligned = false;
			selectTraitsMenu.DoesDrawLargeShadow = false;
			selectTraitsMenu.ColumnWidth = 144;
			menuEntriesList.Add(selectTraitsMenu.AsTimeSpinnerMenuEntry());

			var giftReceiveMenu = MenuEntry.Create("Open Gifts", () => { });
			giftReceiveMenu.IsCenterAligned = false;
			giftReceiveMenu.DoesDrawLargeShadow = false;
			giftReceiveMenu.ColumnWidth = 144;
			menuEntriesList.Add(giftReceiveMenu.AsTimeSpinnerMenuEntry());
		}

		object CreateGiftList(AcceptedTraits acceptedTraits)
		{
			bool OnUseItemSelected(InventoryItem item)
			{
				selectedItem = item;
				selectedPlayer = acceptedTraits;

				ConfirmMenuCollection.SetDescription($"Yeet a '{item.Name}' to {acceptedTraits.Name}?");

				Dynamic.ChangeMenuCollection(~ConfirmMenuCollection, true);

				return true;
			}

			var collection = new GiftingInventoryCollection(OnUseItemSelected);
			foreach (var item in Save.Inventory.UseItemInventory.Inventory.Values)
			{
				if (!TraitMapping.ValuesPerItem.TryGetValue(item.UseItemType, out var traits))
					continue;

				if (acceptedTraits.AcceptsAnyTrait || acceptedTraits.DesiredTraits.Any(t => traits.ContainsKey(t)))
					collection.AddItem(item.UseItemType, item.Count);
			}
			foreach (var item in Save.Inventory.EquipmentInventory.Inventory.Values)
			{
				if (!TraitMapping.ValuesPerItem.TryGetValue(item.EquipmentType, out var traits))
					continue;

				if (acceptedTraits.AcceptsAnyTrait || acceptedTraits.DesiredTraits.Any(t => traits.ContainsKey(t)))
				{
					var count = item.Count - Save.Inventory.AsDynamic().GetEquipmentEquippedCount(item.Key);
					if (count > 0)
						collection.AddItem(item.EquipmentType, count);
				}
			}

			collection.RefreshItemNameAndDescriptions();

			var inventoryMenu = MenuUseItemInventoryType.CreateInstance(false, collection, (Func<InventoryUseItem, bool>)collection.OnUseItemSelected).AsDynamic();
			inventoryMenu.Font = GameContentManager.ActiveFont;

			return ~inventoryMenu;
		}

		protected override void OnGiftItemAccept(object obj, EventArgs args)
		{
			if (selectedPlayer.Team == DummyTeam || GiftingService.Send(selectedItem, selectedPlayer))
			{
				ConfirmMenuCollection.IsVisible = false;
				Dynamic.GoToPreviousMenuCollection();

				InventoryUseItem useItemToRemove;
				switch (selectedItem)
				{
					case InventoryUseItem useItem:
						Save.Inventory.UseItemInventory.RemoveItem((int)useItem.UseItemType);
						useItemToRemove = useItem;
						break;
					case InventoryEquipment equipment:
						Save.Inventory.EquipmentInventory.RemoveItem((int)equipment.EquipmentType);
						useItemToRemove = equipment.ToInventoryUseItem();
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(selectedItem), "paramter should be either UseItem or Equipment");
				}

				foreach (var collection in Dynamic._subMenuCollections)
					if (collection.GetType() == MenuUseItemInventoryType)
						((object)collection).AsDynamic().RemoveItem(useItemToRemove);

				ScreenManager.Jukebox.PlayCue(ESFX.MenuSell);
			}
			else
			{
				ScreenManager.Jukebox.PlayCue(ESFX.MenuError);
			}
		}

		protected override void OnGiftItemCancel(object obj, EventArgs args) => Dynamic.OnCancel(obj, args);

		public override void Update(GameTime gameTime, InputState input)
		{
			base.Update(gameTime, input);

			RefreshPlayerGiftboxInfo(GameContentManager.ActiveFont);
		}

		void RefreshPlayerGiftboxInfo(SpriteFont menuFont)
		{
			var selectedIndex = ((object)Dynamic._primaryMenuCollection).AsDynamic().SelectedIndex;
			if (selectedIndex >= acceptedTraitsPerSlot.Count)
				return;

			AcceptedTraits selectedPlayerTraits = acceptedTraitsPerSlot[selectedIndex];

			var entries = (IList)PlayerInfoCollection.Entries;
			entries.Clear();

			var gameEntry = StatEntryType.CreateInstance().AsDynamic();
			gameEntry.Type = StatEntryDisplayEnumType.GetEnumValue("ColoredText");
			gameEntry.Title = "Game:";
			gameEntry.Text = selectedPlayerTraits.Game;
			gameEntry.TextColor = StatEntryColor;
			gameEntry.Initialize(menuFont);
			gameEntry._drawStringWidth = (int)(menuFont.MeasureString(gameEntry._drawString).X - 24);
			gameEntry._titleTextWidthReduction = gameEntry._drawStringWidth + 2;

			entries.Add(~gameEntry);

			if (selectedPlayerTraits.AcceptsAnyTrait)
			{
				var allTraitsEntry = StatEntryType.CreateInstance().AsDynamic();
				allTraitsEntry.Type = StatEntryDisplayEnumType.GetEnumValue("ColoredText");
				allTraitsEntry.Title = "Wants:";
				allTraitsEntry.Text = "All";
				allTraitsEntry.TextColor = StatEntryColor;
				allTraitsEntry.Initialize(menuFont);
				allTraitsEntry._drawStringWidth = (int)(menuFont.MeasureString(allTraitsEntry._drawString).X - 24);
				allTraitsEntry._titleTextWidthReduction = allTraitsEntry._drawStringWidth + 2;

				entries.Add(~allTraitsEntry);
			}
			else
			{
				var traits = new List<string>(NumberOfTraitsToDisplay + 1) {
					"Wants:"
				};

				for (var i = 0; i < NumberOfTraitsToDisplay; i++)
				{
					if (i == NumberOfTraitsToDisplay - 1)
					{
						if (selectedPlayerTraits.DesiredTraits.Length == NumberOfTraitsToDisplay)
							traits.Add(selectedPlayerTraits.DesiredTraits[i - 1].ToString());
						else if (selectedPlayerTraits.DesiredTraits.Length < NumberOfTraitsToDisplay)
							traits.Add("");
						else
							traits.Add("More...");

					}
					else
					{
						if (i < selectedPlayerTraits.DesiredTraits.Length)
							traits.Add(selectedPlayerTraits.DesiredTraits[i].ToString());
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
		}
	}
}
