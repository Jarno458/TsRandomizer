using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago;
using TsRandomizer.Commands;
using TsRandomizer.Extensions;

namespace TsRandomizer.Screens
{
	class GameConsole : GameScreen
	{
		const int LinesToRender = 28;
		const int PageSize = 12;

		readonly ScreenManager screenManager;
		readonly GCM gcm;

		string commandText = "";
		string commandTextToRender = "";

		int commandIndex;
		GameTime gameTime;

		int scrollOffset;
		MouseState previousMouseState;

		readonly List<string> previousCommands = new List<string>();

		readonly ConcurrentQueue<Message> websocketBuffer = new ConcurrentQueue<Message>();
		readonly List<Message> lines = new List<Message>();

		readonly LookupDictionary<string, ConsoleCommand> commands = new LookupDictionary<string, ConsoleCommand>(c => c.Command);

		public GameConsole(ScreenManager screenManager, GCM gcm)
		{
			this.screenManager = screenManager;
			this.gcm = gcm;

			AddCommand(new HelpCommand());
		}

		public void AddCommand(ConsoleCommand command) => commands.AddOrUpdate(command);

		public override void Update(GameTime gameTime, bool doesOtherScreenHasFocus, bool isCoveredByOtherScreen)
		{
			base.Update(gameTime, doesOtherScreenHasFocus, isCoveredByOtherScreen);

			while (websocketBuffer.TryDequeue(out var message))
				lines.Add(message);

			this.gameTime = gameTime;
		}

		public override void HandleInput(InputState input)
		{
			if (input.IsNewKeyPress(Keys.OemTilde) && (!input.CurrentKeyboardStates[0].IsKeyDown(Keys.LeftShift)
			                                           || !input.CurrentKeyboardStates[0].IsKeyDown(Keys.RightShift)))
				return;

			if (input.IsControllHold() && input.IsNewKeyPress(Keys.V) && SDL.SDL_HasClipboardText() == SDL.SDL_bool.SDL_TRUE)
				commandText += SDL.SDL_GetClipboardText().Trim();
			else if (input.IsNewKeyPress(Keys.Back) && commandText.Length > 0)
				commandText = commandText.Remove(commandText.Length - 1);
			else if (input.IsNewKeyPress(Keys.Enter))
				HandleCommand();
			else if (input.IsNewKeyPress(Keys.Up))
				PreviousCommand();
			else if (input.IsNewKeyPress(Keys.Down))
				NextCommand();
			else if (input.IsNewKeyPress(Keys.PageUp) && lines.Count > PageSize + scrollOffset)
				scrollOffset += PageSize;
			else if (input.IsNewKeyPress(Keys.PageDown) && scrollOffset > 0)
				if (scrollOffset - PageSize < 0)
					scrollOffset = 0;
				else
					scrollOffset -= PageSize;
			else
				commandText += GetInputFromKeyboard(input);

			HandleScrollWheel();

			commandTextToRender = commandText;

			if (gameTime.TotalGameTime.Milliseconds < 500)
				commandTextToRender += "|";
		}

		void HandleScrollWheel()
		{
			var state = Mouse.GetState();

			var scrolledAmount = previousMouseState.ScrollWheelValue - state.ScrollWheelValue;
			scrolledAmount =- scrolledAmount;

			if (scrolledAmount > 0 && lines.Count > PageSize + scrollOffset)
				scrollOffset += PageSize;
			else if (scrolledAmount < 0)
			{
				if (scrollOffset - PageSize < 0)
					scrollOffset = 0;
				else
					scrollOffset -= PageSize;
			}

			previousMouseState = state;
		}

		void HandleCommand()
		{
			try
			{
				if (commandText.StartsWith("/"))
					HandleLocalCommand();
				else
					HandleSay();

				previousCommands.Add(commandText);
				commandIndex = previousCommands.Count;
				commandText = "";
			}
			catch (Exception e)
			{
				AddLine($"ERROR: {e.Message}", Color.Red);

				foreach (var line in e.StackTrace.Split('\n'))
					AddLine(line, Color.Red);
			}
		}

		void HandleSay()
		{
			if(!Client.IsConnected)
				AddLine("Not connected to an Archipelago server");
			else
				Client.Say(commandText);
		}

		void HandleLocalCommand()
		{
			AddLine(commandText);

			var slashRemoved = commandText.Substring(1);
			var parts = slashRemoved.Split(' ');

			if (commands.TryGetValue(parts[0], out var command))
			{
				var handled = command.Handle(this, parts.Skip(1).ToArray());

				if(!handled)
					AddLine(command.Usage, Color.Orange);
			}
			else
			{
				AddLine("Unknown command: parts[0]");
			}
		}

		void PreviousCommand()
		{
			if (commandIndex <= 0)
				return;

			commandIndex--;
			commandText = previousCommands[commandIndex];
		}

		void NextCommand()
		{
			if (commandIndex >= previousCommands.Count - 1) 
				return;

			commandIndex++;
			commandText = previousCommands[commandIndex];
		}

		public void AddLine(string message) 
			=> AddLine(message, Color.White);

		public void AddLine(string message, Color color)
		{
			foreach (var m in message.Split('\n'))
				lines.Add(new Message(new[] { new Part(m, color) }));
		}

		public void Add(params Part[] parts) 
			=> websocketBuffer.Enqueue(new Message(parts));

		public void Close() => screenManager.ToggleConsole();
		
		static string GetInputFromKeyboard(InputState input)
		{
			var keys = input.CurrentKeyboardStates[0].GetPressedKeys();

			var shift = input.CurrentKeyboardStates[0].IsKeyDown(Keys.LeftShift)
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
					case Keys.Tab: text += '\t'; break;
				}
			}

			return text;
		}

		public override void Draw(GameTime gameTime)
		{
			var spriteBatch = ScreenManager.SpriteBatch;
			var gameplayScreenSize = ScreenManager.SmallScreenRect;

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
			{
				DrawBackdrop(spriteBatch, gameplayScreenSize);

				var lineHeight = gcm.LatinFont.LineSpacing * (int)TimeSpinnerGame.Constants.InGameZoom * 0.5f;

				for (var i = 1; i <= LinesToRender; i++)
				{
					var point = new Point(gameplayScreenSize.X, (int)(gameplayScreenSize.Height - lineHeight / 2 - lineHeight * (i + 1)));

					if (lines.Count - (i + scrollOffset) >= 0)
						DrawMessage(spriteBatch, point, lines[lines.Count - (i + scrollOffset)]);
				}

				DrawCommandText(spriteBatch, gameplayScreenSize, lineHeight);
			}
		}

		void DrawCommandText(SpriteBatch spriteBatch, Rectangle screenSize, float lineHeight)
		{
			var point = new Point(screenSize.X, (int)(screenSize.Height - lineHeight / 2 - lineHeight));

			DrawMessage(spriteBatch, point, commandTextToRender);
		}

		void DrawBackdrop(SpriteBatch spriteBatch, Rectangle screenSize)
		{
			var backdropColor = Color.Black;
			backdropColor.A = 200;

			var origin = new Vector2(0, 0);

			spriteBatch.Draw(gcm.TxBlankSquare, screenSize, null, backdropColor, 0, origin, SpriteEffects.None, 1);
		}

		void DrawMessage(SpriteBatch spriteBatch, Point drawPoint, Message message)
		{
			var font = gcm.LatinFont;

			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;
			float partOffset = 0;

			foreach (var part in message.Parts)
			{
				var position = new Vector2(drawPoint.X + partOffset, drawPoint.Y);
				var color = part.Color;

				spriteBatch.DrawString(font, part.Text, position, color, inGameZoom * 0.5f);

				partOffset += font.MeasureString(part.Text).X * inGameZoom * 0.5f;
			}
		}

		void DrawMessage(SpriteBatch spriteBatch, Point drawPoint, string text)
		{
			var font = gcm.LatinFont;

			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;

			var position = new Vector2(drawPoint.X, drawPoint.Y);
			var color = Color.White;

			spriteBatch.DrawString(font, text, position, color, inGameZoom * 0.5f);
		}

		class HelpCommand : ConsoleCommand
		{
			public override string Command => "help";

			public override bool Handle(GameConsole console, string[] parameters)
			{
				console.AddLine("Available commands:");
				foreach (var command in console.commands)
					console.AddLine(command.Usage);
				console.AddLine("More commands available server side under !help");

				return true;
			}
		}
	}
}
