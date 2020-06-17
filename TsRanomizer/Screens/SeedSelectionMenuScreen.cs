using System;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Screens
{
	class SeedSelectionMenuScreen
	{
		static readonly Type PasswordMenuScreenType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.PauseMenu.Options.PasswordMenuScreen");
		static readonly Type MainMenuEntryType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.MenuEntry");

		readonly GameScreen screen;
		readonly GameDifficultyMenuScreen difficultyMenu;
		readonly dynamic reflected;

		SeedSelectionMenuScreen(GameScreen screen, GameDifficultyMenuScreen difficultyMenu)
		{
			this.screen = screen;
			this.difficultyMenu = difficultyMenu;
			reflected = screen.AsDynamic();
		}

		public static SeedSelectionMenuScreen Create(ScreenManager screenManager, GameDifficultyMenuScreen difficultyMenu)
		{
			GCM gcm = screenManager.Reflected.GCM;

			void Noop(){}

			var screen = (GameScreen)Activator.CreateInstance(PasswordMenuScreenType, null, gcm, (Action)Noop);
			var seedSelectionMenu = new SeedSelectionMenuScreen(screen, difficultyMenu);

			seedSelectionMenu.reflected._menuTitle = "Select Seed";

			var extraButtons = new[] {
				MenuEntry.Create("OK", seedSelectionMenu.OnOkayEntrySelected).AsTimeSpinnerMenuEntry(),
				MenuEntry.Create("New", seedSelectionMenu.OnGenerateSelected).AsTimeSpinnerMenuEntry()
			};

			ChangeAvailableButtons(seedSelectionMenu, extraButtons);

			return seedSelectionMenu;
		}

		static void ChangeAvailableButtons(SeedSelectionMenuScreen seedSelectionMenu, object[] extraButtons)
		{
			var menuEntries = (IList)seedSelectionMenu.reflected.MenuEntries;
			var entries = menuEntries
				.Cast<object>()
				.Where(e => IsHex(e) && !IsOkButton(e))
				.Concat(extraButtons)
				.ToList(MainMenuEntryType);

			((object)seedSelectionMenu.reflected._primaryMenuCollection).AsDynamic()._entries = entries;
		}

		static bool IsHex(object menuEntry)
		{
			var reflected = menuEntry.AsDynamic();
			var text = (string)reflected.Text;

			return  text.Length == 1 && text[0] >= '0' && text[0] <= '9' || text[0] >= 'A' && text[0] <= 'F';
		}

		static bool IsOkButton(object menuEntry)
		{
			var reflected = menuEntry.AsDynamic();

			return reflected.Text == "OK"; //not a verry clean check but password menu isnt localised ¯\_(ツ)_/¯
		}

		void OnOkayEntrySelected(PlayerIndex playerIndex)
		{
			var hexString = (string)reflected._currentEnteredPassword;
			if (hexString.Length > 8)
				hexString = hexString.Substring(0, 8);
			if (hexString.Length == 0)
				hexString = "0";

			if (!Seed.TrySetFromHexString(hexString, out Seed seed))
			{
				ShowErrorDescription("Invallid seed id, it cannot be parsed as hexidecimal");
				return;
			}
			
			if (!seed.IsBeatable())
			{
				ShowErrorDescription("Invallid seed id, it cannot be beated");
				return;
			}

			difficultyMenu.SetSeed(seed);

			reflected.OnCancel(playerIndex);
		}

		void ShowErrorDescription(string message)
		{
			var inventoryItemIconType = TimeSpinnerType.Get("Timespinner.GameAbstractions.Inventory.EInventoryItemIcon");
			reflected.ChangeDescription(message, inventoryItemIconType.GetEnumValue("None"));
		}

		void OnGenerateSelected(PlayerIndex playerIndex)
		{
			//TODO generate valid seed, maybe with load menu
			SetSeed(new Seed(new Random().Next()));
		}

		void SetSeed(Seed seed)
		{
			reflected._currentEnteredPassword = $"{seed}{new string(' ', 4)}";
			reflected.RefreshDisplayPassword();
		}

		public static implicit operator GameScreen(SeedSelectionMenuScreen value)
		{
			return value.screen;
		}
	}
}
