using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Console;

namespace TsRandomizer.Commands
{
	class GiveOrbCommand : ConsoleCommand
	{
		readonly Func<Level> level;
		Func<ItemLocationMap> itemLocationMap;

		public override string Command => "giveorb";

		public override string ParameterUsage => "<orbName> <include in state>";

		public GiveOrbCommand(Func<Level> level, Func<ItemLocationMap> itemLocationMap)
		{
			this.level = level;
			this.itemLocationMap = itemLocationMap;
		}

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length < 1 || parameters.Length > 2)
				return false;

			if (Enum.TryParse(parameters[0], true, out EInventoryOrbType orb))
				level().GameSave.AddOrb(orb, EOrbSlot.All);
			else
				console.AddLine($"Invalid orb name, valid orbs are: {string.Join(", ", Enum.GetNames(typeof(EInventoryOrbType)))}");

			if (parameters.Length == 2 && bool.Parse(parameters[1]))
			{
				var itemMap = itemLocationMap();
				itemMap.AddCollected(new ItemIdentifier(orb, EOrbSlot.Melee));
				itemMap.AddCollected(new ItemIdentifier(orb, EOrbSlot.Spell));
				itemMap.AddCollected(new ItemIdentifier(orb, EOrbSlot.Passive));
			}

			return true;
		}
	}
}
