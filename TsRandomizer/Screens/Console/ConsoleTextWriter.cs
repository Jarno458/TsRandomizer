using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

namespace TsRandomizer.Screens.Console
{
	class ConsoleTextWriter : TextWriter
	{
		readonly GameConsole console;
		readonly Color color;

		public override Encoding Encoding => Encoding.UTF8;

		public ConsoleTextWriter(GameConsole console, Color color)
		{
			this.console = console;
			this.color = color;
		}

		public override void Write(string value) => console.AddLine(value, color);
		public override void WriteLine(string value) => console.AddLine(value, color);
		public override void WriteLine() { }
	}
}
