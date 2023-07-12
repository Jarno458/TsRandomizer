using System;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Screens.Console;

namespace TsRandomizer.Commands
{
	class InstaGibCommand : ConsoleCommand
	{
		readonly Func<Level> level;
		public override string Command => "instagib";

		public override string ParameterUsage => "<bool>";

		public InstaGibCommand(Func<Level> level)
		{
			this.level = level;
		}

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length != 1)
				return false;

			var enabled = bool.Parse(parameters[0]);

			level().GameSave.DataKeyBools["TS_INSTAGIB"] = enabled;

			return true;
		}
	}
}
