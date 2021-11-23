using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Extensions;

namespace TsRandomizer.LevelObjects.Other
{
    [TimeSpinnerType("Timespinner.GameObjects.Heroes.LunaisObj")]
    class LunaisObj : LevelObject
    {
        public LunaisObj(Mobile typedObject) : base(typedObject)
        {
        }

        protected override void Initialize(SeedOptions options)
        {
            Level.ReplaceDialogue(options);
        }
    }
}
