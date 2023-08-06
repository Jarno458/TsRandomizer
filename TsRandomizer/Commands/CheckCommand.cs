using TsRandomizer.Archipelago;
using TsRandomizer.Screens.Console;

namespace TsRandomizer.Commands
{
	class CheckCommand : ConsoleCommand
	{
		public override string Command => "check";

		public override string ParameterUsage => "<locationId>";

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (!Client.IsConnected)
			{
				console.AddLine("Your not connected to archipelago");
				return true;
			}

			if (parameters.Length != 1)
				return false;

			Client.LocationCheckHelper.CompleteLocationChecks(long.Parse(parameters[0]));

			return true;
		}
	}
}
