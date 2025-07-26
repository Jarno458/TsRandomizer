using System;
using System.Globalization;

namespace TsRandomizer
{
	public enum Era
	{
		Past,
		Present,
		Pyramid
	}
	public enum Goal
	{
		Nightmare,
		DadPercent,
		Objectives
	}
	struct Seed
	{
		public const int Length = 8 + SeedOptions.Length;

		public readonly uint Id;
		public readonly SeedOptions Options;
		public readonly RisingTides FloodFlags;
		public readonly Era StartingEra;
		public readonly Goal GoalState;

		public static Seed Zero = new Seed(0U, SeedOptions.None);

		public Seed(uint id, SeedOptions options, RisingTides floodFlags = null)
		{
			Id = id;
			Options = options;
			FloodFlags = floodFlags ?? new RisingTides(id, options);
			StartingEra = Era.Present;
			if (options.PyramidStart)
				StartingEra = Era.Pyramid;
			else if (options.Inverted)
				StartingEra = Era.Past;
			GoalState = Goal.Nightmare;
			if (options.DadPercent)
				GoalState = Goal.DadPercent;
		}

		public static Seed GenerateRandom(SeedOptions options, Random random)
		{
			var id = (uint)random.Next();
			return new Seed(id, options);
		}

		public static bool TryParse(string seedString, RisingTides floodFlags, out Seed seed)
		{
			ExceptionLogger.SetSeedContext(seedString);

			if(seedString.Length == Length
				&& uint.TryParse(seedString.Substring(0, Length - SeedOptions.Length), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out var parsedSeedKey)
				&& SeedOptions.TryParse(seedString, out var options))
			{
				seed = new Seed(parsedSeedKey, options, floodFlags);
				return true;
			}

			seed = Zero;
			return false;
		}

		public override string ToString() =>
			Id.ToString($"X{Length - SeedOptions.Length}") + Options;
	}
}
