using System;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SDL2;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;
using TsRandomizer.Screens.SeedSelection;

namespace TsRandomizer.Screens.Archipelago
{
	class ServerAdressSelectionScreen : PasswordMenuOverride
	{
		static readonly Type PasswordMenuScreenType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.PauseMenu.Options.PasswordMenuScreen");
		static readonly Type MainMenuEntryType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.MenuEntry");
		static readonly Type InventoryItemIconType = TimeSpinnerType
			.Get("Timespinner.GameAbstractions.Inventory.EInventoryItemIcon");

		readonly GameDifficultyMenuScreen difficultyMenu;

		public ServerAdressSelectionScreen(ScreenManager screenManager, GameScreen passwordMenuScreen) : base(screenManager, passwordMenuScreen)
		{
			difficultyMenu = screenManager.FirstOrDefault<GameDifficultyMenuScreen>();
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			Dynamic._menuTitle = "Enter server url";

			ChangeAvailableButtons(
				MenuEntry.Create(".", _ => OnCharecterSelect('.')),
				MenuEntry.Create(":", _ => OnCharecterSelect(':')),
				MenuEntry.Create("\\", _ => OnCharecterSelect('\\')),
				MenuEntry.Create("OK", OnOkayEntrySelected));
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			if (input.IsControllHold())
			{
				if (input.IsKeyHold(Keys.V) && SDL.SDL_HasClipboardText() == SDL.SDL_bool.SDL_TRUE)
					GetClipboardValue();
				else if (input.IsKeyHold(Keys.C))
					SDL.SDL_SetClipboardText((string)Dynamic._currentEnteredPassword);
			}
		}

		void ChangeAvailableButtons(params MenuEntry[] extraButtons)
		{
			var entries = ((IList)Dynamic.MenuEntries)
				.Cast<object>()
				.Where(e => !IsOldOkButton(e))
				.Concat(extraButtons.Select(b => b.AsTimeSpinnerMenuEntry()))
				.ToList(MainMenuEntryType);

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = entries;
		}

		static bool IsOldOkButton(object menuEntry)
		{
			var reflected = menuEntry.AsDynamic();

			return reflected.Text == "OK"; //not a verry clean check but password menu isnt localised ¯\_(ツ)_/¯
		}

		void GetClipboardValue()
		{
			var text = SDL.SDL_GetClipboardText().Trim();

			Dynamic._currentEnteredPassword = text;
			Dynamic.RefreshDisplayPassword();

			const int numberOfDisplayDigits = 12;
			for (var i = Seed.Length; i < numberOfDisplayDigits; i++) //RefreshDisplayPassword() only blacks out a single charecter
				Dynamic._displayCharacters[i] = " ";
		}

		void OnOkayEntrySelected(PlayerIndex playerIndex)
		{
			//open player name selection
		}

		void OnCharecterSelect(char charecter)
		{

		}

		void ShowErrorDescription(string message)
		{
			Dynamic.ChangeDescription(message, InventoryItemIconType.GetEnumValue("None"));
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
