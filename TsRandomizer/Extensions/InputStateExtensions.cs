using Microsoft.Xna.Framework.Input;
using Timespinner.GameStateManagement.ScreenManager;

namespace TsRandomizer.Extensions
{
	static class InputStateExtensions
	{
		internal static bool IsKeyHold(this InputState input, Keys key) =>
			input.IsKeyHold(key, null, out _);

		internal static bool IsButtonHold(this InputState input, Buttons button) =>
			input.IsButtonHold(button, null, out _);

		internal static bool IsNewButtonPress(this InputState input, Buttons button) =>
			input.IsNewButtonPress(button, null, out _);

		internal static bool IsControllHold(this InputState input) =>
			input.IsKeyHold(Keys.LeftControl) || input.IsKeyHold(Keys.RightControl);

		internal static bool IsNewKeyPress(this InputState input, Keys key) =>
			input.IsNewKeyPress(key, null, out _);
	}
}
