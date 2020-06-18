using System;
using TsRanodmizer.IntermediateObjects;
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
				default:
					throw new NotImplementedException($"filling method {fillingMethod} is not implemented");
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