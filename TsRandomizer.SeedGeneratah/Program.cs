using System;
using System.IO;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer;
using TsRandomizer.Randomisation;
using TsRandomizer.ReplacementObjects;

namespace TsRandomizerSeedGeneratah
{
	class Program
	{
		const string GeneratedSeedsFileName = @"GeneratedSeeds.csv";

		static void Main(string[] args)
		{
			var seeds = ReadKnownSeeds();

			while (true)
			{
				var result = Randomizer.Generate(FillingMethod.Random, SeedOptions.None);

				if (seeds.Contains(result.Seed)) 
					continue;

				seeds.Add(result);

				WriteSeed(result);
			}
		}

		static LookupDictionary<Seed, GenerationResult> ReadKnownSeeds()
		{
			var seeds = new LookupDictionary<Seed, GenerationResult>(r => r.Seed);

			if (!File.Exists(GeneratedSeedsFileName))
				return seeds;

			var lines = File.ReadAllLines(GeneratedSeedsFileName);

			foreach (var line in lines)
			{
				var segments = line.Split(',');

				if (!Seed.TryParse(segments[0], out var seed)
					|| !int.TryParse(segments[1], out var itterations)
				    || !TimeSpan.TryParse(segments[1], out var time))
					continue;

				var result = new GenerationResult
				{
					Seed = seed,
					Itterations = itterations,
					Elapsed = time
				};

				seeds.Add(result);
			}

			return seeds;
		}

		static void WriteSeed(GenerationResult result)
		{
			var itenLocationMap = Randomizer.Randomize(result.Seed, FillingMethod.Random, GameSave.DemoSave, true);

			var item1 = itenLocationMap[new ItemKey(1, 1, 1528, 144)];
			var item2 = itenLocationMap[new ItemKey(1, 15, 264, 144)];
			var item3 = itenLocationMap[new ItemKey(1, 25, 296, 176)];
			var item4 = itenLocationMap[new ItemKey(1, 9, 600, 144 + TimespinnerWheel.YOffset)];

			File.AppendAllText(GeneratedSeedsFileName, $"{result.Seed},{result.Itterations},{result.Elapsed}\n");
		}
	}
}
