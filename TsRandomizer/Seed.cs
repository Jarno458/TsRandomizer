using System;
using System.Globalization;

namespace TsRandomizer
{
	struct Seed
	{
		public const int Length = 8 + SeedOptions.Length;

		public readonly uint Id;
		public readonly SeedOptions Options;
		public readonly RandomFloodsFlags FloodFlags;

		public static Seed Zero = new Seed(0U, SeedOptions.None);

		public Seed(uint id, SeedOptions options)
		{
			Id = id;
			Options = options;
			FloodFlags = new RandomFloodsFlags(id, options);
		}

		public static Seed GenerateRandom(SeedOptions options, Random random) 
			=> new Seed((uint)random.Next(), options);

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
	}

	class RandomFloodsFlags
	{
		public bool BasementHigh { get; }
		public bool Basement { get; }
		public bool Xarion { get; }
		public bool Maw { get; }
		public bool PyramidShaft { get; }
		public bool BackPyramid { get; }
		public bool CastleMoat { get; }

		public RandomFloodsFlags(uint seedId, SeedOptions options)
		{
			if (!options.FloodBasement)
				return;

			var random = new Random(~(int)seedId);

			BasementHigh = random.Next() % 2 == 0;
			Basement = random.Next() % 3 == 0;
			Xarion = random.Next() % 3 == 0;
			Maw = random.Next() % 3 == 0;
			PyramidShaft = random.Next() % 3 == 0;
			BackPyramid = random.Next() % 3 == 0;
			CastleMoat = random.Next() % 3 == 0;

			CastleMoat = true;
		}
	}
}
