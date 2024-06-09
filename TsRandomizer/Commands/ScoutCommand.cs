using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using TsRandomizer.Archipelago;
using TsRandomizer.Screens.Console;

namespace TsRandomizer.Commands
{
	class ScoutCommand : ConsoleCommand
	{
		public override string Command => "scout";

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

			void OnLocationScouted(Dictionary<long, ScoutedItemInfo> locationInfo)
			{
				var l = locationInfo.First().Value;

				console.AddLine($"Item {l.ItemId}, Player {l.Player}, Location {l.LocationId}, Flags {l.Flags}");
			}

			Client.LocationCheckHelper
				.ScoutLocationsAsync(HintCreationPolicy.CreateAndAnnounce, long.Parse(parameters[0]))
				.ContinueWith(t => OnLocationScouted(t.Result));

			return true;
		}
	}
}
