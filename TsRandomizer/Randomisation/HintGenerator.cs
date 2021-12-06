using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Extensions;
using static TsRandomizer.Randomisation.Gate;

namespace TsRandomizer.Randomisation
{
	class HintGenerator
    {
	    readonly ItemLocationMap itemLocations;
	    readonly Level level;

	    public HintGenerator(ItemLocationMap itemLocations, Level level)
	    {
		    this.itemLocations = itemLocations;
		    this.level = level;
	    }

		static int GetRandomSeedForRoom(Level level) 
	        => level.ID * 100 + level.RoomID + level.GameSave.GetSeed().GetHashCode();

		public string GetProgressionHint()
        {
            var locations = (List<ItemLocation>)itemLocations.ToList(typeof(ItemLocation));
            var requiredLocations = locations.FindAll(l => l.ItemInfo.IsProgression);
            var requiredLocation = requiredLocations.PopRandom(new Random(GetRandomSeedForRoom(level))); //same hint every time per location

            return $"{requiredLocation.AreaName}, {requiredLocation.Name} holds something useful.";
        }

        public string GetRandomItemHint()
        {
            //needs fancying up to look good, but it works
            var locations = (List<ItemLocation>)itemLocations.ToList(typeof(ItemLocation));
            var hintLocation = locations.PopRandom(new Random(GetRandomSeedForRoom(level)));

			return $"{hintLocation.Name} holds {hintLocation.ItemInfo.Identifier}.";
		}

        public string GetRequiredItemHint()
        {
            var explicitlyRequiredItemLocations = itemLocations.Where(l => l.ItemInfo.IsExplicitlyRequired);
            var randomRequiredItemLocation = explicitlyRequiredItemLocations.ToList().PopRandom(new Random(GetRandomSeedForRoom(level)));

            return $"The path to the Ancient Pyramid goes through {randomRequiredItemLocation.AreaName}, {randomRequiredItemLocation.Name}";
        }

        public string GetRequiredProgressionHint()
        {
            var randomSeed = GetRandomSeedForRoom(level);
            var chain = itemLocations.GetProgressionChain();
            var spheres = new List<ProgressionChain>();

            var buriedRequiredItemLocations = itemLocations.Where(l => l.ItemInfo.IsExplicitlyRequired && l.Gate.Gates != null);

            if (!buriedRequiredItemLocations.Any()) 
	            return "The path to the Ancient Pyramid is straight and narrow.";

            var randomRequiredItemLocation = buriedRequiredItemLocations.ToList().PopRandom(new Random(randomSeed));
            var finalGates = randomRequiredItemLocation.Gate.GetRequirementGates();

            while (chain != null)
            {
                spheres.Add(chain);
                chain = chain.Sub;
            }
            spheres.Reverse();

            var gatesFound = new List<string>();

            foreach (var sphere in spheres)
            {
                foreach (var location in sphere.Locations)
                {
                    foreach (var individualUnlock in location.ItemInfo.Unlocks.Split())
                    {
                        var requirementCheck = new RequirementGate(individualUnlock);
                        if (finalGates.Contains(requirementCheck))
                            gatesFound.Add($"{location.AreaName}, {location.Name}");
                    }
                }
            }

            return gatesFound.Count > 0 
	            ? $"One path to the Ancient Pyramid goes through {gatesFound.PopRandom(new Random(randomSeed))}" 
	            : "The path to the Ancient Pyramid is fraught with impossibility.";
        }
    }
}
