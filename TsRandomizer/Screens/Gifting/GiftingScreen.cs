using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago.Gifting;
using TsRandomizer.Randomisation;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using System.Security.Policy;

namespace TsRandomizer.Screens.Gifting
{
	abstract class GiftingScreen : Screen
	{
		static readonly Type ConfirmationMenuEntryCollectionType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.ConfirmationMenuEntryCollection");
		static readonly Type StatCollectionType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Menu.StatCollection");
		static readonly Type EInventoryItemIconType =
			TimeSpinnerType.Get("Timespinner.GameAbstractions.Inventory.EInventoryItemIcon");

		protected readonly Color StatEntryColor = new Color(240, 240, 208);

		protected GCM GameContentManager;
		protected GameSave Save;
		protected GiftingService GiftingService;

		protected dynamic ConfirmMenuCollection;
		protected dynamic PlayerInfoCollection;

		protected GiftingScreen(ScreenManager screenManager, GameScreen gameScreen) : base(screenManager, gameScreen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gcm)
		{
			GameContentManager = gcm;
			Save = Dynamic._saveFile;
			GiftingService = Client.GetGiftingService();

			Dynamic.DoesDrawScrollbarWidget = true;

			var menuCollection = ((object)Dynamic._primaryMenuCollection).AsDynamic();
			menuCollection.DoesMenuAllowScrolling = true;
			menuCollection.ScrollRowHeight = 4;
			menuCollection.SetIsCenterAligned(false);

			ConfirmMenuCollection = ConfirmationMenuEntryCollectionType.CreateInstance(false, 
				TimeSpinnerGame.Localizer.Get("use_item_yes"),
				TimeSpinnerGame.Localizer.Get("use_item_no"), "Gift item to player?",
				new Action<object, EventArgs>(OnGiftItemAccept), 
				new Action<object, EventArgs>(OnGiftItemCancel)).AsDynamic();

			ConfirmMenuCollection.Font = GameContentManager.ActiveFont;
			
			PlayerInfoCollection = StatCollectionType.CreateInstance().AsDynamic();
			((IList)Dynamic.StatCollections).Add(~PlayerInfoCollection);

			Dynamic.ChangeDescription("", EInventoryItemIconType.GetEnumValue("None"));

			PopulateMainMenu();
		}

		protected void PopulateMainMenu()
		{
			var menuEntries = (IList)Dynamic.MenuEntries;
			menuEntries.Clear();

			var subMenuCollections = (IList)Dynamic._subMenuCollections;
			subMenuCollections.Clear();

			PopulateMainMenu(menuEntries, subMenuCollections);

			subMenuCollections.Add(~ConfirmMenuCollection);
		}

		protected abstract void PopulateMainMenu(IList menuEntriesList, IList subMenuCollections);

		protected abstract void OnGiftItemAccept(object obj, EventArgs args);
		protected abstract void OnGiftItemCancel(object obj, EventArgs args);

		public override void Update(GameTime gameTime, InputState input)
		{
			var subMenuCollection = (IList)Dynamic._subMenuCollections;
			foreach (var subMenu in subMenuCollection)
			{
				var dynamicSubMenu = subMenu.AsDynamic();
				dynamicSubMenu.DrawPosition = Dynamic.ListTextDrawPosition;
				dynamicSubMenu.SetColumnWidth(Dynamic.ListColumnWidth, Dynamic.Zoom);
			}

			var menuCollection = ((object)Dynamic._primaryMenuCollection).AsDynamic();
			menuCollection.DrawPosition = new Vector2(0.05f * (float)Dynamic._screenWidth + (float)Dynamic._screenLeft, menuCollection.DrawPosition.Y);

			PlayerInfoCollection.Location = new Vector2(0.55f * (float)Dynamic._screenWidth + (float)Dynamic._screenLeft, (float)Dynamic._screenTop + 5f / 32f * (float)Dynamic._topSectionHeight + (float)Dynamic.Zoom);
			PlayerInfoCollection.Width = (int)(0.3f * (float)Dynamic._screenWidth);

			((object)Dynamic._selectedItemStats).AsDynamic().Location = new Vector2(-10000, 10000); //yeet lunais stats display
			Dynamic._iconDisplayFramePosition = new Vector2(-10000, 10000); //yeet enquipment icons

			ConfirmMenuCollection.DrawPosition = new Vector2(
				(float)Dynamic.DescriptionDrawPosition.X + (float)Dynamic._screenWidth * 0.125f,
				(float)Dynamic.DescriptionDrawPosition.Y + (float)Dynamic._bottomSectionHeight * 0.5f);
			ConfirmMenuCollection.SetColumnWidth(Dynamic.ListColumnWidth, Dynamic.Zoom);
		}
	}
}
