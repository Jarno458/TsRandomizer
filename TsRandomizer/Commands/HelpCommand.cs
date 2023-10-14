using TsRandomizer.Screens.Console;

namespace TsRandomizer.Commands
{
	class HelpCommand : ConsoleCommand
	{
		public override string Command => "help";

		public override bool Handle(GameConsole console, string[] parameters)
		{
			console.AddLine("Available commands:");

			foreach (var command in console.Commands)
				console.AddLine(command.Usage);

			console.AddLine("More commands available server side under !help");

			return true;
		}
	}
}
