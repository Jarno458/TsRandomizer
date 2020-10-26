using System;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens.SeedSelection
{
	class SeedSelectionMenuScreen
	{
		static readonly Type PasswordMenuScreenType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.PauseMenu.Options.PasswordMenuScreen");
		static readonly Type MainMenuEntryType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.MenuEntry");

		readonly ScreenManager screenManager;
		readonly GameScreen screen;
		readonly GameDifficultyMenuScreen difficultyMenu;
		readonly dynamic reflected;

		SeedSelectionMenuScreen(ScreenManager screenManager, GameScreen screen, GameDifficultyMenuScreen difficultyMenu)
		{
			this.screenManager = screenManager;
			this.screen = screen;
			this.difficultyMenu = difficultyMenu;

			reflected = screen.AsDynamic();
		}

		public static SeedSelectionMenuScreen Create(ScreenManager screenManager, GameDifficultyMenuScreen difficultyMenu)
		{
			GCM gcm = screenManager.Reflected.GCM;

			void Noop(){}

			var screen = (GameScreen)Activator.CreateInstance(PasswordMenuScreenType, null, gcm, (Action)Noop);
			var seedSelectionMenu = new SeedSelectionMenuScreen(screenManager, screen, difficultyMenu);

			seedSelectionMenu.reflected._menuTitle = "Select Seed";

			var extraButtons = new[] {
				MenuEntry.Create("OK", seedSelectionMenu.OnOkayEntrySelected).AsTimeSpinnerMenuEntry(),
				MenuEntry.Create("New", seedSelectionMenu.OnGenerateSelected).AsTimeSpinnerMenuEntry(),
				MenuEntry.Create("", _ => {}).AsTimeSpinnerMenuEntry(),
				MenuEntry.Create("Options", seedSelectionMenu.OnOptionsSelected).AsTimeSpinnerMenuEntry()
			};

			ChangeAvailableButtons(seedSelectionMenu, extraButtons);



			return seedSelectionMenu;
		}

		static void ChangeAvailableButtons(SeedSelectionMenuScreen seedSelectionMenu, object[] extraButtons)
		{
			var menuEntries = (IList)seedSelectionMenu.reflected.MenuEntries;
			var entries = menuEntries
				.Cast<object>()
				.Where(e => IsHex(e) || IsDelButton(e))
				.Concat(extraButtons)
				.ToList(MainMenuEntryType);

			((object)seedSelectionMenu.reflected._primaryMenuCollection).AsDynamic()._entries = entries;
		}

		static bool IsHex(object menuEntry)
		{
			var reflected = menuEntry.AsDynamic();
			var text = (string)reflected.Text;

			var isHex = text.Length == 1 
			        && (text[0] >= '0' && text[0] <= '9' || text[0] >= 'A' && text[0] <= 'F');

			return isHex;
		}

		static bool IsDelButton(object menuEntry)
		{
			var reflected = menuEntry.AsDynamic();

			return reflected.Text == "DEL"; //not a verry clean check but password menu isnt localised ¯\_(ツ)_/¯
		}

		void OnOkayEntrySelected(PlayerIndex playerIndex)
		{
			var hexString = GetHexString();

			if (!Seed.TryParse(hexString, out var seed))
			{
				ShowErrorDescription("Invallid seed id, its not a valid hexidecimal value");
				return;
			}

			if (!Randomizer.IsBeatable(seed, FillingMethod.Random))
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

		void OnOptionsSelected(PlayerIndex playerIndex)
		{
			var seedOptionsMenu = SeedOptionsMenuScreen.Create(screenManager, GetCurrentOptions(), OnSeedOptionsUpdated);

			screenManager.AddScreen(seedOptionsMenu, playerIndex);
		}

		void OnSeedOptionsUpdated(SeedOptionsCollection options)
		{
			var hexString = GetHexString();
	
			var seedId = hexString.Substring(0, Seed.Length - SeedOptions.Length);

			if(Seed.TryParse(seedId + options, out var seed))
				SetSeed(seed);
		}

		void SetSeed(Seed seed)
		{
			reflected._currentEnteredPassword = seed.ToString();
			reflected.RefreshDisplayPassword();

			for (var i = 10; i < 12; i++) //RefreshDisplayPassword() only blacks out a single charecter
				reflected._displayCharacters[i] = " ";
		}

		void OnGenerateSelected(PlayerIndex playerIndex)
		{
			var seed = Randomizer.Generate(FillingMethod.Random, GetCurrentOptions()).Seed;

			SetSeed(seed);
		}

		SeedOptionsCollection GetCurrentOptions()
		{
			var hexString = GetHexString();

			return !Seed.TryParse(hexString, out var seed) 
				? new SeedOptionsCollection(SeedOptions.None) 
				: new SeedOptionsCollection(seed.Options);
		}

		string GetHexString()
		{
			var hexString = (string)reflected._currentEnteredPassword;

			if (hexString.Length > Seed.Length)
				return hexString.Substring(0, Seed.Length);
			if (hexString.Length == 0)
				return new string('0', Seed.Length);
			if (hexString.Length < Seed.Length)
 				 return hexString + new string('0', Seed.Length - hexString.Length);

			return hexString;
		}

		public static implicit operator GameScreen(SeedSelectionMenuScreen value)
		{
			return value.screen;
		}
	}
}
