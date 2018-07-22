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
		readonly dynamic reflected;

		SeedSelectionMenuScreen(GameScreen screen)
		{
			this.screen = screen;
			reflected = screen.Reflect();
		}

		public static SeedSelectionMenuScreen Create(ScreenManager screenManager)
		{
			GCM gcm = screenManager.Reflected.GCM;

			var screen = (GameScreen)Activator.CreateInstance(PasswordMenuScreenType, null, gcm);
			var seedSelectionMenu = new SeedSelectionMenuScreen(screen);

			seedSelectionMenu.reflected._menuTitle = "Select Seed";
			seedSelectionMenu.SetSeed(Seed.Current);

			var extraButtons = new[] {
				MenuEntry.Create("OK", seedSelectionMenu.OnOkayEntrySelected).AsTimeSpinnerMenuEntry(),
				MenuEntry.Create("New", seedSelectionMenu.OnGenerateSelected).AsTimeSpinnerMenuEntry()
			};

			var menuEntries = (IList)seedSelectionMenu.reflected.MenuEntries;
			var entries = menuEntries
				.Cast<object>()
				.Where(e => IsHex(e) && !IsOkButton(e))
				.Concat(extraButtons)
				.ToListOfType(MainMenuEntryType);

			((object)seedSelectionMenu.reflected._primaryMenuCollection).Reflect()._entries = entries;

			return seedSelectionMenu;
		}

		static bool IsHex(object menuEntry)
		{
			var reflected = menuEntry.Reflect();
			var text = (string)reflected.Text;

			return  text.Length == 1 && text[0] >= '0' && text[0] <= '9' || text[0] >= 'A' && text[0] <= 'F';
		}

		static bool IsOkButton(object menuEntry)
		{
			var reflected = menuEntry.Reflect();

			//TODO: Check for localised version
			return reflected.Text == "OK";
		}

		void OnOkayEntrySelected(PlayerIndex playerIndex)
		{
			var hexString = (string)reflected._currentEnteredPassword;
			if (hexString.Length > 8)
				hexString = hexString.Substring(0, 8);
			if (hexString.Length == 0)
				hexString = "0";

			Seed.TrySetFromText(hexString);
			Console.WriteLine($"Selected Seed: {Seed.Current}");

			reflected.OnCancel(playerIndex);
		}

		void OnGenerateSelected(PlayerIndex playerIndex)
		{
			SetSeed(new Seed());
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
