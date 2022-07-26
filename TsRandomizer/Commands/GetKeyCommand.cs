using Newtonsoft.Json.Linq;
using TsRandomizer.Archipelago;
using TsRandomizer.Screens;

namespace TsRandomizer.Commands
{
	class GetKeyCommand : ConsoleCommand
	{
		public override string Command => "getkey";

		public override string ParameterUsage => "<key>";

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length != 1)
				return false;

			if (!Client.IsConnected)
			{
				console.AddLine("Not connected to Archipelago.");
			}
			else
			{
				Client.DataStorage[parameters[0]].GetAsync().ContinueWith(v => {
					if (v.Result.Type == JTokenType.Array)
					{
						console.AddLine("[");
						foreach (var entry in v.Result)
							console.AddLine(entry.ToString());
						console.AddLine("]");
					}
					else
					{
						console.AddLine(v.ToString());
					}
				});
			}

			return true;
		}
	}
}
