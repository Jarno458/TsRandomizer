using TsRandomizer.Archipelago;
using TsRandomizer.Screens.Console;

namespace TsRandomizer.Commands
{
	class HintPointsCommand : ConsoleCommand
	{
		public override string Command => "hintpoints";

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (!Client.IsConnected)
			{
				console.AddLine("Your not connected to archipelago");
				return true;
			}

			if (parameters.Length != 0)
				return false;
			
			console.AddLine($"Hints cost {Client.RoomState.HintCost} points. You have {Client.RoomState.HintPoints} points");

			return true;
		}
	}
}
