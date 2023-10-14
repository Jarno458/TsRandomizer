using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Console;

namespace TsRandomizer.Commands
{
	class GiveRelicCommand : ConsoleCommand
	{
		readonly Func<Level> level;
		readonly Func<ItemLocationMap> itemLocationMap;

		public override string Command => "giverelic";

		public override string ParameterUsage => "<relicName>";

		public GiveRelicCommand(Func<Level> level, Func<ItemLocationMap> itemLocationMap)
		{
			this.level = level;
			this.itemLocationMap = itemLocationMap;
		}

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length < 1 || parameters.Length > 2)
				return false;

			if (Enum.TryParse(parameters[0], true, out EInventoryRelicType relic))
				level().GameSave.AddRelic(relic);
			else
				console.AddLine($"Invalid relic name, valid relics are: {string.Join(", ", Enum.GetNames(typeof(EInventoryRelicType)))}");

			if (parameters.Length == 2 && bool.Parse(parameters[1]))
				itemLocationMap().AddCollected(new ItemIdentifier(relic));

			return true;
		}
	}
}
