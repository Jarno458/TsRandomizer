using System.Linq;
using NUnit.Framework;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class RandomizerFixture
	{
		[Test]
		public void Should_generate_single_beatable_seed()
		{
			var result = Randomizer.Generate(FillingMethod.Random, SeedOptions.None);
			Assert.That(Randomizer.IsBeatable(result.Seed, FillingMethod.Random), Is.True);
		}

		[Test]
		public void Should_generate_a_hundert_beatable_seeds()
		{
			var seeds = new LookupDictionairy<Seed, GenerationResult>(r => r.Seed);

			while (seeds.Count != 100)
			{
				var result = Randomizer.Generate(FillingMethod.Random, SeedOptions.None);

				if(!seeds.Contains(result.Seed))
					seeds.Add(result);
			}

			var longestGeneration = seeds.OrderByDescending(s => s.Elapsed).First();
			var validSeeds = string.Join("\n", seeds.Select(s => s.Seed));

			Assert.That(seeds, Is.Not.Empty);
		}
	}
}
