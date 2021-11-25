using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;
using static TsRandomizer.Randomisation.Gate;

namespace TsRandomizer.Extensions
{
    partial class LevelExtensions
    {
        internal static Action TextReplacer(Level level, SeedOptions options, ItemLocationMap itemLocations)
        {
            switch (level.RoomKeyString())
            {
                case "16.27":
                    return () =>
                    {
                        int concussions = level.GameSave.GetConcussionCount();

                        string replacement = "What—? I don't *think* I hit my head...";
                        switch (concussions)
                        {
                            case 1:
                                replacement = "What—? I feel like I've suffered a concussion...";
                                break;
                            case int c when (c > 1):
                                replacement =
                                    string.Format("What—? I feel like I've suffered {0} concussions...",
                                    concussions);
                                break;
                        }
                        TimeSpinnerGame.Localizer.OverrideKey("cs_tem_1_lun_01", replacement);
                    };
                case "2.51":
                    return () =>
                    {
                        if (options.GyreArchives)
                        {
                            TimeSpinnerGame.Localizer.OverrideKey("cs_pro_lun_02",
                            "Yorne? Oh... not quite. Is this... his memory?");
                            TimeSpinnerGame.Localizer.OverrideKey("cs_pro_yor_03",
                                "I still can't believe they picked *her*, I deserved this.");
                            TimeSpinnerGame.Localizer.OverrideKey("cs_pro_lun_04",
                                "*sigh* Even as just a reflection... Yorne is still as Yorne as ever.");
                            TimeSpinnerGame.Localizer.OverrideKey("cs_pro_yor_05",
                                "I have to find where that Kobo has run off to.");
                            TimeSpinnerGame.Localizer.OverrideKey("cs_pro_lun_06",
                                "'Kobo'... There's no one in our village by that name... I don't think this 'memory' is real.");
                        }
                    };
                case "11.4":
                    return () =>
                    {
                        if (options.GyreArchives)
                            TimeSpinnerGame.Localizer.OverrideKey("q_ram_4_lun_29alt",
                                "It says, 'Redacted Temporal Research: Lord of Ravens'. Maybe I should ask the crow about this...");
                    };
                case "3.16":
                    return () =>
                    {
                        TimeSpinnerGame.Localizer.OverrideKey("sign_forest_directions",
                            GetRequiredProgressionHint(itemLocations, level.ID, level.RoomID));
                    };
                default: return () => { };
            }
        }

        internal static string GetProgressionHint(ItemLocationMap itemLocations, int levelId, int roomId)
        {
            List<ItemLocation> locations = (List<ItemLocation>)itemLocations.ToList(typeof(ItemLocation));
            List<ItemLocation> requiredLocations = locations.FindAll(l => l.ItemInfo.IsProgression);
            ItemLocation requiredLocation = requiredLocations.PopRandom(new Random(levelId * 100 + roomId)); //same hint every time per location
            string hint = string.Format("{0} {1} holds something useful.", requiredLocation.AreaName, requiredLocation.Name);
            return hint;
        }

        internal static string GetRandomItemHint(ItemLocationMap itemLocations, int levelId, int roomId)
        {
            //needs fancying up to look good, but it works
            List<ItemLocation> locations = (List<ItemLocation>)itemLocations.ToList(typeof(ItemLocation));
            ItemLocation hintLocation = locations.PopRandom(new Random(levelId * 100 + roomId));
            string hint = string.Format("{0} holds {1}.", hintLocation.Name, hintLocation.ItemInfo.Identifier.ToString());
            return hint;
        }

        internal static string GetRequiredItemHint(ItemLocationMap itemLocations, int levelId, int roomId)
        {
            List<ItemLocation> locations = (List<ItemLocation>)itemLocations.ToList(typeof(ItemLocation));
            var explicitlyRequiredItemLocations = itemLocations.Where(l => l.ItemInfo.IsExplicitlyRequired);
            var randomRequiredItemLocation = explicitlyRequiredItemLocations.ToList().PopRandom(new Random(levelId * 100 + roomId));
            string hint = string.Format("The path to the Ancient Pyramid goes through {0} {1}", randomRequiredItemLocation.Name, randomRequiredItemLocation.AreaName);
            return hint;
        }

        internal static string GetRequiredProgressionHint(ItemLocationMap itemLocations, int levelId, int roomId)
        {
            int randomSeed = levelId * 100 + roomId;
            var chain = itemLocations.GetProgressionChain();
            var spheres = new List<ProgressionChain>();
            var explicitlyRequiredItemLocations = itemLocations.Where(l => l.ItemInfo.IsExplicitlyRequired);
            var randomRequiredItemLocation = explicitlyRequiredItemLocations.ToList().PopRandom(new Random(randomSeed));
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
                    foreach(var individualUnlock in location.ItemInfo.Unlocks.Split())
                    {
                        var requirementCheck = new RequirementGate(individualUnlock);
                        if(finalGates.Contains(requirementCheck)) {
                            gatesFound.Add(location.AreaName + " " + location.Name);
                        }
                    }
                }
                
            }
            return string.Format("One path to the Ancient Pyramid goes through {0}", gatesFound.PopRandom(new Random(randomSeed)));
        }
    }
}
