using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Extensions;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.YorneNPC")]
	// ReSharper disable once UnusedMember.Global
	class YorneNpc : LevelObject
    {
        public YorneNpc(Mobile typedObject) : base(typedObject)
        {
        }


        protected override void Initialize(SeedOptions options)
        {
        }
    }
}
