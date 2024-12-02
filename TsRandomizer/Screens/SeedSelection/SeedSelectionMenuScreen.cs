using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;
using SDL2;
using TsRandomizer.Settings;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace TsRandomizer.Screens.SeedSelection
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.Options.PasswordMenuScreen")]
	class SeedSelectionMenuScreen : Screen
	{
		static readonly Type PasswordMenuScreenType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.PauseMenu.Options.PasswordMenuScreen");
		static readonly Type MainMenuEntryType = TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntry");
		static readonly Type InventoryItemIconType = TimeSpinnerType.Get("Timespinner.GameAbstractions.Inventory.EInventoryItemIcon");

		readonly GameDifficultyMenuScreen difficultyMenu;

		GCM gcm;

		bool IsUsedAsSeedSelectionMenu => difficultyMenu != null;

		bool forceSeed;
		MenuEntry okButton;

		string password = "";

		public static GameScreen Create(ScreenManager screenManager)
		{
			void Noop() { }

			return (GameScreen)Activator.CreateInstance(PasswordMenuScreenType, null, screenManager.GameContentManager, (Action)Noop);
		}

		public SeedSelectionMenuScreen(ScreenManager screenManager, GameScreen passwordScreen) : base(screenManager,
			passwordScreen)
		{
			difficultyMenu = screenManager.FirstOrDefault<GameDifficultyMenuScreen>();
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			if (!IsUsedAsSeedSelectionMenu)
				return;

			if (difficultyMenu.Seed.HasValue)
				SetSeed(difficultyMenu.Seed.Value.ToString());

			gcm = gameContentManager;

			Dynamic._menuTitle = "Select Seed";
			Dynamic._displayCharacters = new string[Seed.Length];
			Dynamic._displayCharacterOrigins = new Vector2[Seed.Length];

			okButton = MenuEntry.Create("OK", OnOkayEntrySelected);

			ChangeAvailableButtons(
				MenuEntry.Create("DEL", OnDeleteCharacter),
				MenuEntry.Create("Copy", OnCopySelected),
				MenuEntry.Create("", () => { }, false),
				MenuEntry.Create("Paste", OnPasteSelected),
				MenuEntry.Create("Flags", OnOptionsSelected),
				MenuEntry.Create("", () => { }, false),
				MenuEntry.Create("Settings", OnSettingsSelected),
				MenuEntry.Create("", () => { }, false),
				okButton,
				MenuEntry.Create("", () => { }, false),
				MenuEntry.Create("New", OnGenerateSelected)
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
				if (input.IsKeyHold(Keys.V) && SDL.SDL_HasClipboardText() == SDL.SDL_bool.SDL_TRUE)
					PasteClipboardSeed();
				else if (input.IsKeyHold(Keys.C))
					CopyClipboardSeed();
			}

			var selectedMenuEntryIndex = Dynamic.SelectedIndex;
			if (GetSelectedMenuEntryText(selectedMenuEntryIndex) == "")
			{
				if (input.IsPressMenuLeft(null))
					SetSelectedMenuItemByIndex(selectedMenuEntryIndex - 1);
				else
					SetSelectedMenuItemByIndex(selectedMenuEntryIndex + 1);
			}

			if (GetSelectedMenuEntryText(selectedMenuEntryIndex) != "OK")
			{
				var defaultDescription = "Select a seed to play\nCtrl+C / Ctrl+V are supported\nPress $G to force play an unbeatable seed";
				Dynamic.ChangeDescription(defaultDescription, InventoryItemIconType.GetEnumValue("None"));
			}
		}

		void CopyClipboardSeed() => SDL.SDL_SetClipboardText(GetHexString());

		void PasteClipboardSeed()
		{
			var text = SDL.SDL_GetClipboardText().Trim();

			if (text.Length > Seed.Length)
				text = text.Substring(0, Seed.Length);
			if (text.Length < Seed.Length)
				text = new string('0', Seed.Length - text.Length) + text;

			SetSeed(text);
		}

		void ChangeAvailableButtons(params MenuEntry[] extraButtons)
		{
			char[] chars = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

			var lettersColor = (Color)PasswordMenuScreenType.GetPrivateStaticField("LettersDrawColor");
			var numbersColor = (Color)PasswordMenuScreenType.GetPrivateStaticField("NumbersDrawColor");

			var buttons = new List<MenuEntry>(16);

			foreach (var c in chars)
			{
				var menuEntry = MenuEntry.Create(c.ToString(), () => AddChar(c));
				menuEntry.BaseDrawColor = (c >= '0' &&c <= '9') ? numbersColor : lettersColor;

				buttons.Add(menuEntry);
			}

			var menuEntries = buttons
				.Concat(extraButtons)
				.Select(b => b.AsTimeSpinnerMenuEntry())
				.ToList(MainMenuEntryType);
			
			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = menuEntries;
		}

		void OnDeleteCharacter()
		{
			if (password.Length > 0)
				password = password.Remove(password.Length - 1);
			else
				Dynamic.PlayErrorSound();
		}

		void AddChar(char c)
		{
			if (password.Length < Seed.Length)
				password += c;
			else
				Dynamic.PlayErrorSound();
		}

		void OnOkayEntrySelected(PlayerIndex playerIndex)
		{
			var hexString = GetHexString();

			if (!Seed.TryParse(hexString, null, out var seed))
			{
				ShowErrorDescription("Invalid seed id, it is not a valid hexidecimal value.");
				return;
			}

			if (!forceSeed && !Randomizer.IsBeatable(seed, new SettingCollection(), FillingMethod.Random))
			{
				ShowErrorDescription("Invalid seed id, it cannot be beaten.\nPress $G to force play an unbeatable seed.");
				return;
			}

			difficultyMenu.SetSeedAndFillingMethod(seed, FillingMethod.Random, GameSettingsLoader.LoadSettingsFromFile());

			Dynamic.OnCancel(playerIndex);
		}

		void ShowErrorDescription(string message) => 
			Dynamic.ChangeDescription(message, InventoryItemIconType.GetEnumValue("None"));

		void OnOptionsSelected(PlayerIndex playerIndex)
		{
			var seedOptionsMenu = SeedOptionsMenuScreen.Create(ScreenManager, GetCurrentOptions());

			ScreenManager.AddScreen(seedOptionsMenu, playerIndex);
		}

		void OnSettingsSelected(PlayerIndex playerIndex)
		{
			var gameSettingsMenu = GameSettingsScreen.Create(ScreenManager);

			ScreenManager.AddScreen(gameSettingsMenu, playerIndex);
		}

		void OnCopySelected(PlayerIndex playerIndex) => CopyClipboardSeed();

		void OnPasteSelected(PlayerIndex playerIndex) => PasteClipboardSeed();

		internal void OnSeedOptionsUpdated(SeedOptionsCollection options)
		{
			var hexString = GetHexString();

			var seedId = hexString.Substring(0, Seed.Length - SeedOptions.Length);

			if (Seed.TryParse(seedId + options, null, out var seed))
				SetSeed(seed.ToString());
		}

		void SetSeed(string seedString) => password = seedString;

		void OnGenerateSelected()
		{
			var seed = Randomizer.Generate(FillingMethod.Random, GetCurrentOptions());

			ScreenManager.Console.AddLine($"Spend {seed.Itterations} itterations to generate seed {seed.Seed}, in {seed.Elapsed}");

			SetSeed(seed.Seed.ToString());
		}

		internal SeedOptionsCollection GetCurrentOptions()
		{
			var hexString = GetHexString();

			return !Seed.TryParse(hexString, null, out var seed)
				? new SeedOptionsCollection(SeedOptions.None)
				: new SeedOptionsCollection(seed.Options);
		}

		string GetHexString()
		{
			var hexString = password;

			if (hexString.Length == 0)
				return new string('0', Seed.Length);
			if (hexString.Length < Seed.Length)
				return hexString + new string('0', Seed.Length - hexString.Length);

			return hexString;
		}

		string GetSelectedMenuEntryText(int selectedMenuEntryIndex)
			=> ((IList)Dynamic.MenuEntries)[selectedMenuEntryIndex].AsDynamic().Text;

		void SetSelectedMenuItemByIndex(int index)
		{
			((object)Dynamic._primaryMenuCollection).AsDynamic().SelectedIndex = index;
			Dynamic.OnSelectedEntryChanged(index);
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			if (!IsUsedAsSeedSelectionMenu || !GameScreen.IsActive)
				return;

			using (spriteBatch.BeginUsing())
			{
				var zoom = (int)TimeSpinnerGame.Constants.InGameZoom;

				var yPosition = ScreenManager.SmallScreenRect.Y + 40 * zoom;

				DrawBackdrop(spriteBatch, yPosition, zoom);
				DrawSeedString(spriteBatch, menuFont, yPosition, zoom);
			}
		}

		void DrawBackdrop(SpriteBatch spriteBatch, int y, int zoom)
		{
			var screenSize = ScreenManager.SmallScreenRect;

			var thirtyPercentOfWith = screenSize.Width * 0.3f;

			var height = 16 * zoom;

			var position = new Rectangle((int)(screenSize.X + thirtyPercentOfWith), y, (int)(screenSize.Width - (2 * thirtyPercentOfWith)), height);

			spriteBatch.Draw(gcm.TxBlankSquare, position, null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 1);
		}

		void DrawSeedString(SpriteBatch spriteBatch, SpriteFont menuFont, int y, int zoom)
		{
			var screenSize = ScreenManager.SmallScreenRect;

			var x = screenSize.X + (screenSize.Width / 2f) - ((menuFont.MeasureString(password).X / 2f) * zoom);

			spriteBatch.DrawString(menuFont, password, new Vector2(x, y), Color.White, zoom);
		}
	}
}
