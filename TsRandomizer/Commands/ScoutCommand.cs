using System;
using System.Linq;
using Archipelago.MultiClient.Net.Packets;
using TsRandomizer.Archipelago;
using TsRandomizer.Screens;

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

			void OnLocationScouted(LocationInfoPacket locationInfo)
			{
				var l = locationInfo.Locations.First();

				console.AddLine($"Item {l.Item}, Player {l.Player}, Location {l.Location}, Flags {l.Flags}");
			}

			Client.LocationCheckHelper
				.ScoutLocationsAsync(long.Parse(parameters[0]))
				.ContinueWith(t => OnLocationScouted(t.Result));

			return true;
		}
	}
}
