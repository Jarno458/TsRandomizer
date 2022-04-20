using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TsRandomizer.Archipelago;
using TsRandomizer.Screens;

namespace TsRandomizer.Commands
{
	class ToggleDeathlinkCommand : ConsoleCommand
	{
		public override string Command => "ToggleDeathlink";
		public override string ParameterUsage => "<state?>";

		public override bool Handle(GameConsole console, string[] parameters)
		{
			

			if (!Client.IsConnected)
			{
				console.AddLine("Not connected to Archipelago.");
				return false;
			}

			var deathLinkEnabled = GameplayScreen.deathLinkService?.deathLinkEnabled ?? false;
			if (parameters.Length == 1)
			{
				if (!bool.TryParse(parameters[1], out var result))
					return false;
				deathLinkEnabled = result;

			}
			else if (parameters.Length != 0)
			{
				return false;
			}
			else
			{
				deathLinkEnabled = !deathLinkEnabled;
			}

			if (GameplayScreen.deathLinkService == null)
			{
				if (deathLinkEnabled)
				{
					GameplayScreen.deathLinkService = new DeathLinker(Client.GetDeathLinkService());
				}
			}
			else
			{
				GameplayScreen.deathLinkService.deathLinkEnabled = deathLinkEnabled;
				if (deathLinkEnabled)
				{
					Client.AddTag("DeathLink");
				}
				else
				{
					Client.RemoveTag("DeathLink");
				}
			}


			return true;
		}
	}
}
