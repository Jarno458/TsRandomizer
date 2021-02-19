using System;
using System.IO;
using SDL2;

namespace TsRandomizerItemTracker
{
	class Program
	{
		static int Main(string[] args)
		{
			if (!File.Exists("Timespinner.exe"))
			{
				SDL.SDL_ShowSimpleMessageBox(
					SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
					"FileNotFound",
					"Timespinner.exe not found in current directory\r\nPleaze place TsRandomizerItemTracker.exe in the same folder as the original game",
					IntPtr.Zero
				);
				return -1;
			}

			if (!File.Exists("TsRandomizer.exe"))
			{
				SDL.SDL_ShowSimpleMessageBox(
					SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
					"FileNotFound",
					"TsRandomizer.exe not found in current directory\r\nPleaze place TsRandomizerItemTracker.exe in the same folder as the original game",
					IntPtr.Zero
				);
				return -1;
			}

			new ItemTracker().Run();

			return 0;
		}
	}
}
