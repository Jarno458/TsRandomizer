using System;
using System.Globalization;

namespace TsRandomizer
{
	struct Seed
	{
		public const int Length = 8 + SeedOptions.Length;

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

		public override string ToString()
		{
			return Id.ToString("X8") + Options;
		}
	}

	struct SeedOptions
	{
		public const int Length = 2;

		public readonly uint Flags;

		public static SeedOptions None = new SeedOptions(0U);

		public bool StartWithJewelryBox => (Flags & 1) > 0;
		public bool ProgressiveVerticalMovement => (Flags & 2) > 0;
		public bool ProgressiveKeycard => (Flags & 4) > 0;

		public SeedOptions(uint flags)
		{
			Flags = flags;
		}

		public static bool TryParse(string seedString, out SeedOptions options)
		{
			if (seedString.Length == Seed.Length
			    && uint.TryParse(seedString.Substring(8, Length), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out var parsedOptionsKey))
			{
				options = new SeedOptions(parsedOptionsKey);
				return true;
			}

			options = None;
			return false;
		}

		public override string ToString()
		{
			return Flags.ToString($"X{Length}");
		}
	}
}
