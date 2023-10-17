using System;
using System.Collections.Generic;
using System.Diagnostics;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.Archipelago;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.Randomisation.ItemPlacers;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.Randomisation
{
	class Randomizer
	{
		public static ItemLocationMap Randomize(
			Seed seed, SettingCollection settings, FillingMethod fillingMethod, GameSave saveGame, bool progressionOnly = false)
		{
			CustomItem.Initialize();

			switch (fillingMethod)
			{
				case FillingMethod.Random:
					var defaultUnlockingMap = new DefaultItemUnlockingMap(seed);
					var progressiveItemInfoProvider = new ProgressiveItemProvider(seed.Options, defaultUnlockingMap);

					return new FullRandomItemLocationRandomizer(seed, settings, progressiveItemInfoProvider, defaultUnlockingMap)
						.GenerateItemLocationMap(progressionOnly);

				case FillingMethod.Archipelago:
					var archipelagoUnlockingMap = new ArchipelagoUnlockingMap(seed, saveGame);
					var itemInfoProvider = new ItemInfoProvider(seed.Options, archipelagoUnlockingMap);

					return new ArchipelagoItemLocationRandomizer(seed, itemInfoProvider, archipelagoUnlockingMap, saveGame)
						.GenerateItemLocationMap(progressionOnly);

				default:
					throw new NotImplementedException($"filling method {fillingMethod} is not implemented");
			}
		}

		public static GenerationResult Generate(FillingMethod fillingMethod, SeedOptions options)
		{
			var random = new Random();

			Seed seed;
			var settings = new SettingCollection();
			var itterations = 0;

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			do
			{
				itterations++;
				seed = Seed.GenerateRandom(options, random);
			} while (!IsBeatable(seed, settings, fillingMethod));

			stopwatch.Stop();

			return new GenerationResult
			{
				Seed = seed,
				Itterations = itterations,
				Elapsed = stopwatch.Elapsed
			};
		}

		public static bool IsBeatable(Seed seed, SettingCollection settings, FillingMethod fillingMethod) =>
			Randomize(seed, settings, fillingMethod, null, true).IsBeatable();
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