using System.IO;
using System.Windows.Forms;

namespace TsRandomizerItemTracker
{
	class Program
	{
		static int Main(string[] args)
		{
			if (!File.Exists("Timespinner.exe"))
			{
				MessageBox.Show("Timespinner.exe not found in current directory\r\nPleaze place TsRandomizerItemTracker.exe in the same folder as the original game", "FileNotFound");
				return -1;
			}

			if (!File.Exists("TsRandomizer.exe"))
			{
				MessageBox.Show("TsRandomizer.exe not found in current directory\r\nPleaze place TsRandomizerItemTracker.exe in the same folder as the original game", "FileNotFound");
				return -1;
			}

			new ItemTracker().Run();

			return 0;
		}
	}
}
