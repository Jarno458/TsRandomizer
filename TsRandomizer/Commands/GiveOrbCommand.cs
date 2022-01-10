using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.Screens;

namespace TsRandomizer.Commands
{
	class GiveOrbCommand : ConsoleCommand
	{
		readonly Func<Level> level;
		public override string Command => "giveorb";

		public override string ParameterUsage => "<orbName>";

		public GiveOrbCommand(Func<Level> level)
		{
			this.level = level;
		}

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length != 1)
				return false;

			if (Enum.TryParse(parameters[0], true, out EInventoryOrbType orb))
				level().GameSave.AddOrb(orb, EOrbSlot.All);
			else
				console.AddLine($"Invalid orb name, valid orbs are: {string.Join(", ", Enum.GetNames(typeof(EInventoryOrbType)))}");

			return true;
		}
	}
}
