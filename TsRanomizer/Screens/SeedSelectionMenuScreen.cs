using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;
using ScreenManager = TsRanodmizer.OverloadedObjects.ScreenManager;

namespace TsRanodmizer.Screens
{
	class SeedSelectionMenuScreen
	{
		static readonly Type PasswordMenuScreenType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.PauseMenu.Options.PasswordMenuScreen");
		static readonly Type MainMenuEntryType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.MenuEntry");

		public GameScreen Screen { get; }

		readonly dynamic reflected;

		SeedSelectionMenuScreen(GameScreen screen)
		{
			Screen = screen;
			reflected = screen.Reflect();
		}

		public static SeedSelectionMenuScreen Create(ScreenManager screenManager)
		{
			GCM gcm = screenManager.Reflected.GCM;

			var screen = (GameScreen)Activator.CreateInstance(PasswordMenuScreenType, null, gcm);
			var seedSelectionMenu = new SeedSelectionMenuScreen(screen);

			seedSelectionMenu.reflected._menuTitle = "Select Seed";
			seedSelectionMenu.SetSeed(Program.Seed);

			var extraButtons = new[] {
				MenuEntry.Create("OK", seedSelectionMenu.OnOkayEntrySelected).Entry,
				MenuEntry.Create("New", seedSelectionMenu.OnGenerateSelected).Entry
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

			var seed = int.Parse(hexString, NumberStyles.HexNumber);

			Program.Seed = seed;
			
			Console.WriteLine($"Selected Seed: {seed:X8}");

			reflected.OnCancel(playerIndex);
		}

		void OnGenerateSelected(PlayerIndex playerIndex)
		{
			SetSeed(new Random().Next());
		}

		void SetSeed(int seed)
		{
			reflected._currentEnteredPassword = $"{seed:X8}{new string(' ', 4)}";
			reflected.RefreshDisplayPassword();
		}
	}
}
