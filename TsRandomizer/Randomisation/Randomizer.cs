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
			var unlockingMap = new ItemUnlockingMap(seed);
			var itemInfoProvider = new ItemInfoProvider(seed.Options, unlockingMap);

			ItemLocationRandomizer randomizer;

			switch (fillingMethod)
			{
				case FillingMethod.Forward:
					randomizer = new ForwardFillingItemLocationRandomizer(seed, itemInfoProvider, unlockingMap);
					break;

				case FillingMethod.Random:
					randomizer = new FullRandomItemLocationRandomizer(seed, itemInfoProvider, unlockingMap);
					break;

				case FillingMethod.Archipelago:
					randomizer = new ArchipelagoItemLocationRandomizer(seed, itemInfoProvider, unlockingMap);
					break;

				default:
					throw new NotImplementedException($"filling method {fillingMethod} is not implemented");
			}

			return randomizer.GenerateItemLocationMap(progressionOnly);
		}

		public static GenerationResult Generate(FillingMethod fillingMethod, SeedOptions options)
		{
			var random = new Random();

			Seed seed;
			var itterations = 0;

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			do
			{
				itterations++;
				seed = Seed.GenerateRandom(options, random);
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

		public static bool IsBeatable(Seed seed, FillingMethod fillingMethod) =>
			Randomize(seed, fillingMethod, true).IsBeatable();
	}

	class GenerationResult : IEqualityComparer<GenerationResult>
	{
		public Seed Seed { get; set; }
		public int Itterations { get; set; }
		public TimeSpan Elapsed { get; set; }

		public override string ToString() => $"Seed: {Seed}, Itterations: {Itterations}, Elapsed: {Elapsed}";

		public bool Equals(GenerationResult a, GenerationResult b)
		{
			if (a == null && b == null)
				return true;
			if (a == null || b == null)
				return false;

			return a.Seed.Id == b.Seed.Id 
			       && a.Seed.Options.Flags == b.Seed.Options.Flags;
		}

		public int GetHashCode(GenerationResult obj) => obj.Seed.GetHashCode();
	}

	enum FillingMethod
	{
		Forward,
		Random,
		Archipelago
	}
}