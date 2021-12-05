using TsRandomizer.Screens;

namespace TsRandomizer.Commands
{
	abstract class ConsoleCommand
	{
		public abstract string Command { get; }

		public virtual string ParameterUsage => "";

		public string Usage => $"\t/{Command} {ParameterUsage}";

		public abstract bool Handle(GameConsole console, string[] parameters);
	}
}