using System;
using System.Collections.Generic;
using System.Diagnostics;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation.ItemPlacers;

namespace TsRandomizer.Randomisation
{
	class Randomizer
	{
		public static ItemLocationMap Randomize(Seed seed, FillingMethod fillingMethod, bool progressionOnly = false)
		{
			var itemInfoProvider = new ItemInfoProvider();
			var itemLocations = new ItemLocationMap(itemInfoProvider);
			var unlockingMap = new ItemUnlockingMap(itemInfoProvider, seed);

			itemInfoProvider.EnableProgressiveItems();

			switch (fillingMethod)
			{
				case FillingMethod.Forward:
					ForwardFillingItemLocationRandomizer.AddRandomItemsToLocationMap(seed, itemInfoProvider, unlockingMap, itemLocations, progressionOnly);
					break;

				case FillingMethod.Random:
					FullRandomItemLocationRandomizer.AddRandomItemsToLocationMap(seed, itemInfoProvider, unlockingMap, itemLocations, progressionOnly);
					break;

				default:
					throw new NotImplementedException($"filling method {fillingMethod} is not implemented");
			}

			return itemLocations;
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
				seed = new Seed(random.Next());
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

		public static bool IsBeatable(Seed seed, FillingMethod fillingMethod)
		{
			return Randomize(seed, fillingMethod, true).IsBeatable();
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