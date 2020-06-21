using System;
using System.Collections.Generic;
using System.Diagnostics;
using TsRanodmizer.Randomisation.ItemPlacers;

namespace TsRanodmizer.Randomisation
{
	class Randomizer
	{
		public static ItemLocationMap Randomize(Seed seed, FillingMethod fillingMethod)
		{
			switch (fillingMethod)
			{
				case FillingMethod.Forward:
					{
						var itemLocations = new ItemLocationMap();
						var unlockingMap = new ItemUnlockingMap(seed);

						ForwardFillingItemLocationRandomizer.AddRandomItemsToLocationMap(seed, unlockingMap, itemLocations);

						return itemLocations;
					}
				case FillingMethod.Random:
					{
						var itemLocations = new ItemLocationMap();
						var unlockingMap = new ItemUnlockingMap(seed);

						FullRandomItemLocationRandomizer.AddRandomItemsToLocationMap(seed, unlockingMap, itemLocations);

						return itemLocations;
					}
				default:
					throw new NotImplementedException($"filling method {fillingMethod} is not implemented");
			}
		}

		public static GenerationResult Generate(FillingMethod fillingMethod)
		{
			var random = new Random();

			Seed seed;
			var itterations = 0;

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			do
			{
				itterations++;
				seed = SelectSeed(random);
			} while (!IsBeatable(seed, fillingMethod));

			stopwatch.Stop();

			Console.Out.WriteLine($"Spend {itterations} itterations to generate seed {seed}, in {stopwatch.Elapsed}");

			return new GenerationResult
			{
				Seed = seed,
				Itterations = itterations,
				Elapsed = stopwatch.Elapsed
			};
		}

		static Seed SelectSeed(Random random)
		{
			//Seed.TrySetFromHexString("18B1A83B", out Seed seed);
			//return seed;
			return new Seed(random.Next());
		}

		public static bool IsBeatable(Seed seed, FillingMethod fillingMethod)
		{
			return Randomize(seed, fillingMethod).IsBeatable();
		}
	}

	class GenerationResult
	{
		public Seed Seed { get; set; }
		public int Itterations { get; set; }
		public TimeSpan Elapsed { get; set; }

		public override string ToString()
		{
			return $"Seed: {Seed}, Itterations: {Itterations}, Elapsed: {Elapsed}";
		}

		internal class Comparer : IEqualityComparer<GenerationResult>
		{
			public bool Equals(GenerationResult a, GenerationResult b)
			{
				if (a == null && b == null)
					return true;
				if (a == null || b == null)
					return false;

				return a.Seed == b.Seed;
			}

			public int GetHashCode(GenerationResult obj)
			{
				return obj.Seed.GetHashCode();
			}
		}
	}

	enum FillingMethod
	{
		Forward,
		Assumption,
		Random
	}
}