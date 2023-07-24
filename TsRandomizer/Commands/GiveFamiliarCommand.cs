using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Console;

namespace TsRandomizer.Commands
{
	class GiveFamiliarCommand : ConsoleCommand
	{
		readonly Func<Level> level;
		readonly Func<ItemLocationMap> itemLocationMap;
		public override string Command => "givefamiliar";

		public override string ParameterUsage => "<familiarName>";

		public GiveFamiliarCommand(Func<Level> level, Func<ItemLocationMap> itemLocationMap)
		{
			this.level = level;
			this.itemLocationMap = itemLocationMap;
		}

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length != 1 || parameters.Length > 2)
				return false;

			if (Enum.TryParse(parameters[0], true, out EInventoryFamiliarType familiar))
				level().GameSave.AddFamiliar(familiar);
			else
				console.AddLine($"Invalid familiar name, valid familiars are: {string.Join(", ", Enum.GetNames(typeof(EInventoryFamiliarType)))}");

			if (parameters.Length == 2 && bool.Parse(parameters[1]))
				itemLocationMap().AddCollected(new ItemIdentifier(familiar));

			return true;
		}
	}
}
