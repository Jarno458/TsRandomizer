using System;
using System.Globalization;

namespace TsRandomizer
{
	struct Seed
	{
		public const int Length = 8 + SeedOptions.Length;
		public const int DisplayLength = 12;

		public readonly uint Id;
		public readonly SeedOptions Options;

		public static Seed Zero = new Seed(0U, SeedOptions.None);

		public Seed(uint id, SeedOptions options)
		{
			Id = id;
			Options = options;
		}

		public static Seed GenerateRandom(SeedOptions options, Random random)
		{
			return new Seed((uint)random.Next(), options);
		}

		public static bool TryParse(string seedString, out Seed seed)
		{
			ExceptionLogger.SetSeedContext(seedString);

			if(seedString.Length == Length
				&& uint.TryParse(seedString.Substring(0, Length - SeedOptions.Length), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out var parsedSeedKey)
				&& SeedOptions.TryParse(seedString, out var options))
			{
				seed = new Seed(parsedSeedKey, options);
				return true;
			}

			seed = Zero;
			return false;
		}

		public override string ToString() =>
			Id.ToString($"X{Length - SeedOptions.Length}") + Options;

		public string ToDisplayString() =>
			Id.ToString($"X{Length - SeedOptions.Length}") + Options.ToDisplayString();
	}
}
