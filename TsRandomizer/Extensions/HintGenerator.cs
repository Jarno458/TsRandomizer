using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Randomisation;
using static TsRandomizer.Randomisation.Gate;

namespace TsRandomizer.Extensions
{
    partial class LevelExtensions
    {
        private static int GetRandomSeedForRoom(Level level)
        {
            return level.ID * 100 + level.RoomID + level.GameSave.GetSeed().GetHashCode();
        }

        internal static string GetProgressionHint(ItemLocationMap itemLocations, Level level)
        {
            List<ItemLocation> locations = (List<ItemLocation>)itemLocations.ToList(typeof(ItemLocation));
            List<ItemLocation> requiredLocations = locations.FindAll(l => l.ItemInfo.IsProgression);
            ItemLocation requiredLocation = requiredLocations.PopRandom(new Random(GetRandomSeedForRoom(level))); //same hint every time per location
            string hint = string.Format("{0}, {1} holds something useful.", requiredLocation.AreaName, requiredLocation.Name);
            return hint;
        }

        internal static string GetRandomItemHint(ItemLocationMap itemLocations, Level level)
        {
            //needs fancying up to look good, but it works
            List<ItemLocation> locations = (List<ItemLocation>)itemLocations.ToList(typeof(ItemLocation));
            ItemLocation hintLocation = locations.PopRandom(new Random(GetRandomSeedForRoom(level)));
            string hint = string.Format("{0} holds {1}.", hintLocation.Name, hintLocation.ItemInfo.Identifier.ToString());
            return hint;
        }

        internal static string GetRequiredItemHint(ItemLocationMap itemLocations, Level level)
        {
            List<ItemLocation> locations = (List<ItemLocation>)itemLocations.ToList(typeof(ItemLocation));
            var explicitlyRequiredItemLocations = itemLocations.Where(l => l.ItemInfo.IsExplicitlyRequired);
            var randomRequiredItemLocation = explicitlyRequiredItemLocations.ToList().PopRandom(new Random(GetRandomSeedForRoom(level)));
            string hint = string.Format("The path to the Ancient Pyramid goes through {0}, {1}", randomRequiredItemLocation.AreaName, randomRequiredItemLocation.Name);
            return hint;
        }

        internal static string GetRequiredProgressionHint(ItemLocationMap itemLocations, Level level)
        {
            int randomSeed = GetRandomSeedForRoom(level);
            var chain = itemLocations.GetProgressionChain();
            var spheres = new List<ProgressionChain>();
            var buriedRequiredItemLocations = itemLocations.Where(l => l.ItemInfo.IsExplicitlyRequired && l.Gate.Gates != null);
            if (buriedRequiredItemLocations.Count() == 0) return "The path to the Ancient Pyramid is straight and narrow.";
            var randomRequiredItemLocation = buriedRequiredItemLocations.ToList().PopRandom(new Random(randomSeed));
            var finalGates = randomRequiredItemLocation.Gate.GetRequirementGates();

            while (chain != null)
            {
                spheres.Add(chain);
                chain = chain.Sub;
            }
            spheres.Reverse();
            List<string> gatesFound = new List<string>();
            foreach (var sphere in spheres)
            {
                foreach (var location in sphere.Locations)
                {
                    foreach (var individualUnlock in location.ItemInfo.Unlocks.Split())
                    {
                        var requirementCheck = new RequirementGate(individualUnlock);
                        if (finalGates.Contains(requirementCheck))
                        {
                            gatesFound.Add(string.Format("{0}, {1}", location.AreaName, location.Name));
                        }
                    }
                }

            }
            if (gatesFound.Count > 0)
            {
                return string.Format("One path to the Ancient Pyramid goes through {0}", gatesFound.PopRandom(new Random(randomSeed)));
            }
            else
            {
                return "The path to the Ancient Pyramid is fraught with impossibility.";
            }

        }
    }
}
