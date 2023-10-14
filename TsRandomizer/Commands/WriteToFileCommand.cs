using System.IO;
using System.Reflection;
using TsRandomizer.Screens.Console;

namespace TsRandomizer.Commands
{
	class WriteToFileCommand : ConsoleCommand
	{
		public override string Command => "writetofile";

		public override string ParameterUsage => "<filename>";

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length > 2)
				return false;

			var fileName = parameters.Length == 1 
				? parameters[0] 
				: "ConsoleLog";

			var fullFilePath = GetFileName(fileName);

			File.WriteAllText(fullFilePath, console.GetText());

			console.AddLine($"Console log saved to: {fullFilePath}");

			return true;
		}

		static string GetFileName(string fileName)
		{
			var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			// ReSharper disable once AssignNullToNotNullAttribute
			return Path.Combine(directory, $"{fileName}.txt");
		}
	}
}
