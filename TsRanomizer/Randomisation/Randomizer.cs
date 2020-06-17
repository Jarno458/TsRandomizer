using System;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation.ItemPlacers;

namespace TsRanodmizer.Randomisation
{
	class Randomizer
	{
		readonly IGameSaveDataAccess gameSaveDataAccess;
		readonly FillingMethod fillingMethod;
		readonly Seed seed;

		public Randomizer(IGameSaveDataAccess gameSaveDataAccess, Seed seed, FillingMethod fillingMethod)
		{
			this.gameSaveDataAccess = gameSaveDataAccess;
			this.seed = seed;
			this.fillingMethod = fillingMethod;
		}

		public ItemLocationMap Randomize()
		{
			switch (fillingMethod)
			{
				case FillingMethod.Forward:
					{
						var itemLocations = new ItemLocationMap(gameSaveDataAccess);
						var unlockingMap = new ItemUnlockingMap(seed);

						ForwardFillingItemLocationRandomizer.AddRandomItemsToLocationMap(seed, unlockingMap, itemLocations);

						return itemLocations;
					}
				default:
					throw new NotImplementedException($"filling method {fillingMethod} is not implemented");
			}
		}

		public bool IsBeatable()
		{
			var itemLocations = Randomize();

			//TODO loop over accesable location look all and see if we reach end and repeat


			return true;
		}
	}

	enum FillingMethod
	{
		Forward,
		Assumption,
		Random
	}
}