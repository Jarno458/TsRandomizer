using System;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;
using SDL2;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace TsRandomizer.Screens.SeedSelection
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.Options.PasswordMenuScreen")]
	class SeedSelectionMenuScreen : Screen
	{
		static readonly Type PasswordMenuScreenType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.PauseMenu.Options.PasswordMenuScreen");
		static readonly Type MainMenuEntryType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.MenuEntry");
		static readonly Type InventoryItemIconType = TimeSpinnerType
			.Get("Timespinner.GameAbstractions.Inventory.EInventoryItemIcon");

		readonly GameDifficultyMenuScreen difficultyMenu;

		bool IsUsedAsSeedSelectionMenu => difficultyMenu != null;

		bool forceSeed;
		MenuEntry okButton;

		public static GameScreen Create(ScreenManager screenManager)
		{
			void Noop() { }

			return (GameScreen)Activator.CreateInstance(PasswordMenuScreenType, null, screenManager.Reflected.GCM, (Action)Noop);
		}

		public SeedSelectionMenuScreen(ScreenManager screenManager, GameScreen passwordScreen) : base(screenManager, passwordScreen)
		{
			difficultyMenu = screenManager.FirstOrDefault<GameDifficultyMenuScreen>();
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			if (!IsUsedAsSeedSelectionMenu)
				return;

			Dynamic._menuTitle = "Select Seed";

			okButton = MenuEntry.Create("OK", OnOkayEntrySelected);

			ChangeAvailableButtons(
				okButton,
				MenuEntry.Create("", () => { }, false),
				MenuEntry.Create("New", OnGenerateSelected),
				MenuEntry.Create("Options", OnOptionsSelected)
			);
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			if (!IsUsedAsSeedSelectionMenu)
				return;

			forceSeed = input.IsButtonHold(Buttons.RightTrigger);

			okButton.Text = forceSeed ? "Force" : "OK";

			if (input.IsControllHold())
			{
				if(input.IsKeyHold(Keys.V) && SDL.SDL_HasClipboardText() == SDL.SDL_bool.SDL_TRUE)
					GetClipboardSeed();
				else if (input.IsKeyHold(Keys.C)) 
					SDL.SDL_SetClipboardText(GetHexString());
			}

			var selectedMenuEntryIndex = Dynamic.SelectedIndex;
			if (GetSelectedMenuEntryText(selectedMenuEntryIndex) == "")
			{
				if (input.IsPressMenuLeft(null))
					SetSelectedMenuItemByIndex(selectedMenuEntryIndex - 1);
				else
					SetSelectedMenuItemByIndex(selectedMenuEntryIndex + 1);
			}
		}

		void GetClipboardSeed()
		{
			var text = SDL.SDL_GetClipboardText().Trim();

			if (text.Length > Seed.Length)
				text = text.Substring(0, Seed.Length);

			SetSeed(text);
		}

		void ChangeAvailableButtons(params MenuEntry[] extraButtons)
		{
			var entries = ((IList)Dynamic.MenuEntries)
				.Cast<object>()
				.Where(e => IsHex(e) || IsDelButton(e))
				.Concat(extraButtons.Select(b => b.AsTimeSpinnerMenuEntry()))
				.ToList(MainMenuEntryType);

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = entries;
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

			if (!forceSeed && !Randomizer.IsBeatable(seed, FillingMethod.Random))
			{
				ShowErrorDescription("Invallid seed id, it cannot be beated");
				return;
			}

			difficultyMenu.SetSeedAndFillingMethod(seed, FillingMethod.Random);

			Dynamic.OnCancel(playerIndex);
		}

		void ShowErrorDescription(string message)
		{
			Dynamic.ChangeDescription(message, InventoryItemIconType.GetEnumValue("None"));
		}

		void OnOptionsSelected(PlayerIndex playerIndex)
		{
			var seedOptionsMenu = SeedOptionsMenuScreen.Create(ScreenManager, GetCurrentOptions());

			ScreenManager.AddScreen(seedOptionsMenu, playerIndex);
		}

		internal void OnSeedOptionsUpdated(SeedOptionsCollection options)
		{
			var hexString = GetHexString();
	
			var seedId = hexString.Substring(0, Seed.Length - SeedOptions.Length);

			if(Seed.TryParse(seedId + options, out var seed))
				SetSeed(seed);
		}

		void SetSeed(Seed seed)
		{
			SetSeed(seed.ToString());
		}

		void SetSeed(string seedString)
		{
			Dynamic._currentEnteredPassword = seedString;
			Dynamic.RefreshDisplayPassword();

			const int numberOfDisplayDigits = 12;
			for (var i = Seed.Length; i < numberOfDisplayDigits; i++) //RefreshDisplayPassword() only blacks out a single charecter
				Dynamic._displayCharacters[i] = " ";
		}

		void OnGenerateSelected()
		{
			var seed = Randomizer.Generate(FillingMethod.Random, GetCurrentOptions()).Seed;

			SetSeed(seed);
		}

		internal SeedOptionsCollection GetCurrentOptions()
		{
			var hexString = GetHexString();

			return !Seed.TryParse(hexString, out var seed) 
				? new SeedOptionsCollection(SeedOptions.None) 
				: new SeedOptionsCollection(seed.Options);
		}

		string GetHexString()
		{
			var hexString = (string)Dynamic._currentEnteredPassword;

			if (hexString.Length > Seed.Length)
				return hexString.Substring(0, Seed.Length);
			if (hexString.Length == 0)
				return new string('0', Seed.Length);
			if (hexString.Length < Seed.Length)
 				 return hexString + new string('0', Seed.Length - hexString.Length);

			return hexString;
		}

		string GetSelectedMenuEntryText(int selectedMenuEntryIndex)
		{
			return ((IList)Dynamic.MenuEntries)[selectedMenuEntryIndex].AsDynamic().Text;
		}

		void SetSelectedMenuItemByIndex(int index)
		{
			((object)Dynamic._primaryMenuCollection).AsDynamic().SelectedIndex = index;
			Dynamic.OnSelectedEntryChanged(index);
		}
	}
}
