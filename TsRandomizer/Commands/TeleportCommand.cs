using System;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Screens;

namespace TsRandomizer.Commands
{
	class TeleportCommand : ConsoleCommand
	{
		readonly Func<Level> level;
		public override string Command => "teleport";

		public override string ParameterUsage => "<levelId> <roomId>";

		public TeleportCommand(Func<Level> level)
		{
			this.level = level;
		}

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length != 2)
				return false;

			level().RequestChangeLevel(new LevelChangeRequest {
				LevelID = int.Parse(parameters[0]), RoomID = int.Parse(parameters[1])
			});

			console.Close();

			return true;
		}
	}
}
