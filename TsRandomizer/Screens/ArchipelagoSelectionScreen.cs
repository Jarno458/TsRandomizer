using System;
using System.Linq;
using Archipelago.MultiClient.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using SDL2;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Randomisation.ItemPlacers;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.OptionsMenuScreen")]
	class ArchipelagoSelectionScreen : Screen
	{
		const string ServerPrefix = "Server: ";
		const string UserPrefix = "User: ";
		const string PasswordPrefix = "Password: ";

		const int ServerIndex = 0;
		const int UserIndex = 1;
		const int PasswordIndex = 2;

		static readonly Type VideoMenuScreen = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.PauseMenu.OptionsMenuScreen");
		static readonly Type MainMenuEntryType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.MenuEntry");

		readonly string[] values = new string[3];

		MenuEntry serverMenuEntry;
		MenuEntry userMenuEntry;
		MenuEntry passwordMenuEntry;
		MenuEntry connectMenuEntry;

		readonly GameDifficultyMenuScreen difficultyMenu;

		bool IsUsedAsArchipelagoSelectionMenu => difficultyMenu != null;

		public static GameScreen Create(ScreenManager screenManager)
		{
			void Noop() { }

			var gameConfig = GameConfigSave.EditorSave;

			return (GameScreen)Activator.CreateInstance(VideoMenuScreen, null, gameConfig, screenManager.Reflected.GCM, (Action)Noop);
		}

		public ArchipelagoSelectionScreen(ScreenManager screenManager, GameScreen videoScreen) : base(screenManager, videoScreen)
		{
			difficultyMenu = screenManager.FirstOrDefault<GameDifficultyMenuScreen>();

#if DEBUG
			values[ServerIndex] = "localhost:38281";
#else
			values[ServerIndex] = "archipelago.gg:";
#endif
			values[UserIndex] = "";
			values[PasswordIndex] = "";
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			if (!IsUsedAsArchipelagoSelectionMenu)
				return;

			Dynamic._menuTitle = "Enter Credentails";

			serverMenuEntry = MenuEntry.Create(ServerPrefix, _ => { });
			userMenuEntry = MenuEntry.Create(UserPrefix, _ => { });
			passwordMenuEntry = MenuEntry.Create(PasswordPrefix, _ => { });
			connectMenuEntry = MenuEntry.Create("Connect", OnConnectEntrySelected, false);

			ChangeAvailableButtons(serverMenuEntry, userMenuEntry, passwordMenuEntry, connectMenuEntry);
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			if (!IsUsedAsArchipelagoSelectionMenu)
				return;

			connectMenuEntry.BaseDrawColor = !string.IsNullOrEmpty(values[ServerIndex]) && !string.IsNullOrEmpty(values[UserIndex])
				? MenuEntry.UnSelectedColor
				: MenuEntry.UnAvailableColor;

			int selectedIndex = ((object)Dynamic._primaryMenuCollection).AsDynamic().SelectedIndex;

			if (selectedIndex > PasswordIndex)
				return;

			if (input.IsControllHold() && input.IsNewKeyPress(Keys.V) && SDL.SDL_HasClipboardText() == SDL.SDL_bool.SDL_TRUE)
				values[selectedIndex] += SDL.SDL_GetClipboardText().Trim();
			else if (input.IsNewKeyPress(Keys.Back))
			{
				if (values[selectedIndex].Length > 0)
					values[selectedIndex] = values[selectedIndex].Remove(values[selectedIndex].Length - 1);
			}
			else if (input.IsNewKeyPress(Keys.Enter))
			{
				//if (selectedIndex < 3)
				//	SetSelectedMenuItemByIndex(selectedIndex++);
			}
			else
				values[selectedIndex] += GetInputFromKeyboard(input);

			serverMenuEntry.Text = ServerPrefix + values[ServerIndex];
			userMenuEntry.Text = UserPrefix + values[UserIndex];
			passwordMenuEntry.Text = PasswordPrefix + values[PasswordIndex];

			BlinkActiveTextInput(gameTime, selectedIndex);
		}

		void BlinkActiveTextInput(GameTime gameTime, int selectedIndex)
		{
			if (gameTime.TotalGameTime.Milliseconds < 500)
			{
				switch (selectedIndex)
				{
					case ServerIndex:
						serverMenuEntry.Text += "|";
						break;
					case UserIndex:
						userMenuEntry.Text += "|";
						break;
					case PasswordIndex:
						passwordMenuEntry.Text += "|";
						break;
				}
			}
		}

		void ChangeAvailableButtons(params MenuEntry[] newMenuEntries)
		{
			foreach (var entry in newMenuEntries)
			{
				entry.IsCenterAligned = false;
				entry.DoesDrawLargeShadow = false;
			}

			((object)Dynamic._primaryMenuCollection).AsDynamic()._entries = newMenuEntries
				.Select(b => b.AsTimeSpinnerMenuEntry())
				.ToList(MainMenuEntryType);
		}

		protected void SetSelectedMenuItemByIndex(int index)
		{
			((object)Dynamic._primaryMenuCollection).AsDynamic().SelectedIndex = index;
			Dynamic.OnSelectedEntryChanged(index);
		}

		void OnConnectEntrySelected(PlayerIndex playerIndex)
		{
			var server = "ws://" + values[ServerIndex];
			if (!values[ServerIndex].Contains(":"))
				server += ":38281";

			var password = string.IsNullOrEmpty(values[PasswordIndex]) ? null : values[PasswordIndex];

			var result = Client.Connect(server, values[UserIndex], password, null);
			if (!result.Successful)
			{
				var failure = (LoginFailure)result;

				var messageBox = MessageBox.Create(ScreenManager, $"Connecting to server failed: {string.Join(", ", failure.Errors)}");

				ScreenManager.AddScreen(messageBox.Screen, GameScreen.ControllingPlayer);
			}
			else
			{
				var connected = (LoginSuccessful)result;

				var slotDataParser = new SlotDataParser(connected.SlotData);

				difficultyMenu.SetSeedAndFillingMethod(slotDataParser.GetSeed(), FillingMethod.Archipelago);
				difficultyMenu.HookOnDifficultySelected(saveGame => {
					saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSaveServerKey] = server; 
					saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSaveUserKey] = values[UserIndex]; 
					if(!string.IsNullOrEmpty(values[PasswordIndex]))
						saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSavePasswordKey] = values[PasswordIndex];
					saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSaveConnectionId] = Client.ConnectionId;
					saveGame.DataKeyInts[ArchipelagoItemLocationMap.GameItemIndex] = 0;
					saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSavePyramidsKeysUnlock] = slotDataParser.GetPyramidKeysGate().ToString();
					saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSavePersonalItemIds] =
						JsonConvert.SerializeObject(slotDataParser.GetPersonalItems());
				});

				Dynamic.OnCancel(playerIndex);
			}
		}

		public static string GetInputFromKeyboard(InputState input)
		{
			Keys[] keys = input.CurrentKeyboardStates[0].GetPressedKeys();

			bool shift = input.CurrentKeyboardStates[0].IsKeyDown(Keys.LeftShift)
						 || input.CurrentKeyboardStates[0].IsKeyDown(Keys.RightShift);

			var text = "";

			foreach (var pressedKey in keys)
			{
				if (!input.IsNewKeyPress(pressedKey))
					continue;

				switch (pressedKey)
				{
					//Alphabet keys
					case Keys.A: text += shift ? 'A' : 'a'; break;
					case Keys.B: text += shift ? 'B' : 'b'; break;
					case Keys.C: text += shift ? 'C' : 'c'; break;
					case Keys.D: text += shift ? 'D' : 'd'; break;
					case Keys.E: text += shift ? 'E' : 'e'; break;
					case Keys.F: text += shift ? 'F' : 'f'; break;
					case Keys.G: text += shift ? 'G' : 'g'; break;
					case Keys.H: text += shift ? 'H' : 'h'; break;
					case Keys.I: text += shift ? 'I' : 'i'; break;
					case Keys.J: text += shift ? 'J' : 'j'; break;
					case Keys.K: text += shift ? 'K' : 'k'; break;
					case Keys.L: text += shift ? 'L' : 'l'; break;
					case Keys.M: text += shift ? 'M' : 'm'; break;
					case Keys.N: text += shift ? 'N' : 'n'; break;
					case Keys.O: text += shift ? 'O' : 'o'; break;
					case Keys.P: text += shift ? 'P' : 'p'; break;
					case Keys.Q: text += shift ? 'Q' : 'q'; break;
					case Keys.R: text += shift ? 'R' : 'r'; break;
					case Keys.S: text += shift ? 'S' : 's'; break;
					case Keys.T: text += shift ? 'T' : 't'; break;
					case Keys.U: text += shift ? 'U' : 'u'; break;
					case Keys.V: text += shift ? 'V' : 'v'; break;
					case Keys.W: text += shift ? 'W' : 'w'; break;
					case Keys.X: text += shift ? 'X' : 'x'; break;
					case Keys.Y: text += shift ? 'Y' : 'y'; break;
					case Keys.Z: text += shift ? 'Z' : 'z'; break;

					//Decimal keys
					case Keys.D0: text += shift ? ')' : '0'; break;
					case Keys.D1: text += shift ? '!' : '1'; break;
					case Keys.D2: text += shift ? '@' : '2'; break;
					case Keys.D3: text += shift ? '#' : '3'; break;
					case Keys.D4: text += shift ? '$' : '4'; break;
					case Keys.D5: text += shift ? '%' : '5'; break;
					case Keys.D6: text += shift ? '^' : '6'; break;
					case Keys.D7: text += shift ? '&' : '7'; break;
					case Keys.D8: text += shift ? '*' : '8'; break;
					case Keys.D9: text += shift ? '(' : '9'; break;

					//Decimal numpad keys
					case Keys.NumPad0: text += '0'; break;
					case Keys.NumPad1: text += '1'; break;
					case Keys.NumPad2: text += '2'; break;
					case Keys.NumPad3: text += '3'; break;
					case Keys.NumPad4: text += '4'; break;
					case Keys.NumPad5: text += '5'; break;
					case Keys.NumPad6: text += '6'; break;
					case Keys.NumPad7: text += '7'; break;
					case Keys.NumPad8: text += '8'; break;
					case Keys.NumPad9: text += '9'; break;

					//Special keys
					case Keys.OemTilde: text += shift ? '~' : '`'; break;
					case Keys.OemSemicolon: text += shift ? ':' : ';'; break;
					case Keys.OemQuotes: text += shift ? '"' : '\''; break;
					case Keys.OemQuestion: text += shift ? '?' : '/'; break;
					case Keys.OemPlus: text += shift ? '+' : '='; break;
					case Keys.OemPipe: text += shift ? '|' : '\\'; break;
					case Keys.OemPeriod: text += shift ? '>' : '.'; break;
					case Keys.OemOpenBrackets: text += shift ? '{' : '['; break;
					case Keys.OemCloseBrackets: text += shift ? '}' : ']'; break;
					case Keys.OemMinus: text += shift ? '_' : '-'; break;
					case Keys.OemComma: text += shift ? '<' : ','; break;
					case Keys.Space: text += ' '; break;
				}
			}

			return text;
		}
	}
}
