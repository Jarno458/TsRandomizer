using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.Screens;

namespace TsRandomizer.Commands
{
	class GiveFamiliarCommand : ConsoleCommand
	{
		readonly Func<Level> level;
		public override string Command => "givefamiliar";

		public override string ParameterUsage => "<familiarName>";

		public GiveFamiliarCommand(Func<Level> level)
		{
			this.level = level;
		}

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length != 1)
				return false;

			if (Enum.TryParse(parameters[0], true, out EInventoryFamiliarType familiar))
				level().GameSave.AddFamiliar(familiar);
			else
				console.AddLine($"Invalid familiar name, valid familiars are: {string.Join(", ", Enum.GetNames(typeof(EInventoryFamiliarType)))}");

			return true;
		}
	}
}
