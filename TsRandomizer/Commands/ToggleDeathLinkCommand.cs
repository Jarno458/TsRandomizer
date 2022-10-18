using System;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Screens;

namespace TsRandomizer.Commands
{
	class ToggleDeathLinkCommand : ConsoleCommand
	{
		readonly DeathLinkService deathLinkService;
		readonly Func<Level> level;
		public override string Command => "deathlink";

		public override string ParameterUsage => "<bool>";

		public ToggleDeathLinkCommand(DeathLinkService deathLinkService, Func<Level> level)
		{
			this.deathLinkService = deathLinkService;
			this.level = level;
		}

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length != 1)
				return false;

			if (bool.TryParse(parameters[0], out var result))
			{
				if (result)
					deathLinkService.EnableDeathLink();
				else
					deathLinkService.DisableDeathLink();

				return true;
			}

			console.AddLine("Invalid deathlink parameter, valid values are: true, false");

			return true;
		}
	}
}
