using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;
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

		readonly GameDifficultyMenuScreen difficultyMenu;

		bool forceSeed;
		MenuEntry okButton;

		bool IsUsedAsSeedSelectionMenu => difficultyMenu != null;

		public static GameScreen Create(ScreenManager screenManager)
		{
			void Noop() { }

			return (GameScreen)Activator.CreateInstance(PasswordMenuScreenType, null, screenManager.Reflected.GCM, (Action)Noop);
		}

		public SeedSelectionMenuScreen(ScreenManager screenManager, GameScreen passwordMenuScreen) : base(screenManager, passwordMenuScreen)
		{
			difficultyMenu = screenManager.FirstOrDefault<GameDifficultyMenuScreen>();
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			if (!IsUsedAsSeedSelectionMenu)
				return;

			Reflected._menuTitle = "Select Seed";

			okButton = MenuEntry.Create("OK", OnOkayEntrySelected);

			var extraButtons = new[] {
				okButton.AsTimeSpinnerMenuEntry(),
				MenuEntry.Create("", () => {}, false).AsTimeSpinnerMenuEntry(),
				MenuEntry.Create("New", OnGenerateSelected).AsTimeSpinnerMenuEntry(),
				MenuEntry.Create("Options", OnOptionsSelected).AsTimeSpinnerMenuEntry()
			};

			ChangeAvailableButtons(extraButtons);
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			if (input.IsButtonHold(Buttons.RightTrigger, null, out _))
			{
				forceSeed = true;
				okButton.Text = "Force";
			}
			else
			{
				forceSeed = false;
				okButton.Text = "OK";
			}

			if (input.IsKeyHold(Keys.LeftControl, null, out _) || input.IsKeyHold(Keys.RightControl, null, out _))
			{
				if(input.IsKeyHold(Keys.V, null, out _) && Clipboard.ContainsText())
					GetClipboardSeed();
				else if (input.IsKeyHold(Keys.C, null, out _)) 
					Clipboard.SetText(GetHexString());
			}

			var selectedMenuEntryIndex = Reflected.SelectedIndex;
			if (GetSelectedMenuEntryText(selectedMenuEntryIndex) == "")
			{
				if (input.IsButtonHold(Buttons.LeftThumbstickLeft, null, out _))
					SetSelectedMenuItemByIndex(selectedMenuEntryIndex - 1);
				else
					SetSelectedMenuItemByIndex(selectedMenuEntryIndex + 1);
			}
		}

		void GetClipboardSeed()
		{
			var text = Clipboard.GetText().Trim();

			if (text.Length > Seed.Length)
				text = text.Substring(0, Seed.Length);

			SetSeed(text);
		}

		void ChangeAvailableButtons(object[] extraButtons)
		{
			var entries = ((IList)Reflected.MenuEntries)
				.Cast<object>()
				.Where(e => IsHex(e) || IsDelButton(e))
				.Concat(extraButtons)
				.ToList(MainMenuEntryType);

			((object)Reflected._primaryMenuCollection).AsDynamic()._entries = entries;
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

			difficultyMenu.SetSeed(seed);

			Reflected.OnCancel(playerIndex);
		}

		void ShowErrorDescription(string message)
		{
			var inventoryItemIconType = TimeSpinnerType.Get("Timespinner.GameAbstractions.Inventory.EInventoryItemIcon");
			Reflected.ChangeDescription(message, inventoryItemIconType.GetEnumValue("None"));
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
			Reflected._currentEnteredPassword = seedString;
			Reflected.RefreshDisplayPassword();

			for (var i = 10; i < 12; i++) //RefreshDisplayPassword() only blacks out a single charecter
				Reflected._displayCharacters[i] = " ";
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
			var hexString = (string)Reflected._currentEnteredPassword;

			if (hexString.Length > Seed.Length)
				return hexString.Substring(0, Seed.Length);
			if (hexString.Length == 0)
				return new string('0', Seed.Length);
			if (hexString.Length < Seed.Length)
 				 return hexString + new string('0', Seed.Length - hexString.Length);

			return hexString;
		}

		protected string GetSelectedMenuEntryText(int selectedMenuEntryIndex)
		{
			return ((IList)Reflected.MenuEntries)[selectedMenuEntryIndex].AsDynamic().Text;
		}

		protected void SetSelectedMenuItemByIndex(int index)
		{
			((object)Reflected._primaryMenuCollection).AsDynamic().SelectedIndex = index;
			Reflected.OnSelectedEntryChanged(index);
		}
	}
}
