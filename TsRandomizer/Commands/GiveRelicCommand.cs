using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.Screens;

namespace TsRandomizer.Commands
{
	class GiveRelicCommand : ConsoleCommand
	{
		readonly Func<Level> level;
		public override string Command => "giverelic";

		public override string ParameterUsage => "<relicName>";

		public GiveRelicCommand(Func<Level> level)
		{
			this.level = level;
		}

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length != 1)
				return false;

			if (Enum.TryParse(parameters[0], true, out EInventoryRelicType relic))
				level().GameSave.AddRelic(relic);
			else
				console.AddLine($"Invalid relic name, valid relics are: {string.Join(", ", Enum.GetNames(typeof(EInventoryRelicType)))}");

			return true;
		}
	}
}
